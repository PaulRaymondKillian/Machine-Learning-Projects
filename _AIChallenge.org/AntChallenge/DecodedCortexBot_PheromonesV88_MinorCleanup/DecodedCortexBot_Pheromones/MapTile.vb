''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'' Decoded Cortex
'' www.DecodedCortex.com
''
'' Project: Ants - Google AI Challenge Fall 2011
'' Author: Paul Killian
'' Contact: PaulKillian@DecodedCortex.com
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Imports System.Collections.Generic
Imports System.Collections.Hashtable
Imports System.Collections

Public Class MapTile
    Public ID As String
    Public Row, Col As Integer
    Public Passable As Boolean
    Public Unseen As Boolean
    Public AntHere As Boolean
    Public FoodStrengthHash As Hashtable
    Public EnemyAntHillStrengthHash As Hashtable
    Public MyAntHillStrength As Integer
    Public MyAntPersistentPheromoneStrength As Integer
    Public MyAntImmediatePheromoneStrengthHash As Hashtable
    Public MyAntSinglePersistentPheromoneStrength As Hashtable
    Public MyAntAttractionStrengthHash As Hashtable
    Public EnemyAntPheromoneStrengthHash As Hashtable
    Public AlertStrength As Integer
    Public DefenderAntPositionStrength As Integer

    '
    '
    '
    Public _MyAntEdgeTileList As List(Of MapTile)

    Private Const _CORNERPHEROMONEMULTIPLIER As Integer = 2
    'Public Const _MAXALLOWEDMYANTPERSISTENTPHEROMONESTRENGTH As Integer = 50

    'Linked tiles are designed to be retrieved from the array using the "Direction" enumeration
    'Direction.n = 0, Direction.s = 1, Direction.e = 2, Direction.w = 3
    Public LinkedTiles(3) As MapTile
    Public CircleTiles(8) As MapTile

    Public Sub New(pRow As Integer, pCol As Integer, pMAXMYANTPHEROMONESTRENGTH As Integer)
        Row = pRow
        Col = pCol
        ID = "(" & Row & "," & Col & ")"
        Passable = False
        Unseen = True
        AntHere = False
        FoodStrengthHash = New Hashtable
        EnemyAntHillStrengthHash = New Hashtable
        MyAntHillStrength = 0
        MyAntPersistentPheromoneStrength = 0
        MyAntImmediatePheromoneStrengthHash = New Hashtable
        MyAntAttractionStrengthHash = New Hashtable
        MyAntSinglePersistentPheromoneStrength = New Hashtable
        EnemyAntPheromoneStrengthHash = New Hashtable
        _MyAntEdgeTileList = New List(Of MapTile)
    End Sub

    Public Sub SetCircleTiles()
        CircleTiles(0) = LinkedTiles(Direction.n)
        CircleTiles(1) = LinkedTiles(Direction.n).LinkedTiles(Direction.e)
        CircleTiles(2) = LinkedTiles(Direction.e)
        CircleTiles(3) = LinkedTiles(Direction.e).LinkedTiles(Direction.s)
        CircleTiles(4) = LinkedTiles(Direction.s)
        CircleTiles(5) = LinkedTiles(Direction.s).LinkedTiles(Direction.w)
        CircleTiles(6) = LinkedTiles(Direction.w)
        CircleTiles(7) = LinkedTiles(Direction.w).LinkedTiles(Direction.n)

    End Sub


#Region "Manage Food Strength Hash"

    Private Sub AddFoodStrengthItem(pID As String, pStrength As Integer)
        FoodStrengthHash.Add(pID, pStrength)
    End Sub

    Public Function GetFoodStrengthItem(pID As String) As Integer
        Dim FoodItemStrength As Integer = 0
        If Not FoodStrengthHash.Item(pID) Is Nothing
            FoodItemStrength = CInt(FoodStrengthHash.Item(pID))
        End If
        Return FoodItemStrength
    End Function

    Public Sub ClearFoodStrengthItems()
        FoodStrengthHash.Clear
    End Sub

    'Recursively go through each MapTile and set its FoodStrength value, and set a diminished FoodStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetFoodTileStrength(pID As String, pStrength As Integer)
        If pStrength > 0
            If FoodStrengthHash.Item(pID) Is Nothing    'If Does Not Exist in HashTable
                AddFoodStrengthItem(pID, pStrength)     'Add new value
            ElseIf GetFoodStrengthItem(pID) < pStrength 'If already exists, but strength value is less than inputed
                FoodStrengthHash.Item(pID) = pStrength  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable                                    'Only do tiles available in a path
                    If AdjacentTile.GetFoodStrengthItem(pID) < pStrength-1  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetFoodTileStrength(pID, pStrength-1)  
                    End If
                End If
            Next
        End If
    End Sub

#End Region

#Region "Manage MyAntHill Strength Hash"

    Private Sub SetMyAntHillStrength(pStrength As Integer)
        MyAntHillStrength = pStrength
    End Sub

    Public Function GetMyAntHillStrength() As Integer
        Return MyAntHillStrength
    End Function

    Public Sub ClearMyAntHillStrengthItems()
        MyAntHillStrength = 0
    End Sub

    'Recursively go through each MapTile and set its MyAntHillStrength value, and set a diminished MyAntHillStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetMyAntHillTileStrength(pStrength As Integer)
        If pStrength > 0 Then
            If GetMyAntHillStrength() < pStrength Then 'If already exists, but strength value is less than inputed
                SetMyAntHillStrength(pStrength)  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In CircleTiles'LinkedTiles
                If AdjacentTile.Passable Then                                    'Only do tiles available in a path
                    If AdjacentTile.GetMyAntHillStrength < pStrength - 1 Then  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetMyAntHillTileStrength(pStrength - 1)
                    End If
                End If
            Next
        End If
    End Sub

#End Region

#Region "Manage Enemy Ant Hill Hash"

    Private Sub AddEnemyAntHillStrengthItem(pID As String, pStrength As Integer)
        EnemyAntHillStrengthHash.Add(pID, pStrength)
    End Sub

    Public Function GetEnemyAntHillStrengthItem(pID As String) As Integer
        Dim EnemyAntHillStrength As Integer = 0
        If Not EnemyAntHillStrengthHash.Item(pID) Is Nothing
            EnemyAntHillStrength = CInt(EnemyAntHillStrengthHash.Item(pID))
        End If
        Return EnemyAntHillStrength
    End Function

    Public Sub ClearEnemyAntHillStrengthItems()
        EnemyAntHillStrengthHash.Clear
    End Sub

    'Recursively go through each MapTile and set its FoodStrength value, and set a diminished FoodStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetEnemyAntHillTileStrength(pID As String, pStrength As Integer)
        If pStrength > 0
            If EnemyAntHillStrengthHash.Item(pID) Is Nothing    'If Does Not Exist in HashTable
                AddEnemyAntHillStrengthItem(pID, pStrength)     'Add new value
            ElseIf GetEnemyAntHillStrengthItem(pID) < pStrength 'If already exists, but strength value is less than inputed
                EnemyAntHillStrengthHash.Item(pID) = pStrength  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable                                    'Only do tiles available in a path
                    If AdjacentTile.GetEnemyAntHillStrengthItem(pID) < pStrength-1  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetEnemyAntHillTileStrength(pID, pStrength-1)  
                    End If
                End If
            Next
        End If
    End Sub

#End Region

#Region "Manage My Ant Pheromone Logic/ Immediate Hash"

    Private Sub AddImmediatePheromoneStrength(pID As String, pStrength As Integer)
        MyAntImmediatePheromoneStrengthHash.Add(pID, pStrength)
    End Sub

    Public Function GetImmediatePheromoneStrength(pID As String) As Integer
        Dim ImmediatePheromoneStrength As Integer = 0
        If Not MyAntImmediatePheromoneStrengthHash.Item(pID) Is Nothing
            ImmediatePheromoneStrength = CInt(MyAntImmediatePheromoneStrengthHash.Item(pID))
        End If
        Return ImmediatePheromoneStrength
    End Function

    Public Sub ClearImmediatePheromoneStrengths()
        MyAntImmediatePheromoneStrengthHash.Clear
    End Sub

    Public Sub SetMyAntPheromoneTileStrength_Both(pID As String, pAntID As String, pStrength As Integer, ByRef pEdgeTiles As List(Of MapTile))
        If pStrength > 0 Then


            If pStrength = 1
                pEdgeTiles.Add(Me)
            Else
                If pEdgeTiles.Contains(Me)
                    pEdgeTiles.Remove(Me)
                End If
            End If


            'Set Immediate Pheromone Hash
            If MyAntImmediatePheromoneStrengthHash.Item(pID) Is Nothing    'If Does Not Exist in HashTable
                'This is a new value to the tile, so add the pheromone value to the persistent pheromone value
                MyAntPersistentPheromoneStrength += pStrength

                AddImmediatePheromoneStrength(pID, pStrength)     'Add new value

                '
                '
                If MyAntSinglePersistentPheromoneStrength.Item(pAntID) Is Nothing
                    MyAntSinglePersistentPheromoneStrength.Add(pAntID, pStrength)
                Else
                    MyAntSinglePersistentPheromoneStrength.Item(pAntID) = CInt(MyAntSinglePersistentPheromoneStrength.Item(pAntID)) + pStrength
                End If
                '
                '

            ElseIf GetImmediatePheromoneStrength(pID) < pStrength 'If already exists, but strength value is less than inputed
                'This tile already has an Immediate pheromone applied to it, so we need to take into account the previously
                'applied pheromone value to the persistent value so we can ensure the persistent value remainsMy accurate.
                MyAntPersistentPheromoneStrength -= GetImmediatePheromoneStrength(pID)    'First remove the old value
                MyAntPersistentPheromoneStrength += pStrength 'Second, add the new strength value

                '
                '
                'We need to do the same thing as directly above, but for the Single Ant Persistent Hash
                MyAntSinglePersistentPheromoneStrength.Item(pAntID) = CInt(MyAntSinglePersistentPheromoneStrength.Item(pAntID)) - GetImmediatePheromoneStrength(pID)
                MyAntSinglePersistentPheromoneStrength.Item(pAntID) = CInt(MyAntSinglePersistentPheromoneStrength.Item(pAntID)) + pStrength
                '
                '

                MyAntImmediatePheromoneStrengthHash.Item(pID) = pStrength  'Set the value
            End If

            'Check if we are in a corner or a crevice, if so, add extra value to peristent pheromone
            ''to prevent this ant or other ants from entering this tile
            If IsInCreviceOrCorner()
                MyAntPersistentPheromoneStrength += (_CORNERPHEROMONEMULTIPLIER*pStrength)
            End If

            'Set Max Allowed Persistent Pheromone Value
            'If MyAntPersistentPheromoneStrength > _MAXALLOWEDMYANTPERSISTENTPHEROMONESTRENGTH
            '    MyAntPersistentPheromoneStrength = _MAXALLOWEDMYANTPERSISTENTPHEROMONESTRENGTH
            'End If

            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable Then                                    'Only do tiles available in a path\
                    If AdjacentTile.GetImmediatePheromoneStrength(pID) < pStrength-1  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetMyAntPheromoneTileStrength_Both(pID, pAntID, pStrength - 1, pEdgeTiles)  
                    End If
                End If
            Next
        End If
    End Sub

    Public Function GetMyAntPersistentPheromoneStrength() As Integer
        Return MyAntPersistentPheromoneStrength
    End Function

    Public Function GetMyAntImmediatePheromoneStrength(pID As String) As Integer
        Return CInt(MyAntImmediatePheromoneStrengthHash(pID))
    End Function

#End Region

#Region "Manage My Ant ATTRACTION Hash"

    Private Sub AddMyAntAttractionStrength(pID As String, pStrength As Integer)
        MyAntAttractionStrengthHash.Add(pID, pStrength)
    End Sub

    Public Function GetMyAntAttraction(pID As String) As Integer
        Dim MyAntAttractionStrength As Integer = 0
        If Not MyAntAttractionStrengthHash.Item(pID) Is Nothing
            MyAntAttractionStrength = CInt(MyAntAttractionStrengthHash.Item(pID))
        End If
        Return MyAntAttractionStrength
    End Function

    Public Sub ClearMyAntAttraction()
        MyAntAttractionStrengthHash.Clear
    End Sub

    'Recursively go through each MapTile and set its FoodStrength value, and set a diminished FoodStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetMyAntAttractionStrength(pID As String, pStrength As Integer)
        If pStrength > 0
            If MyAntAttractionStrengthHash.Item(pID) Is Nothing    'If Does Not Exist in HashTable
                AddMyAntAttractionStrength(pID, pStrength)     'Add new value
            ElseIf GetMyAntAttraction(pID) < pStrength 'If already exists, but strength value is less than inputed
                MyAntAttractionStrengthHash.Item(pID) = pStrength  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable                                    'Only do tiles available in a path
                    If AdjacentTile.GetMyAntAttraction(pID) < pStrength-1  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetMyAntAttractionStrength(pID, pStrength-1)  
                    End If
                End If
            Next
        End If
    End Sub

#End Region

#Region "Manage Enemy Ant Pheromone Logic"

    'Recursively go through each MapTile and set its FoodStrength value, and set a diminished FoodStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetEnemyAntPheromoneTileStrength(pID As String, pStrength As Integer)
        If pStrength > 0
            If EnemyAntPheromoneStrengthHash.Item(pID) Is Nothing    'If Does Not Exist in HashTable
                AddEnemyAntPheromoneStrength(pID, pStrength)     'Add new value
            ElseIf GetEnemyAntPheromoneStrength(pID) < pStrength 'If already exists, but strength value is less than inputed
                EnemyAntPheromoneStrengthHash.Item(pID) = pStrength  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable                                    'Only do tiles available in a path
                    If AdjacentTile.GetEnemyAntPheromoneStrength(pID) < pStrength-1  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetEnemyAntPheromoneTileStrength(pID, pStrength-1)  
                    End If
                End If
            Next
        End If
    End Sub

    Public Function GetEnemyAntPheromoneStrength(pID As String) As Integer
        Dim EnemyAntPheromoneStrength As Integer = 0
        If Not EnemyAntPheromoneStrengthHash.Item(pID) Is Nothing
            EnemyAntPheromoneStrength = CInt(EnemyAntPheromoneStrengthHash.Item(pID))
        End If
        Return EnemyAntPheromoneStrength
    End Function

    Private Sub AddEnemyAntPheromoneStrength(pID As String, pStrength As Integer)
        EnemyAntPheromoneStrengthHash.Add(pID, pStrength)
    End Sub

    Public Sub ClearEnemyAntStrengths()
        EnemyAntPheromoneStrengthHash.Clear
    End Sub

#End Region

#Region "Manage Alert / Defense Logic"

    Private Sub SetAlertStrength(pStrength As Integer)
        AlertStrength = pStrength
    End Sub

    Public Function GetAlertStrength() As Integer
        Return AlertStrength
    End Function

    Public Sub ClearAlertStrength()
        AlertStrength = 0
    End Sub

    'Recursively go through each MapTile and set its AlertStrength value, and set a diminished AlertStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetAlertTileStrength(pStrength As Integer)
        If pStrength > 0 Then
            If GetAlertStrength() < pStrength Then 'If already exists, but strength value is less than inputed
                SetAlertStrength(pStrength)  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable Then                                    'Only do tiles available in a path
                    If AdjacentTile.GetAlertStrength < pStrength - 1 Then  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetAlertTileStrength(pStrength - 1)
                    End If
                End If
            Next
        End If
    End Sub

#End Region

#Region "Manage Defender Ant Logic"

    Private Sub SetDefenderAntPosition(pStrength As Integer)
        DefenderAntPositionStrength = pStrength
    End Sub

    Public Function GetDefenderAntPositionStrength() As Integer
        Return DefenderAntPositionStrength
    End Function

    Public Sub ClearDefenderAntPositionsStrength()
        DefenderAntPositionStrength = 0
    End Sub

    'Recursively go through each MapTile and set its MyAntHillStrength value, and set a diminished MyAntHillStrength value to all 
    ''Adjacent MapTiles that are Passable. This will create a mapping upon the map of the shortest path to travel.
    Public Sub SetDefenderAntPositionTileStrength(pStrength As Integer)
        If pStrength > 0 Then
            If GetDefenderAntPositionStrength() < pStrength Then 'If already exists, but strength value is less than inputed
                SetDefenderAntPosition(pStrength)  'Set the value
            End If
            'Recursively set the strength of all immediately ajacent tiles
            For Each AdjacentTile As MapTile In LinkedTiles
                If AdjacentTile.Passable Then                                    'Only do tiles available in a path
                    If AdjacentTile.GetDefenderAntPositionStrength() < pStrength - 1 Then  'Don't do tiles already done (may cause infinite loop if we do them again)
                        AdjacentTile.SetDefenderAntPositionTileStrength(pStrength - 1)
                    End If
                End If
            Next
        End If
    End Sub

#End Region

#Region "Misc Helper Logic"

    Public Function IsInCreviceOrCorner() As Boolean
        Dim CreviceOrCorner As Boolean
        Dim CountImpassableTiles As Integer = 0
        Dim ImpassableTileIndices As New List(Of Integer)

        'Create list of adjacent tiles that are not passable
        For i As Integer = 0 to LinkedTiles.Length-1
            If Not LinkedTiles(i).Passable
                ImpassableTileIndices.Add(i)
            End If
        Next

        With ImpassableTileIndices
            If .Count < 2   
                CreviceOrCorner = False
            ElseIf .Count = 3
                CreviceOrCorner = True  'We are in a crevice
            ElseIf .Count = 2
                'Find out if we are in a corner...
                ''The direction enum goes n,s,e,w which translates to n=0, s=1, e=2, w=3. We know that an ant is in a corner if
                ''sequential adjacent tiles are impassable, and the count is 2. However, because the direction enum does not go
                ''n,e,s,w we cannot just look for a sequential order of impassable tile indices. However, we do know that if the
                ''count of impassable tiles is 2, we know the only condition we couldnt be in a corner is if the 2 impassable
                ''tiles are either n and s or e and w. Therefore, check for this condition, if true set to not a corner, else
                ''set to it is a corner.
                If (.Contains(0) AndAlso .Contains(1)) Or (.Contains(2) AndAlso .Contains(3))
                    CreviceOrCorner = False     'We are NOT in a corner
                Else
                    CreviceOrCorner = True      'We are in a corner
                End If
            End If
        End With

        Return CreviceOrCorner
    End Function

#End Region

End Class
