Namespace ScottPlot.Plottable

    ''' <summary>
    ''' This plot type is a potentially faster version of the SignalPlot.
    ''' It pre-calculates min/max values for various segments of data, greatly speeding-up rendering
    ''' for extremely large datasets (10s of millions of points).
    ''' Note that these pre-calculations require more memory and an up-front calculation delay.
    ''' If the underlying data is updated, you must call Update() methods to recalculate the min/max values.
    ''' </summary>
    ''' <remarks>
    ''' Variation of PlottableSignal that uses a segmented tree for faster min/max range queries
    ''' - frequent min/max lookups are a bottleneck displaying large signals
    ''' - double[] limited to 60M points (250M in x64 mode) due to memory (tree uses from 2X to 4X memory)
    ''' - smaller data types have higher points count limits
    ''' - in x64 mode limit can be up to maximum array size (2G points) with special solution and 64 GB RAM (Not tested)
    ''' - if source array is changed UpdateTrees() must be called
    ''' - source array can be change by call updateData(), updating by ranges much faster.
    ''' </remarks>
    Public Class SignalPlotConst(Of T As {Structure, IComparable})
        Inherits SignalPlotBase(Of T)

        Public ReadOnly Property TreesReady As Boolean
            Get
                Dim s As MinMaxSearchStrategies.SegmentedTreeMinMaxSearchStrategy(Of T) = TryCast(Strategy, MinMaxSearchStrategies.SegmentedTreeMinMaxSearchStrategy(Of T))
                Return (s IsNot Nothing) AndAlso s.TreesReady
            End Get
        End Property

        Public Sub New()
            Strategy = New MinMaxSearchStrategies.SegmentedTreeMinMaxSearchStrategy(Of T)()
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", (" (" + Label + ")"))
            Return $"PlottableSignalConst{lbl} with {PointCount} points ({GetType(T).Name})."
        End Function

    End Class

End Namespace