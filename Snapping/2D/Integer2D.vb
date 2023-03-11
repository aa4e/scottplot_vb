Namespace ScottPlot.SnapLogic

    ''' <summary>
    ''' Returns the given position snapped to the nearest integer positions.
    ''' </summary>
    Public Class Integer2D
        Implements ISnap2D

        Public Function Snap(value As Coordinate) As Coordinate Implements ScottPlot.SnapLogic.ISnap2D.Snap
            Dim x As Double = Math.Round(value.X)
            Dim y As Double = Math.Round(value.Y)
            Return New Coordinate(x, y)
        End Function

    End Class

End Namespace