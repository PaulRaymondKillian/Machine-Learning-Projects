Public Class Layer3Cortical

    Public Width, Height As Integer
    Public Layer(,) As NeuronCortical3
    Public Layer2 As Layer2Trans
    Public Circuits() As NeuronCortical3
    Public Length As Integer = 0


    Public Sub New(ByVal pLayer2 As Layer2Trans)
        Layer2 = pLayer2
        Width = Layer2.Width
        Height = Layer2.Height
        ReDim Layer(Width + 1, Height + 1)
        InitNeurons()
        SetTransNeuronLinks(Layer2)
        SetSubTransNeuronLinks()
        CalcLayerNeurons()
        CreateCircuitArray()
    End Sub

    Public Sub InitNeurons()
        For y As Integer = 0 To Height + 1
            For x As Integer = 0 To Width + 1
                Layer(x, y) = New NeuronCortical3()
            Next
        Next
    End Sub

    Public Sub SetTransNeuronLinks(ByVal LayerTrans As Layer2Trans)
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                Layer(x, y).TransNeuron = LayerTrans.Layer(x, y)
            Next
        Next
    End Sub

    Public Sub SetSubTransNeuronLinks()
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                'Only set Sub-Trans Links if TransNeuron is active
                If Layer(x, y).TransNeuron.Value > 0 Then
                    Layer(x, y).SetSubTransNeuronLinks()
                End If
            Next
        Next
    End Sub

    Public Sub CalcLayerNeurons()
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                Layer(x, y).CalcNeuron()
            Next
        Next
    End Sub

    Public Sub CreateCircuitArray()
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                If Layer(x, y).Value > 0 Then
                    ReDim Preserve Circuits(Length)
                    Circuits(Length) = Layer(x, y)
                    Length += 1
                End If
            Next
        Next
    End Sub

    Public Function OutputCircuitsBitmap() As Drawing.Bitmap
        Dim bitmapLen As Integer = MaxCircuitLength() + 2
        Dim bitmapHi As Integer = Length * 18
        Dim bitmap As New Drawing.Bitmap(bitmapLen, bitmapHi)

        Dim Circuit1Color As Drawing.Color = Drawing.Color.Red
        Dim Circuit2Color As Drawing.Color = Drawing.Color.Blue
        Dim Circuit3Color As Drawing.Color = Drawing.Color.Green
        Dim Circuit4Color As Drawing.Color = Drawing.Color.Orange
        Dim Circuit5Color As Drawing.Color = Drawing.Color.LightBlue
        Dim Circuit6Color As Drawing.Color = Drawing.Color.Yellow
        Dim Circuit7Color As Drawing.Color = Drawing.Color.Purple

        'Set all pixels to white and add line-delimiters
        'For y As Integer = 0 To bitmapHi - 1
        '    For x As Integer = 0 To bitmapLen - 1
        '        bitmap.SetPixel(x, y, Drawing.Color.White)
        '    Next
        'Next

        'Copy the circuit bitmaps onto 1 large masater bitmap image
        'This is a stupid way to do this, but it doesnt matter, we just want it done, arent looking for performance right now
        Dim tempBitmap As Drawing.Bitmap
        Dim border As Drawing.Color
        For i As Integer = 0 To Length - 1
            If Circuits(i).TransNeuron.SubNeuron.CircuitID Mod 7 = 1 Then
                border = Circuit1Color
            ElseIf Circuits(i).TransNeuron.SubNeuron.CircuitID Mod 7 = 2 Then
                border = Circuit2Color
            ElseIf Circuits(i).TransNeuron.SubNeuron.CircuitID Mod 7 = 3 Then
                border = Circuit3Color
            ElseIf Circuits(i).TransNeuron.SubNeuron.CircuitID Mod 7 = 4 Then
                border = Circuit4Color
            ElseIf Circuits(i).TransNeuron.SubNeuron.CircuitID Mod 7 = 5 Then
                border = Circuit5Color
            ElseIf Circuits(i).TransNeuron.SubNeuron.CircuitID Mod 7 = 6 Then
                border = Circuit6Color
            ElseIf Circuits(i).TransNeuron.SubNeuron.CircuitID Mod 7 = 0 Then
                border = Circuit7Color
            End If
            tempBitmap = Circuits(i).OutputBitmap(border)
            For y As Integer = 0 To tempBitmap.Height - 1
                For x As Integer = 0 To tempBitmap.Width - 1
                    bitmap.SetPixel(x, (y + (i * 18)), tempBitmap.GetPixel(x, y))
                Next
            Next
        Next

        Return bitmap
    End Function

    Public Function MaxCircuitLength() As Integer
        Dim maxlength As Integer = 0
        Dim tempLength As Integer

        For i As Integer = 0 To Length - 1
            tempLength = Circuits(i).CircuitLength
            If tempLength > maxlength Then
                maxlength = tempLength
            End If
        Next
        Return maxlength
    End Function

End Class
