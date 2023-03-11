Imports System.Drawing

Namespace ScottPlot.Styles

    Public Class Burgundy
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#ffffff")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#fffdfd")
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#560013")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#ffdae3")
        Public Overrides ReadOnly Property AxisLabelColor As Color = ColorTranslator.FromHtml("#5e0015")
        Public Overrides ReadOnly Property TitleFontColor As Color = ColorTranslator.FromHtml("#560013")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#5e0015")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#560013")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#560013")

    End Class

End Namespace