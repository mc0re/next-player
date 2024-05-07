Imports System.ComponentModel
Imports System.Windows.Media
Imports PlayerActions


''' <summary>
''' A ListBoxItem with drag-and-drop indicators.
''' </summary>
Public Class PlaylistItem
	Inherits ListBoxItem

#Region " Constants "

	Private Const DragThreshold As Double = 10

#End Region


#Region " IsSelectedItem attached dependency property "

	Public Shared ReadOnly IsSelectedItemProperty As DependencyProperty = DependencyProperty.RegisterAttached(
		NameOf(IsSelectedItem), GetType(Boolean), GetType(PlaylistItem),
		New FrameworkPropertyMetadata(False, FrameworkPropertyMetadataOptions.Inherits))


	Public Property IsSelectedItem As Boolean
		Get
			Return GetIsSelectedItem(Me)
		End Get
		Set(value As Boolean)
			SetIsSelectedItem(Me, value)
		End Set
	End Property


	<AttachedPropertyBrowsableForChildren()>
	Public Shared Function GetIsSelectedItem(elem As UIElement) As Boolean
		Return CBool(elem.GetValue(IsSelectedItemProperty))
	End Function


	Public Shared Sub SetIsSelectedItem(elem As UIElement, value As Boolean)
		elem.SetValue(IsSelectedItemProperty, value)
	End Sub

#End Region


#Region " IsDraggingOverTop dependency property "

	Public Shared ReadOnly IsDraggingOverTopProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(IsDraggingOverTop), GetType(Boolean), GetType(PlaylistItem),
		New PropertyMetadata(False))


	<Category("Appearance"), Description("Whether something is about to be dropped over the top of the item.")>
	Public Property IsDraggingOverTop As Boolean
		Get
			Return CBool(GetValue(IsDraggingOverTopProperty))
		End Get
		Set(value As Boolean)
			SetValue(IsDraggingOverTopProperty, value)
		End Set
	End Property

#End Region


#Region " IsDraggingOverBottom dependency property "

	Public Shared ReadOnly IsDraggingOverBottomProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(IsDraggingOverBottom), GetType(Boolean), GetType(PlaylistItem),
		New PropertyMetadata(False))


	<Category("Appearance"), Description("Whether something is about to be dropped over the bottom of the item.")>
	Public Property IsDraggingOverBottom As Boolean
		Get
			Return CBool(GetValue(IsDraggingOverBottomProperty))
		End Get
		Set(value As Boolean)
			SetValue(IsDraggingOverBottomProperty, value)
		End Set
	End Property

#End Region


#Region " DragPositionBrush dependency property "

	Public Shared ReadOnly DragPositionBrushProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(DragPositionBrush), GetType(Brush), GetType(PlaylistItem))


	<Category("Brushes"), Description("Points to the target of drag-and-drop action")>
	Public Property DragPositionBrush As Brush
		Get
			Return CType(GetValue(DragPositionBrushProperty), Brush)
		End Get
		Set(value As Brush)
			SetValue(DragPositionBrushProperty, value)
		End Set
	End Property

#End Region


#Region " Drag source "

	Private mLastClickSet As Boolean = False


	''' <summary>
	''' Position of last "click"
	''' </summary>
	Private mLastClick As Point


	Private Sub MouseDownHandler(sender As Object, args As MouseButtonEventArgs) Handles Me.PreviewMouseDown
		If Not mLastClickSet AndAlso args.LeftButton = MouseButtonState.Pressed Then
			mLastClickSet = True
			mLastClick = args.GetPosition(Me)
		End If
	End Sub


	Private Sub MouseUpHandler(sender As Object, args As MouseButtonEventArgs) Handles Me.MouseUp
		If args.LeftButton = MouseButtonState.Released Then
			mLastClickSet = False
		End If
	End Sub


	Private Sub MouseMoveHandler(sender As Object, args As MouseEventArgs) Handles Me.MouseMove
		If mLastClickSet AndAlso args.LeftButton = MouseButtonState.Pressed Then
			Dim pos = args.GetPosition(Me)
			pos.Offset(-mLastClick.X, -mLastClick.Y)
			If Math.Abs(pos.X) + Math.Abs(pos.Y) < DragThreshold Then
				Return
			End If

			Dim dobj As New DataObject(GetType(PlayerAction), DataContext)
			DragDrop.DoDragDrop(Me, dobj, DragDropEffects.Move Or DragDropEffects.Copy)
		End If
	End Sub

#End Region

End Class
