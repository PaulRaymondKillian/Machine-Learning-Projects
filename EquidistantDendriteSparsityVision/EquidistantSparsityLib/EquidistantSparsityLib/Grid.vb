Imports System.IO

Public Class Grid
    Public Grid(,) As GridCell
    Public Size As Integer
    Public TerminalCell As New TerminalCell

    Public Sub New(ByVal pSize As Double)
        Size = pSize
        ReDim Grid(Size, Size)
        InitGrid()
        LinkGrid()
    End Sub

    Private Sub InitGrid()
        For i As Integer = 0 To Size - 1
            For j As Integer = 0 To Size - 1
                Grid(i, j) = New GridCell
                Grid(i, j).ID = i & "," & j
            Next
        Next
    End Sub

    Private Sub LinkGrid()
        Dim CurrentCell As GridCell
        For i As Integer = 0 To Size - 1
            For j As Integer = 0 To Size - 1
                CurrentCell = Grid(i, j)

                'Link 0 (Above Adjacent Cell)
                If j = 0 Then
                    CurrentCell.AdjacentCells(0) = TerminalCell
                Else
                    CurrentCell.AdjacentCells(0) = Grid(i, j - 1)
                End If

                'Link 1 (Right Adjacent Cell)
                If i = Size - 1 Then
                    CurrentCell.AdjacentCells(1) = TerminalCell
                Else
                    CurrentCell.AdjacentCells(1) = Grid(i + 1, j)
                End If

                'Link 2 (Below Adjacent Cell)
                If j = Size - 1 Then
                    CurrentCell.AdjacentCells(3) = TerminalCell
                Else
                    CurrentCell.AdjacentCells(3) = Grid(i, j + 1)
                End If

                'Link 3 (Left Adjacent Cell)
                If i = 0 Then
                    CurrentCell.AdjacentCells(3) = TerminalCell
                Else
                    CurrentCell.AdjacentCells(3) = Grid(i - 1, j)
                End If
            Next
        Next
    End Sub

End Class
