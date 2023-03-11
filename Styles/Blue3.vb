Imports System.Drawing

Namespace ScottPlot.Styles

    Friend Class Blue3
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#001021")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#021d38")
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#d3d3d3")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#273c51")
        Public Overrides ReadOnly Property AxisLabelColor As Color = ColorTranslator.FromHtml("#d3d3d3")
        Public Overrides ReadOnly Property TitleFontColor As Color = ColorTranslator.FromHtml("#FFFFFF")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#d3d3d3")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#d3d3d3")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#d3d3d3")

    End Class

End Namespace