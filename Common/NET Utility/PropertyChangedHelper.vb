Imports System.ComponentModel
Imports System.Reflection
Imports System.Xml.Serialization
Imports System.Linq.Expressions


''' <summary>
''' This class implements INotifyPropertyChanged interface with some helper functions,
''' which avoid using strings in the code.
''' </summary>
''' <remarks>
''' To set a property to a new value, in the Setter call SetField(private-field, new-value, Function() property-name).
''' In some cases, this doesn't work properly - the private property is first updated after the End Set is reached.
''' For this case, set the private field manually and call OnPropertyChanged(Function() property-name).
''' The same should be done for read-only calculated properties.
''' </remarks>
<Serializable>
<CLSCompliant(True)>
Public Class PropertyChangedHelper
	Implements INotifyPropertyChanged, ICloneable

#Region " PropertyChanged event "

	<NonSerialized()>
	Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged


	''' <summary>
	''' Set a field to the new value, if not matching the previous one.
	''' Pass the field name further to raise the PropertyChanged event.
	''' </summary>
	''' <remarks>
	''' Do NOT use with <paramref name="field" /> member data declared with WithEvents - such member variables will not update until this method returns, and hence the PropertyChange event will fire
	''' while the field is still holding its old value. This is believed to be a side effect of the member being wrapped in an event class, which compiler generated code hides by using a temporary
	''' variable inside this method.
	''' </remarks>
	''' <typeparam name="T">Field type; omit it when using.</typeparam>
	''' <param name="field">Field name to set.</param>
	''' <param name="value">New value.</param>
	''' <param name="propName">Property name</param>
	<CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId:="0#")>
	Protected Sub SetField(Of T)(ByRef field As T, value As T, propName As String)
		If (EqualityComparer(Of T).Default.Equals(field, value)) Then Return

		field = value
		RaisePropertyChanged(propName)
	End Sub


	''' <summary>
	''' Set a field to the new value, if not matching the previous one.
	''' Pass the field name further to raise the PropertyChanged event.
	''' </summary>
	''' <remarks>
	''' Do NOT use with <paramref name="field" /> member data declared with WithEvents - such member variables will not update until this method returns, and hence the PropertyChange event will fire
	''' while the field is still holding its old value. This is believed to be a side effect of the member being wrapped in an event class, which compiler generated code hides by using a temporary
	''' variable inside this method.
	''' </remarks>
	''' <typeparam name="T">Field type; omit it when using.</typeparam>
	''' <param name="field">Field name to set.</param>
	''' <param name="value">New value.</param>
	''' <param name="prop">Property name in a form Function() PropertyName</param>
	<CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId:="0#")>
	<CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")>
	Protected Sub SetField(Of T)(ByRef field As T, ByVal value As T, ByVal prop As Expression(Of Func(Of T)))
		If (EqualityComparer(Of T).Default.Equals(field, value)) Then Return

		field = value
		RaisePropertyChanged(prop)
	End Sub


	''' <summary>
	''' Get property name from an expression.
	''' </summary>
	''' <typeparam name="T">Type of the field</typeparam>
	''' <param name="prop">Name of the property that has changed in a form Function() PropertyName</param>
	<CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification:="Cannot use lambda 'Function() PropertyName'")>
	<CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")>
	Public Shared Function GetPropertyName(Of T)(prop As Expression(Of Func(Of T))) As String
		If prop Is Nothing Then Throw New ArgumentNullException(NameOf(prop))

		Dim body = CType(prop.Body, MemberExpression)

		If body Is Nothing Then
			Throw New ArgumentException("The body must be a member expression")
		End If

		Return body.Member.Name
	End Function


	''' <summary>
	''' Raise the PropertyChanged event.
	''' </summary>
	''' <param name="propName">Name of the property that has changed</param>
	Protected Sub RaisePropertyChanged(sender As Object, propName As String)
		RaiseEvent PropertyChanged(sender, New PropertyChangedEventArgs(propName))
	End Sub


	''' <summary>
	''' Raise the PropertyChanged event.
	''' </summary>
	''' <param name="propName">Name of the property that has changed</param>
	Protected Sub RaisePropertyChanged(sender As Object, propName As String, propOwner As Object)
		RaiseEvent PropertyChanged(sender, New OwnedPropertyChangedEventArgs(propName, propOwner))
	End Sub


	''' <summary>
	''' Raise the PropertyChanged event.
	''' </summary>
	''' <param name="propName">Name of the property that has changed</param>
	Protected Sub RaisePropertyChanged(propName As String)
		RaisePropertyChanged(Me, propName)
	End Sub


	' ReSharper disable once MemberCanBePrivate.Global

	''' <summary>
	''' Raise the PropertyChanged event.
	''' </summary>
	''' <typeparam name="T">Type of the field</typeparam>
	''' <param name="prop">Name of the property that has changed in a form Function() PropertyName</param>
	<CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")>
	Protected Sub RaisePropertyChanged(Of T)(prop As Expression(Of Func(Of T)))
		RaisePropertyChanged(GetPropertyName(prop))
	End Sub

#End Region


#Region " ICloneable implementation "

	''' <summary>
	''' Create a shallow copy of all XML-serialization-relevant properties.
	''' </summary>
	Public Function Clone() As Object Implements ICloneable.Clone
		Dim copy = Activator.CreateInstance(Me.GetType())
		Dim propList =
			From pi In Me.GetType().GetProperties()
			Where pi.CanRead AndAlso pi.CanWrite AndAlso
			pi.GetCustomAttribute(Of XmlIgnoreAttribute)() Is Nothing

		For Each pi In propList
			Dim value = pi.GetValue(Me)
			pi.SetValue(copy, value)
		Next

		Return copy
	End Function


	''' <summary>
	''' Cast the copy to the given type.
	''' </summary>
	Public Function Clone(Of T)() As T
		Return CType(Clone(), T)
	End Function

#End Region

End Class
