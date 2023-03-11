Namespace ScottPlot.Plottable

    Public Interface IHighlightable

        ''' <summary>
        ''' Scale lines and markers by this fraction (1.0 for no size change)
        ''' </summary>
        Property HighlightCoefficient As Single
        Property IsHighlighted As Boolean

    End Interface

End Namespace