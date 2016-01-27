Class numbers
    Public Shared images As New Dictionary(Of Integer, System.Drawing.Image)
    Public Shared Sub generate(Optional side As Integer = 20)
        Dim vbgame As New VBGame
        Dim bitmap As Bitmap
        Dim c As System.Drawing.Color
        Dim font As Font = New Font("Arial Black", Convert.ToSingle(side / 2)) ', FontStyle.Bold)
        Dim g As Graphics
        Dim pen As Pen

        For n As Integer = -1 To 8
            bitmap = New Bitmap(side, side)
            g = Graphics.FromImage(bitmap)
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
            Select Case n
                    pen = New Pen(Color.Black, side / 10)
                    g.DrawLine(pen, New Point(side / 2, 0), New Point(side / 2, side))
                    g.DrawLine(pen, New Point(0, side / 2), New Point(side, side / 2))
                    g.FillEllipse(Brushes.Black, New Rectangle(side / 10, side / 10, side - (side / 5) - 1, side - (side / 5) - 1))
                    pen.Dispose()
                Case 1
                    c = Color.FromArgb(0, 0, 255)
                Case 2
                    c = Color.FromArgb(0, 128, 0)
                Case 3
                    c = Color.FromArgb(255, 0, 0)
                Case 4
                    c = Color.FromArgb(0, 0, 128)
                Case 5
                    c = Color.FromArgb(128, 0, 0)
                Case 6
                    c = Color.FromArgb(0, 128, 128)
                Case 7
                    c = Color.FromArgb(0, 0, 0)
                Case 8
                    c = Color.FromArgb(128, 128, 128)
            End Select

            If n > 0 Then
                TextRenderer.DrawText(g, n, font, New Rectangle(0, 0, side, side), c, Color.Empty, TextFormatFlags.VerticalCenter Or TextFormatFlags.HorizontalCenter)
            End If

            g.Flush()
            numbers.images.Add(n, bitmap)
        Next
    End Sub
End Class

Class cross

    Public x As Integer
    Public y As Integer
    Public side As Integer
    Public opacity As Integer
    Public vbgame As VBGame

    Public Shared crosses As New List(Of cross)

    Public Sub New(vbgamet As VBGame, xt As Integer, yt As Integer, sidet As Integer)
        side = sidet
        x = xt
        y = yt
        opacity = 255
        vbgame = vbgamet
    End Sub

    Public Sub draw()
        vbgame.drawLine(New Point(x, y), New Point(x + side, y + side), Color.FromArgb(opacity, 255, 0, 0), side / 5)
        vbgame.drawLine(New Point(x + side, y), New Point(x, y + side), Color.FromArgb(opacity, 255, 0, 0), side / 5)
    End Sub

    Public Sub handle()
        opacity -= 10
        If opacity < 0 Then
            opacity = 0
        End If
        draw()
    End Sub

End Class

Class Cell

    Public vbgame As VBGame
    Public type As String = "blank"
    Public flagged As Boolean = False
    Public number As Integer = 0
    Public group As Integer
    Public x, y, ix, iy, side As Integer
    Public dug As Boolean = False
    Public cooldown As Integer = 0
    Public opacity As Integer = 255

    Sub New(ByRef display As VBGame, Optional xt As Integer = 0, Optional yt As Integer = 0, Optional sidet As Integer = 10)

        vbgame = display

        x = xt
        y = yt
        side = sidet
    End Sub

    Function getRect()
        Return New Rectangle(x, y, side, side)
    End Function

    Sub draw(tdug As Boolean)

        If IsNothing(tdug) Then
            tdug = dug
        End If

        If tdug Then
            If number <> 0 Then
                vbgame.blit(numbers.images(number), getRect())
            End If
            vbgame.drawRect(getRect(), Color.FromArgb(opacity, 200, 200, 200))
            vbgame.drawRect(New Rectangle(x + (side / 10), y + (side / 10), side - (side / 10) * 2, side - (side / 10) * 2), Color.FromArgb(opacity, 250, 250, 250))
            If opacity > 0 Then
                opacity -= 100
            End If
            If opacity < 0 Then
                opacity = 0
            End If
        Else
            vbgame.drawRect(getRect(), Color.FromArgb(200, 200, 200))
            vbgame.drawRect(New Rectangle(x + (side / 10), y + (side / 10), side - (side / 10) * 2, side - (side / 10) * 2), Color.FromArgb(250, 250, 250))
        End If
        If flagged Then
            vbgame.drawRect(New Rectangle(x + (side / 4), y + (side / 4), side - (side / 4) * 2, side - (side / 4) * 2), vbgame.red)
        End If
    End Sub

    Function handle() As String
        Dim clicked As Boolean = False

        If cooldown > 0 Then
            cooldown -= 1
        End If

        If flagged And dug Then
            dug = False
        End If

        draw(dug)

        If Not dug Then
            clicked = False 'button.handle()

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
                    digNineAndConnected(cells, x, y)
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

    Sub digNineAndConnected(ByRef cells, x, y)
        Dim flags As Integer
        For Each Cell In getAdjacentCells(x, y)
            If Cell.flagged Then
                flags += 1
            End If
        Next
        If flags = cells(x, y).number Then
            For Each Cell In getAdjacentCells(x, y)
                If Cell.dug = False Then
                    Cell.dug = True
                    If Cell.number = 0 Then
                        digNineAndConnected(cells, Cell.ix, Cell.iy)
                    End If
                End If
            Next
        Else
            cross.crosses.Add(New cross(vbgame, cells(x, y).x, cells(x, y).y, cells(x, y).side))
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
                    digNineAndConnected(cells, x, y)
                ElseIf cmd = "digconnected" Then
                    digNineAndConnected(cells, x, y)
                End If

            Next
        Next
    End Sub

End Class
