Imports System.Drawing

Namespace ScottPlot.Styles

    Public Class Gray1
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#31363a")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#3a4149")
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#757a80")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#444b52")
        Public Overrides ReadOnly Property AxisLabelColor As Color = ColorTranslator.FromHtml("#d6d7d8")
        Public Overrides ReadOnly Property TitleFontColor As Color = ColorTranslator.FromHtml("#FFFFFF")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#757a80")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#757a80")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#757a80")

    End Class

End Namespace