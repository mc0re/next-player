Imports System.ComponentModel
Imports TextChannelLibrary


Public Class TextChannelEditorWindow

#Region " Channel dependency property "

    Public Shared ReadOnly ChannelProperty As DependencyProperty = DependencyProperty.Register(
            NameOf(Channel), GetType(TextPhysicalChannel), GetType(TextChannelEditorWindow))


    <Description("The channel to edit")>
    Public Property Channel As TextPhysicalChannel
        Get
            Return CType(GetValue(ChannelProperty), TextPhysicalChannel)
        End Get
        Set(value As TextPhysicalChannel)
            SetValue(ChannelProperty, value)
        End Set
    End Property

#End Region

End Class
