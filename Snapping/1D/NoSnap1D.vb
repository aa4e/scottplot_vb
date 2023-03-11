Namespace ScottPlot.SnapLogic

    ''' <summary>
    ''' Always returns the original value (snapping disabled).
    ''' </summary>
    Public Class NoSnap1D
        Implements ISnap1D

        Public Function Snap(value As Double) As Double Implements ScottPlot.SnapLogic.ISnap1D.Snap
            Return value
        End Function

    End Class

End Namespace