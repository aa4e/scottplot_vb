Imports System.Drawing

Namespace ScottPlot.Styles

    Public Class Hazel
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#221a0f")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#362712")
        Public Overrides ReadOnly Property FrameColor As Color = Color.White
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#221a0f")
        Public Overrides ReadOnly Property AxisLabelColor As Color = Color.White
        Public Overrides ReadOnly Property TitleFontColor As Color = Color.White
        Public Overrides ReadOnly Property TickLabelColor As Color = Color.Gray
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#757575")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#757575")

    End Class

End Namespace