Imports System.Xml.Serialization
Imports AudioPlayerLibrary
Imports Common


''' <summary>
''' An audio effect, regulating panning over time.
''' </summary>
<Serializable>
Public Class PlayerActionPanningAutomation
    Inherits PlayerActionAutomation

#Region " AutomationPointList notifying property "

    Private WithEvents mAutomationPointList As New AutomationPointCollection()


    ''' <summary>
    ''' A list of points to use for the effect.
    ''' </summary>
    ''' <remarks>
    ''' To allow serialization, the original list must be returned,
    ''' as Add method is called on it.
    ''' </remarks>
    Public Property AutomationPointList As AutomationPointCollection
        Get
            Return mAutomationPointList
        End Get
        Set(value As AutomationPointCollection)
            mAutomationPointList.Clear()

            For Each pt In value
                mAutomationPointList.Add(pt)
            Next

            RaisePropertyChanged(Function() AutomationPointList)
        End Set
    End Property


    Private Sub AutomationPointListChangedHandler() Handles mAutomationPointList.ListChanged
        If Not IsPlaying Then SetInitialValue()
        RaisePropertyChanged(Function() AutomationPointList)
    End Sub

#End Region


#Region " EffectiveBalance non-persistent read-only notifying property "

    Private mEffectiveBalance As Double


    ''' <summary>
    ''' The effective value.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public Property EffectiveBalance As Double
        Get
            Return mEffectiveBalance
        End Get
        Private Set(value As Double)
            If EffectiveBalance = value Then Return

            mEffectiveBalance = value
            RaisePropertyChanged(Function() EffectiveBalance)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Used for deserialization.
    ''' </summary>
    Public Sub New()
        SetDefaults(False, GeneratedVolumeAutomationTypes.FadeIn, Nothing)
    End Sub


    ''' <summary>
    ''' Used for inserting in code, adds two fade-out points.
    ''' </summary>
    Public Sub New(addPoints As Boolean, genType As GeneratedVolumeAutomationTypes, Optional dur As TimeSpan? = Nothing)
        If Not dur.HasValue Then
            Dim conf = InterfaceMapper.GetImplementation(Of IEffectDurationConfiguration)()
            dur = conf.DefaultDuration
        End If

        ExecutionType = ExecutionTypes.MainContinuePrev
        SetDefaults(addPoints, genType, dur.Value)
    End Sub


    ''' <summary>
    ''' Set default values.
    ''' </summary>
    Private Sub SetDefaults(addPoints As Boolean, genType As GeneratedVolumeAutomationTypes, dur As TimeSpan)
        Name = "Fade out"

        If addPoints Then
            If genType = GeneratedVolumeAutomationTypes.FadeOut Then
                Name = "Auto fade out"
                mAutomationPointList.Add(New AutomationPoint(0, 1))
                mAutomationPointList.Add(New AutomationPoint(dur.TotalMilliseconds, 0))
                HasDuration = True
            Else
                Name = "Auto fade in"
                mAutomationPointList.Add(New AutomationPoint(0, 0))
                mAutomationPointList.Add(New AutomationPoint(dur.TotalMilliseconds, 1))
            End If

            Duration = dur
        End If

        SetInitialValue()
    End Sub

#End Region


#Region " PlayerAction overrides "

    Public Overrides Sub ModifyVolume(delta As Single)
        If OperationType <> EffectOperationTypes.Assign Then Return

        For Each pt In AutomationPointList
            pt.Y = CalculateVolume(pt.Y, delta)
        Next
    End Sub

#End Region


#Region " Effect functionality "

    ''' <summary>
    ''' Set targets' volume to the interpolated value.
    ''' </summary>
    Protected Overrides Sub ApplyEffect(op As EffectOperationTypes)
        Dim pos = PlayPosition.TotalMilliseconds
        EffectiveBalance = GetAnimationValue(pos, AutomationPointList)

        For Each target In TargetList.Where(Function(t) t.IsPlaying).ToList()
            Select Case op
                Case EffectOperationTypes.Assign
                    target.SetEffectiveVolume(Me, EffectiveBalance)

                Case EffectOperationTypes.Multiply
                    target.SetEffectiveVolume(Me, target.Volume * EffectiveBalance)

                Case EffectOperationTypes.ChainMultiply
                    target.SetEffectiveVolume(Me, target.EffectiveVolume * EffectiveBalance)

                Case EffectOperationTypes.Bypass
                    ' Bypass, keep the original volume
                    target.SetEffectiveVolume(Me, target.Volume)
            End Select
        Next

        If HasDuration AndAlso pos >= DurationSerialized Then
            For Each target In TargetList
                target.SimulateEndReached()
            Next

            [Stop](False)
        End If
    End Sub


    Private Sub SetInitialValue()
        EffectiveBalance = GetAnimationValue(
            PlayPosition.TotalMilliseconds, AutomationPointList)
    End Sub

#End Region

End Class
