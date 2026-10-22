Public Class HNode
    Public HNodes As New Hashtable
    Public Height As Integer
    Public ID As Integer
    Public Identifier As String

    Public Sub New(ByVal pID As Integer, Optional ByVal pHeight As Integer = 0)
        ID = pID
        Height = pHeight
    End Sub

    Public Sub Add(ByVal pID As Integer)
        Dim newHNode As New HNode(pID, Height + 1)
        HNodes.Add(pID, newHNode)
    End Sub

    Public Function Ret(ByVal pID As Integer) As HNode
        Return HNodes.Item(pID)
    End Function

    Public Function HNodeExists(ByVal pID As Integer) As Boolean
        If Not HNodes.Item(pID) Is Nothing Then
            Return True
        Else
            Return False
        End If
    End Function

End Class
