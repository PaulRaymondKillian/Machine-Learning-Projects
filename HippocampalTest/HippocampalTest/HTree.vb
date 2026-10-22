Public Class HTree
    Dim Root As HNode

    Public Sub New()
        Root = New HNode(-1, 0) '-1 Denotes Root node, 0th Height
    End Sub

    Public Sub LearnSeq(ByVal Sequence As String, ByVal Name As String)
        If Name = "" Then Name = "UnNamed"
        Dim CurrentHNode As HNode = Root
        Dim Split() As String = Sequence.Split(","c)
        Dim Val As Integer

        For i As Integer = 0 To Split.Length - 1
            Val = Convert.ToInt32(Split(i))
            If Not CurrentHNode.HNodeExists(Val) Then
                CurrentHNode.Add(Val)
            End If
            CurrentHNode = CurrentHNode.Ret(Val)
        Next
        CurrentHNode.Identifier = Name
    End Sub

    Public Function RecallSeq(ByVal Sequence As String) As String
        Dim CurrentHNode As HNode = Root
        Dim Split() As String = Sequence.Split(","c)
        Dim Val As Integer

        For i As Integer = 0 To Split.Length - 1
            Val = Convert.ToInt32(Split(i))
            If CurrentHNode.HNodeExists(Val) Then
                CurrentHNode = CurrentHNode.Ret(Val)
            Else
                Return ""
            End If

        Next
        Return CurrentHNode.Identifier
    End Function

End Class
