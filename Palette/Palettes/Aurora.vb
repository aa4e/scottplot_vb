Namespace ScottPlot.Palettes

    Public Class Aurora
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
                Return {"#BF616A", "#D08770", "#EBCB8B", "#A3BE8C", "#B48EAD"}
            End Get
        End Property

    End Class

End Namespace