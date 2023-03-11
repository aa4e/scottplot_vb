Imports System.Drawing

Namespace ScottPlot.Styles

    ''' <summary>
    ''' A theme describes a collection of colors and fonts that can be used to style a plot.
    ''' </summary>
    Public Interface IStyle

        ReadOnly Property AxisLabelFontName As String
        ReadOnly Property TitleFontName As String
        ReadOnly Property TickLabelFontName As String

        ReadOnly Property FigureBackgroundColor As Color
        ReadOnly Property DataBackgroundColor As Color
        ReadOnly Property FrameColor As Color
        ReadOnly Property GridLineColor As Color
        ReadOnly Property AxisLabelColor As Color
        ReadOnly Property TitleFontColor As Color
        ReadOnly Property TickLabelColor As Color
        ReadOnly Property TickMajorColor As Color
        ReadOnly Property TickMinorColor As Color

    End Interface

End Namespace