Public Class SenseReceptor
    Private Const _CLASS As String = "SenseReceptor"
    Private Shared SenseCount As Integer = 0
    Private Columns(,) As Column     'Sense Receptor will use 1-cell columns and treat them as input cells for easy interface with Region

    Public ReadOnly Property GetColumns() As Column(,)
        Get
            Return Columns
        End Get
    End Property
     Public Sub New(pReceptorWidth As Integer, pReceptorHeight As Integer)
        Const _METHOD As String = "new(pRecemptorWidth,pReceptorHeight)"
        If pReceptorWidth < 1 or pReceptorHeight <1 then    'Make sure dimensions positive
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Invalid Receptor Size, cannot be < 1")
        End If

        'Initialize Array
        ReDim Columns(pReceptorWidth-1, pReceptorHeight-1)

        'Set initial values
        For i As Integer = 0 to Columns.GetLength(0) -1
            For j As Integer = 0 to Columns.GetLength(1) -1
                Columns(i,j) = New Column("S" & SenseCount & "-" & i & "," & j,1)   'Creates column of 1 cell
            Next
        Next
        SenseCount += 1 'Increase shared SenseReceptor count
    End Sub

    'Set all cell receptors equal to an inputted array of same size
    Public Sub SetReceptorsInput(pInputs(,) as Byte)
        Const _METHOD As String = "SetReceptorsInput(pInputs() as Byte)"
        If pInputs.GetLength(0) <> Columns.GetLength(0) or pInputs.GetLength(1) <> Columns.GetLength(1) then
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Invalid input size; Columns: " & Columns.GetLength(0) & "x" & Columns.GetLength(1) & ", Inputted: " & pInputs.GetLength(0) & "x" &pInputs.GetLength(1))
        End If

        Try
            For i As Integer = 0 to Columns.GetLength(0) - 1
                For j As Integer = 0 to Columns.GetLength(1) - 1
                    If pInputs(i,j) >= 0.5 then
                        Columns(i,j).Cells(0).State = Cell.CellState.ActiveProximal 'Essentially sets the entire column active
                    Else 'If pInputs(i,j) < 0.5
                        Columns(i,j).Cells(0).State = Cell.CellState.Inactive       'Since only 1 cell column, essentially sets the entire colum inactive
                    End If
                Next
            Next
        Catch ex As Exception
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Error setting column cells equal to inputted array values.")
        End Try
    End Sub
              
    'Set an individual cell receptor (column)
    Public Sub SetReceptorInput(x As Integer, y As Integer, value As Byte)
        Const _METHOD As String = "SetReceptorInput(x,y)"
        If x > Columns.GetLength(0) or x < 0 or y > Columns.GetLength(1) or y < 0 then
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Invalid input size; Columns: " & Columns.GetLength(0) & "x" & Columns.GetLength(1) & ", x: " & x & ", y:" & y)
        End If

        Try
            If value >= 0.5 then
                Columns(x,y).Cells(0).State = Cell.CellState.ActiveProximal
            Else 'If value < 0.5
                Columns(x,y).Cells(0).State = Cell.CellState.Inactive
            End If
        Catch ex As Exception
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Error setting cell receptor: " & x & "," & y & " to value: " & value)
        End Try
    End Sub

    Public Sub SetReceptorsInput( pInputs As Bitmap)
        Const _METHOD As String = "SetReceptorsInput(pInputs at Bitmap)"
        If pInputs.Width <> Columns.GetLength(0) or pInputs.Height <> Columns.GetLength(1) then
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Input size != Receptor size; Input bitmap size: " & pInputs.Width & "x" & pInputs.Height & ", Columns: " & Columns.GetLength(0) & "x" & Columns.GetLength(1))
        End If

        Try
           For i As Integer = 0 to Columns.GetLength(0) - 1
                For j As Integer = 0 to Columns.GetLength(1) - 1
                    If pInputs.GetPixel(i,j).GetBrightness >= 0.5 then
                        Columns(i,j).Cells(0).State = Cell.CellState.ActiveProximal
                    Else 'If pInputs.GetPixel(i,j).GetBrightness < 0.5
                        Columns(i,j).Cells(0).State = Cell.CellState.Inactive
                    End If
                Next
            Next
        Catch ex As Exception
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Error setting column cells equal to inputted bitmap brightness values.")
        End Try

    End Sub

    Public Function OutputBitmap() As Bitmap
        Dim bitmap As New Bitmap(Columns.GetLength(0), Columns.GetLength(1))
        For i As Integer = 0 to Columns.GetLength(0)-1
            For j As Integer = 0 to Columns.GetLength(1)-1
                If Columns(i,j).ActiveColumn = True then
                    bitmap.SetPixel(i,j,Color.Black)
                Else
                    bitmap.SetPixel(i,j,Color.White)
                End If
            Next
        Next
        Return bitmap
    End Function

End Class
