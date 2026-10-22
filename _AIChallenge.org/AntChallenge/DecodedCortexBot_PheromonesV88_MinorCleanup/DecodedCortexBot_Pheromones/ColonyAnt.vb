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

Public Enum ColonyAntMission
    Explorer
    PureExplorer
    AntHillDefender
    AntHillHunter
End Enum


Public Class ColonyAnt
    Public ID As String
    Public CurrentMapTile As MapTile
    Public Mission As ColonyAntMission
    Public MovementController As IColonyAntMovementController
    Public AssignedFoodItemID As String
    Public ExploreBearingDegree As Double
    Public CombatDirectionOrder As Direction
    Public GivenCombatOrder As Boolean
    Public DefenderReturnToAntHill As Boolean = False
    

    Public Sub New(pMission As ColonyAntMission, pCurrentMapTile As MapTile, pRandom As Random, pID As String, pExploreBearingDegree As Double, ByRef pMap As MapData)
        CurrentMapTile = pCurrentMapTile
        Mission = pMission
        MovementController = New ColonyAntMovementController(Me, pRandom, pMap)
        AssignedFoodItemID = ""
        ExploreBearingDegree = pExploreBearingDegree
        ID = pID
        GivenCombatOrder = False
        'MsgBox(ExploreBearingDegree)
    End Sub

    Public Function GetAntMoveDirection As Direction
        Dim MoveDirection As Direction

        MovementController.SetCurrentMapTile(CurrentMapTile)

        Select Case Mission
            Case ColonyAntMission.Explorer:
                MoveDirection = GetExplorerAssignedFoodMoveDirection()
            Case ColonyAntMission.PureExplorer:
                MsgBox("Warning! Not implemented yet")
            Case ColonyAntMission.AntHillDefender:
                MoveDirection = GetAntHillDefenderMoveDirection()
            Case ColonyAntMission.AntHillHunter:
                MsgBox("Warning! Not implemented yet")
        End Select

        Return MoveDirection
    End Function

    Public Sub MoveAnt(pDirection As Direction)
        'Handle the step count to keep track of number of steps in a straight line
        If pDirection = MovementController.GetPreviousDirection Then
            MovementController.IncreaseStepCount()                  'Increase step count because we walked in a straight line again
        Else
            MovementController.ResetStepCount()                   'Chagned direction, reset step count
            MovementController.SetPreviousDirection(pDirection)  'Set the new previous direction to keep track of steps in a straight line
        End If
        CurrentMapTile.AntHere = False
        CurrentMapTile = CurrentMapTile.LinkedTiles(pDirection)
        CurrentMapTile.AntHere = True
    End Sub

    Public Function GetAntLocation() As Location
        Return New Location(CurrentMapTile.Row, CurrentMapTile.Col)
    End Function

End Class
