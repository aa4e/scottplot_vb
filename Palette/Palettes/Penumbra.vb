Namespace ScottPlot.Palettes

    Public Class Penumbra
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String Implements ScottPlot.IPalette.Name
            Get
                Return MyBase.GetType().Name
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "A perceptually uniform color palette by Neal McKee: https://github.com/nealmckee/penumbra"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#CB7459", "#A38F2D", "#46A473",
                    "#00A0BE", "#7E87D6", "#BD72A8"
                }
            End Get
        End Property

    End Class

End Namespace