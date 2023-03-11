Namespace ScottPlot.Palettes

    Public Class LightOcean
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String = "Light Ocean" Implements ScottPlot.IPalette.Name

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A 9-color palette by Arthurits created by a mixture of light greens, blues, and purples"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#dfedd9", "#dbecdc", "#dbede4",
                    "#daeeec", "#daeef3", "#dae6f2",
                    "#dadef1", "#dedaee", "#e5daed"
                }
            End Get
        End Property

    End Class

End Namespace