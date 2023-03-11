Imports System.Drawing

Namespace ScottPlot.Styles

    Public Class [Default]
        Implements IStyle

        Public Overridable ReadOnly Property FigureBackgroundColor As Color = Color.White Implements ScottPlot.Styles.IStyle.FigureBackgroundColor
        Public Overridable ReadOnly Property DataBackgroundColor As Color = Color.White Implements ScottPlot.Styles.IStyle.DataBackgroundColor
        Public Overridable ReadOnly Property GridLineColor As Color = ColorTranslator.FromHtml("#efefef") Implements ScottPlot.Styles.IStyle.GridLineColor
        Public Overridable ReadOnly Property FrameColor As Color = Color.Black Implements ScottPlot.Styles.IStyle.FrameColor
        Public Overridable ReadOnly Property TitleFontColor As Color = Color.Black Implements ScottPlot.Styles.IStyle.TitleFontColor
        Public Overridable ReadOnly Property AxisLabelColor As Color = Color.Black Implements ScottPlot.Styles.IStyle.AxisLabelColor
        Public Overridable ReadOnly Property TickLabelColor As Color = Color.Black Implements ScottPlot.Styles.IStyle.TickLabelColor
        Public Overridable ReadOnly Property TickMajorColor As Color = Color.Black Implements ScottPlot.Styles.IStyle.TickMajorColor
        Public Overridable ReadOnly Property TickMinorColor As Color = Color.Black Implements ScottPlot.Styles.IStyle.TickMinorColor
        Public Overridable ReadOnly Property TitleFontName As String = Drawing.InstalledFont.Default() Implements ScottPlot.Styles.IStyle.TitleFontName
        Public Overridable ReadOnly Property AxisLabelFontName As String = Drawing.InstalledFont.Default() Implements ScottPlot.Styles.IStyle.AxisLabelFontName
        Public Overridable ReadOnly Property TickLabelFontName As String = Drawing.InstalledFont.Default() Implements ScottPlot.Styles.IStyle.TickLabelFontName

    End Class

End Namespace