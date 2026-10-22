Imports System.Collections.Generic
Imports System.Collections.Hashtable
Imports System.Collections

Public Class MapTile
    Public ID As String
    Public Row, Col As Integer
    Public Passable As Boolean
    Public Unseen As Boolean
    'Public FoodHere As Boolean
    Public FoodStrengthHash As Hashtable
    Public MyAntHillStrength As Integer

    'Linked tiles are designed to be retrieved from the array using the "Direction" enumeration
    'Direction.n = 0, Direction.s = 1, Direction.e = 2, Direction.w = 3
    Public LinkedTiles(3) as MapTile

    Public Sub New(pRow As Integer, pCol As Integer)
        Row = pRow
        Col = pCol
        ID = "(" & Row & "," & Col & ")"
        Passable = False
        Unseen = True
        'FoodHere = False
        FoodStrengthHash = New Hashtable
        MyAntHillStrength = 0
    End Sub

#Region "Manage Food Strength Hash"

    Private Sub AddFoodStrengthItem(pID As String, pStrength As Integer)
        FoodStrengthHash.Add(pID, pStrength)
    End Sub

    Public Function GetFoodStrengthItem(pID As String) As Integer
        Dim FoodItemStrength As Integer = 0
        If Not FoodStrengthHash.Item(pID) Is Nothing
            FoodItemStrength = CInt(FoodStrengthHash.Item(pID))
        End If
        Return FoodItemStrength
    End Function

    Public Sub ClearFoodStrengthItems()
        FoodStrengthHash.Clear
    End Sub

    'Recursively go through each MapTile and set its FoodStrength value, and set a diminished FoodStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetFoodTileStrength(pID As String, pStrength As Integer)
        If pStrength > 0
            If FoodStrengthHash.Item(pID) Is Nothing    'If Does Not Exist in HashTable
                AddFoodStrengthItem(pID, pStrength)     'Add new value
            ElseIf GetFoodStrengthItem(pID) < pStrength 'If already exists, but strength value is less than inputed
                FoodStrengthHash.Item(pID) = pStrength  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable                                    'Only do tiles available in a path
                    If AdjacentTile.GetFoodStrengthItem(pID) < pStrength-1  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetFoodTileStrength(pID, pStrength-1)  
                    End If
                End If
            Next
        End If
    End Sub

#End Region

#Region "Manage MyAntHill Strength Hash"

    Private Sub SetMyAntHillStrength(pStrength As Integer)
        MyAntHillStrength = pStrength
    End Sub

    Public Function GetMyAntHillStrength() As Integer
        Return MyAntHillStrength
    End Function

    Public Sub ClearMyAntHillStrengthItems()
        MyAntHillStrength = 0
    End Sub

    'Recursively go through each MapTile and set its MyAntHillStrength value, and set a diminished MyAntHillStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetMyAntHillTileStrength(pStrength As Integer)
        If pStrength > 0 Then
            If GetMyAntHillStrength() < pStrength Then 'If already exists, but strength value is less than inputed
                SetMyAntHillStrength(pStrength)  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable Then                                    'Only do tiles available in a path
                    If AdjacentTile.GetMyAntHillStrength < pStrength - 1 Then  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetMyAntHillTileStrength(pStrength - 1)
                    End If
                End If
            Next
        End If
    End Sub

#End Region

End Class
