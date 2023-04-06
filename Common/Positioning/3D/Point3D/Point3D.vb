Imports System.Diagnostics.CodeAnalysis


<CLSCompliant(True)>
<Serializable>
Public Class Point3D(Of TRef)
    Implements IPoint3D, IRefKeeper(Of TRef)

#Region " IPoint3D properties "

    ''' <summary>
    ''' Coordinate along X axis (in physical units).
    ''' </summary>
    Public Property X As Double Implements IPoint3D.X

    ''' <summary>
    ''' Coordinate along Y axis (in physical units).
    ''' </summary>
    Public Property Y As Double Implements IPoint3D.Y

    ''' <summary>
    ''' Coordinate along Z axis (in physical units).
    ''' </summary>
    Public Property Z As Double Implements IPoint3D.Z

#End Region


#Region " IRefKeeper properties "

    Private ReadOnly mReferences As New List(Of TRef)()


    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a point by 3 coordinates.
    ''' </summary>
    Public Sub New(x As Double, y As Double, z As Double, ref As IEnumerable(Of TRef))
        Me.X = x
        Me.Y = y
        Me.Z = z
        mReferences.AddRange(ref)
    End Sub


    ''' <summary>
    ''' Create a point with the same coordinates as <paramref name="c"/>.
    ''' </summary>
    Public Sub New(c As IPoint3D, ref As IEnumerable(Of TRef))
        Me.New(c.X, c.Y, c.Z, ref)
    End Sub

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format("({0:F2}, {1:F2}, {2:F2}) / {3}", X, Y, Z, ReferencesHelper.AsString(References))
    End Function

#End Region

End Class
