Imports System.Diagnostics.CodeAnalysis


<CLSCompliant(True)>
<Serializable>
Public Class Point2D(Of TRef)
    Implements IPoint2D, IRefKeeper(Of TRef)

#Region " Constants "

    Public Shared ReadOnly Property Origin As IPoint2D = New Point2D(Of NoRef)(0, 0, NoRef.Empty)

#End Region


#Region " Properties "

    Public ReadOnly Property X As Double Implements IPoint2D.X

    Public ReadOnly Property Y As Double Implements IPoint2D.Y


    Private ReadOnly mReferences As New List(Of TRef)()


    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Friend Sub New(x As Double, y As Double, references As IEnumerable(Of TRef))
        Me.X = x
        Me.Y = y
        mReferences.AddRange(references)
    End Sub


    Friend Sub New(orig As IPoint2D, references As IEnumerable(Of TRef))
        X = orig.X
        Y = orig.Y
        mReferences.AddRange(references)
    End Sub

#End Region


#Region " API "

    Public Function Contains(p As IPoint2D) As Boolean Implements IObject2D.Contains
        Return Point2DHelper.IsSame(Me, p)
    End Function


    Public Sub AddExtraReferences(refList As IReadOnlyCollection(Of TRef))
        mReferences.AddRange(refList)
    End Sub

#End Region


#Region " ToString "

    ''' <summary>
    ''' For debugging.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return $"({X:F2}, {Y:F2}) / {ReferencesHelper.AsString(References)}"
    End Function

#End Region

End Class
