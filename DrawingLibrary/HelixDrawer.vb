Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports Common
Imports HelixToolkit.Wpf
Imports WpfResources


Public Class HelixDrawer

#Region " Constants "

    ''' <summary>
    ''' Camera angle above XY plane [degrees].
    ''' </summary>
    Private Const InitAngleXy As Double = 30


    ''' <summary>
    ''' Camera angle above XZ plane [degrees].
    ''' </summary>
    Private Const InitAngleXz As Double = 30

#End Region


#Region " Generate camera model "

    Private Shared Sub SetupViewport(enableRotation As Boolean, view As HelixViewport3D)
        ' Ready?
        If view Is Nothing Then Return

        view.IsManipulationEnabled = enableRotation
        view.ShowCoordinateSystem = enableRotation
        view.IsRotationEnabled = enableRotation
    End Sub


    Private Shared Function LookAtCenter(fromP As Media3D.Point3D) As Media3D.Vector3D
        Return New Media3D.Vector3D(-fromP.X, -fromP.Y, -fromP.Z)
    End Function


    ''' <summary>
    ''' Find the new position for the camera.
    ''' </summary>
    Public Shared Function GenerateCamera(view As HelixViewport3D, room As Room3D, projection As Projections) As ProjectionCamera
        ' Ready?
        If view Is Nothing Then Return Nothing
        If room Is Nothing Then Return Nothing

        Dim cam As ProjectionCamera
        Dim upDir As New Media3D.Vector3D(0, 0, 1)
        Dim c As New RoomDimensions(room)
        Dim vertViewFactor = view.ActualWidth / view.ActualHeight
        Const boxViewFactor = 1.1

        Select Case projection
            Case Projections.XY
                Dim p As New Media3D.Point3D(c.RoomXCenter, c.RoomYCenter, c.RoomZPos + 1)
                upDir = New Media3D.Vector3D(0, 1, 0) ' Shall not match the look direction
                cam = New OrthographicCamera(
                    p, New Media3D.Vector3D(0, 0, -1),
                    upDir, Math.Max(c.RoomXSize, c.RoomYSize * vertViewFactor) * boxViewFactor)
                SetupViewport(False, view)

            Case Projections.XZ
                Dim p As New Media3D.Point3D(c.RoomXCenter, c.RoomXNeg - 1, c.RoomZCenter)
                cam = New OrthographicCamera(
                    p, New Media3D.Vector3D(0, 1, 0),
                    upDir, Math.Max(c.RoomXSize, c.RoomZSize * vertViewFactor) * boxViewFactor)
                SetupViewport(False, view)

            Case Projections.ZY
                Dim p As New Media3D.Point3D(c.RoomXNeg - 1, c.RoomYCenter, c.RoomZCenter)
                cam = New OrthographicCamera(
                    p, New Media3D.Vector3D(1, 0, 0),
                    upDir, Math.Max(c.RoomYSize * vertViewFactor, c.RoomZSize) * boxViewFactor)
                SetupViewport(False, view)

            Case Projections.ThreeD
                Dim sphSize = Math.Sqrt(c.RoomXSize * c.RoomXSize + c.RoomYSize * c.RoomYSize + c.RoomZSize * c.RoomZSize)
                Dim camDist = sphSize / Math.Tan(Math.PI * 45 / 180 / 2)
                Dim xy = InitAngleXy * Math.PI / 180
                Dim xz = InitAngleXz * Math.PI / 180
                Dim camZ = camDist * Math.Sin(xy)
                Dim camY = -camDist * Math.Cos(xz) * Math.Cos(xy)
                Dim camX = -camDist * Math.Sin(xz) * Math.Cos(xy)

                Dim p As New Media3D.Point3D(camX, camY, camZ)
                cam = New PerspectiveCamera(p, LookAtCenter(p), upDir, 45)
                SetupViewport(True, view)

            Case Else
                Throw New ArgumentException("Unknown projection type " & projection)
        End Select

        Return cam
    End Function

#End Region

End Class
