Public Class NeuronCortical6

    Public ID As String = ""
    Public ParentID As String = ""
    Public Value As Double = 0
    Public CircuitID As Integer = 0
    Public Length As CircuitLength
    Public LowerCortNeuron As NeuronCortical5
    Public PrevCorticalNeuron As NeuronCortical6
    Public NextCorticalNeuron As NeuronCortical6

    Public Shared CorticalNeuronCount As Integer = 0
    Public Function CorticalNeuronCounter() As Integer
        CorticalNeuronCounter = CorticalNeuronCount
        CorticalNeuronCount += 1
    End Function


    Public Sub New(ByVal pStartLowerCort As NeuronCortical5, Optional ByVal IDPrefix As String = "")
        If Not IDPrefix = "" Then
            ID = IDPrefix & "-" & CorticalNeuronCounter()
        Else
            ID = CorticalNeuronCounter()
        End If
        LowerCortNeuron = pStartLowerCort
        Length = New CircuitLength
        Length.Value = 1
    End Sub

    Public Sub New(ByVal pStartLowerCort As NeuronCortical5, ByRef pLength As CircuitLength, Optional ByVal IDPrefix As String = "")
        If Not IDPrefix = "" Then
            ID = IDPrefix & "-" & CorticalNeuronCounter()
        Else
            ID = CorticalNeuronCounter()
        End If
        LowerCortNeuron = pStartLowerCort
        Length = pLength
        pLength.Value += 1
    End Sub

    Public Sub CalcNeuron()
        Dim CurrentNeuron, PrevNeuron As NeuronCortical6

        CurrentNeuron = Me
        CurrentNeuron.Value = PointSetDiff2(CurrentNeuron.LowerCortNeuron.Points, CurrentNeuron.LowerCortNeuron.NextCorticalNeuron.Points)
        PrevNeuron = CurrentNeuron
        Do While Not CurrentNeuron.LowerCortNeuron.NextCorticalNeuron.Equals(Me.LowerCortNeuron)
            CurrentNeuron = New NeuronCortical6(PrevNeuron.LowerCortNeuron.NextCorticalNeuron, Me.Length)
            CurrentNeuron.Value = PointSetDiff2(CurrentNeuron.LowerCortNeuron.Points, CurrentNeuron.LowerCortNeuron.NextCorticalNeuron.Points)
            PrevNeuron.NextCorticalNeuron = CurrentNeuron
            CurrentNeuron.PrevCorticalNeuron = PrevNeuron
            PrevNeuron = CurrentNeuron
        Loop
    End Sub

    Public Function PointSetDiff(ByVal Set1 As PointSet, ByVal Set2 As PointSet) As Integer

        ''OLD METHOD 1
        'Return AbsVal((Set1.First - Set2.First)) + AbsVal((Set1.Second - Set2.Second))

        ''OLD METHOD 2
        'Dim val1, val2, val3, val4 As Integer
        'val1 = AbsVal(Set1.First - Set2.First)
        'val2 = AbsVal(Set1.First - Set2.Second)
        'val3 = AbsVal(Set1.Second - Set2.First)
        'val4 = AbsVal(Set1.Second - Set2.Second)

        ''Return the smallest of the 4 values
        'If val1 <= val2 AndAlso val1 <= val3 AndAlso val1 <= val4 Then
        '    Return val1
        'ElseIf val2 <= val1 AndAlso val2 <= val3 AndAlso val2 <= val4 Then
        '    Return val2
        'ElseIf val3 <= val1 AndAlso val3 <= val2 AndAlso val3 <= val4 Then
        '    Return val3
        'ElseIf val4 <= val1 AndAlso val4 <= val2 AndAlso val4 <= val3 Then
        '    Return val4
        'Else
        '    Throw New Exception("Should never reach this point....")
        'End If

        Dim SumDiff, SubDiff As Integer
        Dim Diff1 As Integer = 0
        Dim Diff2 As Integer = 0

        Do
            SumDiff = Base16(Set1.First + Diff1)
            SubDiff = Base16(Set1.First - Diff1)
            If SumDiff - Set2.First = 0 Or SumDiff - Set2.Second = 0 Or SubDiff - Set2.First = 0 Or SubDiff - Set2.Second = 0 Then
                Exit Do
            Else
                Diff1 += 1
            End If
        Loop While Diff1 < 8

        Do
            SumDiff = Base16(Set1.Second + Diff2)
            SubDiff = Base16(Set1.Second - Diff2)
            If SumDiff - Set2.First = 0 Or SumDiff - Set2.Second = 0 Or SubDiff - Set2.First = 0 Or SubDiff - Set2.Second = 0 Then
                Exit Do
            Else
                Diff2 += 1
            End If
        Loop While Diff2 < 8


        If Diff1 > Diff2 Then
            Return Diff1
        Else
            Return Diff2
        End If

    End Function

    Public Function PointSetDiff2(ByVal Set1 As PointSet, ByVal Set2 As PointSet) As Integer
        Dim Index As Integer = 0
        Dim CurrentVal As Integer = Set1.First
        Dim Diff As Integer
        Dim DiffSet(3) As Integer

        'Find first diff
        Diff = 0
        Do
            If Base16(CurrentVal + Index) = Set2.First Then
                CurrentVal = Set2.First
                Exit Do
            ElseIf Base16(CurrentVal + Index) = Set2.Second Then
                CurrentVal = Set2.Second
                Exit Do
            Else
                Diff += 1
            End If
            Index = Base16(Index + 1)
        Loop
        DiffSet(0) = Diff

        'Find second diff
        Diff = 0
        Index = 0
        Do
            If Base16(CurrentVal + Index) = Set1.First Then
                CurrentVal = Set1.First
                Exit Do
            ElseIf Base16(CurrentVal + Index) = Set1.Second Then
                CurrentVal = Set1.Second
                Exit Do
            Else
                Diff += 1
            End If
            Index = Base16(Index + 1)
        Loop
        DiffSet(1) = Diff

        'Find third diff
        Diff = 0
        Index = 0
        Do
            If Base16(CurrentVal + Index) = Set2.First Then
                CurrentVal = Set2.First
                Exit Do
            ElseIf Base16(CurrentVal + Index) = Set2.Second Then
                CurrentVal = Set2.Second
                Exit Do
            Else
                Diff += 1
            End If
            Index = Base16(Index + 1)
        Loop
        DiffSet(2) = Diff

        'Find fourth diff
        Diff = 0
        Index = 0
        Do
            If Base16(CurrentVal + Index) = Set1.First Then
                CurrentVal = Set1.First
                Exit Do
            ElseIf Base16(CurrentVal + Index) = Set1.Second Then
                CurrentVal = Set1.Second
                Exit Do
            Else
                Diff += 1
            End If
            Index = Base16(Index + 1)
        Loop
        DiffSet(3) = Diff

        Return Mode(DiffSet)

    End Function


    'This method is needed to easily stay within the base 16 set, which includes negative values also being modulo properly
    Public Function Base16(ByVal Value As Integer) As Integer
        If Value >= 0 Then
            Return Value Mod 16
        Else
            Return (16 + (Value Mod 16)) Mod 16
        End If
    End Function

    Public Function AbsVal(ByVal val As Integer) As Integer
        If val >= 0 Then
            Return val
        Else
            Return -val
        End If
    End Function

    Public Function OutputBitmap(ByVal border As Drawing.Color) As Drawing.Bitmap
        Dim bitmap As New Drawing.Bitmap(Length.Value + 2, 8 + 2)

        Dim CurrentCort As NeuronCortical6 = Me
        For x As Integer = 0 To bitmap.Width - 1
            'Just set whole col white to start - stupid extra work, but whatever. its just for debugging basically anyway...
            For y As Integer = 0 To bitmap.Height - 1
                If y = 0 Or y = bitmap.Height - 1 Or x = 0 Or x = bitmap.Width - 1 Then
                    bitmap.SetPixel(x, y, border)
                Else
                    bitmap.SetPixel(x, y, Drawing.Color.White)
                End If
            Next
            'Value contains the pointset differental of the lower cort 5 layer neurons
            If x > 0 And x < bitmap.Width - 1 Then
                bitmap.SetPixel(x, CurrentCort.Value + 1, Drawing.Color.Black)
                CurrentCort = CurrentCort.NextCorticalNeuron
            End If
        Next

        Return bitmap
    End Function

    Public Function OutputString() As String
        Dim s As String = ""
        Dim CurrentNeuron As NeuronCortical6 = Me
        For i As Integer = 0 To Length.Value - 1
            s += CurrentNeuron.Value.ToString
            If i <> Length.Value - 1 Then
                s += ","
            End If
            CurrentNeuron = CurrentNeuron.NextCorticalNeuron
        Next
        Return s
    End Function

    Public Function Mode(ByVal Array() As Integer) As Integer
        Dim count(15) As Integer
        'Dim ModeVal As Integer

        'If Integer Array does not default to zero, uncomment out this code
        'For i As Integer = 0 To 15
        '    count(i) = 0
        'Next

        For i As Integer = 0 To Array.Length - 1
            If Array(i) >= 0 And Array(i) <= 15 Then
                count(Array(i)) += 1
            Else
                Throw New Exception("Invalid Value for Neuron Point: " & Array(i))
            End If
        Next

        For i As Integer = 0 To 15
            If i = 0 Then
                Mode = i
            Else
                If count(i) > count(Mode) Then
                    Mode = i
                End If
            End If
        Next

        Return Mode

    End Function

End Class

