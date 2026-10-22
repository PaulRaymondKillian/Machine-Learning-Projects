Module Module1

    Sub Main()

        Dim HLayer As New HippocampusEmulator.HLayer

        Console.WriteLine("Hippocampus Emulator Test Harness")
        Console.WriteLine("Desc: Demonstrate basic functions of Hippocampus Emulator Memory Recognition")
        Console.WriteLine("Written By: Paul Killian (2010)")
        Console.WriteLine()

        'Memorize new sequences
        Console.WriteLine("Memorizing Sequence: (1,2,3,4) - Memory Name: " & HLayer.Memorize("1,2,3,4"))
        Console.WriteLine("Memorizing Sequence: (1,2,3) - Memory Name: " & HLayer.Memorize("1,2,3"))
        Console.WriteLine("Memorizing Sequence: (5,4,5,4) - Memory Name: " & HLayer.Memorize("5,4,5,4"))
        Console.WriteLine("Memorizing Sequence: (9,8,7,6,5,4,3,2,1) - Memory Name: " & HLayer.Memorize("9,8,7,6,5,4,3,2,1"))

        Console.WriteLine()
        Console.WriteLine("Memorizing Already Know Sequence...")
        Console.WriteLine("Memorizing Sequence: (1,2,3,4) - Memory Name: " & HLayer.Memorize("1,2,3,4"))
        Console.WriteLine("It returns the name of the memory already stored, does not create a new memory.")

        Console.WriteLine()
        Console.WriteLine("Test the DoIKnowThis Function...")
        Console.WriteLine("Do I know the sequence (1,2,3,4)? - " & HLayer.DoIKnowThis("1,2,3,4"))
        Console.WriteLine("Do I know the sequence (2,3,4,1)? - " & HLayer.DoIKnowThis("2,3,4,1"))
        Console.WriteLine("Do I know the sequence (3,4,1,2)? - " & HLayer.DoIKnowThis("3,4,1,2"))
        Console.WriteLine("Do I know the sequence (4,1,2,3)? - " & HLayer.DoIKnowThis("4,1,2,3"))
        Console.WriteLine("Do I know the sequence (1,2)? - " & HLayer.DoIKnowThis("1,2"))
        Console.WriteLine("Do I know the sequence (1,2,3,4,5)? - " & HLayer.DoIKnowThis("1,2,3,4,5"))

        Console.WriteLine()
        Console.WriteLine("Test memory recognization...")
        Console.WriteLine("Inputted Sequence: (2,3,4,1) - Name: " & HLayer.Think("2,3,4,1").Attention)
        Console.WriteLine("Inputted Sequence: (3,4,1,2) - Name: " & HLayer.Think("3,4,1,2").Attention)
        Console.WriteLine("Inputted Sequence: (4,1,2,3) - Name: " & HLayer.Think("4,1,2,3").Attention)
        Console.WriteLine("Inputted Sequence: (3,1,2) - Name: " & HLayer.Think("3,1,2").Attention)
        Console.WriteLine("Inputted Sequence: (4,5,4,5) - Name: " & HLayer.Think("4,5,4,5").Attention)
        Console.WriteLine("Inputted Sequence: (8,7,6,5,4,3,2,1,9) - Name: " & HLayer.Think("8,7,6,5,4,3,2,1,9").Attention)
        Console.WriteLine("Inputted Sequence: (5,4,3,2,1,9,8,7,6) - Name: " & HLayer.Think("5,4,3,2,1,9,8,7,6").Attention)
        Console.WriteLine("Inputted Sequence: (3,2,1,9,8,7,6,5,4) - Name: " & HLayer.Think("3,2,1,9,8,7,6,5,4").Attention)

        Console.WriteLine()
        Console.WriteLine("Now test memories that do not exist...")
        Console.WriteLine("Inputted Sequence: (1,2) - Name: " & HLayer.Think("1,2").Attention)
        Console.WriteLine("Inputted Sequence: (1,2,3,4,5,6) - Name: " & HLayer.Think("1,2,3,4,5,6").Attention)

        Dim z As String = Console.ReadLine

    End Sub

End Module
