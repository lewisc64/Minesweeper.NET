Imports System.Threading

Public Class Form1

    Public thread As New Thread(AddressOf mainloop)
    Public vbgame As New VBGame

    Dim side As Integer = 20
    Dim mines As Integer = 99
    Dim gridwidth As Integer = 30
    Dim gridheight As Integer = 16

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        vbgame.setDisplay(Me, New Size(gridwidth * side, gridheight * side + 20), "Minesweeper.NET")
        numbers.generate(side)
        thread.Start()
    End Sub

    Sub custom()
        Try
            gridwidth = InputBox("Grid width?")
            gridheight = InputBox("Grid Height?")
            mines = InputBox("Mines?")
        Catch ex As Exception
            gridwidth = 30
            gridheight = 16
            mines = 99
        End Try
        vbgame.setDisplay(Me, New Size(gridwidth * side, gridheight * side + 20), "Minesweeper.NET")
    End Sub

    Function getPreMineGrid(Optional preminegrid As MineGrid = Nothing)
        If IsNothing(preminegrid) Then
            preminegrid = New MineGrid(vbgame, side, gridwidth, gridheight, mines)
        End If
        For Each Cell In preminegrid.cells
            Cell.dug = True
            Cell.opacity = 0
        Next
        Return preminegrid
    End Function

    Sub mainloop()
        Dim run As Boolean = True

        Dim preminegrid As MineGrid = getPreMineGrid()
        Dim outcome As outcome

        Dim start As New Button(vbgame, "Start", New Rectangle(10, 10, 50, 20), "Arial Black")
        start.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        start.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim customize As New Button(vbgame, "Custom", New Rectangle(10, 40, 50, 15), "Arial Black")
        customize.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        customize.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim quit As New Button(vbgame, "Quit", New Rectangle(70, 10, 50, 20), "Arial Black")
        quit.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        quit.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        While run

            For Each e In vbgame.getKeyDownEvents()
                If e = "R" Then
                    preminegrid = getPreMineGrid()
                ElseIf e = "Escape" Then
                    End
                End If
            Next

            For Each e In vbgame.getMouseEvents()

                If start.handle(e) = MouseEvent.ButtonLeft Then
                    outcome = gameloop()
                    preminegrid = getPreMineGrid(outcome.minegrid)
                ElseIf customize.handle(e) = MouseEvent.ButtonLeft Then
                    custom()
                    preminegrid = getPreMineGrid()
                ElseIf customize.handle(e) = MouseEvent.ButtonRight Then
                    MsgBox(gridwidth & "x" & gridheight & vbCrLf & mines & " mines.")
                ElseIf quit.handle(e) = MouseEvent.ButtonLeft Then
                    End
                End If

            Next
            preminegrid.drawCells()

            start.draw()
            customize.draw()
            quit.draw()

            vbgame.update()

        End While

    End Sub

    Class outcome
        Public action As String
        Public minegrid As MineGrid
        Public timer As Stopwatch
        Public Sub New(actiont As String, minegridt As MineGrid, timert As Stopwatch)
            timert.Stop()
            timer = timert
            action = actiont
            minegrid = minegridt
        End Sub
    End Class

    Sub drawInfo(timer As Stopwatch, minegrid As MineGrid)
        Dim tx As Integer
        Dim s As String
        vbgame.drawText(New Point(0, gridheight * side), Math.Round(timer.ElapsedMilliseconds() / 1000, 2) & "s", vbgame.white, 10, "Arial Black")

        tx = vbgame.width / 2
        s = minegrid.flags
        vbgame.drawText(New Point(tx, gridheight * side), s, vbgame.red, 10, "Arial Black")

        tx += vbgame.displaybuffer.Graphics.MeasureString(s, New Font("Arial Black", 10)).Width
        s = "/"
        vbgame.drawText(New Point(tx, gridheight * side), s, vbgame.white, 10, "Arial Black")

        tx += vbgame.displaybuffer.Graphics.MeasureString(s, New Font("Arial Black", 10)).Width
        s = minegrid.mines
        vbgame.drawText(New Point(tx, gridheight * side), s, vbgame.black, 10, "Arial Black")
    End Sub

    Function gameloop()
        Dim run As Boolean = True
        Dim minegrid As New MineGrid(vbgame, side, gridwidth, gridheight, mines)
        Dim timer As New Stopwatch
        timer.Start()

        While run

            For Each e In vbgame.getKeyDownEvents()
                If e = "R" Then
                    minegrid = New MineGrid(vbgame, side, gridwidth, gridheight, mines)
                    timer.Restart()
                ElseIf e = "C" Then
                    For Each Cell In minegrid.cells
                        Cell.dug = True
                    Next
                ElseIf e = "Escape" Then
                    Return New outcome("escape", minegrid, timer)
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

            drawInfo(timer, minegrid)

            vbgame.clockTick(30)

            vbgame.update()

        End While
        Return Nothing
    End Function

End Class
