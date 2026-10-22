Public Class NeuronTrans
    Inherits Neuron

    Public SubNeuron As Neuron
    Public NextTrans As NeuronTrans
    Public PrevTrans As NeuronTrans
    Public ParentID As Integer = 0

    Public Shared CircuitCount As Integer = 0
    Public Function CircuitCounter() As Integer
        CircuitCount += 1
        CircuitCounter = CircuitCount
    End Function

    Public Structure SubNeuronSet
        Public Subneuron As Neuron
        Public DendriteIndex As DendriteIndex
    End Structure

    Public Structure DendriteIndex
        Public DendriteIndex As Integer
        Private OddFlag As Boolean
        Private Differential As Integer

        Public Sub New(ByVal Index As Integer)
            DendriteIndex = Index
            OddFlag = True
            Differential = 1
        End Sub

        Public Sub NextIndex()
            If OddFlag Then
                DendriteIndex = (DendriteIndex + Differential) Mod 8
                OddFlag = False
            ElseIf Not OddFlag Then
                DendriteIndex = (DendriteIndex - Differential) Mod 8
                OddFlag = True
            End If
            Differential += 1

            If DendriteIndex < 0 Then
                If DendriteIndex = -1 Then
                    DendriteIndex = 7
                ElseIf DendriteIndex = -2 Then
                    DendriteIndex = 6
                ElseIf DendriteIndex = -3 Then
                    DendriteIndex = 5
                ElseIf DendriteIndex = -4 Then
                    DendriteIndex = 4
                ElseIf DendriteIndex = -5 Then
                    DendriteIndex = 3
                ElseIf DendriteIndex = -6 Then
                    DendriteIndex = 2
                ElseIf DendriteIndex = -7 Then
                    DendriteIndex = 1
                ElseIf DendriteIndex = -8 Then
                    DendriteIndex = 0
                End If
            End If
        End Sub

    End Structure


    Public Sub New(Optional ByVal IDPrefix As String = "")
        MyBase.New(IDPrefix)
    End Sub

    Public Sub New(ByVal pPrevTrans As Neuron, ByVal pCircuitID As Integer, ByVal pParentID As Integer)
        MyBase.New(pParentID & "-" & pCircuitID & ".")
        ParentID = pParentID
        PrevTrans = pPrevTrans
        CircuitID = pCircuitID
    End Sub

    Public Overrides Sub CalcNeuron()
        If SubNeuron.Value > 0 And SubNeuron.CircuitID = 0 Then
            Value = 1
            CircuitID = CircuitCounter()
            SubNeuron.CircuitID = CircuitID

            Dim CurrentNeuronTrans As NeuronTrans
            Dim PrevNeuronTrans As NeuronTrans
            Dim PrevDendriteIndex As New DendriteIndex(3)
            Dim NextSubNeuronSet As SubNeuronSet
            CurrentNeuronTrans = Me
            PrevNeuronTrans = Me 'At first, set the prev to the parent subneuron
            Do
                NextSubNeuronSet = FindNextSubNeuronSet(CurrentNeuronTrans.SubNeuron, PrevDendriteIndex.DendriteIndex, PrevNeuronTrans.SubNeuron, Me.SubNeuron)
                If Not NextSubNeuronSet.Subneuron Is Nothing Then 'If NextSubNeuronSet.Subneuron != Nothing
                    If Not NextSubNeuronSet.Subneuron.Equals(Me.SubNeuron) Then 'If NextSubNeuronSet.Subneuron != Me.Subneuron
                        CurrentNeuronTrans = New NeuronTrans(PrevNeuronTrans, CircuitID, ID)
                        PrevDendriteIndex = NextSubNeuronSet.DendriteIndex
                        CurrentNeuronTrans.SubNeuron = NextSubNeuronSet.Subneuron
                        CurrentNeuronTrans.SubNeuron.CircuitID = CurrentNeuronTrans.CircuitID
                        CurrentNeuronTrans.PrevTrans = PrevNeuronTrans
                        PrevNeuronTrans.NextTrans = CurrentNeuronTrans
                        PrevNeuronTrans = CurrentNeuronTrans
                    Else 'If the NextSubNeuronSet.Subneuron = Me.Subneuron
                        'We have now completed the circuit, so lets wrap up the loose ends together
                        PrevNeuronTrans.NextTrans = Me
                        Me.PrevTrans = PrevNeuronTrans
                        Exit Do
                    End If
                Else 'If the NextSubNeuronSet.Subneuron = Nothing
                    'This situation "Should" only happen if there is a single pixel by itself where there is no other neuron to find.
                    'So, lets just ignore this one and reset it to be a non-circuit 
                    Value = 0
                    SubNeuron.CircuitID = 0
                    CircuitCount -= 1
                    Exit Do
                End If
            Loop

        End If

    End Sub

    Public Function FindNextSubNeuronSet(ByVal CurrentSubNeuron As Neuron, ByVal pDendriteIndex As Integer, ByVal PreviousSubNeuron As Neuron, ByVal ParentSubNeuron As Neuron) As SubNeuronSet
        Dim FoundSubNeuron As Neuron
        Dim SubNeuronSet As New SubNeuronSet
        Dim DendriteIndex As DendriteIndex

        'Check for virgin sub-neuron (Activated, No Circuit ID)
        DendriteIndex = New DendriteIndex(pDendriteIndex)
        For i As Integer = 0 To 7
            FoundSubNeuron = CurrentSubNeuron.DendritesLateral(DendriteIndex.DendriteIndex).Neurons(CurrentSubNeuron.ID)
            If FoundSubNeuron.Value > 0 Then
                If FoundSubNeuron.CircuitID = 0 Then
                    SubNeuronSet.Subneuron = FoundSubNeuron
                    SubNeuronSet.DendriteIndex = DendriteIndex
                    Return SubNeuronSet
                End If
            Else
                DendriteIndex.NextIndex()
            End If
        Next

        'Next neuron not found yet, check for neurons whose may be in the same circuit already, but havent been recently fired
        DendriteIndex = New DendriteIndex(pDendriteIndex)
        For i As Integer = 0 To 7
            FoundSubNeuron = CurrentSubNeuron.DendritesLateral(DendriteIndex.DendriteIndex).Neurons(CurrentSubNeuron.ID)
            If FoundSubNeuron.Value > 0 Then
                If Not FoundSubNeuron.Equals(PreviousSubNeuron) Then
                    'Only count subneurons that are in the same circuit - don't over-write already created circuits
                    If FoundSubNeuron.CircuitID = PreviousSubNeuron.CircuitID Then
                        SubNeuronSet.Subneuron = FoundSubNeuron
                        SubNeuronSet.DendriteIndex = DendriteIndex
                        Return SubNeuronSet
                    End If
                End If
            Else
                DendriteIndex.NextIndex()
            End If
        Next

        'Next neuron not found yet, check for any neuron fired that isnt the parent
        DendriteIndex = New DendriteIndex(pDendriteIndex)
        For i As Integer = 0 To 7
            FoundSubNeuron = CurrentSubNeuron.DendritesLateral(DendriteIndex.DendriteIndex).Neurons(CurrentSubNeuron.ID)
            If FoundSubNeuron.Value > 0 Then
                If Not FoundSubNeuron.Equals(ParentSubNeuron) Then
                    'Only count subneurons that are in the same circuit - don't over-write already created circuits
                    If FoundSubNeuron.CircuitID = PreviousSubNeuron.CircuitID Then
                        SubNeuronSet.Subneuron = FoundSubNeuron
                        SubNeuronSet.DendriteIndex = DendriteIndex
                        Return SubNeuronSet
                    End If
                End If
            Else
                DendriteIndex.NextIndex()
            End If
        Next

        'Next neuron not found yet, check for any neuron fired
        DendriteIndex = New DendriteIndex(pDendriteIndex)
        For i As Integer = 0 To 7
            FoundSubNeuron = CurrentSubNeuron.DendritesLateral(DendriteIndex.DendriteIndex).Neurons(CurrentSubNeuron.ID)
            If FoundSubNeuron.Value > 0 Then
                'Only count subneurons that are in the same circuit - don't over-write already created circuits
                If FoundSubNeuron.CircuitID = PreviousSubNeuron.CircuitID Then
                    SubNeuronSet.Subneuron = FoundSubNeuron
                    SubNeuronSet.DendriteIndex = DendriteIndex
                    Return SubNeuronSet
                End If
            Else
                DendriteIndex.NextIndex()
            End If
        Next

        'No neuron found, return nothing...
        Return Nothing

    End Function

    Public Function GetCircuitLength() As Integer
        Dim count As Integer = 0
        Dim CurrentNeuron As NeuronTrans = Me
        Do
            count += 1
            CurrentNeuron = CurrentNeuron.NextTrans
            If CurrentNeuron Is Nothing Then
                Exit Do
            End If
        Loop Until CurrentNeuron.Equals(Me)

        Return count
    End Function

End Class
