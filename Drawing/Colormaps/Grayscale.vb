﻿Namespace ScottPlot.Drawing.Colormaps

    Public Class Grayscale
        Implements IColormap

        Public ReadOnly Property Name As String = "Grayscale" Implements ScottPlot.Drawing.IColormap.Name

        Public Function GetRGB(value As Byte) As Tuple(Of Byte, Byte, Byte) Implements ScottPlot.Drawing.IColormap.GetRGB
            Return New Tuple(Of Byte, Byte, Byte)(value, value, value)
        End Function

    End Class

End Namespace