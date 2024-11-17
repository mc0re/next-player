Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports Common


Public Module DisplayUtility

    ''' <summary>
    ''' Modifies <paramref name="pos"/>
    ''' </summary>
    <Extension()>
    Public Sub LimitByDisplay(pos As WindowPosition)
        Dim bnd = Screen.AllScreens(0).Bounds
        Dim left = bnd.Left
        Dim right = bnd.Right
        Dim top = bnd.Top
        Dim bottom = bnd.Bottom

        For Each scr In Screen.AllScreens.Skip(1)
            If scr.Bounds.Left < left Then left = scr.Bounds.Left
            If scr.Bounds.Right > right Then right = scr.Bounds.Right
            If scr.Bounds.Top < top Then top = scr.Bounds.Top
            If scr.Bounds.Bottom < bottom Then bottom = scr.Bounds.Bottom
        Next

        If pos.Left < left Then pos.Left = left
        If pos.Top < top Then pos.Top = top
        If pos.Left + pos.Width > right Then pos.Left = right - pos.Width
        If pos.Top + pos.Height > bottom Then pos.Top = bottom - pos.Top
    End Sub

End Module
