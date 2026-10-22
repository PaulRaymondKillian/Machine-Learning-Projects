Public Class HLayerSecondary
    Inherits HLayer

    Public PrevHLayer As HLayer

    Public Sub New(ByVal pHLayer As HLayer)
        PrevHLayer = pHLayer
    End Sub

    Public Overloads Function Think() As HThought
        Dim Thoughts(0) As HThought
        Thoughts(0) = Think(PrevHLayer.CurrentThoughts)
        CurrentThoughts = Thoughts
        Return Thoughts(0)
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
End Class
