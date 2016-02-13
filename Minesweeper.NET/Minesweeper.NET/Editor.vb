Imports System.Threading

Public Class Editor

    Private thread As New Thread(AddressOf mainloop)
    Private vbgame As New VBGame

    Dim side As Integer = 20
    Dim gridwidth As Integer = 30
    Dim gridheight As Integer = 16

    Private Sub Editor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

        While run

            vbgame.fill(Color.FromArgb(150, 150, 150))

            For Each e In vbgame.getMouseEvents()
                If create.handle(e) = MouseEvent.ButtonLeft Then
                    custom()
                    editloop(New MineGrid(side, gridwidth, gridheight, 0, True))
                ElseIf load.handle(e) = MouseEvent.ButtonLeft Then
                    editloop(Form1.loadGrid())
                End If
            Next

            create.draw()
            load.draw()

            vbgame.update()

        End While

    End Sub

    Sub editloop(grid As MineGrid)
        Dim run As Boolean = True
        Dim save As Integer
        Dim filename As String
        Dim x, y As Integer

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
                End If
            Next

            For Each e In vbgame.getMouseEvents()
                If e.action = MouseEvent.MouseUp Then
                    x = Math.Floor(e.location.X / side)
                    y = Math.Floor(e.location.Y / side)

                    Try

                        If e.button = MouseEvent.ButtonLeft Then
                            If grid.cells(x, y).number = -1 Then
                                grid.cells(x, y).number = 0
                                grid.mines -= 1
                            Else
                                grid.cells(x, y).number = -1
                                grid.mines += 1
                            End If
                            grid.calculateNumbers(grid.cells)
                            For Each Cell As Cell In grid.getAdjacentCells(x, y)
                                dirty.Add(Cell)
                            Next
                            dirty.Add(grid.cells(x, y))

                        ElseIf e.button = MouseEvent.ButtonRight Then
                            If grid.startpoint.X >= 0 Then
                                dirty.Add(grid.cells(grid.startpoint.X, grid.startpoint.Y))
                            End If
                            If New Point(x, y) <> grid.startpoint Then
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

            For Each Cell As Cell In dirty
                vbgame.drawRect(New Rectangle(Cell.x, Cell.y, Cell.side, Cell.side), Color.FromArgb(150, 150, 150))
                Cell.draw(True, vbgame)
            Next
            dirty.Clear()

            If grid.startpoint.X >= 0 Then
                vbgame.drawRect(New Rectangle(grid.startpoint.X * grid.side, grid.startpoint.Y * grid.side, grid.side, grid.side), Color.FromArgb(100, 0, 0, 255))
            End If

            vbgame.update()

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