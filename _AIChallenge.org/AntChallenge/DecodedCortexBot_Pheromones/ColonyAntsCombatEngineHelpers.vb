''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'' Decoded Cortex
'' www.DecodedCortex.com
''
'' Project: Ants - Google AI Challenge Fall 2011
'' Author: Paul Killian
'' Contact: PaulKillian@DecodedCortex.com
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Option Strict On

Imports System.Collections.Generic
Imports System.Collections.Hashtable
Imports System.Collections

Public Structure CombatAntsPerEnemyAntContainer
    Public EnemyAnt As String
    Public MyCombatAnts As List(Of ColonyAnt)
End Structure

Public Class CombatInfoContainer
    Public CombatAntsPerEnemyAnt As List(Of CombatAntsPerEnemyAntContainer)

    Public Sub New
        CombatAntsPerEnemyAnt = New List(Of CombatAntsPerEnemyAntContainer)
    End Sub

    Public Sub PopulateCombatAntsPerEnemyAntList(pCombatAnts As List(Of ColonyAnt))
        Dim CurrentContainer As CombatAntsPerEnemyAntContainer
        For Each MyAnt As ColonyAnt In pCombatAnts
            'For Each MapTile As MapTile In MyAnt.CurrentMapTile.LinkedTiles
                For Each EnemyAnt As String In MyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Keys'MapTile.EnemyAntPheromoneStrengthHash.Keys
                    CurrentContainer = GetContainerForEnemyAnt(EnemyAnt)
                    If Not CurrentContainer.MyCombatAnts.Contains(MyAnt)
                        CurrentContainer.MyCombatAnts.Add(MyAnt)
                    End If
                Next
            'Next
        Next
    End Sub

    Public Function GetContainerForEnemyAnt(pEnemyAnt As String) As CombatAntsPerEnemyAntContainer
        Dim SelectedContainer As New CombatAntsPerEnemyAntContainer
        Dim FoundContainer As Boolean = False
        For Each Container As CombatAntsPerEnemyAntContainer In CombatAntsPerEnemyAnt
            If Container.EnemyAnt = pEnemyAnt
                SelectedContainer = Container
                FoundContainer = True
                Exit For
            End If
        Next
        If Not FoundContainer
            SelectedContainer.EnemyAnt = pEnemyAnt
            SelectedContainer.MyCombatAnts = New List(Of ColonyAnt)
            CombatAntsPerEnemyAnt.Add(SelectedContainer)
        End If
        Return SelectedContainer
    End Function

    Public Sub OrderCombatAntsPerEnemyAntsListByClosestToEnemyAnt(pMap As MapData)
        Dim OrderedList As New List(Of CombatAntsPerEnemyAntContainer)
        Dim ClosestAntValue As Integer
        Dim ClosestAntContainer As New CombatAntsPerEnemyAntContainer 'Only newed this to get rid of warning message... i know...
        Dim DoneOrdering As Boolean = False
        Dim Distance As Integer

        Do
            ClosestAntValue = Integer.MaxValue
            For Each Container As CombatAntsPerEnemyAntContainer In CombatAntsPerEnemyAnt
                For Each MyAnt As ColonyAnt In Container.MyCombatAnts
                    For Each EnemyStrength As Integer In MyAnt.CurrentMapTile.EnemyAntPheromoneStrengthHash.Values
                        Distance = pMap._MAXENEMYANTPHEROMONESTRENGTH - EnemyStrength
                        If ClosestAntValue > Distance
                            ClosestAntValue = Distance
                            ClosestAntContainer = Container
                        End If
                    Next
                Next
            Next
            If ClosestAntValue < Integer.MaxValue
                OrderedList.Add(ClosestAntContainer)
                CombatAntsPerEnemyAnt.Remove(ClosestAntContainer)
            Else
                DoneOrdering = True
            End If
        Loop Until DoneOrdering

        CombatAntsPerEnemyAnt = OrderedList
    End Sub

    Public Sub OrderCombatAntsPerEnemyAntsListByClosestToMyAntHill()
        Dim OrderedListAll As New List(Of CombatAntsPerEnemyAntContainer)
        Dim OrderedListNearMyAntHill As New List(Of CombatAntsPerEnemyAntContainer)
        Dim OrderedListOthers As New List(Of CombatAntsPerEnemyAntContainer)
        Dim HighestMyAntHillValue As Integer
        Dim HighestMyAntHillContainer As New CombatAntsPerEnemyAntContainer 'Only newed this to get rid of warning message... i know...
        Dim DoneOrdering As Boolean = False

        Do
            HighestMyAntHillValue = Integer.MinValue
            For i As Integer = 0 to CombatAntsPerEnemyAnt.Count-1
                For Each MyAnt As ColonyAnt In CombatAntsPerEnemyAnt(i).MyCombatAnts
                    If MyAnt.CurrentMapTile.MyAntHillStrength > HighestMyAntHillValue
                        HighestMyAntHillValue = MyAnt.CurrentMapTile.MyAntHillStrength
                        HighestMyAntHillContainer = CombatAntsPerEnemyAnt(i)
                    End If
                Next
            Next
            If HighestMyAntHillValue > 0
                OrderedListNearMyAntHill.Add(HighestMyAntHillContainer)
                CombatAntsPerEnemyAnt.Remove(HighestMyAntHillContainer)
            ElseIf HighestMyAntHillValue = 0
                OrderedListOthers.Add(HighestMyAntHillContainer)
                CombatAntsPerEnemyAnt.Remove(HighestMyAntHillContainer)
            Else
                DoneOrdering = True
            End If
        Loop Until DoneOrdering

        'Now combine the two lists together
        For i As Integer = 0 to OrderedListNearMyAntHill.Count-1
            OrderedListAll.Add(OrderedListNearMyAntHill(i))
        Next
        For i As Integer = 0 to OrderedListOthers.Count-1
            OrderedListAll.Add(OrderedListOthers(i))
        Next

        CombatAntsPerEnemyAnt = OrderedListAll
    End Sub

    Public Sub OrderCombatAntsPerEnemyAntsListByMyAntCount()
        Dim OrderedList As New List(Of CombatAntsPerEnemyAntContainer)
        Dim HighestCount As Integer
        Dim HighestContainer As New CombatAntsPerEnemyAntContainer 'Only newed this to get rid of warning message... i know...
        Dim DoneOrdering As Boolean = False

        Do
            HighestCount = Integer.MinValue
            For Each Container As CombatAntsPerEnemyAntContainer In CombatAntsPerEnemyAnt
                If Container.MyCombatAnts.Count > HighestCount
                    HighestCount = Container.MyCombatAnts.Count
                    HighestContainer = Container
                End If
            Next
            If HighestCount > Integer.MinValue
                OrderedList.Add(HighestContainer)
                CombatAntsPerEnemyAnt.Remove(HighestContainer)
            Else
                DoneOrdering = True
            End If
        Loop Until DoneOrdering

        CombatAntsPerEnemyAnt = OrderedList
    End Sub

End Class
