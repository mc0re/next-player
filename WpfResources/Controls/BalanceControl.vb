''' <summary>
''' Slider to regulate stereo balance.
''' </summary>
<TemplatePart(Name:="PART_Indicator", Type:=GetType(FrameworkElement))>
<TemplatePart(Name:="PART_Track", Type:=GetType(FrameworkElement))>
Public Class BalanceControl
    Inherits PositionedRangeBase

#Region " Overrides "

    Protected Overrides Sub ProgressValueChanged(progress As Double)
        Dim bnd = GetBindingExpression(ValueProperty)
        progress = Math.Round(progress, 2)

        Dim obj = bnd.ResolvedSource
        Dim propName = bnd.ResolvedSourcePropertyName

        If propName Is Nothing Then
            Throw New ArgumentException(
                $"Cannot find property '{bnd.ParentBinding.Path.Path}' in object '{bnd.DataItem}'.")
        End If

        Dim prop = obj.GetType().GetProperty(propName)

        prop.SetValue(obj, Convert.ChangeType(progress, prop.PropertyType))
    End Sub

#End Region

End Class
