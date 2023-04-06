<Serializable>
Public Class DuplicateChannelNumberException
    Inherits ArgumentException

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    Protected Sub New(serializationInfo As Runtime.Serialization.SerializationInfo, streamingContext As Runtime.Serialization.StreamingContext)
        Throw New NotImplementedException()
    End Sub

End Class
