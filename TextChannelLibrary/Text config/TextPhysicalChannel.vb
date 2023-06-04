Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Common


''' <summary>
''' Manages a connection to the physical text output.
''' Could be displayed in a window, sent as SMS, text-to-speech...
''' </summary>
<Serializable>
Public Class TextPhysicalChannel
    Inherits ChannelBase
    Implements IPhysicalChannel

#Region " Constants "

    Private Const DefaultChannelDescription As String = "Text output"

#End Region


#Region " TextInterfaceType notifying property "

    <NonSerialized>
    Private mTextInterfaceType As TypeImplementationInfo


    ''' <summary>
    ''' Used to change the interface.
    ''' </summary>
    <XmlIgnore>
    Public Property TextInterfaceType As TypeImplementationInfo
        Get
            Return mTextInterfaceType
        End Get
        Set(value As TypeImplementationInfo)
            If mTextInterfaceType = value Then Return

            SetField(mTextInterfaceType, value, Function() TextInterfaceType)

            TextInterface = CType(
                Activator.CreateInstance(value.ImplementingType),
                TextOutputInterfaceBase)
        End Set
    End Property

#End Region


#Region " TextInterfaceTypeList shared notifying property "

    ''' <summary>
    ''' Is a Friend to allow unit test inject own player implementation.
    ''' </summary>
    Private Shared ReadOnly sTextInterfaceTypeList As New BindingList(Of TypeImplementationInfo)()


    ''' <summary>
    ''' A collection of available text interface types.
    ''' </summary>
    <XmlIgnore>
    Public Shared Property TextInterfaceTypeList As IList(Of TypeImplementationInfo)
        Get
            Return sTextInterfaceTypeList
        End Get
        Set(value As IList(Of TypeImplementationInfo))
            sTextInterfaceTypeList.Clear()

            For Each item In value
                sTextInterfaceTypeList.Add(item)
            Next
        End Set
    End Property

#End Region


#Region " TextInterface notifying property "

    Private WithEvents mTextInterface As TextOutputInterfaceBase


    ''' <summary>
    ''' The actual text interface behind the channel.
    ''' </summary>
    Public Property TextInterface As TextOutputInterfaceBase
        Get
            Return mTextInterface
        End Get
        Set(value As TextOutputInterfaceBase)
            ' SetField is not sufficient here, as the property is not updated upon its RaisePropertyChanged
            If value.Equals(mTextInterface) Then Return

            mTextInterface = value
            RaisePropertyChanged(NameOf(TextInterface))

            Dim implInfo = TextInterfaceTypeList.FirstOrDefault(
                Function(m) m.ImplementingType = value.GetType())

            If implInfo Is Nothing Then
                implInfo = New TypeImplementationInfo With {
                    .Name = "Undefined " & value.GetType().Name,
                    .ImplementingType = value.GetType()
                    }
            End If

            TextInterfaceType = implInfo
        End Set
    End Property


    Private Sub TextInterfacePropertyChangedHandler(sender As Object, args As PropertyChangedEventArgs) Handles mTextInterface.PropertyChanged
        Dim p = mTextInterface.GetType().GetProperty(args.PropertyName)
        ' Ignore read-only properties
        If Not p.CanWrite Then Return

        RaisePropertyChanged(NameOf(TextInterface))
    End Sub



#End Region


#Region " Init and clean-up "

    Shared Sub New()
        CollectImplementations(GetType(ITextOutputInterface), sTextInterfaceTypeList)
    End Sub


    Public Sub New()
        Description = DefaultChannelDescription

        Dim defType = InterfaceMapper.GetImplementingType(Of ITextOutputInterface)()

        Dim ifType = TextInterfaceTypeList.SingleOrDefault(
            Function(info) info.ImplementingType = defType)

        If ifType Is Nothing Then
            Throw New ArgumentException($"Interface type '{defType.Name}' is not found in the list of available implementations.")
        End If

        TextInterfaceType = ifType
    End Sub

#End Region


    Public Sub SendText(text As String)
        mTextInterface?.SendText(text)
    End Sub

End Class
