Imports System.ComponentModel
Imports System.Reflection


Public Module TypeUtility

	''' <summary>
	''' Collect all classes, inheriting or implementing <paramref name="baseType"/>.
	''' If a class is marked with <see cref="DescriptionAttribute"/>,
	''' use its value as <see cref="TypeImplementationInfo.Name"/>,
	''' otherwise use type name.
	''' </summary>
	''' <remarks>
	''' Only instantiatable types are returned, not abstract ones or interfaces.
	''' Only the assembly where the <paramref name="baseType"/> is defined is checked.
	''' </remarks>
	''' <param name="baseType">Base type to check for</param>
	''' <param name="output">A list to append to</param>
	Public Sub CollectImplementations(baseType As Type, output As IList(Of TypeImplementationInfo))
		Dim res = (
			From typ In baseType.Assembly.GetTypes()
			Where baseType.IsAssignableFrom(typ) AndAlso Not typ.IsInterface() AndAlso Not typ.IsAbstract()
			Let nameAttr = typ.GetCustomAttribute(Of DescriptionAttribute)()
			Let name = If(nameAttr Is Nothing, typ.Name, nameAttr.Description)
			Order By name
			Select New TypeImplementationInfo With {.ImplementingType = typ, .Name = name}
		).ToList()

		For Each info In res
			output.Add(info)
		Next
	End Sub

End Module
