Namespace ScottPlot.Palettes

    Public Class Tsitsulin
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Xgfs 25" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A 25-color palette by Arthurits adapted from Tsitsulin's 12-color xgfs palette: http://tsitsul.in/blog/coloropt"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#ebac23", "#b80058", "#008cf9", "#006e00", "#00bbad",
                    "#d163e6", "#b24502", "#ff9287", "#5954d6", "#00c6f8",
                    "#878500", "#00a76c",
                    "#f6da9c", "#ff5caa", "#8accff", "#4bff4b", "#6efff4",
                    "#edc1f5", "#feae7c", "#ffc8c3", "#bdbbef", "#bdf2ff",
                    "#fffc43", "#65ffc8",
                    "#aaaaaa"
                }
            End Get
        End Property

    End Class

End Namespace