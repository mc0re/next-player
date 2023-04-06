''' <summary>
''' Set this attribute on PlayerAction properties,
''' that affect automatic triggers.
''' </summary>
<AttributeUsage(AttributeTargets.Property, Inherited:=True)>
Public NotInheritable Class AffectsTriggersAttribute
	Inherits Attribute

End Class
