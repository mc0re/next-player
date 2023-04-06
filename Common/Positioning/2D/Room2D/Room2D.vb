<CLSCompliant(True)>
Public Class Room2D

#Region " Properties "

    Public Property Borders As Rectangle2D


    Public Property Audience As Rectangle2D

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a symmetrical room around (0, 0).
    ''' </summary>
    ''' <param name="width"></param>
    ''' <param name="height"></param>
    ''' <param name="audWidth"></param>
    ''' <param name="audHeight"></param>
    Public Sub New(width As Double, height As Double, audWidth As Double, audHeight As Double)
        Borders = New Rectangle2D(width, height)
        Audience = New Rectangle2D(audWidth, audHeight)
    End Sub


    ''' <summary>
    ''' Create a room.
    ''' </summary>
    ''' <param name="roomLeft">Distance from 0 to the left room boundary (non-negative)</param>
    ''' <param name="roomRight">Distance from 0 to the right room boundary (non-negative)</param>
    ''' <param name="roomBack">Distance from 0 to the back room boundary (non-negative)</param>
    ''' <param name="roomFront">Distance from 0 to the front room boundary (non-negative)</param>
    ''' <param name="audLeft">Distance from 0 to the left audience boundary (non-negative)</param>
    ''' <param name="audRight">Distance from 0 to the right audience boundary (non-negative)</param>
    ''' <param name="audBack">Distance from 0 to the back audience boundary (non-negative)</param>
    ''' <param name="audFront">Distance from 0 to the front audience boundary (non-negative)</param>
    Public Sub New(roomLeft As Double, roomRight As Double, roomBack As Double, roomFront As Double,
                   audLeft As Double, audRight As Double, audBack As Double, audFront As Double)
        Borders = New Rectangle2D(-roomLeft, roomRight, -roomBack, roomFront)
        Audience = New Rectangle2D(-audLeft, audRight, -audBack, audFront)
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Cut the given polygon in place by the room sides.
    ''' </summary>
    ''' <returns>False, if it was eliminated</returns>
    Public Function CutByRoom(Of TRef)(poly As Polygon2D(Of TRef)) As Boolean
        For Each side In Borders.Sides
            If Not poly.CutByLine(side) Then
                Return False
            End If
        Next

        Return True
    End Function


    ''' <summary>
    ''' Project the given point to the audience.
    ''' </summary>
    Public Function ProjectToAudience(c As IPoint2D) As IPoint2D
        Dim projX = c.X, projY = c.Y

        If Sign(projX - Audience.Left) < 0 Then
            projX = Audience.Left
        ElseIf Sign(projX - Audience.Right) > 0 Then
            projX = Audience.Right
        End If

        If Sign(projY - Audience.Back) < 0 Then
            projY = Audience.Back
        ElseIf Sign(projY - Audience.Front) > 0 Then
            projY = Audience.Front
        End If

        Return Point2DHelper.Create(projX, projY)
    End Function

#End Region

End Class
