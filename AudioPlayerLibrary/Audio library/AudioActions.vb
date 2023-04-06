Public Class AudioActions

#Region " Properties "

    Public Property StartingProducers As New List(Of ISoundProducer)()

    Public Property StartingNonProducers As New List(Of IPlayerAction)()

    Public Property Pausing As New List(Of IPlayerAction)()

    Public Property Stopping As New List(Of IPlayerAction)()

    Public Property Resuming As New List(Of IPlayerAction)()

#End Region


#Region " API "

    Public Sub Merge(other As AudioActions)
        StartingProducers.AddRange(other.StartingProducers)
        StartingNonProducers.AddRange(other.StartingNonProducers)
        Pausing.AddRange(other.Pausing)
        Stopping.AddRange(other.Stopping)
        Resuming.AddRange(other.Resuming)
    End Sub


    Public Function IsEmpty() As Boolean
        Dim hasAny = StartingProducers.Any() OrElse
            StartingNonProducers.Any() OrElse
            Pausing.Any() OrElse
            Stopping.Any() OrElse
            Resuming.Any()

        Return Not hasAny
    End Function


    Public Function GetStartingActions() As ICollection(Of IPlayerAction)
        Return StartingNonProducers.Concat(
               StartingProducers).Concat(
               Resuming).ToList()
    End Function

#End Region

End Class
