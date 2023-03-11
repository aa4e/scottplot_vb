Imports System.Drawing

Namespace ScottPlot.Plottable

    <Obsolete("This plot type is deprecated: Use a regular scatter plot and call GetPointNearest(). See examples in documentation.", True)>
    Public Class ScatterPlotHighlight
        Inherits ScatterPlot
        Implements IPlottable, IHasPoints, IHasPointsGeneric(Of Double, Double), IHasPointsGenericX(Of Double, Double), IHasPointsGenericY(Of Double, Double), IHasHighlightablePoints

        Public Property HighlightedShape As MarkerShape = MarkerShape.OpenCircle
        Public Property HighlightedMarkerSize As Single = 10
        Public Property HighlightedColor As Color = Color.Red
        Protected Shadows Property IsHighlighted As Boolean()

        Public Sub New(xs As Double(), ys As Double(), Optional xErr As Double() = Nothing, Optional yErr As Double() = Nothing)
            MyBase.New(xs, ys, xErr, yErr)
            HighlightClear()
        End Sub

        Public Shadows Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Throw New NotImplementedException()
        End Sub

        Public Sub HighlightClear() Implements ScottPlot.Plottable.IHasHighlightablePoints.HighlightClear
            Throw New NotImplementedException()
        End Sub

        Public Function HighlightPoint(index As Integer) As Tuple(Of Double, Double, Integer) Implements ScottPlot.Plottable.IHasHighlightablePoints.HighlightPoint
            Throw New NotImplementedException()
        End Function

        Public Function HighlightPointNearestX(x As Double) As Tuple(Of Double, Double, Integer) Implements ScottPlot.Plottable.IHasHighlightablePoints.HighlightPointNearestX
            Throw New NotImplementedException()
        End Function

        Public Function HighlightPointNearestY(y As Double) As Tuple(Of Double, Double, Integer) Implements ScottPlot.Plottable.IHasHighlightablePoints.HighlightPointNearestY
            Throw New NotImplementedException()
        End Function

        Public Function HighlightPointNearest(x As Double, y As Double) As Tuple(Of Double, Double, Integer) Implements ScottPlot.Plottable.IHasHighlightablePoints.HighlightPointNearest
            Throw New NotImplementedException()
        End Function

    End Class

End Namespace