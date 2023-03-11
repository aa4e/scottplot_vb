Namespace ScottPlot.Palettes

    Public Class Normal
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "XgfsNormal6" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A color palette adapted from Tsitsulin's 6-color normal xgfs palette: http://tsitsul.in/blog/coloropt"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#4053d3", "#ddb310", "#b51d14",
                    "#00beff", "#fb49b0", "#00b25d",
                    "#cacaca"
                }
            End Get
        End Property

    End Class

End Namespace