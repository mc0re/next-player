
''' <summary>
''' Convenience class for generic object creation.
''' </summary>
Public NotInheritable Class PolyhedronSideHelper

#Region " Factory method "

    ''' <summary>
    ''' Create an infinite side from the given plane and inside direction.
    ''' </summary>
    Public Shared Function CreateNoRef(Of TRef)(
        plane As IPlane3D, inside As Integer
    ) As PolyhedronSide(Of TRef)

        Return New PolyhedronSide(Of TRef)(plane, inside, Enumerable.Empty(Of TRef))
    End Function


    ''' <summary>
    ''' Create an infinite side from the given plane and inside direction.
    ''' Use plane's reference list.
    ''' </summary>
    Public Shared Function Create(Of TRef)(
        plane As Plane3D(Of TRef), inside As Integer
    ) As PolyhedronSide(Of TRef)

        Return New PolyhedronSide(Of TRef)(plane, inside, plane.References)
    End Function


    ''' <summary>
    ''' Create an infinite side from the given plane and inside direction.
    ''' Use the supplied reference list.
    ''' </summary>
    Public Shared Function Create(Of TRef)(
        plane As IPlane3D, inside As Integer, refList As IEnumerable(Of TRef)
    ) As PolyhedronSide(Of TRef)

        Return New PolyhedronSide(Of TRef)(plane, inside, refList)
    End Function


    ''' <summary>
    ''' Create a side from the given polygon and inside direction.
    ''' </summary>
    Public Shared Function Create(Of TRef)(
        poly As Polygon3D(Of TRef), inside As Integer
    ) As PolyhedronSide(Of TRef)

        Return New PolyhedronSide(Of TRef)(poly, inside, poly.References)
    End Function

#End Region

End Class
