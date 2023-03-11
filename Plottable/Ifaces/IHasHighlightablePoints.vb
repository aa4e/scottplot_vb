Namespace ScottPlot.Plottable

    Friend Interface IHasHighlightablePoints

        Function HighlightPoint(index As Integer) As Tuple(Of Double, Double, Integer)
        Function HighlightPointNearestX(x As Double) As Tuple(Of Double, Double, Integer)
        Function HighlightPointNearestY(y As Double) As Tuple(Of Double, Double, Integer)
        Function HighlightPointNearest(x As Double, y As Double) As Tuple(Of Double, Double, Integer)
        Sub HighlightClear()

    End Interface

End Namespace