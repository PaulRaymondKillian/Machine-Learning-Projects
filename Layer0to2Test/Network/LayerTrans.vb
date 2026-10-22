Public MustInherit Class LayerTrans

    Public Width, Height As Integer
    Public Layer(,) As NeuronTrans

    Public Sub New(ByVal pWidth As Integer, ByVal pHeight As Integer)
        Width = pWidth
        Height = pHeight
        ReDim Layer(Width + 1, Height + 1)
        InitNeurons()
    End Sub

    Public Sub InitNeurons()
        For y As Integer = 0 To Height + 1
            For x As Integer = 0 To Width + 1
                Layer(x, y) = New NeuronTrans()
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

    Public MustOverride Sub SetSubNeuronLinks()

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

    Public Function OutputCircuitBitmap() As Drawing.Bitmap
        Dim bitmap As New Drawing.Bitmap(Width, Height)
        Dim Circuit1Color As Drawing.Color = Drawing.Color.Red
        Dim Circuit2Color As Drawing.Color = Drawing.Color.Blue
        Dim Circuit3Color As Drawing.Color = Drawing.Color.Green
        Dim Circuit4Color As Drawing.Color = Drawing.Color.Orange
        Dim Circuit5Color As Drawing.Color = Drawing.Color.LightBlue
        Dim Circuit6Color As Drawing.Color = Drawing.Color.Yellow
        Dim Circuit7Color As Drawing.Color = Drawing.Color.Purple


        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                If Layer(x, y).Value > 0.5 Then
                    bitmap.SetPixel(x - 1, y - 1, Drawing.Color.Black)
                Else
                    If Layer(x, y).SubNeuron.CircuitID > 0 Then
                        If Layer(x, y).SubNeuron.CircuitID Mod 7 = 1 Then
                            bitmap.SetPixel(x - 1, y - 1, Circuit1Color)
                        ElseIf Layer(x, y).SubNeuron.CircuitID Mod 7 = 2 Then
                            bitmap.SetPixel(x - 1, y - 1, Circuit2Color)
                        ElseIf Layer(x, y).SubNeuron.CircuitID Mod 7 = 3 Then
                            bitmap.SetPixel(x - 1, y - 1, Circuit3Color)
                        ElseIf Layer(x, y).SubNeuron.CircuitID Mod 7 = 4 Then
                            bitmap.SetPixel(x - 1, y - 1, Circuit4Color)
                        ElseIf Layer(x, y).SubNeuron.CircuitID Mod 7 = 5 Then
                            bitmap.SetPixel(x - 1, y - 1, Circuit5Color)
                        ElseIf Layer(x, y).SubNeuron.CircuitID Mod 7 = 6 Then
                            bitmap.SetPixel(x - 1, y - 1, Circuit6Color)
                        ElseIf Layer(x, y).SubNeuron.CircuitID Mod 7 = 0 Then
                            bitmap.SetPixel(x - 1, y - 1, Circuit7Color)
                        End If
                    Else
                        bitmap.SetPixel(x - 1, y - 1, Drawing.Color.White)
                    End If
                End If
            Next
        Next

        Return bitmap
    End Function

End Class
