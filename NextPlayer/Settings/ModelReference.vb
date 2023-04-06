Imports System.Collections.Concurrent
Imports System.Windows.Media.Media3D


Public Enum ModelledObjectTypes
    Room
    Audience
    DividingPolyhedrons
    AllSpeakers
    SelectedPolyhedron
    SelectedSpeakers
End Enum


Public Class ModelReferenceItem

    Public Type As ModelledObjectTypes

    Public Model As Visual3D

    Public Reference As Object

End Class


Public Class ModelReferenceList

#Region " Fields "

    Private mStore As New ConcurrentDictionary(Of ModelledObjectTypes, ModelReferenceItem)()

#End Region


#Region " API "

    Public Sub SetModel(type As ModelledObjectTypes, model As Visual3D, ref As Object)
        Dim entry As New ModelReferenceItem With {.Type = type, .Model = model, .Reference = ref}
        mStore.AddOrUpdate(type, entry, Function(k, m) entry)
    End Sub


    Public Function GetModel(type As ModelledObjectTypes) As ModelReferenceItem
        Dim res As New ModelReferenceItem()
        mStore.TryGetValue(type, res)
        Return res
    End Function

#End Region

End Class
