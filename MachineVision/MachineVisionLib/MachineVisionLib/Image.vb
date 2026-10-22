Imports MatrixLib.Matrix
Imports MatrixLib.Operations

Public Enum SeparationType As Integer
    NA = -1
    BlackAndWhite = 0
    'Red = 1
    'Blue = 2
    'Green = 3
End Enum

Public Class Image
    Public Name As String = ""
    Public Width As Integer '= Image.Width
    Public Height As Integer '= Image.Height
    Public Separation As SeparationType = SeparationType.BlackAndWhite
    Public SeparationDegree As Double = 0.5

    Private Const _CLASSNAME As String = "MachineVisionController"
    Public Bitmap As Drawing.Bitmap
    Private FilteredImage As Drawing.Bitmap
    Public ImageVector As MatrixLib.Matrix

    Public Sub New(ByVal pImg As Drawing.Bitmap, ByVal pName As String, ByVal pWidth As Integer, ByVal pHeight As Integer)
        Name = pName
        Width = pWidth
        Height = pHeight
        Bitmap = NormalizeBitmap(pImg)
        Separation = SeparationType.BlackAndWhite
        CalcImageVector()
    End Sub

    Public Sub New(ByVal pImg As Drawing.Bitmap, ByVal pName As String, ByVal pWidth As Integer, ByVal pHeight As Integer, ByVal pSeparation As SeparationType, ByVal pSeparationDegree As Integer)
        Name = pName
        Width = pWidth
        Height = pHeight
        Bitmap = NormalizeBitmap(pImg)
        Separation = pSeparation
        SeparationDegree = pSeparationDegree
        CalcImageVector()
    End Sub

    Public Sub New(ByVal pMatrix As MatrixLib.Matrix, ByVal pName As String, ByVal pWidth As Integer, ByVal pHeight As Integer)
        Name = pName
        Width = pWidth
        Height = pHeight
        ImageVector = pMatrix
        Bitmap = ReturnBitmap(pMatrix)
    End Sub

    Public Function NormalizeBitmap(ByVal pImg As Drawing.Bitmap) As Drawing.Bitmap
        'This method will output an image that has been normalized to fit the desired width and height
        Dim newBitmap As New Drawing.Bitmap(Width, Height)
        For y As Integer = 0 To Height - 1
            For x As Integer = 0 To Width - 1
                If y < pImg.Height - 1 Then
                    If x < pImg.Width - 1 Then
                        newBitmap.SetPixel(x, y, pImg.GetPixel(x, y))
                    Else
                        newBitmap.SetPixel(x, y, Drawing.Color.Blue)
                    End If
                Else
                    newBitmap.SetPixel(x, y, Drawing.Color.Red)
                End If
            Next
        Next
        Return newBitmap
    End Function

    Public Sub CalcImageVector()
        Dim pixel As Integer
        ImageVector = New MatrixLib.Matrix(Width * Height, 1)   'Each image is a column vector
        ImageVector.alignment = vectorAlignment.vectorColumns
        If Separation = SeparationType.BlackAndWhite Then
            For y As Integer = 0 To Height - 1
                For x As Integer = 0 To Width - 1
                    pixel = Bitmap.GetPixel(x, y).GetBrightness
                    If pixel >= SeparationDegree Then
                        ImageVector.Matrix((Width * y) + x)(0) = -1  'Remember it is a vector
                    Else
                        ImageVector.Matrix((Width * y) + x)(0) = 1 'Remember it is a vector
                    End If
                Next
            Next
        End If

    End Sub

    Public Function GetPixel(ByVal row As Integer, ByVal col As Integer) As Drawing.Color
        If ImageVector.Matrix((Width * row) + col)(0) = 1 Then
            Return Drawing.Color.Black
        Else
            Return Drawing.Color.White
        End If
    End Function

    Private Function ReturnBitmap(ByVal pMatrix As MatrixLib.Matrix) As Drawing.Bitmap
        Dim newBitmap As New Drawing.Bitmap(Width, Height)
        For y As Integer = 0 To Height - 1
            For x As Integer = 0 To Width - 1
                If pMatrix.Matrix((y * Width) + x)(0) >= 0 Then
                    newBitmap.SetPixel(x, y, Drawing.Color.Black)
                Else
                    newBitmap.SetPixel(x, y, Drawing.Color.White)
                End If
            Next
        Next
        Return newBitmap
    End Function

    Public Function ReturnFilteredBitmap(Optional ByVal RetainFilteredImage As Boolean = False) As Drawing.Bitmap

        If Not FilteredImage Is Nothing Then
            Return FilteredImage
        Else
            Dim newBitmap As New Drawing.Bitmap(Width, Height)
            For y As Integer = 0 To Height - 1
                For x As Integer = 0 To Width - 1
                    newBitmap.SetPixel(x, y, GetPixel(y, x))
                Next
            Next
            If RetainFilteredImage Then
                FilteredImage = newBitmap
            End If
            Return newBitmap
        End If
    End Function

    Public Function GetMatrix() As MatrixLib.Matrix
        Return ImageVector
    End Function

End Class
