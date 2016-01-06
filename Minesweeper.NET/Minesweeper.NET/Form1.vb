Imports System.Threading

Public Class Form1

    Public thread As New Thread(AddressOf gameloop)
    Public vbgame As New VBGame

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        vbgame.setDisplay(Me, "1280x720")
    End Sub

    Sub gameloop()
        Dim run As Boolean = True
        While run



        End While
    End Sub

End Class
