Namespace ScottPlot.Palettes

    Public Class Redness
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
                Return New String() {"#FF0000", "#FF4F00", "#FFA900", "#900303", "#FF8181"}
            End Get
        End Property

    End Class

End Namespace