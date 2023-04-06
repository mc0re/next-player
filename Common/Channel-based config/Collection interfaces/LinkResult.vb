''' <summary>
''' A helper class to store the pair of values.
''' More explicit than a Tuple.
''' </summary>
<CLSCompliant(True)>
Public NotInheritable Class LinkResult(Of TLink As IChannelLink, TPhys As IPhysicalChannel)

	Public Property Link As TLink

	Public Property Physical As TPhys

End Class
