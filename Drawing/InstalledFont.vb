Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Drawing

    ''' <summary>
    ''' This class is used to retrieve OS-agnostic fonts using those known to be installed on the system.
    ''' </summary>
    Public Module InstalledFont

        Private ReadOnly InstalledFonts As Dictionary(Of String, FontFamily)

        Friend ReadOnly Property SerifFamily As FontFamily
        Friend ReadOnly Property SansFamily As FontFamily
        Friend ReadOnly Property MonospaceFamily As FontFamily

        Sub New()
            ' non-Windows systems may have multiple fonts with the same names
            InstalledFonts = FontFamily.Families _
                .GroupBy(Function(x) x.Name, StringComparer.InvariantCultureIgnoreCase) _
                .ToDictionary(Function(x) x.Key, Function(x) x.First(), StringComparer.InvariantCultureIgnoreCase)

            SerifFamily = ValidFontFamily(New String() {"Times New Roman", "DejaVu Serif", "Times"})
            SansFamily = ValidFontFamily(New String() {"Segoe UI", "DejaVu Sans", "Helvetica"})
            MonospaceFamily = ValidFontFamily(New String() {"Consolas", "DejaVu Sans Mono", "Courier"})
        End Sub

        Public Function [Default]() As String
            Return InstalledFont.SansFamily.Name
        End Function

        Public Function Serif() As String
            Return InstalledFont.SerifFamily.Name
        End Function

        Public Function Sans() As String
            Return InstalledFont.SansFamily.Name
        End Function

        Public Function Monospace() As String
            Return InstalledFont.MonospaceFamily.Name
        End Function

        Public Function Names() As String()
            Return InstalledFonts.Values.Select(Function(x As FontFamily) x.Name).ToArray()
        End Function

        ''' <summary>
        ''' Returns a font name guaranteed to be installed on the system.
        ''' </summary>
        Public Function ValidFontName(fontName As String) As String
            Return InstalledFont.ValidFontFamily(fontName).Name
        End Function

        ''' <summary>
        ''' Returns a font name guaranteed to be installed on the system.
        ''' </summary>
        Public Function ValidFontName(fontNames As String()) As String
            Return InstalledFont.ValidFontFamily(fontNames).Name
        End Function

        ''' <summary>
        ''' Returns a font family guaranteed to be installed on the system.
        ''' </summary>
        Public Function ValidFontFamily(fontName As String) As FontFamily
            If (fontName IsNot Nothing) AndAlso InstalledFont.InstalledFonts.ContainsKey(fontName) Then
                Return InstalledFont.InstalledFonts(fontName)
            End If
            Return SystemFonts.DefaultFont.FontFamily
        End Function

        ''' <summary>
        ''' Returns a font family guaranteed to be installed on the system.
        ''' </summary>
        Public Function ValidFontFamily(fontNames As String()) As FontFamily
            For Each name As String In fontNames
                If (name IsNot Nothing) AndAlso InstalledFont.InstalledFonts.ContainsKey(name) Then
                    Return InstalledFont.InstalledFonts(name)
                End If
            Next
            Return SystemFonts.DefaultFont.FontFamily
        End Function

    End Module

End Namespace