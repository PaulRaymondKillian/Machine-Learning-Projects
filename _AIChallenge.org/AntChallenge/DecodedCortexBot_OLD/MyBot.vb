Imports System.Collections.Generic

Module MyBot

    Class MyBot
        Implements Bot

        Private _Random As Random = New Random(CInt(Now.Ticks Mod 10000000))
        Private _PreviousDir As Direction = Direction.e
        Private _MyColony as Colony

        Public Sub DoSetup(ByVal a As Ants) Implements Bot.DoSetup
            MsgBox("DoSetup")
            _MyColony = New Colony(a.Rows, a.Cols)  'Initialize the My Ant Colony object
        End Sub

        Public Sub DoTurn(ByVal a As Ants) Implements Bot.DoTurn
            '_MyColony.UpdateMapData(a.Map, a.FoodList)
            Try
                _MyColony.UpdateColony(a.Map, a.FoodList, a.MyAnts, a.MyHills)
            Catch ex As Exception
                'Console.WriteLine("!!!!!!!!!!!")
                'Console.WriteLine(ex.Message)
                'Console.WriteLine("!!!!!!!!!!!")
                MsgBox("1" & ex.Message)
            End Try
            
            'MsgBox("DoTurn")
            '_MyColony.UpdateFoodItems(a.FoodList.ToArray)
            For Each antLoc As Location In a.MyAnts()
                'Dim directions() As Direction = {Direction.n, Direction.e, Direction.s, Direction.w}
                'For Each d As Direction In directions
                '    Dim newLoc As Location = a.Destination(antLoc, d)
                '    If a.Passable(newLoc) Then
                '        a.IssueOrder(antLoc, d)
                '        Exit For
                '    End If
                'Next
                'a.IssueOrder(antLoc, Direction.w)


                'Dim newLoc As Location
                'Dim Dir As Direction
                'Dir = _PreviousDir
                'newLoc = a.Destination(antLoc, Dir)
                'If Not a.Passable(newLoc)
                '    Dim r As Integer = _Random.Next Mod 4
                '    Do
                '        r = _Random.Next Mod 4
                '        Select r
                '            Case 0:
                '                Dir = Direction.e
                '            Case 1:
                '                Dir = Direction.n
                '            Case 2:
                '                Dir = Direction.s
                '            Case Else:
                '                Dir = Direction.w
                '        End Select
                '      newLoc = a.Destination(antLoc, Dir) 
                '        _PreviousDir = Dir
                '    Loop Until a.Passable(newLoc)
                'End If
                'a.IssueOrder(antLoc, Dir)

                'Dim ColonyAnt As ColonyAnt = _MyColony.GetAnt(antLoc)
                'Dim moveDir As Direction = ColonyAnt.PreviousDirection
                'Dim newLoc As Location = a.Destination(antLoc, moveDir)
                'While Not a.Passable(newLoc)
                '    Dim r As Integer = _Random.Next Mod 4
                '    Select r
                '        Case 0:
                '            moveDir = Direction.e
                '        Case 1:
                '            moveDir = Direction.n
                '        Case 2:
                '            moveDir = Direction.s
                '        Case Else:
                '            moveDir = Direction.w
                '    End Select
                '    newLoc = a.Destination(antLoc, moveDir) 
                'End While
                '_MyColony.UpdateAnt(antLoc, newLoc, moveDir)
                'a.IssueOrder(antLoc, moveDir)

                'Dim directions() As Direction = {Direction.n, Direction.e, Direction.s, Direction.w}
                'For Each d As Direction In directions
                '    Dim newLoc As Location = a.Destination(antLoc, d)
                '    If a.Passable(newLoc) Then
                '        a.IssueOrder(antLoc, d)
                '        Exit For
                '    End If
                'Next
                'a.IssueOrder(antLoc, _MyColony.GetAntDirectionOrder(antLoc))
                Dim MoveDirection As Direction
                Try
                    MoveDirection = _MyColony.GetAntMoveDirection(antLoc)
                Catch ex As Exception
                    MsgBox("2" & ex.Message & ", InnerEx: " & ex.InnerException.Message)
                End Try
                Dim newLoc As Location = a.Destination(antLoc, MoveDirection)
                If a.Passable(newLoc) AndAlso Not _MyColony.AntExistAtLocation(newLoc) Then'AndAlso Not a.AntAtLocation(newLoc) Then
                    _MyColony.MoveAnt(antLoc, MoveDirection)
                    a.IssueOrder(antLoc, MoveDirection)
                End If

                'If a.TimeRemaining < 10 Then
                '    Exit For
                'End If
            Next
        End Sub

        Public Sub DoEnd(ByVal a As Ants) Implements Bot.DoEnd
        End Sub

    End Class

    Sub Main()
        Ants.Run(New MyBot())
    End Sub

End Module
