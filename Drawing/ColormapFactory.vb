Imports System.Collections.Generic
Imports System.Linq

Namespace ScottPlot.Drawing

    Public Class ColormapFactory

        Private ReadOnly Colormaps As New Dictionary(Of String, IColormap) From {
            {"Algae", New Colormaps.Algae()},
            {"Amp", New Colormaps.Amp()},
            {"Balance", New Colormaps.Balance()},
            {"Blues", New Colormaps.Blues()},
            {"Curl", New Colormaps.Curl()},
            {"Deep", New Colormaps.Deep()},
            {"Delta", New Colormaps.Delta()},
            {"Dense", New Colormaps.Dense()},
            {"Diff", New Colormaps.Diff()},
            {"Grayscale", New Colormaps.Grayscale()},
            {"GrayscaleR", New Colormaps.GrayscaleR()},
            {"Greens", New Colormaps.Greens()},
            {"Haline", New Colormaps.Haline()},
            {"Ice", New Colormaps.Ice()},
            {"Inferno", New Colormaps.Inferno()},
            {"Jet", New Colormaps.Jet()},
            {"Magma", New Colormaps.Magma()},
            {"Matter", New Colormaps.Matter()},
            {"Oxy", New Colormaps.Oxy()},
            {"Phase", New Colormaps.Phase()},
            {"Plasma", New Colormaps.Plasma()},
            {"Rain", New Colormaps.Rain()},
            {"Solar", New Colormaps.Solar()},
            {"Speed", New Colormaps.Speed()},
            {"Tarn", New Colormaps.Tarn()},
            {"Tempo", New Colormaps.Tempo()},
            {"Thermal", New Colormaps.Thermal()},
            {"Topo", New Colormaps.Topo()},
            {"Turbid", New Colormaps.Turbid()},
            {"Turbo", New Colormaps.Turbo()},
            {"Viridis", New Colormaps.Viridis()}
        }

        Public Function GetAvailableNames() As IEnumerable(Of String)
            Return Me.Colormaps.Keys
        End Function

        Public Function GetAvailableColormaps() As IEnumerable(Of Colormap)
            Return Colormaps.Values.Select(Function(x) New Colormap(x))
        End Function

        Public Function GetDefaultColormap() As IColormap
            Return New Colormaps.Grayscale()
        End Function

        Public Function CreateOrDefault(name As String) As Colormap
            Dim cmap As IColormap = Nothing
            If Colormaps.TryGetValue(name, cmap) Then
                Return New Colormap(cmap)
            Else
                Return New Colormap(GetDefaultColormap())
            End If
        End Function

        Public Function CreateOrThrow(name As String) As Colormap
            Dim cmap As IColormap = Nothing
            If Colormaps.TryGetValue(name, cmap) Then
                Return New Colormap(cmap)
            End If
            Throw New ArgumentOutOfRangeException($"No colormap with name '{name}'.")
        End Function

    End Class

End Namespace