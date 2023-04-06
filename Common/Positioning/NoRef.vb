''' <summary>
''' A class denoting no reference information is attached.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class NoRef

    Private Sub New()
        ' Do nothing
    End Sub


    Public Shared ReadOnly Property Empty As IEnumerable(Of NoRef) = Enumerable.Empty(Of NoRef)()

End Class
