Public NotInheritable Class WaitCursor
    Implements IDisposable

#Region " Fields "

    Private mPreviousCursor As Cursor

#End Region


#Region " Init and clean-up "

    Public Sub New()
        mPreviousCursor = Mouse.OverrideCursor
        Mouse.OverrideCursor = Cursors.Wait
    End Sub

#End Region


#Region " IDisposable implementation "

    Public Sub Dispose() Implements IDisposable.Dispose
        Mouse.OverrideCursor = mPreviousCursor
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
