Option Strict On

Imports System.Collections.Generic

Public Structure FoodItem
    Public MapTile As MapTile
    Public AntDispatched As Boolean
End Structure


Public Class MapData
    Public _Map(,) As MapTile
    Public _Rows, _Cols As Integer

    Public _MyAntHill As MapTile
    Public _FoodItems As New List(Of FoodItem)
    Const _MAXFOODSTRENGTH As Integer = 15
    Const _MAXMYANTHILLSTRENGTH As Integer = 40

    Public Sub New(pRows As Integer, pCols As Integer)
        _Rows = pRows
        _Cols = pCols
        _FoodItems = New List(Of FoodItem)
        InitMapTiles()  'Initialize the Tiles array of MapTile objects for the game map
        LinkMapTiles()  'Links all the map tiles together to form a continuum torus
    End Sub

    Private Sub InitMapTiles()
        ReDim _Map(_Rows - 1, _Cols - 1)
        For Row As Integer = 0 To _Rows - 1
            For Col As Integer = 0 To _Cols - 1
                _Map(Row, Col) = New MapTile(Row,Col)
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

    Public Function GetMapTile(pRow As Integer, pCol As Integer) As MapTile
        Return _Map(pRow, pCol)
    End Function

    Public Sub UpdateMyAntHillLocation(pMyAntHillLocation As Location)
        _MyAntHill = GetMapTile(pMyAntHillLocation.Row, pMyAntHillLocation.Col)
    End Sub

    Public Sub UpdateMap(pMapLocations(,) As Tile)
        'UpdateTileMap(pMapLocations)
        UpdateTileMap_Preserve(pMapLocations)
        UpdateFoodMaps()
        UpdateMyAntHillMap()
    End Sub

    'Refresh the Map with a brand new mapping using the inputted array of tiles
    Private Sub UpdateTileMap(pMapLocations(,) As Tile)
        For Row As Integer = 0 To _Rows - 1
            For Col As Integer = 0 To _Cols - 1
                _Map(Row, Col).ClearFoodStrengthItems   'Clear the food items of the tile so they can be re-calculated
                'Maybe switch this to just NOT be a water tile? Because ants move, food disappears, etc.
                If pMapLocations(Row, Col) = Tile.LAND Or pMapLocations(Row, Col) = Tile.FOOD Then
                    _Map(Row, Col).Passable = True
                    'If pMapLocations(Row, Col) = Tile.FOOD Then
                    '    _Map(Row, Col).FoodHere = True
                    'End If
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
                _Map(Row, Col).ClearFoodStrengthItems()   'Clear the food items of the tile so they can be re-calculated

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

    'Pre-condition: All Tiles on the Map have had their FoodStrength Hashtables cleared, we will re-calculate
    Private Sub UpdateFoodMaps()
        For Each FoodItem As FoodItem In _FoodItems
            FoodItem.MapTile.SetFoodTileStrength(FoodItem.MapTile.ID, _MAXFOODSTRENGTH)
        Next
    End Sub

    'Pre-condition: All Tiles on the Map have had their MyAntHillStrength Hashtables cleared, we will re-calculate
    Private Sub UpdateMyAntHillMap()
        If Not _MyAntHill Is Nothing
            _MyAntHill.SetMyAntHillTileStrength(_MAXMYANTHILLSTRENGTH)
        End If
    End Sub

    'Need to add newly discovered food items, keep a peristent state of already known food items, and remove no-longer existing food items.
    Public Sub UpdateMapFoodItems(pFoodLocations As List(Of Location))
        'Add new food items to the list from the current list of food items
        For Each FoodLocation As Location In pFoodLocations
            If Not FoodItemExists(FoodLocation) Then
                'AddFoodItem(_Map(FoodLocation.Row, FoodLocation.Col))

                AddFoodItem(GetMapTile(FoodLocation.Row, FoodLocation.Col))
            End If
        Next

        'Remove food items from the food item list that are not in the newest food list (therefore they no longer exist on the map)
        Dim FoodItemsToBeRemoved As New List(Of FoodItem)
        For Each FoodItem As FoodItem In _FoodItems
            If Not FoodLocationExistsInFoodItems(FoodItem, pFoodLocations) Then
                FoodItemsToBeRemoved.Add(FoodItem)
                'RemoveFoodItem(FoodItem.MapTile)
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
        Dim NewFoodItem As FoodItem
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

    


    '
    '
    '
#Region "Debugging Code"

    Public Sub OutputMapPassableToFile(pFilePath As String)
        Try

        Using sw As New System.IO.StreamWriter(pFilePath)
            'sw.Write(sb.ToString())
            For Row As Integer = 0 to _Rows-1
                For Col As Integer = 0 to _Cols-1
                    If _Map(Row, Col).Unseen
                        sw.Write("?")
                    ElseIf _Map(Row, Col).Passable
                        sw.Write(" ")
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

#End Region
End Class

