Public MustInherit Class Layer

    Public Width, Height As Integer
    Public Layer(,) As Neuron

    Public Sub New(ByVal pWidth As Integer, ByVal pHeight As Integer)
        Width = pWidth
        Height = pHeight
        ReDim Layer(Width + 1, Height + 1)
        InitNeurons()
        InitLateralDendrites()
    End Sub

    Public Sub InitNeurons()
        For y As Integer = 0 To Height + 1
            For x As Integer = 0 To Width + 1
                Layer(x, y) = New Neuron()
            Next
        Next
    End Sub

    Public Sub InitLateralDendrites()

        ''Initialize the 'boundary' neurons as a special case of end-terminating neurons
        'Top Row Boundary Neurons - Set Self-Oriented Lateral Dendrites 0,1,2
        For x As Integer = 0 To Width + 1
            Dim newDendrite As New Dendrite(Layer(x, 0), Layer(x, 0))
            Layer(x, 0).DendritesLateral(0) = newDendrite
            Layer(x, 0).DendritesLateral(1) = newDendrite
            Layer(x, 0).DendritesLateral(2) = newDendrite
        Next

        'Bottom Row Boundary Neurons - Set Self-Oriented Lateral Dendrites 4,5,6
        For x As Integer = 0 To Width + 1
            Dim newDendrite As New Dendrite(Layer(x, Height + 1), Layer(x, Height + 1))
            Layer(x, Height + 1).DendritesLateral(4) = newDendrite
            Layer(x, Height + 1).DendritesLateral(5) = newDendrite
            Layer(x, Height + 1).DendritesLateral(6) = newDendrite
        Next

        'Left Row Boundary Neurons - Set Self-Oriented Lateral Dendrites 0,7,6
        For y As Integer = 0 To Height + 1
            Dim newDendrite As New Dendrite(Layer(0, y), Layer(0, y))
            Layer(0, y).DendritesLateral(0) = newDendrite
            Layer(0, y).DendritesLateral(7) = newDendrite
            Layer(0, y).DendritesLateral(6) = newDendrite
        Next

        'Right Row Boundary Neurons - Set Self-Oriented Lateral Dendrites 2,3,4
        For y As Integer = 0 To Height + 1
            Dim newDendrite As New Dendrite(Layer(Width + 1, y), Layer(Width + 1, y))
            Layer(Width + 1, y).DendritesLateral(2) = newDendrite
            Layer(Width + 1, y).DendritesLateral(3) = newDendrite
            Layer(Width + 1, y).DendritesLateral(4) = newDendrite
        Next

        'Set the rest of the "Normal" Neurons
        For x As Integer = 1 To Width
            For y As Integer = 1 To Height
                Layer(x, y).InitLateralDendrites(Layer(x - 1, y - 1), _
                                                Layer(x, y - 1), _
                                                Layer(x + 1, y - 1), _
                                                Layer(x + 1, y), _
                                                Layer(x + 1, y + 1), _
                                                Layer(x, y + 1), _
                                                Layer(x - 1, y + 1), _
                                                Layer(x - 1, y))
            Next
        Next

    End Sub

    Public Overridable Sub CalcLayerNeurons()
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                Layer(x, y).CalcNeuron()
            Next
        Next
    End Sub

    Public MustOverride Sub SetForwardDendrites()



    Public Function OutputBitmap() As Drawing.Bitmap
        Dim bitmap As New Drawing.Bitmap(Width, Height)

        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                If Layer(x, y).Value > 0.5 Then
                    bitmap.SetPixel(x - 1, y - 1, Drawing.Color.Black)
                Else
                    bitmap.SetPixel(x - 1, y - 1, Drawing.Color.White)
                End If
            Next
        Next
        Return bitmap
    End Function

End Class
