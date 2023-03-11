Namespace ScottPlot.Palettes

    Public Class DarkPastel
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Dark Pastel" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A qualitative 8-color palette generated using colorbrewer2.org"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#66c2a5", "#fc8d62",
                    "#8da0cb", "#e78ac3",
                    "#a6d854", "#ffd92f",
                    "#e5c494", "#b3b3b3"
                }
            End Get
        End Property

    End Class

End Namespace