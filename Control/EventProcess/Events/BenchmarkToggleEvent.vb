Namespace ScottPlot.Control.EventProcess.Events

    ''' <summary>
    ''' This event toggles visibility of the benchmark.
    ''' This event Is typically called after double-clicking the plot.
    ''' </summary>
    Public Class BenchmarkToggleEvent
        Implements IUIEvent

        Private ReadOnly Plot As Plot
        Private ReadOnly Configuration As Configuration

        Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
            Get
                Return Configuration.QualityConfiguration.BenchmarkToggle
            End Get
        End Property

        Public Sub New(plt As Plot, config As Configuration)
            Me.Plot = plt
            Me.Configuration = config
        End Sub

        Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
            Plot.Benchmark(Not Plot.Benchmark(Nothing))
        End Sub

    End Class

End Namespace