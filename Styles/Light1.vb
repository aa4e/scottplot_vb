Imports System.Drawing

Namespace ScottPlot.Styles

    Friend Class Light1
        Inherits [Default]

        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#7f7f7f")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#7f7f7f")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#7f7f7f")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#7f7f7f")

    End Class

End Namespace