Public Class Solver
    Public minegrid As MineGrid
    Public frames As Integer = 0

    Public Sub New(ByRef minegridt As MineGrid)
        minegrid = minegridt
    End Sub

    Sub handle()

        Dim adj As List(Of Cell)
        Dim flags As Integer
        Dim acted As Boolean = False

        If frames >= 0 Then
            frames = 0

            For Each Cell As Cell In minegrid.cells
                Cell.probability = 0
            Next

            For Each Cell As Cell In minegrid.cells
                If Cell.number > 0 And Cell.dug And Not Cell.flagged Then

                    adj = minegrid.getAdjacentCells(Cell.ix, Cell.iy)

                    For Each adjcell As Cell In adj.ToList()
                        If adjcell.dug Then
                            adj.Remove(adjcell)
                        End If
                    Next

                    flags = minegrid.countFlags(minegrid.cells, Cell.ix, Cell.iy)

                    If Cell.number = flags Then
                        minegrid.digNine(minegrid.cells, Cell.ix, Cell.iy)
                        Continue For
                    End If

                    For Each adjcell As Cell In adj.ToList()
                        adjcell.probability = (Cell.number) / (adj.ToArray().Length)

                        If adjcell.probability = 1 Then
                            adjcell.flagged = True
                        ElseIf adjcell.probability = 0 Then
                            adjcell.dug = True
                        End If
                    Next

                End If

            Next
        Else
            frames += 1
        End If

    End Sub

End Class
