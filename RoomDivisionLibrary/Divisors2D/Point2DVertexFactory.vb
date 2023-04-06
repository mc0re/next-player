Imports Common


Public Class Point2DVertexFactory

#Region " Factory API "

    Public Shared Function Create(Of TRef)(point As Point2D(Of TRef)) As Point2DVertex(Of TRef)

        Return New Point2DVertex(Of TRef)(point)
    End Function

#End Region

End Class
