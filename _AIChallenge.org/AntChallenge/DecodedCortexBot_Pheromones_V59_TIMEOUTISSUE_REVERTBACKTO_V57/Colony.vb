Option Strict On

Imports System.Collections.Generic
Imports System.Collections.Hashtable
Imports System.Collections

Public Interface IColony
    Sub UpdateColony(pMapTiles(,) As Tile, pFoodLocations As List(Of Location), pAntLocations As List(Of Location), _
                            pMyAntHillLocation As List(Of Location), pEnemyAntHillLocation As List(Of Location), _
                            pEnemyAntLocations As List(Of Location), pAntViewRadius As Integer)
    Function GetMapTile(pRow As Integer, pCol As Integer) As MapTile
    Function GetAntMoveDirection(pAntLocation As Location) As Direction
    Sub MoveAnt(pAntLocation As Location, pMoveDirection As Direction)
    Function AntExistAtLocation(pLocation As Location) As Boolean

    Function xGetAnts() As List(Of ColonyAnt)   'Debugging Logic Method
End Interface

Public Class Colony : Implements IColony
    Private _ColonyAnts As List(Of ColonyAnt)
    Private _ColonyAntHills As List(Of ColonyAntHill)
    Public _Map As MapData
    Public _EnemyAntHillPredictionEngine As IColonyEnemyAntHillPredictionEngine
    Private _EnemyAntHillMemoryTTL As Hashtable
    Const MAXENEMYANTHILLMEMORYTTL As Integer = 25
    Private _ArchivedEnemyAntHills As List(Of Location)
    Private _DestroyedEnemyAntHills As List(Of Location)
    Private _TurnCount As Integer
    Private _TurnsSinceLastSeenEnemyAntHill As Integer
    Private _CountDefenderAnts As Integer

    Private _Random As New Random(Now.Millisecond)

    Public Sub New(pMapRows As Integer, pMapCols As Integer)
        _Map = New MapData(pMapRows, pMapCols)
        _ColonyAnts = New List(Of ColonyAnt)
        _ColonyAntHills = New List(Of ColonyAntHill)
        _EnemyAntHillPredictionEngine = New ColonyEnemyAntHillPredictionEngine(_Random)
        _EnemyAntHillMemoryTTL = New Hashtable
        _ArchivedEnemyAntHills = New List(Of Location)
        _DestroyedEnemyAntHills = New List(Of Location)
        _TurnCount = 0
        _TurnsSinceLastSeenEnemyAntHill = 0
        _CountDefenderAnts = 0
    End Sub

    Public Sub UpdateColony(pMapTiles(,) As Tile, pFoodLocations As List(Of Location), pAntLocations As List(Of Location), _
                                                pMyAntHillLocation As List(Of Location), pEnemyAntHillLocations As List(Of Location), _
                                                pEnemyAntLocations As List(Of Location), pAntViewRadius As Integer) Implements IColony.UpdateColony
        
        _TurnCount += 1
        UpdateColonyAnts(pAntLocations)
        UpdateColonyAntHills(pMyAntHillLocation)
        MaintainEnemyAntHillArchive(pEnemyAntHillLocations)

        Dim EnemyAntHillLocations As List(Of Location)
        EnemyAntHillLocations = _EnemyAntHillPredictionEngine.ExecutePredictionEngine(_TurnCount, _ColonyAnts, _ColonyAntHills, _
                                                        pEnemyAntLocations, pEnemyAntHillLocations, _ArchivedEnemyAntHills, _ 
                                                        _DestroyedEnemyAntHills, _Map)

        Dim ActiveEnemyAntHillLocations As List(Of Location) = GetActiveEnemyAntHillLocations(EnemyAntHillLocations)
        MaintainDestroyedEnemyAntHillList(ActiveEnemyAntHillLocations, pAntLocations)
        UpdateMapData(pMapTiles, pFoodLocations, pMyAntHillLocation, ActiveEnemyAntHillLocations, pAntLocations, pEnemyAntLocations, pAntViewRadius)
        
        VerifyCurrentAssignedMapFoodItems() 'Verify current food assignments are valid (food items still exist)
        AssignMapFoodItems()    'This occurs after the food item 'strengths' have been added to the map

        For Each AntHill As ColonyAntHill In _ColonyAntHills
            AntHill.UpdateAntHillDefenceVariables(_Map._Rows, _Map._Cols)
            AntHill.CalculateLookoutTowerLocations()
            AntHill.CheckLookoutTowers
        Next

        'Dim MazeMapDetected As Boolean = MazeDetection

    End Sub

    Public Sub MaintainDestroyedEnemyAntHillList(pEnemyAntHills As List(Of Location), pMyAnts As List(Of Location))
        For Each EnemyAntHill As Location In pEnemyAntHills
            For Each MyAnt As Location In pMyAnts
                If EnemyAntHill.Row = MyAnt.Row AndAlso EnemyAntHill.Col = MyAnt.Col
                    'My ant currently exists on the location of an enemy ant hill (predicted or real hill)
                    'Check if the Enemy Ant hill location currently exists in the list of destroyed hills
                    If Not _DestroyedEnemyAntHills.Contains(EnemyAntHill)
                        _DestroyedEnemyAntHills.Add(EnemyAntHill)   'Add the hill to the list of "destroyed" hills
                    End If
                End If
            Next
        Next
    End Sub

    Public Sub MaintainEnemyAntHillArchive(pEnemyAntHillLocations as List(Of Location))
        If pEnemyAntHillLocations.Count > 0
            For Each AntHillLocation As Location In pEnemyAntHillLocations
                If Not _ArchivedEnemyAntHills.Contains(AntHillLocation)
                    _ArchivedEnemyAntHills.Add(AntHillLocation)
                End If
            Next
        End If
    End Sub

    Private Sub UpdateMapData(pMapTiles(,) As Tile, pFoodLocations As List(Of Location), pMyAntHillLocations As List(Of Location), _
                                    pEnemyAntHillLocation As List(Of Location), pAntLocations As List(Of Location), _ 
                                    pEnemyAntLocations As List(Of Location), pAntViewRadius As Integer)

        _Map.UpdateMapPathfinderVariables(pMapTiles.Length, pFoodLocations.Count, pEnemyAntHillLocation.Count, pAntViewRadius, pAntLocations.Count, pEnemyAntLocations.Count)

        _Map.UpdateMapFoodItems(pFoodLocations)         'This must be done before UpdateMap

        _Map.UpdateMap(pMapTiles, pMyAntHillLocations.Count, pAntLocations.Count, pEnemyAntLocations.Count, pEnemyAntHillLocation.Count)                       'This must be done second (requires up to date list of FoodItems)
        
        _Map.UpdateMapEnemyAntHillLocation(pEnemyAntHillLocation)

        _Map.UpdateMyAntHillsMap(pMyAntHillLocations)

        _Map.UpdateMyAntPheromoneMap(pAntLocations) 'This needs to be done after the UpdateMap function so we have an up-to-date map and ensure Pheromones decreased

        _Map.UpdateEnemyAntPheromoneMap(pEnemyAntLocations) 'This needs to be done after the UpdateMap function so we have an up-to-date map

    End Sub

    Public Sub VerifyCurrentAssignedMapFoodItems()
        For Each Ant As ColonyAnt In _ColonyAnts
            If Ant.AssignedFoodItemID <> ""
                If CInt(Ant.CurrentMapTile.FoodStrengthHash(Ant.AssignedFoodItemID)) = 0
                    Ant.AssignedFoodItemID = ""
                End If
            End If
        Next
    End Sub


    Public Sub AssignMapFoodItems()
        Dim FoodItemAssigned As Boolean
        Dim HighestFoodStrength As Integer
        Dim HighestFoodAnt As ColonyAnt

        For Each FoodItem As FoodItem In _Map._FoodItems
            FoodItemAssigned = False
            For Each Ant As ColonyAnt In _ColonyAnts
                If Ant.AssignedFoodItemID = FoodItem.MapTile.ID
                    FoodItemAssigned = True
                End If
            Next
            If Not FoodItemAssigned
                HighestFoodStrength = 0 'This this to 0, NOT Integer.MinValue
                HighestFoodAnt = Nothing
                'This food item has not been assigned to an ant. Assign it now to the ant on the highest strength tile not already dispatched
                For Each Ant As ColonyAnt In _ColonyAnts
                    If Ant.AssignedFoodItemID = ""
                        If CInt(Ant.CurrentMapTile.FoodStrengthHash(FoodItem.MapTile.ID)) > HighestFoodStrength
                            HighestFoodStrength = CInt(Ant.CurrentMapTile.FoodStrengthHash(FoodItem.MapTile.ID))
                            HighestFoodAnt = Ant
                        End If
                    End If
                Next
                If Not HighestFoodAnt Is Nothing
                    'Assign this food item to this ant
                    HighestFoodAnt.AssignedFoodItemID = FoodItem.MapTile.ID
                End If
            End If
        Next
    End Sub

    Public Function GetMapTile(pRow As Integer, pCol As Integer) As MapTile Implements IColony.GetMapTile
        Return _Map._Map(pRow, pCol)
    End Function

#Region "Maintain Colony Ant List Logic"

    Private Sub UpdateColonyAnts(pAntLocations As List(Of Location))
        'Add new ants to the list from the current list of colony ants
        For Each AntLocation As Location In pAntLocations
            If Not AntExists(AntLocation)
                AddNewAnt(AntLocation)
            End If
        Next

        'Remove ants from the colony ant list that are not in the newest ant list (therefore they no longer exist on the map)
        Dim AntsToBeRemoved As New List(Of ColonyAnt)
        For Each Ant As ColonyAnt In _ColonyAnts
            If Not AntLocationExistsInColonyAnts(Ant, pAntLocations)
                AntsToBeRemoved.Add(Ant)
            End If
        Next

        'Now actually remove the food items
        For Each Ant As ColonyAnt In AntsToBeRemoved
            RemoveAnt(Ant)
        Next
    End Sub

        Public Function AntExists(pAntLocation As Location) As Boolean
        Dim Exists As Boolean = False
        For Each ExistingAnt As ColonyAnt In _ColonyAnts
            If pAntLocation.Col = ExistingAnt.CurrentMapTile.Col AndAlso pAntLocation.Row = ExistingAnt.CurrentMapTile.Row
                Exists = True
                Exit For
            End If
        Next
        Return Exists
    End Function

    Public Function AntLocationExistsInColonyAnts(pColonyAnt As ColonyAnt, pAntList As List(Of Location)) As Boolean
        Dim Exists As Boolean = False
        For Each Ant As Location In pAntList
            If Ant.Col = pColonyAnt.CurrentMapTile.Col AndAlso Ant.Row = pColonyAnt.CurrentMapTile.Row
                Exists = True
                Exit For
            End If
        Next
        Return Exists
    End Function

    'Remove ColonyAnt from persistent list of ColonyAnts
    Public Sub RemoveAnt(pColonyAnt As ColonyAnt)
        For Each Ant As ColonyAnt In _ColonyAnts
            If Ant.Equals(pColonyAnt)
                If Ant.Mission = ColonyAntMission.AntHillDefender
                    _CountDefenderAnts -= 1
                End If
                _ColonyAnts.Remove(Ant)
                Exit For 'We found it and removed it, so stop looking!
            End If
        Next
    End Sub

#End Region                               

#Region "Colony Ant List Maintanence Logic"

    Public Function AddNewAnt(pLocation As Location) As ColonyAnt
        Dim AntMission As ColonyAntMission
        If _TurnCount > 15 AndAlso _ColonyAnts.Count > 5 AndAlso _CountDefenderAnts < CInt(_ColonyAnts.Count/10)
            AntMission = ColonyAntMission.AntHillDefender
            _CountDefenderAnts += 1
        Else
            AntMission = ColonyAntMission.Explorer
        End If
        Dim newColonyAnt As ColonyAnt = New ColonyAnt(GetMapTile(pLocation.Row, pLocation.Col), _Random, AntMission)
        _ColonyAnts.Add(newColonyAnt)
        Return newColonyAnt
    End Function

    Public Function GetAnt(pLocation As Location) As ColonyAnt
        Dim selectedAnt As New ColonyAnt(GetMapTile(pLocation.Row, pLocation.Col), _Random)
        Dim FoundAnt As Boolean = False
        If Not _ColonyAnts Is Nothing
            For Each ant As ColonyAnt In _ColonyAnts
                If pLocation.Col = ant.CurrentMapTile.Col AndAlso pLocation.Row = ant.CurrentMapTile.Row
                    selectedAnt = ant
                    FoundAnt = True
                End If
            Next
        End If
        'Ant must not have been in the list, add new ant.
        If FoundAnt = False
            selectedAnt = AddNewAnt(pLocation)    
        End If
        Return selectedAnt
    End Function

    'Public Sub UpdateAnt(pOldLoc As Location, pNewLoc As Location, pDirection As Direction)
    '    For Each ant As ColonyAnt In _ColonyAnts
    '        If pOldLoc.Col = ant.CurrentMapTile.Col AndAlso pOldLoc.Row = ant.CurrentMapTile.Row
    '            ant.CurrentMapTile = GetMapTile(pNewLoc.Row, pNewLoc.Col)
    '            ant.SetPreviousDirection(pDirection)
    '        End If
    '    Next
    'End Sub

#End Region

#Region "Ant Orders Logic"

    'Steps taken to move an ant.
    '1) Get desired move direction for each ant
    '2) Verify in the game that the move is valid (passable)
    '3) Actually move the ant in the colony object

    Public Function GetAntMoveDirection(pAntLocation As Location) As Direction Implements IColony.GetAntMoveDirection
        Dim MoveDirection As Direction

        MoveDirection = GetAnt(pAntLocation).GetAntMoveDirection

        Return MoveDirection
    End Function

    Public Sub MoveAnt(pAntLocation As Location, pMoveDirection As Direction) Implements IColony.MoveAnt
        GetAnt(pAntLocation).MoveAnt(pMoveDirection)
    End Sub

    #End Region

    Public Function AntExistAtLocation(pLocation As Location) As Boolean Implements IColony.AntExistAtLocation
        Dim AntExists As Boolean = False

        For Each Ant As ColonyAnt In _ColonyAnts
            If Ant.CurrentMapTile.Row = pLocation.Row AndAlso Ant.CurrentMapTile.Col = pLocation.Col Then
                AntExists = True
            End If
        Next

        Return AntExists
    End Function

    Friend Function GetActiveEnemyAntHillLocations(pEnemyAnyHillLocations As List(Of Location), Optional pReduceTimers As Boolean = True) As List(Of Location)
        Dim ActiveEnemyAntHillLocations As New List(Of Location)

        If pReduceTimers
            'Reduce all TTL values currently in the Enemy Ant Hill Memory list by 1
            Dim AllActiveEnemyHillKeys As New List(Of String)
            For Each ActiveEnemyHillKey As String In _EnemyAntHillMemoryTTL.Keys
                AllActiveEnemyHillKeys.Add(ActiveEnemyHillKey)
            Next
            For Each Key As String In AllActiveEnemyHillKeys
                _EnemyAntHillMemoryTTL(Key) = CInt(_EnemyAntHillMemoryTTL(Key)) - 1 
            Next
        End If

        'Check if the hills that current exist in the map already exist in the hash table of "remembered" enemy ant hill locations.
        'If the enemy ant hill already exists, reset its value to the max value. If it does not exist, add it to the hash table.
        Dim HillExists As Boolean
        For Each EnemyAntHillLocation As Location In pEnemyAnyHillLocations
            HillExists = False
            For Each ActiveEnemyHillKey As String In _EnemyAntHillMemoryTTL.Keys
                If GetLocationAsString(EnemyAntHillLocation) = ActiveEnemyHillKey
                    HillExists = True
                    _EnemyAntHillMemoryTTL(ActiveEnemyHillKey) = MAXENEMYANTHILLMEMORYTTL
                    Exit For
                End If
            Next
            If Not HillExists
                _EnemyAntHillMemoryTTL.Add(GetLocationAsString(EnemyAntHillLocation), MAXENEMYANTHILLMEMORYTTL)
            End If
        Next

        'Check if any remembered enemy ant hills have a TTL of 0 or less. If so, remove them from the list. We wont remember them.
        Dim RemoveKeys As New List(Of String)
        For Each ActiveEnemyHillKey As String In _EnemyAntHillMemoryTTL.Keys
            If CInt(_EnemyAntHillMemoryTTL(ActiveEnemyHillKey)) <= 0 Then
                RemoveKeys.Add(ActiveEnemyHillKey)
            End If
        Next
        For Each Key As String In RemoveKeys
            _EnemyAntHillMemoryTTL.Remove(Key)
        Next

        'Now create a list of all all active remember enemy ant hills from the list... this algorithm is kinda obtuse and non-elegant...
        Dim Split() As String
        For Each ActiveEnemyHillKey As String In _EnemyAntHillMemoryTTL.Keys
            Split = ActiveEnemyHillKey.Split("("c)
            Split = Split(1).Split(")"c)
            Split = Split(0).Split(","c)
            Dim ActiveLocation As New Location(CInt(Split(0)), CInt(Split(1)))
            ActiveEnemyAntHillLocations.Add(ActiveLocation)
        Next

        Return ActiveEnemyAntHillLocations
    End Function

#Region "Maintain Ant Hill List Logic"

    Private Sub UpdateColonyAntHills(pAntHillLocations As List(Of Location))
        'Add new ants to the list from the current list of colony ants
        For Each AntHillLocation As Location In pAntHillLocations
            If Not AntHillExists(AntHillLocation)
                AddNewAntHill(AntHillLocation)
            End If
        Next

        'Remove ants from the colony ant list that are not in the newest ant list (therefore they no longer exist on the map)
        Dim AntHillsToBeRemoved As New List(Of ColonyAntHill)
        For Each AntHill As ColonyAntHill In _ColonyAntHills
            If Not AntHillLocationExistsInColonyAntHills(AntHill, pAntHillLocations)
                AntHillsToBeRemoved.Add(AntHill)
            End If
        Next

        'Now actually remove the food items
        For Each AntHill As ColonyAntHill In AntHillsToBeRemoved
            RemoveAntHill(AntHill)
        Next
    End Sub

    Public Function AddNewAntHill(pLocation As Location) As ColonyAntHill
        Dim newColonyAntHill As ColonyAntHill = New ColonyAntHill(GetMapTile(pLocation.Row, pLocation.Col))
        _ColonyAntHills.Add(newColonyAntHill)
        Return newColonyAntHill
    End Function

    Public Function AntHillExists(pAntHillLocation As Location) As Boolean
        Dim Exists As Boolean = False
        For Each ExistingAntHill As ColonyAntHill In _ColonyAntHills
            If pAntHillLocation.Col = ExistingAntHill._AntHill.Col AndAlso pAntHillLocation.Row = ExistingAntHill._AntHill.Row
                Exists = True
                Exit For
            End If
        Next
        Return Exists
    End Function

    Public Function AntHillLocationExistsInColonyAntHills(pColonyAntHill As ColonyAntHill, pAntHillList As List(Of Location)) As Boolean
        Dim Exists As Boolean = False
        For Each AntHill As Location In pAntHillList
            If AntHill.Col = pColonyAntHill._AntHill.Col AndAlso AntHill.Row = pColonyAntHill._AntHill.Row
                Exists = True
                Exit For
            End If
        Next
        Return Exists
    End Function

    'Remove ColonyAnt from persistent list of ColonyAnts
    Public Sub RemoveAntHill(pColonyAntHill As ColonyAntHill)
        For Each AntHill As ColonyAntHill In _ColonyAntHills
            If AntHill.Equals(pColonyAntHill)
                _ColonyAntHills.Remove(AntHill)
                Exit For 'We found it and removed it, so stop looking!
            End If
        Next
    End Sub

#End Region

    Public Function GetLocationAsString(pLocation As Location) As String
        Return "(" & pLocation.Row & "," & pLocation.Col & ")"
    End Function

    Public Function MazeDetection() As Boolean
        Dim CurrentTile As MapTile
        Dim WaterTileEdges As Integer = 0

        For Row As Integer = 0 to _Map._Rows-1
            For Col As Integer = 0 to _Map._Cols-1
                CurrentTile = _Map.GetMapTile(Row, Col)
                If Not CurrentTile.Passable
                    If CurrentTile.LinkedTiles(0).Passable = True Or _ 
                        CurrentTile.LinkedTiles(1).Passable = True Or _ 
                        CurrentTile.LinkedTiles(2).Passable = True Or _ 
                        CurrentTile.LinkedTiles(3).Passable = True Then

                        WaterTileEdges += 1 'We detected a water tile that has an edge to land, increment counter
                    End If
                        
                End If
            Next
        Next
        MsgBox("Water Edge Tiles: " & WaterTileEdges & ", MapSize: (" & _Map._Rows & "," & _Map._Cols & ")")
        Return False
    End Function


    #Region "Debugging Logic"
    Public Function xGetAnts() As List(Of ColonyAnt) Implements IColony.xGetAnts
        Return _ColonyAnts
    End Function
    #End Region

End Class
