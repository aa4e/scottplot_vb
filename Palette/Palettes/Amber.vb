Namespace ScottPlot.Palettes

    ''' <summary>
    ''' Sourced from Material Design:
    ''' https//material.io/design/color/the-color-system.html
    ''' </summary>
    Public Class Amber
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String Implements IPalette.Name
            Get
                Return MyBase.GetType().Name
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String Implements IPalette.Description
            Get
                Return String.Empty
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {"#FF6F00", "#FF8F00", "#FFA000", "#FFB300", "#FFC107"}
            End Get
        End Property

    End Class

End Namespace