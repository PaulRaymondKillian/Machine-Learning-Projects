

Public Class Layer0
    Inherits Layer

    Public Image As Drawing.Bitmap
    Public Brightness As Double

    Public Sub New(ByVal pImage As Drawing.Bitmap, ByVal pBrightness As Double)
        MyBase.New(pImage.Width, pImage.Height)
        Image = pImage
        Brightness = pBrightness
    End Sub

    'Layer0 is special because its neurons inputs are external stimulation, not other layers, so it must be done special
    Public Overrides Sub CalcLayerNeurons()
        For y As Integer = 1 To Height
            For x As Integer = 1 To Width
                If Image.GetPixel(x - 1, y - 1).GetBrightness >= Brightness Then
                    Layer(x, y).Value = 0
                Else
                    Layer(x, y).Value = 1
                End If
            Next
        Next
    End Sub

    Public Overrides Sub SetForwardDendrites()
        'Layer0 is a special layer and has no forward dendrites because its input is external stimuli
        'However, this method must be overridden, so we will just stub it here and be happy.
    End Sub
End Class
