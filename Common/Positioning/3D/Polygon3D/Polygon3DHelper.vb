Public Class Polygon3DHelper

    Public Shared Function Create(plane As IPlane3D) As IPolygon3D
        Return New Polygon3D(Of NoRef)(plane)
    End Function


    Public Shared Function Create(plane As IPlane3D, ParamArray vertices() As IPoint3D) As Polygon3D(Of NoRef)
        Return New Polygon3D(Of NoRef)(
            plane,
            From v In vertices Select Point3DHelper.Create(v.X, v.Y, v.Z, NoRef.Empty))
    End Function

End Class
