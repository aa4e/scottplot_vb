Namespace ScottPlot.Palettes

    Public Class OneHalf
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "One Half" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A Sublime color scheme by Son A. Pham: https://github.com/sonph/onehalf"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {"#383a42", "#e4564a", "#50a14f", "#c18402", "#0084bc", "#a626a4", "#0897b3"}
            End Get
        End Property

    End Class

End Namespace