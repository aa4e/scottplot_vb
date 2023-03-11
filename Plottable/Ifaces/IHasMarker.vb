Imports System.Drawing

Namespace ScottPlot.Plottable

    Public Interface IHasMarker

        Property MarkerSize As Single
        Property MarkerLineWidth As Single
        Property MarkerShape As MarkerShape
        Property MarkerColor As Color

    End Interface

End Namespace