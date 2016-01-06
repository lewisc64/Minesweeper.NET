Class Cell

    Public vbgame As VBGame
    Public type As String = "blank"
    Public number As Integer = 0
    Public button As New Button
    Public dug As Boolean = False
    Public colors As Dictionary(Of Integer, System.Drawing.Color)

    Sub New(ByRef display As VBGame, Optional x As Integer = 0, Optional y As Integer = 0, Optional side As Integer = 10)

        colors.Add(0, Color.FromArgb(200, 200, 200))
        colors.Add(1, Color.FromArgb(0, 0, 255))
        colors.Add(2, Color.FromArgb(0, 128, 0))
        colors.Add(3, Color.FromArgb(255, 0, 0))
        colors.Add(4, Color.FromArgb(0, 0, 128))
        colors.Add(5, Color.FromArgb(128, 0, 0))
        colors.Add(6, Color.FromArgb(0, 128, 128))
        colors.Add(7, Color.FromArgb(0, 0, 0))
        colors.Add(8, Color.FromArgb(128, 128, 128))

        vbgame = display
        button.useDisplay(vbgame)
        button.setRect(New Rectangle(x, y, side, side))
        button.setColor(Color.FromArgb(200, 200, 200), Color.FromArgb(128, 128, 128))
    End Sub

    Function handle() As String
        Dim clicked As Boolean = False
        vbgame.drawCenteredText(button.getRect(), CStr(number), colors(number))
        If Not dug Then
            clicked = button.handle()

            If clicked Then
                dug = True
                If type = "mine" Then
                    Return "boom"
                ElseIf number = 0 Then
                    Return "trigger_chain_reaction"
                Else
                    Return "dug"
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

    Sub New(ByRef display As VBGame, Optional gridsizet As Integer = 10)
        Dim x, y As Integer
        gridsize = gridsizet
        vbgame = display
        cells = cells((vbgame.width - gridsize) / gridsize, (vbgame.height - gridsize) / gridsize)

        For x = 0 To (vbgame.width - gridsize) / gridsize
            For y = 0 To (vbgame.height - gridsize) / gridsize

                cells(x, y) = New Cell(vbgame, x, y, gridsize)

            Next
        Next
    End Sub

    Sub draw()
        Dim x, y As Integer
        Dim cmd As String
        For x = 0 To (vbgame.width - gridsize) / gridsize
            For y = 0 To (vbgame.height - gridsize) / gridsize
                cmd = cells(x, y).handle()
            Next
        Next
    End Sub

End Class
