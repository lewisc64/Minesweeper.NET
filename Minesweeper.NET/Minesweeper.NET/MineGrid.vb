Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

<System.Serializable()>
Public Class MineGrid

    Public cells As Array
    Public startpoint As Point = New Point(-1, -1)
    Public side As Integer
    Public mines As Integer
    Public flags As Integer
    Public gridwidth As Integer
    Public gridheight As Integer
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
        If x - 1 >= 0 And y - 1 >= 0 Then
            tcells.Add(cells(x - 1, y - 1))
        End If
        If x + 1 < gridwidth And y - 1 >= 0 Then
            tcells.Add(cells(x + 1, y - 1))
        End If
        If x - 1 >= 0 And y + 1 < gridheight Then
            tcells.Add(cells(x - 1, y + 1))
        End If
        If x + 1 < gridwidth And y + 1 < gridheight Then
            tcells.Add(cells(x + 1, y + 1))
        End If
        If x - 1 >= 0 And y >= 0 And y < gridheight Then
            tcells.Add(cells(x - 1, y))
        End If
        If x >= 0 And x < gridwidth And y - 1 >= 0 Then
            tcells.Add(cells(x, y - 1))
        End If
        If x + 1 < gridwidth And y >= 0 And y < gridheight Then
            tcells.Add(cells(x + 1, y))
        End If
        If x >= 0 And x < gridwidth And y + 1 < gridheight Then
            tcells.Add(cells(x, y + 1))
        End If
        Return tcells
    End Function

    Sub calculateNumbers(ByRef cells)
        Dim n = 0
        For x = 0 To gridwidth - 1
            For y = 0 To gridheight - 1
                If cells(x, y).number = -1 Then
                    Continue For
                End If
                n = 0
                For Each Cell In getAdjacentCells(x, y)
                    If Cell.number = -1 Then
                        n += 1
                    End If
                Next
                cells(x, y).number = n
            Next
        Next
    End Sub

    Sub New(sidet As Integer, gridwidtht As Integer, gridheightt As Integer, minest As Integer, Optional skipgen As Boolean = False)
        Dim x, y, i As Integer
        Dim acted As Boolean
        mines = minest
        side = sidet
        gridwidth = gridwidtht
        gridheight = gridheightt

        Try
            Dim cellst(gridwidth - 1, gridheight - 1) As Cell

            cells = cellst
            For x = 0 To gridwidth - 1
                For y = 0 To gridheight - 1

                    cells(x, y) = New Cell(x * side, y * side, side)
                    cells(x, y).ix = x
                    cells(x, y).iy = y

                Next
            Next

            If Not skipgen Then

                For i = 1 To mines
                    While True
                        x = random.Next(0, gridwidth)
                        y = random.Next(0, gridheight)
                        If cells(x, y).number <> -1 Then
                            Exit While
                        End If
                    End While
                    cells(x, y).number = -1
                Next

                calculateNumbers(cells)

                acted = False
                For x = 0 To gridwidth - 1
                    For y = 0 To gridheight - 1
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
            End If
        Catch ex As System.OverflowException
            MsgBox("Invalid")
            cells = New MineGrid(20, 10, 10, 0, True).cells
        End Try
    End Sub

    Function countFlags(ByRef cells, x, y)
        Dim flags As Integer

        For Each Cell In getAdjacentCells(x, y)
            If Cell.flagged Then
                flags += 1
            End If
        Next

        Return flags
    End Function

    Function digNine(ByRef cells, x, y) As String
        Dim flags As Integer
        flags = countFlags(cells, x, y)

        If flags = cells(x, y).number Then

            For Each Cell As Cell In getAdjacentCells(x, y)

                If Cell.dug = False And Not Cell.flagged Then

                    Cell.dug = True
                    If Cell.number = -1 Then
                        Return "boom"
                    End If

                    If Cell.number = 0 Then
                        digNine(cells, Cell.ix, Cell.iy)
                    End If
                End If
            Next
        Else
            cross.crosses.Add(New cross(cells(x, y).x, cells(x, y).y, cells(x, y).side))
        End If
        Return Nothing
    End Function

    Sub drawCells(ByRef vbgame As VBGame)
        Dim x, y As Integer

        vbgame.fill(Color.FromArgb(150, 150, 150))

        flags = 0

        For x = 0 To gridwidth - 1
            For y = 0 To gridheight - 1

                If cells(x, y).flagged Then
                    flags += 1
                End If

                cells(x, y).draw(cells(x, y).dug, vbgame)

                'vbgame.drawCenteredText(New Rectangle(cells(x, y).x, cells(x, y).y, cells(x, y).side, cells(x, y).side), Math.Round(cells(x, y).probability, 2), vbgame.black, 8)
            Next
        Next
    End Sub

    Public Shared Sub save(filename As String, minegrid As MineGrid)
        Dim fs As FileStream = New FileStream(filename, FileMode.OpenOrCreate)
        Dim bf As New BinaryFormatter
        bf.Serialize(fs, minegrid)
        fs.Close()
    End Sub

    Public Shared Function load(filename As String) As MineGrid
        Dim minegrid As MineGrid
        Try
            Dim fs As FileStream = New FileStream(filename, FileMode.Open)
            Dim bf As New BinaryFormatter
            minegrid = bf.Deserialize(fs)
            fs.Close()
            fs.Dispose()
        Catch
            MsgBox("Invalid or corrupt file.")
            minegrid = New MineGrid(20, 10, 10, 0, True)
        End Try
        Return minegrid
    End Function

    Function handleCells(mouse As MouseEvent)
        Dim x, y As Integer
        Dim cmd As String = ""

        x = Math.Floor(mouse.location.X / side)
        y = Math.Floor(mouse.location.Y / side)

        Try
            cmd = cells(x, y).handle(mouse)

            If cmd = "dig9" Then
                Return digNine(cells, x, y)
            ElseIf cmd = "boom" Then
                Return "boom"
            End If
        Catch ex As IndexOutOfRangeException
        End Try

        Return Nothing
    End Function

End Class
