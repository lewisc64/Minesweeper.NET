Imports System.Threading

Public Class Form1

    Public thread As New Thread(AddressOf mainloop)
    Public vbgame As New VBGame

    Dim side As Integer = 20
    Dim mines As Integer = 99
    Dim gridwidth As Integer = 30
    Dim gridheight As Integer = 16

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        adjustSize()
        numbers.generate(side)
        Me.Icon = Icon.FromHandle(New Bitmap(numbers.images(-1)).GetHicon())
        thread.Start()
    End Sub

    Sub adjustSize()
        vbgame.setDisplay(Me, New Size(Math.Max(gridwidth * side, 200), Math.Max(gridheight * side + 20, 140)), "Minesweeper.NET")
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
        adjustSize()
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

    Sub setAll(ByRef preminegrid As MineGrid, gwidth As Integer, gheight As Integer, gmines As Integer)
        mines = gmines
        gridwidth = gwidth
        gridheight = gheight
        adjustSize()
        preminegrid = getPreMineGrid()
    End Sub

    Sub mainloop()
        Dim run As Boolean = True

        Dim preminegrid As MineGrid = getPreMineGrid()
        Dim outcome As outcome

        Dim start As New Button(vbgame, "Start", New Rectangle(10, 10, 110, 20), "Arial Black", 11)
        start.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        start.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim customize As New Button(vbgame, "Custom", New Rectangle(10, 60, 110, 20), "Arial Black", 11)
        customize.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        customize.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim beginner As New Button(vbgame, "Beginner", New Rectangle(10, 85, 110, 20), "Arial Black", 11)
        beginner.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        beginner.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim intermediate As New Button(vbgame, "Intermediate", New Rectangle(10, 110, 110, 20), "Arial Black", 11)
        intermediate.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        intermediate.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim expert As New Button(vbgame, "Expert", New Rectangle(10, 135, 110, 20), "Arial Black", 11)
        expert.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        expert.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim quit As New Button(vbgame, "Quit", New Rectangle(vbgame.width - 60, 10, 50, 20), "Arial Black", 11)
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

            quit.x = vbgame.width - 60

            For Each e In vbgame.getMouseEvents()

                If start.handle(e) = MouseEvent.ButtonLeft Then
                    outcome = gameloop()
                    preminegrid = getPreMineGrid(outcome.minegrid)

                ElseIf customize.handle(e) = MouseEvent.ButtonLeft Then
                    custom()
                    preminegrid = getPreMineGrid()

                ElseIf beginner.handle(e) = MouseEvent.ButtonLeft Then
                    setAll(preminegrid, 9, 9, 10)
                ElseIf intermediate.handle(e) = MouseEvent.ButtonLeft Then
                    setAll(preminegrid, 16, 16, 40)
                ElseIf expert.handle(e) = MouseEvent.ButtonLeft Then
                    setAll(preminegrid, 30, 16, 99)

                ElseIf customize.handle(e) = MouseEvent.ButtonRight Then
                    MsgBox(gridwidth & "x" & gridheight & vbCrLf & mines & " mines.")
                ElseIf quit.handle(e) = MouseEvent.ButtonLeft Then
                    End
                End If

            Next
            preminegrid.drawCells()

            start.draw()
            customize.draw()
            beginner.draw()
            intermediate.draw()
            expert.draw()
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

    Function gameloop(Optional autosolve As Boolean = False)
        Dim run As Boolean = True
        Dim minegrid As New MineGrid(vbgame, side, gridwidth, gridheight, mines)
        Dim timer As New Stopwatch
        timer.Start()

        Dim solver As Solver = New Solver(minegrid)

        While run

            For Each e In vbgame.getKeyDownEvents()
                If e = "R" Then
                    minegrid = New MineGrid(vbgame, side, gridwidth, gridheight, mines)
                    timer.Restart()
                    solver = New Solver(minegrid)
                ElseIf e = "C" Then
                    For Each Cell In minegrid.cells
                        Cell.dug = True
                    Next
                ElseIf e = "Escape" Then
                    Return New outcome("escape", minegrid, timer)
                ElseIf e = "S" Then
                    autosolve = True
                End If
            Next

            For Each e In vbgame.getMouseEvents()
                If e.action = MouseEvent.MouseUp Then
                    minegrid.handleCells(e)
                End If
            Next

            If autosolve Then
                solver.handle()
            End If

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
