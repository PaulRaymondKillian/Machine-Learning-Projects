Public Class Form1
    Dim Image As Bitmap
    Dim Layer0 As Network.Layer0
    Dim Layer1 As Network.Layer1
    Dim Layer2 As Network.Layer2Trans
    Dim Layer3 As Network.Layer3Cortical
    Dim Layer4 As Network.Layer4CortHyper
    Dim Layer5 As Network.Layer5CortHyper
    Dim Layer6 As Network.Layer6CortHyper
    Dim Layer7 As Hippocampus.HLayer = New Hippocampus.HLayer
    Dim Layer8 As Hippocampus.HLayerSecondary = New Hippocampus.HLayerSecondary(Layer7)

    Dim v1, v2, v3, v4, v5, v6, v7, v8 As Integer
    Dim r1, r2, r3, r4 As Integer
    Dim buffer As Integer = 5

    Private Sub btnImg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImg.Click
        SetImage()
    End Sub

    Private Sub btnLayer0_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer0.Click
        CalcLayer0()
    End Sub

    Private Sub btnLayer1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer1.Click
        CalcLayer1()
    End Sub

    Private Sub btnLayer2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer2.Click
        CalcLayer2()
    End Sub

    Private Sub btnLayer3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer3.Click
        CalcLayer3()
    End Sub

    Private Sub btnLayer4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer4.Click
        CalcLayer4()
    End Sub

    Private Sub btnLayer5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer5.Click
        CalcLayer5()
    End Sub

    Private Sub btnLayer6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer6.Click
        CalcLayer6()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        RecalcColRowVals()
        ResizeformObjects()
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        RecalcColRowVals()
        ResizeformObjects()
    End Sub

    Public Sub RecalcColRowVals()
        Dim formWidth As Integer = Me.Width
        Dim formHeight As Integer = Me.Height

        v1 = 0
        v2 = formWidth * 0.125
        v3 = formWidth * 0.25
        v4 = formWidth * 0.375
        v5 = formWidth * 0.5
        v6 = formWidth * 0.6
        v7 = formWidth * 0.7
        v8 = formWidth * 0.85

        r1 = 20
        r2 = formHeight * 0.35
        r3 = formHeight * 0.65
        r4 = formHeight * 0.9
    End Sub

    Public Sub ResizeformObjects()

        btnImg.Left = v1 + buffer
        btnImg.Top = r1 + buffer
        pbImage.Left = v1 + buffer
        pbImage.Top = r1 + buffer + btnImg.Height + buffer
        pbImage.Width = (v2 - v1) - buffer
        pbImage.Height = (r2 - r1) - buffer - btnImg.Height - buffer

        btnLayer0.Left = v2 + buffer
        btnLayer0.Top = r1 + buffer
        pbLayer0.Left = v2 + buffer
        pbLayer0.Top = r1 + buffer + btnLayer0.Height + buffer
        pbLayer0.Width = (v3 - v2) - buffer
        pbLayer0.Height = (r2 - r1) - buffer - btnLayer0.Height - buffer

        btnLayer1.Left = v3 + buffer
        btnLayer1.Top = r1 + buffer
        pbLayer1.Left = v3 + buffer
        pbLayer1.Top = r1 + buffer + btnLayer1.Height + buffer
        pbLayer1.Width = (v4 - v3) - buffer
        pbLayer1.Height = (r2 - r1) - buffer - btnLayer1.Height - buffer

        btnLayer2.Left = v4 + buffer
        btnLayer2.Top = r1 + buffer
        pbLayer2.Left = v4 + buffer
        pbLayer2.Top = r1 + buffer + btnLayer2.Height + buffer
        pbLayer2.Width = (v5 - v4) - buffer
        pbLayer2.Height = (r2 - r1) - buffer - btnLayer2.Height - buffer

        btnLayer3.Left = v5 + buffer
        btnLayer3.Top = r1 + buffer
        pbLayer3.Left = v5 + buffer
        pbLayer3.Top = r1 + buffer + btnLayer3.Height + buffer
        pbLayer3.Width = 0.95 * v5 - buffer
        pbLayer3.Height = (r2 - r1) - buffer - btnLayer3.Height - buffer



        btnLayer4.Left = v1 + buffer
        btnLayer4.Top = r2 + buffer
        pbLayer4.Left = v1 + buffer
        pbLayer4.Top = r2 + buffer + btnLayer4.Height + buffer
        pbLayer4.Width = (v5 - v1) - buffer
        pbLayer4.Height = (r4 - r2) - buffer - btnLayer4.Height - buffer




        btnLayer5.Left = v5 + buffer
        btnLayer5.Top = r2 + buffer
        pbLayer5.Left = v5 + buffer
        pbLayer5.Top = r2 + buffer + btnLayer5.Height + buffer
        pbLayer5.Width = (v6 - v5) - buffer
        pbLayer5.Height = (r3 - r2) - buffer - btnLayer5.Height - buffer

        btnLayer6.Left = v6 + buffer
        btnLayer6.Top = r2 + buffer
        pbLayer6.Left = v6 + buffer
        pbLayer6.Top = r2 + buffer + btnLayer6.Height + buffer
        pbLayer6.Width = (v7 - v6) - buffer
        pbLayer6.Height = (r3 - r2) - buffer - btnLayer6.Height - buffer

        txtLayer6.Left = v7 + buffer
        txtLayer6.Top = r2 + buffer + btnLayer6.Height + buffer
        txtLayer6.Width = (v8 - v7) - buffer
        txtLayer6.Height = (r3 - r2) - buffer - btnLayer6.Height - buffer

        txtCircuitInfo.Left = v8 + buffer
        txtCircuitInfo.Top = r2 + buffer
        txtCircuitInfo.Width = (Me.Width * 0.12) - buffer
        txtCircuitInfo.Height = (r3 - r2) - buffer


        btnLayer7.Left = v5 + buffer
        btnLayer7.Top = r3 + buffer
        txtLayer7.Left = v5 + buffer
        txtLayer7.Top = r3 + buffer + btnLayer7.Height + buffer
        txtLayer7.Width = (v6 - v5) - buffer
        txtLayer7.Height = (r4 - r3) - buffer - btnLayer7.Height - buffer

        btnLayer8.Left = v6 + buffer
        btnLayer8.Top = r3 + buffer
        txtLayer8.Left = v6 + buffer
        txtLayer8.Top = r3 + buffer + btnLayer7.Height + buffer
        txtLayer8.Width = (v7 - v6) - buffer
        txtLayer8.Height = (r4 - r3) - buffer - btnLayer7.Height - buffer


    End Sub

    Private Sub btnLayer7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer7.Click
        CalcLayer7()
    End Sub

    Private Sub btnLayer8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLayer8.Click
        CalcLayer8()
    End Sub

    Public Sub SetImage()
        If ofdImg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Image = New Drawing.Bitmap(ofdImg.FileName)
            pbImage.Image = Image
        End If
    End Sub

    Public Sub CalcLayer0()
        Layer0 = New Network.Layer0(Image, 0.8)
        Layer0.CalcLayerNeurons()
        pbLayer0.Image = Layer0.OutputBitmap
    End Sub
    Public Sub CalcLayer1()
        Layer1 = New Network.Layer1(Layer0)
        pbLayer1.Image = Layer1.OutputBitmap
    End Sub
    Public Sub CalcLayer2()
        Layer2 = New Network.Layer2Trans(Layer1)
        pbLayer2.Image = Layer2.OutputCircuitBitmap
        txtCircuitInfo.Text += "Layer 2 Information:" & vbCrLf
        txtCircuitInfo.Text += "Circuits: " & Layer2.CountCircuits & vbCrLf
        txtCircuitInfo.Text += "Lengths: " & Layer2.GetCircuitLengths & vbCrLf
    End Sub
    Public Sub CalcLayer3()
        Layer3 = New Network.Layer3Cortical(Layer2)
        pbLayer3.Image = Layer3.OutputCircuitsBitmap
    End Sub
    Public Sub CalcLayer4()
        Layer4 = New Network.Layer4CortHyper(Layer3)
        pbLayer4.Image = Layer4.OutputCircuitsBitmap
    End Sub
    Public Sub CalcLayer5()
        Layer5 = New Network.Layer5CortHyper(Layer4)
        pbLayer5.Image = Layer5.OutputCircuitsBitmap
    End Sub
    Public Sub CalcLayer6()
        Layer6 = New Network.Layer6CortHyper(Layer5)
        pbLayer6.Image = Layer6.OutputCircuitsBitmap
        txtLayer6.Text = Layer6.OutputCircuitsString
    End Sub
    Public Sub CalcLayer7()
        'Layer7 = New Hippocampus.HLayer
        Dim Sequences(Layer6.Length - 1) As String

        For i As Integer = 0 To Sequences.Length - 1
            Sequences(i) = Layer6.Circuits(i).OutputString
        Next

        For i As Integer = 0 To Sequences.Length - 1
            If Not Layer7.DoIKnowThis(Sequences(i)) Then
                Layer7.Memorize(Sequences(i))
            End If
        Next

        Layer7.Think(Sequences)
        txtLayer7.Text += Layer7.CurrentThoughtsString & vbCrLf
    End Sub
    Public Sub CalcLayer8()
        'Layer8 = New Hippocampus.HLayerSecondary(Layer7)

        If Not Layer8.DoIKnowThis(Layer7.CurrentThoughtsString) Then
            Dim Name As String = InputBox("This image is not yet known", "Image Name:")
            Layer8.Memorize(Layer7.CurrentThoughtsString, Name)
        End If

        Layer8.Think()

        txtLayer8.Text += Layer8.CurrentThoughts(0).AttentionName & vbCrLf
    End Sub

    Private Sub btnAllLayers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllLayers.Click
        SetImage()
        CalcLayer0()
        CalcLayer1()
        CalcLayer2()
        CalcLayer3()
        CalcLayer4()
        CalcLayer5()
        CalcLayer6()
        CalcLayer7()
        CalcLayer8()
    End Sub
End Class
