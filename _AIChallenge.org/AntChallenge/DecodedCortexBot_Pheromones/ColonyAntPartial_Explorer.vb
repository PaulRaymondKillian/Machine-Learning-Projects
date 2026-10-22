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

        If MovementController.NearEnemyAntHill(2) Or (MovementController.NearEnemyAntHill(10) AndAlso Not MovementController.NearEnemyAnt)
            MoveDirection = MovementController.GoTowardClosestEnemyAntHill
        ElseIf MovementController.GivenCombatOrder
            MoveDirection = MovementController.GetCombatOrderDirection 
            MovementController.ResetCombatOrder
        ElseIf MovementController.NearAlert
            MoveDirection = MovementController.GoTowardAlert
        ElseIf MovementController.NearEnemyAntHill(6)
            MoveDirection = MovementController.GoTowardClosestEnemyAntHill
        ElseIf MovementController.NearAssignedFood
            MoveDirection = MovementController.GoTowardAssignedFood
        ElseIf MovementController.NearEnemyAntHill AndAlso Not MovementController.NearMyAntHill
            MoveDirection = MovementController.GoTowardClosestEnemyAntHill
        Else
            MoveDirection = MovementController.GoExplore
        End If
        
        Return MoveDirection
    End Function

End Class
