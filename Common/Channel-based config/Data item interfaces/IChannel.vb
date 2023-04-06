Imports System.ComponentModel


''' <summary>
''' Necessary minimal data for a channel-based configuration item,
''' covering both logical and physical channels.
''' </summary>
<CLSCompliant(True)>
Public Interface IChannel
	Inherits INotifyPropertyChanged

	''' <summary>
	''' Channel number, 1-...
	''' </summary>
	Property Channel As Integer


	''' <summary>
	''' Channel description.
	''' </summary>
	''' <returns></returns>
	Property Description As String


	''' <summary>
	''' Whether the channel usage is enabled.
	''' </summary>
	Property IsEnabled As Boolean


	''' <summary>
	''' Whether the channel is currently active.
	''' </summary>
	ReadOnly Property IsActive As Boolean


	Function Clone(Of T As IChannel)() As T

End Interface
