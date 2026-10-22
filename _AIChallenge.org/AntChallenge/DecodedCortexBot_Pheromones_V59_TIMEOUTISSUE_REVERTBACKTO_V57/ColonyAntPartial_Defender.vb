Public Partial Class ColonyAnt

    Private Function GetDefenderMoveDirection() As Direction
        Dim MoveDirection As Direction
        'Walk away from MyAntHill until we get far enough away that we have no more guidance using
        '' the Strength hash table. Then, do a Random Walk.

        If MovementController.NearAlert
            MoveDirection = MovementController.GoDefendAntHill
        Else
            MoveDirection = MovementController.GoDefensePosition
        End If
           
        Return MoveDirection
    End Function

    'Private Function GetDefenderMoveDirection2() As Direction
    '    Dim MoveDirection As Direction

    '    If MovementController.NearMyAntHill
    '        MoveDirection = Movem
    '    End If

    '    Return MoveDirection
    'End Function


End Class
