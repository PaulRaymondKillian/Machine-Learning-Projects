Public Class SenseReceptor_OLD
    Private Const _CLASS As String = "SenseReceptor"
    Public Receptors() As Byte  'Receptors will be used as an array of "bits" where only valid values are 0 or 1
                                'Using a byte because it is the smallest data type to represent a single bit
                                'Byte = 1 byte, Boolean = 4 bytes

    Public ReadOnly Property ReceptorSize As Integer
        Get
            Return Receptors.Length
        End Get
    End Property

    Public Sub new(pReceptorSize As Integer)
        Const _METHOD As String = "new(ReceptorSize)"
        If pReceptorSize < 1 then
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Invalid Receptor Size, cannot be < 1")
        End If

        'Initialize Array
        ReDim Receptors(pReceptorSize-1)

        'Set initial values
        For i As Integer = 0 to Receptors.Length-1
            Receptors(i) = 0
        Next

    End Sub

    Public Sub SetReceptors(pInputs() As Byte)
        Const _METHOD As String = "SetReceptors(pInputs() as Byte)"
        If pInputs.Length <> Receptors.Length then
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Input size != Receptor size")
        End If

        For i As Integer = 0 to Receptors.Length-1
            If pInputs(i) <= 0 then
                Receptors(i) = 0 
            Else
                Receptors(i) = 1
            End If
        Next
    End Sub

    Public Sub SetReceptors(pInputs As Bitmap)
        Const _METHOD As String = "SetReceptors(pInputs at Bitmap)"
        If pInputs.Width * pInputs.Height <> Receptors.Length then
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Input size != Receptor size")
        End If

        'Inputted image can be either a long image of height 1, a tall image of width 1, or a 2d image to be transformed to the 1-d bit array
        'This For loop should handle all bitmap variations
        'XXX THIS BELOW LOOP PROBABLY IS BUGGED AND NEEDS TO BE WATCHED CLOSELY THE FIRST TIME USED... XXXXX

            For x As Integer = 0 to pInputs.Width
                For y As Integer = 0 to pInputs.Height
                    If pInputs.GetPixel(x,y) = Color.Black
                        Receptors((x*pInputs.Width) + y) = 1
                    Else
                        Receptors((x*pInputs.Width) + y) = 0
                    End If
                Next
            Next

    End Sub


End Class
