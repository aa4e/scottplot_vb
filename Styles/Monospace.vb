Namespace ScottPlot.Styles

    Public Class Monospace
        Inherits [Default]

        Public Overrides ReadOnly Property TitleFontName As String = Drawing.InstalledFont.Monospace()
        Public Overrides ReadOnly Property AxisLabelFontName As String = Drawing.InstalledFont.Monospace()
        Public Overrides ReadOnly Property TickLabelFontName As String = Drawing.InstalledFont.Monospace()

    End Class

End Namespace