Namespace ScottPlot.Palettes

    Public Class SnowStorm
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Snow Storm" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "From the Nord collection of palettes: https://github.com/arcticicestudio/nord"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {"#D8DEE9", "#E5E9F0", "#ECEFF4"}
            End Get
        End Property

    End Class

End Namespace