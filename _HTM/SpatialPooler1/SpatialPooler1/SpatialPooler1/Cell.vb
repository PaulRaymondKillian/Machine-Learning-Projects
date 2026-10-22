Public Class Cell
    Public ID As String
    Public State As CellState

    Public Enum CellState
        Inactive = 0
        ActiveProximal = 1
        ActiveDistal = 2
    End Enum

    Public Sub New(pID As String)
        ID = pID
        State = CellState.Inactive
    End Sub

End Class
