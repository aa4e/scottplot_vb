Namespace ScottPlot.Plottable

    Public Interface IHasColormap

        ReadOnly Property Colormap As Drawing.Colormap
        ReadOnly Property ColormapMin As Double
        ReadOnly Property ColormapMax As Double
        ReadOnly Property ColormapMinIsClipped As Boolean
        ReadOnly Property ColormapMaxIsClipped As Boolean

    End Interface

End Namespace