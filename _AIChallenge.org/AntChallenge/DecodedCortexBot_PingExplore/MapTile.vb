Imports System.Collections.Generic
Imports System.Collections.Hashtable
Imports System.Collections

Public Structure LeafPingTiles
    Public MapTile As MapTile
    Public N As Integer
    Public S As Integer
    Public E As Integer
    Public W As Integer
End Structure

Public Class MapTile
    Public ID As String
    Public Row, Col As Integer
    Public Passable As Boolean
    Public Unseen As Boolean
    Public AntHere As Boolean
    Public FoodStrengthHash As Hashtable
    Public EnemyAntHillStrengthHash As Hashtable
    Public MyAntStrengthHash As Hashtable
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
        AntHere = False
        FoodStrengthHash = New Hashtable
        EnemyAntHillStrengthHash = New Hashtable
        MyAntStrengthHash = New Hashtable
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

#Region "Manage Move Explore, Ping Pathfinding"

    Private Sub AddMyAntStrength(pID As String, pStrength As Integer)
        MyAntStrengthHash.Add(pID, pStrength)
    End Sub

    Public Function GetMyAntStrength(pID As String) As Integer
        Dim MyAntStrength As Integer = 0
        If Not MyAntStrengthHash.Item(pID) Is Nothing
            MyAntStrength = CInt(MyAntStrengthHash.Item(pID))
        End If
        Return MyAntStrength
    End Function

    Public Sub ClearMyAntStrengthItems()
        MyAntStrengthHash.Clear
    End Sub

    'Recursively go through each MapTile and set its FoodStrength value, and set a diminished FoodStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetMyAntTileStrength(pID As String, pStrength As Integer, ByRef pListOfLeafTiles As List(Of LeafPingTiles), _
                                    pCountN As Integer, pCountS As Integer, pCountE As Integer, pCountW As Integer)
        'Make sure we are at a true leaf node and did not "backtrack" back towards the ant...
        If Not ((pCountN > 0 AndAlso pCountS > 0) Or (pCountE > 0 AndAlso pCountW > 0))
            If pStrength = 1
                'If this Tile has a strength of 1, then we want to add this maptile to our list of 'pinged' map tiles the ant can move towards
                Dim NewLeafTile As New LeafPingTiles
                NewLeafTile.MapTile = Me
                NewLeafTile.N = pCountN
                NewLeafTile.S = pCountS
                NewLeafTile.E = pCountE
                NewLeafTile.W = pCountW

                'Add this new leaf tile to the reference list of leaf tiles
                pListOfLeafTiles.Add(NewLeafTile)
            Else
                If MyAntStrengthHash.Item(pID) Is Nothing    'If Does Not Exist in HashTable
                    AddMyAntStrength(pID, pStrength)     'Add new value
                ElseIf GetMyAntStrength(pID) < pStrength 'If already exists, but strength value is less than inputed
                    MyAntStrengthHash.Item(pID) = pStrength  'Set the value
                End If

                ''Recursively set the strength of all immediately ajacent tiles
                For Dir As Integer = 0 to LinkedTiles.Length-1
                    If LinkedTiles(Dir).Passable
                        If CType(Dir, Direction) = Direction.n
                            LinkedTiles(Dir).SetMyAntTileStrength(pID, pStrength-1, pListOfLeafTiles, pCountN+1, pCountS, pCountE, pCountW)
                        ElseIf CType(Dir, Direction) = Direction.s
                            LinkedTiles(Dir).SetMyAntTileStrength(pID, pStrength-1, pListOfLeafTiles, pCountN, pCountS+1, pCountE, pCountW)
                        ElseIf CType(Dir, Direction) = Direction.e
                            LinkedTiles(Dir).SetMyAntTileStrength(pID, pStrength-1, pListOfLeafTiles, pCountN, pCountS, pCountE+1, pCountW)
                        Else 'If West
                            LinkedTiles(Dir).SetMyAntTileStrength(pID, pStrength-1, pListOfLeafTiles, pCountN, pCountS, pCountE, pCountW+1)
                        End If
                    End If
                Next
            End If
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

#Region "Manage Enemy Ant Hill Hash"

    Private Sub AddEnemyAntHillStrengthItem(pID As String, pStrength As Integer)
        EnemyAntHillStrengthHash.Add(pID, pStrength)
    End Sub

    Public Function GetEnemyAntHillStrengthItem(pID As String) As Integer
        Dim EnemyAntHillStrength As Integer = 0
        If Not EnemyAntHillStrengthHash.Item(pID) Is Nothing
            EnemyAntHillStrength = CInt(EnemyAntHillStrengthHash.Item(pID))
        End If
        Return EnemyAntHillStrength
    End Function

    Public Sub ClearEnemyAntHillStrengthItems()
        EnemyAntHillStrengthHash.Clear
    End Sub

    'Recursively go through each MapTile and set its FoodStrength value, and set a diminished FoodStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetEnemyAntHillTileStrength(pID As String, pStrength As Integer)
        If pStrength > 0
            If EnemyAntHillStrengthHash.Item(pID) Is Nothing    'If Does Not Exist in HashTable
                AddEnemyAntHillStrengthItem(pID, pStrength)     'Add new value
            ElseIf GetEnemyAntHillStrengthItem(pID) < pStrength 'If already exists, but strength value is less than inputed
                EnemyAntHillStrengthHash.Item(pID) = pStrength  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable                                    'Only do tiles available in a path
                    If AdjacentTile.GetEnemyAntHillStrengthItem(pID) < pStrength-1  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetEnemyAntHillTileStrength(pID, pStrength-1)  
                    End If
                End If
            Next
        End If
    End Sub

#End Region

Public Function GetMapTileAsString() As String
        Return "(" & Row & "," & Col & ")"
End Function


End Class
