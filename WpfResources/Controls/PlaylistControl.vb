Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Windows.Controls.Primitives
Imports AudioPlayerLibrary
Imports Common
Imports PlayerActions


''' <summary>
''' Whether the drag target is closer to the top or to the bottom of the item.
''' </summary>
Public Enum DropItemParts
    None
    Top
    Bottom
End Enum


''' <summary>
''' Definition of event arguments.
''' </summary>
Public Class InsertItemsEventArgs
    Inherits RoutedEventArgs

    Public Property InsertIndex As Integer

    Public Property ItemList As IEnumerable(Of PlayerAction)


    Public Sub New(evt As RoutedEvent)
        MyBase.New(evt)
    End Sub

End Class


''' <summary>
''' Delegate type for handling InsertItem event.
''' </summary>
Public Delegate Sub InsertItemsEventHandler(sender As Object, args As InsertItemsEventArgs)


''' <summary>
''' The main song list.
''' </summary>
''' <remarks>
''' Supports drag-and-drop.
''' </remarks>
<StyleTypedProperty(Property:="ItemContainerStyle", StyleTargetType:=GetType(PlaylistItem))>
Public Class PlaylistControl
    Inherits ListBox

#Region " Drag and drop constants "

    Private Const ThresholdWhenEnter = 0.15


    Private Const ThresholdWhenLeave = 0.1

#End Region


#Region " Command definitions "

    Public Property LoadPlaylistCommand As New DelegateCommand(AddressOf LoadPlaylistCommandExecuted)

#End Region


#Region " PlaylistLoaded routed event "

    Public Shared ReadOnly PlaylistLoadedEvent As RoutedEvent = EventManager.RegisterRoutedEvent(
        NameOf(PlaylistLoaded), RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(PlaylistControl))


    Public Custom Event PlaylistLoaded As RoutedEventHandler
        AddHandler(ByVal value As RoutedEventHandler)
            Me.AddHandler(PlaylistLoadedEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As RoutedEventHandler)
            Me.RemoveHandler(PlaylistLoadedEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

#End Region


#Region " InsertItems routed event "

    Public Shared ReadOnly InsertItemsEvent As RoutedEvent = EventManager.RegisterRoutedEvent(
        NameOf(InsertItems), RoutingStrategy.Bubble, GetType(InsertItemsEventHandler), GetType(PlaylistControl))


    Public Custom Event InsertItems As InsertItemsEventHandler
        AddHandler(ByVal value As InsertItemsEventHandler)
            Me.AddHandler(InsertItemsEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As InsertItemsEventHandler)
            Me.RemoveHandler(InsertItemsEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As InsertItemsEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

#End Region


#Region " Fields "

    Private mLastDragTarget As PlaylistItem


    Private mDropPart As DropItemParts


    Private mKeyBindingHolder As UIElement

#End Region


#Region " Init and clean-up "

    Private Sub LoadedHandler() Handles Me.Loaded
        mKeyBindingHolder = FindMeOrVisualChild(Me, Function(c) CType(c, UIElement).InputBindings.Count > 0)
    End Sub

#End Region


#Region " Override item type "

    Protected Overrides Function IsItemItsOwnContainerOverride(item As Object) As Boolean
        Return TypeOf item Is PlaylistItem
    End Function


    Protected Overrides Function GetContainerForItemOverride() As DependencyObject
        Return New PlaylistItem()
    End Function

#End Region


#Region " IsDraggingToEnd read-only dependency property "

    Private Shared ReadOnly IsDraggingToEndPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(IsDraggingToEnd), GetType(Boolean), GetType(PlaylistControl),
        New PropertyMetadata(False))


    Public Shared ReadOnly IsDraggingToEndProperty As DependencyProperty = IsDraggingToEndPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Whether the drag target is the list, but none of its elements")>
    Public Property IsDraggingToEnd As Boolean
        Get
            Return CBool(GetValue(IsDraggingToEndProperty))
        End Get
        Private Set(value As Boolean)
            SetValue(IsDraggingToEndPropertyKey, value)
        End Set
    End Property

#End Region


#Region " Drag and drop "

    ''' <summary>
    ''' Allow all effects.
    ''' Check whether this is before or after the item.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="Style is not a name")>
    <CodeAnalysis.SuppressMessage("Style", "CC0014:Use Ternary operator.", Justification:="If is more readable here")>
    Private Sub DragOverHandler(sender As Object, args As DragEventArgs) Handles Me.DragOver
        Dim coordRelToList = args.GetPosition(Me)
        Dim newItem = FindVisualParent(Of PlaylistItem)(TryCast(InputHitTest(coordRelToList), DependencyObject))

        Dim isAction = args.Data.GetDataPresent(GetType(PlayerAction))

        If args.KeyStates.HasFlag(DragDropKeyStates.ControlKey) OrElse Not isAction Then
            args.Effects = DragDropEffects.Copy
        Else
            args.Effects = DragDropEffects.Move
        End If

        If newItem IsNot Nothing Then
            Dim coordRelToItem = args.GetPosition(newItem)
            CheckCoordinates(coordRelToItem, mLastDragTarget, newItem)
        End If

        If newItem Is Nothing AndAlso isAction Then
            args.Effects = DragDropEffects.None
        End If

        mLastDragTarget = newItem
        args.Handled = True
    End Sub


    ''' <summary>
    ''' Raised for every child, so check whether the item has changed.
    ''' </summary>
    Private Sub DragLeaveHandler(sender As Object, args As DragEventArgs) Handles Me.DragLeave
        Dim coordRelToList = args.GetPosition(Me)
        Dim newItem = FindVisualParent(Of PlaylistItem)(TryCast(InputHitTest(coordRelToList), DependencyObject))

        If newItem Is Nothing Then
            RemoveDragEffects()
        End If

        args.Handled = True
    End Sub


    ''' <summary>
    ''' The item is dropped.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="Style is not a name")>
    <CodeAnalysis.SuppressMessage("Style", "CC0014:Use Ternary operator.", Justification:="If is more readable here")>
    Private Sub DropHandler(sender As Object, args As DragEventArgs) Handles Me.Drop
        ' Find index to insert
        Dim idx = -1

        If mLastDragTarget IsNot Nothing Then
            idx = ItemContainerGenerator.IndexFromContainer(mLastDragTarget)

            If mDropPart = DropItemParts.Bottom Then
                If idx >= Items.Count Then
                    idx = -1
                Else
                    idx += 1
                End If
            End If
        ElseIf Items.Count > 0 AndAlso args.Data.GetDataPresent(GetType(PlayerAction)) Then
            ' If not an empty list, disallow dragging list items to "no target"
            Return
        End If

        RemoveDragEffects()

        ' Collect the items
        Dim itemList As New List(Of PlayerAction)()

        If args.Data.GetDataPresent(GetType(PlayerAction)) Then
            DragDropMoveOrCopyAction(args, itemList)
        ElseIf args.Data.GetDataPresent(DataFormats.FileDrop) Then
            DragDropAddFile(args, itemList)
        ElseIf args.Data.GetDataPresent(DataFormats.StringFormat) Then
            DragDropAddComment(args, itemList)
        End If

        If Not itemList.Any() Then Return

        args.Handled = True

        Dim insArgs = New InsertItemsEventArgs(InsertItemsEvent) With {
            .InsertIndex = idx, .ItemList = itemList
        }
        RaiseEvent InsertItems(Me, insArgs)
        SelectedItem = itemList.LastOrDefault()
    End Sub


    ''' <summary>
    ''' The dragged item is <see cref="PlayerAction"/>, move or copy (if "Ctrl").
    ''' </summary>
    Private Shared Sub DragDropMoveOrCopyAction(args As DragEventArgs, itemList As List(Of PlayerAction))
        Dim item = CType(args.Data.GetData(GetType(PlayerAction)), PlayerAction)
        If args.KeyStates.HasFlag(DragDropKeyStates.ControlKey) Then
            item = CType(item.Clone(), PlayerAction)
        End If
        itemList.Add(item)
    End Sub


    ''' <summary>
    ''' The dragged item is a file, add as audio producer.
    ''' </summary>
    Private Shared Sub DragDropAddFile(args As DragEventArgs, itemList As List(Of PlayerAction))
        Dim finfo = CType(args.Data.GetData(DataFormats.FileDrop), String())

        If finfo Is Nothing Then
            ' AIMP delivers "ACL.FileURIs" stream
            Dim strm = CType(args.Data.GetData("ACL.FileURIs"), MemoryStream)
            If strm Is Nothing Then Return

            Dim arr = strm.ToArray()
            Dim offset = BitConverter.ToInt32(arr, 0)
            Dim isWide = BitConverter.ToBoolean(arr, 16)
            Dim encoder = If(isWide, Encoding.Unicode, Encoding.ASCII)
            Dim namesStr = encoder.GetString(arr, offset, arr.Length - offset)
            finfo = namesStr.
                Split(vbNullChar.First()).
                TakeWhile(Function(s) Not String.IsNullOrEmpty(s)).
                ToArray()
        End If

        For Each fName In finfo
            Dim act = New PlayerActionFile(fName)
            act.AfterLoad(String.Empty)
            itemList.Add(act)
        Next
    End Sub


    ''' <summary>
    ''' The dragged item is a <see cref="String"/>, add as a comment.
    ''' </summary>
    Private Shared Sub DragDropAddComment(args As DragEventArgs, itemList As List(Of PlayerAction))
        Dim text = CStr(args.Data.GetData(DataFormats.StringFormat))
        Dim item = New PlayerActionComment() With {.Name = text}
        itemList.Add(item)
    End Sub

#End Region


#Region " Drag and drop utility "

    Private Sub RemoveDragEffects()
        CheckCoordinates(Nothing, mLastDragTarget, Nothing)
        mLastDragTarget = Nothing
        IsDraggingToEnd = False
    End Sub


    ''' <summary>
    ''' Check the dragging coordinates, set flags.
    ''' </summary>
    Private Sub CheckCoordinates(coord As Point, oldTarget As PlaylistItem, newTarget As PlaylistItem)
        If Not oldTarget Is newTarget Then
            ' The item has changed, release the old item.
            If oldTarget IsNot Nothing Then
                oldTarget.IsDraggingOverTop = False
                oldTarget.IsDraggingOverBottom = False
            End If

            ' Check whether this is addition to the end.
            IsDraggingToEnd = newTarget Is Nothing
        End If

        If newTarget IsNot Nothing Then
            ' Normalize to -0.5..+0.5
            Dim dy = (coord.Y - newTarget.ActualHeight / 2) / newTarget.ActualHeight

            ' Hysteresis
            If dy < 0 Then
                ' More to the top
                newTarget.IsDraggingOverTop =
                    (newTarget.IsDraggingOverTop AndAlso dy < -ThresholdWhenLeave) OrElse
                    (Not newTarget.IsDraggingOverTop AndAlso dy < -ThresholdWhenEnter)
            Else
                ' More to the bottom
                newTarget.IsDraggingOverBottom =
                    (newTarget.IsDraggingOverBottom AndAlso dy > ThresholdWhenLeave) OrElse
                    (Not newTarget.IsDraggingOverBottom AndAlso dy > ThresholdWhenEnter)
            End If

            mDropPart = If(newTarget.IsDraggingOverTop, DropItemParts.Top,
                           If(newTarget.IsDraggingOverBottom, DropItemParts.Bottom,
                           DropItemParts.None))
        End If
    End Sub

#End Region


#Region " Command handling "

    Private Sub LoadPlaylistCommandExecuted(param As Object)

    End Sub

#End Region


#Region " Keypresses "

    ''' <summary>
    ''' Raised when ListBox is in focus.
    ''' </summary>
    Private Sub TextInputHandler(sender As Object, args As TextCompositionEventArgs) Handles Me.PreviewTextInput
        Static keyConv As New KeyConverter()

        args.Handled = True

        Dim sentEvent = Keyboard.KeyDownEvent
        Dim sentKey = Key.None

        Select Case args.Text
            Case " "
                sentKey = Key.Space
            Case vbCr
                sentKey = Key.Enter
            Case "."
                sentKey = Key.OemPeriod
            Case ","
                sentKey = Key.OemComma
            Case "+"
                sentKey = Key.OemPlus
            Case "-"
                sentKey = Key.OemMinus
            Case Else
                If Char.IsLetterOrDigit(args.Text(0)) Then
                    Try
                        sentKey = CType(keyConv.ConvertFromString(args.Text), Key)
                    Catch ex As Exception
                        InterfaceMapper.GetImplementation(Of IMessageLog)().LogKeyError(
                            "Cannot convert key '{0}'", args.Text)
                    End Try
                Else
                    InterfaceMapper.GetImplementation(Of IMessageLog)().LogKeyError(
                        "The application is not ready for key '{0}'", args.Text)
                End If
        End Select

        ' Unknown key
        If sentKey = Key.None Then Return

        Dim keyArgs = New KeyEventArgs(
            Keyboard.PrimaryDevice, PresentationSource.FromVisual(mKeyBindingHolder), 0, sentKey) With {
                .RoutedEvent = sentEvent
            }
        mKeyBindingHolder.RaiseEvent(keyArgs)
    End Sub

#End Region


#Region " Focus "

    Private mLastItemInFocus As Control


    Public Sub SetFocusToMe()
        If mLastItemInFocus IsNot Nothing Then
            mLastItemInFocus.Focus()
            mLastItemInFocus.BringIntoView()
        Else
            Focus()
        End If
    End Sub


    Private Sub GotFocusHandler() Handles Me.GotFocus
        Dim cont = TryCast(ItemContainerGenerator.ContainerFromItem(SelectedItem), FrameworkElement)

        If cont IsNot Nothing Then
            Try
                cont.Focus()
            Catch ex As Exception
                ' Apparently, a CLR exception can occur here, probably due to PInvoke
                InterfaceMapper.GetImplementation(Of IMessageLog)().LogLoadingError(
                    $"Exception when setting focus: {ex.Message}")
            End Try
        End If
    End Sub


    ''' <summary>
    ''' Force showing the given item.
    ''' </summary>
    Public Sub BringItemToView(act As IPlayerAction)
        ScrollIntoView(act) ' Usually fails, so we proceed

        If ItemContainerGenerator.Status <> GeneratorStatus.ContainersGenerated Then
            ' Not ready
            Return
        End If

        Dim container = ItemContainerGenerator.ContainerFromItem(act)
        Dim actListItem = CType(container, PlaylistItem)
        actListItem.BringIntoView()

        mLastItemInFocus = actListItem
    End Sub

#End Region

End Class
