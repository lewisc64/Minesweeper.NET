Imports System.Threading

Public Class Form1

    Public thread As New Thread(AddressOf mainloop)
    Public vbgame As New VBGame

    Dim side As Integer = 20
    Dim mines As Integer = 99
    Dim gridwidth As Integer = 30
    Dim gridheight As Integer = 16

    Dim guesslessgen As Boolean = False

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

    Function loadGrid() As MineGrid
        Dim grid As MineGrid
        Dim filename As String = InputBox("Name of puzzle?")
        grid = MineGrid.load(filename & ".minegrid")
        mines = grid.mines
        gridwidth = grid.gridwidth
        gridheight = grid.gridheight
        Return grid
    End Function

    Function getPreMineGrid(Optional preminegrid As MineGrid = Nothing)
        If IsNothing(preminegrid) Then
            preminegrid = New MineGrid(side, gridwidth, gridheight, mines)
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

    Sub startmenu()
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

        Dim load As New Button(vbgame, "Load", New Rectangle(10, 160, 110, 20), "Arial Black", 11)
        load.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        load.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim quit As New Button(vbgame, "Back", New Rectangle(vbgame.width - 60, 10, 50, 20), "Arial Black", 11)
        quit.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        quit.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim guessless As New Button(vbgame, "Guessless", New Rectangle(vbgame.width - 60, vbgame.height - 30, 50, 20), "Arial Black")
        guessless.setColor(vbgame.red, vbgame.red)
        guessless.setTextColor(vbgame.black, vbgame.black)

        While run

            quit.x = vbgame.width - 60
            guessless.x = vbgame.width - 60
            guessless.y = vbgame.height - 30

            For Each e In vbgame.getKeyDownEvents()
                If e = "R" Then
                    preminegrid = getPreMineGrid()
                ElseIf e = "Escape" Then
                    run = False
                End If
            Next

            For Each e In vbgame.getMouseEvents()

                If start.handle(e) = MouseEvent.ButtonLeft Then
                    gameloop()

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

                ElseIf load.handle(e) = MouseEvent.ButtonLeft Then
                    gameloop(loadGrid())

                ElseIf quit.handle(e) = MouseEvent.ButtonLeft Then
                    run = False
                ElseIf guessless.handle(e) = MouseEvent.ButtonLeft Then
                    If guesslessgen Then
                        guesslessgen = False
                        guessless.setColor(vbgame.red, vbgame.red)
                    Else
                        guesslessgen = True
                        guessless.setColor(vbgame.green, vbgame.green)
                    End If
                End If

            Next

            preminegrid.drawCells(vbgame)

            start.draw()
            customize.draw()
            beginner.draw()
            intermediate.draw()
            expert.draw()
            load.draw()
            quit.draw()
            vbgame.drawText(New Point(guessless.x - 40, guessless.y - 40), "Experimental", vbgame.white, 10, "Arial Black")
            guessless.draw()

            vbgame.update()
            vbgame.clockTick(30)
        End While
    End Sub

    Sub mainloop()
        Dim run As Boolean = True

        Dim bg As New MineGrid(side, gridwidth, gridheight, mines, True)

        Dim start As New Button(vbgame, "Start", New Rectangle(10, 10, 110, 20), "Arial Black", 11)
        start.setColor(vbgame.black, vbgame.white)
        start.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim editorb As New Button(vbgame, "Editor", New Rectangle(10, 35, 110, 20), "Arial Black", 11)
        editorb.setColor(vbgame.black, vbgame.white)
        editorb.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim quit As New Button(vbgame, "Quit", New Rectangle(vbgame.width - 60, 10, 50, 20), "Arial Black", 11)
        quit.setColor(vbgame.black, vbgame.white)
        quit.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        While run

            For Each e In vbgame.getKeyDownEvents()
                If e = "Escape" Then
                    End
                End If
            Next

            For Each e In vbgame.getMouseEvents()

                If start.handle(e) = MouseEvent.ButtonLeft Then
                    startmenu()
                    gridwidth = 30
                    gridheight = 16
                    mines = 99
                    adjustSize()
                    bg = New MineGrid(side, gridwidth, gridheight, mines, True)
                ElseIf editorb.handle(e) = MouseEvent.ButtonLeft Then
                    Me.Invoke(Sub() Editor.Show())
                ElseIf quit.handle(e) = MouseEvent.ButtonLeft Then
                    End
                End If

            Next

            bg.drawCells(vbgame)

            start.draw()
            editorb.draw()
            quit.draw()

            vbgame.drawCenteredText(vbgame.getRect(), "Minesweeper.NET", vbgame.black, 16, "Arial Black")

            vbgame.update()
            vbgame.clockTick(30)
        End While

    End Sub

    Function getDensity() As Double
        Return mines / (gridwidth * gridheight)
    End Function

    Function getGuessless() As MineGrid
        Dim solver As Solver
        Dim minegrid As New MineGrid(side, gridwidth, gridheight, mines)
        Dim flags As Integer
        Dim x, y As Integer

        For x = 1 To 500

            minegrid = New MineGrid(side, gridwidth, gridheight, mines)
            solver = New Solver(minegrid)
            flags = 0

            For y = 1 To 5

                If Not solver.handle() Then
                    Exit For
                End If

                If minegrid.flags = mines Then
                    Exit For
                End If

            Next

            If minegrid.flags = mines Then
                Exit For
            End If

        Next

        If x >= 499 Then
            MsgBox("Unable to create guessless game (Failed after " & x & " attempts.)")
        End If

        For Each Cell As Cell In minegrid.cells
            Cell.dug = False
            Cell.flagged = False
        Next

        For Each Cell As Cell In minegrid.cells
            If Cell.number = 0 Then
                minegrid.digNine(minegrid.cells, Cell.ix, Cell.iy)
                Cell.dug = True
                Exit For
            End If
        Next

        Return minegrid

    End Function

    Function getNewGrid()
        If guesslessgen And getDensity() <= 0.2 Then
            Return getGuessless()
        Else
            If getDensity() > 0.2 And guesslessgen Then
                MsgBox("Mine density too high for guessless.")
            End If
            Return New MineGrid(side, gridwidth, gridheight, mines)
        End If
    End Function

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

    Function gameloop(Optional minegrid As MineGrid = Nothing, Optional autosolve As Boolean = False)
        Dim run As Boolean = True
        Dim timer As New Stopwatch
        timer.Start()

        If IsNothing(minegrid) Then
            minegrid = getNewGrid()
        Else
            adjustSize()
        End If

        Dim solver As Solver = New Solver(minegrid)

        While run

            For Each e In vbgame.getKeyDownEvents()
                If e = "R" Then
                    minegrid = getNewGrid()
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

            minegrid.drawCells(vbgame)

            For Each effect In cross.crosses.ToList()
                If effect.opacity = 0 Then
                    cross.crosses.Remove(effect)
                End If
                effect.handle(vbgame)
            Next

            drawInfo(timer, minegrid)

            vbgame.clockTick(30)

            vbgame.update()

        End While
        Return Nothing
    End Function

End Class
