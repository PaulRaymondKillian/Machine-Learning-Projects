Imports MatrixLib.Matrix
Imports MatrixLib.Operations

Public Class MachineVisionController
    Private Const _CLASSNAME As String = "MachineVisionController"
    Private ImageList As Image()
    Public ImageCount As Integer = 0

    Private InputImage As Image
    Private ResultImage As Image
    Public ImageVectorLength As Integer

    Private countInputVectors As Integer '= ImageList.Count
    'Private InputMatrix As MatrixLib.Matrix
    'Private InputMatrixT As MatrixLib.Matrix '= Transpose(InputMatrix)
    Private WeightMatrix As MatrixLib.Matrix '= Multiply(Multiply(InputMatrix, InputMatrixT), 1 / InputMatrix.cols)
    'Private inputVector As MatrixLib.Matrix
    'Private resultVector As MatrixLib.Matrix


    Public Sub New(ByVal pImageVectorLength As Integer)
        Const _METHODNAME As String = "New()"
        ImageVectorLength = pImageVectorLength
    End Sub

    Public Sub AddImage(ByVal Img As Image)
        Const _METHODNAME As String = "AddImage(Img)"
        ImageCount += 1
        ReDim Preserve ImageList(ImageCount - 1)
        ImageList(ImageCount - 1) = Img
    End Sub

    Public Function isWeightMatrixNothing() As Boolean
        If WeightMatrix Is Nothing Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub ResetWeightMatrix()
        WeightMatrix = Nothing
    End Sub

    Public Sub RemoveImage(ByVal index As Integer)
        Const _METHODNAME As String = "RemoveImage(index)"
        If ImageCount > 0 And index < ImageCount Then
            For i As Integer = index To ImageCount - 2
                ImageList(i) = ImageList(i + 1)
            Next
            ImageList(ImageCount - 1) = Nothing
            ImageCount -= 1
            ReDim Preserve ImageList(ImageCount - 1)
        Else
            Throw New Exception("Error - Class: " & _CLASSNAME & ", Method: " & _METHODNAME & " Descr: image index value is invalid")
        End If
    End Sub

    Public Function GetImage(ByVal index As Integer) As Image
        Const _METHODNAME As String = "GetImage(index)"
        If index >= 0 And index < ImageList.Count Then
            Return ImageList(index)
        Else
            Throw New Exception("Error - Class: " & _CLASSNAME & ", Method: " & _METHODNAME & " Descr: image index value is invalid")
        End If
    End Function

    Public Sub SetInputImage(ByVal Img As Image)
        InputImage = Img
        ResultImage = Img
        'inputVector = InputImage.GetMatrix
    End Sub

    Public Function GetInputImage() As Image
        Return InputImage
    End Function

    Public Function GetInputBitmap() As Drawing.Bitmap
        Return InputImage.ReturnFilteredBitmap
    End Function

    Public Function GetResultBitmap() As Drawing.Bitmap
        'Return ResultImage.Bitmap
        Return ResultImage.ReturnFilteredBitmap
    End Function

    Public Function CalcMemoryMatrix() As MatrixLib.Matrix
        Dim vectorArray(ImageList.Count - 1) As MatrixLib.Matrix
        For i As Integer = 0 To ImageCount - 1
            vectorArray(i) = ImageList(i).GetMatrix
        Next
        Return New MatrixLib.Matrix(vectorArray, ImageVectorLength)
    End Function

    Public Sub CalcWeightMatrix()
        Const _METHODNAME As String = "CalcWeightMatrix()"

        If ImageCount > 0 Then
            Dim MemoryMatrix As MatrixLib.Matrix
            MemoryMatrix = CalcMemoryMatrix()
            Dim MemoryMatrixT As MatrixLib.Matrix = Transpose(MemoryMatrix)
            If Determinant(Multiply(MemoryMatrixT, MemoryMatrix)) <> 0 Then
                WeightMatrix = Multiply(Multiply(MemoryMatrix, Inverse(Multiply(MemoryMatrixT, MemoryMatrix))), MemoryMatrixT)
            Else
                MsgBox("Inputted images are invalid for this memory method. No inverse exists for the memory matrix created, cannot continue program.")
            End If
        Else
            Throw New Exception("Error - Class: " & _CLASSNAME & ", Method: " & _METHODNAME & " Descr: cannot calculate weight matrix without any images")
        End If
    End Sub

    Public Sub StepRunHopfield()
        'inputVector = SignFunc(inputVector)
        'resultVector = Multiply(WeightMatrix, inputVector)
        'resultVector.alignment = vectorAlignment.vectorColumns
        'ResultImage = New Image(resultVector, InputImage.Name & "x", InputImage.Width, InpuImage.Height)
        'Dim InputVector As New MatrixLib.Matrix(InputImage.ImageVector, ImageVectorLength)
        InputImage.ImageVector = SignFunc(InputImage.ImageVector)
        ResultImage.ImageVector = Multiply(WeightMatrix, InputImage.ImageVector)
        ResultImage.ImageVector = SignFunc(ResultImage.ImageVector)
        ResultImage.ImageVector.alignment = vectorAlignment.vectorColumns
        ResultImage.Name = ResultImage.Name + "t"
    End Sub

    Public Sub StepRunHopfield2()
        'resultVector = SignFunc(resultVector)
        'resultVector = Multiply(WeightMatrix, resultVector)
        'resultVector.alignment = vectorAlignment.vectorColumns
        'ResultImage = New Image(resultVector, InputImage.Name & "y", InputImage.Width, InputImage.Height)
    End Sub

    Public Sub SetOutputAsInput()
        'Dim newImg As New Image(ResultImage.GetMatrix, ResultImage.Name, ResultImage.Width, ResultImage.Height)
        'SetInputImage(newImg)
        'inputVector = resultVector
        'Transpose(inputVector)
        'Transpose(resultVector)
        SetInputImage(ResultImage)
    End Sub

    Public Function GetResultImageName() As String
        Return ResultImage.Name
    End Function

    Public Function GetResultImage() As Image
        Return ResultImage
    End Function

    Public Function PercentSimilarImage(ByVal Image1 As Image, ByVal Image2 As Image)
        Dim SimPixels As Integer = 0
        If Image1.Width <> Image2.Width Or Image1.Height <> Image2.Height Then
            Throw New Exception("Error - Cannot compare images, they are of different sizes")
        End If
        For i As Integer = 0 To Image1.ImageVector.Matrix.Count - 1
            If Image1.ImageVector.Matrix(i)(0) = Image2.ImageVector.Matrix(i)(0) Then
                SimPixels += 1
            End If
        Next

        Return (SimPixels / (Image1.ImageVector.Matrix.Count)) * 100
    End Function

    Public Function FindMostSimilarImage(ByVal Image As Image) As Image
        Dim bestImage As Image
        Dim bestImagePercent, currentImagePercent As Integer

        For i As Integer = 0 To ImageCount - 1
            If i = 0 Then
                bestImage = ImageList(0)
                bestImagePercent = PercentSimilarImage(Image, ImageList(0))
            Else
                currentImagePercent = PercentSimilarImage(Image, ImageList(i))
                If bestImagePercent < currentImagePercent Then
                    bestImagePercent = currentImagePercent
                    bestImage = ImageList(i)
                End If
            End If
        Next

        Return bestImage
    End Function

End Class
