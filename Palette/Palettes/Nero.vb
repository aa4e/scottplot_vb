Namespace ScottPlot.Palettes

    Public Class Nero
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
                Return New String() {"#013A20", "#478C5C", "#94C973", "#BACC81", "#CDD193"}
            End Get
        End Property

    End Class

End Namespace