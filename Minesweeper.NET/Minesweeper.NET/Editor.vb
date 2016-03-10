Imports System.Threading

Public Class Editor

    Private thread As New Thread(AddressOf mainloop)
    Private vbgame As New VBGame

    Dim side As Integer = 20
    Dim gridwidth As Integer = 30
    Dim gridheight As Integer = 16

    Private Sub Editor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ControlBox = False
        MaximizeBox = False
        adjustSize()
        thread.Start()
    End Sub

    Sub adjustSize()
        vbgame.setDisplay(Me, New Size(Math.Max(gridwidth * side, 200), Math.Max(gridheight * side + 20, 140)), "Minesweeper.NET Editor")
    End Sub

    Sub custom()
        Try
            gridwidth = InputBox("Grid width?")
            gridheight = InputBox("Grid Height?")
        Catch ex As Exception
            gridwidth = 30
            gridheight = 16
        End Try
        adjustSize()
    End Sub

    Private Sub mainloop()

        Dim run As Boolean = True

        Dim create As New Button(vbgame, "New...", New Rectangle(10, 10, 110, 20), "Arial Black", 11)
        create.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        create.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim load As New Button(vbgame, "Load", New Rectangle(10, 35, 110, 20), "Arial Black", 11)
        load.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        load.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        Dim close As New Button(vbgame, "Close", New Rectangle(10, 60, 110, 20), "Arial Black", 11)
        close.setColor(Color.FromArgb(0, 0, 0, 0), vbgame.white)
        close.setTextColor(vbgame.white, Color.FromArgb(0, 0, 0, 0))

        While run

            vbgame.fill(Color.FromArgb(150, 150, 150))

            For Each e In vbgame.getMouseEvents()
                If create.handle(e) = MouseEvent.buttons.left Then
                    custom()
                    editloop(New MineGrid(side, gridwidth, gridheight, 0, True))
                ElseIf load.handle(e) = MouseEvent.buttons.left Then
                    editloop(Form1.loadGrid())
                    side = 20
                    gridwidth = 30
                    gridheight = 16
                    adjustSize()
                ElseIf close.handle(e) = MouseEvent.buttons.left Then
                    Me.Invoke(Sub() Me.Hide())
                End If
            Next

            create.draw()
            load.draw()
            close.draw()

            vbgame.update()
            vbgame.clockTick(30)

        End While

    End Sub

    Sub editloop(grid As MineGrid)
        Dim run As Boolean = True
        Dim gridlines As Boolean = True
        Dim save As Integer
        Dim filename As String
        Dim x, y As Integer

        side = grid.side
        gridwidth = grid.gridwidth
        gridheight = grid.gridheight

        adjustSize()

        Dim dirty As New List(Of Cell)

        For Each Cell As Cell In grid.cells
            Cell.dug = True
            Cell.opacity = 0
            Cell.flagged = False
        Next

        grid.drawCells(vbgame)

        While run

            For Each e In vbgame.getKeyDownEvents()
                If e = "Escape" Then
                    run = False
                ElseIf e = "G" Then
                    gridlines = Not gridlines
                    If Not gridlines Then
                        grid.drawCells(vbgame)
                    End If
                End If
            Next

            For Each e In vbgame.getMouseEvents()
                If e.action = MouseEvent.actions.up Then
                    x = Math.Floor(e.location.X / side)
                    y = Math.Floor(e.location.Y / side)

                    Try

                        If e.button = MouseEvent.buttons.left Then
                            If grid.cells(x, y).number = -1 Then
                                grid.cells(x, y).number = 0
                                grid.mines -= 1
                                For Each Cell In grid.getAdjacentCells(x, y)
                                    If Cell.number > 0 Then
                                        Cell.number -= 1
                                    ElseIf Cell.number = -1 Then
                                        grid.cells(x, y).number += 1
                                    End If
                                Next

                            Else
                                grid.cells(x, y).number = -1
                                grid.mines += 1
                                For Each Cell In grid.getAdjacentCells(x, y)
                                    If Cell.number <> -1 Then
                                        Cell.number += 1
                                    End If
                                Next
                            End If

                            For Each Cell As Cell In grid.getAdjacentCells(x, y)
                                dirty.Add(Cell)
                            Next
                            dirty.Add(grid.cells(x, y))

                        ElseIf e.button = MouseEvent.buttons.right Then
                            If grid.startpoint.X >= 0 Then
                                dirty.Add(grid.cells(grid.startpoint.X, grid.startpoint.Y))
                            End If
                            If New Point(x, y) <> grid.startpoint And x < grid.gridwidth And y < grid.gridheight Then
                                grid.startpoint = New Point(x, y)
                                dirty.Add(grid.cells(x, y))
                            Else
                                grid.startpoint = New Point(-1, -1)
                            End If
                        End If

                    Catch ex As IndexOutOfRangeException
                    End Try

                End If
            Next

            If grid.startpoint.X >= 0 Then
                dirty.Add(grid.cells(grid.startpoint.X, grid.startpoint.Y))
            End If

            For Each Cell As Cell In dirty
                vbgame.drawRect(New Rectangle(Cell.x, Cell.y, Cell.side, Cell.side), Color.FromArgb(150, 150, 150))
                Cell.draw(True, vbgame)
            Next
            dirty.Clear()

            If grid.startpoint.X >= 0 Then
                vbgame.drawRect(New Rectangle(grid.startpoint.X * grid.side, grid.startpoint.Y * grid.side, grid.side, grid.side), Color.FromArgb(100, 0, 0, 255))
            End If

            If gridlines Then
                For x = 1 To gridwidth
                    vbgame.drawLine(New Point(x * side, 0), New Point(x * side, vbgame.height - 20), vbgame.black)
                Next
                For y = 1 To gridheight
                    vbgame.drawLine(New Point(0, y * side), New Point(vbgame.width, y * side), vbgame.black)
                Next
            End If

            vbgame.update()
            vbgame.clockTick(30)

        End While

        For Each Cell In grid.cells
            Cell.dug = False
            Cell.opacity = 255
        Next

        If grid.startpoint.X >= 0 Then
            grid.digNine(grid.cells, grid.startpoint.X, grid.startpoint.Y)
            grid.cells(grid.startpoint.X, grid.startpoint.Y).dug = True
        End If

        save = MsgBox("Would you like to save?", vbYesNo)

        If save = MsgBoxResult.Yes Then
            filename = InputBox("Name of puzzle?")
            MineGrid.save(filename & ".minegrid", grid)
        End If

    End Sub

End Class
