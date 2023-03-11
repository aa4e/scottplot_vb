Namespace ScottPlot.Palettes

    Public Class ColorblindFriendly
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Colorblind Friendly" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A set of 8 colorblind-friendly colors from Bang Wong's Nature Methods paper https://www.nature.com/articles/nmeth.1618.pdf"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#000000", "#E69F00", "#56B4E9", "#009E73", "#F0E442",
                    "#0072B2", "#D55E00", "#CC79A7"
                }
            End Get
        End Property

    End Class

End Namespace