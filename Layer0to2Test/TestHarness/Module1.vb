Module Module1

    Public Structure DendriteIndex
        Public DendriteIndex As Integer
        Private OddFlag As Boolean
        Private Differential As Integer

        Public Sub New(ByVal Index As Integer)
            DendriteIndex = Index
            OddFlag = True
            Differential = 1
        End Sub


        Public Sub NextIndex()
            If OddFlag Then
                DendriteIndex = (DendriteIndex + Differential) Mod 8
                OddFlag = False
            ElseIf Not OddFlag Then
                DendriteIndex = (DendriteIndex - Differential) Mod 8
                OddFlag = True
            End If
            Differential += 1

            If DendriteIndex < 0 Then
                If DendriteIndex = -1 Then
                    DendriteIndex = 7
                ElseIf DendriteIndex = -2 Then
                    DendriteIndex = 6
                ElseIf DendriteIndex = -3 Then
                    DendriteIndex = 5
                ElseIf DendriteIndex = -4 Then
                    DendriteIndex = 4
                ElseIf DendriteIndex = -5 Then
                    DendriteIndex = 3
                ElseIf DendriteIndex = -6 Then
                    DendriteIndex = 2
                ElseIf DendriteIndex = -7 Then
                    DendriteIndex = 1
                ElseIf DendriteIndex = -8 Then
                    DendriteIndex = 0
                End If
            End If
        End Sub

    End Structure

    Sub Main()
        'Dim a As New Network.Layer0(100, 100)
        'Dim n As New Network.Neuron()
        'Dim n2 As New Network.Neuron("2")
        'Dim l As New Network.Layer(3, 3)
        Dim a As New DendriteIndex(0)
        For i As Integer = 0 To 7
            a.DendriteIndex = i
            For j As Integer = 0 To 7
                Console.WriteLine(a.DendriteIndex)
                a.NextIndex()
            Next
            Console.WriteLine()
        Next

        Console.WriteLine("Press Enter to Exit...")
        Dim z As String = Console.ReadLine()
    End Sub

End Module
