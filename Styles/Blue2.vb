Imports System.Drawing

Namespace ScottPlot.Styles

    Friend Class Blue2
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#1b2138")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#252c48")
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#bbbdc4")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#2c334e")
        Public Overrides ReadOnly Property AxisLabelColor As Color = ColorTranslator.FromHtml("#bbbdc4")
        Public Overrides ReadOnly Property TitleFontColor As Color = ColorTranslator.FromHtml("#d8dbe3")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#bbbdc4")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#bbbdc4")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#bbbdc4")

    End Class

End Namespace