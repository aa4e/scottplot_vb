Namespace ScottPlot.Palettes

    Public Class PolarNight
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Polar Night" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "From the Nord collection of palettes: https://github.com/arcticicestudio/nord"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {"#2E3440", "#3B4252", "#434C5E", "#4C566A"}
            End Get
        End Property

    End Class

End Namespace