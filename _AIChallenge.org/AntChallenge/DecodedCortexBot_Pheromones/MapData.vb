''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'' Decoded Cortex
'' www.DecodedCortex.com
''
'' Project: Ants - Google AI Challenge Fall 2011
'' Author: Paul Killian
'' Contact: PaulKillian@DecodedCortex.com
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Option Strict On

Imports System.Collections.Generic

Public Class FoodItem
    Public MapTile As MapTile
    Public AntDispatched As Boolean
    Public Sub Dispatched
        AntDispatched = True
    End Sub
    Public Sub ResetDispatched
        AntDispatched = False
    End Sub
End Class


Public Class MapData
    Public _Map(,) As MapTile
    Public _Rows, _Cols As Integer

    Public _FoodItems As New List(Of FoodItem)
    Public _MAXFOODSTRENGTH As Integer = 8
    Public _MAXMYANTHILLSTRENGTH As Integer = 10
    Public _MAXENEMYANTHILLSTRENGTH As Integer = 80
    Public _MAXDEFENDERANTSTRENGTH As Integer = 10
    Public _MAXALLOWEDENEMYANTHILLSTRENGTH As Integer = 80 '100 seems to cause timeouts, dont go above 80
    Public _MAXMYANTPHEROMONESTRENGTH As Integer = 8
    Public _MAXENEMYANTPHEROMONESTRENGTH As Integer = 8
    'Public _MAXENEMYANTPHEROMONESTRENGTH_HUNTERMODE As Integer = 7
    Public _MYANTPHEROMONE_DECREASEAMOUNT As Integer = 1

    Public CountMyAntHills As Integer
    Public CountMyAnts As Integer
    Public CountEnemyAntHills As Integer
    Public CountEnemyAnts As Integer

    Public Sub New(pRows As Integer, pCols As Integer)
        _Rows = pRows
        _Cols = pCols
        _FoodItems = New List(Of FoodItem)
        InitMapTiles()  'Initialize the Tiles array of MapTile objects for the game map
        LinkMapTiles()  'Links all the map tiles together to form a continuum torus
        SetCircleTiles()'Sets an array for all the maptiles surrounding each map tile
    End Sub

    Private Sub InitMapTiles()
        ReDim _Map(_Rows - 1, _Cols - 1)
        For Row As Integer = 0 To _Rows - 1
            For Col As Integer = 0 To _Cols - 1
                _Map(Row, Col) = New MapTile(Row,Col, _MAXMYANTPHEROMONESTRENGTH)
            Next
        Next
    End Sub

    Private Sub LinkMapTiles()
        'Link tiles in each row East-West
        For row As Integer = 0 to _Rows-1
            For col As Integer = 0 to _Cols-1
                If col < _Cols-1    'All columns except edge of map
                    _Map(row, col).LinkedTiles(Direction.e) = _Map(row, col+1)
                    _Map(row, col+1).LinkedTiles(Direction.w) = _Map(row, col)
                Else                'Edge of map columns
                    _Map(row, col).LinkedTiles(Direction.e) = _Map(row, 0)
                    _Map(row, 0).LinkedTiles(Direction.w) = _Map(row, col)
                End If
            Next
        Next

        'Link tiles in each col North-South
        For col As Integer = 0 to _Cols-1
            For row As Integer = 0 to _Rows-1
                If row < _Rows-1    'All columns except edge of map
                    _Map(row, col).LinkedTiles(Direction.s) = _Map(row+1, col)
                    _Map(row+1, col).LinkedTiles(Direction.n) = _Map(row, col)
                Else                'Edge of map columns
                    _Map(row, col).LinkedTiles(Direction.s) = _Map(0, col)
                    _Map(0, col).LinkedTiles(Direction.n) = _Map(row, col)
                End If
            Next
        Next
    End Sub

    Public Sub SetCircleTiles()
        'Call the circle tiles method for each MapTile in the map
        For row As Integer = 0 to _Rows-1
            For col As Integer = 0 to _Cols-1
                _Map(row, col).SetCircleTiles
            Next
        Next
    End Sub

    Public Function GetMapTile(pRow As Integer, pCol As Integer) As MapTile
        Return _Map(pRow, pCol)
    End Function

    Public Sub UpdateMapPathfinderVariables(pCountMapTiles As Integer, pFoodListCount As Integer, pEnemyAntHillCount As Integer, pAntViewRadius As Integer, pAntCombatRadius as Integer, pMyAntCount As Integer, pEnemyAntCount As Integer)

        'Enemy Ant Hill Strength - For each ant hill over 2, divide 100 by that number
        If pEnemyAntHillCount > 0
            _MAXENEMYANTHILLSTRENGTH = CInt(_MAXALLOWEDENEMYANTHILLSTRENGTH / pEnemyAntHillCount)
            If _MAXENEMYANTHILLSTRENGTH < 10
                _MAXENEMYANTHILLSTRENGTH = 10
            End If
        End If

        'If pEnemyAntCount < 5
        '    _MAXENEMYANTPHEROMONESTRENGTH = 15
        'Else
        '    _MAXENEMYANTPHEROMONESTRENGTH = 8
        'End If

        'Food Strength - If few food, increase size of food strength
        _MAXFOODSTRENGTH = pAntViewRadius + 1
        If pFoodListCount < 5
            _MAXFOODSTRENGTH = 10
        Else
            _MAXFOODSTRENGTH = pAntViewRadius+1
        End If



        'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        'Set this to prevent timeouts?
        If pMyAntCount > 150
            _MAXMYANTPHEROMONESTRENGTH = pAntCombatRadius + 2
            '_MAXENEMYANTPHEROMONESTRENGTH = pAntCombatRadius + 1
        End If


    End Sub

    Public Sub UpdateMap(pMapLocations(,) As Tile, pCountMyAntHillLocations As Integer, pCountAntLocations As Integer, pCountEnemyAntLocations As Integer, pCountEnemyAntHillLocation As Integer)
        
        CountMyAntHills = pCountMyAntHillLocations
        CountMyAnts = pCountAntLocations
        CountEnemyAntHills = pCountEnemyAntHillLocation
        CountEnemyAnts = pCountEnemyAntLocations

        'UpdateTileMap(pMapLocations)
        UpdateTileMap_Preserve(pMapLocations)
        UpdateFoodMaps()
    End Sub

    'Refresh the Map with a brand new mapping using the inputted array of tiles
    Private Sub UpdateTileMap(pMapLocations(,) As Tile)
        For Row As Integer = 0 To _Rows - 1
            For Col As Integer = 0 To _Cols - 1
                _Map(Row, Col).ClearFoodStrengthItems   'Clear the food items of the tile so they can be re-calculated
                _Map(Row, Col).ClearEnemyAntHillStrengthItems 'Clear the enemy ant hill strength tile values so they can be re-calculated
                If _Map(Row, Col).MyAntPersistentPheromoneStrength > 0
                    _Map(Row, Col).MyAntPersistentPheromoneStrength -= 1
                End If
                'Maybe switch this to just NOT be a water tile? Because ants move, food disappears, etc.
                If pMapLocations(Row, Col) = Tile.LAND Or pMapLocations(Row, Col) = Tile.FOOD Then
                    _Map(Row, Col).Passable = True
                Else
                    _Map(Row, Col).Passable = False
                End If
            Next
        Next
    End Sub

    'Add to the already known Map with any additional information from the inputted array of tiles
    Private Sub UpdateTileMap_Preserve(pMapLocations(,) As Tile)
        For Row As Integer = 0 To _Rows - 1
            For Col As Integer = 0 To _Cols - 1
                
                ResetMapTileValues(_Map(Row, Col))
                DecreasePersistentPheromoneValue(_Map(Row, Col))

                'Check if we already know about this tile. If we do, no need to update it.
                'If _Map(Row, Col).Unseen = True Then
                    'Ignore all UNSEEN tiles of the map (Unseen from the inputted mapping perspective)
                    'If pMapLocations(Row, Col) <> Tile.UNSEEN Then
                        'We see the tile, so see if it is passable or not
                        If pMapLocations(Row, Col) <> Tile.WATER Then
                            _Map(Row, Col).Passable = True
                        Else
                            _Map(Row, Col).Passable = False
                        End If
                        'We now know what tile this is, even if we can't see it later, 
                        'so set the value that we have seen it so we can preserve the info.
                        _Map(Row, Col).Unseen = False
                    'End If
                'End If

            Next
        Next
    End Sub

    Public Sub ResetMapTileValues(pMapTile As MapTile)
                pMapTile.ClearFoodStrengthItems()   'Clear the food items of the tile so they can be re-calculated
                pMapTile.ClearEnemyAntHillStrengthItems 'Clear the enemy ant hill strength tile values so they can be re-calculated
                pMapTile.ClearImmediatePheromoneStrengths()  'Reset this value each time
                
                '    
                pMapTile.MyAntAttractionStrengthHash.Clear ' Clear this hash table...
                '

                pMapTile.ClearEnemyAntStrengths
                pMapTile.AlertStrength = 0
                pMapTile.MyAntHillStrength = 0

                pMapTile.AntHere = False    'Reset to false, make sure to perform the colony ant location update check

                pMapTile.AntReservedTileForNextMove = False

                pMapTile.ClearDefenderAntPositionsStrength

        '
        '
        '
        '
        pMapTile._MyAntEdgeTileList.Clear
        '
        '
        '
        '
    End Sub

    Public Sub DecreasePersistentPheromoneValue(pMapTile As MapTile)
        If pMapTile.MyAntPersistentPheromoneStrength > 0
            pMapTile.MyAntPersistentPheromoneStrength -= _MYANTPHEROMONE_DECREASEAMOUNT
            pMapTile.MyAntPersistentPheromoneStrength = CInt(pMapTile.MyAntPersistentPheromoneStrength * 0.9)
        End If
    End Sub

    'Pre-condition: All Tiles on the Map have had their FoodStrength Hashtables cleared, we will re-calculate
    Private Sub UpdateFoodMaps()
        For Each FoodItem As FoodItem In _FoodItems
            FoodItem.MapTile.SetFoodTileStrength(FoodItem.MapTile.ID, _MAXFOODSTRENGTH)
        Next
    End Sub

    'Need to add newly discovered food items, keep a peristent state of already known food items, and remove no-longer existing food items.
    Public Sub UpdateMapFoodItems(pFoodLocations As List(Of Location))
        '_FoodItems.Clear
        'Add new food items to the list from the current list of food items
        'If pFoodLocations.Count > 0
            For Each FoodLocation As Location In pFoodLocations
                If Not FoodItemExists(FoodLocation) Then
                    AddFoodItem(GetMapTile(FoodLocation.Row, FoodLocation.Col))
                End If
            Next
        'End If

        'Remove food items from the food item list that are not in the newest food list (therefore they no longer exist on the map)
        Dim FoodItemsToBeRemoved As New List(Of FoodItem)
        For Each FoodItem As FoodItem In _FoodItems
            If Not FoodLocationExistsInFoodItems(FoodItem, pFoodLocations) Then
                FoodItemsToBeRemoved.Add(FoodItem)
            End If
        Next

        'Now actually remove the food items
        For Each FoodItem As FoodItem In FoodItemsToBeRemoved
            RemoveFoodItem(FoodItem.MapTile)
        Next
    End Sub

    Public Function FoodItemExists(pFoodItemLocation As Location) As Boolean
        Dim Exists As Boolean = False
        For Each ExistingFoodItem As FoodItem In _FoodItems
            If pFoodItemLocation.Col = ExistingFoodItem.MapTile.Col AndAlso pFoodItemLocation.Row = ExistingFoodItem.MapTile.Row Then
                Exists = True
                Exit For
            End If
        Next
        Return Exists
    End Function

    Public Function FoodLocationExistsInFoodItems(pFoodItem As FoodItem, pFoodLocations As List(Of Location)) As Boolean
        Dim Exists As Boolean = False
        For Each FoodLocation As Location In pFoodLocations
            If FoodLocation.Col = pFoodItem.MapTile.Col AndAlso FoodLocation.Row = pFoodItem.MapTile.Row Then
                Exists = True
                Exit For
            End If
        Next
        Return Exists
    End Function

    'Add FoodItem to persistent list of FoodItems
    Public Sub AddFoodItem(pMapTile As MapTile)
        Dim NewFoodItem As New FoodItem
        NewFoodItem.MapTile = pMapTile
        NewFoodItem.AntDispatched = False
        _FoodItems.Add(NewFoodItem)
    End Sub

    Public Sub AddFoodItem(pRow As Integer, pCol As Integer)
        AddFoodItem(_Map(pRow, pCol))
    End Sub

    'Remove FoodItem from persistent list of FoodItems
    Public Sub RemoveFoodItem(pMapTile As MapTile)
        For Each FoodItem As FoodItem In _FoodItems
            If FoodItem.MapTile.Equals(pMapTile) Then
                _FoodItems.Remove(FoodItem)
                Exit For 'We found it and removed it, so stop looking!
            End If
        Next
    End Sub

    Public Sub RemoveFoodItem(pRow As Integer, pCol As Integer)
        RemoveFoodItem(_Map(pRow, pCol))
    End Sub

    Public Sub UpdateMapEnemyAntHillLocation(pEnemyAntHillLocation As List(Of Location))

        For Each EnemyAntHillLocation As Location In pEnemyAntHillLocation
            GetMapTile(EnemyAntHillLocation.Row, EnemyAntHillLocation.Col).SetEnemyAntHillTileStrength( _
                            "(" & EnemyAntHillLocation.Row & "," & EnemyAntHillLocation.Col & ")", _MAXENEMYANTHILLSTRENGTH)
        Next
    End Sub

    Public Sub UpdateMyAntPheromoneMap(pAntLocations As List(Of ColonyAnt))
        For Each Ant As ColonyAnt In pAntLocations
            GetMapTile(Ant.CurrentMapTile.Row, Ant.CurrentMapTile.Col).AntHere = True
            GetMapTile(Ant.CurrentMapTile.Row, Ant.CurrentMapTile.Col).SetMyAntPheromoneTileStrength_Both("(" & Ant.CurrentMapTile.Row & "," & Ant.CurrentMapTile.Col & ")", Ant.ID,  _MAXMYANTPHEROMONESTRENGTH, Ant.CurrentMapTile._MyAntEdgeTileList)
        Next
    End Sub

    Public Sub UpdateMyAntHillsMap(pAntHillLocations As List(Of Location))
        If pAntHillLocations.Count > 0
            For Each AntHill As Location In pAntHillLocations
            GetMapTile(AntHill.Row, AntHill.Col).SetMyAntHillTileStrength(_MAXMYANTHILLSTRENGTH)
        Next
        End If
    End Sub

    Public Sub UpdateDefenderAntPositions(pDefenderAntPositions As List(Of MapTile))
        For Each Tile As MapTile In pDefenderAntPositions
            Tile.SetDefenderAntPositionTileStrength(_MAXDEFENDERANTSTRENGTH)
        Next
    End Sub

    Public Sub UpdateEnemyAntPheromoneMap(pEnemyAntLocations As List(Of Location))  
        For Each EnemyAnt As Location In pEnemyAntLocations
            GetMapTile(EnemyAnt.Row, EnemyAnt.Col).SetEnemyAntPheromoneTileStrength(GetIDStringFromLocation(EnemyAnt), _MAXENEMYANTPHEROMONESTRENGTH)
        Next
    End Sub


    Public Function GetIDStringFromLocation(pLocation As Location) As String
        Return "(" & pLocation.Row & "," & pLocation.Col & ")"
    End Function


#Region "Debugging Code"

    Public Sub OutputMapPassableToFile(pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & ".txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Unseen
                        sw.Write("?")
                    ElseIf _Map(Row, Col).Passable
                        sw.Write("-")
                    ElseIf Not _Map(Row, Col).Passable
                        sw.Write("X")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub

    Public Sub OutputMapMyAntHillStrengthToFile(pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & ".txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).MyAntHillStrength = 0
                        sw.Write("---")
                    Else
                        sw.Write(CStr(_Map(Row, Col).MyAntHillStrength).PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub

    Public Sub OutputMapDefenderStrengthToFile(pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & ".txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).DefenderAntPositionStrength = 0
                        sw.Write("---")
                    Else
                        sw.Write(CStr(_Map(Row, Col).DefenderAntPositionStrength).PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub


    Public Sub OutputMapEnemyAntHillsToFile(pFilePath As String, pEnemyAntHills As List(Of Location))
        If pEnemyAntHills.Count > 0
            For Each EnemyAntHill As Location In pEnemyAntHills
                OutputMapEnemyAntHillsToFile(pFilePath, "(" & EnemyAntHill.Row & "," & EnemyAntHill.Col & ")")
            Next
        End If
    End Sub

    Public Sub OutputMapEnemyAntHillsToFile(pFilePath As String, pEnemyAntHillID As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & pEnemyAntHillID & ".txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).EnemyAntHillStrengthHash(pEnemyAntHillID) Is Nothing
                        sw.Write("---")
                    Else
                        sw.Write(CStr(_Map(Row, Col).EnemyAntHillStrengthHash(pEnemyAntHillID)).PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub

    Public Sub OutputMapMyAntPersistentPheromoneToFile(pFilePath As String, Optional pTurnIndex As String = "")
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & "MyAntsPersistentPheromone" & pTurnIndex & ".txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXXX")
                    ElseIf _Map(Row, Col).MyAntPersistentPheromoneStrength = 0
                        sw.Write("----")
                    ElseIf _Map(Row, Col).AntHere = True
                        'sw.Write("_A_")
                            If _Map(Row, Col).MyAntPersistentPheromoneStrength > 999
                                sw.Write("***#")
                            Else
                                sw.Write(CStr(_Map(Row, Col).MyAntPersistentPheromoneStrength).PadLeft(3,"0"c) & "#")
                            End If
                    Else
                        If _Map(Row, Col).MyAntPersistentPheromoneStrength > 999
                                sw.Write("***,")
                            Else
                                sw.Write(CStr(_Map(Row, Col).MyAntPersistentPheromoneStrength).PadLeft(3,"0"c) & ",")
                            End If
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub

Public Sub OutputMapMyAntImmediatePheromoneToFile(pID As String, pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & "MyAntsImmediatePheromone-" & pID & ".txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).MyAntImmediatePheromoneStrengthHash(pID) Is Nothing
                        sw.Write("---")
                    Else
                        sw.Write(CStr(_Map(Row, Col).MyAntImmediatePheromoneStrengthHash(pID)).PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub


Public Sub OutputMapMyAntSinglePersistentPheromoneToFile(pAntID As String, pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & "MyAntsSinglePersistentPheromone-" & pAntID & ".txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).MyAntSinglePersistentPheromoneStrength(pAntID) Is Nothing
                        sw.Write("---")
                    Else
                        sw.Write(CStr(_Map(Row, Col).MyAntSinglePersistentPheromoneStrength(pAntID)).PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub


    Public Sub OutputMapEnemyAntPheromoneToFile(pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & "EnemyAntsPheromone.txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).EnemyAntPheromoneStrengthHash.Count = 0
                        sw.Write("---")
                    Else
                        Dim Sum As Integer = 0
                        For Each Value As Integer In _Map(Row, Col).EnemyAntPheromoneStrengthHash.Values
                            Sum += Value
                        Next
                        sw.Write(CStr(Sum).PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub

    Public Sub OutputMapEnemyAntPheromoneToFile2(pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & "EnemyAntsPheromone3.txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).EnemyAntPheromoneStrengthHash.Count = 0
                        sw.Write("---")
                    Else
                        Dim Count As Integer = _Map(Row, Col).EnemyAntPheromoneStrengthHash.Count
                        sw.Write(CStr(Count).PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub

    Public Sub OutputMapEnemyAntPheromoneToFile3(pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & "EnemyAntsPheromone3.txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).EnemyAntPheromoneStrengthHash.Count = 0
                        sw.Write("---")
                    Else
                        Dim Ant As String = "00"
                        For Each Value As Integer In _Map(Row, Col).EnemyAntPheromoneStrengthHash.Values
                            If Value = _MAXENEMYANTPHEROMONESTRENGTH
                                    Ant = "AA"
                            End If
                        Next
                        sw.Write(Ant.PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub

    Public Sub OutputMapAlertToFile(pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath & "Alert" & ".txt")
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Passable = False
                        sw.Write("XXX")
                    ElseIf _Map(Row, Col).AlertStrength = 0
                        sw.Write("---")
                    Else
                        sw.Write(CStr(_Map(Row, Col).AlertStrength).PadLeft(2,"0"c) & ",")
                    End If
                Next
                sw.WriteLine()
            Next
            sw.Close
        End Using

        Catch ex As Exception

        End Try
        
    End Sub

#End Region

End Class

