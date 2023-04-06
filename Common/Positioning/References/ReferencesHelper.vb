Imports System.Diagnostics.CodeAnalysis

Public Class ReferencesHelper

#Region " Reference API "

    ''' <summary>
    ''' Create a new list with all references from the given reference keepers.
    ''' </summary>
    Public Shared Function MergeReferences(Of TRef)(
        refKeepers As IEnumerable(Of IRefKeeper(Of TRef))
    ) As List(Of TRef)
        Dim res As New List(Of TRef)()

        For Each keeper In refKeepers
            For Each ref In keeper.References
                If Not res.Contains(ref) Then
                    res.Add(ref)
                End If
            Next
        Next

        Return res
    End Function


    ''' <summary>
    ''' String representation of the reference list.
    ''' Used for debugging only.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Shared Function AsString(Of TRef)(references As IReadOnlyCollection(Of TRef)) As Object
        If references.Any() Then
            Return $"{{{String.Join(", ", references)}}}"
        Else
            Return "-"
        End If
    End Function

#End Region

End Class
