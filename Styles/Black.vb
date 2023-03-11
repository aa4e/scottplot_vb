Imports System.Drawing

Namespace ScottPlot.Styles

    Public Class Black
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = Color.Black
        Public Overrides ReadOnly Property DataBackgroundColor As Color = Color.Black
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#757575")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#2d2d2d")
        Public Overrides ReadOnly Property TitleFontColor As Color = Color.White
        Public Overrides ReadOnly Property AxisLabelColor As Color = ColorTranslator.FromHtml("#b9b9ba")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#757575")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#757575")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#757575")

    End Class

End Namespace