Public Class Layer2Trans
    Inherits LayerTrans

    Public Layer1 As Layer1

    Public Sub New(ByVal pLayer1 As Layer1)
        MyBase.New(pLayer1.Width, pLayer1.Height)
        Layer1 = pLayer1
        SetSubNeuronLinks()
        CalcLayerNeurons()
    End Sub

    Public Overrides Sub SetSubNeuronLinks()
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                Layer(x, y).SubNeuron = Layer1.Layer(x, y)
            Next
        Next
    End Sub

    Public Function CountCircuits() As Integer
        Dim count As Integer = 0
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                If Layer(x, y).Value > 0 Then
                    count += 1
                End If
            Next
        Next
        Return count
    End Function

    Public Function GetCircuitLengths() As String
        Dim s As String = ""
        Dim count As Integer
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                If Layer(x, y).Value > 0 Then
                    count = 0
                    Dim CurrentNeuron As NeuronTrans = Layer(x, y)
                    Do
                        count += 1
                        CurrentNeuron = CurrentNeuron.NextTrans
                        If CurrentNeuron Is Nothing Then
                            Exit Do
                        End If
                    Loop Until CurrentNeuron.Equals(Layer(x, y))
                    s = s & "C" & Layer(x, y).CircuitID & ":" & count & ", "
                End If
            Next
        Next
        Return s
    End Function

End Class
