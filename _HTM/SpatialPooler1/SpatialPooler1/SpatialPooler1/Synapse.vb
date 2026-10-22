Public Class Synapse
    Public Column As Column
    Private Permanence As Double
    Private ConnectedPerm As Double

    Public ReadOnly Property IsConnected()
        Get
            If Permanence >= ConnectedPerm then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property IsActive()
        Get
            If Column.ActiveColumn Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property xPermanence()
        Get
            Return Permanence
        End Get
    End Property

    Public ReadOnly Property xConnectedPerm
        Get
            Return ConnectedPerm
        End Get
    End Property

    Public Sub New(pColumn As Column, pPermanence As Double, pConnectedPerm As Double)
        Column = pColumn
        Permanence = pPermanence
        ConnectedPerm = pConnectedPerm
    End Sub
End Class
