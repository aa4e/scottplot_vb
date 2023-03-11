Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A variation of the SignalPlotConst optimized for unevenly-spaced ascending X values.
    ''' </summary>
    Public Class SignalPlotXYConst(Of TX As {Structure, IComparable}, TY As {Structure, IComparable})
        Inherits SignalPlotXYGeneric(Of TX, TY)

        Public ReadOnly Property TreesReady As Boolean
            Get
                Dim s As MinMaxSearchStrategies.SegmentedTreeMinMaxSearchStrategy(Of TY) = TryCast(Strategy, MinMaxSearchStrategies.SegmentedTreeMinMaxSearchStrategy(Of TY))
                Return (s IsNot Nothing) AndAlso s.TreesReady
            End Get
        End Property

        Public Sub New()
            Strategy = New MinMaxSearchStrategies.SegmentedTreeMinMaxSearchStrategy(Of TY)()
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", (" (" + Label + ")"))
            Return $"PlottableSignalXYConst{lbl} with {PointCount} points ({GetType(TX).Name}, {GetType(TY).Name})."
            'return $"PlottableSignalXYConst{label} with {PointCount} points ({typeof(TX).Name}, {typeof(TY).Name})";
        End Function

    End Class

End Namespace