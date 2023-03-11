Namespace ScottPlot.Palettes

    Public Class OneHalfDark
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "One Half (Dark)" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A Sublime color scheme by Son A. Pham: https://github.com/sonph/onehalf"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#e06c75", "#98c379", "#e5c07b",
                    "#61aff0", "#c678dd", "#56b6c2", "#dcdfe4"
                }
            End Get
        End Property

    End Class

End Namespace