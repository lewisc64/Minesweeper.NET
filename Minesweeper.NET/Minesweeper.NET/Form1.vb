Imports System.Threading

Public Class Form1

    Public thread As New Thread(AddressOf gameloop)
    Public vbgame As New VBGame

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        vbgame.setDisplay(Me, "1280x720")
        thread.Start()

        numbers.images.Add(-1, vbgame.getImage("-1.png"))
        numbers.images.Add(1, vbgame.getImage("1.png"))
        numbers.images.Add(2, vbgame.getImage("2.png"))
        numbers.images.Add(3, vbgame.getImage("3.png"))
        numbers.images.Add(4, vbgame.getImage("4.png"))
        numbers.images.Add(5, vbgame.getImage("5.png"))
        numbers.images.Add(6, vbgame.getImage("6.png"))
        numbers.images.Add(7, vbgame.getImage("7.png"))
        numbers.images.Add(8, vbgame.getImage("8.png"))

    End Sub

    Sub gameloop()
        Dim run As Boolean = True
        vbgame.fill(Color.FromArgb(150, 150, 150))
        Dim minegrid As New MineGrid(vbgame, 20, 300)
        While run

            vbgame.fill(Color.FromArgb(150, 150, 150))

            For Each e In vbgame.getKeyDownEvents()
                If e = "R" Then
                    minegrid = New MineGrid(vbgame, 20, 300)
                End If
            Next

            minegrid.handleCells()

            For Each effect In cross.crosses.ToList()
                If effect.opacity = 0 Then
                    cross.crosses.Remove(effect)
                End If
                effect.handle()
            Next

            vbgame.update()
            vbgame.clockTick(30)

        End While
    End Sub

End Class
