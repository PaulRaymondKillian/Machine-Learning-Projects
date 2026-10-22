Public Class Layer4CortHyper

    Public Layer3 As Layer3Cortical
    Public Circuits() As NeuronCortical4
    Public Length As Integer = 0



    Public Sub New(ByVal pLayer3 As Layer3Cortical)
        Layer3 = pLayer3
        Length = Layer3.Length
        ReDim Circuits(Length - 1)
        InitNeurons()
        CalcLayerNeurons()                  'First Image (Top-Most)
        FilterLayerCurvedLineSegments()     'Second Image
        FilterLayerNeuronValues()           'Third Image
        'FilterLayerStraightLineCohesion()   'Fourth Image (Bottom-Most)
    End Sub

    Public Sub InitNeurons()
        For i As Integer = 0 To Length - 1
            Circuits(i) = New NeuronCortical4(Layer3.Circuits(i))
        Next
    End Sub

    'Public Sub SetTransNeuronLinks(ByVal LayerTrans As Layer2Trans)
    '    For y As Integer = 1 To Height
    '        For x As Integer = 1 To Width
    '            Layer(x, y).TransNeuron = LayerTrans.Layer(x, y)
    '        Next
    '    Next
    'End Sub

    'Public Sub SetSubTransNeuronLinks()
    '    For y As Integer = 1 To Height
    '        For x As Integer = 1 To Width
    '            'Only set Sub-Trans Links if TransNeuron is active
    '            If Layer(x, y).TransNeuron.Value > 0 Then
    '                Layer(x, y).SetSubTransNeuronLinks()
    '            End If
    '        Next
    '    Next
    'End Sub

    Public Sub CalcLayerNeurons()
        For i As Integer = 0 To Length - 1
            Circuits(i).CalcNeuron()
        Next
    End Sub

    Public Sub FilterLayerCurvedLineSegments()
        Dim CurrentCort As NeuronCortical4
        For i As Integer = 0 To Length - 1
            CurrentCort = Circuits(i)
            CurrentCort.FilterCurvedLineSegments()
        Next
    End Sub

    Public Sub FilterLayerNeuronValues()
        Dim CurrentCort As NeuronCortical4
        For i As Integer = 0 To Length - 1
            CurrentCort = Circuits(i)
            'Do
            CurrentCort.FilterNeuronValues3()
            'CurrentCort = CurrentCort.NextCorticalNeuron
            'Loop Until CurrentCort.Equals(Circuits(i))
        Next
    End Sub

    Public Sub FilterLayerStraightLineCohesion()
        Dim Currentcort As NeuronCortical4
        For i As Integer = 0 To Length - 1
            Currentcort = Circuits(i)
            Currentcort.FilterStraightLineCohesion()
        Next
    End Sub

    'Public Sub CreateCircuitArray()
    '    For y As Integer = 1 To Height
    '        For x As Integer = 1 To Width
    '            If Layer(x, y).Value > 0 Then
    '                ReDim Preserve Circuits(Length)
    '                Circuits(Length) = Layer(x, y)
    '                Length += 1
    '            End If
    '        Next
    '    Next
    'End Sub



    Public Function OutputCircuitsBitmap() As Drawing.Bitmap

        Dim bitmapHi As Integer = Length * 69
        Dim bitmap As Drawing.Bitmap

        Dim Circuit1Color As Drawing.Color = Drawing.Color.Red
        Dim Circuit2Color As Drawing.Color = Drawing.Color.Blue
        Dim Circuit3Color As Drawing.Color = Drawing.Color.Green
        Dim Circuit4Color As Drawing.Color = Drawing.Color.Orange
        Dim Circuit5Color As Drawing.Color = Drawing.Color.LightBlue
        Dim Circuit6Color As Drawing.Color = Drawing.Color.Yellow
        Dim Circuit7Color As Drawing.Color = Drawing.Color.Purple

        'Copy the circuit bitmaps onto 1 large masater bitmap image
        'This is a stupid way to do this, but it doesnt matter, we just want it done, arent looking for performance right now
        Dim tempBitmap As Drawing.Bitmap
        Dim border As Drawing.Color
        For i As Integer = 0 To Length - 1
            If Circuits(i).LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 1 Then
                border = Circuit1Color
            ElseIf Circuits(i).LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 2 Then
                border = Circuit2Color
            ElseIf Circuits(i).LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 3 Then
                border = Circuit3Color
            ElseIf Circuits(i).LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 4 Then
                border = Circuit4Color
            ElseIf Circuits(i).LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 5 Then
                border = Circuit5Color
            ElseIf Circuits(i).LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 6 Then
                border = Circuit6Color
            ElseIf Circuits(i).LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 0 Then
                border = Circuit7Color
            End If
            tempBitmap = Circuits(i).OutputBitmap(border)
            If i = 0 Then
                'The first image of he circuit will have the longest length, so we need to use that value
                bitmap = New Drawing.Bitmap(tempBitmap.Width, bitmapHi)
            End If
            For y As Integer = 0 To tempBitmap.Height - 1
                For x As Integer = 0 To tempBitmap.Width - 1
                    bitmap.SetPixel(x, (y + (i * tempBitmap.Height)), tempBitmap.GetPixel(x, y))
                Next
            Next
        Next

        Return bitmap
    End Function

    Public Function MaxCircuitLength() As Integer
        Dim maxlength As Integer = 0
        Dim tempLength As Integer

        For i As Integer = 0 To Length - 1
            tempLength = Circuits(i).Length.Value
            If tempLength > maxlength Then
                maxlength = tempLength
            End If
        Next
        Return maxlength
    End Function

End Class
