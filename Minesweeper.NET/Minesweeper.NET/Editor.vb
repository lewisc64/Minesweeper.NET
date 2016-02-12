Imports System.Threading

Public Class Editor

    Private thread As New Thread(AddressOf mainloop)
    Private vbgame As New VBGame

    Dim side As Integer = 20
    Dim mines As Integer = 99
    Dim gridwidth As Integer = 30
    Dim gridheight As Integer = 16

    Private Sub Editor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        adjustSize()
    End Sub

    Sub adjustSize()
        vbgame.setDisplay(Me, New Size(Math.Max(gridwidth * side, 200), Math.Max(gridheight * side + 20, 140)), "Minesweeper.NET Editor")
    End Sub

    Private Sub mainloop()

        Dim run As Boolean = True

        While run

        End While

    End Sub

End Class