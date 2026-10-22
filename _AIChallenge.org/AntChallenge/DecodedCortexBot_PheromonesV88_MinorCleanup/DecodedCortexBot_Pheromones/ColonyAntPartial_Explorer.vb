''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'' Decoded Cortex
'' www.DecodedCortex.com
''
'' Project: Ants - Google AI Challenge Fall 2011
'' Author: Paul Killian
'' Contact: PaulKillian@DecodedCortex.com
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Partial Class ColonyAnt
    
    Private Function GetExplorerAssignedFoodMoveDirection() As Direction
        Dim MoveDirection As Direction

        If MovementController.GivenCombatOrder
            MoveDirection = MovementController.GetCombatOrderDirection 
            MovementController.ResetCombatOrder
        ElseIf MovementController.NearAlert
            MoveDirection = MovementController.GoTowardAlert
        ElseIf MovementController.NearEnemyAntHill(5)
            MoveDirection = MovementController.GoTowardClosestEnemyAntHill
        ElseIf MovementController.NearAssignedFood
            MoveDirection = MovementController.GoTowardAssignedFood
        ElseIf MovementController.NearEnemyAntHill
            MoveDirection = MovementController.GoTowardClosestEnemyAntHill
        Else
            MoveDirection = MovementController.GoExplore
        End If
        
        Return MoveDirection
    End Function

End Class
