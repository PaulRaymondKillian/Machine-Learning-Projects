Module Module1

    Sub Main()
        Dim Receptor As new SpatialPooler1.SenseReceptor(10,10)
        Dim Region1 As New SpatialPooler1.Region(Receptor.GetColumns,4,10,10)
        Dim Region2 As New SpatialPooler1.Region(Region1.GetColumns,4,4,10)
        Dim Region3 As New SpatialPooler1.Region(Region2.GetColumns,4,4,10)
        Console.WriteLine(Region1.xPotentialSynapseCoverageMetric)
        Console.WriteLine()
        Console.WriteLine(Region2.xPotentialSynapseCoverageMetric)
        Console.WriteLine()
        Console.WriteLine(Region3.xPotentialSynapseCoverageMetric)

        Dim Random As New Random(System.DateTime.Now.Ticks Mod System.Int32.MaxValue)
        Dim InputW As Integer = 10
        Dim InputH As Integer = 10
        Dim InputArray(InputW-1, InputH-1) As Byte
        For i As Integer = 0 to InputW-1
            For j As Integer = 0 to InputH-1
                InputArray(i,j) = Random.Next mod 2
            Next
        Next

        Receptor.SetReceptorsInput(InputArray)
    End Sub

End Module
