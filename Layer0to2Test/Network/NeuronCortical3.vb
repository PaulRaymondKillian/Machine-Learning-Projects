Public Class NeuronCortical3
    Public ID As String = ""
    Public ParentID As String = ""
    Public Value As Double = 0
    Public CircuitID As Integer = 0
    Public CircuitLength As Integer = 0
    Public TransNeuron As NeuronTrans
    Public SubTransNeuron(16) As Neuron
    Public PrevCorticalNeuron As NeuronCortical3
    Public NextCorticalNeuron As NeuronCortical3

    Public Shared CorticalNeuronCount As Integer = 0
    Public Function CorticalNeuronCounter() As Integer
        CorticalNeuronCounter = CorticalNeuronCount
        CorticalNeuronCount += 1
    End Function


    Public Sub New(Optional ByVal IDPrefix As String = "")
        If Not IDPrefix = "" Then
            ID = IDPrefix & "-" & CorticalNeuronCounter()
        Else
            ID = CorticalNeuronCounter()
        End If
        CircuitLength = 1
    End Sub

    Public Sub New(ByVal pPrevCort As NeuronCortical3, ByVal pCircuitID As Integer, ByVal pParentID As Integer, ByRef pCircuitLength As Integer)
        'MyBase.New(pParentID & "-" & pCircuitID & ".")
        ID = pParentID & "-" & pCircuitID & ":" & CorticalNeuronCounter()
        ParentID = pParentID
        PrevCorticalNeuron = pPrevCort
        TransNeuron = pPrevCort.TransNeuron.NextTrans
        CircuitID = pCircuitID
        CircuitLength = pCircuitLength
        CircuitLength += 1
        SetSubTransNeuronLinks()
    End Sub

    Public Sub SetSubTransNeuronLinks()
        Dim SubNeuron As Neuron = TransNeuron.SubNeuron
        Dim TempSubNeuron As Neuron

        TempSubNeuron = SubNeuron.DendritesLateral(0).Neurons(SubNeuron.ID)
        SubTransNeuron(15) = TempSubNeuron.DendritesLateral(7).Neurons(TempSubNeuron.ID)
        SubTransNeuron(0) = TempSubNeuron.DendritesLateral(0).Neurons(TempSubNeuron.ID)
        SubTransNeuron(1) = TempSubNeuron.DendritesLateral(1).Neurons(TempSubNeuron.ID)
        SubTransNeuron(2) = TempSubNeuron.DendritesLateral(2).Neurons(TempSubNeuron.ID)

        TempSubNeuron = SubNeuron.DendritesLateral(2).Neurons(SubNeuron.ID)
        SubTransNeuron(3) = TempSubNeuron.DendritesLateral(1).Neurons(TempSubNeuron.ID)
        SubTransNeuron(4) = TempSubNeuron.DendritesLateral(2).Neurons(TempSubNeuron.ID)
        SubTransNeuron(5) = TempSubNeuron.DendritesLateral(3).Neurons(TempSubNeuron.ID)
        SubTransNeuron(6) = TempSubNeuron.DendritesLateral(4).Neurons(TempSubNeuron.ID)

        TempSubNeuron = SubNeuron.DendritesLateral(4).Neurons(SubNeuron.ID)
        SubTransNeuron(7) = TempSubNeuron.DendritesLateral(3).Neurons(TempSubNeuron.ID)
        SubTransNeuron(8) = TempSubNeuron.DendritesLateral(4).Neurons(TempSubNeuron.ID)
        SubTransNeuron(9) = TempSubNeuron.DendritesLateral(5).Neurons(TempSubNeuron.ID)
        SubTransNeuron(10) = TempSubNeuron.DendritesLateral(6).Neurons(TempSubNeuron.ID)

        TempSubNeuron = SubNeuron.DendritesLateral(6).Neurons(SubNeuron.ID)
        SubTransNeuron(11) = TempSubNeuron.DendritesLateral(5).Neurons(TempSubNeuron.ID)
        SubTransNeuron(12) = TempSubNeuron.DendritesLateral(6).Neurons(TempSubNeuron.ID)
        SubTransNeuron(13) = TempSubNeuron.DendritesLateral(7).Neurons(TempSubNeuron.ID)
        SubTransNeuron(14) = TempSubNeuron.DendritesLateral(0).Neurons(TempSubNeuron.ID)

    End Sub


    Public Sub CalcNeuron()
        If TransNeuron.Value > 0 And TransNeuron.GetCircuitLength > 3 Then
            Value = 1
            BuildHyperLateralCorticalNeurons()
        End If
    End Sub

    Public Sub BuildHyperLateralCorticalNeurons()
        'Use iteration to build cortical neurons, not recursion. This method could then be used for the trans-neurons
        ''to fix the stack overflow issue for very large image maps...
        Dim ParentTrans As NeuronTrans = Me.TransNeuron
        Dim CurrentCort As NeuronCortical3 = Me
        Dim PrevCort As NeuronCortical3


        Do
            Dim NextCort As New NeuronCortical3(CurrentCort, ParentTrans.CircuitID, ParentTrans.ID, CurrentCort.CircuitLength)
            CurrentCort.NextCorticalNeuron = NextCort
            CurrentCort.CircuitLength = CurrentCort.CircuitLength + 1

            'This logic will blow up if there exits only 1 trans-neuron in this circuit, but the trans-neuron layer
            'should have eliminated that possibility...
            If Not NextCort.TransNeuron.NextTrans.Equals(ParentTrans) Then
                CurrentCort = NextCort
            Else
                CurrentCort.NextCorticalNeuron = Me
                PrevCorticalNeuron = CurrentCort
                CircuitLength = CurrentCort.CircuitLength
                Exit Do
            End If


        Loop 'While Not CurrentCort.TransNeuron.Equals(ParentTrans)

    End Sub

    Public Function OutputBitmap(ByVal BorderColor As Drawing.Color) As Drawing.Bitmap
        Dim bitmap As New Drawing.Bitmap(CircuitLength + 2, 16 + 2)
        'Dim bitmap As New Drawing.Bitmap(100, 16)

        'For y As Integer = 1 To Height
        '    For x As Integer = 1 To Width
        '        If Layer(x, y).Value > 0.5 Then
        '            bitmap.SetPixel(x - 1, y - 1, Drawing.Color.Black)
        '        Else
        '            bitmap.SetPixel(x - 1, y - 1, Drawing.Color.White)
        '        End If
        '    Next
        'Next
        Dim CurrentCort As NeuronCortical3 = Me
        For x As Integer = 0 To CircuitLength + 1
            For y As Integer = 0 To 17
                If x = 0 Or x = CircuitLength + 1 Then
                    bitmap.SetPixel(x, y, BorderColor)
                ElseIf y = 0 Or y = 17 Then
                    bitmap.SetPixel(x, y, BorderColor)
                Else
                    If CurrentCort.SubTransNeuron(y - 1).Value > 0 Then
                        bitmap.SetPixel(x, y, Drawing.Color.Black)
                    Else
                        bitmap.SetPixel(x, y, Drawing.Color.White)
                    End If
                End If
            Next
            If x <> 0 Then
                CurrentCort = CurrentCort.NextCorticalNeuron
            End If
        Next

        Return bitmap
    End Function

End Class
