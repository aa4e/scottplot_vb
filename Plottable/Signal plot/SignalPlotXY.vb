Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A variation of the SignalPlot optimized for unevenly-spaced ascending X values.
    ''' </summary>
    Public Class SignalPlotXY
        Inherits SignalPlotXYGeneric(Of Double, Double)

        Public Sub New()
            Me.Strategy = New MinMaxSearchStrategies.LinearDoubleOnlyMinMaxStrategy()
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableSignalXY{lbl} with {PointCount} points."
        End Function

    End Class

End Namespace