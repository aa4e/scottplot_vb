Namespace ScottPlot.Drawing

    Public Interface IColormap

        Function GetRGB(value As Byte) As Tuple(Of Byte, Byte, Byte)
        ReadOnly Property Name As String

    End Interface

End Namespace