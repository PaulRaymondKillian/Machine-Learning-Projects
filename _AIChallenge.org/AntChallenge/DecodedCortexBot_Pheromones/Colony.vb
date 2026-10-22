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
Imports System.Collections.Hashtable
Imports System.Collections



Public Interface IColony
    Sub UpdateColony(pMapTiles(,) As Tile, pFoodLocations As List(Of Location), pAntLocations As List(Of Location), _
                            pMyAntHillLocation As List(Of Location), pEnemyAntHillLocation As List(Of Location), _
                            pEnemyAntLocations As List(Of Location), pAntViewRadius As Integer, pAntCombatRadius As Integer)
    Function GetMapTile(pRow As Integer, pCol As Integer) As MapTile
    Function GetAntMoveDirection(pAntLocation As Location) As Direction
    Sub MoveAnt(pAntLocation As Location, pMoveDirection As Direction)
    Function AntExistAtLocation(pLocation As Location) As Boolean

    Function xGetAnts() As List(Of ColonyAnt)   'Debugging Logic Method
End Interface

Public Class Colony : Implements IColony
    Public _ColonyAnts As List(Of ColonyAnt)
    Private _ColonyAntCounter As Integer = 0
    Private _ColonyAntHills As List(Of ColonyAntHill)
    Public _Map As MapData
    Public _ColonyAntsCombatEngine As IColonyAntsCombatEngine
    Public _EnemyAntHillPredictionEngine As IColonyEnemyAntHillPredictionEngine
    Private _EnemyAntHillMemoryTTL As Hashtable
    Const MAXENEMYANTHILLMEMORYTTL As Integer = 20
    Private _ArchivedEnemyAntHills As List(Of Location)
    Private _DestroyedEnemyAntHills As List(Of Location)
    Private _TurnCount As Integer
    Private _TurnsSinceLastSeenEnemyAntHill As Integer
    Private _CountDefenderAnts As Integer
    Private _CountDefenderAntsQuota As Integer

    Private _Random As New Random(Now.Millisecond)

    Public Sub New(pMapRows As Integer, pMapCols As Integer)
        _Map = New MapData(pMapRows, pMapCols)
        _ColonyAnts = New List(Of ColonyAnt)
        _ColonyAntHills = New List(Of ColonyAntHill)
        _ColonyAntsCombatEngine = New ColonyAntsCombatEngine(_Random)
        _EnemyAntHillPredictionEngine = New ColonyEnemyAntHillPredictionEngine(_Random)
        _EnemyAntHillMemoryTTL = New Hashtable
        _ArchivedEnemyAntHills = New List(Of Location)
        _DestroyedEnemyAntHills = New List(Of Location)
        _TurnCount = 0
        _TurnsSinceLastSeenEnemyAntHill = 0
        _CountDefenderAntsQuota = 0
    End Sub


    Public Sub UpdateColony(pMapTiles(,) As Tile, pFoodLocations As List(Of Location), pAntLocations As List(Of Location), _
                                                pMyAntHillLocation As List(Of Location), pEnemyAntHillLocations As List(Of Location), _
                                                pEnemyAntLocations As List(Of Location), pAntViewRadius As Integer, pAttackRadius As Integer) Implements IColony.UpdateColony
        
        _TurnCount += 1
        UpdateColonyAnts(pAntLocations)
        UpdateColonyAntHills(pMyAntHillLocation)
        Dim DefenderAntPositions As List(Of MapTile) = GetDefenderAntPositions()
        MaintainEnemyAntHillArchive(pEnemyAntHillLocations)

        Dim EnemyAntHillLocations As List(Of Location)
        EnemyAntHillLocations = _EnemyAntHillPredictionEngine.ExecutePredictionEngine(_TurnCount, _ColonyAnts, _ColonyAntHills, _
                                                        pEnemyAntLocations, pEnemyAntHillLocations, _ArchivedEnemyAntHills, _ 
                                                        _DestroyedEnemyAntHills, _Map)

        Dim ActiveEnemyAntHillLocations As List(Of Location) = GetActiveEnemyAntHillLocations(EnemyAntHillLocations)

        'Dim ActiveEnemyAntHillLocations As List(Of Location) = GetActiveEnemyAntHillLocations(pEnemyAntHillLocations)
        MaintainDestroyedEnemyAntHillList(ActiveEnemyAntHillLocations, pAntLocations)

        'Dim sw As New System.IO.StreamWriter("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\TurnTimers_UpdateColony.txt",True)
        'Dim Timer As New System.Diagnostics.Stopwatch
        'Timer.Start
        UpdateMapData(pMapTiles, pFoodLocations, pMyAntHillLocation, ActiveEnemyAntHillLocations, _ColonyAnts, pEnemyAntLocations, pAntViewRadius, pAttackRadius, DefenderAntPositions)
        'Timer.Stop
        'sw.WriteLine(Timer.ElapsedMilliseconds)
        'sw.Close

        'Combat engine gives ants orders pertaining to what they should do if they are in combat, these orders take precendent over all
        _ColonyAntsCombatEngine.ExecuteColonyAntsCombatEngine(_TurnCount, _ColonyAnts, _ColonyAntHills, _
                                                        pEnemyAntLocations, pEnemyAntHillLocations, _ArchivedEnemyAntHills, _ 
                                                        _DestroyedEnemyAntHills, _Map, pAttackRadius)

        ResetAllFoodAssignments
        AssignMapFoodItems()    'This occurs after the food item 'strengths' have been added to the map

        For Each AntHill As ColonyAntHill In _ColonyAntHills
            AntHill.CalculateLookoutTowerLocations(5)
            AntHill.CheckLookoutTowers
        Next

    End Sub

    Private Function GetDefenderAntPositions() As List(Of MapTile)
        Dim DefenderTiles As New List(Of MapTile)
        Dim CurrentTile As MapTile
        'We will calculate 2 defender ant positions per ant hill, trying to be in opposite directions
        For Each AntHill As ColonyAntHill In _ColonyAntHills
            For i As Integer = 0 to 3
                CurrentTile = AntHill._AntHill
                For j As Integer = 0 to 2
                    CurrentTile = CurrentTile.LinkedTiles(i)
                Next
                If CurrentTile.Passable
                    DefenderTiles.Add(CurrentTile)
                    For Each AdjTile As MapTile In CurrentTile.LinkedTiles
                        If AdjTile.Passable
                            DefenderTiles.Add(AdjTile)
                        End If
                    Next
                End If
            Next
        Next

        If DefenderTiles.Count > 2 Then
            Dim RandIndex1 As Integer = _Random.Next Mod DefenderTiles.Count
            Dim RandIndex2 As Integer = CInt((RandIndex1 + (DefenderTiles.Count/2)) Mod DefenderTiles.Count)'_Random.Next Mod DefenderTiles.Count
            Dim TempDefenderList As New List(Of MapTile)
            TempDefenderList.Add(DefenderTiles(RandIndex1))
            TempDefenderList.Add(DefenderTiles(RandIndex2))
            DefenderTiles = TempDefenderList
        End If
        
        If DefenderTiles.Count = 0
            'MsgBox("No Defender Tiles")
        End If

        Return DefenderTiles
    End Function

    Public Sub ResetAllFoodAssignments
        For Each Ant As ColonyAnt In _ColonyAnts
            Ant.AssignedFoodItemID = ""
        Next
        For i As Integer = 0 to _Map._FoodItems.Count-1
            _Map._FoodItems(i).ResetDispatched
        Next
    End Sub

    Public Function GetExploreBearingDegree() As Double
        Dim ExploreBearingDegree As Double

        'Perform special heuristic rules for when there are less than 4 ants
        If _ColonyAnts.Count = 1
            ExploreBearingDegree = (_ColonyAnts(0).ExploreBearingDegree + 180) Mod 360
        Else If _ColonyAnts.Count = 2
            ExploreBearingDegree = (_ColonyAnts(0).ExploreBearingDegree + _ColonyAnts(1).ExploreBearingDegree + 90) Mod 360
        ElseIf _ColonyAnts.Count = 3
            ExploreBearingDegree = (_ColonyAnts(0).ExploreBearingDegree + _ColonyAnts(1).ExploreBearingDegree + 270) Mod 360
        Else
            ExploreBearingDegree = ((_ColonyAntCounter-1) * 150) Mod 360
        End If

        'MsgBox(ExploreBearingDegree)
        Return ExploreBearingDegree
    End Function


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
                                    pEnemyAntHillLocation As List(Of Location), pColonyAnts As List(Of ColonyAnt), _ 
                                    pEnemyAntLocations As List(Of Location), pAntViewRadius As Integer, pAntCombatRadius As Integer, _ 
                                    pDefenderAntPositions As List(Of MapTile))

        _Map.UpdateMapPathfinderVariables(pMapTiles.Length, pFoodLocations.Count, pEnemyAntHillLocation.Count, pAntViewRadius, pAntCombatRadius, pColonyAnts.Count, pEnemyAntLocations.Count)

        _Map.UpdateMapFoodItems(pFoodLocations)         'This must be done before UpdateMap

        _Map.UpdateMap(pMapTiles, pMyAntHillLocations.Count, pColonyAnts.Count, pEnemyAntLocations.Count, pEnemyAntHillLocation.Count)                       'This must be done second (requires up to date list of FoodItems)
        
        _Map.UpdateMapEnemyAntHillLocation(pEnemyAntHillLocation)

        _Map.UpdateMyAntHillsMap(pMyAntHillLocations)

        _Map.UpdateMyAntPheromoneMap(pColonyAnts) 'This needs to be done after the UpdateMap function so we have an up-to-date map and ensure Pheromones decreased

        _Map.UpdateEnemyAntPheromoneMap(pEnemyAntLocations) 'This needs to be done after the UpdateMap function so we have an up-to-date map

        _Map.UpdateDefenderAntPositions(pDefenderAntPositions)

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
        Dim HighestStrengthValue, CurrentStrengthValue As Integer
        Dim HighestAnt As ColonyAnt
        Dim HighestFoodIndex As Integer 
        Dim DoneAssigning As Boolean = False
        'Need to assign the closest colony ant and food item pairings so that the ants closest to a food item always goes towards it

        'Go through all colony ants and look at all food strength values. Find the highest food item strength value for an ant that
        'has not already been assigned a food item, and for food items that have not already had an ant dispatched for it.
        Do
            HighestStrengthValue = Integer.MinValue
            HighestAnt = Nothing
            HighestFoodIndex = -1
            For Each Ant As ColonyAnt In _ColonyAnts
                If Ant.AssignedFoodItemID = ""  'If not assigned a food item
                    'For Each Food As FoodItem In _Map._FoodItems
                    For i As Integer = 0 to _Map._FoodItems.Count-1
                        If Not _Map._FoodItems(i).AntDispatched   'If this food item has not been assigned to an ant
                            CurrentStrengthValue = CInt(Ant.CurrentMapTile.FoodStrengthHash(_Map._FoodItems(i).MapTile.ID)) 
                            If CurrentStrengthValue > HighestStrengthValue AndAlso CurrentStrengthValue <> 0
                                HighestStrengthValue = CurrentStrengthValue
                                HighestAnt = Ant
                                HighestFoodIndex = i
                            End If
                        End If
                    Next
                End If
            Next
            If Not HighestAnt Is Nothing
                'We found the current highest ant and food item pairing. Lets assign them to each other.
                HighestAnt.AssignedFoodItemID = _Map._FoodItems(HighestFoodIndex).MapTile.ID
                _Map._FoodItems(HighestFoodIndex).Dispatched
            Else
                DoneAssigning = True
            End If
        Loop Until DoneAssigning
    End Sub


    Public Sub AssignMapFoodItems_OLD()
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

        'Now actually remove the ant
        For Each Ant As ColonyAnt In AntsToBeRemoved
            RemoveAnt(Ant)
        Next

        'Update defender ant count
        _CountDefenderAnts = 0
        For Each Ant As ColonyAnt In _ColonyAnts
            If Ant.Mission = ColonyAntMission.AntHillDefender
                If Ant.CurrentMapTile.DefenderAntPositionStrength = 0 Or _CountDefenderAnts >= _CountDefenderAntsQuota
                    'If the ant is no longer on a defender tile, switch it to an explorer
                    'MsgBox("Ant Switched from Defender to Explorer")
                    Ant.Mission = ColonyAntMission.Explorer
                End If
                _CountDefenderAnts += 1
            End If
        Next
        
        'Calculate the current Ant Defender Quota
        If _ColonyAnts.Count > (_ColonyAntHills.Count * 4) AndAlso _ColonyAnts.Count <= 50
            _CountDefenderAntsQuota = _ColonyAntHills.Count
        ElseIf _ColonyAnts.Count > 50 AndAlso _ColonyAnts.Count <= 100
            _CountDefenderAntsQuota = 2 * _ColonyAntHills.Count
        ElseIf _ColonyAnts.Count > 100
            _CountDefenderAntsQuota = 3 * _ColonyAntHills.Count
        Else
            _CountDefenderAntsQuota = 0
        End If
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
                _ColonyAnts.Remove(Ant)
                Exit For 'We found it and removed it, so stop looking!
            End If
        Next
    End Sub

#End Region                               

#Region "Colony Ant List Maintanence Logic"

    Public Function AddNewAnt(pLocation As Location) As ColonyAnt
        _ColonyAntCounter += 1  'Increment the ant counter so each ant has its own ID
        Dim newColonyAnt As ColonyAnt = New ColonyAnt(GetNewAntMission, GetMapTile(pLocation.Row, pLocation.Col), _Random, CStr(_ColonyAntCounter), GetExploreBearingDegree(), _Map)
        _ColonyAnts.Add(newColonyAnt)
        Return newColonyAnt
    End Function

    Public Function GetNewAntMission() As ColonyAntMission
        Dim Mission As ColonyAntMission = ColonyAntMission.Explorer

        If _ColonyAntHills.Count > 0
            If _CountDefenderAnts < _CountDefenderAntsQuota
                Mission = ColonyAntMission.AntHillDefender
            End If
        End If

        Return Mission
    End Function

    Public Function GetAnt(pLocation As Location) As ColonyAnt
        Dim selectedAnt As New ColonyAnt(ColonyAntMission.AntHillDefender, GetMapTile(pLocation.Row, pLocation.Col), _Random, "N/A", -1, _Map)
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

#End Region

#Region "Ant Orders Logic"

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


    #Region "Debugging Logic"
    Public Function xGetAnts() As List(Of ColonyAnt) Implements IColony.xGetAnts
        Return _ColonyAnts
    End Function
    #End Region

End Class
