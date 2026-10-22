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
    Const _PREDICTIONSINCELASTSEENHILL As Integer = 30
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
Try
        Dim EnemyAntHillLocations As New List(Of Location)  'Init the list but keep it empty to prevent possible null ref
        'Perform Enemy Ant Hill Maintainance Logic / Archival / Prediction
        If pEnemyAntHillLocations.Count = 0
            _TurnsSinceLastSeenEnemyAntHill += 1
            If CheckPredictEnemyAntHillLocationNow(pTurnCount, pColonyAnts.Count)
                EnemyAntHillLocations = PredictEnemyAntHillLocation(pColonyAnts, pColonyAntHills, pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap, pTurnCount)
                _TurnsSinceLastSeenEnemyAntHill = 0
            ElseIf pTurnCount < 50 AndAlso pEnemyAntLocations.Count > 0 AndAlso pEnemyAntLocations.Count < 4
                'If we are near the start of a game and see an enemy ant right away, attack it right away.
                'MsgBox("Found early ant at " & pEnemyAntLocations(0).Row & "," & pEnemyAntLocations(0).Col)
                For Each EnemyAnt As Location In pEnemyAntLocations
                    EnemyAntHillLocations.Add(New Location(EnemyAnt.Row, EnemyAnt.Col))
                Next
            End If
        Else
            EnemyAntHillLocations = pEnemyAntHillLocations
            _TurnsSinceLastSeenEnemyAntHill = 0
        End If

        Return EnemyAntHillLocations

Catch ex As Exception
    MsgBox("PREDICTION")
    Throw New Exception("Error in Prediction Engine: " & ex.Message)
End Try
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
        If pColonyAntHills.Count = 1
            If pArchivedEnemyAntHills.Count = 0
                PredictedEnemyAntHillLocation = GeneratePossibleEnemyAntHillLocations(pColonyAnts, pColonyAntHills(0), pDestroyedEnemyAntHills, pMap)
            Else
                PredictedEnemyAntHillLocation = GenerateExpectedEnemyAntHillLocations(pColonyAnts, pColonyAntHills(0), pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap)
            End If
        ElseIf pColonyAntHills.Count > 1
            'If pArchivedEnemyAntHills.Count = 0
            '    PredictedEnemyAntHillLocation = GenerateMultiPossibleEnemyAntHillLocations(pColonyAnts, pColonyAntHills, pDestroyedEnemyAntHills, pMap)
            'Else
            '    PredictedEnemyAntHillLocation = GenerateMultiExpectedEnemyAntHillLocations(pColonyAnts, pColonyAntHills, pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap)
            'End If

            'XXXXX TEMPORARY
            If pArchivedEnemyAntHills.Count = 0
                PredictedEnemyAntHillLocation = GeneratePossibleEnemyAntHillLocations(pColonyAnts, pColonyAntHills(0), pDestroyedEnemyAntHills, pMap)
            Else
                PredictedEnemyAntHillLocation = GenerateExpectedEnemyAntHillLocations(pColonyAnts, pColonyAntHills(0), pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap)
            End If
        End If

        'For Each Location As Location In PredictedEnemyAntHillLocation
        '    MsgBox(pTurnCount & " - (" & Location.Row & "," & Location.Col & ")")
        'Next

        Return PredictedEnemyAntHillLocation
    End Function

    Public Function GenerateExpectedEnemyAntHillLocations(pColonyAnts As List(Of ColonyAnt), pColonyAntHill As ColonyAntHill, _
                                                          pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                          pMap As MapData) As List(Of Location)
        Dim PredictedEnemyAntHillLocations As New List(Of Location)
        Dim DeltaRows, DeltaCols As Integer
        Dim DeltaZero As Boolean = False
        Dim MyAntHill As Location = New Location(pColonyAntHill._AntHill.Row, pColonyAntHill._AntHill.Col)

        'Determine if we have any archived enemy ant hills that have a delta of 0. Maps where the delta is 0 need to be handled special
        'because the enemy ant hills are arranged on the map in a different way
        For Each EnemyAntHill As Location In pArchivedEnemyAntHills
            DeltaRows = pColonyAntHill._AntHill.Col - EnemyAntHill.Col
            DeltaCols = pColonyAntHill._AntHill.Row - EnemyAntHill.Row
            If DeltaRows = 0 or DeltaCols = 0 Then
                DeltaZero = True
                Exit For
            End If
        Next

        If DeltaZero then
            PredictedEnemyAntHillLocations = PredictDeltaZeroAntHills(pColonyAnts, pColonyAntHill, pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap)
        Else
            PredictedEnemyAntHillLocations = PredictDiagonalAntHills(pColonyAnts, pColonyAntHill, pArchivedEnemyAntHills, pDestroyedEnemyAntHills, pMap)
        End If

        'If the predictions are invalid, the predicted hill list will be empty.
        If PredictedEnemyAntHillLocations.Count > 0
            'We should have a list of all predicted enemy ant hills now. 
            'Our next step is to remove all predicted enemy ant hills that are already known to be destroyed
            PredictedEnemyAntHillLocations = RemoveLocationsFromListOfLocations(PredictedEnemyAntHillLocations,pDestroyedEnemyAntHills)

            'We now have a list of predicted ant hills that have not been seen before. Choose the closest one to my ant hill and 
            'return it as our predicted enemy ant hill.

            'Calculate the closest valid (non-destroyed) ant hill to each of my ant hills
            Dim ClosestValidPredictedLocation As Location = ReturnClosestValidEnemyAntHillLocation(MyAntHill, PredictedEnemyAntHillLocations, pDestroyedEnemyAntHills, pMap)
            
            Dim ValidPredictedEnemyAntHillLocation As New List(Of Location)
            ValidPredictedEnemyAntHillLocation.Add(ClosestValidPredictedLocation)

            PredictedEnemyAntHillLocations = ValidPredictedEnemyAntHillLocation
        Else
            'Our calculate of expected enemy ant hills produced too many results and therefore we do not expect them to be correct or
            'accurate. Throw away these calculations and instead guess where the enemy ant hills may be...
            PredictedEnemyAntHillLocations = GeneratePossibleEnemyAntHillLocations(pColonyAnts, pColonyAntHill, pDestroyedEnemyAntHills, pMap)
        End If


        Return PredictedEnemyAntHillLocations 
       
    End Function

    Private Function PredictDeltaZeroAntHills(pColonyAnts As List(Of ColonyAnt), pColonyAntHill As ColonyAntHill, _
                                                          pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                          pMap As MapData) As List(Of Location)

        Dim PredictedEnemyAntHillLocations As New List(Of Location)
        Dim DeltaZeroEnemyAntHill As Location
        Dim DeltaCol, DeltaRow, Delta As Integer
        Dim MyAntHill As Location = New Location(pColonyAntHill._AntHill.Row, pColonyAntHill._AntHill.Col)

        'Find the Enemy Ant Hill that is Delta Zero to my Ant hill
        For Each EnemyAntHill As Location In pArchivedEnemyAntHills
            DeltaRow = EnemyAntHill.Row - pColonyAntHill._AntHill.Row
            DeltaCol = EnemyAntHill.Col - pColonyAntHill._AntHill.Col
            If DeltaRow = 0 or DeltaCol = 0
                DeltaZeroEnemyAntHill = EnemyAntHill
                Exit For
            End If
        Next

        'Set the delta value for both row and col that is not 0
        If DeltaRow > 0
            Delta = DeltaRow
        Else
            Delta = DeltaCol
        End If

        'Now, we have the ant hill that is Delta Zero to our ant hill.
        Dim CountHalfHillsVertical, CountHalfHillsHorizontal As Integer
        CountHalfHillsVertical= CInt((pMap._Cols/Delta)/2)
        CountHalfHillsHorizontal = CInt((pMap._Rows/Delta)/2)

        If CountHalfHillsVertical < 1
            CountHalfHillsVertical = 1
        End If
        If CountHalfHillsHorizontal < 1
            CountHalfHillsHorizontal = 1
        End If

        Dim EnemyAntHill1, EnemyAntHill2 As Location
        For i As Integer = 1 to CountHalfHillsVertical
            EnemyAntHill1 = New Location(MyAntHill.Row + (i*Delta), MyAntHill.Col)
            EnemyAntHill2 = New Location(MyAntHill.Row - (i*Delta), MyAntHill.Col)
            PredictedEnemyAntHillLocations.Add(EnemyAntHill1)
            PredictedEnemyAntHillLocations.Add(EnemyAntHill2)
        Next

        For i As Integer = 1 to CountHalfHillsHorizontal
            EnemyAntHill1 = New Location(MyAntHill.Row, MyAntHill.Col + (i*Delta))
            EnemyAntHill2 = New Location(MyAntHill.Row, MyAntHill.Col - (i*Delta))
            PredictedEnemyAntHillLocations.Add(EnemyAntHill1)
            PredictedEnemyAntHillLocations.Add(EnemyAntHill2)
        Next

        Return PredictedEnemyAntHillLocations
    End Function

    Private Function PredictDiagonalAntHills(pColonyAnts As List(Of ColonyAnt), pColonyAntHill As ColonyAntHill, _
                                                          pArchivedEnemyAntHills As List(Of Location), pDestroyedEnemyAntHills As List(Of Location), _ 
                                                          pMap As MapData) As List(Of Location)

        'If we have seen an enemy ant hill, use this logic to calculate where we should expect an ant hill to be.
        Dim PredictedEnemyAntHillLocations As New List(Of Location)
        Dim MyAntHill As Location = New Location(pColonyAntHill._AntHill.Row, pColonyAntHill._AntHill.Col)
        Dim RandomArchivedEnemyAntHill As Location = pArchivedEnemyAntHills(_Random.Next Mod pArchivedEnemyAntHills.Count)
        Dim DeltaRows As Integer = MyAntHill.Row - RandomArchivedEnemyAntHill.Row
        Dim DeltaCols As Integer = MyAntHill.Col - RandomArchivedEnemyAntHill.Col
        Dim CurrentPredictedEnemyAntHill As Location = MyAntHill
        Dim DonePredicting As Boolean = False
        Dim PredictionInvalid As Boolean = False

        Do
            CurrentPredictedEnemyAntHill.Row = GetValidRowIndex(CurrentPredictedEnemyAntHill.Row + DeltaRows, pMap)
            CurrentPredictedEnemyAntHill.Col = GetValidColIndex(CurrentPredictedEnemyAntHill.Col + DeltaCols, pMap)

            'If Not (CurrentPredictedEnemyAntHill.Row = MyAntHill.Row AndAlso CurrentPredictedEnemyAntHill.Col = MyAntHill.Col)
            If GetDistance(MyAntHill, CurrentPredictedEnemyAntHill, pMap) > 2
                PredictedEnemyAntHillLocations.Add(CurrentPredictedEnemyAntHill)
            ElseIf PredictedEnemyAntHillLocations.Count >= _MAXCOUNTPREDICTEDENEMYANTHILLSALLOWED Or DeltaCols = 0 or DeltaRows = 0
                PredictionInvalid = True
            Else
                DonePredicting = True    
            End If
        Loop Until DonePredicting Or PredictionInvalid

        'If the prediction is invalid, clear all predicted ant hills so we can then later generate "possible" ant hill locations
        If PredictionInvalid
            PredictedEnemyAntHillLocations.Clear
        End If

        Return PredictedEnemyAntHillLocations

    End Function

    Public Function GeneratePossibleEnemyAntHillLocations(pColonyAnts As List(Of ColonyAnt), pColonyAntHill As ColonyAntHill, _
                                                          pDestroyedEnemyAntHills As List(Of Location), pMap As MapData) As List(Of Location)
        Dim PredictedEnemyAntHillLocation As New List(Of Location)
        Dim PercentMapToCover As Double
        Dim PredictSingleLocation As Boolean = False
        Dim FoundValidPredictions As Boolean
        'If we have never seen an enemy ant hill yet, use this logic to predic where an enemy ant hill may be
            
        'Heuristically expect an ant hill to be a distance of perhaps XX% of the map away in both directions
        If pColonyAnts.Count < 50
            PercentMapToCover = 0.25
        ElseIf pColonyAnts.Count < 150
            PercentMapToCover = 0.4
        Else
            'We we have over 150 ants, just create a single location at the exact opposite side of the map from my ant hill location...
            PercentMapToCover = 0.5
            PredictSingleLocation = True
        End If
        Dim DeltaRows As Integer = CInt(pMap._Rows*PercentMapToCover)
        Dim DeltaCols As Integer = CInt(pMap._Cols*PercentMapToCover)

        'Calculate the 4 closest ant hills using the heuristically calculated delta distance values
        'Dim EnemyAntHill1 As New Location
        'EnemyAntHill1.Row = (_ColonyAntHills(0)._AntHill.Row + DeltaRows) Mod _Map._Rows
        'EnemyAntHill1.Col = (_ColonyAntHills(0)._AntHill.Col + DeltaCols) Mod _Map._Cols
        Do
            FoundValidPredictions = True    'Reset value to true, we will check if false at the end
            PredictedEnemyAntHillLocation.Clear 'Reset the list of predicted ant hills

            With pColonyAntHill._AntHill
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
