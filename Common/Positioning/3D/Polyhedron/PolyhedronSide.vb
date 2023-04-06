Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' Represents a single side of a polyhedron.
''' The side is merely a plane and "inside" direction.
''' </summary>
''' <typeparam name="TRef">Type of reference objects carried by the planes</typeparam>
<CLSCompliant(True)>
<Serializable>
Public Class PolyhedronSide(Of TRef)
    Implements IPolyhedronSide, IRefKeeper(Of TRef)

#Region " IPolyhedronSide properties "

    ''' <summary>
    ''' Plane, along which the side is located.
    ''' </summary>
    Public ReadOnly Property Polygon As IPolygon3D Implements IPolyhedronSide.Polygon


    ''' <summary>
    ''' Which way is inside, according to the plane.
    ''' </summary>
    Public ReadOnly Property Inside As Integer Implements IPolyhedronSide.Inside

#End Region


#Region " IRefKeeper properties "

    Private ReadOnly mReferences As New List(Of TRef)()


    ''' <summary>
    ''' Which speaker projections lie on this side.
    ''' </summary>
    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create an infinite side of a polyhedron.
    ''' </summary>
    ''' <param name="plane">Plane, along which the side is located</param>
    ''' <param name="inside">Which way is inside, according to the plane (-1 or 1)</param>
    ''' <param name="refList">A list references belonging to this side</param>
    Public Sub New(plane As IPlane3D, inside As Integer, refList As IEnumerable(Of TRef))
        If inside = 0 Then
            Throw New ArgumentException("Inside direction cannot be 0")
        End If

        Polygon = Polygon3DHelper.Create(plane)
        Me.Inside = inside
        mReferences.AddRange(refList)
    End Sub


    ''' <summary>
    ''' Create a side of a polyhedron.
    ''' </summary>
    ''' <param name="polygon">Polygon describing the side</param>
    ''' <param name="inside">Which way is inside, according to the polygon's plane (-1 or 1)</param>
    ''' <param name="refList">A list references belonging to this side</param>
    Public Sub New(polygon As IPolygon3D, inside As Integer, refList As IEnumerable(Of TRef))
        If inside = 0 Then
            Throw New ArgumentException("Inside direction cannot be 0")
        End If

        Me.Polygon = polygon
        Me.Inside = inside
        mReferences.AddRange(refList)
    End Sub

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format(
            "{0}, sign {1} / {2}", Polygon, Inside, ReferencesHelper.AsString(References))
    End Function

#End Region

End Class
