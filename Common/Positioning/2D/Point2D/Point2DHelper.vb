<CLSCompliant(True)>
Public Class Point2DHelper

#Region " Equality comparer "

    Private NotInheritable Class Point2DEqualityComparer
        Implements IEqualityComparer(Of IPoint2D)

        Public Shadows Function Equals(x As IPoint2D, y As IPoint2D) As Boolean Implements IEqualityComparer(Of IPoint2D).Equals
            Return IsSame(x, y)
        End Function


        ''' <summary>
        ''' Get object's hash code.
        ''' </summary>
        ''' <remarks>
        ''' Different hash codes mean different objects,
        ''' so the actual equality method would not be even called.
        ''' </remarks>
        Public Shadows Function GetHashCode(obj As IPoint2D) As Integer Implements IEqualityComparer(Of IPoint2D).GetHashCode
            Return obj.X.GetHashCode() Xor obj.Y.GetHashCode()
        End Function

    End Class


    Public Shared ReadOnly Property EqualityComparer As IEqualityComparer(Of IPoint2D) =
        New Point2DEqualityComparer()

#End Region


#Region " Factory API "

    Public Shared Function Create(x As Double, y As Double) As IPoint2D
        Return New Point2D(Of NoRef)(x, y, NoRef.Empty)
    End Function


    Public Shared Function Create(Of TRef)(copyFrom As IPoint2D) As Point2D(Of TRef)
        Return New Point2D(Of TRef)(copyFrom, Enumerable.Empty(Of TRef)())
    End Function


    Public Shared Function Create(Of TRef)(
        x As Double, y As Double, references As IEnumerable(Of TRef)
    ) As Point2D(Of TRef)

        Return New Point2D(Of TRef)(x, y, references)
    End Function


    Public Shared Function Create(Of TRef)(copyFrom As IPoint2D, references As IEnumerable(Of TRef)) As Point2D(Of TRef)
        Return New Point2D(Of TRef)(copyFrom, references)
    End Function

#End Region


#Region " Geometry API "

    ''' <summary>
    ''' Get distance between two 2D points.
    ''' </summary>
    Public Shared Function Distance(a As IPoint2D, b As IPoint2D) As Double
        Dim dx = b.X - a.X
        Dim dy = b.Y - a.Y

        Return Math.Sqrt(dx * dx + dy * dy)
    End Function


    ''' <summary>
    ''' Compare two points to be exactly the same.
    ''' A missing point (Nothing) always compares False.
    ''' </summary>
    Public Shared Function IsSame(a As IPoint2D, b As IPoint2D) As Boolean
        If a Is Nothing Or b Is Nothing Then Return False

        Return IsEqual(Distance(a, b), 0)
    End Function


    ''' <summary>
    ''' Find a point in the middle of the given points.
    ''' </summary>
    Public Shared Function Average(points As IEnumerable(Of IPoint2D)) As IPoint2D
        Dim pt = points.Where(Function(p) p IsNot Nothing).ToList()
        Dim n = pt.Count

        If n = 0 Then Return Point2D(Of NoRef).Origin

        Return Create(
            pt.Sum(Function(p) p.X) / n,
            pt.Sum(Function(p) p.Y) / n)
    End Function

#End Region


#Region " Management API "

    ''' <summary>
    ''' Combine points with the same coordinates into one point,
    ''' joining their references together.
    ''' </summary>
    ''' <param name="points">Original points collection</param>
    Public Shared Function MergePoints(Of TRef)(
        points As IEnumerable(Of Point2D(Of TRef))
    ) As IList(Of Point2D(Of TRef))

        Dim res As New List(Of Point2D(Of TRef))()

        For Each pt In points
            Dim existing = res.FirstOrDefault(Function(p) IsSame(p, pt))

            If existing Is Nothing Then
                existing = Create(pt, pt.References)
                res.Add(existing)
            Else
                existing.AddExtraReferences(pt.References)
            End If
        Next

        Return res
    End Function

#End Region

End Class
