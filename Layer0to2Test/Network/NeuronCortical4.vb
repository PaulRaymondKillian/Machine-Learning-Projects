Public Class CircuitLength
    Public Value As Integer
    Public Sub New()
        Value = 0
    End Sub
End Class

Public Class NeuronCortical4

    'Public Structure PointSet
    '    Public First As Integer
    '    Public Second As Integer
    'End Structure

    Public ID As String = ""
    Public ParentID As String = ""
    Public Value As Double = 0
    Public CircuitID As Integer = 0
    'Public CircuitLength As Integer = 0
    Public Length As CircuitLength
    Public LowerCortNeuron As NeuronCortical3
    'Public SubTransNeuron(16) As Neuron
    Public PrevCorticalNeuron As NeuronCortical4
    Public NextCorticalNeuron As NeuronCortical4
    Public Points As PointSet

    Public ImgFirstFilter As Drawing.Bitmap
    Public ImgSecondFilter As Drawing.Bitmap
    Public ImgThirdFilter As Drawing.Bitmap
    Public ImgFourthFilter As Drawing.Bitmap

    Public Shared CorticalNeuronCount As Integer = 0
    Public Function CorticalNeuronCounter() As Integer
        CorticalNeuronCounter = CorticalNeuronCount
        CorticalNeuronCount += 1
    End Function


    Public Sub New(ByVal pStartLowerCort As NeuronCortical3, Optional ByVal IDPrefix As String = "")
        If Not IDPrefix = "" Then
            ID = IDPrefix & "-" & CorticalNeuronCounter()
        Else
            ID = CorticalNeuronCounter()
        End If
        LowerCortNeuron = pStartLowerCort
        'CircuitLength = 1
        Length = New CircuitLength
        Length.Value = 1
    End Sub

    Public Sub New(ByVal pStartLowerCort As NeuronCortical3, ByRef pLength As CircuitLength, Optional ByVal IDPrefix As String = "")
        If Not IDPrefix = "" Then
            ID = IDPrefix & "-" & CorticalNeuronCounter()
        Else
            ID = CorticalNeuronCounter()
        End If
        LowerCortNeuron = pStartLowerCort
        'CircuitLength = pCircuitLength
        'pCircuitLength += 1
        Length = pLength
        pLength.Value += 1
        SetPointValues2()
    End Sub

    'Public Sub New(ByVal pPrevCort As NeuronCortical3, ByVal pCircuitID As Integer, ByVal pParentID As Integer, ByRef pCircuitLength As Integer)
    '    'MyBase.New(pParentID & "-" & pCircuitID & ".")
    '    ID = pParentID & "-" & pCircuitID & ":" & CorticalNeuronCounter()
    '    ParentID = pParentID
    '    PrevCorticalNeuron = pPrevCort
    '    TransNeuron = pPrevCort.TransNeuron.NextTrans
    '    CircuitID = pCircuitID
    '    CircuitLength = pCircuitLength
    '    CircuitLength += 1
    '    SetSubTransNeuronLinks()
    'End Sub

    'Public Sub SetSubTransNeuronLinks()
    '    Dim SubNeuron As Neuron = TransNeuron.SubNeuron
    '    Dim TempSubNeuron As Neuron

    '    TempSubNeuron = SubNeuron.DendritesLateral(0).Neurons(SubNeuron.ID)
    '    SubTransNeuron(15) = TempSubNeuron.DendritesLateral(7).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(0) = TempSubNeuron.DendritesLateral(0).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(1) = TempSubNeuron.DendritesLateral(1).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(2) = TempSubNeuron.DendritesLateral(2).Neurons(TempSubNeuron.ID)

    '    TempSubNeuron = SubNeuron.DendritesLateral(2).Neurons(SubNeuron.ID)
    '    SubTransNeuron(3) = TempSubNeuron.DendritesLateral(1).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(4) = TempSubNeuron.DendritesLateral(2).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(5) = TempSubNeuron.DendritesLateral(3).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(6) = TempSubNeuron.DendritesLateral(4).Neurons(TempSubNeuron.ID)

    '    TempSubNeuron = SubNeuron.DendritesLateral(4).Neurons(SubNeuron.ID)
    '    SubTransNeuron(7) = TempSubNeuron.DendritesLateral(3).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(8) = TempSubNeuron.DendritesLateral(4).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(9) = TempSubNeuron.DendritesLateral(5).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(10) = TempSubNeuron.DendritesLateral(6).Neurons(TempSubNeuron.ID)

    '    TempSubNeuron = SubNeuron.DendritesLateral(6).Neurons(SubNeuron.ID)
    '    SubTransNeuron(11) = TempSubNeuron.DendritesLateral(5).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(12) = TempSubNeuron.DendritesLateral(6).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(13) = TempSubNeuron.DendritesLateral(7).Neurons(TempSubNeuron.ID)
    '    SubTransNeuron(14) = TempSubNeuron.DendritesLateral(0).Neurons(TempSubNeuron.ID)

    'End Sub


    Public Function CalcNeuron() As Boolean
        Dim FoundCleanLineSegment As Boolean = False

        'It is possible we will start our circuit in the middle of a straight line, this causes problems.
        'Go forward through the circuit until we reach a vertex (Non-clean segment) and then start processing from there.
        Do
            FoundCleanLineSegment = IsCleanLineSegment(LowerCortNeuron)
            If FoundCleanLineSegment = True Then
                LowerCortNeuron = LowerCortNeuron.NextCorticalNeuron
            End If
        Loop Until FoundCleanLineSegment = False

        'Find the "next" first "Clean" line segment for the parent neuron to use as its LowerCortical
        Do
            FoundCleanLineSegment = IsCleanLineSegment(LowerCortNeuron)
            If FoundCleanLineSegment = False Then
                LowerCortNeuron = LowerCortNeuron.NextCorticalNeuron
            End If
        Loop Until FoundCleanLineSegment = True

        Dim CurrentLowerCort As NeuronCortical3 = LowerCortNeuron.NextCorticalNeuron
        Dim PrevCort4 As NeuronCortical4 = Me
        Do
            'Find next "Clean" line segment
            Do
                FoundCleanLineSegment = IsCleanLineSegment(CurrentLowerCort)
                If FoundCleanLineSegment = False Then
                    CurrentLowerCort = CurrentLowerCort.NextCorticalNeuron
                End If
            Loop Until FoundCleanLineSegment = True Or CurrentLowerCort.Equals(Me.LowerCortNeuron)

            If Not CurrentLowerCort.Equals(Me.LowerCortNeuron) And FoundCleanLineSegment Then
                Dim NewCort4 As New NeuronCortical4(CurrentLowerCort, Me.Length)
                PrevCort4.NextCorticalNeuron = NewCort4
                NewCort4.PrevCorticalNeuron = PrevCort4
                PrevCort4 = NewCort4
                CurrentLowerCort = CurrentLowerCort.NextCorticalNeuron
            ElseIf CurrentLowerCort.Equals(Me.LowerCortNeuron) Then
                Me.PrevCorticalNeuron = PrevCort4
                PrevCort4.NextCorticalNeuron = Me
            Else
                MsgBox("?")
            End If

        Loop Until CurrentLowerCort.Equals(Me.LowerCortNeuron)
        SetPointValues()

        ImgFirstFilter = OutputTempBitmap()

    End Function

    Public Function IsCleanLineSegment(ByVal NeuronC3 As NeuronCortical3) As Boolean
        Dim ActiveNeurons As Integer = 0
        Dim Differential As Integer = -1
        Dim MetFirstNeuron As Boolean = False
        Dim MetSecondNeuron As Boolean = False
        For i As Integer = 0 To 15
            If NeuronC3.SubTransNeuron(i).Value > 0 Then
                ActiveNeurons += 1
                If Not MetFirstNeuron And Not MetSecondNeuron Then
                    MetFirstNeuron = True
                ElseIf MetFirstNeuron And Not MetSecondNeuron Then
                    MetSecondNeuron = True
                End If
            End If
            If MetFirstNeuron And Not MetSecondNeuron Then
                Differential += 1
            End If
        Next

        If ActiveNeurons = 2 And Differential >= 6 And Differential <= 8 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub SetPointValues()
        Dim FoundFirstPoint As Boolean = False
        For i As Integer = 0 To 15
            If LowerCortNeuron.SubTransNeuron(i).Value > 0 And Not FoundFirstPoint Then
                Points.First = i
                FoundFirstPoint = True
            ElseIf LowerCortNeuron.SubTransNeuron(i).Value > 0 And FoundFirstPoint Then
                Points.Second = i
                Exit For
            End If
        Next
    End Sub

    Public Sub SetPointValues2()
        Dim FoundFirstPoint As Boolean = False
        If PrevCorticalNeuron Is Nothing Then
            'This is the first neuron in the cycle
            For i As Integer = 0 To 15
                If LowerCortNeuron.SubTransNeuron(i).Value > 0 And Not FoundFirstPoint Then
                    Points.First = i
                    FoundFirstPoint = True
                ElseIf LowerCortNeuron.SubTransNeuron(i).Value > 0 And FoundFirstPoint Then
                    Points.Second = i
                    Exit For
                End If
            Next
        Else
            'This is not the first neuron in circuit, use the previous neuron's points to help find your points
            'to ensure the first/second points are in a congruent order with other nearby similar neurons

            'Start searching where the prev cort has its first point
            Dim index As Integer = PrevCorticalNeuron.Points.First
            Dim diff As Integer = 0

            Do
                index += diff
                If LowerCortNeuron.SubTransNeuron(Base16(index)).Value > 0 Then
                    Points.First = Base16(index)
                    FoundFirstPoint = True
                Else
                    diff = -diff
                    If diff < 0 Then
                        diff -= 1
                    Else
                        diff += 1
                    End If
                End If
            Loop Until FoundFirstPoint

            'Found first point, now find the second. Whatever is left has to be the second point
            For i As Integer = 0 To 15
                If LowerCortNeuron.SubTransNeuron(i).Value > 0 And i <> Points.First Then
                    Points.Second = i
                    Exit For
                End If
            Next

        End If

    End Sub

    Public Sub FilterNeuronValues()
        'Dim Values(4, 1) As Integer
        Dim PointSets(4) As PointSet
        Dim index As Integer = 0
        Dim CurrentCort As NeuronCortical4 = PrevCorticalNeuron.PrevCorticalNeuron

        Do
            PointSets(index) = CurrentCort.Points
            'Values(index, 0) = CurrentCort.Points.First
            'Values(index, 1) = CurrentCort.Points.Second
            CurrentCort = CurrentCort.NextCorticalNeuron
            index += 1
        Loop While Not CurrentCort.Equals(NextCorticalNeuron.NextCorticalNeuron.NextCorticalNeuron)

        'Now you have array containing nearby point value sets, find a normalized value set
        If (PointSetDiff(Points, PointSets(3)) = 0 And PointSetDiff(Points, PointSets(4)) = 0) Or _
            (PointSetDiff(Points, PointSets(1)) = 0 And PointSetDiff(Points, PointSets(0)) = 0) Then
            'Point Set Values are good, exit. No normalizing required
            Exit Sub
        End If

        'CHECK PREV CORTS NEEDS TO BE FIRST - BEFORE CHECKING NEXT CORTS
        'Check if Normalizing to Prev 2 Corts
        If PointSetDiff(Points, PointSets(3)) <= 2 And PointSetDiff(Points, PointSets(4)) <= 2 Then
            Dim First, Second As Integer
            First = Math.Round((Points.First + PointSets(0).First + PointSets(1).First) / 3)
            Second = Math.Round((Points.Second + PointSets(0).Second + PointSets(1).Second) / 3)
            Points.First = First
            Points.Second = Second
            Exit Sub
        End If

        'Check if Normalizing to Next 2 Corts
        If PointSetDiff(Points, PointSets(3)) <= 2 And PointSetDiff(Points, PointSets(4)) <= 2 Then
            Dim First, Second As Integer
            First = Math.Round((Points.First + PointSets(3).First + PointSets(4).First) / 3)
            Second = Math.Round((Points.Second + PointSets(3).Second + PointSets(4).Second) / 3)
            Points.First = First
            Points.Second = Second
            Exit Sub
        End If

        'If the neuron doesnt match the next 2 or prev 2 corts, then just set it to the closest adjacent neuron...
        If PointSetDiff(Points, PointSets(3)) <= 1 Then
            Points.First = PointSets(3).First
            Points.Second = PointSets(3).Second
            Exit Sub
        End If

        If PointSetDiff(Points, PointSets(1)) <= 1 Then
            Points.First = PointSets(1).First
            Points.Second = PointSets(1).Second
            Exit Sub
        End If

    End Sub

    Public Sub FilterNeuronValues2()
        Dim Index As Integer = 0
        Dim CircuitLength As Integer = Length.Value
        Dim CurrentNeuron As NeuronCortical4
        Dim StartNeuron As NeuronCortical4
        Dim EndNeuron As NeuronCortical4
        Dim NeuronCount As Integer = 0
        Dim NeuronSumPoints As PointSet

        CurrentNeuron = Me

        For NeuronTotal As Integer = 0 To CircuitLength - 1
            StartNeuron = CurrentNeuron
            NeuronSumPoints = New PointSet
            NeuronCount = 0
            Do While PointSetDiff(CurrentNeuron.Points, CurrentNeuron.NextCorticalNeuron.Points) <= 1
                NeuronSumPoints.First += CurrentNeuron.Points.First
                NeuronSumPoints.Second += CurrentNeuron.Points.Second
                NeuronCount += 1
                CurrentNeuron = CurrentNeuron.NextCorticalNeuron
            Loop
            EndNeuron = CurrentNeuron
            If NeuronCount <> 0 Then
                NeuronSumPoints.First = Math.Round(NeuronSumPoints.First / NeuronCount)
                NeuronSumPoints.Second = Math.Round(NeuronSumPoints.Second / NeuronCount)


                CurrentNeuron = StartNeuron
                Do
                    CurrentNeuron.Points = NeuronSumPoints
                    CurrentNeuron = CurrentNeuron.NextCorticalNeuron
                Loop While Not CurrentNeuron.Equals(EndNeuron.NextCorticalNeuron)
                CurrentNeuron = EndNeuron.NextCorticalNeuron
                NeuronTotal += NeuronCount
            Else
                'Remove this neuron from the circuit - it is an anomaly that will cause problems
                CurrentNeuron.PrevCorticalNeuron.NextCorticalNeuron = CurrentNeuron.NextCorticalNeuron
                CurrentNeuron.NextCorticalNeuron.PrevCorticalNeuron = CurrentNeuron.PrevCorticalNeuron
                Length.Value -= 1
                CurrentNeuron = CurrentNeuron.NextCorticalNeuron
            End If

        Next
        ImgThirdFilter = OutputTempBitmap()
    End Sub

    Public Sub FilterNeuronValues3()
        Dim Index As Integer = 0
        Dim CircuitLength As Integer = Length.Value
        Dim CurrentNeuron As NeuronCortical4
        Dim StartNeuron As NeuronCortical4
        Dim EndNeuron As NeuronCortical4
        Dim NeuronCount As Integer = 0
        Dim NeuronSumPoints As PointSet
        Dim NeuronPointFirst() As Integer
        Dim NeuronPointSecond() As Integer

        CurrentNeuron = Me

        For NeuronTotal As Integer = 0 To CircuitLength - 1
            StartNeuron = CurrentNeuron
            NeuronSumPoints = New PointSet
            NeuronCount = 0
            Do While PointSetDiff(CurrentNeuron.Points, CurrentNeuron.NextCorticalNeuron.Points) <= 1
                NeuronCount += 1
                CurrentNeuron = CurrentNeuron.NextCorticalNeuron
            Loop
            EndNeuron = CurrentNeuron


            If NeuronCount <> 0 Then
                'Go through the neurons in this set again and add the point values to the respective arrays
                ReDim NeuronPointFirst(NeuronCount)
                ReDim NeuronPointSecond(NeuronCount)
                Index = 0
                CurrentNeuron = StartNeuron
                Do
                    NeuronPointFirst(Index) = CurrentNeuron.Points.First
                    NeuronPointSecond(Index) = CurrentNeuron.Points.Second
                    CurrentNeuron = CurrentNeuron.NextCorticalNeuron
                    Index += 1
                Loop While Not CurrentNeuron.Equals(EndNeuron.NextCorticalNeuron)

                Dim ModeFirstPoint As Integer = Mode(NeuronPointFirst)
                Dim ModeSecondPoint As Integer = Mode(NeuronPointSecond)

                NeuronSumPoints.First = ModeFirstPoint
                NeuronSumPoints.Second = ModeSecondPoint


                CurrentNeuron = StartNeuron
                Do
                    CurrentNeuron.Points = NeuronSumPoints
                    CurrentNeuron = CurrentNeuron.NextCorticalNeuron
                Loop While Not CurrentNeuron.Equals(EndNeuron.NextCorticalNeuron)
                CurrentNeuron = EndNeuron.NextCorticalNeuron
                NeuronTotal += NeuronCount
            Else
                'Remove this neuron from the circuit - it is an anomaly that will cause problems
                CurrentNeuron.PrevCorticalNeuron.NextCorticalNeuron = CurrentNeuron.NextCorticalNeuron
                CurrentNeuron.NextCorticalNeuron.PrevCorticalNeuron = CurrentNeuron.PrevCorticalNeuron
                Length.Value -= 1
                CurrentNeuron = CurrentNeuron.NextCorticalNeuron
            End If

        Next
        ImgThirdFilter = OutputTempBitmap()
    End Sub

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


    Public Sub FilterCurvedLineSegments()
        Dim Index As Integer = 0
        Dim CircuitLength As Integer = Length.Value
        Dim CurrentNeuron As NeuronCortical4
        Dim StartNeuron As NeuronCortical4
        Dim EndNeuron As NeuronCortical4
        Dim NeuronCount As Integer = 0
        Dim NeuronSumPoints As PointSet

        'Prep the circuit so that we don't delete the parent neuron, which will cause infinite loops later if that occurs
        While PointSetDiff(Me.PrevCorticalNeuron.Points, Me.NextCorticalNeuron.Points) = 2
            'Cannot re-assign Me, but can re-assign the parts of Me to the next neuron
            Me.Points = Me.NextCorticalNeuron.Points
            Me.LowerCortNeuron = Me.NextCorticalNeuron.LowerCortNeuron
            Me.Length.Value -= 1
            Me.NextCorticalNeuron = Me.NextCorticalNeuron.NextCorticalNeuron
            'By doing all this, we "essentially" skip our current parent
        End While

        CurrentNeuron = Me

        For NeuronTotal As Integer = 0 To CircuitLength - 1
            If PointSetDiff(CurrentNeuron.PrevCorticalNeuron.Points, CurrentNeuron.NextCorticalNeuron.Points) = 2 Then
                CurrentNeuron.PrevCorticalNeuron.NextCorticalNeuron = CurrentNeuron.NextCorticalNeuron
                CurrentNeuron.NextCorticalNeuron.PrevCorticalNeuron = CurrentNeuron.PrevCorticalNeuron
                CurrentNeuron.Length.Value -= 1
            End If
            CurrentNeuron = CurrentNeuron.NextCorticalNeuron
        Next
        ImgSecondFilter = OutputTempBitmap()
    End Sub

    Public Sub FilterStraightLineCohesion()
        Dim Index As Integer = 0
        Dim CircuitLength As Integer = Length.Value
        Dim CurrentNeuron As NeuronCortical4
        Dim StartNeuron As NeuronCortical4
        Dim EndNeuron As NeuronCortical4
        Dim NeuronCount As Integer = 0
        Dim NeuronPointsDiff As Integer


        CurrentNeuron = Me

        For NeuronTotal As Integer = 0 To CircuitLength - 1
            NeuronPointsDiff = AbsVal(CurrentNeuron.Points.Second - CurrentNeuron.Points.First) - 1
            If NeuronPointsDiff = 8 Then
                'Pull the point value downwards
                If CurrentNeuron.Points.First = 15 Then
                    CurrentNeuron.Points.First = 0
                Else
                    CurrentNeuron.Points.First += 1
                End If
            ElseIf NeuronPointsDiff = 6 Then
                'Push the point value upwards
                If CurrentNeuron.Points.First = 0 Then
                    CurrentNeuron.Points.First = 15
                Else
                    CurrentNeuron.Points.First -= 1
                End If
            ElseIf NeuronPointsDiff >= 9 Or NeuronPointsDiff <= 5 Then
                Throw New Exception("Invalid Point Set Different - Error: Something set us up the bomb!")
            End If
            CurrentNeuron = CurrentNeuron.NextCorticalNeuron
        Next

        'Check once again to make sure all have a difference of 7 - can remove this code later if feel safe it works well
        For NeuronTotal As Integer = 0 To CircuitLength - 1
            NeuronPointsDiff = AbsVal(CurrentNeuron.Points.First - CurrentNeuron.Points.Second) - 1
            If NeuronPointsDiff <> 7 Then
                Throw New Exception("Non-Good Things Happened! Oh No!")
            End If
            CurrentNeuron = CurrentNeuron.NextCorticalNeuron
        Next

        ImgFourthFilter = OutputTempBitmap()
    End Sub

    Public Function PointSetDiff(ByVal Set1 As PointSet, ByVal Set2 As PointSet) As Integer

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

    Public Function OutputBitmapSubTrans() As Drawing.Bitmap
        Dim bitmap As New Drawing.Bitmap(Length.Value, 16)
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
        Dim CurrentCort As NeuronCortical4 = Me
        For x As Integer = 0 To Length.Value - 1
            For y As Integer = 0 To 15
                If CurrentCort.LowerCortNeuron.SubTransNeuron(y).Value > 0 Then
                    bitmap.SetPixel(x, y, Drawing.Color.Black)
                Else
                    bitmap.SetPixel(x, y, Drawing.Color.White)
                End If
            Next
            CurrentCort = CurrentCort.NextCorticalNeuron
        Next

        Return bitmap
    End Function

    Public Function OutputBitmap(ByVal border As Drawing.Color) As Drawing.Bitmap
        Dim bitmapLen As Integer = ImgFirstFilter.Width + 2
        Dim bitmapHi As Integer = (16 * 4) + 3 + 2
        Dim bitmap As New Drawing.Bitmap(bitmapLen, bitmapHi)

        Dim Circuit1Color As Drawing.Color = Drawing.Color.Red
        Dim Circuit2Color As Drawing.Color = Drawing.Color.Blue
        Dim Circuit3Color As Drawing.Color = Drawing.Color.Green
        Dim Circuit4Color As Drawing.Color = Drawing.Color.Orange
        Dim Circuit5Color As Drawing.Color = Drawing.Color.LightBlue
        Dim Circuit6Color As Drawing.Color = Drawing.Color.Yellow
        Dim Circuit7Color As Drawing.Color = Drawing.Color.Purple


        'Create border
        For x As Integer = 0 To bitmapLen - 1
            For y As Integer = 0 To bitmapHi - 1
                If x = 0 Or x = bitmapLen - 1 Or y = 0 Or y = bitmapHi - 1 Then
                    bitmap.SetPixel(x, y, border)
                End If
            Next
        Next

        'Copy the bitmaps onto 1 large master bitmap image
        'This is a stupid way to do this, but it doesnt matter, we just want it done, arent looking for performance right now
        For y As Integer = 1 To 16
            For x As Integer = 1 To ImgFirstFilter.Width - 1
                bitmap.SetPixel(x, y, ImgFirstFilter.GetPixel(x - 1, y - 1))
            Next
        Next
        'Create first line delimiter
        For x As Integer = 0 To bitmap.Width - 1
            bitmap.SetPixel(x, 17, border)
        Next
        For y As Integer = 1 To 16
            For x As Integer = 1 To ImgSecondFilter.Width - 1
                bitmap.SetPixel(x, y + 17, ImgSecondFilter.GetPixel(x - 1, y - 1))
            Next
        Next
        'Create second line delimiter
        For x As Integer = 0 To bitmap.Width - 1
            bitmap.SetPixel(x, 34, border)
        Next
        For y As Integer = 1 To 16
            For x As Integer = 1 To ImgThirdFilter.Width - 1
                bitmap.SetPixel(x, y + 34, ImgThirdFilter.GetPixel(x - 1, y - 1))
            Next
        Next
        ''Create third line delimiter
        'For x As Integer = 0 To bitmap.Width - 1
        '    bitmap.SetPixel(x, 51, border)
        'Next
        'For y As Integer = 1 To 16
        '    For x As Integer = 1 To ImgFourthFilter.Width - 1
        '        bitmap.SetPixel(x, y + 51, ImgFourthFilter.GetPixel(x - 1, y - 1))
        '    Next
        'Next

        Return bitmap
    End Function

    Private Function OutputTempBitmap() As Drawing.Bitmap
        Dim bitmap As New Drawing.Bitmap(Length.Value, 16)


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
        Dim CurrentCort As NeuronCortical4 = Me
        For x As Integer = 0 To Length.Value - 1
            'Just set whole col white to start - stupid extra work, but whatever. its just for debugging basically anyway...
            For y As Integer = 0 To 15
                bitmap.SetPixel(x, y, Drawing.Color.White)
            Next
            bitmap.SetPixel(x, CurrentCort.Points.First, Drawing.Color.Black)
            bitmap.SetPixel(x, CurrentCort.Points.Second, Drawing.Color.Black)
            CurrentCort = CurrentCort.NextCorticalNeuron
        Next

        Return bitmap
    End Function


    'Public Sub BuildHyperLateralCorticalNeurons()
    '    'Use iteration to build cortical neurons, not recursion. This method could then be used for the trans-neurons
    '    ''to fix the stack overflow issue for very large image maps...
    '    Dim ParentTrans As NeuronTrans = Me.TransNeuron
    '    Dim CurrentCort As NeuronCortical3 = Me
    '    Dim PrevCort As NeuronCortical3


    '    Do
    '        Dim NextCort As New NeuronCortical3(CurrentCort, ParentTrans.CircuitID, ParentTrans.ID, CurrentCort.CircuitLength)
    '        CurrentCort.NextCorticalNeuron = NextCort
    '        CurrentCort.CircuitLength = CurrentCort.CircuitLength + 1

    '        'This logic will blow up if there exits only 1 trans-neuron in this circuit, but the trans-neuron layer
    '        'should have eliminated that possibility...
    '        If Not NextCort.TransNeuron.NextTrans.Equals(ParentTrans) Then
    '            CurrentCort = NextCort
    '        Else
    '            CurrentCort.NextCorticalNeuron = Me
    '            PrevCorticalNeuron = CurrentCort
    '            CircuitLength = CurrentCort.CircuitLength
    '            Exit Do
    '        End If


    '    Loop 'While Not CurrentCort.TransNeuron.Equals(ParentTrans)

    'End Sub

    'Public Function OutputBitmap() As Drawing.Bitmap
    '    Dim bitmap As New Drawing.Bitmap(CircuitLength, 16)
    '    'Dim bitmap As New Drawing.Bitmap(100, 16)

    '    'For y As Integer = 1 To Height
    '    '    For x As Integer = 1 To Width
    '    '        If Layer(x, y).Value > 0.5 Then
    '    '            bitmap.SetPixel(x - 1, y - 1, Drawing.Color.Black)
    '    '        Else
    '    '            bitmap.SetPixel(x - 1, y - 1, Drawing.Color.White)
    '    '        End If
    '    '    Next
    '    'Next
    '    Dim CurrentCort As NeuronCortical3 = Me
    '    For x As Integer = 0 To CircuitLength - 1
    '        For y As Integer = 0 To 15
    '            If CurrentCort.SubTransNeuron(y).Value > 0 Then
    '                bitmap.SetPixel(x, y, Drawing.Color.Black)
    '            Else
    '                bitmap.SetPixel(x, y, Drawing.Color.White)
    '            End If
    '        Next
    '        CurrentCort = CurrentCort.NextCorticalNeuron
    '    Next

    '    Return bitmap
    'End Function

End Class
