Class numbers
    Public Shared images As New Dictionary(Of Integer, System.Drawing.Image)
End Class

Class Cell

    Public vbgame As VBGame
    Public type As String = "blank"
    Public flagged As Boolean = False
    Public number As Integer = 0
    Public group As Integer
    Public x, y, side As Integer
    Public dug As Boolean = False
    Public cooldown As Integer = 0
    'Public colors As New Dictionary(Of Integer, System.Drawing.Color)

    Sub New(ByRef display As VBGame, Optional xt As Integer = 0, Optional yt As Integer = 0, Optional sidet As Integer = 10)

        'colors.Add(0, Color.FromArgb(200, 200, 200))
        'colors.Add(1, Color.FromArgb(0, 0, 255))
        'colors.Add(2, Color.FromArgb(0, 128, 0))
        'colors.Add(3, Color.FromArgb(255, 0, 0))
        'colors.Add(4, Color.FromArgb(0, 0, 128))
        'colors.Add(5, Color.FromArgb(128, 0, 0))
        'colors.Add(6, Color.FromArgb(0, 128, 128))
        'colors.Add(7, Color.FromArgb(0, 0, 0))
        'colors.Add(8, Color.FromArgb(128, 128, 128))

        vbgame = display

        x = xt
        y = yt
        side = sidet
    End Sub

    Function getRect()
        Return New Rectangle(x, y, side, side)
    End Function

    Function handle() As String
        Dim clicked As Boolean = False

        If cooldown > 0 Then
            cooldown -= 1
        End If

        If flagged And dug Then
            dug = False
        End If

        If Not dug Then
            clicked = False 'button.handle()

            vbgame.drawRect(getRect(), Color.FromArgb(200, 200, 200))
            vbgame.drawRect(New Rectangle(x + 2, y + 2, side - 4, side - 4), Color.FromArgb(250, 250, 250))
            If Not IsNothing(vbgame.mouse) Then
                If vbgame.collideRect(New Rectangle(vbgame.mouse.Location().X, vbgame.mouse.Location().Y, 1, 1), getRect()) Then
                    'vbgame.drawRect(getRect(), Color.FromArgb(128, 128, 128))
                    If vbgame.mouse.Button = vbgame.mouse_left And Not flagged Then
                        clicked = True
                    ElseIf vbgame.mouse.Button = vbgame.mouse_right Then
                        If cooldown <= 0 Then
                            cooldown = 5
                            If flagged Then
                                flagged = False
                            Else
                                flagged = True
                            End If
                        End If
                    End If
                End If
            End If

            If flagged Then
                vbgame.drawRect(New Rectangle(x + 5, y + 5, side - 10, side - 10), vbgame.red)
            End If


            If clicked Then
                dug = True
                If type = "mine" Then
                    Return "boom"
                ElseIf number = 0 Then
                    Return "digconnected"
                Else
                    Return "dug"
                End If
            End If
        Else
            If number <> 0 Then
                vbgame.blit(numbers.images(number), getRect())
            End If
            If Not IsNothing(vbgame.mouse) Then
                If vbgame.collideRect(New Rectangle(vbgame.mouse.Location().X, vbgame.mouse.Location().Y, 1, 1), getRect()) Then
                    If vbgame.mouse.Button = vbgame.mouse_right Then
                        Return "dig9"
                    End If
                End If
            End If
            End If
            Return "nothing"
    End Function

End Class

Public Class MineGrid

    Public vbgame As VBGame
    Public cells As Array
    Public gridsize As Integer
    Public random As New Random

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

    Sub New(ByRef display As VBGame, Optional gridsizet As Integer = 20, Optional mines As Integer = 300)
        Dim x, y, n, i As Integer
        gridsize = gridsizet
        vbgame = display
        Dim cellst((vbgame.width - gridsize) / gridsize, (vbgame.height - gridsize) / gridsize) As Cell

        cells = cellst
        For x = 0 To (vbgame.width - gridsize) / gridsize
            For y = 0 To (vbgame.height - gridsize) / gridsize

                cells(x, y) = New Cell(vbgame, x * gridsize, y * gridsize, gridsize)

            Next
        Next

        For i = 1 To mines
            x = random.Next(0, (vbgame.width - gridsize) / gridsize)
            y = random.Next(0, (vbgame.height - gridsize) / gridsize)
            cells(x, y).number = -1
            cells(x, y).type = "mine"
        Next

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

    Sub digNine(ByRef cells, x, y)
        Dim flags As Integer
        For Each Cell In getAdjacentCells(x, y)
            If Cell.flagged Then
                flags += 1
            End If
        Next
        If flags = cells(x, y).number Then
            For Each Cell In getAdjacentCells(x, y)
                Cell.dug = True
            Next
        End If
    End Sub

    Sub handleCells()
        Dim tx, ty, x, y As Integer
        Dim cmd As String
        For tx = 0 To (vbgame.width - gridsize) / gridsize
            For ty = 0 To (vbgame.height - gridsize) / gridsize
                x = tx
                y = ty
                cmd = cells(x, y).handle()

                If cmd = "dig9" Then
                    digNine(cells, x, y)
                End If

            Next
        Next
    End Sub

End Class
