Imports System.Globalization


''' <summary>
''' Convert the audio file into a waveform.
''' 
''' Parameters:
''' - File name (String)
''' - Output width (Double)
''' - Element (DependencyObject)
''' 
''' The output width is given for optimization.
''' The element is used to retrieve Dispatcher.
''' 
''' The maximum X-coordinate is the lentgh of the file, in milliseconds.
''' </summary>
<ValueConversion(GetType(String), GetType(Geometry))>
Public Class WaveformConverter
    Implements IMultiValueConverter

#Region " IMultiValueConverter implementation "

    Public Function Convert(values As Object(), targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        If values.Any(Function(value) value Is DependencyProperty.UnsetValue) Then
            Return Binding.DoNothing
        End If

        Dim fName = CStr(values(0))
        Dim outWidth = CInt(CDbl(values(1)))
        If outWidth = 0 Then Return Binding.DoNothing

        Dim ctrl = CType(values(2), DependencyObject)

        Dim fig = WaveformStorage.GetGeometry(fName, outWidth, ctrl.Dispatcher)
        Dim geom As New PathGeometry()
        If fig IsNot Nothing Then
            geom.Figures.Add(fig)
        End If

        Return geom
    End Function


    Public Function ConvertBack(value As Object, targetTypes As Type(), parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

#End Region

End Class
