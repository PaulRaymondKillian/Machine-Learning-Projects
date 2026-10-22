Public Partial Class ColonyAnt

    Private Function GetExplorerMoveDirection() As Direction
        Dim MoveDirection As Direction
        'Walk away from MyAntHill until we get far enough away that we have no more guidance using
        '' the Strength hash table. Then, do a Random Walk.

        If MovementController.NearAlert
            If MovementController.NearMyAntHill
                MoveDirection = MovementController.GoDefendAntHill
            Else
                If MovementController.NearEnemyAnt
                    MoveDirection = MovementController.GoCombat
                Else
                    MoveDirection = MovementController.GoDefendAntHill  
                End If
                'MoveDirection = MovementController.GoTowardClosestEnemyAnt 'Alternative Action... Dont know which is better
            End If
            'If MovementController.NearEnemyAnt
            '    'MoveDirection = MovementController.GoTowardEnemyAnts
            '    MoveDirection = MovementController.GoDefendAntHill
            'Else
            '    MoveDirection = MovementController.GoTowardAlert
            'End If
        ElseIf MovementController.NearMyAntHill
            If MovementController.NearEnemyAnt
                'MoveDirection = MovementController.GoTowardEnemyAnts
                MoveDirection = MovementController.GoDefendAntHill
            ElseIf MovementController.NearFood
                MoveDirection = MovementController.GoTowardClosestFood
            Else
                MoveDirection = MovementController.GoExplore
            End If
        ElseIf MovementController.NearFood
            If MovementController.NearEnemyAnt
                MoveDirection = MovementController.GoCombat
            Else
                MoveDirection = MovementController.GoTowardClosestFood
            End If
            'MoveDirection = MovementController.GoTowardClosestFood
        ElseIf MovementController.NearEnemyAnt
            MoveDirection = MovementController.GoCombat
        ElseIf MovementController.NearEnemyAntHill
            If MovementController.NearEnemyAnt
                MoveDirection = MovementController.GoCombat
            Else
                MoveDirection = MovementController.GoTowardClosestEnemyAntHill
            End If
        Else
            MoveDirection = MovementController.GoExplore
        End If
        
        Return MoveDirection
    End Function

    
    Private Function GetExplorerAssignedFoodMoveDirection() As Direction
        Dim MoveDirection As Direction
        'Walk away from MyAntHill until we get far enough away that we have no more guidance using
        '' the Strength hash table. Then, do a Random Walk.

        If MovementController.NearAlert
            If MovementController.NearMyAntHill
                MoveDirection = MovementController.GoTowardAlert
            Else
                If MovementController.NearEnemyAnt
                    MoveDirection = MovementController.GoCombat
                Else
                    MoveDirection = MovementController.GoTowardAlert 
                End If
                'MoveDirection = MovementController.GoTowardClosestEnemyAnt 'Alternative Action... Dont know which is better
            End If
            'If MovementController.NearEnemyAnt
            '    'MoveDirection = MovementController.GoTowardEnemyAnts
            '    MoveDirection = MovementController.GoDefendAntHill
            'Else
            '    MoveDirection = MovementController.GoTowardAlert
            'End If
        ElseIf MovementController.NearMyAntHill
            If MovementController.NearEnemyAnt  'This shouldnt happen because an alert should always be sounded...
                'MoveDirection = MovementController.GoTowardEnemyAnts
                MoveDirection = MovementController.GoDefendAntHill
            ElseIf MovementController.NearAssignedFood
                MoveDirection = MovementController.GoTowardAssignedFood
            Else
                MoveDirection = MovementController.GoExplore
            End If
        ElseIf MovementController.NearAssignedFood
            If MovementController.NearEnemyAnt
                MoveDirection = MovementController.GoCombat
            Else
                MoveDirection = MovementController.GoTowardAssignedFood
            End If
            'MoveDirection = MovementController.GoTowardClosestFood
        ElseIf MovementController.NearEnemyAnt
            MoveDirection = MovementController.GoCombat
        ElseIf MovementController.NearEnemyAntHill
            If MovementController.NearEnemyAnt
                MoveDirection = MovementController.GoCombat
            Else
                MoveDirection = MovementController.GoTowardClosestEnemyAntHill
            End If
        Else
            MoveDirection = MovementController.GoExplore
        End If
        
        Return MoveDirection
    End Function






















        Private Function GetExplorerMoveDirection2() As Direction
        Dim MoveDirection As Direction
        'Walk away from MyAntHill until we get far enough away that we have no more guidance using
        '' the Strength hash table. Then, do a Random Walk.

        If MovementController.NearAlert
            If MovementController.NearMyAntHill
                MoveDirection = MovementController.GoDefendAntHill
            ElseIf MovementController.NearEnemyAntHill
                MoveDirection = MovementController.GoTowardClosestEnemyAntHill
            Else
                'MoveDirection = MovementController.GoCombat
                MoveDirection = MovementController.GoTowardClosestEnemyAnt 'Alternative Action... Dont know which is better
            End If
            'If MovementController.NearEnemyAnt
            '    'MoveDirection = MovementController.GoTowardEnemyAnts
            '    MoveDirection = MovementController.GoDefendAntHill
            'Else
            '    MoveDirection = MovementController.GoTowardAlert
            'End If
        ElseIf MovementController.NearMyAntHill
            If MovementController.NearEnemyAntHill
                MoveDirection = MovementController.GoTowardClosestEnemyAntHill
            ElseIf MovementController.NearEnemyAnt
                'MoveDirection = MovementController.GoTowardEnemyAnts
                MoveDirection = MovementController.GoDefendAntHill
            ElseIf MovementController.NearFood
                MoveDirection = MovementController.GoTowardClosestFood
            Else
                MoveDirection = MovementController.GoExplore
            End If
        ElseIf MovementController.NearFood
            MoveDirection = MovementController.GoTowardClosestFood
        'ElseIf MovementController.NearEnemyAnt
        '    MoveDirection = MovementController.GoCombat
        ElseIf MovementController.NearEnemyAntHill
            MoveDirection = MovementController.GoTowardClosestEnemyAntHill
        Else
            MoveDirection = MovementController.GoExplore
        End If
        
        Return MoveDirection
    End Function


End Class
