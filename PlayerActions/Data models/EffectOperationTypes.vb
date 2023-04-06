Public Enum EffectOperationTypes

	''' <summary>
	''' All previous effects are bypassed.
	''' </summary>
	Bypass

	''' <summary>
	''' All previous effects are ignored,
	''' the volume is set to the effect's value.
	''' </summary>
	Assign

	''' <summary>
	''' All previous effects are ignored,
	''' the volume is multiplied by the effect's value.
	''' </summary>
	Multiply

	''' <summary>
	''' The volume after all previous effects
	''' is multiplied by the effect's value.
	''' </summary>
	ChainMultiply

End Enum
