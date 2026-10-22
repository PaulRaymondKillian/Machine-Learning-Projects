Public Class HLayer

    Public HTree As HTree
    Public CurrentThoughts() As HThought
    Public Shared NameCounter As Integer = 0

    Public Function GetNewName() As Integer
        NameCounter += 1
        Return NameCounter
    End Function

    Public Sub New()
        HTree = New HTree
    End Sub

    Public Function Think(ByVal Sequence As String) As HThought
        Dim Thought(0) As HThought
        Thought(0) = New HThought
        Dim CurrName As String

        CurrName = HTree.RecallSeq(Sequence)
        If CurrName <> "" Then
            Thought(0).AddRecollection(CurrName)
        End If

        CurrentThoughts = Thought
        Return Thought(0)

    End Function

    Public Function Think(ByVal Sequence() As String) As HThought()
        Dim Thoughts(Sequence.Length - 1) As HThought
        'Dim CurrName As String

        'For i As Integer = 0 To Sequence.Length - 1
        '    Thought(i) = New Thought
        '    CurrName = HTree.RecallSeq(Sequence(i))
        '    If CurrName <> "" Then
        '        Thought(i).AddRecollection(CurrName)
        '    End If
        'Next

        For i As Integer = 0 To Sequence.Length - 1
            Thoughts(i) = Think(Sequence(i))
        Next

        CurrentThoughts = Thoughts
        Return Thoughts

    End Function

    Public Function Think(ByVal Thoughts() As HThought) As HThought
        Dim Thought(0) As HThought
        Dim s As String = ""

        For i As Integer = 0 To Thoughts.Length - 1
            If Thoughts(i).Attention <> -1 Then
                s += Thoughts(i).Attention.ToString
            End If
            If i <> Thoughts.Length - 1 Then
                s += ","
            End If
        Next

        Thought(0) = Think(s)
        CurrentThoughts = Thought
        Return Thought(0)

    End Function

    Public Function DoIKnowThis(ByVal Sequence As String) As Boolean
        Return Think(Sequence).Known
    End Function

    Public Function Memorize(ByVal Sequence As String) As Integer
        If DoIKnowThis(Sequence) = True Then
            Return CurrentThoughts(0).Attention
        Else
            Dim Name As Integer = GetNewName()

            Dim Split() As String = Sequence.Split(","c)
            'Dim Nodes(Split.Length - 1) As String
            Dim CurrentSeq As String
            Dim Index As Integer

            For i As Integer = 0 To Split.Length - 1
                CurrentSeq = ""
                Index = i
                For j As Integer = 0 To Split.Length - 1
                    CurrentSeq += Split(Index)
                    If (i <> 0 And Index <> i - 1) Or (i = 0 And Index <> Split.Length - 1) Then
                        CurrentSeq += ","
                    End If
                    If Index = Split.Length - 1 Then
                        Index = 0
                    Else
                        Index += 1
                    End If
                Next
                HTree.LearnSeq(CurrentSeq, Name)
            Next

            'Learn the permutations of the sequence again, but this time backwards order
            Dim temp As String
            'Reverse the order of the array of chars
            For i As Integer = 0 To (Convert.ToInt32(Split.Length / 2)) - 1
                temp = Split(i)
                Split(i) = Split(Split.Length - 1 - i)
                Split(Split.Length - 1 - i) = temp
            Next

            For i As Integer = 0 To Split.Length - 1
                CurrentSeq = ""
                Index = i
                For j As Integer = 0 To Split.Length - 1
                    CurrentSeq += Split(Index)
                    If (i <> 0 And Index <> i - 1) Or (i = 0 And Index <> Split.Length - 1) Then
                        CurrentSeq += ","
                    End If
                    If Index = Split.Length - 1 Then
                        Index = 0
                    Else
                        Index += 1
                    End If
                Next
                HTree.LearnSeq(CurrentSeq, Name)
            Next

            Return Name

        End If

    End Function

    Public Function CurrentThoughtsString() As String
        Dim s As String = ""

        For i As Integer = 0 To CurrentThoughts.Length - 1
            s += CurrentThoughts(i).Attention.ToString
            If i <> CurrentThoughts.Length - 1 Then
                s += ","
            End If
        Next

        Return s
    End Function
End Class
