Imports Common
Imports System.Xml.Serialization


'<XmlInclude(GetType(ShowMessageTextOutputInterface))>
''' <summary>
''' This class is to overcome problems when serializing interfaces.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public MustInherit Class TextOutputInterfaceBase
    Inherits PropertyChangedHelper
    Implements ITextOutputInterface

    Public MustOverride Sub SendText(text As String) Implements ITextOutputInterface.SendText

End Class
