Imports System.Drawing

Namespace ScottPlot.Styles

    Friend Class Gray2
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#131519")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#262626")
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#757575")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#2d2d2d")
        Public Overrides ReadOnly Property AxisLabelColor As Color = ColorTranslator.FromHtml("#b9b9ba")
        Public Overrides ReadOnly Property TitleFontColor As Color = ColorTranslator.FromHtml("#FFFFFF")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#757575")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#757575")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#757575")

    End Class

End Namespace