Imports System.Windows.Markup
Imports System.ComponentModel


''' <summary>
''' A Binding with possibility for delaying the actual update.
''' </summary>
Public Class DelayBindingExtension
    Inherits BindingExtensionBase

#Region " UpdateTargetDelay property "

    Private m_UpdateTargetDelay As TimeSpan


    Public Property UpdateTargetDelay As TimeSpan
        Get
            Return m_UpdateTargetDelay
        End Get
        Set(value As TimeSpan)
            m_UpdateTargetDelay = value
        End Set
    End Property

#End Region


#Region " UpdateSourceDelay property "

    Private m_UpdateSourceDelay As TimeSpan


    Public Property UpdateSourceDelay As TimeSpan
        Get
            Return m_UpdateSourceDelay
        End Get
        Set(value As TimeSpan)
            m_UpdateSourceDelay = value
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New()
        ' Do nothing
    End Sub


    Public Sub New(path As String)
        Me.Path = New PropertyPath(path)
    End Sub

#End Region


#Region " Binding overrides "

    Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object
        Dim service As IProvideValueTarget = DirectCast(serviceProvider.GetService(GetType(IProvideValueTarget)), IProvideValueTarget)
        Dim targetObject As DependencyObject = TryCast(service.TargetObject, DependencyObject)
        Dim targetProperty As DependencyProperty = TryCast(service.TargetProperty, DependencyProperty)

        ' Prevent the designer from reporting exceptions because
        ' GetMetadata returns null in design mode
        If targetObject IsNot Nothing AndAlso DesignerProperties.GetIsInDesignMode(targetObject) = True Then
            Return Nothing
        End If

        If service.TargetObject.GetType().FullName = "System.Windows.SharedDp" Then
            Return Me
        End If

        If targetObject Is Nothing OrElse targetProperty Is Nothing Then
            Return MyBase.ProvideValue(serviceProvider)
        End If

        SetBinding(targetObject, targetProperty)

        ' Return the current value
        Return targetObject.GetValue(targetProperty)
    End Function


    Public Overrides Sub SetBinding(targetObject As DependencyObject, targetProperty As DependencyProperty)
        Binding.Mode = GetBindingMode(targetObject, targetProperty)
        ' Used as a workaround to bug that happends when the setter rejects the new value.
        ' The GUI control doesn't stay in sync. E.g SelectedItem for a ComboBox
        Binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit

        Dim delayBindingController As New DelayBindingController(
            targetObject, targetProperty, UpdateSourceDelay, UpdateTargetDelay, Binding, Binding.Mode)

        DelayBindingManager.GetDelayBindingControllers(targetObject).Add(delayBindingController)
        delayBindingController.SetupBindingListeners()
    End Sub


    Private Function GetBindingMode(targetObject As DependencyObject, targetProperty As DependencyProperty) As BindingMode
        If Binding.Mode <> BindingMode.[Default] Then Return Binding.Mode

        Dim metadata As FrameworkPropertyMetadata =
            TryCast(targetProperty.GetMetadata(targetObject.[GetType]()), FrameworkPropertyMetadata)

        Return If(metadata IsNot Nothing AndAlso metadata.BindsTwoWayByDefault,
                BindingMode.TwoWay,
                BindingMode.OneWay)
    End Function

#End Region

End Class
