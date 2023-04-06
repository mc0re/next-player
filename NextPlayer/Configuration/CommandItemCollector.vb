Imports System.Windows.Markup
Imports System.ComponentModel


<ContentProperty("Children")>
Public Class CommandItemCollector
	Inherits Control
	Implements IAddChild

#Region " Children read-only dependency property "

    Private Shared ReadOnly ChildrenPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(Children), GetType(CommandItemCollection), GetType(CommandItemCollector),
        New PropertyMetadata(New CommandItemCollection()))


    Public Shared ReadOnly ChildrenProperty As DependencyProperty = ChildrenPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("A list of commands")>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Public ReadOnly Property Children As CommandItemCollection
        Get
            Return CType(GetValue(ChildrenProperty), CommandItemCollection)
        End Get
    End Property

#End Region


#Region " IAddChild implementation "

    Public Sub AddChild(value As Object) Implements IAddChild.AddChild
		If value Is Nothing Then
			Throw New ArgumentNullException(NameOf(value))
		End If

		Dim elem = TryCast(value, CommandItem)
		If elem Is Nothing Then
            Throw New ArgumentException($"Expected type {NameOf(CommandItem)}, got {value.GetType().Name}", NameOf(value))
        End If

		Children.Add(elem)
	End Sub


	Public Sub AddText(text As String) Implements IAddChild.AddText
		If Not String.IsNullOrWhiteSpace(text) Then
			Throw New ArgumentException("Text is not accepted")
		End If
	End Sub

#End Region

End Class
