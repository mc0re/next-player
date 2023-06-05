Imports Common
Imports System.Xml.Serialization


''' <summary>
''' This class is to overcome problems when serializing interfaces.
''' </summary>
<CLSCompliant(True)>
<Serializable>
<XmlInclude(GetType(RenderTextInterface))>
Public MustInherit Class TextOutputInterfaceBase
    Inherits PropertyChangedHelper
    Implements ITextOutputInterface

    Protected mPhysicalChannel As TextPhysicalChannel


    Sub SetChannel(physicalChannel As TextPhysicalChannel)
        mPhysicalChannel = physicalChannel
    End Sub


    ''' <inheritdoc/>
    Public MustOverride Sub SendText(text As String) Implements ITextOutputInterface.SendText


    ''' <inheritdoc/>
    Public MustOverride Sub SetPosition(position As Double) Implements ITextOutputInterface.SetPosition

End Class
