Public Class Dendrite

    Public ID As String
    Public Neurons As New Hashtable
    Public Value As Double
    Public RecentlyActive As Boolean


    Public Sub New(ByVal Neuron1 As Neuron, ByVal Neuron2 As Neuron)
        Neurons.Add(Neuron2.ID, Neuron1)
        If Not Neuron1.ID = Neuron2.ID Then
            Neurons.Add(Neuron1.ID, Neuron2)
        End If
        ID = Neuron1.ID & "-" & Neuron2.ID
        Value = 0
        RecentlyActive = False
    End Sub

    Public Function GetNeuronValue(ByVal SourceNeuronID As String) As Double
        Return Value * Neurons(SourceNeuronID).Value
    End Function

End Class
