Imports System.ComponentModel
Imports System.Globalization
Imports System.Collections.ObjectModel


''' <summary>
''' Mimics Binding class, but is open.
''' </summary>
Public MustInherit Class BindingExtensionBase
    Inherits BindingBaseExtensionBase

#Region "Properties"

    Private _binding As BindingBase


    Protected Overrides ReadOnly Property BindingBase() As BindingBase
        Get
            If _binding Is Nothing Then
                _binding = New Binding()
            End If
            Return _binding
        End Get
    End Property


    Public ReadOnly Property Binding() As Binding
        Get
            Return TryCast(BindingBase, Binding)
        End Get
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property AsyncState() As Object
        Get
            Return Binding.AsyncState
        End Get
        Set(value As Object)
            Binding.AsyncState = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property BindsDirectlyToSource() As Boolean
        Get
            Return Binding.BindsDirectlyToSource
        End Get
        Set(value As Boolean)
            Binding.BindsDirectlyToSource = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, IValueConverter))>
    Public Property Converter() As IValueConverter
        Get
            Return Binding.Converter
        End Get
        Set(value As IValueConverter)
            Binding.Converter = value
        End Set
    End Property


    <TypeConverter(GetType(CultureInfoIetfLanguageTagConverter))>
    <DefaultValue(CType(Nothing, CultureInfo))>
    Public Property ConverterCulture() As CultureInfo
        Get
            Return Binding.ConverterCulture
        End Get
        Set(value As CultureInfo)
            Binding.ConverterCulture = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property ConverterParameter() As Object
        Get
            Return Binding.ConverterParameter
        End Get
        Set(value As Object)
            Binding.ConverterParameter = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, String))>
    Public Property ElementName() As String
        Get
            Return Binding.ElementName
        End Get
        Set(value As String)
            Binding.ElementName = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property IsAsync() As Boolean
        Get
            Return Binding.IsAsync
        End Get
        Set(value As Boolean)
            Binding.IsAsync = value
        End Set
    End Property


    <DefaultValue(BindingMode.[Default])>
    Public Property Mode() As BindingMode
        Get
            Return Binding.Mode
        End Get
        Set(value As BindingMode)
            Binding.Mode = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property NotifyOnSourceUpdated() As Boolean
        Get
            Return Binding.NotifyOnSourceUpdated
        End Get
        Set(value As Boolean)
            Binding.NotifyOnSourceUpdated = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property NotifyOnTargetUpdated() As Boolean
        Get
            Return Binding.NotifyOnTargetUpdated
        End Get
        Set(value As Boolean)
            Binding.NotifyOnTargetUpdated = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property NotifyOnValidationError() As Boolean
        Get
            Return Binding.NotifyOnValidationError
        End Get
        Set(value As Boolean)
            Binding.NotifyOnValidationError = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, PropertyPath))>
    Public Property Path() As PropertyPath
        Get
            Return Binding.Path
        End Get
        Set(value As PropertyPath)
            Binding.Path = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, RelativeSource))>
    Public Property RelativeSource() As RelativeSource
        Get
            Return Binding.RelativeSource
        End Get
        Set(value As RelativeSource)
            Binding.RelativeSource = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property Source() As Object
        Get
            Return Binding.Source
        End Get
        Set(value As Object)
            Binding.Source = value
        End Set
    End Property


    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Property UpdateSourceExceptionFilter() As UpdateSourceExceptionFilterCallback
        Get
            Return Binding.UpdateSourceExceptionFilter
        End Get
        Set(value As UpdateSourceExceptionFilterCallback)
            Binding.UpdateSourceExceptionFilter = value
        End Set
    End Property


    <DefaultValue(UpdateSourceTrigger.PropertyChanged)>
    Public Property UpdateSourceTrigger() As UpdateSourceTrigger
        Get
            Return Binding.UpdateSourceTrigger
        End Get
        Set(value As UpdateSourceTrigger)
            Binding.UpdateSourceTrigger = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property ValidatesOnDataErrors() As Boolean
        Get
            Return Binding.ValidatesOnDataErrors
        End Get
        Set(value As Boolean)
            Binding.ValidatesOnDataErrors = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property ValidatesOnExceptions() As Boolean
        Get
            Return Binding.ValidatesOnExceptions
        End Get
        Set(value As Boolean)
            Binding.ValidatesOnExceptions = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, String))>
    Public Property XPath() As String
        Get
            Return Binding.XPath
        End Get
        Set(value As String)
            Binding.XPath = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Collection(Of ValidationRule)))>
    Public ReadOnly Property ValidationRules() As Collection(Of ValidationRule)
        Get
            Return Binding.ValidationRules
        End Get
    End Property

#End Region

End Class
