Imports System.Diagnostics.CodeAnalysis


<CLSCompliant(True)>
<Serializable>
Public Class Vector2D

#Region " Constants "

    Public Shared ReadOnly Property AlongX As Vector2D = Vector2DHelper.Create(1, 0)

    Public Shared ReadOnly Property AlongY As Vector2D = Vector2DHelper.Create(0, 1)

#End Region


#Region " Own properties "

    Public ReadOnly Property X As Double

    Public ReadOnly Property Y As Double

#End Region


#Region " Square read-only property "

    ''' <summary>
    ''' Squared length of the vector, which is a dot product with itself.
    ''' </summary>
    Public ReadOnly Property Square As Double
        Get
            Return X * X + Y * Y
        End Get
    End Property

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


#Region " Angle read-only property "

    Private mAngle As Double?


    ''' <summary>
    ''' Get the inclination of the vector. Increases clockwise.
    ''' </summary>
    ''' <remarks>
    '''   IV |  I
    ''' -----+----
    '''  III | II
    '''  
    ''' I quadrant: -PI/2..0
    ''' II quadrant: 0..PI/2
    ''' III quadrant: PI/2..PI
    ''' IV quadrant: -PI..-PI/2
    ''' 
    ''' See <see cref="Vector2DHelper.CompareAngles"/>.
    ''' </remarks>
    Public ReadOnly Property Angle As Double
        Get
            If Not mAngle.HasValue Then
                mAngle = -Math.Atan2(Y, X)
            End If

            Return mAngle.Value
        End Get
    End Property

#End Region


#Region " Perpendicular spawning property "

    Private mPerpendicular As Vector2D


    ''' <summary>
    ''' Create a perpendicular to this vector,
    ''' that is 90 degrees to the right and has a length of 1.
    ''' </summary>
    Public ReadOnly Property Perpendicular As Vector2D
        Get
            If mPerpendicular Is Nothing Then
                Dim sz = Math.Sqrt(X * X + Y * Y)

                mPerpendicular = Vector2DHelper.Create(Y / sz, -X / sz)
            End If

            Return mPerpendicular
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Friend Sub New(x As Double, y As Double)
        Me.X = x
        Me.Y = y
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Add two vectors together.
    ''' </summary>
    Public Function Plus(other As Vector2D) As Vector2D
        Return New Vector2D(X + other.X, Y + other.Y)
    End Function


    ''' <summary>
    ''' Multiply a vector by a number.
    ''' </summary>
    Public Function Multiply(a As Double) As Vector2D
        Return New Vector2D(X * a, Y * a)
    End Function


    ''' <summary>
    ''' Create a new point setting this vector onto a given point.
    ''' Empty reference list is created.
    ''' </summary>
    Public Function FromPoint(a As IPoint2D) As IPoint2D
        Return Point2DHelper.Create(a.X + X, a.Y + Y)
    End Function

#End Region


#Region " ToString "

    ''' <summary>
    ''' For debugging.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return $"-> ({X:F2}, {Y:F2})"
    End Function

#End Region

End Class
