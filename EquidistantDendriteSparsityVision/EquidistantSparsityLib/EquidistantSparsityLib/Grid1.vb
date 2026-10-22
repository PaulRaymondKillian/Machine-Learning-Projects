Public Class Grid1
    Inherits Grid

    Public Grid0 As Grid0

    Public Sub New(ByVal pGrid0 As Grid0)
        MyBase.New(5)
        Grid0 = pGrid0
        InitBelowDendrites()

    End Sub

    Public Sub InitBelowDendrites()
        For i As Integer = 0 To Size - 1
            For j As Integer = 0 To Size - 1
                ReDim Grid(i, j).BelowCells(Math.Pow(Grid0.Size / 5, 2))
                For k As Integer = i To Grid0.Size - 1 Step +5
                    For l As Integer = j To Grid0.Size - 1 Step +5
                        Grid(i, j).BelowCells((l / 5) + k) = Grid0.Grid(k, l)
                    Next
                Next
            Next
        Next
    End Sub

End Class
