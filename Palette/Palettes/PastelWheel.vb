Namespace ScottPlot.Palettes

    Public Class PastelWheel
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Pastel wheel" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A 12-color palette by Arthurits created by lightening the color wheel"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#f8c5c7", "#fadec3", "#fbf6c4",
                    "#e1ecc8", "#d7e8cb", "#daebd7",
                    "#d9eef3", "#cadbed", "#c7d2e6",
                    "#d4d1e5", "#e8d3e6", "#f8c7de"
                }
            End Get
        End Property

    End Class

End Namespace