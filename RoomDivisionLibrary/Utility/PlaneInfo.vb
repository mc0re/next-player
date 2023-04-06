Imports System.Diagnostics.CodeAnalysis
Imports Common


Public Class PlaneInfo(Of TRef)

#Region " Properties "

    ''' <summary>
    ''' The original points.
    ''' </summary>
    Public Property Points As IList(Of IPoint3D)


    ''' <summary>
    ''' Projections in the same order as <see cref="Points"/>.
    ''' </summary>
    Public Property Projections As IList(Of ILineSegment3D)


    ''' <summary>
    ''' The plane joining the face's points.
    ''' </summary>
    Public Property PointsPlane As IPlane3D


	''' <summary>
	''' Which direction the face faces.
	''' </summary>
	Public Property Direction As FaceDirections

#End Region


#Region " ToString "

	<ExcludeFromCodeCoverage>
	Public Overrides Function ToString() As String
		Return String.Format("{0}: {1}", Direction, String.Join(", ", Points))
	End Function

#End Region

End Class
