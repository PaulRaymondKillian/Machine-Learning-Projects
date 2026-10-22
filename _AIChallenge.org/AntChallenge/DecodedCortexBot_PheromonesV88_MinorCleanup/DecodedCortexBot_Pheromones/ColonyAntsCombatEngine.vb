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

        Dim MyCombatAnts As New List(Of ColonyAnt)
        Dim MyHillDefenseAnts As New List(Of ColonyAnt)
        Dim CombatInfoContainer As New CombatInfoContainer

        'Gather a list of all of my ants that are near enemy ants. We can then ignore all other ants because they don't immediately matter
        For Each MyAnt As ColonyAnt In pColonyAnts
            For Each AdjTile As MapTile In MyAnt.CurrentMapTile.LinkedTiles
                If AdjTile.EnemyAntPheromoneStrengthHash.Count > 0
                    MyCombatAnts.Add(MyAnt)

                    'Find out of this ant is near my ant hill, if so, add them to the defense ant list as well
                    If MyAnt.CurrentMapTile.MyAntHillStrength > 0 Then
                        MyHillDefenseAnts.Add(MyAnt)
                    End If

                    Exit For 'We added the ant, exit the for loop and look at the other my ants
                End If
            Next
        Next

        'Create and organize a list of my combat ants near each enemy ant
        CombatInfoContainer.PopulateCombatAntsPerEnemyAntList(MyCombatAnts)
        'CombatInfoContainer.OrderCombatAntsPerEnemyAntsListByMyAntCount()
        CombatInfoContainer.OrderCombatAntsPerEnemyAntsListByClosestToEnemyAnt()
        CombatInfoContainer.OrderCombatAntsPerEnemyAntsListByClosestToMyAntHill()

        For i As Integer = 0 to CombatInfoContainer.CombatAntsPerEnemyAnt.Count-1
            IssueCombatOrdersForEnemyAnt(CombatInfoContainer.CombatAntsPerEnemyAnt(i), MyHillDefenseAnts, pAttackRadius, pMap)
        Next

        
        '
        '
        ''Create a list of my ants near each enemy ant, use "index" of pEnemyAntLocations as the key for the hash table
        'For Index As Integer = 0 to pEnemyAntLocations.Count-1
        '    For Each MyAnt As ColonyAnt In MyCombatAnts
        '        For Each AdjTile As MapTile In MyAnt.CurrentMapTile.LinkedTiles
        '            If CInt(AdjTile.EnemyAntPheromoneStrengthHash.Item(pMap.GetMapTile(pEnemyAntLocations(Index).Row, pEnemyAntLocations(Index).Col).ID)) > 0
        '                'If CombatAntsPerEnemyAntList.
        '                If Not EnemyAntCombatAntListHash.ContainsKey(Index)
        '                    EnemyAntCombatAntListHash.Add(Index, New List(Of ColonyAnt))
        '                End If
        '                CType(EnemyAntCombatAntListHash.Item(Index), List(Of ColonyAnt)).Add(MyAnt)
        '                Exit For'We added this ant for this enemy ant in the hash table, go on to the next ant
        '            End If
        '        Next
        '    Next
        'Next

        ''Transpose the kinda convoluted hashtable to the list of the combat ants structure
        'For Each Key As String In EnemyAntCombatAntListHash.Keys
        '    Dim NewStructure As CombatAntsPerEnemyAntContainer
        '    NewStructure.EnemyAntLocation = pEnemyAntLocations(CInt(Key))
        '    NewStructure.MyCombatAnts = CType(EnemyAntCombatAntListHash.Item(CInt(Key)), List(Of ColonyAnt))
        '    CombatAntsPerEnemyAntList.Add(NewStructure)
        'Next

        ''Now we have a nice list of objects that contain information on enemy ants and nearby my ants... lets do stuff with it now.

        ''Find the enemy ant who has the most of my ants nearby
        'Dim HighestCombatContainer As CombatAntsPerEnemyAntContainer
        'Dim HighestMyAntCount As Integer = Integer.MinValue
        'For Each Container As CombatAntsPerEnemyAntContainer In CombatAntsPerEnemyAntList
        '    If Container.MyCombatAnts.Count > HighestMyAntCount
        '        HighestMyAntCount = Container.MyCombatAnts.Count
        '        HighestCombatContainer = Container
        '    End If
        'Next

        'If Not HighestCombatContainer.MyCombatAnts Is Nothing
        '    IssueCombatOrdersForEnemyAnt(HighestCombatContainer.EnemyAntLocation, HighestCombatContainer.MyCombatAnts)
        'End If

        'Timer.Stop
        'sw.WriteLine(Timer.ElapsedMilliseconds)
        'sw.Close
    End Sub

    'Public Sub IssueCombatOrdersForEnemyAnt(pEnemyAntLocation As Location, pMyCombatAnts As List(Of ColonyAnt))

    'End Sub

    Public Sub IssueCombatOrdersForEnemyAnt(pCombatContainer As CombatAntsPerEnemyAntContainer, pMyHillDefenseAnts As List(Of ColonyAnt), pAttackRadius As Integer, pMap As MapData)
        If pCombatContainer.MyCombatAnts.Count > 1
            Dim HighestStrength, LowestStrength, StrengthRange, CountAntsAtHighestStrength As Integer
            GetRangeOfStrengthToEnemyAnt(pCombatContainer.MyCombatAnts, pCombatContainer.EnemyAnt, HighestStrength, LowestStrength, CountAntsAtHighestStrength)
            StrengthRange = HighestStrength - LowestStrength

            If StrengthRange = 0 Or CountAntsAtHighestStrength > 3 Then
                'Ants are balanced, go in for attack
                IssueCombatOrdersAdvanceOnEnemyAnt(pCombatContainer.MyCombatAnts, pCombatContainer.EnemyAnt, HighestStrength, LowestStrength, pAttackRadius)
            Else
                'Ant distances are unbalanced, balance them before attack, let method know if ant is defending ant hill by including the list
                IssueCombatOrdersBalanceCombatAnts(pCombatContainer.MyCombatAnts, pCombatContainer.EnemyAnt, HighestStrength, LowestStrength, pMyHillDefenseAnts, pAttackRadius, pMap)
            End If

        Else
            'There is only 1 ant in range of this enemy ant. Call for help by issuing an alert on the map tile, this will call for re-enforcements
            'There should be only 1 ant, but do a For Each just in case... to prevent index out of bounds errors... 
            Dim CurrentMoveDirectionOrder As Direction
            For Each MyAnt As ColonyAnt In pCombatContainer.MyCombatAnts
                MyAnt.CurrentMapTile.SetAlertTileStrength(8)
                CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pCombatContainer.EnemyAnt)
                MyAnt.CombatDirectionOrder = CurrentMoveDirectionOrder
                MyAnt.GivenCombatOrder = True
            Next
        End If
    End Sub

    Public Sub IssueCombatOrdersAdvanceOnEnemyAnt(pCombatAnts As List(Of ColonyAnt), pEnemyAntID As String, pHighestStrength As Integer, pLowestStrength As Integer, pAttackRadius As Integer)
        Dim CurrentMoveDirectionOrder As Direction
        For Each MyAnt As ColonyAnt In pCombatAnts
            CurrentMoveDirectionOrder = GetMoveDirectionTowardThisEnemyAnt(MyAnt, pEnemyAntID, pAttackRadius)
            MyAnt.CombatDirectionOrder = CurrentMoveDirectionOrder
            MyAnt.GivenCombatOrder = True
        Next
    End Sub

    Public Sub IssueCombatOrdersBalanceCombatAnts(pCombatAnts As List(Of ColonyAnt), pEnemyAntID As String, pHighestStrength As Integer, pLowestStrength As Integer, pMyHillDefenseAnts As List(Of ColonyAnt), pAttackRadius As Integer, pMap As MapData)
        Dim CurrentStrength As Integer
        Dim CurrentMoveDirectionOrder As Direction
        For Each MyAnt As ColonyAnt In pCombatAnts
            CurrentStrength = CInt(MyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
            If CurrentStrength = pHighestStrength
                'If this ant is the closest ant to the enemy, go away from the enemy ant without getting any closer to other enemy ants
                'CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pEnemyAntID, pMyHillDefenseAnts.Contains(MyAnt))
            
                'XXXX NEW COMBAT LOGIC - Now if ant at "Highest Strength" and more than AttackRadius+1, stay put, do not move...
                If (pMap._MAXENEMYANTPHEROMONESTRENGTH - CurrentStrength) > pAttackRadius + 1
                    CurrentMoveDirectionOrder = Direction.None
                ElseIf MyAnt.CurrentMapTile.MyAntSinglePersistentPheromoneStrength.Count = 1 Then
                    CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pEnemyAntID, pMyHillDefenseAnts.Contains(MyAnt))
                'Else
                    'CurrentMoveDirectionOrder = GetMoveDirectionAwayFromThisEnemyAnt(MyAnt, pEnemyAntID, pMyHillDefenseAnts.Contains(MyAnt))
                End If
            Else
                'If this ant is not the closest ant to the enemy, go towards the the enemy ant without getting ant closer to other enemy ants
                CurrentMoveDirectionOrder = GetMoveDirectionTowardThisEnemyAnt(MyAnt, pEnemyAntID, pAttackRadius)
            End If
            MyAnt.CombatDirectionOrder = CurrentMoveDirectionOrder
            MyAnt.GivenCombatOrder = True
        Next
    End Sub

    Public Function GetMoveDirectionTowardThisEnemyAnt(pCombatAnt As ColonyAnt, pEnemyAntID As String, pAttackRadius As Integer) As Direction
        Dim MoveDirection As Direction

        'Get a list of all linked tiles that have a smaller strength than current tile
        Dim MoveOptionTileIndices As New List(Of Integer)
        For i As Integer = 0 to pCombatAnt.CurrentMapTile.LinkedTiles.Length-1
            If pCombatAnt.CurrentMapTile.LinkedTiles(i).Passable AndAlso _ 
                CInt(pCombatAnt.CurrentMapTile.LinkedTiles(i).EnemyAntPheromoneStrengthHash.Item(pEnemyAntID)) >= CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
                MoveOptionTileIndices.Add(i)
            End If
        Next

        'We now have a list of indices of linked map tiles whose strength is less than the current strength
        'Find the linked tile that also does not increase the strength towards other enemy ants
        Dim BestMoveOptionTileIndicies As New List(Of Integer)
        Dim LinkedTile As MapTile
        Dim DisqualifyTile As Boolean
        For i As Integer = 0 to MoveOptionTileIndices.Count-1
            DisqualifyTile = False
            LinkedTile = pCombatAnt.CurrentMapTile.LinkedTiles(MoveOptionTileIndices(i))
            For Each EnemyAntID As String In pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Keys
                If CInt(LinkedTile.EnemyAntPheromoneStrengthHash.Item(EnemyAntID)) > CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(EnemyAntID))
                    DisqualifyTile = True
                End If
            Next
            If Not DisqualifyTile
                BestMoveOptionTileIndicies.Add(MoveOptionTileIndices(i))
            End If
        Next

        'We should now have a list of maptiles whose enemy ant strength value is less than the current tile, and does not increase the 
        'strength values of any other nearby ants.
        'If there are no such tiles, and therefore the new list is empty, lets choose a tile randomly from the first list.
        'If the first list is empty and therefore does not have any available moves, then just move north by default...
        If BestMoveOptionTileIndicies.Count > 0
            Dim RandIndex As Integer = _Random.Next Mod BestMoveOptionTileIndicies.Count
            MoveDirection = CType(BestMoveOptionTileIndicies(RandIndex), Direction)
        Else
            If MoveOptionTileIndices.Count > 0
                Dim RandIndex As Integer = _Random.Next Mod MoveOptionTileIndices.Count
                MoveDirection = CType(MoveOptionTileIndices(RandIndex), Direction)
            Else
                MoveDirection = Direction.None
            End If
        End If

        Return MoveDirection
    End Function

    Public Function GetMoveDirectionAwayFromThisEnemyAnt(pCombatAnt As ColonyAnt, pEnemyAntID As String, Optional pIsAntHillDefender As Boolean=False) As Direction
        Dim MoveDirection As Direction

        'If Ant Hill Defender, get list of all maptiles that get closer to the ant hill
        Dim DefenderAntMoveOptionTileIndices As New List(Of Integer)
        For i As Integer = 0 to pCombatAnt.CurrentMapTile.LinkedTiles.Length-1
            If pCombatAnt.CurrentMapTile.LinkedTiles(i).Passable AndAlso _ 
                CInt(pCombatAnt.CurrentMapTile.LinkedTiles(i).MyAntHillStrength) <= CInt(pCombatAnt.CurrentMapTile.MyAntHillStrength)
                DefenderAntMoveOptionTileIndices.Add(i)
            End If
        Next

        'Get a list of all linked tiles that have a smaller strength than current tile
        Dim MoveOptionTileIndices As New List(Of Integer)
        For i As Integer = 0 to pCombatAnt.CurrentMapTile.LinkedTiles.Length-1
            If pCombatAnt.CurrentMapTile.LinkedTiles(i).Passable AndAlso _ 
                CInt(pCombatAnt.CurrentMapTile.LinkedTiles(i).EnemyAntPheromoneStrengthHash.Item(pEnemyAntID)) <= CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(pEnemyAntID))
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
                If CInt(LinkedTile.EnemyAntPheromoneStrengthHash.Item(EnemyAntID)) > CInt(pCombatAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Item(EnemyAntID))
                    DisqualifyTile = True
                End If
            Next
            If Not DisqualifyTile
                BestMoveOptionTileIndicies.Add(MoveOptionTileIndices(i))
            End If
        Next

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
            If MoveOptionTileIndices.Count > 0
                Dim RandIndex As Integer = _Random.Next Mod MoveOptionTileIndices.Count
                MoveDirection = CType(MoveOptionTileIndices(RandIndex), Direction)
            Else
                MoveDirection = Direction.n
            End If
        End If

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
End Class
