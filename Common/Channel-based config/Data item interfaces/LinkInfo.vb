''' <summary>
''' Helper class for return values.
''' </summary>
<CLSCompliant(True)>
Public Class LinkInfo(Of TPhys As IPhysicalChannel, TLink As IChannelLink)

	Public Property Physical As TPhys

	Public Property Link As TLink

End Class
