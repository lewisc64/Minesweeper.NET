Public Class Solver
    Public minegrid As MineGrid
    Public frames As Integer = 0
    Public framelimit As Integer = 0

    Public Sub New(ByRef minegridt As MineGrid)
        minegrid = minegridt
    End Sub

    Function handle()

        Dim change As Boolean = False
        Dim adj As List(Of Cell)
        Dim flags As Integer
        Dim probability As Double
        Dim acted As Boolean = False

        If frames >= framelimit Then
            frames = 0

            For Each Cell As Cell In minegrid.cells
                Cell.probability = 0
            Next

            For Each Cell As Cell In minegrid.cells
                If Cell.number > 0 And Cell.dug And Not Cell.flagged Then

                    adj = minegrid.getAdjacentCells(Cell.ix, Cell.iy)

                    For Each adjcell As Cell In adj.ToList()
                        If adjcell.dug Or adjcell.flagged Then
                            adj.Remove(adjcell)
                        End If
                    Next

                    flags = minegrid.countFlags(minegrid.cells, Cell.ix, Cell.iy)

                    If Cell.number = flags Then
                        minegrid.digNine(minegrid.cells, Cell.ix, Cell.iy)
                        Continue For
                    End If

                    For Each adjcell As Cell In adj.ToList()
                        probability = (Cell.number - flags) / (adj.ToArray().Length)

                        adjcell.probability = probability

                        If adjcell.probability = 1 Then
                            adjcell.flagged = True
                            minegrid.flags += 1
                            change = True
                        ElseIf adjcell.probability = 0 Then
                            adjcell.dug = True
                        End If
                    Next

                End If

            Next
        Else
            frames += 1
        End If

        Return change

    End Function

End Class
