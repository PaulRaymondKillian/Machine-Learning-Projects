Option Strict On

Imports System.Collections.Generic
Imports System.Collections.Hashtable
Imports System.Collections

Public Interface IColony
    Sub UpdateColony(pMapTiles(,) As Tile, pFoodLocations As List(Of Location), pAntLocations As List(Of Location), _
                            pMyAntHillLocation As List(Of Location), pEnemyAntHillLocation As List(Of Location))
    Function GetMapTile(pRow As Integer, pCol As Integer) As MapTile
    Function GetAntMoveDirection(pAntLocation As Location) As Direction
    Sub MoveAnt(pAntLocation As Location, pMoveDirection As Direction)
    Function AntExistAtLocation(pLocation As Location) As Boolean

    Function xGetAnts() As List(Of ColonyAnt)   'Debugging Logic Method
End Interface

Public Class Colony : Implements IColony
    Private _ColonyAnts As List(Of ColonyAnt)
    Public _Map As MapData
    Private _EnemyAntHillMemoryTTL As Hashtable
    'Public _EnemyAntHillTTL As List(Of EnemyAntHillTTL)
    Const MAXENEMYANTHILLMEMORYTTL As Integer = 10
    Const MAXMYANTTILESTRENGTH As Integer = 5

    Private _Random As New Random(Now.Millisecond)

    Public Sub New(pMapRows As Integer, pMapCols As Integer)
        _Map = New MapData(pMapRows, pMapCols)
        _ColonyAnts = New List(Of ColonyAnt)
        _EnemyAntHillMemoryTTL = New Hashtable
        '_EnemyAntHillTTL = New List(Of EnemyAntHillTTL)
    End Sub

    Public Sub UpdateColony(pMapTiles(,) As Tile, pFoodLocations As List(Of Location), pAntLocations As List(Of Location), _
                                                pMyAntHillLocation As List(Of Location), pEnemyAntHillLocation As List(Of Location)) Implements IColony.UpdateColony
        UpdateMapData(pMapTiles, pFoodLocations, pMyAntHillLocation, pEnemyAntHillLocation)
        UpdateColonyAnts(pAntLocations)
        UpdateLeafPingMapTiles()    'This must be done after UpdateColonyAnts and after UpdateMapData
    End Sub

    Private Sub UpdateMapData(pMapTiles(,) As Tile, pFoodLocations As List(Of Location), pMyAntHillLocation As List(Of Location), _
                                                                                        pEnemyAntHillLocation As List(Of Location))
        If pMyAntHillLocation.Count > 0  Then
            _Map.UpdateMyAntHillLocation(pMyAntHillLocation(0)) 'This must be done before UpdateMap
        End If
        If pFoodLocations.Count > 0 Then
            _Map.UpdateMapFoodItems(pFoodLocations)         'This must be done before UpdateMap
        End If
        _Map.UpdateMap(pMapTiles)                       'This must be done second (requires up to date list of FoodItems)
        
        
        Dim ActiveEnemyAntHillLocations As List(Of Location) = GetActiveEnemyAntHillLocations(pEnemyAntHillLocation)
        If ActiveEnemyAntHillLocations.Count > 0 Then
            _Map.UpdateMapEnemyAntHillLocation(ActiveEnemyAntHillLocations)
        End If
    End Sub

    Public Function GetMapTile(pRow As Integer, pCol As Integer) As MapTile Implements IColony.GetMapTile
        Return _Map._Map(pRow, pCol)
    End Function

    '
    '
    '

    #Region "Maintain Colony Ant List Logic"

    Private Sub UpdateColonyAnts(pAntLocations As List(Of Location))
        'Add new ants to the list from the current list of colony ants
        For Each AntLocation As Location In pAntLocations
            If Not AntExists(AntLocation)
                'AddFoodItem(_Map(FoodLocation.Row, FoodLocation.Col))

                'AddFoodItem(GetMapTile(FoodLocation.Row, FoodLocation.Col))
                AddNewAnt(AntLocation)
            End If
        Next

        'Remove ants from the colony ant list that are not in the newest ant list (therefore they no longer exist on the map)
        Dim AntsToBeRemoved As New List(Of ColonyAnt)
        For Each Ant As ColonyAnt In _ColonyAnts
            If Not AntLocationExistsInColonyAnts(Ant, pAntLocations)
                AntsToBeRemoved.Add(Ant)
                'RemoveFoodItem(FoodItem.MapTile)
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
                _ColonyAnts.Remove(Ant)
                Exit For 'We found it and removed it, so stop looking!
            End If
        Next
    End Sub

    #End Region
    '
    '
    '
                                 

    #Region "Colony Ant List Maintanence Logic"

    'Public Function AddNewAnt(pLocation As Location) As ColonyAnt
    '    Dim newColonyAnt As ColonyAnt = New ColonyAnt(pLocation)
    '    _ColonyAnts.Add(newColonyAnt)
    '    Return newColonyAnt
    'End Function
    Public Function AddNewAnt(pLocation As Location) As ColonyAnt
        Dim newColonyAnt As ColonyAnt = New ColonyAnt(GetMapTile(pLocation.Row, pLocation.Col), _Random)
        _ColonyAnts.Add(newColonyAnt)
        Return newColonyAnt
    End Function

    'Public Function GetAnt(pLocation As Location) As ColonyAnt
    '    Dim selectedAnt As New ColonyAnt(pLocation)
    '    Dim FoundAnt As Boolean = False
    '    If Not _ColonyAnts Is Nothing
    '        For Each ant As ColonyAnt In _ColonyAnts
    '            If pLocation.Col = ant.CurrentLocation.Col AndAlso pLocation.Row = ant.CurrentLocation.Row
    '                selectedAnt = ant
    '                FoundAnt = True
    '            End If
    '        Next
    '    End If
    '    'Ant must not have been in the list, add new ant.
    '    If FoundAnt = False
    '        selectedAnt = AddNewAnt(pLocation)    
    '    End If
    '    Return selectedAnt
    'End Function
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
    '        If pOldLoc.Col = ant.CurrentLocation.Col AndAlso pOldLoc.Row = ant.CurrentLocation.Row
    '            ant.CurrentLocation = pNewLoc
    '            ant.PreviousDirection = pDirection
    '        End If
    '    Next
    'End Sub
    Public Sub UpdateAnt(pOldLoc As Location, pNewLoc As Location, pDirection As Direction)
        For Each ant As ColonyAnt In _ColonyAnts
            If pOldLoc.Col = ant.CurrentMapTile.Col AndAlso pOldLoc.Row = ant.CurrentMapTile.Row
                ant.CurrentMapTile = GetMapTile(pNewLoc.Row, pNewLoc.Col)
                ant.PreviousDirection = pDirection
            End If
        Next
    End Sub

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

    Private Function GetActiveEnemyAntHillLocations(pEnemyAnyHillLocations As List(Of Location)) As List(Of Location)
        Dim ActiveEnemyAntHillLocations As New List(Of Location)

        'Reduce all TTL values currently in the Enemy Ant Hill Memory list by 1
        Dim AllActiveEnemyHillKeys As New List(Of String)
        For Each ActiveEnemyHillKey As String In _EnemyAntHillMemoryTTL.Keys
            '_EnemyAntHillMemoryTTL(ActiveEnemyHillKey) = CInt(_EnemyAntHillMemoryTTL(ActiveEnemyHillKey)) - 1 
            AllActiveEnemyHillKeys.Add(ActiveEnemyHillKey)
        Next
        For Each Key As String In AllActiveEnemyHillKeys
            _EnemyAntHillMemoryTTL(Key) = CInt(_EnemyAntHillMemoryTTL(Key)) - 1 
        Next
        
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
                '_EnemyAntHillMemoryTTL.Remove(ActiveEnemyHillKey)
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

    Public Sub UpdateLeafPingMapTiles()
        For Each Ant As ColonyAnt In _ColonyAnts
            Dim ListLeafPingTiles As New List(Of LeafPingTiles)
            Ant.CurrentMapTile.SetMyAntTileStrength(Ant.CurrentMapTile.GetMapTileAsString, MAXMYANTTILESTRENGTH, ListLeafPingTiles, 0, 0, 0, 0)
            Ant.SetLeafPingMaptiles(ListLeafPingTiles)
        Next
    End Sub

    Public Function GetLocationAsString(pLocation As Location) As String
        Return "(" & pLocation.Row & "," & pLocation.Col & ")"
    End Function


    #Region "Debugging Logic"
    Public Function xGetAnts() As List(Of ColonyAnt) Implements IColony.xGetAnts
        Return _ColonyAnts
    End Function
    #End Region

End Class
