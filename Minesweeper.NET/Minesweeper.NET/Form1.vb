Imports System.Threading

Public Class Form1

    Public thread As New Thread(AddressOf mainloop)
    Public vbgame As New VBGame

    Dim side As Integer = 20
    Dim mines As Integer = 99
    Dim gridwidth As Integer = 30
    Dim gridheight As Integer = 16

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        vbgame.setDisplay(Me, gridwidth * side & "x" & gridheight * side, "Minesweeper.NET")
        thread.Start()
    End Sub

    Sub custom()
        'width = InputBox("Grid width?")
        'height = InputBox("Grid Height?")
        mines = InputBox("Mines?")
        'vbgame.setDisplay(Me, width * side & "x" & height * side, "Minesweeper.NET")
    End Sub

    Sub mainloop()
        Dim run As Boolean = True

        Dim start As New Button(vbgame, "Start", New Rectangle(10, 10, 50, 20))
        start.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        start.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim customize As New Button(vbgame, "Customize", New Rectangle(10, 40, 50, 15))
        customize.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        customize.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim quit As New Button(vbgame, "Quit", New Rectangle(70, 10, 50, 20))
        quit.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        quit.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        While run

            For Each e In vbgame.getMouseEvents()

                If start.handle(e) = MouseEvent.ButtonLeft Then
                    gameloop()
                ElseIf customize.handle(e) = MouseEvent.ButtonLeft Then
                    custom()
                ElseIf quit.handle(e) = MouseEvent.ButtonLeft Then
                    End
                End If

            Next
            vbgame.fill(vbgame.black)

            start.draw()
            customize.draw()
            quit.draw()

            vbgame.update()

        End While

    End Sub

    Function gameloop()
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
                ElseIf e = "Escape" Then
                    Return "escape"
                End If
            Next

            For Each e In vbgame.getMouseEvents()
                If e.action = MouseEvent.MouseDown Then
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
        Return Nothing
    End Function

End Class
