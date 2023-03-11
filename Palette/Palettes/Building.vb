Namespace ScottPlot.Palettes

    Public Class Building
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String Implements ScottPlot.IPalette.Name
            Get
                Return MyBase.GetType().Name
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String = "" Implements ScottPlot.IPalette.Description

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {"#FF6F00", "#FF8F00", "#FFA000", "#FFB300", "#FFC107"}
            End Get
        End Property

    End Class

End Namespace