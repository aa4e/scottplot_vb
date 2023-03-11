Imports System.Drawing

Namespace ScottPlot.Styles

    Friend Class Light2
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#e4e6ec")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#f1f3f7")
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#145665")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#e5e7ea")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#77787b")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#77787b")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#77787b")

    End Class

End Namespace