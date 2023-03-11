Imports System.Drawing

Namespace ScottPlot.Styles

    Friend Class Seaborn
        Inherits [Default]

        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#eaeaf2")
        Public Overrides ReadOnly Property FrameColor As Color = Color.Transparent
        Public Overrides ReadOnly Property GridLineColor As Color = Color.White
        Public Overrides ReadOnly Property AxisLabelColor As Color = ColorTranslator.FromHtml("#777777")
        Public Overrides ReadOnly Property TitleFontColor As Color = ColorTranslator.FromHtml("#777777")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#AAAAAA")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#eaeaf2")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#eaeaf2")

    End Class

End Namespace