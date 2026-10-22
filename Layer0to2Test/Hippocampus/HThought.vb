Public Class HThought
    Public Recollections As New Hashtable
    Public UniqueRecollections As Integer
    Public TotalRecollections As Integer
    Public AttentionName As String = "N/A"

    Public ReadOnly Property Attention() As Integer
        Get
            Dim Rec As Recollection() = OrderRecollections()
            If Rec.Length = 0 Then
                Return -1
            Else
                Return Rec(0).Name
            End If
        End Get
    End Property

    Public ReadOnly Property Known() As Boolean
        Get
            Dim Rec As Recollection() = OrderRecollections()
            If Rec.Length = 0 Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    'Public ReadOnly Property ThoughtString() As String
    '    Get
    '        Dim Rec As Recollection() = OrderRecollections()
    '        If Rec.Length = 0 Then
    '            Return "I dont know what I am looking at."
    '        Else
    '            Return "I think I am looking at a " & Rec(0).Name
    '        End If
    '    End Get
    'End Property

    Public Structure Recollection
        Dim Name As String
        Dim Certainty As Integer
    End Structure

    Public Sub New()
        UniqueRecollections = 0
        TotalRecollections = 0
    End Sub

    Public Sub AddRecollection(ByVal Name As String)
        If Not Recollections.Item(Name) Is Nothing Then
            Recollections.Item(Name).Certainty += 1
            TotalRecollections += 1
        Else
            Dim newRecollection As Recollection
            newRecollection.Name = Name
            newRecollection.Certainty = 1
            Recollections.Add(Name, newRecollection)
            UniqueRecollections += 1
            TotalRecollections += 1
        End If
    End Sub

    Public Function OrderRecollections() As Recollection()
        Dim RecollectionArray(UniqueRecollections - 1) As Recollection
        Dim Recall
        Dim Index As Integer = UniqueRecollections - 1
        Dim CertaintyValue As Integer = 0

        Do
            For Each Recall In Recollections
                If Recall.Value.Certainty = CertaintyValue Then
                    RecollectionArray(Index) = Recall.Value
                    Index -= 1
                End If
            Next
            CertaintyValue += 1
        Loop Until Index = -1

        Return RecollectionArray

    End Function




End Class
