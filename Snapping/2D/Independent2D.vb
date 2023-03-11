Namespace ScottPlot.SnapLogic

    ''' <summary>
    ''' Customizable 2D snap system from independent 1D snap objects.
    ''' </summary>
    Public Class Independent2D
        Implements ISnap2D

        Public Property SnapX As ISnap1D = New NoSnap1D()
        Public Property SnapY As ISnap1D = New NoSnap1D()

        Public Sub New()
        End Sub

        Public Sub New(x As ISnap1D, y As ISnap1D)
            SnapX = x
            SnapY = y
        End Sub

        Public Function Snap(value As Coordinate) As Coordinate Implements ScottPlot.SnapLogic.ISnap2D.Snap
            Dim x As Double = SnapX.Snap(value.X)
            Dim y As Double = SnapY.Snap(value.Y)
            Return New Coordinate(x, y)
        End Function

    End Class

End Namespace