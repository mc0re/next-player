Imports PlayerActions


<ValueConversion(GetType(Integer), GetType(ImageSource))>
Public Class SlideIndexToImageConverter
    Implements IMultiValueConverter

#Region " IMultiValueConverter implementation "

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        For Each value In values
            If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing
        Next

        Dim slideIndex = CInt(values(0))
        Dim presIndex = CInt(values(1))

        Dim presRef = PresenterFactory.GetReference(presIndex)
        If presRef Is Nothing Then Return String.Empty

        Dim slidePath = presRef.GetThumbnailName(False, slideIndex)
        If Not IO.File.Exists(slidePath) Then Return Nothing

        Try
            Dim src = New BitmapImage()
            src.BeginInit()
            src.UriSource = New Uri(slidePath, UriKind.Absolute)
            src.EndInit()

            Return src

        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

#End Region

End Class
