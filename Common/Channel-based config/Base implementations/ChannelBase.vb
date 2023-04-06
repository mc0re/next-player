Imports System.ComponentModel
Imports System.Xml.Serialization


''' <summary>
''' Stores a single channel definition - channel number, description, etc.
''' </summary>
<Serializable>
<CLSCompliant(True)>
Public MustInherit Class ChannelBase
	Inherits PropertyChangedHelper
	Implements IChannel

#Region " IChannel properties "

#Region " Channel notifying property "

	Private mChannel As Integer = 1


	<Category("Appearance"), Description("Output channel number (1..)")>
	Public Property Channel As Integer Implements IChannel.Channel
		Get
			Return mChannel
		End Get
		Set(value As Integer)
			SetField(mChannel, value, NameOf(Channel))
		End Set
	End Property

#End Region


#Region " Description notifying property "

	Private mDescription As String = "New channel"


	<Category("Appearance"), Description("Output channel short description")>
	Public Property Description As String Implements IChannel.Description
		Get
			Return mDescription
		End Get
		Set(value As String)
			SetField(mDescription, value, NameOf(Description))
		End Set
	End Property

#End Region


#Region " IsEnabled notifying property "

	Private mIsEnabled As Boolean = True


	<Category("Appearance"), Description("Whether the channel is enabled")>
	Public Property IsEnabled As Boolean Implements IChannel.IsEnabled
		Get
			Return mIsEnabled
		End Get
		Set(value As Boolean)
			SetField(mIsEnabled, value, NameOf(IsEnabled))
		End Set
	End Property

#End Region


#Region " IsActive non-persistant notifying property "

	Private mIsActive As Boolean


	<XmlIgnore>
	<Category("Common Properties"), Description("Whether the channel is active (showing, playing, etc)")>
	Public Property IsActive As Boolean Implements IChannel.IsActive
		Get
			Return mIsActive
		End Get
		Set(value As Boolean)
			SetField(mIsActive, value, NameOf(IsActive))
		End Set
	End Property

#End Region

#End Region


#Region " IChannel methods "

	Public Shadows Function Clone(Of T As IChannel)() As T Implements IChannel.Clone
		Return MyBase.Clone(Of T)()
	End Function

#End Region


#Region " Equals overrides "

	Public Overrides Function Equals(obj As Object) As Boolean
		Dim other = TryCast(obj, IChannel)
		If other Is Nothing Then Return False

		Return Channel = other.Channel AndAlso Description = other.Description
	End Function


	Public Overrides Function GetHashCode() As Integer
		Return Channel.GetHashCode() Xor Description.GetHashCode()
	End Function

#End Region


#Region " ToString override "

	Public Overrides Function ToString() As String
		Return String.Format("Channel {0}: {1}", Channel, Description)
	End Function

#End Region

End Class
