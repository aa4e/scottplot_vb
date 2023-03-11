Imports System.Drawing

Namespace ScottPlot.Styles

    Friend Class Pink
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#d7c0d0")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#f7c7db")
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#f79ad3")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#c86fc9")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#8e518d")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#d7c0d0")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#f79ad3")

    End Class

End Namespace