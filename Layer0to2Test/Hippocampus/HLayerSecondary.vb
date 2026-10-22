Public Class HLayerSecondary
    Inherits HLayer

    Public PrevHLayer As HLayer
    Public Names As New Hashtable

    Public Sub New(ByVal pHLayer As HLayer)
        PrevHLayer = pHLayer
        Names.Add(-1, "Unknown Object")
    End Sub

    Public Overloads Function Think() As HThought
        Dim Thoughts(0) As HThought
        Thoughts(0) = Think(PrevHLayer.CurrentThoughts)
        CurrentThoughts = Thoughts
        If Not Names.Item(Thoughts(0).Attention) = Nothing Then
            Thoughts(0).AttentionName = Names.Item(Thoughts(0).Attention)
        End If
        Return Thoughts(0)
    End Function

    Public Overloads Function Memorize(ByVal Sequence As String, ByVal Name As String) As Integer
        Dim ID As Integer = Memorize(Sequence)
        SetName(ID, Name)
        Return ID
    End Function

    Public Overloads Function Memorize(ByVal Sequence() As String) As Integer
        Dim Name As Integer
        For i As Integer = 0 To Sequence.Length - 1
            PrevHLayer.Memorize(Sequence(i))
        Next

        PrevHLayer.Think(Sequence)
        Dim s As String = ""
        For i As Integer = 0 To PrevHLayer.CurrentThoughts.Length - 1
            If PrevHLayer.CurrentThoughts(i).Attention <> -1 Then
                s += PrevHLayer.CurrentThoughts(i).Attention.ToString
            End If
            If i <> PrevHLayer.CurrentThoughts.Length - 1 Then
                s += ","
            End If
        Next

        Name = Memorize(s)

        Return Name

    End Function

    Public Sub SetName(ByVal ID As Integer, ByVal Name As String)
        Names.Add(ID, Name)
    End Sub

End Class
