Namespace ScottPlot.Palettes

    Public Class LightSpectrum
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Light spectrum" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A 9-color palette by Arthurits created by lightening the colors in the visible spectrum"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#fce5e6", "#fff8e7", "#fffce8",
                    "#eff5e4", "#e7f2e6", "#ddf0f5",
                    "#e6f2fc", "#e6eaf7", "#eee0f0"
                }
            End Get
        End Property

    End Class

End Namespace