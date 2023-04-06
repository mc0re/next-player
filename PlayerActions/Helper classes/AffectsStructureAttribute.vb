''' <summary>
''' Set this attribute on PlayerAction properties,
''' that affect structural tree.
''' </summary>
<AttributeUsage(AttributeTargets.Property, Inherited:=True)>
Public NotInheritable Class AffectsStructureAttribute
	Inherits Attribute

End Class
