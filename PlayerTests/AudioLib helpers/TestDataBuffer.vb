Imports System.Collections.ObjectModel


Public Class TestDataBuffer

#Region " Fields "

    Private ReadOnly mData As List(Of Single)

#End Region


#Region " Properties "

    Public ReadOnly Property Data As ReadOnlyCollection(Of Single)
        Get
            Return New ReadOnlyCollection(Of Single)(mData)
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New()
        mData = New List(Of Single)()
    End Sub


    Public Sub New(def As String)
        mData = ParseDefinition(def)
    End Sub

#End Region


#Region " API "

    Public Sub Write(moreData As IEnumerable(Of Single))
        mData.AddRange(moreData)
    End Sub

#End Region


#Region " Utility "

    Private Shared Function ParseDefinition(sampleDef As String) As List(Of Single)
        Dim res As New List(Of Single)()

        For Each part In sampleDef.Split(";"c)
            If String.IsNullOrEmpty(part) Then
                Continue For
            End If

            Dim multiple = part.Split("x"c)

            Dim seed = (
                    From d In multiple(0).Split(","c)
                    Select Single.Parse(d)
                    ).ToList()

            Dim count = 1
            For Each mult In multiple.Skip(1)
                count *= Integer.Parse(mult)
            Next

            For idx = 1 To count
                res.AddRange(seed)
            Next
        Next

        Return res
    End Function

#End Region

End Class
