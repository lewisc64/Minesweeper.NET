Public Class MineGrid

    Public vbgame As VBGame
    Public cells As Array
    Public gridsize As Integer
    Public random As New Random

    Function getDirectlyAdjacentCells(x, y)
        Dim tcells As New List(Of Cell)
        Try
            tcells.Add(cells(x - 1, y))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x, y - 1))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x + 1, y))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x, y + 1))
        Catch ex As Exception
        End Try
        Return tcells
    End Function

    Function getAdjacentCells(x, y)
        Dim tcells As New List(Of Cell)
        Try
            tcells.Add(cells(x - 1, y - 1))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x + 1, y - 1))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x - 1, y + 1))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x + 1, y + 1))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x - 1, y))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x, y - 1))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x + 1, y))
        Catch ex As Exception
        End Try
        Try
            tcells.Add(cells(x, y + 1))
        Catch ex As Exception
        End Try
        Return tcells
    End Function

    Sub calculateNumbers(ByRef cells)
        Dim n = 0
        For x = 0 To (vbgame.width - gridsize) / gridsize
            For y = 0 To (vbgame.height - gridsize) / gridsize
                If cells(x, y).type = "mine" Then
                    Continue For
                End If
                n = 0
                For Each Cell In getAdjacentCells(x, y)
                    If Cell.type = "mine" Then
                        n += 1
                    End If
                Next
                cells(x, y).number = n
            Next
        Next
    End Sub

    Sub New(ByRef display As VBGame, Optional gridsizet As Integer = 20, Optional mines As Integer = 300)
        Dim x, y, i As Integer
        Dim acted As Boolean
        gridsize = gridsizet
        vbgame = display
        Dim cellst((vbgame.width - gridsize) / gridsize, (vbgame.height - gridsize) / gridsize) As Cell

        cells = cellst
        For x = 0 To (vbgame.width - gridsize) / gridsize
            For y = 0 To (vbgame.height - gridsize) / gridsize

                cells(x, y) = New Cell(vbgame, x * gridsize, y * gridsize, gridsize)
                cells(x, y).ix = x
                cells(x, y).iy = y

                cells(x, y).draw(False)
            Next
            vbgame.update()
        Next

        For i = 1 To mines
            While True
                x = random.Next(0, (vbgame.width - gridsize) / gridsize + 1)
                y = random.Next(0, (vbgame.height - gridsize) / gridsize + 1)
                If cells(x, y).number <> -1 Then
                    Exit While
                End If
            End While
            cells(x, y).number = -1
            cells(x, y).type = "mine"
        Next

        calculateNumbers(cells)

        acted = False
        For x = 0 To (vbgame.width - gridsize) / gridsize
            For y = 0 To (vbgame.height - gridsize) / gridsize
                If cells(x, y).number = 0 Then
                    cells(x, y).dug = True
                    digNine(cells, x, y)
                    acted = True
                End If
                If acted Then
                    Exit For
                End If
            Next
            If acted Then
                Exit For
            End If
        Next
    End Sub

    Sub digNine(ByRef cells, x, y)
        Dim flags As Integer

        For Each Cell In getAdjacentCells(x, y)
            If Cell.flagged Then
                flags += 1
            End If
        Next

        If flags = cells(x, y).number Then

            For Each Cell In getAdjacentCells(x, y)

                If Cell.dug = False And Not Cell.flagged Then

                    Cell.dug = True

                    If Cell.number = 0 Then
                        digNine(cells, Cell.ix, Cell.iy)
                    End If
                End If
            Next
        Else
            cross.crosses.Add(New cross(vbgame, cells(x, y).x, cells(x, y).y, cells(x, y).side))
        End If
    End Sub

    Sub drawCells()
        Dim tx, ty, x, y As Integer

        vbgame.fill(Color.FromArgb(150, 150, 150))

        For tx = 0 To (vbgame.width - gridsize) / gridsize
            For ty = 0 To (vbgame.height - gridsize) / gridsize

                x = tx
                y = ty

                cells(x, y).draw(cells(x, y).dug)

            Next
        Next
    End Sub

    Function handleCells(mouse As MouseEvent)

        Dim tx, ty, x, y As Integer
        Dim cmd As String = ""
        For tx = 0 To (vbgame.width - gridsize) / gridsize
            For ty = 0 To (vbgame.height - gridsize) / gridsize

                x = tx
                y = ty

                cmd = cells(x, y).handle(mouse)

                If cmd = "dig9" Then
                    digNine(cells, x, y)
                ElseIf cmd = "boom" Then
                    Return "boom"
                End If

            Next
        Next
        Return Nothing
    End Function

End Class
