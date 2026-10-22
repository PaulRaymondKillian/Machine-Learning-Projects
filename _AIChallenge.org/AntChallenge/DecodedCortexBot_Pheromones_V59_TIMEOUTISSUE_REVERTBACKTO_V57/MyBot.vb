Imports System.Collections.Generic

Module MyBot

    Class MyBot
        Implements Bot

        Private _MyColony as Colony

        Public Sub DoSetup(ByVal a As Ants) Implements Bot.DoSetup
            'MsgBox("DoSetup")
            _MyColony = New Colony(a.Rows, a.Cols)  'Initialize the My Ant Colony object
        End Sub

        Public Sub DoTurn(ByVal a As Ants) Implements Bot.DoTurn
            'Dim sw As New System.IO.StreamWriter("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\TurnTimers.txt",True)
            'Dim Timer As New System.Diagnostics.Stopwatch
            'Timer.Start
            Try
                _MyColony.UpdateColony(a.Map, a.FoodList, a.MyAnts, a.MyHills, a.EnemyHills, a.EnemyAnts, CInt(Math.Sqrt(a.ViewRadius2))+1)
                
                '_MyColony._Map.OutputMapAlertToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
                '_MyColony._Map.OutputMapEnemyAntPheromoneToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
                '_MyColony._Map.OutputMapMyAntPersistentPheromoneToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
                'For Each MyAntLoc As Location In a.MyAnts
                '    _MyColony._Map.OutputMapMyAntImmediatePheromoneToFile("(" & MyAntLoc.Row & "," & MyAntLoc.Col & ")", _
                '                                        "C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
                'Next
                '_MyColony._Map.OutputMapEnemyAntHillsToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\EnemyAntHills", _MyColony.GetActiveEnemyAntHillLocations(a.EnemyHills,False))
                '_MyColony._Map.OutputMapPassableToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\Map")

                For Each antLoc As Location In a.MyAnts()

                    Dim MoveDirection As Direction
                    MoveDirection = _MyColony.GetAntMoveDirection(antLoc)

                    Dim newLoc As Location = a.Destination(antLoc, MoveDirection)
                    If a.Passable(newLoc) AndAlso Not _MyColony.AntExistAtLocation(newLoc) Then
                        _MyColony.MoveAnt(antLoc, MoveDirection)
                        a.IssueOrder(antLoc, MoveDirection)
                    End If

                    'If a.TimeRemaining < 10 Then
                    '    'MsgBox("Ran Out Of Time!")
                    '    Exit For
                    'End If
                Next
            Catch ex As Exception
                MsgBox("MYBOT")
                MsgBox(ex.Message)
            End Try
            'Timer.Stop
            'sw.WriteLine(Timer.ElapsedMilliseconds)
            'sw.Close
        End Sub

        Public Sub DoEnd(ByVal a As Ants) Implements Bot.DoEnd
        End Sub

    End Class

    Sub Main()
        Ants.Run(New MyBot())
    End Sub

End Module
