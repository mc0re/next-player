<CLSCompliant(True)>
<Serializable>
Public Class PositionRelative
    Implements IPositionRelative

#Region " Properties "

    Public Property XRel As Single Implements IPositionRelative.X

    Public Property YRel As Single Implements IPositionRelative.Y

    Public Property ZRel As Single Implements IPositionRelative.Z

#End Region


#Region " Init and clean-up "

    Public Sub New(x As Single, y As Single, z As Single)
        XRel = x
        YRel = y
        ZRel = z
    End Sub


    Public Sub New(c As IPositionRelative)
        XRel = c.X
        YRel = c.Y
        ZRel = c.Z
    End Sub

#End Region

End Class
