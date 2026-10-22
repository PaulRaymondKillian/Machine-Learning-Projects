Option Strict On

Imports System.Collections.Generic

Public Enum ColonyAntMission
    Explorer
    FoodGatherer
    AntHunter
    AntHillHunter
End Enum


Public Class ColonyAnt
    'Public CurrentLocation As Location
    Public CurrentMapTile As MapTile
    Public LeafPingMapTiles As List(Of LeafPingTiles)
    Public Mission As ColonyAntMission
    Public PreviousDirection As Direction
    Public StepCount As Integer
    Const STEPCOUNTTHRESHOLD As Integer = 10

    Private _Random As Random

    Public Sub New(pCurrentMapTile As MapTile, pRandom As Random)
        CurrentMapTile = pCurrentMapTile
        Mission = ColonyAntMission.Explorer
        PreviousDirection = Direction.e
        StepCount = 0
        _Random = pRandom
    End Sub

    Public Function GetAntMoveDirection As Direction
        Dim MoveDirection As Direction
        Select Case Mission
            Case ColonyAntMission.Explorer:
                MoveDirection = GetExplorerMoveDirection()
            Case ColonyAntMission.FoodGatherer:
                MsgBox("Warning! Not implemented yet")
            Case ColonyAntMission.AntHunter:
                MsgBox("Warning! Not implemented yet")
            Case ColonyAntMission.AntHillHunter:
                MsgBox("Warning! Not implemented yet")

        End Select

        Return MoveDirection
    End Function

    Private Function GetExplorerMoveDirection() As Direction
        Dim MoveDirection As Direction
        'Walk away from MyAntHill until we get far enough away that we have no more guidance using
        '' the Strength hash table. Then, do a Random Walk.

        If OnFoodItemStrengthTile() Then
            'We are near a food item, lets move towards it

            'Find the highest food strength value
            Dim HighestStrengthID As String = GetHighestAdjacentFoodStrengthValue()

            'Choose the adjacent tile that has the highest value for the found ID
            MoveDirection = ChooseHighestAdjacentFoodItemStrengthDirection(HighestStrengthID)

        ElseIf OnEnemyAntHillStrengthTile() Then
            'We are near an enemy ant hill, lets move towards it

            'If CurrentMapTile.EnemyAntHillStrengthHash.Count > 0
            '    Dim CurrentStr As Integer = CInt(CurrentMapTile.EnemyAntHillStrengthHash("(32,65)"))
            '    If CurrentStr = 31
            '        MsgBox("AHHH")
            '    End If
            'End If

            'Find the highest enemy ant hill strength value
            Dim HighestStrengthID As String = GetHighestAdjacentEnemyAntHillStrengthValue()

            'Choose the adjacent tile that has the highest value for the found ID
            MoveDirection = ChooseHighestAdjacentEnemyAntHillStrengthDirection(HighestStrengthID)
        Else
            'Dim CurrentLeafPingMaptiles As List(Of LeafPingTiles)
            'CurrentLeafPingMaptiles = GetListOfPingLeafMaptiles()

            Dim TestHash As New Hashtable
            For Each Tile As LeafPingTiles In LeafPingMaptiles
                If TestHash(Tile.MapTile.GetMapTileAsString) is Nothing
                    TestHash(Tile.MapTile.GetMapTileAsString) = 1
                Else
                    TestHash(Tile.MapTile.GetMapTileAsString) = CInt(TestHash(Tile.MapTile.GetMapTileAsString)) + 1 
                End If
            Next


            'We are not near a food item or enemy ant hill, lets move away from the ant hill and explore,
             ''If too far from ant hill, do a random walk

            'Find the lowest MyAntHill Strength of all adjacent tiles
            Dim LowestStrengthValue As Integer = GetLowestAdjacentMyAntHillStrengthValue()

            If LowestStrengthValue = 0 Then
                'We are at the edge of our MyAntHill Strength mapping, start random walking
                MoveDirection = GetRandomWalkMoveDirection()
            Else
                'We are still on our MyAntHill Strength mapping, determine our next move along the mapping
                MoveDirection = ChooseLowestAdjacentMyAntHillStrengthDirection(LowestStrengthValue)
            End If

            'MoveDirection = GetRandomWalkMoveDirection

        End If
        
        Return MoveDirection
    End Function

#Region "Move Towards Closest Food Item Logic"

    Private Function OnFoodItemStrengthTile() As Boolean
        Dim FoodStrengthItemHere As Boolean
        If CurrentMapTile.FoodStrengthHash.Count > 0 Then
            FoodStrengthItemHere = True
        Else
            FoodStrengthItemHere = False
        End If
        Return FoodStrengthItemHere
    End Function

    Private Function ChooseHighestAdjacentFoodItemStrengthDirection(pHighestStrengthID As String) As Direction
        Dim MoveDirection As Direction
        'Determine highest food strength value for inputted food item ID of all adjacent tiles
        Dim HighestFoodStrengthMapTile As MapTile = CurrentMapTile.LinkedTiles(0) 'Set default value to prevent null ref
        Dim HighestFoodStrengthValue As Integer = Integer.MinValue
        For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
            If CInt(MapTile.FoodStrengthHash(pHighestStrengthID)) >= HighestFoodStrengthValue Then
                HighestFoodStrengthValue = CInt(MapTile.FoodStrengthHash(pHighestStrengthID))
                HighestFoodStrengthMapTile = MapTile
            End If
        Next

        'Determine what direction we must move to go to this tile
        For i As Integer = 0 To CurrentMapTile.LinkedTiles.Length - 1
            If CurrentMapTile.LinkedTiles(i).Equals(HighestFoodStrengthMapTile) Then
                MoveDirection = CType(i, Direction)
                Exit For
            End If
        Next
        Return MoveDirection
    End Function

    Private Function GetHighestAdjacentFoodStrengthValue() As String
        'Find the highest food strength value
        Dim HighestStrengthValue As Integer = Integer.MinValue
        Dim HighestStrengthID As String = ""
        For Each FoodStrengthItemID As String In CurrentMapTile.FoodStrengthHash.Keys
            If HighestStrengthValue < CInt(CurrentMapTile.FoodStrengthHash(FoodStrengthItemID)) Then
                HighestStrengthValue = CInt(CurrentMapTile.FoodStrengthHash(FoodStrengthItemID))
                HighestStrengthID = FoodStrengthItemID
            End If
        Next
        Return HighestStrengthID
    End Function

#End Region

#Region "Move Towards Closest Enemy Ant Hill Logic"

    Private Function OnEnemyAntHillStrengthTile() As Boolean
        Dim EnemyAntHillStrengthItemHere As Boolean
        If CurrentMapTile.EnemyAntHillStrengthHash.Count > 0 Then
            EnemyAntHillStrengthItemHere = True
        Else
            EnemyAntHillStrengthItemHere = False
        End If
        Return EnemyAntHillStrengthItemHere
    End Function

    Private Function ChooseHighestAdjacentEnemyAntHillStrengthDirection(pHighestStrengthID As String) As Direction
        Dim MoveDirection As Direction
        'Determine highest food strength value for inputted food item ID of all adjacent tiles
        Dim HighestEnemyAntHillStrengthMapTile As MapTile = CurrentMapTile.LinkedTiles(0) 'Set default value to prevent null ref
        Dim HighestEnemyAntHillStrengthValue As Integer = Integer.MinValue
        For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
            If CInt(MapTile.EnemyAntHillStrengthHash(pHighestStrengthID)) >= HighestEnemyAntHillStrengthValue Then
                HighestEnemyAntHillStrengthValue = CInt(MapTile.EnemyAntHillStrengthHash(pHighestStrengthID))
                HighestEnemyAntHillStrengthMapTile = MapTile
            End If
        Next

        'Determine what direction we must move to go to this tile
        For i As Integer = 0 To CurrentMapTile.LinkedTiles.Length - 1
            If CurrentMapTile.LinkedTiles(i).Equals(HighestEnemyAntHillStrengthMapTile) Then
                MoveDirection = CType(i, Direction)
                Exit For
            End If
        Next
        Return MoveDirection
    End Function

    Private Function GetHighestAdjacentEnemyAntHillStrengthValue() As String
        'Find the highest food strength value
        Dim HighestStrengthValue As Integer = Integer.MinValue
        Dim HighestStrengthID As String = ""
        For Each EnemyAntHillStrengthItemID As String In CurrentMapTile.EnemyAntHillStrengthHash.Keys
            If HighestStrengthValue < CInt(CurrentMapTile.EnemyAntHillStrengthHash(EnemyAntHillStrengthItemID)) Then
                HighestStrengthValue = CInt(CurrentMapTile.EnemyAntHillStrengthHash(EnemyAntHillStrengthItemID))
                HighestStrengthID = EnemyAntHillStrengthItemID
            End If
        Next
        Return HighestStrengthID
    End Function

#End Region

#Region "Move Explore - Ping Pathfinding"

    Public Sub SetLeafPingMaptiles(pLeafPingMaptiles As List(Of LeafPingTiles))
        LeafPingMapTiles = pLeafPingMaptiles
    End Sub
                                    

    'Private Function GetListOfPingLeafMaptiles() As List(Of LeafPingTiles)
    '    Dim ListLeafPingTiles As New List(Of LeafPingTiles)

    '    CurrentMapTile.SetMyAntTileStrength(CurrentMapTile.GetMapTileAsString, MAXMYANTTILESTRENGTH, ListLeafPingTiles, 0, 0, 0, 0)

    '    Return ListLeafPingTiles
    'End Function

    'Private Function ChooseHighestAdjacentEnemyAntHillStrengthDirection(pHighestStrengthID As String) As Direction
    '    Dim MoveDirection As Direction
    '    'Determine highest food strength value for inputted food item ID of all adjacent tiles
    '    Dim HighestEnemyAntHillStrengthMapTile As MapTile = CurrentMapTile.LinkedTiles(0) 'Set default value to prevent null ref
    '    Dim HighestEnemyAntHillStrengthValue As Integer = Integer.MinValue
    '    For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
    '        If CInt(MapTile.EnemyAntHillStrengthHash(pHighestStrengthID)) >= HighestEnemyAntHillStrengthValue Then
    '            HighestEnemyAntHillStrengthValue = CInt(MapTile.EnemyAntHillStrengthHash(pHighestStrengthID))
    '            HighestEnemyAntHillStrengthMapTile = MapTile
    '        End If
    '    Next

    '    'Determine what direction we must move to go to this tile
    '    For i As Integer = 0 To CurrentMapTile.LinkedTiles.Length - 1
    '        If CurrentMapTile.LinkedTiles(i).Equals(HighestEnemyAntHillStrengthMapTile) Then
    '            MoveDirection = CType(i, Direction)
    '            Exit For
    '        End If
    '    Next
    '    Return MoveDirection
    'End Function

    'Private Function GetHighestAdjacentEnemyAntHillStrengthValue() As String
    '    'Find the highest food strength value
    '    Dim HighestStrengthValue As Integer = Integer.MinValue
    '    Dim HighestStrengthID As String = ""
    '    For Each EnemyAntHillStrengthItemID As String In CurrentMapTile.EnemyAntHillStrengthHash.Keys
    '        If HighestStrengthValue < CInt(CurrentMapTile.EnemyAntHillStrengthHash(EnemyAntHillStrengthItemID)) Then
    '            HighestStrengthValue = CInt(CurrentMapTile.EnemyAntHillStrengthHash(EnemyAntHillStrengthItemID))
    '            HighestStrengthID = EnemyAntHillStrengthItemID
    '        End If
    '    Next
    '    Return HighestStrengthID
    'End Function

#End Region

#Region "Move Away From MyAntHill Logic"

    Private Function GetLowestAdjacentMyAntHillStrengthValue() As Integer
        'Find the lowest MyAntHill Strength of all adjacent tiles
        Dim LowestStrengthValue As Integer = Integer.MaxValue
        For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
            If MapTile.GetMyAntHillStrength < LowestStrengthValue And MapTile.Passable And MapTile.AntHere = False Then
                LowestStrengthValue = MapTile.GetMyAntHillStrength
            End If
        Next


        If LowestStrengthValue = Integer.MaxValue
            LowestStrengthValue = 0
            'Throw New Exception("LowestStrengthValue=" & LowestStrengthValue & ", " & _ 
            '                    "Linked(0)=" & CurrentMapTile.LinkedTiles(0).GetMyAntHillStrength & "-" & CurrentMapTile.LinkedTiles(0).Passable & ", " & _
            '                    "Linked(1)=" & CurrentMapTile.LinkedTiles(1).GetMyAntHillStrength & "-" & CurrentMapTile.LinkedTiles(1).Passable & ", " & _
            '                    "Linked(2)=" & CurrentMapTile.LinkedTiles(2).GetMyAntHillStrength & "-" & CurrentMapTile.LinkedTiles(2).Passable & ", " & _
            '                    "Linked(3)=" & CurrentMapTile.LinkedTiles(3).GetMyAntHillStrength & "-" & CurrentMapTile.LinkedTiles(3).Passable & ", ")
        End If



        Return LowestStrengthValue
    End Function

    Private Function ChooseLowestAdjacentMyAntHillStrengthDirection(pLowestStrengthValue As Integer) As Direction

        Dim MoveDirection As Direction
        'Create list of all map tiles with the lowest MyAntHill Strength (may be multiple tiles)
        Dim LowestMapTiles As New List(Of MapTile)

        Try
            For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
                If MapTile.GetMyAntHillStrength = pLowestStrengthValue Then
                    LowestMapTiles.Add(MapTile)
                End If
            Next

        Catch ex As Exception
            Throw New Exception("Error in 1 - " & ex.Message)
        End Try

        'Now we have a list of lowest adjacent map tiles. Choose one randomly.
        Dim ChosenAdjacentMapTile As MapTile
        Try

            If LowestMapTiles.Count > 0
                ChosenAdjacentMapTile = LowestMapTiles(_Random.Next Mod LowestMapTiles.Count)
            Else
                Throw New Exception("Lowest Map Tiles Count = 0!!!!, " & _ 
                                    "LowestStrengthValue=" & pLowestStrengthValue & ", " & _
                                    "CountLinkedTiles=" & CurrentMapTile.LinkedTiles.Length & ", " & _
                                    "0= " & CurrentMapTile.LinkedTiles(0).GetMyAntHillStrength & ", " & _
                                    "1= " & CurrentMapTile.LinkedTiles(1).GetMyAntHillStrength & ", " & _
                                    "2= " & CurrentMapTile.LinkedTiles(2).GetMyAntHillStrength & ", " & _
                                    "3= " & CurrentMapTile.LinkedTiles(3).GetMyAntHillStrength & ", ")
            End If
        Catch ex As Exception
            Throw New Exception("Error in 2 - " & ex.Message)
        End Try

        Try

            'Determine waht direction we must move to go to this tile
            For i As Integer = 0 To CurrentMapTile.LinkedTiles.Length - 1
                If CurrentMapTile.LinkedTiles(i).Equals(ChosenAdjacentMapTile) Then
                    MoveDirection = CType(i, Direction)
                    Exit For
                End If
            Next
        Catch ex As Exception
            Throw New Exception("Error in 3 - " & ex.Message)
        End Try

        Return MoveDirection
    End Function

#End Region

#Region "Move in Random Walk Logic"

    Private Function GetRandomWalkMoveDirection() As Direction
        Dim MoveDirection As Direction = Direction.w 'Set a default move direction....

        'Walk in a straight line until either the forward unit is not-passable, or we have walked in a straight line enough times
        '' to warrant turning in a random direction 90 degrees (180 degrees if we have to).

        If StepCount < STEPCOUNTTHRESHOLD And CheckIfPassable(PreviousDirection) And CurrentMapTile.LinkedTiles(PreviousDirection).AntHere = False Then
            MoveDirection = PreviousDirection   'Keep moving in a straight line, so move forward in previous direction
        Else 'Cannot keep going straight, need to turn left or right or turn around.

            'Perform a Mod 2 over a random number to get a 50-50 of a 0 or a 1 returned to represent a Boolean
            Dim ClockwiseTurn As Boolean = Convert.ToBoolean(_Random.Next Mod 2)
            MoveDirection = GetTurn90DegreeDirection(PreviousDirection, ClockwiseTurn)

            'Verify new direction is passable, If not, try other direction
            If Not CheckIfPassable(MoveDirection) Or CurrentMapTile.LinkedTiles(MoveDirection).AntHere = True Then
                MoveDirection = GetTurn90DegreeDirection(PreviousDirection, Not ClockwiseTurn)

                'Now, Verify is the New New direction is passable, if it isnt, then turn around from Previous direction
                If Not CheckIfPassable(MoveDirection)  Or CurrentMapTile.LinkedTiles(MoveDirection).AntHere = True Then
                    MoveDirection = GetTurn180DegreeDirection(PreviousDirection)
                End If
            End If

        End If

        Return MoveDirection
    End Function

#End Region

#Region "Misc Movement Helper Logic"

    Public Function CheckIfPassable(pDirection As Direction) As Boolean
        Return CurrentMapTile.LinkedTiles(pDirection).Passable
    End Function

    Private Function GetTurn90DegreeDirection(pCurrentDirection As Direction, pClockwiseTurn As Boolean) As Direction
        Dim NewDirection As Direction = PreviousDirection    'Set this to a value to prevent null ref 

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
        Dim NewDirection As Direction = PreviousDirection 'Set this to a value to prevent null ref

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

    Public Sub MoveAnt(pDirection As Direction)
        'Handle the step count to keep track of number of steps in a straight line
        If pDirection = PreviousDirection Then
            StepCount += 1                  'Increase step count because we walked in a straight line again
        Else
            StepCount = 0                   'Chagned direction, reset step count
            PreviousDirection = pDirection  'Set the new previous direction to keep track of steps in a straight line
        End If
        CurrentMapTile.AntHere = False
        CurrentMapTile = CurrentMapTile.LinkedTiles(pDirection)
        CurrentMapTile.AntHere = True
    End Sub

    Public Function GetAntLocation() As Location
        Return New Location(CurrentMapTile.Row, CurrentMapTile.Col)
    End Function

#End Region

End Class
