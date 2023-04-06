Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data
Imports System.Windows.Media
Imports PlayerActions
Imports WpfResources


'''<summary>
''' Class containing test cases for all converters.
'''</summary>
<TestClass>
Public Class ConverterTest

    '''<summary>
    '''A test for BooleanNotConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub BooleanNotConverterTest()
        Dim target As IValueConverter = New BooleanNotConverter()
        Dim actual As Object

        actual = target.Convert(False, GetType(Boolean), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual(True, actual)

        actual = target.Convert(True, GetType(Boolean), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual(False, actual)
    End Sub


    '''<summary>
    '''A test for BooleanToVisibilityConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub BooleanToVisibilityConverterTest()
        Dim target As IValueConverter = New BooleanToVisibilityConverter()
        Dim actual As Object

        actual = target.Convert(False, GetType(Visibility), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual(Visibility.Collapsed, actual)

        actual = target.Convert(True, GetType(Visibility), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual(Visibility.Visible, actual)
    End Sub


    '''<summary>
    '''A test for BooleanToWrapConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub BooleanToWrapConverterTest()
        Dim target As IValueConverter = New BooleanToWrapConverter()
        Dim actual As Object

        actual = target.Convert(False, GetType(TextWrapping), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual(TextWrapping.NoWrap, actual)

        actual = target.Convert(True, GetType(TextWrapping), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual(TextWrapping.Wrap, actual)
    End Sub


    '''<summary>
    '''A test for BrushToStringConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub BrushToStringConverterTest()
        Dim target As IValueConverter = New BrushToStringConverter()
        Dim actual As Object

        ' Forward conversion
        actual = target.Convert(New SolidColorBrush(Colors.Red), GetType(String), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual("#FFFF0000", actual)

        Dim lbr As New LinearGradientBrush()
        lbr.GradientStops.Add(New GradientStop(Colors.Red, 0))
        lbr.GradientStops.Add(New GradientStop(Colors.Blue, 1))
        actual = target.Convert(lbr, GetType(String), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual("#FFFF0000 #FF0000FF", actual)

        ' Back conversion
        actual = target.ConvertBack("#FFFF0000", GetType(Brush), Nothing, CultureInfo.InvariantCulture)
        Dim actualSbr = TryCast(actual, SolidColorBrush)
        Assert.IsNotNull(actualSbr)
        Assert.AreEqual(Colors.Red, actualSbr.Color)

        actual = target.ConvertBack("-#FFFF0000 #FF0000FF", GetType(Brush), Nothing, CultureInfo.InvariantCulture)
        Dim actualLbr = TryCast(actual, LinearGradientBrush)
        Assert.IsNotNull(actualLbr)
        Assert.AreEqual(2, actualLbr.GradientStops.Count)
        Assert.AreEqual(Colors.Red, actualLbr.GradientStops(0).Color)
        Assert.AreEqual(Colors.Blue, actualLbr.GradientStops(1).Color)
    End Sub


    '''<summary>
    '''A test for EnumToVisibilityConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub EnumToVisibilityConverterTest()
        Dim target As IValueConverter = New EnumToVisibilityConverter()
        Dim actual As Object

        actual = target.Convert(HorizontalAlignment.Center, GetType(Visibility), "Center", CultureInfo.InvariantCulture)
        Assert.AreEqual(Visibility.Visible, actual)

        actual = target.Convert(HorizontalAlignment.Center, GetType(Visibility), "Left", CultureInfo.InvariantCulture)
        Assert.AreEqual(Visibility.Collapsed, actual)

        Try
            actual = Nothing
            actual = target.Convert(HorizontalAlignment.Center, GetType(Visibility), "NoSuchValue", CultureInfo.InvariantCulture)
            Assert.Fail("Should throw exception")
        Catch ex As ArgumentException
            Assert.IsNull(actual)
        Catch ex As Exception
            Assert.Fail("Incorrect exception thrown: " & ex.Message)
        End Try

        actual = target.Convert(HorizontalAlignment.Center, GetType(Visibility), HorizontalAlignment.Center, CultureInfo.InvariantCulture)
        Assert.AreEqual(Visibility.Visible, actual)
    End Sub


    '''<summary>
    '''A test for PlayerActionPlaceholderConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub PlayerActionPlaceholderConverterTest()
        Dim target As IValueConverter = New PlayerActionPlaceholderConverter()
        Dim actual As Object

        actual = target.Convert(Nothing, GetType(PlayerAction), Nothing, CultureInfo.InvariantCulture)
        Assert.AreSame(PlayerAction.PlaceHolder, actual)

        actual = target.Convert(New PlayerActionFile(), GetType(PlayerAction), Nothing, CultureInfo.InvariantCulture)
        Assert.AreNotSame(PlayerAction.PlaceHolder, actual)
    End Sub


    '''<summary>
    '''A test for RadioButtonCheckedConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub RadioButtonCheckedConverterTest()
        Dim target As IValueConverter = New RadioButtonCheckedConverter()
        Dim actual As Object

        actual = target.Convert(False, GetType(Boolean), False, CultureInfo.InvariantCulture)
        Assert.AreEqual(True, actual)

        actual = target.Convert(HorizontalAlignment.Center, GetType(Boolean), HorizontalAlignment.Center, CultureInfo.InvariantCulture)
        Assert.AreEqual(True, actual)

        actual = target.Convert(HorizontalAlignment.Center, GetType(Boolean), HorizontalAlignment.Left, CultureInfo.InvariantCulture)
        Assert.AreEqual(False, actual)

        actual = target.ConvertBack(True, GetType(Boolean), HorizontalAlignment.Center, CultureInfo.InvariantCulture)
        Assert.AreEqual(HorizontalAlignment.Center, actual)

        actual = target.ConvertBack(False, GetType(Boolean), HorizontalAlignment.Center, CultureInfo.InvariantCulture)
        Assert.AreEqual(Binding.DoNothing, actual)
    End Sub


    '''<summary>
    '''A test for StringToVisibilityConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub StringToVisibilityConverterTest()
        Dim target As IValueConverter = New StringToVisibilityConverter()
        Dim actual As Object

        actual = target.Convert(String.Empty, GetType(Visibility), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual(Visibility.Collapsed, actual)

        actual = target.Convert("Text", GetType(Visibility), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual(Visibility.Visible, actual)
    End Sub


    '''<summary>
    '''A test for TimeSpanFormatConverter
    '''</summary>
    <TestMethod(), TestCategory("Converters")>
    Public Sub TimeSpanFormatConverterTest()
        Dim target As IValueConverter = New TimeSpanFormatConverter()
        Dim actual As Object

        actual = target.Convert(TimeSpan.FromSeconds(3), GetType(String), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual("00:03.000", actual)

        actual = target.Convert(TimeSpan.FromMinutes(4), GetType(String), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual("04:00.000", actual)

        actual = target.Convert(New TimeSpan(1, 2, 3), GetType(String), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual("01:02:03.000", actual)

        actual = target.Convert(TimeSpan.FromSeconds(-5), GetType(String), Nothing, CultureInfo.InvariantCulture)
        Assert.AreEqual("-00:05.000", actual)

        actual = target.Convert(TimeSpan.FromSeconds(-6), GetType(String), "Start {1}{0:mm\:ss} end", CultureInfo.InvariantCulture)
        Assert.AreEqual("Start -00:06 end", actual)

        actual = target.Convert(TimeSpan.FromSeconds(7), GetType(String), "Start {1}{0:mm\:ss} end", CultureInfo.InvariantCulture)
        Assert.AreEqual("Start 00:07 end", actual)

        actual = target.Convert(TimeSpan.FromSeconds(8), GetType(String), "Start {2}{0:mm\:ss} end", CultureInfo.InvariantCulture)
        Assert.AreEqual("Start +00:08 end", actual)
    End Sub

End Class
