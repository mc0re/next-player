Imports System.Windows.Markup
Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Globalization


''' <summary>
''' Mimics MultiBinding, but is open.
''' </summary>
<ContentProperty("Bindings")>
Public MustInherit Class MultiBindingExtensionBase
    Inherits BindingBaseExtensionBase
    Implements IAddChild

#Region "IAddChild"

    Private Sub IAddChild_AddChild(value As [Object]) Implements IAddChild.AddChild
        Dim binding As BindingBase = TryCast(value, BindingBase)
        If binding IsNot Nothing Then
            Bindings.Add(binding)
        End If
    End Sub


    Private Sub IAddChild_AddText(text As String) Implements IAddChild.AddText
    End Sub

#End Region


#Region "Properties"

    Private _binding As BindingBase


    Protected Overrides ReadOnly Property BindingBase() As BindingBase
        Get
            If _binding Is Nothing Then
                _binding = New MultiBinding()
            End If
            Return _binding
        End Get
    End Property


    Public ReadOnly Property MultiBinding() As MultiBinding
        Get
            Return TryCast(BindingBase, MultiBinding)
        End Get
    End Property


    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
    Public ReadOnly Property Bindings() As Collection(Of BindingBase)
        Get
            Return MultiBinding.Bindings
        End Get
    End Property


    <DefaultValue(CType(Nothing, IMultiValueConverter))>
    Public Property Converter() As IMultiValueConverter
        Get
            Return MultiBinding.Converter
        End Get
        Set(value As IMultiValueConverter)
            MultiBinding.Converter = value
        End Set
    End Property


    <TypeConverter(GetType(CultureInfoIetfLanguageTagConverter))>
    <DefaultValue(CType(Nothing, CultureInfo))>
    Public Property ConverterCulture() As CultureInfo
        Get
            Return MultiBinding.ConverterCulture
        End Get
        Set(value As CultureInfo)
            MultiBinding.ConverterCulture = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property ConverterParameter() As Object
        Get
            Return MultiBinding.ConverterParameter
        End Get
        Set(value As Object)
            MultiBinding.ConverterParameter = value
        End Set
    End Property


    <DefaultValue(BindingMode.[Default])>
    Public Property Mode() As BindingMode
        Get
            Return MultiBinding.Mode
        End Get
        Set(value As BindingMode)
            MultiBinding.Mode = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property NotifyOnSourceUpdated() As Boolean
        Get
            Return MultiBinding.NotifyOnSourceUpdated
        End Get
        Set(value As Boolean)
            MultiBinding.NotifyOnSourceUpdated = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property NotifyOnTargetUpdated() As Boolean
        Get
            Return MultiBinding.NotifyOnTargetUpdated
        End Get
        Set(value As Boolean)
            MultiBinding.NotifyOnTargetUpdated = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property NotifyOnValidationError() As Boolean
        Get
            Return MultiBinding.NotifyOnValidationError
        End Get
        Set(value As Boolean)
            MultiBinding.NotifyOnValidationError = value
        End Set
    End Property


    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property UpdateSourceExceptionFilter() As UpdateSourceExceptionFilterCallback
        Get
            Return MultiBinding.UpdateSourceExceptionFilter
        End Get
        Set(value As UpdateSourceExceptionFilterCallback)
            MultiBinding.UpdateSourceExceptionFilter = value
        End Set
    End Property


    <DefaultValue(UpdateSourceTrigger.PropertyChanged)>
    Public Property UpdateSourceTrigger() As UpdateSourceTrigger
        Get
            Return MultiBinding.UpdateSourceTrigger
        End Get
        Set(value As UpdateSourceTrigger)
            MultiBinding.UpdateSourceTrigger = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property ValidatesOnDataErrors() As Boolean
        Get
            Return MultiBinding.ValidatesOnDataErrors
        End Get
        Set(value As Boolean)
            MultiBinding.ValidatesOnDataErrors = value
        End Set
    End Property


    <DefaultValue(False)>
    Public Property ValidatesOnExceptions() As Boolean
        Get
            Return MultiBinding.ValidatesOnExceptions
        End Get
        Set(value As Boolean)
            MultiBinding.ValidatesOnExceptions = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Collection(Of ValidationRule)))>
    Public ReadOnly Property ValidationRules() As Collection(Of ValidationRule)
        Get
            Return MultiBinding.ValidationRules
        End Get
    End Property

#End Region

End Class
