Public Class DelayBindingControllerCollection
    Inherits FreezableCollection(Of DelayBindingController)

End Class


''' <summary>
''' Holds an attached property, where delayed bindings are kept.
''' </summary>
Public Class DelayBindingManager

#Region " DelayBindingControllers attached dependency property "

    Public Shared DelayBindingControllersProperty As DependencyProperty = DependencyProperty.RegisterAttached(
        "DelayBindingControllers", GetType(DelayBindingControllerCollection), GetType(DelayBindingManager),
        New FrameworkPropertyMetadata(Nothing))


    Public Shared Function GetDelayBindingControllers(obj As DependencyObject) As DelayBindingControllerCollection
        If obj.GetValue(DelayBindingControllersProperty) Is Nothing Then
            obj.SetValue(DelayBindingControllersProperty, New DelayBindingControllerCollection())
        End If
        Return DirectCast(obj.GetValue(DelayBindingControllersProperty), DelayBindingControllerCollection)
    End Function


    Public Shared Sub SetDelayBindingControllers(obj As DependencyObject, value As DelayBindingControllerCollection)
        obj.SetValue(DelayBindingControllersProperty, value)
    End Sub

#End Region

End Class
