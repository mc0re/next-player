Imports System.Reflection
Imports Common
Imports System.Linq.Expressions


''' <summary>
''' Base class for copying properties for report.
''' </summary>
Public MustInherit Class ReportDataMirror(Of TSource)

#Region " Mapping non-copy properties "

	Private Shared ReadOnly PropertyMapList As New Dictionary(Of String, Func(Of TSource, Object))


	''' <summary>
	''' Add a mapper for the given target property.
	''' </summary>
	''' <param name="prop">An expression refering to the destination property</param>
	''' <param name="mapper">An action returning a new value of the destination property based on the source object</param>
	Protected Shared Sub AddPropertyMap(Of TDest)(prop As Expression(Of Func(Of TDest)), mapper As Func(Of TSource, Object))
		Dim propName = PropertyChangedHelper.GetPropertyName(prop)
		PropertyMapList.Add(propName, mapper)
	End Sub

#End Region


	Protected Sub CopyFields(source As TSource)
		For Each targetPropInfo In Me.GetType().GetProperties()
			Dim mapper As Func(Of TSource, Object) = Nothing
			If PropertyMapList.TryGetValue(targetPropInfo.Name, mapper) Then
				' There exists a mapper, use it and override all other settings
				targetPropInfo.SetValue(Me, mapper(source))
				Continue For
			End If

			' If the property is not found, it is probably because it is used
			' on a type from another branch of the hierarchy. Ignore.
			Dim sourcePropInfo = source.GetType().GetProperty(targetPropInfo.Name)
			If sourcePropInfo Is Nothing Then Continue For

			If sourcePropInfo.GetCustomAttribute(Of IgnoreForReportAttribute)() IsNot Nothing Then
				' Ignored property
				Continue For
			End If

			Dim newValue = sourcePropInfo.GetValue(source)
			If sourcePropInfo.PropertyType.IsEnum Then
				' Enums are represented as string values
				newValue = newValue.ToString()
			End If

			Try
				targetPropInfo.SetValue(Me, newValue)
			Catch ex As ArgumentException
				Debug.Fail(String.Format("Target type for property {0} must be {1}",
										 targetPropInfo.Name, sourcePropInfo.PropertyType.ToString()))
			End Try
		Next
	End Sub

End Class
