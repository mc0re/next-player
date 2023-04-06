Public Class LineSegment3DHelper

    Public Shared Function Create(
        a As IPoint3D, b As IPoint3D
    ) As ILineSegment3D

        If Point3DHelper.IsSame(a, b) Then
            Return Nothing
        End If

        Return New LineSegment3D(Of NoRef)(a, b, NoRef.Empty)
    End Function


    Public Shared Function Create(Of TRef)(
        a As IPoint3D, b As IPoint3D, ref As IEnumerable(Of TRef)
    ) As LineSegment3D(Of TRef)

        If Point3DHelper.IsSame(a, b) Then
            Return Nothing
        End If

        Return New LineSegment3D(Of TRef)(a, b, ref)
    End Function

End Class
