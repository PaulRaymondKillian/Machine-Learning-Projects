Public Class Neuron

    Public ID As String = ""
    Public Value As Double = 0
    Public CircuitID As Integer = 0
    Public DendritesForward() As Dendrite
    'Public DendritesFeedback() As Dendrite
    Public DendritesLateral(7) As Dendrite

    Public Shared NeuronCount As Integer = 0
    Public Function NeuronCounter() As Integer
        NeuronCounter = NeuronCount
        NeuronCount += 1
    End Function


    Public Sub New(Optional ByVal IDPrefix As String = "")
        If Not IDPrefix = "" Then
            ID = IDPrefix & "-" & NeuronCounter()
        Else
            ID = NeuronCounter()
        End If
    End Sub

    Public Sub InitLateralDendrites(ByVal L0 As Neuron, _
                                     ByVal L1 As Neuron, _
                                     ByVal L2 As Neuron, _
                                     ByVal L3 As Neuron, _
                                     ByVal L4 As Neuron, _
                                     ByVal L5 As Neuron, _
                                     ByVal L6 As Neuron, _
                                     ByVal L7 As Neuron)

        If DendritesLateral(0) Is Nothing Then
            Dim newDendrite As New Dendrite(Me, L0)
            DendritesLateral(0) = newDendrite
            L0.DendritesLateral(4) = newDendrite
        End If
        If DendritesLateral(1) Is Nothing Then
            Dim newDendrite As New Dendrite(Me, L1)
            DendritesLateral(1) = newDendrite
            L1.DendritesLateral(5) = newDendrite
        End If
        If DendritesLateral(2) Is Nothing Then
            Dim newDendrite As New Dendrite(Me, L2)
            DendritesLateral(2) = newDendrite
            L2.DendritesLateral(6) = newDendrite
        End If
        If DendritesLateral(3) Is Nothing Then
            Dim newDendrite As New Dendrite(Me, L3)
            DendritesLateral(3) = newDendrite
            L3.DendritesLateral(7) = newDendrite
        End If
        If DendritesLateral(4) Is Nothing Then
            Dim newDendrite As New Dendrite(Me, L4)
            DendritesLateral(4) = newDendrite
            L4.DendritesLateral(0) = newDendrite
        End If
        If DendritesLateral(5) Is Nothing Then
            Dim newDendrite As New Dendrite(Me, L5)
            DendritesLateral(5) = newDendrite
            L5.DendritesLateral(1) = newDendrite
        End If
        If DendritesLateral(6) Is Nothing Then
            Dim newDendrite As New Dendrite(Me, L6)
            DendritesLateral(6) = newDendrite
            L6.DendritesLateral(2) = newDendrite
        End If
        If DendritesLateral(7) Is Nothing Then
            Dim newDendrite As New Dendrite(Me, L7)
            DendritesLateral(7) = newDendrite
            L7.DendritesLateral(3) = newDendrite
        End If
    End Sub

    Public Sub InitForwardDendrites(ByVal count As Integer)
        ReDim DendritesForward(count - 1)
    End Sub

    'Public Sub InitFeedbackDendrites(ByVal count As Integer)
    '    ReDim DendritesFeedback(count)
    'End Sub

    Public Overridable Sub CalcNeuron()
        Value = 0
        For Each Dendrite In DendritesForward
            Value += Dendrite.GetNeuronValue(ID)
        Next
    End Sub

End Class
