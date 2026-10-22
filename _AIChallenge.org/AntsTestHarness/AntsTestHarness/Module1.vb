Imports MyBot

Module Module1
    Dim Rows As Integer = 50
    Dim Cols As Integer = 50
    Dim Colony As MyBot.Colony
    Dim Map(Rows,Cols) As MyBot.Tile
    Dim FoodList As New List(Of MyBot.Location)
    Dim MyAntList As New List(Of MyBot.Location)
    Dim MyHillsList As New List(Of MyBot.Location)
    Dim EnemyHillsList As New List(Of MyBot.Location)
    Dim EnemyAntsList As New List(Of MyBot.Location)
    Dim ViewRadius As Integer = 7
    Sub Main()
        For row As Integer = 0 to Rows-1
            For col As Integer = 0 to Cols-1
                If (row = 10 and col >= 10 and col < 30) or _
                    (col = 20 and row > 10 and row < 20) or _
                    (col = 10 and row > 10 and row < 14)
                    Map(row, col) = Tile.WATER
                Else
                    Map(row, col) = Tile.LAND
                End If
            Next
        Next
        OutputMap
        Colony = New MyBot.Colony(Rows, Cols)

       ' MyHillsList.Add(New Location(13, 13))

        Dim Ant1Loc As New Location(17,14)
        Dim Ant2Loc As New Location(8,8)

        MyAntList.Add(Ant1Loc)
        'MyAntList.Add(Ant2Loc)

        EnemyHillsList.Add(New Location(22, 12))

        EnemyAntsList.Add(New Location(24, 14))
        EnemyAntsList.Add(New Location(20, 13))

        
        '
        '
        '
        'TIME UNIT 1
        '
        '
        '

        Colony.UpdateColony(Map, FoodList, MyAntList, MyHillsList, EnemyHillsList, EnemyAntsList, ViewRadius, 3)
        Dim MoveDirection As Direction = Colony._ColonyAnts(0).GetAntMoveDirection()
        OutputDataToFiles()


        'Dim MoveDirection As Direction
        'MoveDirection = Colony.GetAntMoveDirection()
        Dim Value As Boolean
        Value = Colony.GetAnt(Ant2Loc).MovementController.NearMyAntHill
        Value = Colony.GetAnt(Ant2Loc).MovementController.NearMyAnt
        Value = Colony.GetAnt(Ant2Loc).MovementController.NearEnemyAntHill
        Value = Colony.GetAnt(Ant2Loc).MovementController.NearEnemyAnt
        Value = Colony.GetAnt(Ant2Loc).MovementController.NearAlert
        Value = Colony.GetAnt(Ant2Loc).MovementController.NearFood


        '
        '
        '
        'TIME UNIT 2
        '
        '
        '
        EnemyAntsList.Clear
        EnemyAntsList.Add(New Location(29, 13))
        Colony.UpdateColony(Map, FoodList, MyAntList, MyHillsList, EnemyHillsList, EnemyAntsList, ViewRadius, 3)

        OutputDataToFiles()

    End Sub

    Public Sub OutputDataToFiles()

        Colony._Map.OutputMapEnemyAntPheromoneToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
        Colony._Map.OutputMapEnemyAntPheromoneToFile2("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
        Colony._Map.OutputMapEnemyAntPheromoneToFile3("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")

        Colony._Map.OutputMapAlertToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
        'For Each EnemyAnt As Location In EnemyAntsList
        '    Colony._Map.OutputMapEnemyAntPheromoneToFile("(" & EnemyAnt.Row & "," & EnemyAnt.Col & ")", _ 
        '                                                 "C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
        'Next
        Colony._Map.OutputMapMyAntPersistentPheromoneToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
        Colony._Map.OutputMapEnemyAntHillsToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\EnemyAntHills", EnemyHillsList)
        Colony._Map.OutputMapPassableToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\Map")
        Colony._Map.OutputMapMyAntHillStrengthToFile("C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\MyAntHillStrengthsMap")
        For Each MyAntLoc As Location In MyAntList
            Colony._Map.OutputMapMyAntImmediatePheromoneToFile("(" & MyAntLoc.Row & "," & MyAntLoc.Col & ")", _
                                                "C:\Killian Intelligence Systems\_AIChallenge.org\AntChallenge\")
        Next
    End Sub

    Public Sub OutputMap()
        For row As Integer = 0 to Rows-1
            For col As Integer = 0 to Cols-1
                If Map(row, col) = Tile.WATER
                    Console.Write("X")
                Else
                    Console.Write("-")
                End If
            Next
            Console.WriteLine
        Next
    End Sub

End Module
