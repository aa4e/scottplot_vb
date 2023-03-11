Namespace ScottPlot.Palettes

    Public Class Dark
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String Implements ScottPlot.IPalette.Name
            Get
                Return MyBase.GetType().Name
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A qualitative 8-color palette generated using colorbrewer2.org"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#1b9e77", "#d95f02",
                    "#7570b3", "#e7298a",
                    "#66a61e", "#e6ab02",
                    "#a6761d", "#666666"
                }
            End Get
        End Property

    End Class

End Namespace