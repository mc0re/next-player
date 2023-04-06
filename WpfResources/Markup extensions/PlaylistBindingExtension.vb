Imports Common
Imports PlayerActions

''' <summary>
''' Custom binding for playlist properties.
''' </summary>
Public Class PlaylistBindingExtension
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


    Public Sub New()
        MyBase.New(NameOf(IConfiguration.CurrentActionCollection))
        InitializeLink()
    End Sub


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
        Dim firstProp = path.Split("."c).First()

        If GetType(PlayerActionCollection).GetProperty(firstProp) IsNot Nothing Then
            Return NameOf(IConfiguration.CurrentActionCollection) & "." & path
        Else
            Return NameOf(IConfiguration.CurrentActionCollection) & "." & NameOf(IPlaylist.CurrentEnvironment) & "." & path
        End If
    End Function

#End Region

End Class
