Imports System.Collections.Concurrent
Imports Common
Imports RoomDivisionLibrary


Public Class PositionCoefficientGenerator
    Implements ICoefficientGenerator

#Region " Constants "

    Private Const MinSourceDistance As Single = 0.2F

    Private Const MinSpeakerDistance As Single = 0.5F

    Private Shared ReadOnly sEmptyCollection As New ChannelModifierCollection()

#End Region


#Region " Fields "

    Private ReadOnly mModifiers As New ConcurrentDictionary(Of Integer, ChannelModifierCollection)()

#End Region


#Region " Init and clean-up "

    Public Sub New(
        positionControl As IPositionRelative,
        volumeControl As ISimpleVolume,
        layouter As I3DLayouter(Of AudioPhysicalChannel),
        info As SourcePanning3DCollection
    )
        If volumeControl.IsMuted Then Return

        Dim source = layouter.Room.ConvertRelative(positionControl)
        Dim channels = layouter.GetReferences(source)

        If Not channels.Any() Then Return

        ' The reflections should be calculated and added here as well
        AddPoint(source, layouter, info, volumeControl.Volume, channels)
    End Sub

#End Region


#Region " ICoefficientGenerator implementation "

    Public Function Generate(phChNr As Integer) As ChannelModifierCollection Implements ICoefficientGenerator.Generate
        Dim mc As ChannelModifierCollection = Nothing

        If mModifiers.TryGetValue(phChNr, mc) Then
            Return mc
        End If

        Return sEmptyCollection
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Add a source point to all provided physical channels.
    ''' This adds data to <see cref="mModifiers"/> collection.
    ''' </summary>
    ''' <param name="source">Location of the sound source</param>
    ''' <param name="volume">Source volume (0..1)</param>
    ''' <remarks>
    ''' Given:
    ''' - point P <paramref name="source"/>
    ''' - original volume = V <paramref name="volume"/>
    ''' 
    ''' Calculate:
    ''' - projection of P onto the audience = A
    ''' - distance |PA| = d
    ''' - the distances from the channels to A = d[i]
    '''   (to calculate each channel's delay and volume reduction),
    ''' - the distances from the channels to P as seen from A = p[i]
    '''   (to determine relative volumes)
    ''' 
    ''' The volume levels for the channels V[i] must be such that:
    ''' - If the source point is colocated with a channel (but not listener), V[i] = V
    ''' - SUM(V[i]/d[i]^2) = V/d^2
    ''' - V[i]/d[i]^2 ~ 1/p[i]
    ''' 
    ''' Delays are proportional to (max - d[i]).
    ''' 
    ''' Then the parameters are adjusted by the channels' volumes and delayes.
    ''' </remarks>
    Private Sub AddPoint(
        source As IPoint3D,
        layouter As I3DLayouter(Of AudioPhysicalChannel),
        info As SourcePanning3DCollection,
        volume As Single,
        channels As IReadOnlyCollection(Of AudioPhysicalChannel)
    )
        ' Find the projection A
        Dim sourceProj = layouter.Room.ProjectToAudience(source)
        Dim sourceToProj = Vector3D.CreateA2B(source, sourceProj)
        Dim distToListener = sourceToProj.Length

        sourceToProj = AdjustProjectionVector(sourceToProj, channels)

        Dim dist = CalculateDistances(sourceProj, sourceToProj, channels)
        Dim angleCoef = CalculateAngleCoefficients(dist.ChToSourceProj)
        Dim volCoef = AdjustByDistance(angleCoef, dist.ChToListener, distToListener)
        Dim delays = CalculateDelays(dist.ChToListener, distToListener)

        Dim useIdx = 0
        For chIdx = 0 To channels.Count - 1
            If Not dist.UseChannel(chIdx) Then Continue For

            Dim ch = channels(chIdx)
            Dim chInfo As SourcePanningInfo(Of IPoint3D) = info.GetInfo(ch.Channel)

            If chInfo.IsEnabled Then
                Dim modList = mModifiers.GetOrAdd(ch.Channel, Function() New ChannelModifierCollection())
                modList.Add(CreateChannelCoef(volCoef(useIdx), delays(useIdx), volume, chInfo))
            End If

            useIdx += 1
        Next
    End Sub


    ''' <summary>
    ''' If the vector has length of 0, adjust it.
    ''' </summary>
    Private Function AdjustProjectionVector(sourceToProj As Vector3D, channels As IReadOnlyCollection(Of AudioPhysicalChannel)) As Vector3D
        If Not sourceToProj.IsZero Then Return sourceToProj

        ' If the source is located on the audience plane,
        ' try pointing to the center of the channels.
        Dim centerX = channels.Average(Function(ch) ch.X)
        Dim centerY = channels.Average(Function(ch) ch.Y)
        Dim centerZ = channels.Average(Function(ch) ch.Z)
        Dim centerToAud = Vector3D.CreateA2B(Point3DHelper.Create(centerX, centerY, centerZ), Point3DHelper.Origin)
        If Not centerToAud.IsZero Then Return centerToAud

        ' If the center is on the audience plane, assume the sound comes brom above
        Return Vector3D.AlongZ.Negate
    End Function


    ''' <summary>
    ''' Calculate the distances between the source (projection) point and
    ''' the channels.
    ''' </summary>
    ''' <param name="sourceProj">Projection of audio source onto audience</param>
    ''' <param name="sourceToProj">Vector from source to its projection</param>
    Private Shared Function CalculateDistances(
        sourceProj As IPoint3D,
        sourceToProj As Vector3D,
        channels As IReadOnlyCollection(Of AudioPhysicalChannel)
    ) As Distances

        Dim use = New Boolean(channels.Count - 1) {}
        Dim distList = New List(Of Single)()
        Dim projList = New List(Of Single)()

        ' Calculate distances d[i] towards A and their projections p[i]
        For chIdx = 0 To channels.Count - 1
            Dim ch = channels(chIdx)
            Dim chToProj = Vector3D.CreateA2B(ch, sourceProj)

            ' Only use if they are on the same side of the source projection
            If Sign(sourceToProj.DotProduct(chToProj)) >= 0 Then
                use(chIdx) = True
                distList.Add(chToProj.Length)
                projList.Add(chToProj.Rejection(sourceToProj).Length)
            End If
        Next

        Return New Distances With {
            .UseChannel = use,
            .ChToListener = distList.ToArray(),
            .ChToSourceProj = projList.ToArray()
        }
    End Function


    ''' <summary>
    ''' Based on the projected distances, calculate the volume coefficients
    ''' as if the channels were all on the same distance from the listener.
    ''' </summary>
    ''' <param name="projDist">Projected distances</param>
    ''' <returns>Array of the coefficients (same size as <paramref name="projDist"/>)</returns>
    ''' <remarks>
    ''' If any of the distances are 0, divide 1 evenly between them and set the others to 0.
    ''' Otherwise, ac[i] = SUM(p[i]) / p[i], normalized so SUM(ac[i]) = 1.
    ''' </remarks>
    Private Function CalculateAngleCoefficients(projDist() As Single) As Single()
        Dim nofZeros = projDist.Count(Function(p) IsEqual(p, 0))

        If nofZeros > 0 Then
            Return projDist.Select(Function(p) If(IsEqual(p, 0), 1.0F / nofZeros, 0)).ToArray()
        End If

        Dim sum = projDist.Sum()
        Dim ac = projDist.Select(Function(p) sum / p).ToArray()
        Dim acSum = ac.Sum()

        Return ac.Select(Function(a) a / acSum).ToArray()
    End Function


    ''' <summary>
    ''' Adjust the calculated angle-determined coefficients by distances from the channels.
    ''' </summary>
    ''' <param name="angleCoef">Angle coefficients per channel (SUM = 1)</param>
    ''' <param name="dist">Distances from each channel to the listener</param>
    ''' <param name="distToListener">DIstance source-to-listener</param>
    ''' <returns>An array of volume coefficients vc</returns>
    ''' <remarks>
    ''' V[i] = V * vc[i]
    ''' </remarks>
    Private Function AdjustByDistance(angleCoef() As Single, dist() As Single, distToListener As Single) As Single()
        Dim var2 = Math.Max(distToListener, MinSourceDistance)
        var2 *= var2
        Dim vc = New Single(dist.Length - 1) {}

        For idx = 0 To dist.Length - 1
            Dim di = Math.Max(dist(idx), MinSpeakerDistance)
            di *= di
            vc(idx) = angleCoef(idx) * di / var2
        Next

        Return vc
    End Function


    ''' <summary>
    ''' Get sound delay from the distances.
    ''' </summary>
    ''' <remarks>
    ''' The delays cannot be negative, so the base is the farthest point,
    ''' even though it's a source.
    ''' </remarks>
    Private Function CalculateDelays(dist() As Single, distToListener As Single) As Single()
        Dim delays = New Single(dist.Length - 1) {}

        If dist.Length = 0 Then
            Return delays
        End If

        Dim maxDist = Math.Max(dist.Max(), distToListener)

        For idx = 0 To dist.Length - 1
            delays(idx) = (maxDist - dist(idx)) / SpeedOfSound
        Next

        Return delays
    End Function


    ''' <summary>
    ''' Create a ChannelModifier from the given information.
    ''' </summary>
    ''' <param name="vc">Volume coefficient</param>
    ''' <param name="delay">Delay</param>
    ''' <param name="srcVolume">Source volume</param>
    ''' <remarks>The coefficients can be more than 1</remarks>
    Private Function CreateChannelCoef(
        vc As Single, delay As Single, srcVolume As Single, info As SourcePanningInfo(Of IPoint3D)
    ) As ChannelModifier

        Dim coefList = New Single(info.SourceToMono.Length - 1) {}

        If info.IsEnabled Then
            Dim baseVolume = srcVolume * vc

            For chIdx = 0 To info.SourceToMono.Length - 1
                If info.EnabledMap.IsEnabled(chIdx) Then
                    coefList(chIdx) = baseVolume * info.SourceToMono(chIdx)
                End If
            Next
        End If

        Return New ChannelModifier(delay + info.Delay, coefList)
    End Function

#End Region

End Class
