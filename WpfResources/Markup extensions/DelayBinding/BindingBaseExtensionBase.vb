Imports System.ComponentModel
Imports System.Windows.Markup


''' <summary>
''' Mimics BindingBase, but is open.
''' </summary>
Public MustInherit Class BindingBaseExtensionBase
    Inherits MarkupExtension

#Region "Properties"

    Protected MustOverride ReadOnly Property BindingBase() As BindingBase


    <DefaultValue(CType(Nothing, Type))>
    Public Property FallbackValue() As Object
        Get
            Return BindingBase.FallbackValue
        End Get
        Set(value As Object)
            BindingBase.FallbackValue = value
        End Set
    End Property


    <DefaultValue("")> _
    Public Property BindingGroupName() As String
        Get
            Return BindingBase.BindingGroupName
        End Get
        Set(value As String)
            BindingBase.BindingGroupName = value
        End Set
    End Property


    <DefaultValue(CType(Nothing, String))>
    Public Property StringFormat() As String
        Get
            Return BindingBase.StringFormat
        End Get
        Set(value As String)
            BindingBase.StringFormat = value
        End Set
    End Property


    Public Property TargetNullValue() As Object
        Get
            Return BindingBase.TargetNullValue
        End Get
        Set(value As Object)
            BindingBase.TargetNullValue = value
        End Set
    End Property

#End Region


#Region "Public Methods"

    Public Overrides Function ProvideValue(provider As IServiceProvider) As Object
        'create a binding and associate it with the target
        Return BindingBase.ProvideValue(provider)
    End Function


    Public MustOverride Sub SetBinding(targetObject As DependencyObject, targetProperty As DependencyProperty)

#End Region

End Class
