Class Cell

    Public vbgame As VBGame
    Public type As String = "blank"
    Public flagged As Boolean = False
    Public number As Integer = 0
    Public group As Integer
    Public x, y, ix, iy, side As Integer
    Public dug As Boolean = False
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
                opacity -= 65
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

    Function handle(mouse As MouseEvent) As String
        Dim clicked As Boolean = False

        If flagged And dug Then
            dug = False
        End If

        If Not dug Then
            clicked = False

            If vbgame.collideRect(New Rectangle(mouse.location.X, mouse.location.Y, 0, 0), getRect()) Then

                If mouse.button = MouseEvent.ButtonLeft And Not flagged Then
                    clicked = True

                ElseIf mouse.button = MouseEvent.ButtonRight Then
                    If flagged Then
                        flagged = False
                    Else
                        flagged = True
                    End If
                End If
            End If

            If clicked Then
                dug = True
                If type = "mine" Then
                    Return "boom"
                ElseIf number = 0 Then
                    Return "dig9"
                Else
                    Return "dug"
                End If
            End If
        Else
            If vbgame.collideRect(New Rectangle(vbgame.mouse.Location().X, vbgame.mouse.Location().Y, 1, 1), getRect()) Then
                If mouse.button = MouseEvent.ButtonRight Then
                    Return "dig9"
                End If
            End If
        End If
        Return "nothing"
    End Function

End Class