Public Class Layer5CortHyper

    Public Layer4 As Layer4CortHyper
    Public Circuits() As NeuronCortical5
    Public Length As Integer = 0


    Public Sub New(ByVal pLayer4 As Layer4CortHyper)
        Layer4 = pLayer4
        Length = Layer4.Length
        ReDim Circuits(Length - 1)
        InitNeurons()
        CalcLayerNeurons()
    End Sub

    Public Sub InitNeurons()
        For i As Integer = 0 To Length - 1
            Circuits(i) = New NeuronCortical5(Layer4.Circuits(i))
        Next
    End Sub

    Public Sub CalcLayerNeurons()
        For i As Integer = 0 To Length - 1
            Circuits(i).CalcNeuron()
        Next
    End Sub


    Public Function OutputCircuitsBitmap() As Drawing.Bitmap
        'Dim bitmapLen As Integer = MaxCircuitLength() + 1
        'Dim bitmapHi As Integer = Length * 17 + 1
        'Dim bitmap As New Drawing.Bitmap(bitmapLen, bitmapHi)

        ''Set all pixels to white and add line-delimiters
        'For y As Integer = 1 To bitmapHi - 1
        '    For x As Integer = 1 To bitmapLen - 1
        '        If y Mod 17 = 0 Then
        '            bitmap.SetPixel(x, y, Drawing.Color.Red)
        '        Else
        '            bitmap.SetPixel(x, y, Drawing.Color.White)
        '        End If
        '    Next
        'Next

        ''Copy the circuit bitmaps onto 1 large masater bitmap image
        ''This is a stupid way to do this, but it doesnt matter, we just want it done, arent looking for performance right now
        'Dim tempBitmap As Drawing.Bitmap
        'For i As Integer = 0 To Length - 1
        '    tempBitmap = Circuits(i).OutputBitmap(Drawing.Color.Red)
        '    For y As Integer = 1 To tempBitmap.Height - 1
        '        For x As Integer = 1 To tempBitmap.Width - 1
        '            bitmap.SetPixel(x, (y + (i * 17)), tempBitmap.GetPixel(x, y))
        '        Next
        '    Next
        'Next

        'Return bitmap
        Dim bitmapLen As Integer = MaxCircuitLength() + 2
        Dim bitmapHi As Integer = Length * 53
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
            If Circuits(i).LowerCortNeuron.LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 1 Then
                border = Circuit1Color
            ElseIf Circuits(i).LowerCortNeuron.LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 2 Then
                border = Circuit2Color
            ElseIf Circuits(i).LowerCortNeuron.LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 3 Then
                border = Circuit3Color
            ElseIf Circuits(i).LowerCortNeuron.LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 4 Then
                border = Circuit4Color
            ElseIf Circuits(i).LowerCortNeuron.LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 5 Then
                border = Circuit5Color
            ElseIf Circuits(i).LowerCortNeuron.LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 6 Then
                border = Circuit6Color
            ElseIf Circuits(i).LowerCortNeuron.LowerCortNeuron.TransNeuron.SubNeuron.CircuitID Mod 7 = 0 Then
                border = Circuit7Color
            End If
            tempBitmap = Circuits(i).OutputBitmap(border)
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
