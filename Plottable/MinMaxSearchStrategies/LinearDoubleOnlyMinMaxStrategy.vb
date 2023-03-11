Namespace ScottPlot.MinMaxSearchStrategies

    Public Class LinearDoubleOnlyMinMaxStrategy
        Implements IMinMaxSearchStrategy(Of Double)

        Public Property SourceArray As Double() = New Double() {} Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of Double).SourceArray

        Public Sub MinMaxRangeQuery(left As Integer, right As Integer, ByRef lowestValue As Double, ByRef highestValue As Double) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of Double).MinMaxRangeQuery
            lowestValue = SourceArray(left)
            highestValue = SourceArray(left)
            For i As Integer = left To right
                If (SourceArray(i) < lowestValue) Then
                    lowestValue = SourceArray(i)
                End If
                If (SourceArray(i) > highestValue) Then
                    highestValue = SourceArray(i)
                End If
            Next
        End Sub

        Public Function SourceElement(index As Integer) As Double Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of Double).SourceElement
            Return SourceArray(index)
        End Function

        Public Sub UpdateElement(index As Integer, newValue As Double) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of Double).UpdateElement
            SourceArray(index) = newValue
        End Sub

        Public Sub UpdateRange(from As Integer, [to] As Integer,
                               newData As Double(), Optional fromData As Integer = 0) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of Double).UpdateRange
            For i As Integer = from To [to] - 1
                SourceArray(i) = newData(i - from + fromData)
            Next
        End Sub

    End Class

End Namespace