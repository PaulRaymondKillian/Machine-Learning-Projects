<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.pbLibraryImage = New System.Windows.Forms.PictureBox
        Me.pbPreImage = New System.Windows.Forms.PictureBox
        Me.lbImageLibrary = New System.Windows.Forms.ListBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnAddImgLibFld = New System.Windows.Forms.Button
        Me.btnRmvImgLib = New System.Windows.Forms.Button
        Me.btnAddImgLib = New System.Windows.Forms.Button
        Me.rdoImgLibFiltered = New System.Windows.Forms.RadioButton
        Me.btnCalcWeightMatrix = New System.Windows.Forms.Button
        Me.rdoImgLibOriginal = New System.Windows.Forms.RadioButton
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.btnImportPreImg = New System.Windows.Forms.Button
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.lblRecoverProcess = New System.Windows.Forms.Label
        Me.probarRecoverProcess = New System.Windows.Forms.ProgressBar
        Me.pbPostImage = New System.Windows.Forms.PictureBox
        Me.btnRecoverImage = New System.Windows.Forms.Button
        Me.ofdAddImg = New System.Windows.Forms.OpenFileDialog
        Me.ofdPreImg = New System.Windows.Forms.OpenFileDialog
        Me.lblPostImageName = New System.Windows.Forms.Label
        Me.fbdAddImgFld = New System.Windows.Forms.FolderBrowserDialog
        CType(Me.pbLibraryImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbPreImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.pbPostImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbLibraryImage
        '
        Me.pbLibraryImage.Location = New System.Drawing.Point(132, 45)
        Me.pbLibraryImage.Name = "pbLibraryImage"
        Me.pbLibraryImage.Size = New System.Drawing.Size(150, 150)
        Me.pbLibraryImage.TabIndex = 0
        Me.pbLibraryImage.TabStop = False
        '
        'pbPreImage
        '
        Me.pbPreImage.Location = New System.Drawing.Point(6, 45)
        Me.pbPreImage.Name = "pbPreImage"
        Me.pbPreImage.Size = New System.Drawing.Size(150, 150)
        Me.pbPreImage.TabIndex = 3
        Me.pbPreImage.TabStop = False
        '
        'lbImageLibrary
        '
        Me.lbImageLibrary.FormattingEnabled = True
        Me.lbImageLibrary.Location = New System.Drawing.Point(6, 97)
        Me.lbImageLibrary.Name = "lbImageLibrary"
        Me.lbImageLibrary.Size = New System.Drawing.Size(120, 108)
        Me.lbImageLibrary.TabIndex = 4
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnAddImgLibFld)
        Me.GroupBox1.Controls.Add(Me.btnRmvImgLib)
        Me.GroupBox1.Controls.Add(Me.btnAddImgLib)
        Me.GroupBox1.Controls.Add(Me.rdoImgLibFiltered)
        Me.GroupBox1.Controls.Add(Me.btnCalcWeightMatrix)
        Me.GroupBox1.Controls.Add(Me.rdoImgLibOriginal)
        Me.GroupBox1.Controls.Add(Me.lbImageLibrary)
        Me.GroupBox1.Controls.Add(Me.pbLibraryImage)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(291, 227)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Image Library"
        '
        'btnAddImgLibFld
        '
        Me.btnAddImgLibFld.Location = New System.Drawing.Point(6, 71)
        Me.btnAddImgLibFld.Name = "btnAddImgLibFld"
        Me.btnAddImgLibFld.Size = New System.Drawing.Size(75, 23)
        Me.btnAddImgLibFld.TabIndex = 10
        Me.btnAddImgLibFld.Text = "Add Folder"
        Me.btnAddImgLibFld.UseVisualStyleBackColor = True
        '
        'btnRmvImgLib
        '
        Me.btnRmvImgLib.Location = New System.Drawing.Point(69, 45)
        Me.btnRmvImgLib.Name = "btnRmvImgLib"
        Me.btnRmvImgLib.Size = New System.Drawing.Size(57, 23)
        Me.btnRmvImgLib.TabIndex = 8
        Me.btnRmvImgLib.Text = "Remove"
        Me.btnRmvImgLib.UseVisualStyleBackColor = True
        '
        'btnAddImgLib
        '
        Me.btnAddImgLib.Location = New System.Drawing.Point(6, 45)
        Me.btnAddImgLib.Name = "btnAddImgLib"
        Me.btnAddImgLib.Size = New System.Drawing.Size(57, 23)
        Me.btnAddImgLib.TabIndex = 7
        Me.btnAddImgLib.Text = "Add"
        Me.btnAddImgLib.UseVisualStyleBackColor = True
        '
        'rdoImgLibFiltered
        '
        Me.rdoImgLibFiltered.AutoSize = True
        Me.rdoImgLibFiltered.Checked = True
        Me.rdoImgLibFiltered.Location = New System.Drawing.Point(210, 22)
        Me.rdoImgLibFiltered.Name = "rdoImgLibFiltered"
        Me.rdoImgLibFiltered.Size = New System.Drawing.Size(59, 17)
        Me.rdoImgLibFiltered.TabIndex = 6
        Me.rdoImgLibFiltered.TabStop = True
        Me.rdoImgLibFiltered.Text = "Filtered"
        Me.rdoImgLibFiltered.UseVisualStyleBackColor = True
        '
        'btnCalcWeightMatrix
        '
        Me.btnCalcWeightMatrix.Enabled = False
        Me.btnCalcWeightMatrix.Location = New System.Drawing.Point(6, 16)
        Me.btnCalcWeightMatrix.Name = "btnCalcWeightMatrix"
        Me.btnCalcWeightMatrix.Size = New System.Drawing.Size(120, 23)
        Me.btnCalcWeightMatrix.TabIndex = 9
        Me.btnCalcWeightMatrix.Text = "Calc Weight Matrix"
        Me.btnCalcWeightMatrix.UseVisualStyleBackColor = True
        '
        'rdoImgLibOriginal
        '
        Me.rdoImgLibOriginal.AutoSize = True
        Me.rdoImgLibOriginal.Location = New System.Drawing.Point(144, 22)
        Me.rdoImgLibOriginal.Name = "rdoImgLibOriginal"
        Me.rdoImgLibOriginal.Size = New System.Drawing.Size(60, 17)
        Me.rdoImgLibOriginal.TabIndex = 5
        Me.rdoImgLibOriginal.Text = "Original"
        Me.rdoImgLibOriginal.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnImportPreImg)
        Me.GroupBox2.Controls.Add(Me.pbPreImage)
        Me.GroupBox2.Location = New System.Drawing.Point(309, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(165, 227)
        Me.GroupBox2.TabIndex = 6
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Pre-Image"
        '
        'btnImportPreImg
        '
        Me.btnImportPreImg.Location = New System.Drawing.Point(6, 19)
        Me.btnImportPreImg.Name = "btnImportPreImg"
        Me.btnImportPreImg.Size = New System.Drawing.Size(150, 23)
        Me.btnImportPreImg.TabIndex = 4
        Me.btnImportPreImg.Text = "Import Filtered Pre-Image"
        Me.btnImportPreImg.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.lblPostImageName)
        Me.GroupBox3.Controls.Add(Me.lblRecoverProcess)
        Me.GroupBox3.Controls.Add(Me.probarRecoverProcess)
        Me.GroupBox3.Controls.Add(Me.pbPostImage)
        Me.GroupBox3.Controls.Add(Me.btnRecoverImage)
        Me.GroupBox3.Location = New System.Drawing.Point(480, 12)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(166, 227)
        Me.GroupBox3.TabIndex = 7
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Post-Image"
        '
        'lblRecoverProcess
        '
        Me.lblRecoverProcess.AutoSize = True
        Me.lblRecoverProcess.Location = New System.Drawing.Point(6, 298)
        Me.lblRecoverProcess.Name = "lblRecoverProcess"
        Me.lblRecoverProcess.Size = New System.Drawing.Size(0, 13)
        Me.lblRecoverProcess.TabIndex = 8
        '
        'probarRecoverProcess
        '
        Me.probarRecoverProcess.Location = New System.Drawing.Point(6, 201)
        Me.probarRecoverProcess.Name = "probarRecoverProcess"
        Me.probarRecoverProcess.Size = New System.Drawing.Size(151, 10)
        Me.probarRecoverProcess.TabIndex = 7
        '
        'pbPostImage
        '
        Me.pbPostImage.Location = New System.Drawing.Point(6, 45)
        Me.pbPostImage.Name = "pbPostImage"
        Me.pbPostImage.Size = New System.Drawing.Size(150, 150)
        Me.pbPostImage.TabIndex = 0
        Me.pbPostImage.TabStop = False
        '
        'btnRecoverImage
        '
        Me.btnRecoverImage.Enabled = False
        Me.btnRecoverImage.Location = New System.Drawing.Point(19, 16)
        Me.btnRecoverImage.Name = "btnRecoverImage"
        Me.btnRecoverImage.Size = New System.Drawing.Size(120, 23)
        Me.btnRecoverImage.TabIndex = 5
        Me.btnRecoverImage.Text = "Recover Image"
        Me.btnRecoverImage.UseVisualStyleBackColor = True
        '
        'ofdAddImg
        '
        Me.ofdAddImg.Filter = "Bitmap|*.bmp"
        '
        'ofdPreImg
        '
        Me.ofdPreImg.Filter = "Bitmap|*.bmp"
        '
        'lblPostImageName
        '
        Me.lblPostImageName.AutoSize = True
        Me.lblPostImageName.Location = New System.Drawing.Point(6, 211)
        Me.lblPostImageName.Name = "lblPostImageName"
        Me.lblPostImageName.Size = New System.Drawing.Size(0, 13)
        Me.lblPostImageName.TabIndex = 8
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(663, 254)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "Form1"
        Me.Text = "MachineVisionApp1"
        CType(Me.pbLibraryImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbPreImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.pbPostImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pbLibraryImage As System.Windows.Forms.PictureBox
    Friend WithEvents pbPreImage As System.Windows.Forms.PictureBox
    Friend WithEvents lbImageLibrary As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents pbPostImage As System.Windows.Forms.PictureBox
    Friend WithEvents btnAddImgLib As System.Windows.Forms.Button
    Friend WithEvents rdoImgLibFiltered As System.Windows.Forms.RadioButton
    Friend WithEvents rdoImgLibOriginal As System.Windows.Forms.RadioButton
    Friend WithEvents btnImportPreImg As System.Windows.Forms.Button
    Friend WithEvents btnRecoverImage As System.Windows.Forms.Button
    Friend WithEvents ofdAddImg As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ofdPreImg As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnRmvImgLib As System.Windows.Forms.Button
    Friend WithEvents lblRecoverProcess As System.Windows.Forms.Label
    Friend WithEvents probarRecoverProcess As System.Windows.Forms.ProgressBar
    Friend WithEvents btnCalcWeightMatrix As System.Windows.Forms.Button
    Friend WithEvents lblPostImageName As System.Windows.Forms.Label
    Friend WithEvents btnAddImgLibFld As System.Windows.Forms.Button
    Friend WithEvents fbdAddImgFld As System.Windows.Forms.FolderBrowserDialog

End Class
