Class numbers
    Public Shared images As New Dictionary(Of Integer, System.Drawing.Image)
    Public Shared Sub generate(Optional side As Integer = 20)
        Dim vbgame As New VBGame
        Dim bitmap As Bitmap
        Dim c As System.Drawing.Color
        Dim font As Font = New Font("Arial Black", Convert.ToSingle(side / 2)) ', FontStyle.Bold)
        Dim g As Graphics
        Dim pen As Pen

        images.Clear()

        For n As Integer = -1 To 8
            bitmap = New Bitmap(side, side)
            g = Graphics.FromImage(bitmap)
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
            Select Case n
                Case -1
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
