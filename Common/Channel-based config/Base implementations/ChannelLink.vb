''' <summary>
''' Defines a connection between logical and physical channels.
''' </summary>
<Serializable>
<CLSCompliant(True)>
Public MustInherit Class ChannelLink
	Inherits PropertyChangedHelper
	Implements IChannelLink

#Region " IsEnabled notifying property "

	Private mIsEnabled As Boolean = True


	Public Property IsEnabled As Boolean Implements IChannelLink.IsEnabled
		Get
			Return mIsEnabled
		End Get
		Set(value As Boolean)
			SetField(mIsEnabled, value, NameOf(IsEnabled))
		End Set
	End Property

#End Region


#Region " Logical notifying property "

	Private mLogical As Integer


	Public Property Logical As Integer Implements IChannelLink.Logical
		Get
			Return mLogical
		End Get
		Set(value As Integer)
			SetField(mLogical, value, NameOf(Logical))
		End Set
	End Property

#End Region


#Region " Physical notifying property "

	Private mPhysical As Integer


	Public Property Physical As Integer Implements IChannelLink.Physical
		Get
			Return mPhysical
		End Get
		Set(value As Integer)
			SetField(mPhysical, value, NameOf(Physical))
		End Set
	End Property

#End Region


#Region " ChannelLink overrides "

	''' <summary>
	''' Override this to ensure proper initialization after
	''' creating or loading each link.
	''' </summary>
	Public MustOverride Sub AfterLoad() Implements IChannelLink.AfterLoad

#End Region


#Region " IChannelLink methods "

	Public Shadows Function Clone(Of T As IChannelLink)() As T Implements IChannelLink.Clone
		Return MyBase.Clone(Of T)()
	End Function

#End Region

End Class
