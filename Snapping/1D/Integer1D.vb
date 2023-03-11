Namespace ScottPlot.SnapLogic

    ''' <summary>
    ''' Snaps to the nearest integer position.
    ''' </summary>
    Public Class Integer1D
        Implements ISnap1D

        Public Function Snap(value As Double) As Double Implements ScottPlot.SnapLogic.ISnap1D.Snap
            Return Math.Round(value)
        End Function

    End Class

End Namespace