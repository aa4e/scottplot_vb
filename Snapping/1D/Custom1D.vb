Namespace ScottPlot.SnapLogic

    ''' <summary>
    ''' Create a snap object from a custom function.
    ''' </summary>
    Public Class Custom1D
        Implements ISnap1D

        Private SnapFunction As Func(Of Double, Double)

        Public Sub New(snapFunction As Func(Of Double, Double))
            Me.SnapFunction = snapFunction
        End Sub

        Public Function Snap(value As Double) As Double Implements ScottPlot.SnapLogic.ISnap1D.Snap
            Return Me.SnapFunction(value)
        End Function

    End Class

End Namespace