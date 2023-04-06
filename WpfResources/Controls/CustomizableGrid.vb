Imports System.ComponentModel
Imports System.Windows.Media


''' <summary>
''' Typedef for a list of column definitions.
''' </summary>
Public Class ColumnDefinitionList
	Inherits List(Of ColumnDefinition)

End Class


''' <summary>
''' A grid with a property, accepting a style-defined collection of
''' <see cref="ColumnDefinition"/> objects.
''' </summary>
Public Class CustomizableGrid
	Inherits Grid

#Region " ColumnDefList dependency property "

	Public Shared ReadOnly Property ColumnDefListProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(ColumnDefList), GetType(ColumnDefinitionList), GetType(CustomizableGrid),
		New FrameworkPropertyMetadata(New PropertyChangedCallback(AddressOf OnColumnDefListChanged)))


	Public Property ColumnDefList As ColumnDefinitionList
		Get
			Return CType(GetValue(ColumnDefListProperty), ColumnDefinitionList)
		End Get
		Set(value As ColumnDefinitionList)
			SetValue(ColumnDefListProperty, value)
		End Set
	End Property


	Private Shared Sub OnColumnDefListChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = DirectCast(obj, CustomizableGrid)
		Dim cols = DirectCast(args.NewValue, ColumnDefinitionList)

		this.ColumnDefinitions.Clear()

		' Add copies of original definitions to avoid multiple parents.
		For Each def In cols
			this.ColumnDefinitions.Add(
				New ColumnDefinition With {
					.Width = def.Width,
					.MaxWidth = def.MaxWidth, .MinWidth = def.MinWidth,
					.SharedSizeGroup = def.SharedSizeGroup
				})
		Next
	End Sub

#End Region


#Region " Padding dependency property "

	Public Shared ReadOnly Property PaddingProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Padding), GetType(Thickness), GetType(CustomizableGrid),
		New FrameworkPropertyMetadata(New PropertyChangedCallback(AddressOf OnPaddingChanged)))


	Public Property Padding As Thickness
		Get
			Return CType(GetValue(PaddingProperty), Thickness)
		End Get
		Set(value As Thickness)
			SetValue(PaddingProperty, value)
		End Set
	End Property


	Private Shared Sub OnPaddingChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = DirectCast(obj, CustomizableGrid)
		this.UpdateLayout()
	End Sub

#End Region


#Region " Init and clean-up "

	Public Sub OnLoaded() Handles Me.Loaded
		BindMarginToPadding()
	End Sub

#End Region


#Region " Padding children "

	''' <summary>
	''' Go through all visual children and bind their Margin property
	''' to <see cref="Padding"/>.
	''' </summary>
	Private Sub BindMarginToPadding()
		Dim childCount = VisualTreeHelper.GetChildrenCount(Me)

		For i = 0 To childCount - 1
			Dim child = VisualTreeHelper.GetChild(Me, i)
			Dim MarginProperty = GetMarginProperty(child)

			'  If we have a margin property, bind it to the padding.
			If MarginProperty IsNot Nothing Then
				'  Create the binding.
				Dim binding As New Binding() With {
					.Source = Me,
					.Path = New PropertyPath(NameOf(Padding))
				}

				' Bind the child's margin to the grid's padding.
				BindingOperations.SetBinding(child, MarginProperty, binding)
			End If
		Next
	End Sub


	''' <summary>
	''' Find Margin property descriptor.
	''' </summary>
	Private Shared Function GetMarginProperty(obj As DependencyObject) As DependencyProperty
		For Each prop In TypeDescriptor.GetProperties(obj).Cast(Of PropertyDescriptor)()
			Dim dpd = DependencyPropertyDescriptor.FromProperty(prop)

			'  Have we found the margin?
			If dpd IsNot Nothing AndAlso dpd.Name = NameOf(Margin) Then
				Return dpd.DependencyProperty
			End If
		Next

		Return Nothing
	End Function

#End Region

End Class
