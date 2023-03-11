Namespace ScottPlot.MinMaxSearchStrategies

    Public Interface IMinMaxSearchStrategy(Of T)

        Property SourceArray As T()
        Sub MinMaxRangeQuery(l As Integer, r As Integer, <System.Runtime.InteropServices.OutAttribute()> ByRef lowestValue As Double, <System.Runtime.InteropServices.OutAttribute()> ByRef highestValue As Double)
        Sub UpdateElement(index As Integer, newValue As T)
        Sub UpdateRange(from As Integer, [to] As Integer, newData As T(), Optional fromData As Integer = 0)
        Function SourceElement(index As Integer) As Double

    End Interface

End Namespace