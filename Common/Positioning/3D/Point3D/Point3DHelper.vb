

Public Class Point3DHelper

#Region " Constants "

    ''' <summary>
    ''' Coordinate system origin, all coordinates are 0.
    ''' </summary>
    Public Shared ReadOnly Origin As IPoint3D = Create(0, 0, 0)

#End Region


#Region " Factory API "

    Public Shared Function Create(x As Double, y As Double, z As Double) As IPoint3D
        Return New Point3D(Of NoRef)(x, y, z, NoRef.Empty)
    End Function


    Public Shared Function Create(c As IPoint3D) As IPoint3D
        Return New Point3D(Of NoRef)(c.X, c.Y, c.Z, NoRef.Empty)
    End Function


    Public Shared Function Create(Of TRef)(x As Double, y As Double, z As Double, ref As IEnumerable(Of TRef)) As Point3D(Of TRef)
        Return New Point3D(Of TRef)(x, y, z, ref)
    End Function

#End Region


#Region " Geometry API "

    ''' <summary>
    ''' Get the distance between two points.
    ''' Beware of rounding, use <see cref="AbsoluteCoordPrecision"/>.
    ''' </summary>
    Public Shared Function Distance(a As IPoint3D, b As IPoint3D) As Double
        Dim dx = b.X - a.X
        Dim dy = b.Y - a.Y
        Dim dz = b.Z - a.Z

        Dim denom = dx * dx + dy * dy + dz * dz

        Return Math.Sqrt(denom)
    End Function


    ''' <summary>
    ''' Check whether the two points are essentially the same.
    ''' </summary>
    ''' <remarks>
    ''' The check is performed by comparing distance to 0.
    ''' Same code as in <see cref="Line3DHelper"/> constructor.
    ''' </remarks>
    Public Shared Function IsSame(a As IPoint3D, b As IPoint3D) As Boolean
        ' Using Vector3D.CreateA2B(a, b).IsZero is possible,
        ' but leads to unnecessary memory allocation
        Return IsEqual(Distance(a, b), 0)
    End Function


    ''' <summary>
    ''' Find a point in the middle of the given points.
    ''' </summary>
    Public Shared Function Average(ParamArray pt() As IPoint3D) As IPoint3D
        Dim n = pt.Length
        Debug.Assert(n > 0)

        Return Create(
            pt.Sum(Function(p) p.X) / n,
            pt.Sum(Function(p) p.Y) / n,
            pt.Sum(Function(p) p.Z) / n)
    End Function

#End Region


#Region " EqualityComparer "

    Private NotInheritable Class Point3DEqualityComparer
        Implements IEqualityComparer(Of IPoint3D)

        Public Shadows Function Equals(x As IPoint3D, y As IPoint3D) As Boolean Implements IEqualityComparer(Of IPoint3D).Equals
            Return IsSame(x, y)
        End Function


        ''' <summary>
        ''' Get object's hash code.
        ''' </summary>
        ''' <remarks>
        ''' Different hash codes mean different objects,
        ''' so the actual equality method would not be even called.
        ''' </remarks>
        Public Shadows Function GetHashCode(obj As IPoint3D) As Integer Implements IEqualityComparer(Of IPoint3D).GetHashCode
            Dim x = Math.Round(obj.X, PositioningUtility.AbsoluteCoordPrecisionDigits)
            Dim y = Math.Round(obj.Y, PositioningUtility.AbsoluteCoordPrecisionDigits)
            Dim z = Math.Round(obj.Z, PositioningUtility.AbsoluteCoordPrecisionDigits)
            Return x.GetHashCode() Xor y.GetHashCode() Xor z.GetHashCode()
        End Function

    End Class


    Public Shared ReadOnly Property EqualityComparer As IEqualityComparer(Of IPoint3D) =
        New Point3DEqualityComparer()

#End Region

End Class
