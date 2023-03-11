Namespace ScottPlot.Statistics

    Public Class BasicStats

        Public ReadOnly Count As Integer
        Public ReadOnly Min As Double
        Public ReadOnly Max As Double
        Public ReadOnly Sum As Double
        Public ReadOnly Mean As Double
        Public ReadOnly StDev As Double
        Public ReadOnly StdErr As Double

        Public Sub New(values As Double())
            If (values is Nothing) Then
                Throw New ArgumentNullException()
            End If
            If (values.Length = 0) Then
                Throw New ArgumentException("Input cannot be empty.")
            End If

            Count = values.Length
            Dim t As Tuple(Of Double, Double, Double) = Common.MinMaxSum(values)
            Min = t.Item1
            Max = t.Item2
            Sum = t.Item3
            Mean = Sum / Count
            StDev = Common.StDev(values, Mean)
            StdErr = StDev / Math.Sqrt(Count)
        End Sub

    End Class

End Namespace