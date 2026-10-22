Option Strict On

Imports System.Collections.Generic

Public Enum ColonyAntMission
    Explorer
    PureExplorer
    AntHillDefender
    AntHillHunter
End Enum


Public Class ColonyAnt
    Public CurrentMapTile As MapTile
    Public Mission As ColonyAntMission
    Public MovementController As IColonyAntMovementController
    Public AssignedFoodItemID As String
    

    Public Sub New(pCurrentMapTile As MapTile, pRandom As Random)
        CurrentMapTile = pCurrentMapTile
        Mission = ColonyAntMission.Explorer
        MovementController = New ColonyAntMovementController(Me, pRandom)
        AssignedFoodItemID = ""
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
                MsgBox("Warning! Not implemented yet")
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
