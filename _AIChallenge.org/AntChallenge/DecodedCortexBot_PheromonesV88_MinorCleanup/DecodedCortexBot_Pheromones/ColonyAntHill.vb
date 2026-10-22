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

Public Class ColonyAntHill
    Public _AntHill As MapTile
    Public _LookoutTower As List(Of MapTile)
    Public _MAXALERTSTRENGTH As Integer = 15

    Public Sub New(pAntHillMapTile As MapTile)
        _AntHill = pAntHillMapTile
        _LookoutTower = New List(Of MapTile)
    End Sub

    Public Sub CalculateLookoutTowerLocations(pRadius As Integer)
        _LookoutTower.Clear

        Dim N As MapTile = _AntHill
        Dim NE As MapTile = _AntHill
        Dim E As MapTile = _AntHill
        Dim SE As MapTile = _AntHill
        Dim S As MapTile = _AntHill
        Dim SW As MapTile = _AntHill
        Dim W As MapTile = _AntHill
        Dim NW As MapTile = _AntHill

        'Do the cardinal directions
        For i As Integer = 0 to pRadius
            N = N.LinkedTiles(Direction.n)
            S = S.LinkedTiles(Direction.s)
            E = E.LinkedTiles(Direction.e)
            W = W.LinkedTiles(Direction.w)
        Next

        'Do the more difficult directions...
        Dim Turn As Boolean = False
        For i As Integer = 0 to pRadius+1
            If Not Turn
                NE = NE.LinkedTiles(Direction.n)
                SE = SE.LinkedTiles(Direction.s)
                SW = SW.LinkedTiles(Direction.s)
                NW = NW.LinkedTiles(Direction.n)
                Turn = Not Turn
            Else
                NE = NE.LinkedTiles(Direction.e)
                SE = SE.LinkedTiles(Direction.e)
                SW = SW.LinkedTiles(Direction.w)
                NW = NW.LinkedTiles(Direction.w)
                Turn = Not Turn
            End If
        Next

        'The NSEW cardinal direction lookout towers are now set at their furthest position. Go through each lookout tower and verify
        'They exist on a passable tile. If not, move closer and closer to the ant hill until a passable tile found, and setup lookout there.
        For i As Integer = 0 to pRadius-1 
            If N.Passable = False
                N = N.LinkedTiles(Direction.s)
            End If
            If E.Passable = False
                E = E.LinkedTiles(Direction.w)
            End If
            If S.Passable = False
                S = S.LinkedTiles(Direction.n)
            End If
            If W.Passable = False
                W = W.LinkedTiles(Direction.e)
            End If
        Next

        'For the Non-cardinal direction lookout towers now set at their furthest position. Go through each lookout tower and verify they
        'exist at a passable tile. If not, move closer to the ant hill until a passable tile is found, and setup lookout there.
        Turn = True
        For i As Integer = 0 to pRadius+1
            If Not Turn
                If NE.Passable = False
                    NE = NE.LinkedTiles(Direction.s)
                End If
                If SE.Passable = False
                    SE = SE.LinkedTiles(Direction.n)
                End If
                If SW.Passable = False
                    SW = SW.LinkedTiles(Direction.n)
                End If
                If NW.Passable = False
                    NW = NW.LinkedTiles(Direction.s)
                End If
                Turn = Not Turn
            Else
                If NE.Passable = False
                    NE = NE.LinkedTiles(Direction.w)
                End If
                If SE.Passable = False
                    SE = SE.LinkedTiles(Direction.w)
                End If
                If SW.Passable = False
                    SW = SW.LinkedTiles(Direction.e)
                End If
                If NW.Passable = False
                    NW = NW.LinkedTiles(Direction.e)
                End If
                Turn = Not Turn
            End If
        Next

        If N.Passable
            _LookoutTower.Add(N)
        End If
        If NE.Passable
            _LookoutTower.Add(NE)
        End If
        If E.Passable
            _LookoutTower.Add(E)
        End If
        If SE.Passable
            _LookoutTower.Add(SE)
        End If
        If S.Passable
            _LookoutTower.Add(S)
        End If
        If SW.Passable
            _LookoutTower.Add(SW)
        End If
        If W.Passable
            _LookoutTower.Add(W)
        End If
        If NW.Passable
            _LookoutTower.Add(NW)
        End If

        'Always consider The AntHill to be a lookout tower as well as the real lookout towers
        _LookoutTower.Add(_AntHill)

    End Sub

    Public Sub CheckLookoutTowers()
        For Each Tower As MapTile In _LookoutTower  
            If Tower.EnemyAntPheromoneStrengthHash.Count > 0
                Tower.SetAlertTileStrength(_MAXALERTSTRENGTH)
            End If
        Next
    End Sub

    Public Sub SetMAXALERTSTRENGTH(pNewMAXALERTSTRENGTH As Integer)
        _MAXALERTSTRENGTH = pNewMAXALERTSTRENGTH
    End Sub

End Class
