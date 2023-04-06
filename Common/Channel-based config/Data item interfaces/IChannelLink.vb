Imports System.ComponentModel


''' <summary>
''' Defines a link between a logical and a physical channels.
''' A part of M-to-M relationship.
''' </summary>
''' <remarks>
''' Links are stored in the machine-specific part
''' of the playlist.
''' </remarks>
<CLSCompliant(True)>
Public Interface IChannelLink
	Inherits INotifyPropertyChanged

	''' <summary>
	''' Channel number of a connected logical channel.
	''' </summary>
	Property Logical As Integer


	''' <summary>
	''' Channel number of a connected physical channel.
	''' </summary>
	Property Physical As Integer


	''' <summary>
	''' Whether the link is in use.
	''' </summary>
	Property IsEnabled As Boolean


	''' <summary>
	''' Use this to ensure proper initialization after
	''' creating or loading each link.
	''' </summary>
	Sub AfterLoad()


	''' <summary>
	''' Create a copy of the link.
	''' </summary>
	Function Clone(Of T As IChannelLink)() As T

End Interface
