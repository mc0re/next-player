Public Class Vector2DHelper

#Region " Angle comparer "

    Private NotInheritable Class AngleComparerImplementation
        Implements IComparer(Of Double)

        Public Function Compare(x As Double, y As Double) As Integer Implements IComparer(Of Double).Compare
            Return CompareAngles(x, y)
        End Function

    End Class


    Public Shared ReadOnly Property AngleComparer As IComparer(Of Double) =
        New AngleComparerImplementation()

#End Region


#Region " Factory API "

    Public Shared Function Create(x As Double, y As Double) As Vector2D
        Return New Vector2D(x, y)
    End Function


    Public Shared Function Create(a As IPoint2D, b As IPoint2D) As Vector2D
        If a Is Nothing OrElse b Is Nothing Then
            Throw New ArgumentException("Cannot create a vector into infinity")
        End If

        Return New Vector2D(b.X - a.X, b.Y - a.Y)
    End Function

#End Region


#Region " API "

    ''' <summary>
    ''' Compare two angles, assuming they differ by no more than 180 degrees.
    ''' </summary>
    ''' <param name="a1">First angle</param>
    ''' <param name="a2">Second angle</param>
    ''' <returns>
    ''' The comparison result is as usual:
    ''' -1 if <paramref name="a1"/> is counter-clockwise ("less", "to the left") from <paramref name="a2"/>
    ''' 0 if they are (almost) identical
    ''' 1 if <paramref name="a1"/> is clockwise ("greater", "to the right") from <paramref name="a2"/>
    ''' </returns>
    Public Shared Function CompareAngles(a1 As Double, a2 As Double) As Integer
        If IsEqual(a1, a2) Then
            Return 0
        ElseIf a1 > a2 AndAlso a1 - a2 < Math.PI Then
            Return 1
        ElseIf a1 + 2 * Math.PI > a2 AndAlso a1 + 2 * Math.PI - a2 < Math.PI Then
            Return 1
        End If

        Return -1
    End Function


    ''' <summary>
    ''' Compare the directions of two vectors.
    ''' Assuming they are parallel, 1 means point the same way, -1 - opposite.
    ''' </summary>
    Friend Shared Function CompareDirections(v1 As Vector2D, v2 As Vector2D) As Integer
        Dim sameWay = Sign(v1.X) = Sign(v2.X) AndAlso Sign(v1.Y) = Sign(v2.Y)

        Return If(sameWay, 1, -1)
    End Function

#End Region

End Class
