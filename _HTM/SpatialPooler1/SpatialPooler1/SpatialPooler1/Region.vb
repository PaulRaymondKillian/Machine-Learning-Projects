Public Class Region
    Private Columns(,) As Column
    Public MinOverlap
    Public DesiredOverlap
    Private Shared RegionCount As Integer = 0

    Public ReadOnly Property GetColumns() As Column(,)
        Get
            Return Columns
        End Get
    End Property

    Public Sub New(ByRef pLowerColumns(,) As Column, pWidth As Integer, pHeight As Integer, pCountCells As Integer, pCountPotentialSynapses As Integer, pMinOverlap As Double, pDesiredOverlap As Double)
        ReDim Columns(pWidth-1,pHeight-1)
        For i As Integer = 0 to Columns.GetLength(0) - 1
            For j As Integer = 0 to Columns.GetLength(1) - 1
                Columns(i,j) = New Column("R" & RegionCount & "-" & i & "," & j,pCountCells,pCountPotentialSynapses,pLowerColumns)
            Next
        Next
        MinOverlap = pMinOverlap
        DesiredOverlap = pDesiredOverlap
        RegionCount += 1    'Incease shared Region count
    End Sub

    Public Sub New(pLowerColumns(,) As Column, pCountCells As Integer, pCountPotentialSynapses As Integer, Optional pMinOverlap As Double = 10, Optional pDesiredOverlap As Double = 10)
        Me.New(pLowerColumns, pLowerColumns.GetLength(0), pLowerColumns.GetLength(1), pCountCells, pCountPotentialSynapses, pMinOverlap, pDesiredOverlap)
    End Sub

    Public Sub CalcOverlap(pMinOverlap As Double)
        For Each Column As Column In Columns
            Column.CalcOverlap(pMinOverlap)
        Next
    End Sub

    Public Function xPotentialSynapseCoverageMetric() As String
        Dim htCoverage As New Hashtable
        Dim s As String = ""

        For i As Integer = 0 to Columns.GetLength(0) - 1
            For j As Integer = 0 to Columns.GetLength(1) - 1
                For k As Integer = 0 to Columns(i,j).PotentialSynapses.Length - 1
                    If htCoverage.Contains(Columns(i,j).PotentialSynapses(k).Column.ID) then
                        htCoverage.Item(Columns(i,j).PotentialSynapses(k).Column.ID) = htCoverage.Item(Columns(i,j).PotentialSynapses(k).Column.ID)+1
                    Else
                            htCoverage.Add(Columns(i,j).PotentialSynapses(k).Column.ID, 1)
                    End If                                  
                Next
            Next
        Next

        'For Each item In htCoverage
        '    s = s & vbCrLf & item.key & ": " & item.value
        'Next

        Dim split(), split2() As String
        Dim maxX As Integer = 0
        Dim maxY As Integer = 0 
        For Each item In htCoverage
            split = item.key.split("-"c)
            split2 = split(1).Split(","c)
            If Convert.ToInt32(split2(0)) > maxX then
                maxX = Convert.ToInt32(split2(0))
            End If
            If Convert.ToInt32(split2(1)) > maxY then
                maxY = Convert.ToInt32(split2(1))
            End If                    
        Next

        Dim Array(maxX, maxY,1) As String
        For i As Integer = 0 to maxX
            For j As Integer = 0 to maxY
                Array(i,j,0) = 0
                Array(i,j,1) = "N/A"
            Next
        Next
        For Each item In htCoverage
            split = item.key.split("-"c)
            split2 = split(1).Split(","c)
            Array(split2(0),split2(1),0) = item.value
            Array(split2(0),split2(1),1) = item.key
        Next

        For i As Integer = 0 to maxX
            For j As Integer = 0 to maxY
                s = s & vbCrLf & Array(i,j,1) & " - " & i & "," & j & ": " & Array(i,j,0)
            Next
        Next

        s = s & vbCrLf & htCoverage.Count & " lower cells covered."

        Return s

    End Function

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
