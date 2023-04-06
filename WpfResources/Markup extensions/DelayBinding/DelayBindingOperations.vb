Public NotInheritable Class DelayBindingOperations

    Private Sub New()
        ' Do nothing
    End Sub


    Public Shared Sub ClearBinding(targetObject As DependencyObject, targetProperty As DependencyProperty)
        If targetObject Is Nothing Then Throw New ArgumentNullException("target")
        If targetProperty Is Nothing Then Throw New ArgumentNullException("dp")

        Dim controllers As DelayBindingControllerCollection = DelayBindingManager.GetDelayBindingControllers(targetObject)
        For i As Integer = 0 To controllers.Count - 1
            Dim controller As DelayBindingController = controllers(i)
            If controller.TargetProperty Is targetProperty Then
                controller.ClearBinding()
                controllers.Remove(controller)
                Exit For
            End If
        Next
    End Sub


    Public Shared Sub SetBinding(targetObject As DependencyObject, targetProperty As DependencyProperty, delayBinding As BindingBaseExtensionBase)
        If targetObject Is Nothing Then Throw New ArgumentNullException(NameOf(targetObject))
        If targetProperty Is Nothing Then Throw New ArgumentNullException(NameOf(targetProperty))
        If delayBinding Is Nothing Then Throw New ArgumentNullException(NameOf(delayBinding))

        delayBinding.SetBinding(targetObject, targetProperty)
    End Sub

End Class
