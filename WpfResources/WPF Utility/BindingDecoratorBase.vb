Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.Windows.Markup


''' <summary>
''' A base class for custom markup extension which provides properties
''' that can be found on regular <see cref="Binding"/> markup extension.<br/>
''' See: http://www.hardcodet.net/2008/04/wpf-custom-binding-class 
''' </summary>
<MarkupExtensionReturnType(GetType(Object))>
Public MustInherit Class BindingDecoratorBase
    Inherits MarkupExtension

    ''' <summary>
    ''' The decorated binding class.
    ''' </summary>
    Private m_binding As New Binding()


#Region " Properties "

    ''' <summary>
    ''' The decorated binding class.
    ''' </summary>
    <Browsable(False)> _
    Public Property Binding() As Binding
        Get
            Return m_binding
        End Get
        Set(value As Binding)
            m_binding = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property AsyncState() As Object
        Get
            Return m_binding.AsyncState
        End Get
        Set(value As Object)
            m_binding.AsyncState = value
        End Set
    End Property


    <DefaultValue(False)> _
    Public Property BindsDirectlyToSource() As Boolean
        Get
            Return m_binding.BindsDirectlyToSource
        End Get
        Set(value As Boolean)
            m_binding.BindsDirectlyToSource = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, IValueConverter))>
    Public Property Converter() As IValueConverter
        Get
            Return m_binding.Converter
        End Get
        Set(value As IValueConverter)
            m_binding.Converter = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property TargetNullValue() As Object
        Get
            Return m_binding.TargetNullValue
        End Get
        Set(value As Object)
            m_binding.TargetNullValue = value
        End Set
    End Property


    <TypeConverter(GetType(CultureInfoIetfLanguageTagConverter))>
    <DefaultValue(CType(Nothing, CultureInfo))>
    Public Property ConverterCulture() As CultureInfo
        Get
            Return m_binding.ConverterCulture
        End Get
        Set(value As CultureInfo)
            m_binding.ConverterCulture = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property ConverterParameter() As Object
        Get
            Return m_binding.ConverterParameter
        End Get
        Set(value As Object)
            m_binding.ConverterParameter = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, String))>
    Public Property ElementName() As String
        Get
            Return m_binding.ElementName
        End Get
        Set(value As String)
            m_binding.ElementName = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property FallbackValue() As Object
        Get
            Return m_binding.FallbackValue
        End Get
        Set(value As Object)
            m_binding.FallbackValue = value
        End Set
    End Property


    <DefaultValue(False)> _
    Public Property IsAsync() As Boolean
        Get
            Return m_binding.IsAsync
        End Get
        Set(value As Boolean)
            m_binding.IsAsync = value
        End Set
    End Property


    <DefaultValue(BindingMode.[Default])> _
    Public Property Mode() As BindingMode
        Get
            Return m_binding.Mode
        End Get
        Set(value As BindingMode)
            m_binding.Mode = value
        End Set
    End Property


    <DefaultValue(False)> _
    Public Property NotifyOnSourceUpdated() As Boolean
        Get
            Return m_binding.NotifyOnSourceUpdated
        End Get
        Set(value As Boolean)
            m_binding.NotifyOnSourceUpdated = value
        End Set
    End Property


    <DefaultValue(False)> _
    Public Property NotifyOnTargetUpdated() As Boolean
        Get
            Return m_binding.NotifyOnTargetUpdated
        End Get
        Set(value As Boolean)
            m_binding.NotifyOnTargetUpdated = value
        End Set
    End Property


    <DefaultValue(False)> _
    Public Property NotifyOnValidationError() As Boolean
        Get
            Return m_binding.NotifyOnValidationError
        End Get
        Set(value As Boolean)
            m_binding.NotifyOnValidationError = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, PropertyPath))>
    Public Property Path() As PropertyPath
        Get
            Return m_binding.Path
        End Get
        Set(value As PropertyPath)
            m_binding.Path = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, RelativeSource))>
    Public Property RelativeSource() As RelativeSource
        Get
            Return m_binding.RelativeSource
        End Get
        Set(value As RelativeSource)
            m_binding.RelativeSource = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Type))>
    Public Property Source() As Object
        Get
            Return m_binding.Source
        End Get
        Set(value As Object)
            m_binding.Source = value
        End Set
    End Property


    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Property UpdateSourceExceptionFilter() As UpdateSourceExceptionFilterCallback
        Get
            Return m_binding.UpdateSourceExceptionFilter
        End Get
        Set(value As UpdateSourceExceptionFilterCallback)
            m_binding.UpdateSourceExceptionFilter = value
        End Set
    End Property


    <DefaultValue(UpdateSourceTrigger.[Default])> _
    Public Property UpdateSourceTrigger() As UpdateSourceTrigger
        Get
            Return m_binding.UpdateSourceTrigger
        End Get
        Set(value As UpdateSourceTrigger)
            m_binding.UpdateSourceTrigger = value
        End Set
    End Property


    <DefaultValue(False)> _
    Public Property ValidatesOnDataErrors() As Boolean
        Get
            Return m_binding.ValidatesOnDataErrors
        End Get
        Set(value As Boolean)
            m_binding.ValidatesOnDataErrors = value
        End Set
    End Property


    <DefaultValue(False)> _
    Public Property ValidatesOnExceptions() As Boolean
        Get
            Return m_binding.ValidatesOnExceptions
        End Get
        Set(value As Boolean)
            m_binding.ValidatesOnExceptions = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, String))>
    Public Property XPath() As String
        Get
            Return m_binding.XPath
        End Get
        Set(value As String)
            m_binding.XPath = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, Collection(Of ValidationRule)))>
    Public ReadOnly Property ValidationRules() As Collection(Of ValidationRule)
        Get
            Return m_binding.ValidationRules
        End Get
    End Property


    <DefaultValue(CType(Nothing, String))>
    Public Property StringFormat() As String
        Get
            Return m_binding.StringFormat
        End Get
        Set(value As String)
            m_binding.StringFormat = value
        End Set
    End Property


    <DefaultValue("")> _
    Public Property BindingGroupName() As String
        Get
            Return m_binding.BindingGroupName
        End Get
        Set(value As String)
            m_binding.BindingGroupName = value
        End Set
    End Property

#End Region


#Region " MarkupExtension overrides "

    ''' <summary>
    ''' This basic implementation just sets a binding on the targeted
    ''' <see cref="DependencyObject"/> and returns the appropriate
    ''' <see cref="BindingExpressionBase"/> instance.<br/>
    ''' All this work is delegated to the decorated <see cref="Binding"/>
    ''' instance.
    ''' </summary>
    ''' <returns>
    ''' The object value to set on the property where the extension is applied. 
    ''' In case of a valid binding expression, this is a <see cref="BindingExpressionBase"/>
    ''' instance.
    ''' </returns>
    ''' <param name="provider">Object that can provide services for the markup
    ''' extension.</param>
    Public Overrides Function ProvideValue(provider As IServiceProvider) As Object
        'create a binding and associate it with the target
        Return m_binding.ProvideValue(provider)
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Validates a service provider that was submitted to the <see cref="ProvideValue"/>
    ''' method. This method checks whether the provider is null (happens at design time),
    ''' whether it provides an <see cref="IProvideValueTarget"/> service, and whether
    ''' the service's <see cref="IProvideValueTarget.TargetObject"/> and
    ''' <see cref="IProvideValueTarget.TargetProperty"/> properties are valid
    ''' <see cref="DependencyObject"/> and <see cref="DependencyProperty"/>
    ''' instances.
    ''' </summary>
    ''' <param name="provider">The provider to be validated.</param>
    ''' <param name="target">The binding target of the binding.</param>
    ''' <param name="dp">The target property of the binding.</param>
    ''' <returns>True if the provider supports all that's needed.</returns>
    Protected Overridable Function TryGetTargetItems(provider As IServiceProvider, ByRef target As DependencyObject, ByRef dp As DependencyProperty) As Boolean
        target = Nothing
        dp = Nothing
        If provider Is Nothing Then
            Return False
        End If

        'create a binding and assign it to the target
        Dim service As IProvideValueTarget = DirectCast(provider.GetService(GetType(IProvideValueTarget)), IProvideValueTarget)
        If service Is Nothing Then
            Return False
        End If

        'we need dependency objects / properties
        target = TryCast(service.TargetObject, DependencyObject)
        dp = TryCast(service.TargetProperty, DependencyProperty)
        Return target IsNot Nothing AndAlso dp IsNot Nothing
    End Function

#End Region

End Class
