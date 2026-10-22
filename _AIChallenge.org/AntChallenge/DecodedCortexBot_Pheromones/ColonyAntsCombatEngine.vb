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

Public Interface IColonyAntsCombatEngine
    Sub ExecuteColonyAntsCombatEngine(pTurnCount As Integer, pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                pEnemyAntLocations As List(Of Location), pEnemyAntHillLocations As List(Of Location), _ 
                                                pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                pMap As MapData, pAttackRadius As Integer)
End Interface


Public Class ColonyAntsCombatEngine : Implements IColonyAntsCombatEngine
    Private _Random As Random

    Public Sub New(pRandom As Random)
        _Random = pRandom
    End Sub

    Public Sub ExecuteColonyAntsCombatEngine(pTurnCount As Integer, pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                pEnemyAntLocations As List(Of Location), pEnemyAntHillLocations As List(Of Location), _ 
                                                pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                pMap As MapData, pAttackRadius As Integer) Implements IColonyAntsCombatEngine.ExecuteColonyAntsCombatEngine
        'Dim sw As New System.IO.StreamWriter("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\TurnTimers_CombatEngine.txt",True)
        'Dim Timer As New System.Diagnostics.Stopwatch
        'Timer.Start

        'Dim MyCombatAnts As New List(Of ColonyAnt)
        Dim MyHillDefenseAnts As New List(Of ColonyAnt)
        Dim CombatInfoContainer As New CombatInfoContainer

        'Gather a list of all of my ants that are near enemy ants. We can then ignore all other ants because they don't immediately matter
        For Each MyAnt As ColonyAnt In pColonyAnts
            'For Each AdjTile As MapTile In MyAnt.CurrentMapTile.LinkedTiles
                'If AdjTile.EnemyAntPheromoneStrengthHash.Count > 0
                '    MyCombatAnts.Add(MyAnt)

                    'Find out of this ant is near my ant hill, if so, add them to the defense ant list as well
                    If MyAnt.CurrentMapTile.MyAntHillStrength > 0 AndAlso MyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Count > 0
                        MyHillDefenseAnts.Add(MyAnt)
                    End If

                '    Exit For 'We added the ant, exit the for loop and look at the other my ants
                'End If
            'Next
        Next

        'Create and organize a list of my combat ants near each enemy ant
        'CombatInfoContainer.PopulateCombatAntsPerEnemyAntList(MyCombatAnts)
        CombatInfoContainer.PopulateCombatAntsPerEnemyAntList(pColonyAnts)

        'Sort the containers by certain criteria
        CombatInfoContainer.OrderCombatAntsPerEnemyAntsListByClosestToEnemyAnt(pMap)
        CombatInfoContainer.OrderCombatAntsPerEnemyAntsListByMyAntCount()
        CombatInfoContainer.OrderCombatAntsPerEnemyAntsListByClosestToMyAntHill()

        For i As Integer = 0 to CombatInfoContainer.CombatAntsPerEnemyAnt.Count-1
            IssueCombatOrdersForEnemyAnt(CombatInfoContainer.CombatAntsPerEnemyAnt(i), MyHillDefenseAnts, pAttackRadius, pMap)
        Next

        'Timer.Stop
        'sw.WriteLine(Timer.ElapsedMilliseconds)
        'sw.Close
    End Sub

    Public Sub IssueCombatOrdersForEnemyAnt(pCombatContainer As CombatAntsPerEnemyAntContainer, pMyHillDefenseAnts As List(Of ColonyAnt), pAttackRadius As Integer, pMap As MapData)
        If pCombatContainer.MyCombatAnts.Count > 2
            Dim HighestStrength, LowestStrength, StrengthRange, CountAntsAtHighestStrength As Integer
            GetRangeOfStrengthToEnemyAnt(pCombatContainer.MyCombatAnts, pCombatContainer.EnemyAnt, HighestStrength, LowestStrength, CountAntsAtHighestStrength)
            StrengthRange = HighestStrength - LowestStrength

            If StrengthRange = 0 Or CountAntsAtHighestStrength >= 2 Then
                'Ants are balanced, go in for attack
                IssueCombatOrdersAdvanceOnEnemyAnt(pCombatContainer.MyCombatAnts, pCombatContainer.EnemyAnt, HighestStrength, LowestStrength, pAttackRadius, pMap)
            Else
                'Ant distances are unbalanced, balance them before attack, let method know if ant is defending ant hill by including the list
                IssueCombatOrdersBalanceCombatAnts(pCombatContainer.MyCombatAnts, pCombatContainer.EnemyAnt, HighestStrength, LowestStrength, pMyHillDefenseAnts, pAttackRadius, pMap, CountAntsAtHighestStrength)
            End If

        Else
            'There is only 1 ant in range of this enemy ant. Call for help by issuing an alert on the map tile, this will call for re-enforcements
            'There should be only 1 ant, but do a For Each just in case... to prevent index out of bounds errors... 
            Dim CurrentMoveDirectionOrder As Direction
            For Each MyAnt As ColonyAnt In pCombatContainer.MyCombatAnts
                MyAnt.CurrentMapTile.SetAlertTileStrength(8)
                CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pCombatContainer.EnemyAnt)
                MyAnt.SetCombatDirectionOrder(CurrentMoveDirectionOrder)
                'MyAnt.CombatDirectionOrder = CurrentMoveDirectionOrder
                'MyAnt.GivenCombatOrder = True
            Next
        End If
    End Sub

    Public Sub IssueCombatOrdersAdvanceOnEnemyAnt(pCombatAnts As List(Of ColonyAnt), pEnemyAntID As String, pHighestStrength As Integer, pLowestStrength As Integer, pAttackRadius As Integer, pMap As MapData)
        Dim CurrentMoveDirectionOrder As Direction
        For Each MyAnt As ColonyAnt In pCombatAnts
            If Not MyAnt.GivenCombatOrder
                CurrentMoveDirectionOrder = GetMoveDirectionTowardThisEnemyAnt(MyAnt, pEnemyAntID, pAttackRadius, pMap)
                MyAnt.SetCombatDirectionOrder(CurrentMoveDirectionOrder)
            End If
        Next
    End Sub

    Public Sub IssueCombatOrdersBalanceCombatAnts(pCombatAnts As List(Of ColonyAnt), pEnemyAntID As String, pHighestStrength As Integer, pLowestStrength As Integer, pMyHillDefenseAnts As List(Of ColonyAnt), pAttackRadius As Integer, pMap As MapData, pCountAntsAtHighestStrength As Integer)
        Dim CurrentStrength As Integer
        Dim CurrentMoveDirectionOrder As Direction
        Dim Distance As Integer
        For Each MyAnt As ColonyAnt In pCombatAnts
            If Not MyAnt.GivenCombatOrder
                CurrentStrength = CInt(MyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
                If CurrentStrength = pHighestStrength 
                    'If this ant is the closest ant to the enemy, go away from the enemy ant without getting any closer to other enemy ants
                    'CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pEnemyAntID, pMyHillDefenseAnts.Contains(MyAnt))
            
                    'XXXX NEW COMBAT LOGIC - Now if ant at "Highest Strength" and more than AttackRadius+1, stay put, do not move...
                    Distance = pMap._MAXENEMYANTPHEROMONESTRENGTH - CurrentStrength - 1
                    If Distance > pAttackRadius + 2' AndAlso pCountAntsAtHighestStrength > 2'AndAlso MyAnt.CurrentMapTile.MyAntImmediatePheromoneStrengthHash.Count > 2
                        CurrentMoveDirectionOrder = Direction.None
                        'CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pEnemyAntID, pMyHillDefenseAnts.Contains(MyAnt))
                    Else
                        CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pEnemyAntID, pMyHillDefenseAnts.Contains(MyAnt))
                    'Else
                        'CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pEnemyAntID, pMyHillDefenseAnts.Contains(MyAnt))
                    End If
                Else
                    'If this ant is not the closest ant to the enemy, go towards the the enemy ant without getting ant closer to other enemy ants
                    CurrentMoveDirectionOrder = GetMoveDirectionTowardThisEnemyAnt(MyAnt, pEnemyAntID, pAttackRadius, pMap)
                End If
                MyAnt.SetCombatDirectionOrder(CurrentMoveDirectionOrder)
            End If
        Next
    End Sub

    Public Function GetMoveDirectionTowardThisEnemyAnt(pCombatAnt As ColonyAnt, pEnemyAntID As String, pAttackRadius As Integer, pMap As MapData) As Direction
        Dim MoveDirection As Direction
        Try
        'Get a list of all linked tiles that have a greater strength than current tile
        Dim MoveOptionTileIndices As New List(Of Integer)
        Dim CurrentTileStrength, AdjTileStrength As Integer
        For i As Integer = 0 to pCombatAnt.CurrentMapTile.LinkedTiles.Length-1
            CurrentTileStrength = CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
            AdjTileStrength = CInt(pCombatAnt.CurrentMapTile.LinkedTiles(i).EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
            If pCombatAnt.CurrentMapTile.LinkedTiles(i).Passable AndAlso Not pCombatAnt.CurrentMapTile.LinkedTiles(i).AntReservedTileForNextMove AndAlso _ 
                AdjTileStrength >= CurrentTileStrength
                MoveOptionTileIndices.Add(i)
            End If
        Next

        'We now have a list of indices of linked map tiles whose strength is greater than or equal to the current strength
        'Find the linked tile that also does not increase the strength towards other enemy ants
        Dim BestMoveOptionTileIndicies As New List(Of Integer)
        Dim LinkedTile As MapTile
        Dim DisqualifyTile As Boolean
        Dim Distance As Integer
        For i As Integer = 0 to MoveOptionTileIndices.Count-1
            DisqualifyTile = False
            LinkedTile = pCombatAnt.CurrentMapTile.LinkedTiles(MoveOptionTileIndices(i))
            For Each EnemyAntID As String In pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Keys
                CurrentTileStrength = CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(EnemyAntID))
                AdjTileStrength = CInt(LinkedTile.EnemyAntPheromoneStrengthHash.Item(EnemyAntID))
                Distance = (pMap._MAXENEMYANTPHEROMONESTRENGTH - AdjTileStrength)-1
                If AdjTileStrength > CurrentTileStrength AndAlso Distance < pAttackRadius' + 1
                    DisqualifyTile = True
                End If
            Next
            If Not DisqualifyTile
                BestMoveOptionTileIndicies.Add(MoveOptionTileIndices(i))
            End If
        Next

        'We should now have a list of maptiles whose enemy ant strength value is less than the current tile, and does not increase the 
        'strength values of any other nearby ants.
        'Lets further filter to only include the best moves. The enemy ant pheromone can be greater than or equal to the current tile's
        'enemy pheromone value. Lets filter this list so it only includes moves with the highest pheromone value, whether that is a greater
        'value or the same value as the current tile.
        Dim TempMoveOptions As New List(Of Integer)
        Dim AdjStrength As Integer
        Dim HighestStrength As Integer = Integer.MinValue
        If BestMoveOptionTileIndicies.Count > 0
            For i As Integer = 0 to BestMoveOptionTileIndicies.Count-1
                LinkedTile = pCombatAnt.CurrentMapTile.LinkedTiles(BestMoveOptionTileIndicies(i))
                'CurrentStrength = CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
                AdjStrength = CInt(LinkedTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
                If AdjStrength > HighestStrength
                    'We found a new highest value, clear out the old list and start a new one
                    HighestStrength = AdjStrength
                    TempMoveOptions.Clear
                    TempMoveOptions.Add(BestMoveOptionTileIndicies(i))
                ElseIf AdjStrength = HighestStrength
                    TempMoveOptions.Add(BestMoveOptionTileIndicies(i))
                End If
            Next
            BestMoveOptionTileIndicies = TempMoveOptions
        End If


        'If there are no such tiles, and therefore the new list is empty, lets choose a tile randomly from the first list.
        'If the first list is empty and therefore does not have any available moves, then just move north by default...
        If BestMoveOptionTileIndicies.Count > 0
            Dim RandIndex As Integer = _Random.Next Mod BestMoveOptionTileIndicies.Count
            MoveDirection = CType(BestMoveOptionTileIndicies(RandIndex), Direction)
        Else
            MoveDirection = Direction.None  'XXXXXX - Do not move if there are no best moves available...
            'Alternatively we can add logic here to check if the tile moved to for nearby enemy ants puts the ants within range of 
            'attack radius. We could allow our ants to get closer to our focused ant even if it means we get closer to other ants 
            'as long as getting closer to that other ant does not get them too close to the other ant......


            
            'If MoveOptionTileIndices.Count > 0
            '    Dim RandIndex As Integer = _Random.Next Mod MoveOptionTileIndices.Count
            '    MoveDirection = CType(MoveOptionTileIndices(RandIndex), Direction)
            'Else
            '    MoveDirection = Direction.None
            'End If
        End If
        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try
        Return MoveDirection
    End Function

    Public Function GetMoveDirectionAwayFromThisEnemyAnt(pCombatAnt As ColonyAnt, pEnemyAntID As String, Optional pIsAntHillDefender As Boolean=False) As Direction
        Dim MoveDirection As Direction
        Try
        'If Ant Hill Defender, get list of all maptiles that get closer to the ant hill
        Dim DefenderAntMoveOptionTileIndices As New List(Of Integer)
        If pIsAntHillDefender
            For i As Integer = 0 to pCombatAnt.CurrentMapTile.LinkedTiles.Length-1
                If pCombatAnt.CurrentMapTile.LinkedTiles(i).Passable AndAlso Not pCombatAnt.CurrentMapTile.LinkedTiles(i).AntReservedTileForNextMove AndAlso _ 
                    CInt(pCombatAnt.CurrentMapTile.LinkedTiles(i).MyAntHillStrength) <= CInt(pCombatAnt.CurrentMapTile.MyAntHillStrength)
                    DefenderAntMoveOptionTileIndices.Add(i)
                End If
            Next
        End If

        'Get a list of all linked tiles that have a smaller strength than current tile
        Dim MoveOptionTileIndices As New List(Of Integer)
        Dim CurrentTileStrength, AdjTileStrength As Integer
        For i As Integer = 0 to pCombatAnt.CurrentMapTile.LinkedTiles.Length-1
            CurrentTileStrength = CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
            AdjTileStrength = CInt(pCombatAnt.CurrentMapTile.LinkedTiles(i).EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
            If pCombatAnt.CurrentMapTile.LinkedTiles(i).Passable AndAlso Not pCombatAnt.CurrentMapTile.LinkedTiles(i).AntReservedTileForNextMove AndAlso _ 
                AdjTileStrength < CurrentTileStrength 'XXXX - ONLY TILES Less Than... AdjTileStrength <= CurrentTileStrength
                MoveOptionTileIndices.Add(i)
            End If
        Next

        'If Ant Hill Defender, get list of available tiles that do not get further away from the the ant hill, but get further away from the enemy ant
        If pIsAntHillDefender
            Dim TempFilteredDefenderMoveOptionTileIndices As New List(Of Integer)
            For i As Integer = 0 to pCombatAnt.CurrentMapTile.LinkedTiles.Length-1
                If pCombatAnt.CurrentMapTile.LinkedTiles(i).Passable AndAlso _ 
                    CInt(pCombatAnt.CurrentMapTile.LinkedTiles(i).EnemyAntPheromoneStrengthHash.Item(pEnemyAntID)) <= CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID)) _ 
                    AndAlso DefenderAntMoveOptionTileIndices.Contains(i)
                    TempFilteredDefenderMoveOptionTileIndices.Add(i)
                End If
            Next
            'Do not count these filtered tiles if none were valid
            If TempFilteredDefenderMoveOptionTileIndices.Count > 0 Then
                DefenderAntMoveOptionTileIndices = TempFilteredDefenderMoveOptionTileIndices
            End If
        End If

        'We now have a list of indices of linked map tiles whose strength is less than the current strength
        'Find the linked tile that also does not increase the strength towards other enemy ants
        Dim BestMoveOptionTileIndicies As New List(Of Integer)
        Dim LinkedTile As MapTile
        Dim DisqualifyTile As Boolean
        For i As Integer = 0 to MoveOptionTileIndices.Count-1
            DisqualifyTile = False
            LinkedTile = pCombatAnt.CurrentMapTile.LinkedTiles(MoveOptionTileIndices(i))
            For Each EnemyAntID As String In pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Keys
                CurrentTileStrength = CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(EnemyAntID))
                AdjTileStrength = CInt(LinkedTile.EnemyAntPheromoneStrengthHash.Item(EnemyAntID))
                If AdjTileStrength > CurrentTileStrength
                    DisqualifyTile = True
                End If
            Next
            If Not DisqualifyTile
                BestMoveOptionTileIndicies.Add(MoveOptionTileIndices(i))
            End If
        Next

        'Now we have a list of moves that will move us away from the enemy ant that does not get us any closer to other ants.
        'Lets now further filter this list so that if it contains any values, it will only contain values that move the ant the
        'furthest away
        Dim TempMoveOptions As New List(Of Integer)
        Dim AdjStrength As Integer
        Dim LowestStrength As Integer = Integer.MaxValue
        If BestMoveOptionTileIndicies.Count > 0
            For i As Integer = 0 to BestMoveOptionTileIndicies.Count-1
                LinkedTile = pCombatAnt.CurrentMapTile.LinkedTiles(BestMoveOptionTileIndicies(i))
                'CurrentStrength = CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
                AdjStrength = CInt(LinkedTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
                If AdjStrength < LowestStrength
                    'We found a new lowest value, clear out the old list and start a new one
                    LowestStrength = AdjStrength
                    TempMoveOptions.Clear
                    TempMoveOptions.Add(BestMoveOptionTileIndicies(i))
                ElseIf AdjStrength = LowestStrength
                    TempMoveOptions.Add(BestMoveOptionTileIndicies(i))
                End If
            Next
            BestMoveOptionTileIndicies = TempMoveOptions
        End If

        'We have our list filtered, lets filter it some more. Make sure that of the move options available, lets remove any tiles
        'That are "crevices" - they have 3 unpassable adjacent tiles...
        Dim TempMoveOptions2 As New List(Of Integer)
        Dim CountUnpassable As Integer
        For i As Integer = 0 to BestMoveOptionTileIndicies.Count-1
            CountUnpassable = 0
            LinkedTile = pCombatAnt.CurrentMapTile.LinkedTiles(BestMoveOptionTileIndicies(i))
            For Each AdjLinkedTile As MapTile In LinkedTile.LinkedTiles
                If Not AdjLinkedTile.Passable
                    CountUnpassable += 1
                End If
            Next
            If CountUnpassable < 3
                TempMoveOptions2.Add(BestMoveOptionTileIndicies(i))
            End If
            'If CountUnpassable = 3
            '    BestMoveOptionTileIndicies.Remove(BestMoveOptionTileIndicies(i))
            'End If
        Next
        BestMoveOptionTileIndicies = TempMoveOptions2

        'We should now have a list of maptiles whose enemy ant strength value is less than the current tile, and does not increase the 
        'strength values of any other nearby ants.
        'If there are no such tiles, and therefore the new list is empty, lets choose a tile randomly from the first list.
        'If the first list is empty and therefore does not have any available moves, then just move north by default...
        If pIsAntHillDefender AndAlso DefenderAntMoveOptionTileIndices.Count > 0
            Dim RandIndex As Integer = _Random.Next Mod DefenderAntMoveOptionTileIndices.Count
            MoveDirection = CType(DefenderAntMoveOptionTileIndices(RandIndex), Direction)
        ElseIf BestMoveOptionTileIndicies.Count > 0
            Dim RandIndex As Integer = _Random.Next Mod BestMoveOptionTileIndicies.Count
            MoveDirection = CType(BestMoveOptionTileIndicies(RandIndex), Direction)
        Else
            MoveDirection = Direction.None

            'If MoveOptionTileIndices.Count > 0
            '    Dim RandIndex As Integer = _Random.Next Mod MoveOptionTileIndices.Count
            '    MoveDirection = CType(MoveOptionTileIndices(RandIndex), Direction)
            'Else
            '    MoveDirection = Direction.None
            'End If
        End If
        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try
        Return MoveDirection
    End Function

    Public Sub GetRangeOfStrengthToEnemyAnt(pCombatAnts As List(Of ColonyAnt), pEnemyAntID As String, _ 
                                            ByRef pHighestStrength As Integer, ByRef pLowestStrength As Integer, _ 
                                            ByRef pCountAntsAtHighestStrength As Integer)
        Dim HighestStrength As Integer = Integer.MinValue
        Dim LowestStrength As Integer = Integer.MaxValue
        Dim CurrentStrength As Integer
        Dim CountAntsAtHighestStrength As Integer = 0

        'Determine the highest strength value to the enemy ant for the combat ants
        For Each MyAnt As ColonyAnt In pCombatAnts
            CurrentStrength = CInt(MyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
            If CurrentStrength > HighestStrength
                HighestStrength = CurrentStrength
                CountAntsAtHighestStrength = 1
            ElseIf CurrentStrength = HighestStrength
                CountAntsAtHighestStrength += 1
            End If
            If CurrentStrength < LowestStrength
                LowestStrength = CurrentStrength
            End If
        Next
        pHighestStrength = HighestStrength
        pLowestStrength = LowestStrength
        pCountAntsAtHighestStrength = CountAntsAtHighestStrength
    End Sub

    Public Function GetHighestStrengthToEnemyAnt(pCombatAnts As List(Of ColonyAnt), pEnemyAntID As String) As Integer
        Dim HighestStrength As Integer = Integer.MinValue
        Dim CurrentStrength As Integer

        'Determine the highest strength value to the enemy ant for the combat ants
        For Each MyAnt As ColonyAnt In pCombatAnts
            CurrentStrength = CInt(MyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
            If CurrentStrength > HighestStrength
                HighestStrength = CurrentStrength
            End If
        Next
        Return HighestStrength
    End Function

    Public Function GetStrengthToEnemyAnt(pCombatAnt As ColonyAnt, pEnemyAntID As String) As Integer
        Return CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
    End Function


#Region "TEST COMBAT METHODS"
    Public Sub IssueCombatAntMoveDirectionOrder_TEST(pColonyAnt As ColonyAnt, pAttackRadius As Integer, pMap As MapData)
        'Get the current tile's enemy ant influence
        Dim CurrentTileEnemyAntInfluence As Integer = 0
        For Each Value As Integer In pColonyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Values
            CurrentTileEnemyAntInfluence += Value
        Next

        For i As Integer = 0 to pColonyAnt.CurrentMapTile.LinkedTiles.Length-1

        Next

    End Sub
#End Region
End Class
