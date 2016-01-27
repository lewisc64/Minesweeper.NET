Imports System.Threading

Public Class Form1

    Public thread As New Thread(AddressOf gameloop)
    Public vbgame As New VBGame

    Dim side As Integer = 20
    Dim mines As Integer = 400

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim width, height As Integer
        'width = InputBox("Grid width?")
        'height = InputBox("Grid Height?")
        'mines = InputBox("Mines?")
        width = 30
        height = 16
        mines = 99
        vbgame.setDisplay(Me, width * side & "x" & height * side, "Minesweeper.NET")
        'vbgame.setDisplay(Me, "800x800", "Minesweeper.NET")
        thread.Start()

    End Sub

    Sub gameloop()
        Dim run As Boolean = True
        vbgame.fill(Color.FromArgb(150, 150, 150))
        Dim minegrid As New MineGrid(vbgame, side, mines)

        numbers.generate(side)

        While run

            For Each e In vbgame.getKeyDownEvents()
                If e = "R" Then
                    minegrid = New MineGrid(vbgame, side, mines)
                ElseIf e = "C" Then
                    For Each Cell In minegrid.cells
                        Cell.dug = True
                    Next
                End If
            Next

            For Each e In vbgame.getMouseEvents()
                If e.action = MouseEvent.MouseDown Then
                    Console.WriteLine(e.button)
                    minegrid.handleCells(e)
                End If
            Next

            minegrid.drawCells()

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
