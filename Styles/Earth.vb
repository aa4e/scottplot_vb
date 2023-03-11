Imports System.Drawing

Namespace ScottPlot.Styles

    Friend Class Earth
        Inherits [Default]

        Public Overrides ReadOnly Property FigureBackgroundColor As Color = Color.SandyBrown
        Public Overrides ReadOnly Property DataBackgroundColor As Color = Color.SaddleBrown
        Public Overrides ReadOnly Property GridLineColor As Color = Color.Sienna
        Public Overrides ReadOnly Property FrameColor As Color = Color.Brown
        Public Overrides ReadOnly Property TitleFontColor As Color = Color.Brown
        Public Overrides ReadOnly Property AxisLabelColor As Color = Color.Brown
        Public Overrides ReadOnly Property TickLabelColor As Color = Color.Brown
        Public Overrides ReadOnly Property TickMajorColor As Color = Color.Brown
        Public Overrides ReadOnly Property TickMinorColor As Color = Color.Brown

    End Class

End Namespace