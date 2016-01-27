Imports System.Threading

Public Class Form1

    Public thread As New Thread(AddressOf gameloop)
    Public vbgame As New VBGame

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        vbgame.setDisplay(Me, "1280x720")
        thread.Start()

    End Sub

    Sub gameloop()
        Dim run As Boolean = True
        Dim side As Integer = 20
        Dim mines As Integer = 400
        vbgame.fill(Color.FromArgb(150, 150, 150))
        Dim minegrid As New MineGrid(vbgame, side, mines)

        numbers.generate(side)

        While run

            For Each e In vbgame.getKeyDownEvents()
                If e = "R" Then
                    minegrid = New MineGrid(vbgame, side, mines)
                End If
                ElseIf e = "C" Then
                    For Each Cell In minegrid.cells
                        Cell.dug = True
                    Next
                End If
            Next

            vbgame.fill(Color.FromArgb(150, 150, 150))

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
