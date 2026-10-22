Imports System.IO

Public Class Form1
    Private MVController As MachineVisionLib.MachineVisionController
    Private Image1 As MachineVisionLib.Image
    Private ImageHeight = 100
    Private ImageWidth = 100

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        MVController = New MachineVisionLib.MachineVisionController(ImageHeight * ImageWidth)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Image1 = New MachineVisionLib.Image(pbLibraryImage.Image, "asdfas", MachineVisionLib.SeparationType.BlackAndWhite, 0.75)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim newBitMap As New Drawing.Bitmap(Image1.Width, Image1.Height)
        pbPreImage.Image = newBitMap
        For y As Integer = 0 To Image1.Height - 1
            For x As Integer = 0 To Image1.Width - 1
                newBitMap.SetPixel(x, y, Image1.GetPixel(x, y))
            Next
        Next
    End Sub

    Private Sub btnAddImgLib_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddImgLib.Click
        If ofdAddImg.ShowDialog = Windows.Forms.DialogResult.OK Then
            AddImageToLibrary(ofdAddImg.FileName)
        End If
        If MVController.ImageCount > 0 Then
            btnCalcWeightMatrix.Enabled = True
        End If
        MVController.ResetWeightMatrix()
        btnRecoverImage.Enabled = False
    End Sub

    Public Sub AddImageToLibrary(ByVal filepath As String)
        Dim tempBitMap As New Drawing.Bitmap(filepath)
        If tempBitMap.Height <= ImageHeight And tempBitMap.Width <= ImageWidth Then
            lbImageLibrary.Enabled = False
            Dim tempImg As New MachineVisionLib.Image(tempBitMap, ReturnFilename(filepath), ImageWidth, ImageHeight)
            MVController.AddImage(tempImg)
            UpdateLibraryListBox()
            lbImageLibrary.Enabled = True
            lbImageLibrary.SelectedIndex = MVController.ImageCount - 1
        Else
            MsgBox("Invalid Image Size; Max Height: " & ImageHeight & ", Max Width: " & ImageWidth & "; " & filepath & " was not added to library.")
        End If
    End Sub

    Public Sub UpdateLibraryListBox()
        lbImageLibrary.Items.Clear()
        For i As Integer = 0 To MVController.ImageCount - 1
            lbImageLibrary.Items.Add(MVController.GetImage(i).Name)
        Next
    End Sub

    Private Function ReturnFilename(ByVal File As String)
        Dim FileName As String
        Dim Split As String()
        Split = File.Split("\")
        FileName = Split(Split.Count - 1)
        Return FileName
    End Function

    Private Sub lbImageLibrary_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbImageLibrary.SelectedIndexChanged
        UpdateLibraryImage()
    End Sub

    Public Sub UpdateLibraryImage()
        If rdoImgLibOriginal.Checked = True Then
            pbLibraryImage.Image = MVController.GetImage(lbImageLibrary.SelectedIndex).Bitmap
        Else
            pbLibraryImage.Image = MVController.GetImage(lbImageLibrary.SelectedIndex).ReturnFilteredBitmap(True)
        End If
    End Sub

    Private Sub rdoImgLibOriginal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoImgLibOriginal.CheckedChanged
        If Not lbImageLibrary.SelectedIndex = -1 Then
            UpdateLibraryImage()
        End If
    End Sub

    Private Sub btnRmvImgLib_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRmvImgLib.Click
        If Not lbImageLibrary.SelectedIndex = -1 Then
            Dim index As Integer = lbImageLibrary.SelectedIndex
            MVController.RemoveImage(index)
            UpdateLibraryListBox()
            If lbImageLibrary.Items.Count > 1 And index > 0 Then
                lbImageLibrary.SelectedIndex = index - 1
            ElseIf lbImageLibrary.Items.Count = 0 Then
                lbImageLibrary.SelectedIndex = -1
                pbLibraryImage.Image = Nothing
            Else
                lbImageLibrary.SelectedIndex = 0
            End If
        End If
        If MVController.ImageCount = 0 Then
            btnCalcWeightMatrix.Enabled = False
        End If
        MVController.ResetWeightMatrix()
        btnRecoverImage.Enabled = False
    End Sub

    Private Sub btnImportPreImg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImportPreImg.Click
        pbPostImage.Image = Nothing
        If ofdPreImg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim tempBitMap As New Drawing.Bitmap(ofdPreImg.FileName)
            If tempBitMap.Height <= 250 And tempBitMap.Width <= 200 Then
                Dim tempImg As New MachineVisionLib.Image(tempBitMap, ReturnFilename(ofdPreImg.FileName), ImageWidth, ImageHeight)
                MVController.SetInputImage(tempImg)
                pbPreImage.Image = MVController.GetInputBitmap
                'pbPostImage.Image = MVController.GetResultBitmap
            Else
                MsgBox("Invalid Image Size; Max Height: 250, Max Width: 200")
            End If
        End If

        If MVController.isWeightMatrixNothing = False And Not MVController.GetInputImage Is Nothing Then
            btnRecoverImage.Enabled = True
        Else
            btnRecoverImage.Enabled = False
        End If

    End Sub

    Private Sub btnRecoverImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRecoverImage.Click
        MVController.StepRunHopfield()
        pbPostImage.Image = MVController.GetResultBitmap
        MVController.SetOutputAsInput()
        probarRecoverProcess.Value = 100
        Dim MostSimilarImage As MachineVisionLib.Image = MVController.FindMostSimilarImage(MVController.GetResultImage)
        lblPostImageName.Text = "I am " & MVController.PercentSimilarImage(MVController.GetResultImage, MostSimilarImage) & "% sure this is " & MostSimilarImage.Name
    End Sub

    Private Sub btnCalcWeightMatrix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcWeightMatrix.Click
        probarRecoverProcess.Value = 0
        Dim dr As Windows.Forms.Form = CalcWeightMatrix
        dr.Show()
        MVController.ResetWeightMatrix()
        MVController.CalcWeightMatrix()
        If MVController.isWeightMatrixNothing = False Then
            btnCalcWeightMatrix.Enabled = False
        End If
        If MVController.isWeightMatrixNothing = False And Not MVController.GetInputImage Is Nothing Then
            btnRecoverImage.Enabled = True
        Else
            btnRecoverImage.Enabled = False
        End If

        dr.Close()

    End Sub

    Private Sub btnAddImgLibFld_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddImgLibFld.Click
        If fbdAddImgFld.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim files() As String
            files = Directory.GetFiles(fbdAddImgFld.SelectedPath)
            For Each file In files
                If InStr(file.ToString, ".bmp") > 0 Then
                    AddImageToLibrary(file)
                End If
            Next
        End If
        If MVController.ImageCount > 0 Then
            btnCalcWeightMatrix.Enabled = True
        End If
        MVController.ResetWeightMatrix()
        btnRecoverImage.Enabled = False
    End Sub
End Class
