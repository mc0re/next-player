Imports System.Diagnostics.CodeAnalysis


<CLSCompliant(True)>
Public Class Border3D(Of TRef)
    Implements IBorder3D, IRefKeeper(Of TRef)

#Region " Properties "

    Public ReadOnly Property Plane As IPlane3D Implements IBorder3D.Plane

    Public ReadOnly Property Inside As Integer Implements IBorder3D.Inside

#End Region


#Region " IRefKeeper properties "

    Private mReferences As New List(Of TRef)


    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " Init and clean- up "

    Public Sub New(plane As IPlane3D, inside As Integer, refList As IEnumerable(Of TRef))
        Me.Plane = plane
        Me.Inside = inside
        mReferences = refList.ToList()
    End Sub

#End Region


#Region " ToString "

    ''' <summary>
    ''' Debugging.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return $"{Plane}, inside {Inside}"
    End Function

#End Region

End Class
