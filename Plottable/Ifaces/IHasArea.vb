Imports System.Drawing

Namespace ScottPlot.Plottable

    Public Interface IHasArea

        Property BorderColor As Color
        Property BorderLineWidth As Single
        Property BorderLineStyle As LineStyle
        Property HatchColor As Color
        Property HatchStyle As ScottPlot.Drawing.HatchStyle

    End Interface

End Namespace