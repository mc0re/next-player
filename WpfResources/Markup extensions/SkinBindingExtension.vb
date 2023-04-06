Imports Common


''' <summary>
''' Custom binding for skin properties.
''' </summary>
Public Class SkinBindingExtension
    Inherits Binding

    ''' <summary>
    ''' Workaround for VS bug. Another solution is to move to another project.
    ''' </summary>
    Public Property ConverterType As Type
        Get
            Return If(Converter Is Nothing, Nothing, Converter.GetType())
        End Get
        Set(value As Type)
            Converter = If(value Is Nothing, Nothing,
                CType(Activator.CreateInstance(value), IValueConverter))
        End Set
    End Property


    Public Sub New()
        MyBase.New(NameOf(IConfiguration.Skin))
        InitializeLink()
    End Sub


    Public Sub New(path As String)
        MyBase.New(NameOf(IConfiguration.Skin) & "." & path)
        InitializeLink()
    End Sub


    Private Sub InitializeLink()
        ' Protection for design mode
        Source = InterfaceMapper.GetImplementation(Of IConfiguration)(True)
        Mode = BindingMode.TwoWay
    End Sub

End Class
