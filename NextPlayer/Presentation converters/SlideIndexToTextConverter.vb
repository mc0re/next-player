Imports PlayerActions


''' <summary>
''' Find first text on the slide or in the notes.
''' </summary>
''' <remarks>
''' Parameters:
'''  0) Slide index
'''  1) Presenter index
'''  2) Presenter version (for updates)
''' </remarks>
<ValueConversion(GetType(Integer), GetType(String))>
Public Class SlideIndexToTextConverter
	Implements IMultiValueConverter

#Region " IMultiValueConverter implementation "

	Public Function ConvertMulti(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
		For Each value In values
			If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing
		Next

		Dim slideIndex = CInt(values(0))
		Dim presIndex = CInt(values(1))

		Dim presRef = PresenterFactory.GetReference(presIndex)
		If presRef Is Nothing Then Return String.Empty

		Return presRef.GetSlideText(slideIndex)
	End Function


	Public Function ConvertMultiBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
