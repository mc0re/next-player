Imports System.ComponentModel


Public Class ActionHeaderControl
    Inherits Control

#Region " HeaderText dependency property "

    Public Shared ReadOnly HeaderTextProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(HeaderText), GetType(String), GetType(ActionHeaderControl))


    <Category("Common Properties"), Description("Header to show (e.g. action type)")>
    Public Property HeaderText As String
        Get
            Return CStr(GetValue(HeaderTextProperty))
        End Get
        Set(value As String)
            SetValue(HeaderTextProperty, value)
        End Set
    End Property

#End Region

End Class
