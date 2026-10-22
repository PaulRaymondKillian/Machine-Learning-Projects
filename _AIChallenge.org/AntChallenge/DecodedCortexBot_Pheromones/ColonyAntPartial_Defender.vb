''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'' Decoded Cortex
'' www.DecodedCortex.com
''
'' Project: Ants - Google AI Challenge Fall 2011
'' Author: Paul Killian
'' Contact: PaulKillian@DecodedCortex.com
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Partial Class ColonyAnt

    Private Function GetAntHillDefenderMoveDirection() As Direction
        Dim MoveDirection As Direction

        If MovementController.GivenCombatOrder
            MoveDirection = MovementController.GetCombatOrderDirection 
            MovementController.ResetCombatOrder
        ElseIf MovementController.NearAlert
            MoveDirection = MovementController.GoTowardAlert
        ElseIf MovementController.NearDefenderPosition
            MoveDirection = MovementController.GoTowardClosestDefenderPosition
        End If

        Return MoveDirection
    End Function

End Class
