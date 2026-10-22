Public Class Form1
        Dim Receptor As new SpatialPooler1.SenseReceptor(100,100)
        Dim Region1 As New SpatialPooler1.Region(Receptor.GetColumns,4,10,10)
        Dim Region2 As New SpatialPooler1.Region(Region1.GetColumns,4,4,10)
        Dim Region3 As New SpatialPooler1.Region(Region2.GetColumns,4,4,10)

Private Sub Form1_Load( ByVal sender As System.Object,  ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Random As New Random(System.DateTime.Now.Ticks Mod System.Int32.MaxValue)
        Dim InputW As Integer = 100
        Dim InputH As Integer = 100
        Dim InputArray(InputW-1, InputH-1) As Byte
        For i As Integer = 0 to InputW-1
            For j As Integer = 0 to InputH-1
                InputArray(i,j) = Random.Next mod 2
            Next
        Next

        Receptor.SetReceptorsInput(InputArray)
        PictureBox1.Image = Receptor.OutputBitmap
        Region1.CalcOverlap(8)
        PictureBox2.Image = Region1.OutputBitmap
End Sub
End Class
