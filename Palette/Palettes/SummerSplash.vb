Namespace ScottPlot.Palettes

    Public Class SummerSplash
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Summer Splash" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String = "" Implements ScottPlot.IPalette.Description

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {"#05445E", "#189AB4", "#75E6DA", "#D4F1F4"}
            End Get

        End Property
    End Class

End Namespace