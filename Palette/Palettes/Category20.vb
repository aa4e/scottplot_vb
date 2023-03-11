Namespace ScottPlot.Palettes

    Public Class Category20
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Category 20" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A set of 10 unque colors used in many data visualization libraries such as Matplotlib, Vega, and Tableau"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#1f77b4", "#aec7e8", "#ff7f0e", "#ffbb78", "#2ca02c",
                    "#98df8a", "#d62728", "#ff9896", "#9467bd", "#c5b0d5",
                    "#8c564b", "#c49c94", "#e377c2", "#f7b6d2", "#7f7f7f",
                    "#c7c7c7", "#bcbd22", "#dbdb8d", "#17becf", "#9edae5"
                }
            End Get
        End Property

    End Class

End Namespace