Imports System.Windows.Markup
Imports System.ComponentModel


Public Class DelayMultiBindingExtension
    Inherits MultiBindingExtensionBase

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


#Region " Binding overrides "

    Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object
        Dim service = DirectCast(serviceProvider.GetService(GetType(IProvideValueTarget)), IProvideValueTarget)
        Dim targetObject = TryCast(service.TargetObject, DependencyObject)
        Dim targetProperty = TryCast(service.TargetProperty, DependencyProperty)

        If targetObject Is Nothing OrElse targetProperty Is Nothing Then
            Return MyBase.ProvideValue(serviceProvider)
        End If

        ' Prevent the designer from reporting exceptions because
        ' GetMetadata returns null in design mode
        If DesignerProperties.GetIsInDesignMode(targetObject) = True Then
            Return BindingMode.OneWay
        End If

        SetBinding(targetObject, targetProperty)

        ' Return the current value
        Return targetObject.GetValue(targetProperty)
    End Function


    Public Overrides Sub SetBinding(targetObject As DependencyObject, targetProperty As DependencyProperty)
        Dim metadata = TryCast(targetProperty.GetMetadata(targetObject.GetType()), FrameworkPropertyMetadata)

        MultiBinding.Mode = GetBindingMode(metadata)

        ' Used as a workaround to bug that happends when the setter rejects the new value.
        ' The GUI control doesn't stay in sync. E.g SelectedItem for a ComboBox
        MultiBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit

        For Each binding In MultiBinding.Bindings.Cast(Of Binding)()
            If binding.Mode = BindingMode.[Default] Then
                ' Usually for a MultiBinding, when the TargetProperty has the BindsTwoWayByDefault flag set to true
                ' then the Bindings also binds TwoWay if their BindingMode is set to Default. Since the SourcePropertyMirror,
                ' (which ultimately uses the Binding) hasn't got this flag set, this workaround is needed
                If MultiBinding.Mode = BindingMode.TwoWay AndAlso metadata.BindsTwoWayByDefault = True Then
                    ' This one is also need since we are setting the MultiBinding Mode to TwoWay.
                    binding.Mode = BindingMode.TwoWay

                ElseIf MultiBinding.Mode = BindingMode.OneWayToSource Then
                    binding.Mode = BindingMode.OneWayToSource
                End If
            End If
        Next

        Dim delayBindingController As New DelayBindingController(
            targetObject, targetProperty, UpdateSourceDelay, UpdateTargetDelay, MultiBinding, MultiBinding.Mode)

        DelayBindingManager.GetDelayBindingControllers(targetObject).Add(delayBindingController)
        delayBindingController.SetupBindingListeners()
    End Sub


    Private Function GetBindingMode(metadata As FrameworkPropertyMetadata) As BindingMode
        If MultiBinding.Mode <> BindingMode.[Default] Then Return MultiBinding.Mode

        Return If(metadata IsNot Nothing AndAlso metadata.BindsTwoWayByDefault,
                BindingMode.TwoWay,
                BindingMode.OneWay)
    End Function

#End Region

End Class
