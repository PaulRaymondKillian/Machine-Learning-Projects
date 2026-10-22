Public Class TerminalCell
    Inherits GridCell

    Public Sub New()
        Value = 0
        AdjacentCells(0) = Me
        AdjacentCells(1) = Me
        AdjacentCells(2) = Me
        AdjacentCells(3) = Me
    End Sub

End Class
