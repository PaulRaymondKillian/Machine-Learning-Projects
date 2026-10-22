Public Class Layer1
    Inherits Layer

    Public Layer0 As Layer0

    Public Sub New(ByVal pLayer0 As Layer0)
        MyBase.New(pLayer0.Width, pLayer0.Height)
        Layer0 = pLayer0
        SetForwardDendrites()
        CalcLayerNeurons()
        Do Until Not FilterActiveNeurons1Connection()
        Loop

        Do Until Not FilterActiveNeurons2AdjacentConnections()
        Loop
        'For i As Integer = 0 To 0
        '    FilterActiveNeurons1Connection()
        'Next
    End Sub

    Public Overrides Sub SetForwardDendrites()
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                Layer(x, y).InitForwardDendrites(9)
                SetLayer1Topology(Layer(x, y), Layer0.Layer(x, y))
            Next
        Next
    End Sub


    Public Function FilterActiveNeurons2AdjacentConnections() As Boolean
        Dim DeactivatedNeuron As Boolean = False
        'Remove de-activate neurons that are active and do not have 2 adjacent neurons (To resolve problems that occur in higher layers caused by this)
        Dim countAdjAdjNeurons As Integer
        Dim countAdjNeurons As Integer

        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                countAdjAdjNeurons = 0
                countAdjNeurons = 0
                'Although this is neuron level logic, this purpose for this action is layer-level, so lets add the logic to
                'the layer itself rather than adding neuron logic that makes no sense without high level layer perspective
                If Layer(x, y).Value > 0 Then
                    For i As Integer = 0 To 7
                        If Layer(x, y).DendritesLateral(i).Neurons(Layer(x, y).ID).value > 0 _
                            And Layer(x, y).DendritesLateral((i + 1) Mod 8).Neurons(Layer(x, y).ID).value > 0 Then
                            countAdjAdjNeurons += 1
                        End If
                        If Layer(x, y).DendritesLateral(i).Neurons(Layer(x, y).ID).value > 0 Then
                            countAdjNeurons += 1
                        End If
                    Next
                    If countAdjAdjNeurons > 0 And countAdjNeurons = 2 Then
                        'De-activate this neuron
                        Layer(x, y).Value = 0
                        DeactivatedNeuron = True
                    End If
                End If
            Next
        Next
        Return DeactivatedNeuron
    End Function

    Public Function FilterActiveNeurons1Connection() As Boolean
        Dim DeactivatedNeuron As Boolean = False
        'Remove de-activate neurons that are active and do not have 2 adjacent neurons (To resolve problems that occur in higher layers caused by this)
        Dim countAdjNeurons As Integer = 0
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                countAdjNeurons = 0
                'Although this is neuron level logic, this purpose for this action is layer-level, so lets add the logic to
                'the layer itself rather than adding neuron logic that makes no sense without high level layer perspective
                If Layer(x, y).Value > 0 Then
                    For i As Integer = 0 To 7
                        If Layer(x, y).DendritesLateral(i).Neurons(Layer(x, y).ID).value > 0 Then
                            countAdjNeurons += 1
                        End If
                    Next
                    If countAdjNeurons = 1 Then
                        'De-activate this neuron
                        Layer(x, y).Value = 0
                        DeactivatedNeuron = True
                    End If
                End If
            Next
        Next
        Return DeactivatedNeuron
    End Function

    Public Sub SetLayer1Topology(ByVal UpperNeuron As Neuron, ByVal LowerNeuron As Neuron)
        Dim newDendrite1 As New Dendrite(UpperNeuron, LowerNeuron.DendritesLateral(0).Neurons(LowerNeuron.ID))
        Dim newDendrite2 As New Dendrite(UpperNeuron, LowerNeuron.DendritesLateral(1).Neurons(LowerNeuron.ID))
        Dim newDendrite3 As New Dendrite(UpperNeuron, LowerNeuron.DendritesLateral(2).Neurons(LowerNeuron.ID))
        Dim newDendrite4 As New Dendrite(UpperNeuron, LowerNeuron.DendritesLateral(7).Neurons(LowerNeuron.ID))
        Dim newDendrite5 As New Dendrite(UpperNeuron, LowerNeuron)
        Dim newDendrite6 As New Dendrite(UpperNeuron, LowerNeuron.DendritesLateral(3).Neurons(LowerNeuron.ID))
        Dim newDendrite7 As New Dendrite(UpperNeuron, LowerNeuron.DendritesLateral(6).Neurons(LowerNeuron.ID))
        Dim newDendrite8 As New Dendrite(UpperNeuron, LowerNeuron.DendritesLateral(5).Neurons(LowerNeuron.ID))
        Dim newDendrite9 As New Dendrite(UpperNeuron, LowerNeuron.DendritesLateral(4).Neurons(LowerNeuron.ID))
        newDendrite1.Value = 0
        newDendrite2.Value = 1
        newDendrite3.Value = 0
        newDendrite4.Value = 1
        newDendrite5.Value = -10
        newDendrite6.Value = 1
        newDendrite7.Value = 0
        newDendrite8.Value = 1
        newDendrite9.Value = 0
        UpperNeuron.DendritesForward(0) = newDendrite1
        UpperNeuron.DendritesForward(1) = newDendrite2
        UpperNeuron.DendritesForward(2) = newDendrite3
        UpperNeuron.DendritesForward(3) = newDendrite4
        UpperNeuron.DendritesForward(4) = newDendrite5
        UpperNeuron.DendritesForward(5) = newDendrite6
        UpperNeuron.DendritesForward(6) = newDendrite7
        UpperNeuron.DendritesForward(7) = newDendrite8
        UpperNeuron.DendritesForward(8) = newDendrite9
    End Sub

End Class
