Imports System.ComponentModel

Public Class VoiceCommandSettingItemCollection
    Inherits BindingList(Of CommandSettingItem)

    Public Sub New()
        ' Do nothing
    End Sub


    Public Sub New(ops As IEnumerable(Of CommandSettingItem))
        For Each op In ops
            Me.Add(op)
        Next
    End Sub

End Class