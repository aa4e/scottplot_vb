Namespace ScottPlot.SnapLogic

    ''' <summary>
    ''' Always returns the original value (snapping disabled).
    ''' </summary>
    Public Class NoSnap2D
        Implements ISnap2D

        Public Function Snap(value As Coordinate) As Coordinate Implements ScottPlot.SnapLogic.ISnap2D.Snap
            Return value
        End Function

    End Class

End Namespace