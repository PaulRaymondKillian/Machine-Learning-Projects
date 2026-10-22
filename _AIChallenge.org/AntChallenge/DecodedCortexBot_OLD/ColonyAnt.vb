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

        Else
            'We are not near a good item, lets move away from the ant hill and explore,
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

#Region "Move Away From MyAntHill Logic"

    Private Function GetLowestAdjacentMyAntHillStrengthValue() As Integer
        'Find the lowest MyAntHill Strength of all adjacent tiles
        Dim LowestStrengthValue As Integer = Integer.MaxValue
        For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
            If MapTile.GetMyAntHillStrength < LowestStrengthValue And MapTile.Passable Then
                LowestStrengthValue = MapTile.GetMyAntHillStrength
            End If
        Next
        Return LowestStrengthValue
    End Function

    Private Function ChooseLowestAdjacentMyAntHillStrengthDirection(pLowestStrengthValue As Integer) As Direction
        Dim MoveDirection As Direction
        'Create list of all map tiles with the lowest MyAntHill Strength (may be multiple tiles)
        Dim LowestMapTiles As New List(Of MapTile)
        For Each MapTile As MapTile In CurrentMapTile.LinkedTiles
            If MapTile.GetMyAntHillStrength = pLowestStrengthValue Then
                LowestMapTiles.Add(MapTile)
            End If
        Next

        'Now we have a list of lowest adjacent map tiles. Choose one randomly.
        Dim ChosenAdjacentMapTile As MapTile
        If LowestMapTiles.Count > 0
            ChosenAdjacentMapTile = LowestMapTiles(_Random.Next Mod LowestMapTiles.Count)
        Else
            MsgBox("Warning! Zero LowestMapTiles!")
        End If

        'Determine waht direction we must move to go to this tile
        For i As Integer = 0 To CurrentMapTile.LinkedTiles.Length - 1
            If CurrentMapTile.LinkedTiles(i).Equals(ChosenAdjacentMapTile) Then
                MoveDirection = CType(i, Direction)
                Exit For
            End If
        Next
        Return MoveDirection
    End Function

#End Region

#Region "Move in Random Walk Logic"

    Private Function GetRandomWalkMoveDirection() As Direction
        Dim MoveDirection As Direction
        'Walk in a straight line until either the forward unit is not-passable, or we have walked in a straight line enough times
        '' to warrant turning in a random direction 90 degrees (180 degrees if we have to).

        If StepCount < STEPCOUNTTHRESHOLD And CheckIfPassable(PreviousDirection) Then
            MoveDirection = PreviousDirection   'Keep moving in a straight line, so move forward in previous direction
        Else 'Cannot keep going straight, need to turn left or right or turn around.

            'Perform a Mod 2 over a random number to get a 50-50 of a 0 or a 1 returned to represent a Boolean
            Dim ClockwiseTurn As Boolean = Convert.ToBoolean(_Random.Next Mod 2)
            MoveDirection = GetTurn90DegreeDirection(PreviousDirection, ClockwiseTurn)

            'Verify new direction is passable, If not, try other direction
            If Not CheckIfPassable(MoveDirection) Then
                MoveDirection = GetTurn90DegreeDirection(PreviousDirection, Not ClockwiseTurn)

                'Now, Verify is the New New direction is passable, if it isnt, then turn around from Previous direction
                If Not CheckIfPassable(MoveDirection) Then
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
        CurrentMapTile = CurrentMapTile.LinkedTiles(pDirection)
    End Sub

    Public Function GetAntLocation() As Location
        Return New Location(CurrentMapTile.Row, CurrentMapTile.Col)
    End Function

#End Region

End Class
