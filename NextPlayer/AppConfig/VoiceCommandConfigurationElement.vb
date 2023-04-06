Imports System.Configuration
Imports Common


''' <summary>
''' A set of settings, which depend on the environment.
''' </summary>
Public Class VoiceCommandConfigurationElement
	Inherits ConfigurationElement

#Region " Name property "

	<ConfigurationProperty(NameOf(Name), IsKey:=True)>
	Public Property Name As String
		Get
			Return CStr(Me(NameOf(Name)))
		End Get
		Set(value As String)
			Me(NameOf(Name)) = value
		End Set
	End Property

#End Region


#Region " IsEnabled property "

	<ConfigurationProperty(NameOf(IsEnabled))>
	Public Property IsEnabled As Boolean
		Get
			Return CBool(Me(NameOf(IsEnabled)))
		End Get
		Set(value As Boolean)
			Me(NameOf(IsEnabled)) = value
		End Set
	End Property

#End Region


#Region " RecognitionText property "

	<ConfigurationProperty(NameOf(RecognitionText))>
	Public Property RecognitionText As String
		Get
			Return CStr(Me(NameOf(RecognitionText)))
		End Get
		Set(value As String)
			Me(NameOf(RecognitionText)) = value
		End Set
	End Property

#End Region


#Region " Model conversion "

	Public Function ToModel() As VoiceCommandConfigItem
		Return New VoiceCommandConfigItem With {
			.CommandName = Name, .IsEnabled = IsEnabled, .RecognitionText = RecognitionText
		}
	End Function


	Public Shared Function FromModel(model As VoiceCommandConfigItem) As VoiceCommandConfigurationElement
		Return New VoiceCommandConfigurationElement With {
			.Name = model.CommandName, .IsEnabled = model.IsEnabled, .RecognitionText = model.RecognitionText
		}
	End Function

#End Region

End Class
