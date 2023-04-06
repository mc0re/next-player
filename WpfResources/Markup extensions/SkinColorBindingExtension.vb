Imports Common


''' <summary>
''' Custom binding for skin configuration properties.
''' </summary>
Public Class SkinColorBindingExtension
    Inherits Binding

    Private Shared ReadOnly mColorConverter As New ColorChangerConverter()


    Public Sub New()
        InitializeLink()
    End Sub


    Public Sub New(path As String)
        MyBase.New(path)
        InitializeLink()
    End Sub


    Private Sub InitializeLink()
        ' Protection for design mode
        Dim conf = InterfaceMapper.GetImplementation(Of IConfiguration)(True)
        Source = conf?.Skin

        Mode = BindingMode.TwoWay
        Converter = mColorConverter
    End Sub

End Class
