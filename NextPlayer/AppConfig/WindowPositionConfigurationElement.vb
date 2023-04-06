Imports System.Configuration
Imports Common


Public Class WindowPositionConfigurationElement
	Inherits ConfigurationElement

#Region " Left property "

	<ConfigurationProperty(NameOf(Left))>
	Public Property Left As Double
		Get
			Return CDbl(Me(NameOf(Left)))
		End Get
		Set(value As Double)
			Me(NameOf(Left)) = Math.Round(value)
		End Set
	End Property

#End Region


#Region " Top property "

	<ConfigurationProperty(NameOf(Top))>
	Public Property Top As Double
		Get
			Return CDbl(Me(NameOf(Top)))
		End Get
		Set(value As Double)
			Me(NameOf(Top)) = Math.Round(value)
		End Set
	End Property

#End Region


#Region " Width property "

	<ConfigurationProperty(NameOf(Width), DefaultValue:=680.0)>
	Public Property Width As Double
		Get
			Return CDbl(Me(NameOf(Width)))
		End Get
		Set(value As Double)
			Me(NameOf(Width)) = Math.Round(value)
		End Set
	End Property

#End Region


#Region " Height property "

	<ConfigurationProperty(NameOf(Height), DefaultValue:=800.0)>
	Public Property Height As Double
		Get
			Return CDbl(Me(NameOf(Height)))
		End Get
		Set(value As Double)
			Me(NameOf(Height)) = Math.Round(value)
		End Set
	End Property

#End Region


#Region " Model conversion "

	Public Function ToModel() As WindowPosition
		Return New WindowPosition With {
			.Left = Left, .Top = Top, .Width = Width, .Height = Height
		}
	End Function


	Public Shared Function FromModel(model As WindowPosition) As WindowPositionConfigurationElement
		Return New WindowPositionConfigurationElement With {
			.Left = model.Left, .Top = model.Top, .Width = model.Width, .Height = model.Height
		}
	End Function

#End Region

End Class
