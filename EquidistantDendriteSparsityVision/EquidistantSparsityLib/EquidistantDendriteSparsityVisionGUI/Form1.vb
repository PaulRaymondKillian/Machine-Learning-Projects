Public Class Form1

    Public Grid0 As EquidistantSparsityLib.Grid0
    Public Grid1 As EquidistantSparsityLib.Grid1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Grid0 = New EquidistantSparsityLib.Grid0(100)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim newBitmap As New Drawing.Bitmap(OpenFileDialog1.FileName)
            OriginalImage.Image = newBitmap
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Grid0.ImportBitmap(OriginalImage.Image)
        PreImage.Image = Grid0.OutputBitmap
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Grid1 = New EquidistantSparsityLib.Grid1(Grid0)
    End Sub
End Class
