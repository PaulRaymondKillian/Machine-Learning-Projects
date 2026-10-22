Option Strict On

Imports System.Collections.Generic
Imports System.Collections.Hashtable
Imports System.Collections

Public Partial Class ColonyAntMovementController

#Region "Alert"

    Private Function GoTowardAlert As Direction Implements IColonyAntMovementController.GoTowardAlert
        Dim MoveDirection As Direction

        'Private Function ChooseHighestNonOccupiedAdjacentStrengthDirection(pCurrentTileStrength As Integer, pStrengths() As Integer, _ 
        '                                                                                        pMapTiles() As MapTile) As Direction
        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CurrentMapTile.LinkedTiles(i).AlertStrength
        Next
        MoveDirection = ChooseHighestNonOccupiedAdjacentStrengthDirection(CurrentMapTile.AlertStrength, LinkedStrengths, CurrentMapTile.LinkedTiles)


        Return MoveDirection
    End Function

    'Private Function ChooseHighestNonOccupiedAdjacentAlertStrengthDirection() As Direction Implements IColonyAntMovementController.GoTowardAlert
    '    Dim MoveDirection As Direction
    '    'Determine highest food strength value for inputted food item ID of all adjacent tiles
    '    Dim HighestAlertStrengthMapTile As MapTile = CurrentMapTile.LinkedTiles(0) 'Set default value to prevent null ref
    '    Dim HighestAlertStrengthValue As Integer = Integer.MinValue
    '    For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
    '        If MapTile.AlertStrength >= HighestAlertStrengthValue AndAlso MapTile.AntHere = False Then
    '            HighestAlertStrengthValue = MapTile.AlertStrength
    '            HighestAlertStrengthMapTile = MapTile
    '        End If
    '    Next

    '    'Determine what direction we must move to go to this tile
    '    For i As Integer = 0 To CurrentMapTile.LinkedTiles.Length - 1
    '        If CurrentMapTile.LinkedTiles(i).Equals(HighestAlertStrengthMapTile) Then
    '            MoveDirection = CType(i, Direction)
    '            Exit For
    '        End If
    '    Next
    '    Return MoveDirection
    'End Function

#End Region

#Region "Food"

    Public Function GoTowardAssignedFood() As Direction Implements IColonyAntMovementController.GoTowardAssignedFood
        Dim MoveDirection As Direction
        Dim AssignedFoodID As String = _ThisColonyAnt.AssignedFoodItemID
        
        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CInt(CurrentMapTile.LinkedTiles(i).FoodStrengthHash(AssignedFoodID))
        Next
        MoveDirection = ChooseHighestNonOccupiedAdjacentStrengthDirection(CInt(CurrentMapTile.FoodStrengthHash(AssignedFoodID)), LinkedStrengths, CurrentMapTile.LinkedTiles)

        Return MoveDirection
    End Function

    Public Function GoTowardClosestFood() As Direction Implements IColonyAntMovementController.GoTowardClosestFood
        Dim MoveDirection As Direction
        Dim HighestFoodID As String
        HighestFoodID = GetHighestValuedHashKey(CurrentMapTile.FoodStrengthHash)
        
        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CInt(CurrentMapTile.LinkedTiles(i).FoodStrengthHash(HighestFoodID))
        Next
        MoveDirection = ChooseHighestNonOccupiedAdjacentStrengthDirection(CInt(CurrentMapTile.FoodStrengthHash(HighestFoodID)), LinkedStrengths, CurrentMapTile.LinkedTiles)

        Return MoveDirection
    End Function

    Private Function ChooseHighestAdjacentFoodItemStrengthDirection(pHighestStrengthID As String) As Direction
        Dim MoveDirection As Direction
        Dim MoveChosen As Boolean = False
        Dim CurrentHighestStrengthValue As Integer = Integer.MinValue
        Dim LowThresholdStrengthValue As Integer = CInt(CurrentMapTile.FoodStrengthHash(pHighestStrengthID))
        Dim HighThresholdStrengthValue As Integer = Integer.MaxValue
        Dim AvailableTiles As New List(Of Integer)
        Dim CurrentValue As Integer

        Do
            AvailableTiles.Clear    'Reset this to ensure we always have an empty list to start with
            'Determine highest food strength value for inputted food item ID of all adjacent tiles
            For i As Integer = 0 to CurrentMapTile.LinkedTiles.Length-1
                CurrentValue = CInt(CurrentMapTile.LinkedTiles(i).FoodStrengthHash(pHighestStrengthID))
                If CurrentValue >= CurrentHighestStrengthValue AndAlso CurrentValue > LowThresholdStrengthValue  _
                                                                AndAlso CurrentValue < HighThresholdStrengthValue
                    If CurrentHighestStrengthValue < CurrentValue
                        AvailableTiles.Clear    'If we found a new highest value, clear all the old tiles and start fresh for new high
                    End If
                    AvailableTiles.Add(i)
                    CurrentHighestStrengthValue = CurrentValue
                End If
            Next

            If AvailableTiles.Count > 0
                For Each Index As Integer In AvailableTiles
                    If CurrentMapTile.LinkedTiles(Index).AntHere = False
                        MoveDirection = CType(Index, Direction)
                        MoveChosen = True
                        Exit For
                    End If
                Next
            Else
                'If no tiles are available to be chosen, then there does not exist a good move that can be moved to. 
                ''This means that all moves are either worse strength values, an ant exists there, or is not passable.
                ''In this scenario, we do not want to move at all.
                
                'We do not want this ant to actually move, so we can force this ant to not move by choosing a direction the
                'ant cannot actually move to, because our logic will later block it. This prevents us from needing to create
                'A new MoveDirection enum that includes a 'NoMove' parameter; it also prevents us from neeing to refactor the code.
                
                'For an adjacent tile this ant cannot actually move to, and choose it, causing our ant to stay where it is.
                For i As Integer = 0 to CurrentMapTile.LinkedTiles.Length-1
                    If CurrentMapTile.LinkedTiles(i).Passable = False or CurrentMapTile.LinkedTiles(i).AntHere = True
                        MoveDirection = CType(i, Direction)
                        MoveChosen = True
                        Exit For
                    End If
                Next
                
                'Verify a move was chosen, if not, we have a problem...
                If Not MoveChosen
                    'MsgBox("AHHHHH!!! - No move was chosen, and it should have been by now...Food")
                End If
            End If

            'Check if a move was chosen, if not, reset parameters to look for next highest adjacent strength value
            If Not MoveChosen
                HighThresholdStrengthValue = CurrentHighestStrengthValue
                CurrentHighestStrengthValue = Integer.MinValue
            End If
        Loop Until MoveChosen
        Return MoveDirection
    End Function

#End Region

#Region "My Ant Hill"

    Private Function GoTowardMyAntHill As Direction Implements IColonyAntMovementController.GoTowardMyAntHill
        Dim MoveDirection As Direction

        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CurrentMapTile.LinkedTiles(i).MyAntHillStrength
        Next
        MoveDirection = ChooseHighestNonOccupiedAdjacentStrengthDirection(CurrentMapTile.MyAntHillStrength, LinkedStrengths, CurrentMapTile.LinkedTiles)


        Return MoveDirection
    End Function

#End Region

#Region "Enemy Ant Hill"

    Public Function GoTowardClosestEnemyAntHill() As Direction Implements IColonyAntMovementController.GoTowardClosestEnemyAntHill
        Dim MoveDirection As Direction
        Dim HighestEnemyAntHillID As String

        HighestEnemyAntHillID = GetHighestValuedHashKey(CurrentMapTile.EnemyAntHillStrengthHash)

        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CInt(CurrentMapTile.LinkedTiles(i).EnemyAntHillStrengthHash(HighestEnemyAntHillID))
        Next
        MoveDirection = ChooseHighestNonOccupiedAdjacentStrengthDirection(CInt(CurrentMapTile.EnemyAntHillStrengthHash(HighestEnemyAntHillID)), LinkedStrengths, CurrentMapTile.LinkedTiles)

        Return MoveDirection
    End Function

#End Region

#Region "Closest Enemy Ant - EMPTY"

#End Region

#Region "Enemy Ants Pheromone - Towards"

    Public Function GoTowardClosestEnemyAnt() As Direction Implements IColonyAntMovementController.GoTowardClosestEnemyAnt
        Dim MoveDirection As Direction
        Dim HighestEnemyAntID As String

        HighestEnemyAntID = GetHighestValuedHashKey(CurrentMapTile.EnemyAntPheromoneStrengthHash)

        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CInt(CurrentMapTile.LinkedTiles(i).EnemyAntPheromoneStrengthHash(HighestEnemyAntID))
        Next
        MoveDirection = ChooseHighestNonOccupiedAdjacentStrengthDirection(CInt(CurrentMapTile.EnemyAntPheromoneStrengthHash(HighestEnemyAntID)), LinkedStrengths, CurrentMapTile.LinkedTiles)

        Return MoveDirection
    End Function

#End Region

#Region "Enemy Ants Pheromone - Away - EMPTY"

#End Region

#Region "My Ant Immediate Pheromone - Towards"

    Public Function GoTowardClosestMyAnt() As Direction Implements IColonyAntMovementController.GoTowardMyAntImmediatePheromone
        Dim MoveDirection As Direction
        Dim HighestMyAntID As String

        HighestMyAntID = GetHighestValuedHashKey(CurrentMapTile.MyAntImmediatePheromoneStrengthHash, CurrentMapTile.ID)

        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CInt(CurrentMapTile.LinkedTiles(i).MyAntImmediatePheromoneStrengthHash(HighestMyAntID))
        Next
        MoveDirection = ChooseHighestNonOccupiedAdjacentStrengthDirection(CInt(CurrentMapTile.MyAntImmediatePheromoneStrengthHash(HighestMyAntID)), LinkedStrengths, CurrentMapTile.LinkedTiles)

        Return MoveDirection
    End Function

#End Region

#Region "My Ant Immediate Pheromone - Away - EMTPY"

    'Public Function GoAwayFromMyAntImmediatePheromone() As Direction Implements IColonyAntMovementController.GoAwayFromMyAntImmediatePheromone
    '    Dim MoveDirection As Direction
    '    Dim HighestMyAntImmediatePheromoneValue As Integer
        
    '    HighestMyAntImmediatePheromoneValue = GetLowestAdjacentMyAntImmediatePheromoneStrengthValue
    '    MoveDirection = ChooseLowestNonOccupiedAdjacentMyAntImmediatePheromoneStrengthDirection(HighestMyAntImmediatePheromoneValue)
        
    '    Return MoveDirection
    'End Function

    'Private Function ChooseLowestAdjacentMyAntImmediatePheromoneStrengthDirection(pLowestStrengthValue As Integer) As Direction
    '    Dim MoveDirection As Direction
    '    If pLowestStrengthValue = -1
    '        'There is no valid move... set move direction to north just because... prevents null ref 
    '        MoveDirection = Direction.n
    '    Else
    
    '        'Create list of all map tiles with the lowest MyAntHill Strength (may be multiple tiles)
    '        Dim LowestMapTiles As New List(Of MapTile)

    '        Try
    '            For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
    '                If MapTile.GetMyAntImmediatePheromoneStrength = pLowestStrengthValue Then
    '                    LowestMapTiles.Add(MapTile)
    '                End If
    '            Next

    '        Catch ex As Exception
    '            Throw New Exception("Error in 1 - " & ex.Message)
    '        End Try

    '        'Now we have a list of lowest adjacent map tiles. Choose one randomly.
    '        Dim ChosenAdjacentMapTile As MapTile
    '        Try

    '            If LowestMapTiles.Count > 0
    '                ChosenAdjacentMapTile = LowestMapTiles(_Random.Next Mod LowestMapTiles.Count)
    '            Else
    '                Throw New Exception("Lowest Map Tiles Count = 0!!!!, " & _ 
    '                                    "LowestStrengthValue=" & pLowestStrengthValue & ", " & _
    '                                    "CountLinkedTiles=" & CurrentMapTile.LinkedTiles.Length & ", " & _
    '                                    "0= " & CurrentMapTile.LinkedTiles(0).GetMyAntImmediatePheromoneStrength & ", " & _
    '                                    "1= " & CurrentMapTile.LinkedTiles(1).GetMyAntImmediatePheromoneStrength & ", " & _
    '                                    "2= " & CurrentMapTile.LinkedTiles(2).GetMyAntImmediatePheromoneStrength & ", " & _
    '                                    "3= " & CurrentMapTile.LinkedTiles(3).GetMyAntImmediatePheromoneStrength & ", ")
    '            End If
    '        Catch ex As Exception
    '            Throw New Exception("Error in ChooseLowestAdjacentMyAntPheromoneStrengthDirection - " & ex.Message)
    '        End Try

    '        Try

    '            'Determine what direction we must move to go to this tile
    '            For i As Integer = 0 To CurrentMapTile.LinkedTiles.Length - 1
    '                If CurrentMapTile.LinkedTiles(i).Equals(ChosenAdjacentMapTile) Then
    '                    MoveDirection = CType(i, Direction)
    '                    Exit For
    '                End If
    '            Next
    '        Catch ex As Exception
    '            Throw New Exception("Error in 3 - " & ex.Message)
    '        End Try
    '    End If
    '    Return MoveDirection
    'End Function

    'Private Function ChooseLowestNonOccupiedAdjacentMyAntImmediatePheromoneStrengthDirection(pLowestStrengthValue As Integer) As Direction
    '    Dim MoveDirection As Direction
    '    If pLowestStrengthValue = -1
    '        'There is no valid move... set move direction to north just because... prevents null ref 
    '        MoveDirection = Direction.n
    '    Else
    
    '        'Create list of all map tiles with the lowest MyAntHill Strength (may be multiple tiles)
    '        Dim LowestMapTiles As New List(Of MapTile)

    '        Try
    '            For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
    '                If MapTile.GetMyAntImmediatePheromoneStrength = pLowestStrengthValue AndAlso MapTile.AntHere = False Then
    '                    LowestMapTiles.Add(MapTile)
    '                End If
    '            Next

    '        Catch ex As Exception
    '            Throw New Exception("Error in 1 - " & ex.Message)
    '        End Try

    '        'Now we have a list of lowest adjacent map tiles. Choose one randomly.
    '        Dim ChosenAdjacentMapTile As MapTile
    '        Try

    '            If LowestMapTiles.Count > 0
    '                ChosenAdjacentMapTile = LowestMapTiles(_Random.Next Mod LowestMapTiles.Count)
    '            Else
    '                Throw New Exception("Lowest Map Tiles Count = 0!!!!, " & _ 
    '                                    "LowestStrengthValue=" & pLowestStrengthValue & ", " & _
    '                                    "CountLinkedTiles=" & CurrentMapTile.LinkedTiles.Length & ", " & _
    '                                    "0= " & CurrentMapTile.LinkedTiles(0).GetMyAntImmediatePheromoneStrength & ", " & _
    '                                    "1= " & CurrentMapTile.LinkedTiles(1).GetMyAntImmediatePheromoneStrength & ", " & _
    '                                    "2= " & CurrentMapTile.LinkedTiles(2).GetMyAntImmediatePheromoneStrength & ", " & _
    '                                    "3= " & CurrentMapTile.LinkedTiles(3).GetMyAntImmediatePheromoneStrength & ", ")
    '            End If
    '        Catch ex As Exception
    '            Throw New Exception("Error in ChooseLowestAdjacentMyAntPheromoneStrengthDirection - " & ex.Message)
    '        End Try

    '        Try

    '            'Determine what direction we must move to go to this tile
    '            For i As Integer = 0 To CurrentMapTile.LinkedTiles.Length - 1
    '                If CurrentMapTile.LinkedTiles(i).Equals(ChosenAdjacentMapTile) Then
    '                    MoveDirection = CType(i, Direction)
    '                    Exit For
    '                End If
    '            Next
    '        Catch ex As Exception
    '            Throw New Exception("Error in 3 - " & ex.Message)
    '        End Try
    '    End If
    '    Return MoveDirection
    'End Function

    'Private Function GetLowestAdjacentMyAntImmediatePheromoneStrengthValue() As Integer
    '    'Find the lowest MyAntHill Strength of all adjacent tiles
    '    Dim LowestStrengthValue As Integer = Integer.MaxValue
    '    For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
    '        If MapTile.GetMyAntImmediatePheromoneStrength < LowestStrengthValue And MapTile.Passable And MapTile.AntHere = False Then
    '            LowestStrengthValue = MapTile.GetMyAntImmediatePheromoneStrength
    '        End If
    '    Next


    '    If LowestStrengthValue = Integer.MaxValue
    '        LowestStrengthValue = -1
    '        'Throw New Exception("LowestStrengthValue=" & LowestStrengthValue & ", " & _ 
    '        '                    "Linked(0)=" & CurrentMapTile.LinkedTiles(0).GetMyAntHillStrength & "-" & CurrentMapTile.LinkedTiles(0).Passable & ", " & _
    '        '                    "Linked(1)=" & CurrentMapTile.LinkedTiles(1).GetMyAntHillStrength & "-" & CurrentMapTile.LinkedTiles(1).Passable & ", " & _
    '        '                    "Linked(2)=" & CurrentMapTile.LinkedTiles(2).GetMyAntHillStrength & "-" & CurrentMapTile.LinkedTiles(2).Passable & ", " & _
    '        '                    "Linked(3)=" & CurrentMapTile.LinkedTiles(3).GetMyAntHillStrength & "-" & CurrentMapTile.LinkedTiles(3).Passable & ", ")
    '    End If



    '    Return LowestStrengthValue
    'End Function

#End Region

#Region "My Ant Persistent - Towards"

    Private Function ChooseHighestNonOccupiedAdjacentMyAntPersistentPheromoneStrengthDirection() As Direction Implements IColonyAntMovementController.GoTowardMyAntPersistentPheromone
        Dim MoveDirection As Direction

        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CurrentMapTile.LinkedTiles(i).MyAntPersistentPheromoneStrength
        Next
        MoveDirection = ChooseHighestNonOccupiedAdjacentStrengthDirection(CurrentMapTile.MyAntPersistentPheromoneStrength, LinkedStrengths, CurrentMapTile.LinkedTiles)

        Return MoveDirection
    End Function

#End Region

#Region "My Ant Persistent - Away"

    Private Function GoAwayFromMyAntPersistentPheromone() As Direction Implements IColonyAntMovementController.GoAwayFromMyAntPersistentPheromone
        Dim MoveDirection As Direction

        'Create array of Hashtables for the ChooseHighestAdjacentStrengthDirection method
        Dim LinkedStrengths(CurrentMapTile.LinkedTiles.Length-1) As Integer
        For i As Integer = 0 to LinkedStrengths.Length-1
            LinkedStrengths(i) = CurrentMapTile.LinkedTiles(i).MyAntPersistentPheromoneStrength
        Next
        MoveDirection = ChooseLowestNonOccupiedAdjacentStrengthDirection(CurrentMapTile.MyAntPersistentPheromoneStrength, LinkedStrengths, CurrentMapTile.LinkedTiles)

        Return MoveDirection
    End Function

#End Region

#Region "Random Walk"

    Private Function GetRandomWalkMoveDirection() As Direction
        Dim MoveDirection As Direction = Direction.w 'Set a default move direction....

        'Walk in a straight line until either the forward unit is not-passable, or we have walked in a straight line enough times
        '' to warrant turning in a random direction 90 degrees (180 degrees if we have to).

        If _StepCount < STEPCOUNTTHRESHOLD And CheckIfPassable(_PreviousDirection) And CurrentMapTile.LinkedTiles(_PreviousDirection).AntHere = False Then
            MoveDirection = _PreviousDirection   'Keep moving in a straight line, so move forward in previous direction
        Else 'Cannot keep going straight, need to turn left or right or turn around.

            'Perform a Mod 2 over a random number to get a 50-50 of a 0 or a 1 returned to represent a Boolean
            Dim ClockwiseTurn As Boolean = Convert.ToBoolean(_Random.Next Mod 2)
            MoveDirection = GetTurn90DegreeDirection(_PreviousDirection, ClockwiseTurn)

            'Verify new direction is passable, If not, try other direction
            If Not CheckIfPassable(MoveDirection) Or CurrentMapTile.LinkedTiles(MoveDirection).AntHere = True Then
                MoveDirection = GetTurn90DegreeDirection(_PreviousDirection, Not ClockwiseTurn)

                'Now, Verify is the New New direction is passable, if it isnt, then turn around from Previous direction
                If Not CheckIfPassable(MoveDirection)  Or CurrentMapTile.LinkedTiles(MoveDirection).AntHere = True Then
                    MoveDirection = GetTurn180DegreeDirection(_PreviousDirection)
                End If
            End If

        End If

        Return MoveDirection
    End Function

#End Region

#Region "Go Explore"

    Public Function GoExplore() As Direction Implements IColonyAntMovementController.GoExplore
        Dim MoveDirection As Direction

        MoveDirection = GoAwayFromMyAntPersistentPheromone()
        If CurrentMapTile.LinkedTiles(CInt(MoveDirection)).MyAntPersistentPheromoneStrength = CurrentMapTile.MyAntPersistentPheromoneStrength
            MoveDirection = GetRandomWalkMoveDirection
        End If

        Return MoveDirection
    End Function

#End Region

#Region "Go Combat"
    Public Function GoCombat() As Direction Implements IColonyAntMovementController.GoCombat
        Dim MoveDirection As Direction
        If CountEnemyAntsNearby <> 0 Then
            If CountEnemyAntsNearby <= CountMyAntsNearby(0.6) Then
                MoveDirection = GoTowardClosestEnemyAnt()
            Else
                If CountMyAntsNearby(0.50) > 0
                    MoveDirection = GoTowardClosestMyAnt
                Else
                    MoveDirection = ChooseHighestNonOccupiedAdjacentMyAntPersistentPheromoneStrengthDirection()
                End If
            End If
        Else
            If CountMyAntsNearby > 0
                    MoveDirection = GoTowardClosestMyAnt
                Else
                    MoveDirection = ChooseHighestNonOccupiedAdjacentMyAntPersistentPheromoneStrengthDirection()
                End If
        End If
        Return MoveDirection
    End Function
#End Region

#Region "Go Defend Ant Hill"
    Public Function GoDefendAntHill() As Direction Implements IColonyAntMovementController.GoDefendAntHill
        Dim MoveDirection As Direction

        'For now our Defend Ant Hill logic will just tell ants to go towards the ant hill
        MoveDirection = GoTowardMyAntHill

        Return MoveDirection
    End Function
#End Region

#Region "Misc Movement Helper Logic"

    Private Function ChooseLowestNonOccupiedAdjacentStrengthDirection(pCurrentTileStrength As Integer, pStrengths() As Integer, _ 
                                                                                                pMapTiles() As MapTile) As Direction
        Dim MoveDirection As Direction
        Dim MoveChosen As Boolean = False
        Dim CurrentLowestStrengthValue As Integer = Integer.MaxValue
        Dim LowThresholdStrengthValue As Integer = Integer.MinValue
        Dim AvailableTilesIndices As New List(Of Integer)
        Dim CurrentValue As Integer
        Dim CurrentTile As MapTile

        Do
            AvailableTilesIndices.Clear    'Reset this to ensure we always have an empty list to start with
            'Determine highest food strength value for inputted food item ID of all adjacent tiles
            For i As Integer = 0 to pStrengths.Length-1
                CurrentValue = pStrengths(i)
                CurrentTile = pMapTiles(i)
                If CurrentValue <= CurrentLowestStrengthValue AndAlso CurrentValue > LowThresholdStrengthValue  _
                                    AndAlso CurrentTile.Passable 'AndAlso CurrentValue < HighThresholdStrengthValue 'XXX REMOVED THIS PART FOR NOW
                    If CurrentLowestStrengthValue > CurrentValue
                        AvailableTilesIndices.Clear    'If we found a new lowest value, clear all the old tiles and start fresh for new high
                    End If
                    AvailableTilesIndices.Add(i)
                    CurrentLowestStrengthValue = CurrentValue
                End If
            Next

            If AvailableTilesIndices.Count > 0
                For Each Index As Integer In AvailableTilesIndices
                    If pMaptiles(Index).AntHere = False
                        MoveDirection = CType(Index, Direction)
                        MoveChosen = True
                        Exit For
                    End If
                Next
            Else
                'If no tiles are available to be chosen, then there does not exist a good move that can be moved to. 
                ''This means that all moves are either worse strength values, an ant exists there, or is not passable.
                ''In this scenario, we do not want to move at all.
                
                'We do not want this ant to actually move, so we can force this ant to not move by choosing a direction the
                'ant cannot actually move to, because our logic will later block it. This prevents us from needing to create
                'A new MoveDirection enum that includes a 'NoMove' parameter; it also prevents us from neeing to refactor the code.
                
                'For an adjacent tile this ant cannot actually move to, and choose it, causing our ant to stay where it is.
                For i As Integer = 0 to pMaptiles.Length-1
                    If pMaptiles(i).Passable = False or pMaptiles(i).AntHere = True
                        MoveDirection = CType(i, Direction)
                        MoveChosen = True
                        Exit For
                    End If
                Next
                
                'Verify a move was chosen, if not, we have a problem...
                If Not MoveChosen
                    'MsgBox("AHHHHH!!! - No move was chosen, and it should have been by now...")
                    'If a move was not chosen yet, this means that likely the ant has reached the tile with the highest strength
                    'value and therefore cannot move to any higher of a strength value, and none of the adjacent tiles contain ants 
                    'or water tiles. Therefore, in this case, lets just move to a default direction for the heck of it...
                    MoveDirection = Direction.n
                    MoveChosen = True
                End If
            End If

            'Check if a move was chosen, if not, reset parameters to look for next highest adjacent strength value
            If Not MoveChosen
                LowThresholdStrengthValue = CurrentLowestStrengthValue
                CurrentLowestStrengthValue = Integer.MaxValue
            End If
        Loop Until MoveChosen

        Return MoveDirection
    End Function


    Private Function ChooseHighestNonOccupiedAdjacentStrengthDirection(pCurrentTileStrength As Integer, pStrengths() As Integer, _ 
                                                                                                pMapTiles() As MapTile) As Direction
        Dim MoveDirection As Direction
        Dim MoveChosen As Boolean = False
        Dim CurrentHighestStrengthValue As Integer = Integer.MinValue
        Dim HighThresholdStrengthValue As Integer = Integer.MaxValue
        Dim AvailableTilesIndices As New List(Of Integer)
        Dim CurrentValue As Integer
        Dim CurrentTile As MapTile

        Do
            AvailableTilesIndices.Clear    'Reset this to ensure we always have an empty list to start with
            'Determine highest food strength value for inputted food item ID of all adjacent tiles
            For i As Integer = 0 to pStrengths.Length-1
                CurrentValue = pStrengths(i)
                CurrentTile = pMapTiles(i)
                If CurrentValue >= CurrentHighestStrengthValue AndAlso CurrentValue < HighThresholdStrengthValue _ 
                                        AndAlso CurrentTile.Passable 'AndAlso CurrentValue > LowThresholdStrengthValue 'XXX REMOVED THIS FOR NOW
                    If CurrentHighestStrengthValue < CurrentValue
                        AvailableTilesIndices.Clear    'If we found a new highest value, clear all the old tiles and start fresh for new high
                    End If
                    AvailableTilesIndices.Add(i)
                    CurrentHighestStrengthValue = CurrentValue
                End If
            Next

            If AvailableTilesIndices.Count > 0
                For Each Index As Integer In AvailableTilesIndices
                    If pMaptiles(Index).AntHere = False
                        MoveDirection = CType(Index, Direction)
                        MoveChosen = True
                        Exit For
                    End If
                Next
            Else
                'If no tiles are available to be chosen, then there does not exist a good move that can be moved to. 
                ''This means that all moves are either worse strength values, an ant exists there, or is not passable.
                ''In this scenario, we do not want to move at all.
                
                'We do not want this ant to actually move, so we can force this ant to not move by choosing a direction the
                'ant cannot actually move to, because our logic will later block it. This prevents us from needing to create
                'A new MoveDirection enum that includes a 'NoMove' parameter; it also prevents us from neeing to refactor the code.
                
                'For an adjacent tile this ant cannot actually move to, and choose it, causing our ant to stay where it is.
                For i As Integer = 0 to pMaptiles.Length-1
                    If pMaptiles(i).Passable = False or pMaptiles(i).AntHere = True
                        MoveDirection = CType(i, Direction)
                        MoveChosen = True
                        Exit For
                    End If
                Next
                
                'Verify a move was chosen, if not, we have a problem...
                If Not MoveChosen
                    'MsgBox("AHHHHH!!! - No move was chosen, and it should have been by now...")
                    'If a move was not chosen yet, this means that likely the ant has reached the tile with the highest strength
                    'value and therefore cannot move to any higher of a strength value, and none of the adjacent tiles contain ants 
                    'or water tiles. Therefore, in this case, lets just move to a default direction for the heck of it...
                    MoveDirection = Direction.n
                    MoveChosen = True
                End If
            End If

            'Check if a move was chosen, if not, reset parameters to look for next highest adjacent strength value
            If Not MoveChosen
                HighThresholdStrengthValue = CurrentHighestStrengthValue
                CurrentHighestStrengthValue = Integer.MinValue
            End If
        Loop Until MoveChosen
        Return MoveDirection
    
    End Function

    Public Function GetHighestValuedHashKey(pHashTable As Hashtable, Optional pIgnoreThisKey As String = "") As String
        Dim HighestKey As String = ""
        Dim HighestValue As Integer = Integer.MinValue
        For Each Key As String In pHashTable.Keys
            If HighestValue < CInt(pHashTable(Key)) AndAlso Key <> pIgnoreThisKey
                HighestValue = CInt(pHashTable(Key))
                HighestKey = Key
            End If
        Next
        If HighestKey <> ""
            Return HighestKey
        Else
            Throw New Exception("Error in GetHighestValuedHashKey: Key=" & HighestKey)
        End If
    End Function

    Public Function CheckIfPassable(pDirection As Direction) As Boolean
        Return CurrentMapTile.LinkedTiles(pDirection).Passable
    End Function

    Private Function GetTurn90DegreeDirection(pCurrentDirection As Direction, pClockwiseTurn As Boolean) As Direction
        Dim NewDirection As Direction = _PreviousDirection    'Set this to a value to prevent null ref 

        'I hate this algorithm... it is a stupid brute force. Need to fine a more elegant way to compute this.
        If pClockwiseTurn Then
            Select Case pCurrentDirection
                Case Direction.n
                    NewDirection = Direction.e
                Case Direction.e
                    NewDirection = Direction.s
                Case Direction.s
                    NewDirection = Direction.w
                Case Direction.w
                    NewDirection = Direction.n
            End Select
        Else
            Select Case pCurrentDirection
                Case Direction.n
                    NewDirection = Direction.w
                Case Direction.e
                    NewDirection = Direction.n
                Case Direction.s
                    NewDirection = Direction.e
                Case Direction.w
                    NewDirection = Direction.s
            End Select
        End If

        Return NewDirection
    End Function

    Private Function GetTurn180DegreeDirection(pCurrentDirection As Direction) As Direction
        Dim NewDirection As Direction = _PreviousDirection 'Set this to a value to prevent null ref

        Select Case pCurrentDirection
            Case Direction.n
                NewDirection = Direction.s
            Case Direction.e
                NewDirection = Direction.w
            Case Direction.s
                NewDirection = Direction.n
            Case Direction.w
                NewDirection = Direction.e
        End Select

        Return NewDirection
    End Function

#End Region

End Class
