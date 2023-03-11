Imports System.Linq
Imports System.Reflection

Namespace ScottPlot

    ''' <summary>
    ''' Fields and methods for accessing color palettes.
    ''' </summary>
    Public Module Palette

        'TODO Add rest

        Public ReadOnly Property Amber As IPalette = New Palettes.Amber()
        Public ReadOnly Property Aurora As IPalette = New Palettes.Aurora()
        Public ReadOnly Property Category10 As IPalette = New Palettes.Category10()
        Public ReadOnly Property Category20 As IPalette = New Palettes.Category20()
        Public ReadOnly Property ColorblindFriendly As IPalette = New Palettes.ColorblindFriendly()
        Public ReadOnly Property Dark As IPalette = New Palettes.Dark()
        Public ReadOnly Property DarkPastel As IPalette = New Palettes.DarkPastel()
        Public ReadOnly Property Frost As IPalette = New Palettes.Frost()
        Public ReadOnly Property Microcharts As IPalette = New Palettes.Microcharts()
        Public ReadOnly Property Nero As IPalette = New Palettes.Nero()
        Public ReadOnly Property Nord As IPalette = New Palettes.Nord()
        Public ReadOnly Property OneHalf As IPalette = New Palettes.OneHalf()
        Public ReadOnly Property Ocean As IPalette = New Palettes.LightOcean()
        Public ReadOnly Property OneHalfDark As IPalette = New Palettes.OneHalfDark()
        Public ReadOnly Property PolarNight As IPalette = New Palettes.PolarNight()
        Public ReadOnly Property Redness As IPalette = New Palettes.Redness()
        Public ReadOnly Property SnowStorm As IPalette = New Palettes.SnowStorm()
        Public ReadOnly Property Spectrum As IPalette = New Palettes.LightSpectrum()
        Public ReadOnly Property Tsitsulin As IPalette = New Palettes.Tsitsulin()

        ''' <summary>
        ''' Create a new color palette from an array of HTML colors.
        ''' </summary>
        Public Function FromHtmlColors(htmlColors As String()) As IPalette
            Return New Palettes.Custom(htmlColors)
        End Function

        ''' <summary>
        ''' Return an array containing every available palette.
        ''' </summary>
        Public Function GetPalettes() As IPalette()
            Return Assembly.GetExecutingAssembly().GetTypes() _
                .Where(Function(x) x.IsClass) _
                .Where(Function(x) Not x.IsAbstract) _
                .Where(Function(x) x.GetInterfaces().Contains(GetType(ScottPlot.IPalette))) _
                .Where(Function(x) x.GetConstructors().Where(Function(y) (y.GetParameters().Count() = 0)).Any()) _
                .Select(Function(x) CType(Activator.CreateInstance(x), IPalette)).ToArray()
        End Function

    End Module

End Namespace