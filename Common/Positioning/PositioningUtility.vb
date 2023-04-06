Public Module PositioningUtility

#Region " Constants "

    Public Const AbsoluteCoordPrecisionDigits As Integer = 3


    Public Const AbsoluteCoordPrecision As Double = 0.001

#End Region


#Region " API "

    Public Function Sign(c As Single) As Integer
        If Math.Abs(c) < AbsoluteCoordPrecision Then
            Return 0
        ElseIf c > 0 Then
            Return 1
        Else
            Return -1
        End If
    End Function


    Public Function Sign(c As Double) As Integer
        If Math.Abs(c) < AbsoluteCoordPrecision Then
            Return 0
        ElseIf c > 0 Then
            Return 1
        Else
            Return -1
        End If
    End Function


    ''' <summary>
    ''' Check that the two coordinates can be considered equal.
    ''' </summary>
    Public Function IsEqual(a As Single, b As Single) As Boolean
        Return Sign(a - b) = 0
    End Function


    ''' <summary>
    ''' Check that the two coordinates can be considered equal.
    ''' </summary>
    Public Function IsEqual(a As Double, b As Double) As Boolean
        Return Sign(a - b) = 0
    End Function

#End Region

End Module
