''' <summary>
''' Stores a single polyhedron and checks, whether the given point is inside it.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class Polyhedron(Of TRef)
    Implements IPolyhedron, IRefKeeper(Of TRef)

#Region " IRefKeeper.References property "

    Private ReadOnly mReferences As New List(Of TRef)()


    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " IPolyhedron.Sides property "

    Private ReadOnly mSides As New List(Of PolyhedronSide(Of TRef))()


    ''' <summary>
    ''' A list of sides, which build up the polyhedron - plane and "inside" direction.
    ''' </summary>
    ReadOnly Property Sides As IReadOnlyCollection(Of IPolyhedronSide) Implements IPolyhedron.Sides
        Get
            Return mSides
        End Get
    End Property

#End Region


#Region " IPolyhedron.Vertices property "

    Private ReadOnly mVertices As New List(Of IPoint3D)()


    ''' <summary>
    ''' A list of sides, which build up the polyhedron - plane and "inside" direction.
    ''' </summary>
    ReadOnly Property Vertices As IReadOnlyCollection(Of IPoint3D) Implements IPolyhedron.Vertices
        Get
            Return mVertices
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create an infinite polyhedron that takes up the whole space.
    ''' </summary>
    Public Sub New(referenceList As IEnumerable(Of TRef))
        mReferences.AddRange(referenceList)
    End Sub


    ''' <summary>
    ''' Create a polyhedron from the set of planes.
    ''' </summary>
    ''' <param name="sides">A list of 3D planes and the signs "inside"</param>
    Public Sub New(sides As IEnumerable(Of PolyhedronSide(Of TRef)), referenceList As IEnumerable(Of TRef))
        mSides.AddRange(sides)
        mReferences.AddRange(referenceList)
        mVertices.AddRange(
            mSides.SelectMany(Function(s) s.Polygon.Vertices).Distinct(Point3DHelper.EqualityComparer))
    End Sub

#End Region


#Region " IObject3D implementation "

    ''' <inheritdoc/>
    Public Function Contains(p As IPoint3D) As Boolean Implements IObject3D.Contains
        If Not mSides.Any() Then
            ' Infinite polyhedron, contains all points
            Return True
        End If

        ' Check "inside" by comparing, that "p" is on the right side of every plane
        For Each pl In mSides
            Select Case Sign(pl.Polygon.Plane.GetDistanceToPoint(p))
                Case 1
                    If pl.Inside < 0 Then Return False
                Case -1
                    If pl.Inside > 0 Then Return False
            End Select
        Next

        Return True
    End Function

#End Region

End Class
