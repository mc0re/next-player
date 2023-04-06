Imports System.Reflection


Public Module EnumUtility

	Public Function GetEnumAttribute(Of TAttr As Attribute)(enumValue As [Enum]) As TAttr
		Dim eval = enumValue.GetType().GetMember(enumValue.ToString()).
			FirstOrDefault(Function(member) member.MemberType = MemberTypes.Field)

		If eval Is Nothing Then Return Nothing

		Dim attr = eval.GetCustomAttributes(GetType(TAttr), False).Cast(Of TAttr)().SingleOrDefault()

		If attr Is Nothing Then Return Nothing

		Return attr
	End Function

End Module
