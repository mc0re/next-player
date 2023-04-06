Imports System.Windows.Threading


''' <summary>
''' This object controls the actual delay.
''' </summary>
''' <remarks>
''' Taken from http://meleak.wordpress.com/2012/05/02/delaybinding-and-delaymultibinding-with-source-and-target-delay/.
''' The delay is modified from the original, so a timer is started,
''' when a property is changed, and it is not restarted upon the next
''' change. It "accumulates" changes instead and fires them after
''' the given delay.
''' </remarks>
Public Class DelayBindingController
    Inherits Freezable

#Region " Fields "

    Private WithEvents mUpdateTargetTimer As DispatcherTimer

    Private WithEvents mUpdateSourceTimer As DispatcherTimer

    Private mDelayTarget As Boolean = False

    Private mDelaySource As Boolean = False

    Private mStringFormatFix As Boolean = False

    Private mUpdatingSourceValue As Boolean = False

    Private mUpdatingTargetValue As Boolean = False

#End Region


#Region " TargetObject read-only property "

    Private mTargetObject As DependencyObject


    Public Property TargetObject As DependencyObject
        Get
            Return mTargetObject
        End Get
        Private Set(value As DependencyObject)
            mTargetObject = value
        End Set
    End Property

#End Region


#Region " TargetProperty read-only property "

    Private mTargetProperty As DependencyProperty


    Public Property TargetProperty As DependencyProperty
        Get
            Return mTargetProperty
        End Get
        Private Set(value As DependencyProperty)
            mTargetProperty = value
        End Set
    End Property

#End Region


#Region " Binding property "

    Private mBinding As BindingBase


    Public Property Binding As BindingBase
        Get
            Return mBinding
        End Get
        Set(value As BindingBase)
            mBinding = value
        End Set
    End Property

#End Region


#Region " SourcePropertyMirror dependency property "

    Public Shared SourcePropertyMirrorProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(SourcePropertyMirror), GetType(Object), GetType(DelayBindingController),
        New FrameworkPropertyMetadata(Nothing, AddressOf OnSourcePropertyMirrorChanged))


    Public Property SourcePropertyMirror As Object
        Get
            Return GetValue(SourcePropertyMirrorProperty)
        End Get
        Set(value As Object)
            SetValue(SourcePropertyMirrorProperty, value)
        End Set
    End Property


    Private Shared Sub OnSourcePropertyMirrorChanged(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim delayBindingController As DelayBindingController = TryCast(sender, DelayBindingController)
        delayBindingController.SourcePropertyValueChanged()
    End Sub

#End Region


#Region " StringSourcePropertyMirror dependency property "

    Public Shared StringSourcePropertyMirrorProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(StringSourcePropertyMirror), GetType(String), GetType(DelayBindingController),
        New FrameworkPropertyMetadata(Nothing, AddressOf OnSourcePropertyMirrorChanged))


    ''' <summary>
    ''' This DependencyProperty is only used because
    ''' StringFormat requires a DependencyProperty of type string to work
    ''' </summary>
    Public Property StringSourcePropertyMirror As String
        Get
            Return DirectCast(GetValue(StringSourcePropertyMirrorProperty), String)
        End Get
        Set(value As String)
            SetValue(StringSourcePropertyMirrorProperty, value)
        End Set
    End Property

#End Region


#Region " TargetPropertyMirror dependency property "

    Public Shared TargetPropertyMirrorProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(TargetPropertyMirror), GetType(Object), GetType(DelayBindingController),
        New FrameworkPropertyMetadata(Nothing, AddressOf OnTargetPropertyMirrorChanged))


    Public Property TargetPropertyMirror As Object
        Get
            Return GetValue(TargetPropertyMirrorProperty)
        End Get
        Set(value As Object)
            SetValue(TargetPropertyMirrorProperty, value)
        End Set
    End Property


    Private Shared Sub OnTargetPropertyMirrorChanged(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim delayBindingController As DelayBindingController = TryCast(sender, DelayBindingController)
        delayBindingController.TargetPropertyValueChanged()
    End Sub

#End Region


#Region " Init and clean-up "

    Public Sub New()
        ' Do nothing
    End Sub


    Public Sub New(
        targetObject As DependencyObject,
        targetProperty As DependencyProperty,
        updateSourceDelay As TimeSpan,
        updateTargetDelay As TimeSpan,
        binding As BindingBase,
        mode As BindingMode
    )

        Me.TargetObject = targetObject
        Me.TargetProperty = targetProperty

        If mode = BindingMode.TwoWay OrElse mode = BindingMode.OneWayToSource Then
            mDelaySource = True
            mUpdateSourceTimer = New DispatcherTimer() With {
                .Interval = updateSourceDelay
            }
        End If

        If mode <> BindingMode.OneWayToSource Then
            mDelayTarget = True
            mUpdateTargetTimer = New DispatcherTimer() With {
                .Interval = updateTargetDelay
            }
        End If

        Me.Binding = binding
    End Sub

#End Region


#Region " Freezable overrides "

    Protected Overrides Function CreateInstanceCore() As Freezable
        Return New DelayBindingController()
    End Function

#End Region


#Region " API: set and clear bindings"

    Public Sub SetupBindingListeners()
        If TargetObject Is Nothing Then
            Return
        End If

        Dim tBinding As New Binding() With {
            .Source = TargetObject,
            .Path = New PropertyPath(TargetProperty),
            .Mode = BindingMode.TwoWay
        }
        BindingOperations.SetBinding(Me, TargetPropertyMirrorProperty, tBinding)

        If TargetProperty.PropertyType = GetType(String) Then
            mStringFormatFix = True
            BindingOperations.SetBinding(Me, StringSourcePropertyMirrorProperty, Binding)
        Else
            BindingOperations.SetBinding(Me, SourcePropertyMirrorProperty, Binding)
        End If
    End Sub


    Public Sub ClearBinding()
        BindingOperations.ClearBinding(Me, TargetPropertyMirrorProperty)

        If TargetProperty.PropertyType = GetType(String) Then
            BindingOperations.ClearBinding(Me, StringSourcePropertyMirrorProperty)
        Else
            BindingOperations.ClearBinding(Me, SourcePropertyMirrorProperty)
        End If
    End Sub

#End Region


#Region " Private methods "

    ''' <summary>
    ''' Unless the "update target" timer is running, start it.
    ''' </summary>
    Private Sub SourcePropertyValueChanged()
        If mDelayTarget AndAlso Not mUpdatingTargetValue Then
            If mUpdateSourceTimer IsNot Nothing Then
                mUpdateSourceTimer.Stop()
            End If

            If Not mUpdateTargetTimer.IsEnabled Then
                mUpdateTargetTimer.Start()
            End If
        End If
    End Sub


    ''' <summary>
    ''' This is the original behaviour: start the time anew whenever the value is changed.
    ''' </summary>
    Private Sub TargetPropertyValueChanged()
        If mDelaySource AndAlso Not mUpdatingSourceValue Then
            If mUpdateTargetTimer IsNot Nothing Then
                mUpdateTargetTimer.Stop()
            End If

            mUpdateSourceTimer.Stop()
            mUpdateSourceTimer.Start()
        End If
    End Sub


    Private Sub UpdateSourceTimer_Tick(sender As Object, e As EventArgs) Handles mUpdateSourceTimer.Tick
        mUpdateSourceTimer.Stop()
        UpdateSourceValue()
    End Sub


    Private Sub UpdateTargetTimer_Tick(sender As Object, e As EventArgs) Handles mUpdateTargetTimer.Tick
        mUpdateTargetTimer.Stop()
        UpdateTargetValue()
    End Sub


    Private Sub UpdateSourceValue()
        Dim sourceValue = GetSourcePropertyMirrorValue()
        Dim targetValue = GetValue(TargetPropertyMirrorProperty)

        If targetValue IsNot Nothing AndAlso Not targetValue.Equals(sourceValue) OrElse
            sourceValue IsNot Nothing AndAlso Not sourceValue.Equals(targetValue) Then

            mUpdatingSourceValue = True
            SetSourcePropertyMirrorValue(targetValue)
            mUpdatingSourceValue = False
        End If
    End Sub


    Private Sub UpdateTargetValue()
        Dim sourceValue = GetSourcePropertyMirrorValue()
        Dim targetValue = GetValue(TargetPropertyMirrorProperty)

        If targetValue IsNot Nothing AndAlso Not targetValue.Equals(sourceValue) OrElse
            sourceValue IsNot Nothing AndAlso Not sourceValue.Equals(targetValue) Then

            mUpdatingTargetValue = True
            Me.SetValue(TargetPropertyMirrorProperty, sourceValue)
            mUpdatingTargetValue = False
        End If
    End Sub


    Private Function GetSourcePropertyMirrorValue() As Object
        If mStringFormatFix Then
            Return GetValue(StringSourcePropertyMirrorProperty)
        End If

        Return GetValue(SourcePropertyMirrorProperty)
    End Function


    Private Sub SetSourcePropertyMirrorValue(targetValue As Object)
        If mStringFormatFix Then
            SetValue(StringSourcePropertyMirrorProperty, targetValue)
            Dim bindingExpressionBase = BindingOperations.GetBindingExpressionBase(Me, StringSourcePropertyMirrorProperty)

            If bindingExpressionBase IsNot Nothing Then
                bindingExpressionBase.UpdateSource()
            End If
        Else
            SetValue(SourcePropertyMirrorProperty, targetValue)
            Dim bindingExpressionBase = BindingOperations.GetBindingExpressionBase(Me, SourcePropertyMirrorProperty)

            If bindingExpressionBase IsNot Nothing Then
                bindingExpressionBase.UpdateSource()
            End If
        End If
    End Sub

#End Region

End Class
