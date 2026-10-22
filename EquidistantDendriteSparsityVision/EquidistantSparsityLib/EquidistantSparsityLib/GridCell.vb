Public Class GridCell
    Public BelowCells() As GridCell
    Public AboveCells() As GridCell
    Public AdjacentCells(4) As GridCell

    Public Value As Double
    Public ID As String

    Public Sub New()
        Value = 0
    End Sub
End Class
