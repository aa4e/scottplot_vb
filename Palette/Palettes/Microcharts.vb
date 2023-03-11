Namespace ScottPlot.Palettes

    Public Class Microcharts
        Inherits HexPaletteBase
        Implements IPalette

        Public Overrides ReadOnly Property Name As String Implements ScottPlot.IPalette.Name
            Get
                Return MyBase.GetType().Name
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
            Get
                Return "Soft color palette sourced from the Microcharts project: https://github.com/microcharts-dotnet/Microcharts"
            End Get
        End Property

        Friend Overrides ReadOnly Property HexColors As String()
            Get
                Return {
                    "#266489", "#68B9C0", "#90D585",
                    "#F3C151", "#F37F64", "#424856",
                    "#8F97A4", "#DAC096", "#76846E",
                    "#DABFAF", "#A65B69", "#97A69D"
                }
            End Get
        End Property

    End Class

End Namespace