Imports Common


''' <summary>
''' Custom binding for configuration properties.
''' </summary>
Public Class ConfigBindingExtension
    Inherits Binding

#Region " Binding functionality "

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


    ''' <summary>
    ''' This constructor is used, when the extension is set using XML syntax.
    ''' </summary>
    Public Sub New()
        InitializeLink()
    End Sub


    ''' <summary>
    ''' This constructor is used, when the extension is set using {} syntax.
    ''' </summary>
    Public Sub New(path As String)
        MyBase.New(AdjustPath(path))
        InitializeLink()
    End Sub


    Private Sub InitializeLink()
        ' Protection for design mode
        Source = InterfaceMapper.GetImplementation(Of IConfiguration)(True)

        If Mode = BindingMode.Default Then
            Mode = BindingMode.TwoWay
        End If
    End Sub

#End Region


#Region " Utility "

    Private Shared Function AdjustPath(path As String) As String
        ' Protection for design mode
        Dim config = InterfaceMapper.GetImplementation(Of IConfiguration)(True)

        If config Is Nothing Then Return path ' To facilitate XAML editor

        Dim firstProp = path.Split("."c).First()

        If config.GetType().GetProperty(firstProp) IsNot Nothing Then
            Return path
        Else
            Return NameOf(IConfiguration.CurrentEnvironment) & "." & path
        End If
    End Function

#End Region

End Class
