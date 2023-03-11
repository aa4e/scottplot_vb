Imports System.Drawing

Namespace ScottPlot.Styles

    Public Class Blue1
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = ColorTranslator.FromHtml("#07263b")
        Public Overrides ReadOnly Property DataBackgroundColor As Color = ColorTranslator.FromHtml("#0b3049")
        Public Overrides ReadOnly Property FrameColor As Color = ColorTranslator.FromHtml("#145665")
        Public Overrides ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#0e3d54")
        Public Overrides ReadOnly Property AxisLabelColor As Color = ColorTranslator.FromHtml("#b5bec5")
        Public Overrides ReadOnly Property TitleFontColor As Color = ColorTranslator.FromHtml("#d0dae2")
        Public Overrides ReadOnly Property TickLabelColor As Color = ColorTranslator.FromHtml("#b5bec5")
        Public Overrides ReadOnly Property TickMajorColor As Color = ColorTranslator.FromHtml("#145665")
        Public Overrides ReadOnly Property TickMinorColor As Color = ColorTranslator.FromHtml("#145665")

    End Class

End Namespace