Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' 3D vector, holds 3 coordinates.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class Vector3D

#Region " Constants "

    ''' <summary>
    ''' Standard unit vector going "right" along X axis.
    ''' </summary>
    Public Shared ReadOnly Property AlongX As Vector3D = New Vector3D(1, 0, 0)


    ''' <summary>
    ''' Standard unit vector going "forward" along Y axis.
    ''' </summary>
    Public Shared ReadOnly Property AlongY As Vector3D = New Vector3D(0, 1, 0)


    ''' <summary>
    ''' Standard unit vector going "up" along Z axis.
    ''' </summary>
    Public Shared ReadOnly Property AlongZ As Vector3D = New Vector3D(0, 0, 1)

#End Region


#Region " Coordinate properties "

    Public ReadOnly Property X As Double

    Public ReadOnly Property Y As Double

    Public ReadOnly Property Z As Double

#End Region


#Region " Length cached read-only property "

    Private mLength As Single?


    ''' <summary>
    ''' Length of the vector.
    ''' </summary>
    Public ReadOnly Property Length As Single
        Get
            If Not mLength.HasValue Then
                mLength = CSng(Math.Sqrt(Square))
            End If

            Return mLength.Value
        End Get
    End Property

#End Region


#Region " IsZero read-only property "

    ''' <summary>
    ''' Whether the length of the vector is 0.
    ''' </summary>
    Public ReadOnly Property IsZero As Boolean
        Get
            Return IsEqual(Length, 0)
        End Get
    End Property

#End Region


#Region " Square read-only property "

    ''' <summary>
    ''' Squared length of the vector, which is a dot product with itself.
    ''' </summary>
    Public ReadOnly Property Square As Double
        Get
            Return X * X + Y * Y + Z * Z
        End Get
    End Property

#End Region


#Region " Unit read-only property "

    Private mUnit As Vector3D


    ''' <summary>
    ''' Get a unit vector in the same direction as this vector.
    ''' </summary>
    ''' <returns>A unit vector (<see cref="Length"/> is 1)</returns>
    Public ReadOnly Property Unit As Vector3D
        Get
            If mUnit Is Nothing Then
                mUnit = Multiply(1 / Length)
            End If

            Return mUnit
        End Get
    End Property

#End Region


#Region " AnyPerpendicular cached read-only property "

    Private mAnyPerpendicular As Vector3D


    ''' <summary>
    ''' Any unit vector perpendicular to this one.
    ''' </summary>
    ''' <returns>A unit vector with <see cref="Length"/> 1</returns>
    Public ReadOnly Property AnyPerpendicular As Vector3D
        Get
            If mAnyPerpendicular Is Nothing Then
                If IsEqual(X, 0) Then
                    ' Not colinear with (1, 0, 0)
                    mAnyPerpendicular = New Vector3D(0, Z, -Y)
                ElseIf IsEqual(Y, 0) Then
                    ' Not colinear with (0, 1, 0)
                    mAnyPerpendicular = New Vector3D(-Z, 0, X)
                Else
                    ' Not colinear with (0, 0, 1)
                    mAnyPerpendicular = New Vector3D(Y, -X, 0)
                End If

                mAnyPerpendicular = mAnyPerpendicular.Unit
            End If

            Return mAnyPerpendicular
        End Get
    End Property

#End Region


#Region " OtherPerpendicular cached read-only property "

    Private mOtherPerpendicular As Vector3D


    ''' <summary>
    ''' A unit vector perpendicular to this one and to <see cref="AnyPerpendicular"/>.
    ''' </summary>
    ''' <returns>A unit vector with <see cref="Length"/> 1</returns>
    Public ReadOnly Property OtherPerpendicular As Vector3D
        Get
            If mOtherPerpendicular Is Nothing Then
                mOtherPerpendicular = CrossProduct(AnyPerpendicular)
                mOtherPerpendicular = mOtherPerpendicular.Unit
            End If

            Return mOtherPerpendicular
        End Get
    End Property

#End Region


#Region " Negate read-only property "

    Public ReadOnly Property Negate As Vector3D
        Get
            Return New Vector3D(-X, -Y, -Z)
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a vector from 3 coordinates.
    ''' </summary>
    Public Sub New(x As Double, y As Double, z As Double)
        Me.X = x
        Me.Y = y
        Me.Z = z
    End Sub


    ''' <summary>
    ''' Create a vector from A to B.
    ''' </summary>
    Public Shared Function CreateA2B(a As IPoint3D, b As IPoint3D) As Vector3D
        Return New Vector3D(b.X - a.X, b.Y - a.Y, b.Z - a.Z)
    End Function


    ''' <summary>
    ''' Find an average vector between vectors A-B and B-C.
    ''' </summary>
    ''' <remarks>
    ''' A or C can be Nothing, but not both. B cannot be Nothing.
    ''' </remarks>
    Public Shared Function AverageVector(a As IPoint3D, b As IPoint3D, c As IPoint3D) As Vector3D
        If a Is Nothing AndAlso c Is Nothing Then
            Throw New ArgumentException("Both A and C cannot be absent to find an average")
        End If

        If b Is Nothing Then
            Throw New ArgumentException("B cannot be absent to find an average")
        End If

        If a Is Nothing Then
            Return CreateA2B(b, c)
        ElseIf c Is Nothing Then
            Return CreateA2B(a, b)
        ElseIf Point3DHelper.IsSame(a, c) Then
            Return CreateA2B(a, b)
        Else
            Return CreateA2B(a, c)
        End If
    End Function

#End Region


#Region " API "

    ''' <summary>
    ''' Multiply the vector by a scalar.
    ''' </summary>
    Public Function Multiply(a As Double) As Vector3D
        Return New Vector3D(a * X, a * Y, a * Z)
    End Function


    ''' <summary>
    ''' Move the given point by the vector.
    ''' </summary>
    Public Function [From](pt As IPoint3D) As IPoint3D
        Return Point3DHelper.Create(pt.X + X, pt.Y + Y, pt.Z + Z)
    End Function


    ''' <summary>
    ''' Subtract the other vector from this one.
    ''' </summary>
    Public Function Minus(other As Vector3D) As Vector3D
        Return New Vector3D(X - other.X, Y - other.Y, Z - other.Z)
    End Function


    ''' <summary>
    ''' Ordinary dot-product of two vectors.
    ''' </summary>
    Public Function DotProduct(other As Vector3D) As Double
        Return X * other.X + Y * other.Y + Z * other.Z
    End Function


    ''' <summary>
    ''' "Dot-product" of a vector and a point.
    ''' </summary>
    Public Function DotProduct(other As IPoint3D) As Double
        Return X * other.X + Y * other.Y + Z * other.Z
    End Function


    ''' <summary>
    ''' Cross-product, a vector perpendicular to the two given vectors.
    ''' </summary>
    Public Function CrossProduct(other As Vector3D) As Vector3D
        Return New Vector3D(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X)
    End Function


    ''' <summary>
    ''' The projection of a vector (dist) on a plane, defined by its normal (audToSource),
    ''' is called a rejection (as opposed to a projection on a vector).
    ''' </summary>
    ''' <param name="normal">Plane's normal</param>
    ''' <returns>Projection onto a plane defined by <paramref name="normal"/></returns>
    Public Function Rejection(normal As Vector3D) As Vector3D
        Return Minus(normal.Multiply(DotProduct(normal) / normal.Square))
    End Function


    ''' <summary>
    ''' Check whether the two vectors are the same.
    ''' </summary>
    Public Shared Function IsSame(a As Vector3D, b As Vector3D) As Boolean
        Return IsEqual(a.X, b.X) AndAlso IsEqual(a.Y, b.Y) AndAlso IsEqual(a.Z, b.Z)
    End Function

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format("->({0:F2}, {1:F2}, {2:F2})", X, Y, Z)
    End Function

#End Region

End Class
