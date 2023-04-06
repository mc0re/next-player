Imports System.Windows.Controls.Primitives
Imports Common


Public Class EnvironmentSelector
	Inherits ContentControl

#Region " ManageEnvironmentCommand command "

	Public Property CommandSelectedCommand As New DelegateCommand(AddressOf ManageEnvironmentCommandExecuted)


	Private Sub ManageEnvironmentCommandExecuted(param As Object)
		Dim elem = CType(param, ComboBox)
		Dim bnd = elem.GetBindingExpression(Selector.SelectedValueProperty)

		Dim item = TryCast(elem.SelectedItem, NamedCommand)

		If item Is Nothing Then
			bnd.UpdateSource()
		Else
			item.Command.Execute(Me)
			bnd.UpdateTarget()
		End If
	End Sub

#End Region


#Region " CopyCommand dependency property "

	Public Shared ReadOnly CopyCommandProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(CopyCommand), GetType(ICommand), GetType(EnvironmentSelector))


	Public Property CopyCommand As ICommand
		Get
			Return CType(GetValue(CopyCommandProperty), ICommand)
		End Get
		Set(value As ICommand)
			SetValue(CopyCommandProperty, value)
		End Set
	End Property

#End Region


#Region " EditCommand dependency property "

	Public Shared ReadOnly EditCommandProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(EditCommand), GetType(ICommand), GetType(EnvironmentSelector))


	Public Property EditCommand As ICommand
		Get
			Return CType(GetValue(EditCommandProperty), ICommand)
		End Get
		Set(value As ICommand)
			SetValue(EditCommandProperty, value)
		End Set
	End Property

#End Region

End Class
