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

Public Interface IColonyEnemyAntHillPredictionEngine
    Function ExecutePredictionEngine(pTurnCount As Integer, pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                pEnemyAntLocations As List(Of Location), pEnemyAntHillLocations As List(Of Location), _ 
                                                pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                pMap As MapData) As List(Of Location)
End Interface

Public Class ColonyEnemyAntHillPredictionEngine : Implements IColonyEnemyAntHillPredictionEngine

    Public _Random As Random
    Public _TurnsSinceLastSeenEnemyAntHill As Integer

    Const _PREDICTIONMINTURNCOUNT As Integer = 50
    Const _PREDICTIONSINCELASTSEENHILL As Integer = 50
    Const _PREDICTIONMINMYANTCOUNT As Integer = 15
    Const _MAXCOUNTPREDICTEDENEMYANTHILLSALLOWED As Integer = 15    'How many ant hills should be generated before we consider an error occurred


    Public Sub New(pRandom As Random)
        _Random = pRandom
        _TurnsSinceLastSeenEnemyAntHill = 0
    End Sub

    Public Function ExecutePredictionEngine(pTurnCount As Integer, pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                pEnemyAntLocations As List(Of Location), pEnemyAntHillLocations As List(Of Location), _ 
                                                pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                pMap As MapData) As List(Of Location) Implements IColonyEnemyAntHillPredictionEngine.ExecutePredictionEngine
        Dim EnemyAntHillLocations As New List(Of Location)  'Init the list but keep it empty to prevent possible null ref
        'Perform Enemy Ant Hill Maintainance Logic / Archival / Prediction
        If pEnemyAntHillLocations.Count = 0
            _TurnsSinceLastSeenEnemyAntHill += 1
            If CheckPredictEnemyAntHillLocationNow(pTurnCount, pColonyAnts.Count)
                EnemyAntHillLocations = PredictEnemyAntHillLocation(pColonyAnts, pColonyAntHills, pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap, pTurnCount)
                _TurnsSinceLastSeenEnemyAntHill = 0
                'MsgBox("Predictions: " & EnemyAntHillLocations.Count)
            ElseIf pTurnCount < 200 AndAlso pEnemyAntLocations.Count > 0 AndAlso pEnemyAntLocations.Count < 4
                'If we are near the start of a game and see an enemy ant right away, attack it right away.
                For Each EnemyAnt As Location In pEnemyAntLocations
                    EnemyAntHillLocations.Add(EnemyAnt)
                    'MsgBox("Predictions: " & EnemyAntHillLocations.Count)
                Next
            End If
        Else
            EnemyAntHillLocations = pEnemyAntHillLocations
            _TurnsSinceLastSeenEnemyAntHill = 0
        End If
        
        Return EnemyAntHillLocations
    End Function

    Public Function CheckPredictEnemyAntHillLocationNow(pTurnCount As Integer, pColonyAntCount As Integer) As Boolean
        If pTurnCount > _PREDICTIONMINTURNCOUNT AndAlso _TurnsSinceLastSeenEnemyAntHill > _PREDICTIONSINCELASTSEENHILL AndAlso pColonyAntCount > _PREDICTIONMINMYANTCOUNT
            Return True
        Else
            Return False
        End If
    End Function

    Public Function PredictEnemyAntHillLocation(pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _
                                                pMap As MapData, pTurnCount As Integer)  As List(Of Location)
        Dim PredictedEnemyAntHillLocation As New List(Of Location)

        'We must have at least 1 colony ant hill for this logic to work, we base our calcs off _ColonyAntHill(0) member of list
        If pColonyAntHills.Count > 0
            If pArchivedEnemyAntHills.Count = 0
                PredictedEnemyAntHillLocation = GeneratePossibleEnemyAntHillLocations(pColonyAnts, pColonyAntHills, pDestroyedEnemyAntHills, pMap)
            Else
                PredictedEnemyAntHillLocation = GenerateExpectedEnemyAntHillLocations(pColonyAnts, pColonyAntHills, pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap)
            End If
        End If

        Dim TempPredictedLocations As New List(Of Location)
        For Each Location As Location In PredictedEnemyAntHillLocation
            'MsgBox(pTurnCount & " - (" & Location.Row & "," & Location.Col & ")")
            TempPredictedLocations.Add(GetNearbyValidMapLocation(Location, pMap))
        Next

        'MsgBox("AntHillsPredicted")
        PredictedEnemyAntHillLocation = TempPredictedLocations

        Return PredictedEnemyAntHillLocation
    End Function

    Public Function GenerateExpectedEnemyAntHillLocations(pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                          pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                          pMap As MapData) As List(Of Location)
        'If we have seen an enemy ant hill, use this logic to calculate where we should expect an ant hill to be.
        Dim PredictedEnemyAntHillLocations As New List(Of Location)
        Dim MyAntHill As Location
        Dim RandomArchivedEnemyAntHill As Location = pArchivedEnemyAntHills(_Random.Next Mod pArchivedEnemyAntHills.Count)
        Dim DeltaRows As Integer
        Dim DeltaCols As Integer
        Dim DeltaZeroExists As Boolean = False

        For Each MyHill As ColonyAntHill In pColonyAntHills
            DeltaCols = Math.Abs(MyHill._AntHill.Col - RandomArchivedEnemyAntHill.Col)
            DeltaRows = Math.Abs(MyHill._AntHill.Row - RandomArchivedEnemyAntHill.Row)
            If DeltaCols <= 2 Or DeltaRows <= 2
                MyAntHill = New Location(MyHill._AntHill.Row, MyHill._AntHill.Col)
                DeltaZeroExists = True
                Exit For
            End If
        Next

        If DeltaZeroExists
            PredictedEnemyAntHillLocations = PredictEnemyAntHillLocation_DeltaZero(pColonyAnts, pColonyAntHills, pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap, RandomArchivedEnemyAntHill, MyAntHill)
        Else
            PredictedEnemyAntHillLocations = PredictEnemyAntHillLocation_Diagonal(pColonyAnts, pColonyAntHills, pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap)
        End If

        Return PredictedEnemyAntHillLocations
    End Function

    Private Function PredictEnemyAntHillLocation_DeltaZero(pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                          pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                          pMap As MapData, pChosenRandomArchivedHill As Location, pChosenMyHill As Location) As List(Of Location)
        Dim PredictedEnemyAntHillLocation As New List(Of Location)
        Dim MyAntHill As Location = pChosenMyHill
        Dim DeltaRows As Integer = MyAntHill.Row - pChosenRandomArchivedHill.Row
        Dim DeltaCols As Integer = MyAntHill.Col - pChosenRandomArchivedHill.Col
        Dim DeltaValue As Integer
        Dim CurrentPredictedEnemyAntHillN As Location
        Dim CurrentPredictedEnemyAntHillS As Location
        Dim CurrentPredictedEnemyAntHillE As Location
        Dim CurrentPredictedEnemyAntHillW As Location
        Dim DonePredicting As Boolean = False
        Dim PredictionInvalid As Boolean = False
        Dim CountLinearPredictions As Integer = 2

        'Set the Delta value to be used to calc ant hills in each direction, must be the non-zero delta value
        If Math.Abs(DeltaCols) < Math.Abs(DeltaRows)
            DeltaValue = Math.Abs(DeltaRows)
        Else
            DeltaValue = Math.Abs(DeltaCols)
        End If

        For Each AntHill As ColonyAntHill In pColonyAntHills
            MyAntHill = New Location(AntHill._AntHill.Row, AntHill._AntHill.Col)
            CurrentPredictedEnemyAntHillN = MyAntHill
            CurrentPredictedEnemyAntHillS = MyAntHill
            CurrentPredictedEnemyAntHillE = MyAntHill
            CurrentPredictedEnemyAntHillW = MyAntHill

            CurrentPredictedEnemyAntHillN.Row = GetValidRowIndex(CurrentPredictedEnemyAntHillN.Row + DeltaValue, pMap)
            CurrentPredictedEnemyAntHillS.Row = GetValidRowIndex(CurrentPredictedEnemyAntHillN.Row - DeltaValue, pMap)
            CurrentPredictedEnemyAntHillE.Col = GetValidColIndex(CurrentPredictedEnemyAntHillN.Col + DeltaValue, pMap)
            CurrentPredictedEnemyAntHillW.Col = GetValidColIndex(CurrentPredictedEnemyAntHillN.Col - DeltaValue, pMap)

            PredictedEnemyAntHillLocation.Add(CurrentPredictedEnemyAntHillN)
            PredictedEnemyAntHillLocation.Add(CurrentPredictedEnemyAntHillS)
            PredictedEnemyAntHillLocation.Add(CurrentPredictedEnemyAntHillE)
            PredictedEnemyAntHillLocation.Add(CurrentPredictedEnemyAntHillW)
        Next

        'Verify the predicted hills are not too close to the my ant hill, if they are too close, remove it from the list
        Dim TempFilteredPredicted As New List(Of Location)
        Dim PredictedTooClose As Boolean
        For Each PredictedHill As Location In PredictedEnemyAntHillLocation
            PredictedTooClose = False
            For Each AntHill As ColonyAntHill In pColonyAntHills
                If GetDistance(PredictedHill, New Location(AntHill._AntHill.Row, AntHill._AntHill.Col), pMap) < 10
                    PredictedTooClose = True
                    Exit For
                End If
            Next
            If Not PredictedTooClose 
                TempFilteredPredicted.Add(PredictedHill)
            End If
        Next
        PredictedEnemyAntHillLocation = TempFilteredPredicted

        'We should have a list of all predicted enemy ant hills now. 
        'Our next step is to remove all predicted enemy ant hills that are already known to be destroyed
        PredictedEnemyAntHillLocation = RemoveLocationsFromListOfLocations(PredictedEnemyAntHillLocation,pDestroyedEnemyAntHills)


        If PredictedEnemyAntHillLocation.Count = 0
            PredictedEnemyAntHillLocation = GeneratePossibleEnemyAntHillLocations(pColonyAnts, pColonyAntHills, pDestroyedEnemyAntHills, pMap)
        End If

        Return PredictedEnemyAntHillLocation
    End Function

    Private Function PredictEnemyAntHillLocation_Diagonal(pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                          pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                          pMap As MapData) As List(Of Location)
        Dim PredictedEnemyAntHillLocation As New List(Of Location)
        Dim MyAntHill As Location = New Location(pColonyAntHills(0)._AntHill.Row, pColonyAntHills(0)._AntHill.Col)
        Dim RandomArchivedEnemyAntHill As Location = pArchivedEnemyAntHills(_Random.Next Mod pArchivedEnemyAntHills.Count)
        Dim DeltaRows As Integer = MyAntHill.Row - RandomArchivedEnemyAntHill.Row
        Dim DeltaCols As Integer = MyAntHill.Col - RandomArchivedEnemyAntHill.Col
        Dim CurrentPredictedEnemyAntHill As Location = MyAntHill
        Dim DonePredicting As Boolean = False
        Dim PredictionInvalid As Boolean = False

        Do
            CurrentPredictedEnemyAntHill.Row = GetValidRowIndex(CurrentPredictedEnemyAntHill.Row + DeltaRows, pMap)
            CurrentPredictedEnemyAntHill.Col = GetValidColIndex(CurrentPredictedEnemyAntHill.Col + DeltaCols, pMap)

            If GetDistance(MyAntHill, CurrentPredictedEnemyAntHill, pMap) > 10
                PredictedEnemyAntHillLocation.Add(CurrentPredictedEnemyAntHill)
            ElseIf PredictedEnemyAntHillLocation.Count >= _MAXCOUNTPREDICTEDENEMYANTHILLSALLOWED Or DeltaCols = 0 or DeltaRows = 0
                PredictionInvalid = True
            Else
                DonePredicting = True    
            End If
        Loop Until DonePredicting Or PredictionInvalid

        If Not PredictionInvalid
            'We should have a list of all predicted enemy ant hills now. 
            'Our next step is to remove all predicted enemy ant hills that are already known to be destroyed
            PredictedEnemyAntHillLocation = RemoveLocationsFromListOfLocations(PredictedEnemyAntHillLocation,pDestroyedEnemyAntHills)

            'We now have a list of predicted ant hills that have not been seen before. Choose the closest one to my ant hill and 
            'return it as our predicted enemy ant hill.
            Dim ValidPredictedEnemyAntHillLocation As New List(Of Location)
            For Each ColonyAntHill As ColonyAntHill In pColonyAntHills
                MyAntHill = New Location(ColonyAntHill._AntHill.Row, ColonyAntHill._AntHill.Col)
                'Calculate the closest valid (non-destroyed) ant hill to each of my ant hills
                Dim ClosestValidPredictedLocation As Location = ReturnClosestValidEnemyAntHillLocation(MyAntHill, PredictedEnemyAntHillLocation, pDestroyedEnemyAntHills, pMap)
                
                'Check if the calculated ant hill already exists in the predicted list, if not, add it
                If Not ValidPredictedEnemyAntHillLocation.Contains(ClosestValidPredictedLocation)
                    ValidPredictedEnemyAntHillLocation.Add(ClosestValidPredictedLocation)
                End If
            Next
            PredictedEnemyAntHillLocation = ValidPredictedEnemyAntHillLocation
        Else
            'Our calculate of expected enemy ant hills produced too many results and therefore we do not expect them to be correct or
            'accurate. Throw away these calculations and instead guess where the enemy ant hills may be...
            PredictedEnemyAntHillLocation = GeneratePossibleEnemyAntHillLocations(pColonyAnts, pColonyAntHills, pDestroyedEnemyAntHills, pMap)
        End If

        Return PredictedEnemyAntHillLocation
    End Function

    Public Function GeneratePossibleEnemyAntHillLocations(pColonyAnts As List(Of ColonyAnt), pColonyAntHills As List(Of ColonyAntHill), _
                                                          pDestroyedEnemyAntHills As List(Of Location), pMap As MapData) As List(Of Location)
        Dim PredictedEnemyAntHillLocation As New List(Of Location)
        Dim PercentMapToCover As Double
        Dim PredictSingleLocation As Boolean = False
        Dim FoundValidPredictions As Boolean
        'If we have never seen an enemy ant hill yet, use this logic to predic where an enemy ant hill may be
            
        'Heuristically expect an ant hill to be a distance of perhaps XX% of the map away in both directions
        If pColonyAnts.Count < 50
            PercentMapToCover = 0.25
        ElseIf pColonyAnts.Count < 75
            PercentMapToCover = 0.35
        ElseIf pColonyAnts.Count < 100
            PercentMapToCover = 0.4
        Else
            'We we have over 150 ants, just create a single location at the exact opposite side of the map from my ant hill location...
            PercentMapToCover = 0.5
            PredictSingleLocation = True
        End If
        Dim DeltaRows As Integer = CInt(pMap._Rows*PercentMapToCover)
        Dim DeltaCols As Integer = CInt(pMap._Cols*PercentMapToCover)

        'Calculate the closest ant hills using the heuristically calculated delta distance values
        Do
            FoundValidPredictions = True    'Reset value to true, we will check if false at the end
            PredictedEnemyAntHillLocation.Clear 'Reset the list of predicted ant hills

            For Each MyColonyAntHill As ColonyAntHill In pColonyAntHills
                With MyColonyAntHill._AntHill
                    If PredictSingleLocation
                        'Only generate a single enemy ant hill location on the exact opposite point of the map...
                        PredictedEnemyAntHillLocation.Add(GetNearbyValidMapLocation( _
                                                            New Location(GetValidRowIndex(.Row + DeltaRows, pMap), _
                                                                        GetValidColIndex(.Col + DeltaCols, pMap)), pMap))
                    Else
                        'Only generate two predicted enemy ant hill locations, in opposite directions of each other.
                        If( _Random.Next Mod 2 = 0)
                            PredictedEnemyAntHillLocation.Add(GetNearbyValidMapLocation( _
                                                            New Location(GetValidRowIndex(.Row + DeltaRows, pMap), _
                                                                        GetValidColIndex(.Col + DeltaCols, pMap)), pMap))
                
                            PredictedEnemyAntHillLocation.Add(GetNearbyValidMapLocation( _ 
                                                            New Location(GetValidRowIndex(.Row - DeltaRows, pMap), _
                                                                        GetValidColIndex(.Col - DeltaCols, pMap)), pMap))
                        Else
                            PredictedEnemyAntHillLocation.Add(GetNearbyValidMapLocation( _
                                                            New Location(GetValidRowIndex(.Row - DeltaRows, pMap), _
                                                                        GetValidColIndex(.Col + DeltaCols, pMap)), pMap))
                
                            PredictedEnemyAntHillLocation.Add(GetNearbyValidMapLocation( _
                                                            New Location(GetValidRowIndex(.Row + DeltaRows, pMap), _
                                                                        GetValidColIndex(.Col - DeltaCols, pMap)), pMap))
                        End If
                    End If
                End With
            Next

            'Now we calculated our predicted possible enemy ant hill locations. Lets make sure they are valid. If not, re-guess new locs
            For Each PredictedLocation As Location In PredictedEnemyAntHillLocation
                If pDestroyedEnemyAntHills.Contains(PredictedLocation)
                    'MsgBox("Invalid enemy ant hill predicted")
                    FoundValidPredictions = False
                    DeltaCols = CInt(DeltaCols * 1.1)   'Push the guessed enemy ant hill out further by 10%
                    DeltaRows = CInt(DeltaRows * 1.1)   'Push the guessed enemy ant hill out further by 10%
                End If
            Next
        Loop Until FoundValidPredictions

        Return PredictedEnemyAntHillLocation
    End Function



    Public Function GetValidColShortestDistance(pCol1 As Integer, pCol2 As Integer, pMap As MapData) As Integer
        Dim ShortestDistance As Integer
        Dim Dist1, Dist2 As Integer
        'This function is necessary because you cannot just do a simple subtraction to find the distance between two points.
        'Because the map is a continuum torus, we must look off the edges of the map to determine which direction is shortest.
        Dist1 = Math.Abs(pCol1 - pCol2)
        Dist2 = Math.Abs(Dist1 - pMap._Cols)

        If Dist1 < Dist2 Then
            ShortestDistance = Dist1
        Else
            ShortestDistance = Dist2
        End If

        Return ShortestDistance
    End Function

    Public Function GetValidRowShortestDistance(pRow1 As Integer, pRow2 As Integer, pMap As MapData) As Integer
        Dim ShortestDistance As Integer
        Dim Dist1, Dist2 As Integer
        'This function is necessary because you cannot just do a simple subtraction to find the distance between two points.
        'Because the map is a continuum torus, we must look off the edges of the map to determine which direction is shortest.
        Dist1 = Math.Abs(pRow1 - pRow2)
        Dist2 = Math.Abs(Dist1 - pMap._Rows)

        If Dist1 < Dist2 Then
            ShortestDistance = Dist1
        Else
            ShortestDistance = Dist2
        End If

        Return ShortestDistance
    End Function

    Public Function ReturnClosestValidEnemyAntHillLocation(pSourceLocation As Location, pListOfEnemyAntHills As List(Of Location), _
                                                           pDestroyedEnemyAntHills As List(Of Location), pMap As MapData) As Location
        Dim ClosestValidLocation As Location
        Dim CurrentDistance As Integer
        Dim LowestDistance As Integer = Integer.MaxValue

        For Each Location As Location In pListOfEnemyAntHills
            If Not pDestroyedEnemyAntHills.Contains(Location)
                CurrentDistance = GetDistance(pSourceLocation, Location, pMap)
                If CurrentDistance < LowestDistance
                    LowestDistance = CurrentDistance
                    ClosestValidLocation = Location
                End If
            End If
        Next

        Return ClosestValidLocation
    End Function

    Public Function ReturnClosestLocation(pSourceLocation As Location, pListOfLocs As List(Of Location), pMap As MapData) As Location
        Dim ClosestLocation As Location
        Dim CurrentDistance As Integer
        Dim LowestDistance As Integer = Integer.MaxValue

        For Each Location As Location In pListOfLocs
            CurrentDistance = GetDistance(pSourceLocation, Location, pMap)
            If CurrentDistance < LowestDistance
                LowestDistance = CurrentDistance
                ClosestLocation = Location
            End If
        Next

        Return ClosestLocation
    End Function

    Public Function GetDistance(pLocation1 As Location, pLocation2 As Location, pMap As MapData) As Integer
        Return CInt(Math.Sqrt(Math.Pow(GetValidRowShortestDistance(pLocation1.Row, pLocation2.Row, pMap), 2) + _ 
                              Math.Pow(GetValidColShortestDistance(pLocation1.Col, pLocation2.Col, pMap), 2)))
    End Function

    Public Function RemoveLocationsFromListOfLocations(pSourceLocations As List(Of Location), pLocsToRemove As List(Of Location)) As List(Of Location)
        For Each LocToRemove As Location In pLocsToRemove
            If pSourceLocations.Contains(LocToRemove)
                pSourceLocations.Remove(LocToRemove)
            End If
        Next
        Return pSourceLocations
    End Function

    Public Function GetValidRowIndex(pRowIndex As Integer, pMap As MapData) As Integer
        Dim RowIndex As Integer = pRowIndex

        Do Until RowIndex >= 0 AndAlso RowIndex < pMap._Rows
            If RowIndex < 0
                RowIndex = pMap._Rows + RowIndex    'Row index is negative, so it will essentially subtract from _Map._Rows
            End If
            RowIndex = RowIndex Mod pMap._Rows
        Loop

        Return RowIndex
    End Function

    Public Function GetValidColIndex(pColIndex As Integer, pMap As MapData) As Integer
        Dim ColIndex As Integer = pColIndex

        Do Until ColIndex >= 0 AndAlso ColIndex < pMap._Cols
            If ColIndex < 0
                ColIndex = pMap._Cols + ColIndex    'Col index is negative, so it will essentially subtract from _Map._Rows
            End If
            ColIndex = ColIndex Mod pMap._Cols
        Loop

        Return ColIndex
    End Function
                                                
    Public Function GetNearbyValidMapLocation(pLocation As Location, pMap As MapData) As Location
        Dim MapTile As MapTile = pMap.GetMapTile(pLocation.Row, pLocation.Col)
        Dim Turn As Boolean = False
        Dim PossibleErrorCounter As Integer = 0 'This is to prevent an infite loop if that were to somehow be possible on a specific map
        Do Until MapTile.Passable Or PossibleErrorCounter > 100
            If Turn
                MapTile = MapTile.LinkedTiles(CInt(Direction.w))
            Else
                MapTile = MapTile.LinkedTiles(CInt(Direction.n))
            End If
            PossibleErrorCounter += 1
        Loop
        Return New Location(MapTile.Row, MapTile.Col)
    End Function
    

End Class
