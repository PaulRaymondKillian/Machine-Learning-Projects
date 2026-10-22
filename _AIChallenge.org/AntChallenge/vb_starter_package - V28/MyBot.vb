Imports System.Collections.Generic

Module MyBot

    Class MyBot
        Implements Bot

        'Private _Random As Random = New Random(CInt(Now.Ticks Mod 10000000))
        Private _MyColony as Colony

        Public Sub DoSetup(ByVal a As Ants) Implements Bot.DoSetup
            'MsgBox("DoSetup")
            _MyColony = New Colony(a.Rows, a.Cols)  'Initialize the My Ant Colony object
        End Sub

        Public Sub DoTurn(ByVal a As Ants) Implements Bot.DoTurn
            _MyColony.UpdateColony(a.Map, a.FoodList, a.MyAnts, a.MyHills)
            
            '_MyColony._Map.OutputMapPassableToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\Map.txt")
            For Each antLoc As Location In a.MyAnts()

                Dim MoveDirection As Direction
                MoveDirection = _MyColony.GetAntMoveDirection(antLoc)

                Dim newLoc As Location = a.Destination(antLoc, MoveDirection)
                If a.Passable(newLoc) AndAlso Not _MyColony.AntExistAtLocation(newLoc) Then'AndAlso Not a.AntAtLocation(newLoc) Then
                    _MyColony.MoveAnt(antLoc, MoveDirection)
                    a.IssueOrder(antLoc, MoveDirection)
                End If

                'Dim directions() As Direction = {Direction.n, Direction.e, Direction.s, Direction.w}
                'For Each d As Direction In directions
                '    Dim newLoc As Location = a.Destination(antLoc, d)
                '    If a.Passable(newLoc) Then
                '        a.IssueOrder(antLoc, d)
                '        Exit For
                '    End If
                'Next
                If a.TimeRemaining < 10 Then
                    'MsgBox("Ran Out Of Time!")
                    Exit For
                End If
            Next
        End Sub

        Public Sub DoEnd(ByVal a As Ants) Implements Bot.DoEnd
        End Sub

    End Class

    Sub Main()
        Ants.Run(New MyBot())
    End Sub

End Module
