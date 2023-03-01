Namespace ScottPlot.Drawing

    <Obsolete("This class is obsolete. Standard palettes are now in: ScottPlot.Palette", True)>
    Public Class PaletteObsolete

        Public Shared ReadOnly Property Amber As IPalette = New Palettes.Amber()
        Public Shared ReadOnly Property Aurora As IPalette = New Palettes.Aurora()
        Public Shared ReadOnly Property Category10 As IPalette = New Palettes.Category10()
        Public Shared ReadOnly Property Category20 As IPalette = New Palettes.Category20()
        Public Shared ReadOnly Property ColorblindFriendly As IPalette = New Palettes.ColorblindFriendly()
        Public Shared ReadOnly Property Dark As IPalette = New Palettes.Dark()
        Public Shared ReadOnly Property DarkPastel As IPalette = New Palettes.DarkPastel()
        Public Shared ReadOnly Property Frost As IPalette = New Palettes.Frost()
        Public Shared ReadOnly Property Ocean As IPalette = New Palettes.LightOcean()
        Public Shared ReadOnly Property Spectrum As IPalette = New Palettes.LightSpectrum()
        Public Shared ReadOnly Property Microcharts As IPalette = New Palettes.Microcharts()
        Public Shared ReadOnly Property Nero As IPalette = New Palettes.Nero()
        Public Shared ReadOnly Property Nord As IPalette = New Palettes.Nord()
        Public Shared ReadOnly Property OneHalf As IPalette = New Palettes.OneHalf()
        Public Shared ReadOnly Property OneHalfDark As IPalette = New Palettes.OneHalfDark()
        Public Shared ReadOnly Property PolarNight As IPalette = New Palettes.PolarNight()
        Public Shared ReadOnly Property Redness As IPalette = New Palettes.Redness()
        Public Shared ReadOnly Property SnowStorm As IPalette = New Palettes.SnowStorm()
        Public Shared ReadOnly Property Tsitsulin As IPalette = New Palettes.Tsitsulin()

        <Obsolete("This class is obsolete. Create custom palettes by calling: ScottPlot.Palette.FromHtmlColors()", True)>
        Public Sub New()
        End Sub

    End Class

End Namespace