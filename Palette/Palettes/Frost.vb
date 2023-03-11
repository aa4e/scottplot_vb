Namespace ScottPlot.Palettes

    Public Class Frost
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String Implements ScottPlot.IPalette.Name
            Get
                Return MyBase.GetType().Name
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "From the Nord collection of palettes: https://github.com/arcticicestudio/nord"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {"#8FBCBB", "#88C0D0", "#81A1C1", "#5E81AC"}
            End Get
        End Property

    End Class

End Namespace