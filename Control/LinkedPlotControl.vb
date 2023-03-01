Namespace ScottPlot.Control

    Public Structure LinkedPlotControl

        Public PlotControl As IPlotControl
        Public LinkHorizontal As Boolean
        Public LinkVertical As Boolean
        Public LinkLayout As Boolean

        Public Sub New(plotControl As IPlotControl, linkHorizontal As Boolean, linkVertical As Boolean, linkLayout As Boolean)
            Me.PlotControl = plotControl
            Me.LinkVertical = linkVertical
            Me.LinkHorizontal = linkHorizontal
            Me.LinkLayout = linkLayout
        End Sub

    End Structure

End Namespace