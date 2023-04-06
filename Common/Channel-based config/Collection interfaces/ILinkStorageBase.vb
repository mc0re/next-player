Imports System.ComponentModel
Imports System.Xml.Serialization


''' <summary>
''' This is a non-generic base interface to use in XAML-related code.
''' </summary>
<CLSCompliant(True)>
Public Interface ILinkStorageBase
	Inherits INotifyPropertyChanged

	''' <summary>
	''' A collection of link objects for this machine.
	''' </summary>
	<XmlIgnore>
	ReadOnly Property Links As IChannelLinkCollectionBase

End Interface
