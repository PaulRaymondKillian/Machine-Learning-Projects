Option Strict On

Imports System.Collections.Generic
Imports System.Collections.Hashtable
Imports System.Collections

Public Interface IColonyAntMovementController
    Sub SetCurrentMapTile(pCurrentMapTile As MapTile)
    Function GetPreviousDirection As Direction
    Sub SetPreviousDirection(pDirection As Direction)
    Sub IncreaseStepCount()
    Sub ResetStepCount()

    Function NearAlert As Boolean
    Function NearAssignedFood As Boolean
    Function NearFood As Boolean
    Function NearMyAntHill As Boolean
    Function NearEnemyAntHill As Boolean
    Function NearEnemyAnt As Boolean
    Function NearMyAnt As Boolean
    Function CountMyAntsNearby As Integer
    Function CountEnemyAntsNearby As Integer

    Function GoTowardAlert As Direction
    Function GoTowardAssignedFood As Direction
    Function GoTowardClosestFood As Direction
    Function GoTowardMyAntHill As Direction
    Function GoTowardClosestEnemyAntHill As Direction
    Function GoTowardClosestEnemyAnt As Direction
    'Function GoTowardEnemyAnts As Direction
    'Function GoAwayFromEnemyAnts As Direction
    Function GoTowardMyAntImmediatePheromone As Direction
    'Function GoAwayFromMyAntImmediatePheromone As Direction
    Function GoTowardMyAntPersistentPheromone As Direction
    Function GoAwayFromMyAntPersistentPheromone As Direction
    'Function StayNearMyAntHill As Direction
    'Function GoTowardDefensePosition(pPosition As Integer) As Direction

    Function GoExplore As Direction
    Function GoCombat As Direction
    Function GoDefendAntHill As Direction
End Interface

Public Class ColonyAntMovementController : Implements IColonyAntMovementController
    Private CurrentMapTile As MapTile
    Public _PreviousDirection As Direction
    Public _StepCount As Integer
    Private _Random As Random

    Private _ThisColonyAnt As ColonyAnt

    Const STEPCOUNTTHRESHOLD As Integer = 30

    Public Sub New(pThisColonyAnt As ColonyAnt, pRandom As Random)
        _Random = pRandom
        _PreviousDirection = Direction.e
        _StepCount = 0
        _Random = pRandom
        _ThisColonyAnt = pThisColonyAnt
    End Sub

#Region "Getters & Setters"

    Public Sub SetCurrentMapTile(pCurrentMapTile As MapTile) Implements IColonyAntMovementController.SetCurrentMapTile
        CurrentMapTile = pCurrentMapTile
    End Sub

    Function GetPreviousDirection As Direction Implements IColonyAntMovementController.GetPreviousDirection
        Return _PreviousDirection
    End Function

    Sub SetPreviousDirection(pDirection As Direction) Implements IColonyAntMovementController.SetPreviousDirection
        _PreviousDirection = pDirection
    End Sub

    Sub IncreaseStepCount() Implements IColonyAntMovementController.IncreaseStepCount
        _StepCount += 1
    End Sub

    Sub ResetStepCount() Implements IColonyAntMovementController.ResetStepCount
        _StepCount = 0
    End Sub

#End Region

#Region "Near Ant Logic / Counts"

    Private Function OnAlertStrengthTile() As Boolean Implements IColonyAntMovementController.NearAlert
        If _ThisColonyAnt.CurrentMapTile.AlertStrength > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function OnAssignedFoodItemStrengthTile() As Boolean Implements IColonyAntMovementController.NearAssignedFood
        Dim FoodStrengthItemHere As Boolean
        If _ThisColonyAnt.AssignedFoodItemID = ""
            FoodStrengthItemHere = False
        Else
            If CInt(_ThisColonyAnt.CurrentMapTile.FoodStrengthHash(_ThisColonyAnt.AssignedFoodItemID)) > 0 Then
            FoodStrengthItemHere = True
        Else
            FoodStrengthItemHere = False
        End If
        End If
        Return FoodStrengthItemHere
    End Function

    Private Function OnFoodItemStrengthTile() As Boolean Implements IColonyAntMovementController.NearFood
        Dim FoodStrengthItemHere As Boolean
        If _ThisColonyAnt.CurrentMapTile.FoodStrengthHash.Count > 0 Then
            FoodStrengthItemHere = True
        Else
            FoodStrengthItemHere = False
        End If
        Return FoodStrengthItemHere
    End Function

    Private Function OnMyAntHillStrengthTile() As Boolean Implements IColonyAntMovementController.NearMyAntHill
        If _ThisColonyAnt.CurrentMapTile.MyAntHillStrength > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function OnEnemyAntHillStrengthTile() As Boolean Implements IColonyAntMovementController.NearEnemyAntHill
        Dim EnemyAntHillStrengthItemHere As Boolean
        If _ThisColonyAnt.CurrentMapTile.EnemyAntHillStrengthHash.Count > 0 Then
            EnemyAntHillStrengthItemHere = True
        Else
            EnemyAntHillStrengthItemHere = False
        End If
        Return EnemyAntHillStrengthItemHere
    End Function

    Private Function OnEnemyAntStrengthTile() As Boolean Implements IColonyAntMovementController.NearEnemyAnt
        If _ThisColonyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function NearMyAnt() As Boolean Implements IColonyAntMovementController.NearMyAnt
        Dim MyAntHillImmediateStrengthHere As Boolean
        'Subtract the current ant's pheromone from the current maptile to determine if another any is nearby - detect another ant's pheromone
        If _ThisColonyAnt.CurrentMapTile.MyAntImmediatePheromoneStrengthHash.Count > 1 Then
            MyAntHillImmediateStrengthHere = True
        Else
            MyAntHillImmediateStrengthHere = False
        End If

        Return MyAntHillImmediateStrengthHere
    End Function

    Public Function CountMyAntsNearby As Integer Implements IColonyAntMovementController.CountMyAntsNearby
        Return _ThisColonyAnt.CurrentMapTile.MyAntImmediatePheromoneStrengthHash.Count
    End Function
    
    Public Function CountEnemyAntsNearby As Integer Implements IColonyAntMovementController.CountEnemyAntsNearby
        Return _ThisColonyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Count
    End Function

    Public Function CountMyAntsNearby(pAtLeastPheromoneStrengthPercentage As Double) As Integer
        Dim CountMyAnts As Integer = 0
        For Each MyAntStrength As Integer In _ThisColonyAnt.CurrentMapTile.MyAntImmediatePheromoneStrengthHash.Values
            If MyAntStrength >= pAtLeastPheromoneStrengthPercentage * CInt(CurrentMapTile.MyAntImmediatePheromoneStrengthHash(CurrentMapTile.ID))
                CountMyAnts += 1
            End If
        Next
        Return CountMyAnts-1  'It will always include itself in this result, so remove it to get an accurate count.
    End Function

#End Region

End Class
