Module Module1

    Sub Main()
        Dim HLayer As New HLayer
        Dim HLayer1 As New HLayerSecondary(HLayer)
        HLayer.Think("1,2,3,4")
        HLayer.Memorize("1,2,3,4")
        HLayer.Memorize("1,2,3,2")
        HLayer.Memorize("2,2,3,4")
        Dim Thought As HThought = HLayer.Think("2,2,3,4,5")
        MsgBox(Thought.Attention)
        MsgBox(Thought.Known)

        Dim arr(2) As String
        arr(2) = "1,2,3,4"
        arr(1) = "1,2,3,2"
        arr(0) = "2,2,3,4"
        HLayer1.Memorize(arr)
        HLayer.Think(arr)
        HLayer1.Think()

        arr(0) = "1,2,3,4"
        arr(2) = "1,2,3,2"
        arr(1) = "2,2,3,4"
        HLayer.Think(arr)
        HLayer1.Think()

    End Sub

End Module
