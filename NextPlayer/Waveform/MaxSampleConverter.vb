''' <summary>
''' Get the maximum sample value from sample cache.
''' </summary>
<ValueConversion(GetType(String), GetType(Geometry))>
Public Class MaxSampleConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		If Not TypeOf value Is String Then Return Binding.DoNothing

		Dim fName = CStr(value)
		Return WaveformStorage.GetMaxSample(fName)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException("MaxSampleConverter cannot convert back")
	End Function

#End Region

End Class
