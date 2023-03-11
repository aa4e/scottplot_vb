Namespace ScottPlot.Palettes

    Public Class Nord
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String Implements ScottPlot.IPalette.Name
            Get
                Return MyBase.GetType().Name
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "From the Nord ConEmu color scheme: https://github.com/arcticicestudio/nord-conemu"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#bf616a", "#a3be8c", "#ebcb8b",
                    "#81a1c1", "#b48ead", "#88c0d0", "#e5e9f0"
                }
            End Get
        End Property

    End Class

End Namespace