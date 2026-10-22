Public Class Grid0
    Inherits Grid

    Public Sub New(ByVal pSize As Double)
        MyBase.New(pSize)
    End Sub

    Public Sub ImportBitmap(ByVal Image As Drawing.Bitmap)
        If Image.Size.Height > Size Or Image.Size.Width > Size Then
            Throw New Exception("Invalid Image Loaded - Image is too large. Must not be larger than " & Size & "x" & Size & ".")
        End If

        For i As Integer = 0 To Image.Width - 1
            For j As Integer = 0 To Image.Height - 1
                If Image.GetPixel(i, j).GetBrightness > 0.5 Then
                    Grid(i, j).Value = 0
                Else
                    Grid(i, j).Value = 1
                End If
            Next
        Next
    End Sub

    Public Function OutputBitmap() As Drawing.Bitmap
        Dim bitmap As New Drawing.Bitmap(Size, Size)

        For i As Integer = 0 To Size - 1
            For j As Integer = 0 To Size - 1
                If Grid(i, j).Value > 0.5 Then
                    bitmap.SetPixel(i, j, Drawing.Color.Black)
                Else
                    bitmap.SetPixel(i, j, Drawing.Color.White)
                End If
            Next
        Next

        Return bitmap
    End Function

End Class
