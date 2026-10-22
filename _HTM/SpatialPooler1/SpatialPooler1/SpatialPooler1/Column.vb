Public Class Column
    Private Const _CLASS As String = "Column"
    Public ID As String
    Public Cells() As Cell                  'Cells contained in the column
    Public PotentialSynapses() As Synapse   'Proximal Dendrite - all cells in a column will share the same proximal dendrite to the lower region (feedforward)
    Public Boost As Double = 1              'Default Boost is 1, no boosting occurs
    Private CountOverlap As Double = 0      'Use Double because "boost" may require decimals

    Private Shared Random As New Random(System.DateTime.Now.Ticks Mod System.Int32.MaxValue)

    Private xInitialConnectedPermVariance As Double = 5.0'%    'Controls are close or far from connectedPerm allowed (Percentage of ConnectedPerm)

    Public ReadOnly Property ActiveColumn As Boolean
        Get
            Dim isActive As Boolean = False
            For i As Integer = 0 to Cells.Length-1
                If Cells(i).State <> Cell.CellState.Inactive
                    isActive = True
                    Exit For                'We know the column is active, no need to go through the rest of the cells
                End If
            Next
            Return isActive
        End Get
    End Property

    Public ReadOnly Property GetOverlap() As Double
        Get
            Return CountOverlap
        End Get
    End Property

    'Normal region will initialize a column with all  parameters
    'SenseReceptor will not pass pCountPotentialSynapses and pLowerColumns because it has no lower region
    Public Sub New(pID As String, pCountCells As Integer, Optional pCountPotentialSynapses As Integer = 0, Optional ByRef pLowerColumns(,) As Column = Nothing, Optional pConnectedPerm As Double=1)
        Const _METHOD As String = "New(pID, pCountCells, Optional pCountPotentialSynapses, Option pLowerColumns)"
        'A normal region will initialize a column with PotentialSynapses and LowerColumns. A SenseReceptor, however, has no lower region
        ''and therefore no lower columns nor potential synapses. Therefore, pCountPotentialSynapses and pLowerColumns either both need
        ''to be a valid inputted parameter passed, or neither passed. Any other combination passed is invalid.
        If  (pCountPotentialSynapses < 1 and Not pLowerColumns is Nothing) or _
            (pCountPotentialSynapses > 0 and pLowerColumns is Nothing) then
            Throw New Exception("Exception - Class: " & _CLASS & ", Method: " & _METHOD & ", Desc: Invalid parameter combination - pCountPotentialSynapses, pLowerColumns")
        End If

        ID = pID
        ReDim Cells(pCountCells-1)
        For i As Integer = 0 to Cells.Length - 1
            Cells(i) = New Cell(ID & "-C" & i)
        Next
        If pCountPotentialSynapses > 0 then
            ReDim PotentialSynapses(pCountPotentialSynapses-1)
        End If
        
        If Not pLowerColumns is Nothing andalso pCountPotentialSynapses > 0 then
            SetPotentialSynapses(pLowerColumns,pConnectedPerm)
        End If
    End Sub


    Private sub SetPotentialSynapses(ByRef pLowerColumns(,) As Column, pConnectedPerm As Double)
        Dim width As Integer = pLowerColumns.GetLength(0)
        Dim height As Integer = pLowercolumns.GetLength(1)
        Dim randW, randH As Integer
        Dim htPoints As New Hashtable

        For i As Integer = 0 to PotentialSynapses.Length-1
            'Make sure we do not have duplicate potential synapses to the same lower region column
            ''NOTE: If the number of potential synapses is larger than the number of columns in the lower region, this will be an infinite loop... but how likely is that?
            Do
                randW = Random.Next mod width
                randH = Random.Next mod height
            Loop Until Not htPoints.Contains(randW & "," & randH)

            PotentialSynapses(i) = New Synapse(pLowerColumns(randW, randH), CalcInitialPermanence(pConnectedPerm,xInitialConnectedPermVariance), pConnectedPerm)
            htPoints.Add(randW & "," & randH,0)
        Next
    End Sub

        Public Function CalcInitialPermanence(pConnectedPerm As Double, pPercentVariance As Double)
        'Ideally, Numenta's paper says there should be a natural center of synapses to the lower region where the initial permanence is 
        ''strongest near the center, but due to ambigous understanding of their meaning, that aspect is currently not included and will
        ''be tackled in the future. At the moment, this function returns a random value near the ConnectedPerm value, as the paper states.
        Dim VarianceDifferential As Double = pConnectedPerm * (pPercentVariance/100) * ((Random.Next mod 1000) / 1000)
        If (Random.Next mod 100) < 50 then
            VarianceDifferential = -VarianceDifferential
        End If
        Return pConnectedPerm + VarianceDifferential
    End Function

    Public Sub CalcOverlap(pMinOverlap As Double)
        CountOverlap = 0
        For Each Synapse As Synapse In PotentialSynapses
            If Synapse.IsConnected AndAlso Synapse.IsActive Then
                CountOverlap += 1
            End If
        Next
        If CountOverlap < pMinOverlap Then
            CountOverlap = 0
        Else
            CountOverlap *= Boost
        End If
    End Sub
End Class
