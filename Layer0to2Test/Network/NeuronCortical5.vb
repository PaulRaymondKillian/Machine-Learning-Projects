Public Structure PointSet
    Public First As Integer
    Public Second As Integer
End Structure

Public Class NeuronCortical5

    Public ID As String = ""
    Public ParentID As String = ""
    Public Value As Double = 0
    Public CircuitID As Integer = 0
    Public Length As CircuitLength
    Public LowerCortNeuron As NeuronCortical4
    Public PrevCorticalNeuron As NeuronCortical5
    Public NextCorticalNeuron As NeuronCortical5
    Public ReadOnly Property Points() As PointSet
        Get
            Return LowerCortNeuron.Points
        End Get
    End Property

    Public Shared CorticalNeuronCount As Integer = 0
    Public Function CorticalNeuronCounter() As Integer
        CorticalNeuronCounter = CorticalNeuronCount
        CorticalNeuronCount += 1
    End Function


    Public Sub New(ByVal pStartLowerCort As NeuronCortical4, Optional ByVal IDPrefix As String = "")
        If Not IDPrefix = "" Then
            ID = IDPrefix & "-" & CorticalNeuronCounter()
        Else
            ID = CorticalNeuronCounter()
        End If
        LowerCortNeuron = pStartLowerCort
        Length = New CircuitLength
        Length.Value = 1
    End Sub

    Public Sub New(ByVal pStartLowerCort As NeuronCortical4, ByRef pLength As CircuitLength, Optional ByVal IDPrefix As String = "")
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
        Dim CurrentCort5 As NeuronCortical5
        Dim CurrentCort4 As NeuronCortical4
        Dim NextCort4 As NeuronCortical4

        CurrentCort5 = Me
        CurrentCort4 = Me.LowerCortNeuron
        NextCort4 = CurrentCort4.NextCorticalNeuron

        Do
            While PointSetDiff(CurrentCort4.Points, NextCort4.Points) = 0
                NextCort4 = NextCort4.NextCorticalNeuron
            End While
            CurrentCort4 = NextCort4
            If NextCort4.Equals(Me.LowerCortNeuron) Then
                Exit Do
            End If
            NextCort4 = CurrentCort4.NextCorticalNeuron
            Dim newCort5 As New NeuronCortical5(NextCort4, Length)
            CurrentCort5.NextCorticalNeuron = newCort5
            newCort5.PrevCorticalNeuron = CurrentCort5
            CurrentCort5 = newCort5
        Loop While Not CurrentCort5.LowerCortNeuron.NextCorticalNeuron.Equals(Me.LowerCortNeuron)

        CurrentCort5.NextCorticalNeuron = Me
        Me.PrevCorticalNeuron = CurrentCort5

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
        Dim bitmap As New Drawing.Bitmap(Length.Value + 2, 16 + 2)

        Dim CurrentCort As NeuronCortical5 = Me
        For x As Integer = 0 To bitmap.Width - 1
            'Just set whole col white to start - stupid extra work, but whatever. its just for debugging basically anyway...
            For y As Integer = 0 To bitmap.Height - 1
                If y = 0 Or y = bitmap.Height - 1 Or x = 0 Or x = bitmap.Width - 1 Then
                    bitmap.SetPixel(x, y, border)
                Else
                    bitmap.SetPixel(x, y, Drawing.Color.White)
                End If
            Next
            If x > 0 And x < bitmap.Width - 1 Then
                bitmap.SetPixel(x, CurrentCort.LowerCortNeuron.Points.First + 1, Drawing.Color.Black)
                bitmap.SetPixel(x, CurrentCort.LowerCortNeuron.Points.Second + 1, Drawing.Color.Black)
                CurrentCort = CurrentCort.NextCorticalNeuron
            End If
        Next

        Return bitmap
    End Function

End Class
