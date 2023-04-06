''' <summary>
''' A result of finding implementation types.
''' </summary>
<CLSCompliant(True)>
Public Class TypeImplementationInfo

#Region " Properties "

	Public Property Name As String

	Public Property ImplementingType As Type

#End Region


#Region " Equals "

	Public Overrides Function Equals(obj As Object) As Boolean
		Dim other = TryCast(obj, TypeImplementationInfo)
		If other Is Nothing Then Return False

		Return Name = other.Name
	End Function


	Public Shared Operator =(obj1 As TypeImplementationInfo, obj2 As TypeImplementationInfo) As Boolean
		If obj1 Is Nothing Then Return obj2 Is Nothing
		Return obj1.Equals(obj2)
	End Operator


	Public Shared Operator <>(obj1 As TypeImplementationInfo, obj2 As TypeImplementationInfo) As Boolean
		Return Not obj1 = obj2
	End Operator

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Return $"{Name}: {ImplementingType.Name}"
	End Function

#End Region

End Class
