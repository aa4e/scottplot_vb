Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Display circles of user-defined sizes and colors at specific X/Y positions.
    ''' </summary>
    Public Class BubblePlot
        Implements IPlottable

#Region "NESTED TYPES"

        Private Structure Bubble
            Public X As Double
            Public Y As Double
            Public Radius As Single
            Public FillColor As Color
            Public EdgeWidth As Single
            Public EdgeColor As Color
        End Structure

#End Region '/NESTED TYPES

#Region "PROPS, FIELDS"

        Private ReadOnly Bubbles As New List(Of BubblePlot.Bubble)()

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"BubblePlot with {Bubbles.Count} bubbles."
        End Function

        ''' <summary>
        ''' Clear all bubbles.
        ''' </summary>
        Public Sub Clear()
            Bubbles.Clear()
        End Sub

        ''' <summary>
        ''' Add a single bubble.
        ''' </summary>
        ''' <param name="x">Horizontal position (in coordinate space).</param>
        ''' <param name="y">Horizontal vertical (in coordinate space).</param>
        ''' <param name="radius">Size of the bubble (in pixels).</param>
        ''' <param name="edgeWidth">Size of the outline (in pixels).</param>
        Public Sub Add(x As Double, y As Double, radius As Double, fillColor As Color, edgeWidth As Double, edgeColor As Color)
            'TODO: inconsistent argumen tnames in overloads (radius vs size)
            Bubbles.Add(New BubblePlot.Bubble() With {
                        .X = x,
                        .Y = y,
                        .Radius = CSng(radius),
                        .FillColor = fillColor,
                        .EdgeWidth = CSng(edgeWidth),
                        .EdgeColor = edgeColor})
        End Sub

        ''' <summary>
        ''' Add many bubbles with the same size and style.
        ''' </summary>
        Public Sub Add(xs As Double(), ys As Double(), size As Double, fillColor As Color, edgeWidth As Double, edgeColor As Color)
            If (xs is Nothing) OrElse (ys is Nothing) Then
                Throw New ArgumentException("Xs and Ys cannot be null.")
            End If
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must have the same number of elements.")
            End If
            For i As Integer = 0 To xs.Length - 1
                Bubbles.Add(New BubblePlot.Bubble() With {
                            .X = xs(i),
                            .Y = ys(i),
                            .Radius = CSng(size),
                            .FillColor = fillColor,
                            .EdgeWidth = CSng(edgeWidth),
                            .EdgeColor = edgeColor})
            Next
        End Sub

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If (bubbles.Count = 0) Then
                Return AxisLimits.NoLimits
            End If
            Dim xs = Bubbles.Select(Function(b) b.X)
            Dim ys = Bubbles.Select(Function(b) b.Y)
            Return New AxisLimits(xs.Min(), xs.Max(), ys.Min(), ys.Max())
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            For Each bubble As BubblePlot.Bubble In Me.Bubbles
                If Double.IsNaN(bubble.X) OrElse Double.IsNaN(bubble.Y) OrElse Double.IsNaN(bubble.Radius) OrElse Double.IsNaN(bubble.EdgeWidth) Then
                    Throw New InvalidOperationException("Bubble positions and sizes must not be NaN.")
                End If
                If Double.IsInfinity(bubble.X) OrElse Double.IsInfinity(bubble.Y) OrElse Double.IsInfinity(bubble.Radius) OrElse Double.IsInfinity(bubble.EdgeWidth) Then
                    Throw New InvalidOperationException("Bubble position and size must real.")
                End If
                If (bubble.Radius < 0) OrElse (bubble.EdgeWidth < 0) Then
                    Throw New InvalidOperationException("Bubble sizes cannot be negative.")
                End If
            Next
        End Sub

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                    brsh As Brush = Drawing.GDI.Brush(Color.Magenta),
                    pen As Pen = Drawing.GDI.Pen(Color.Black)

                    For Each bubble As BubblePlot.Bubble In Bubbles
                        Dim pixelX As Single = dims.GetPixelX(bubble.X)
                        Dim pixelY As Single = dims.GetPixelY(bubble.Y)
                        Dim radiusPx As Single = bubble.Radius

                        Dim location As New PointF(pixelX - radiusPx, pixelY - radiusPx)
                        Dim size As New SizeF(radiusPx * 2, radiusPx * 2)
                        Dim rect As New RectangleF(location, size)

                        CType(brsh, SolidBrush).Color = bubble.FillColor
                        gfx.FillEllipse(brsh, rect)

                        pen.Color = bubble.EdgeColor
                        pen.Width = bubble.EdgeWidth
                        gfx.DrawEllipse(pen, rect)
                    Next
                End Using
            End If
        End Sub

        ''' <summary>
        ''' Return the X/Y coordinates of the point nearest the X position.
        ''' </summary>
        ''' <param name="x">X position in plot space.</param>
        Public Function GetPointNearestX(x As Double) As Tuple(Of Double, Double, Integer)
            If (Bubbles.Count = 0) Then
                Throw New InvalidOperationException("BubblePlot is empty.")
            End If
            Dim closestBubbleDistance As Double = Double.PositiveInfinity
            Dim closestBubbleIndex As Integer = 0
            For i As Integer = 0 To Bubbles.Count - 1
                Dim currDistance As Double = Math.Abs(Bubbles(i).X - x)
                If (currDistance < closestBubbleDistance) Then
                    closestBubbleIndex = i
                    closestBubbleDistance = currDistance
                End If
            Next
            Return New Tuple(Of Double, Double, Integer)(Bubbles(closestBubbleIndex).X, Bubbles(closestBubbleIndex).Y, closestBubbleIndex)
        End Function

        ''' <summary>
        ''' Return the X/Y coordinates of the point nearest the Y position.
        ''' </summary>
        ''' <param name="y">Y position in plot space.</param>
        Public Function GetPointNearestY(y As Double) As Tuple(Of Double, Double, Integer)
            If (Bubbles.Count = 0) Then
                Throw New InvalidOperationException("BubblePlot is empty.")
            End If
            Dim closestBubbleDistance As Double = Double.PositiveInfinity
            Dim closestBubbleIndex As Integer = 0
            For i As Integer = 0 To Bubbles.Count - 1
                Dim currDistance As Double = Math.Abs(Bubbles(i).Y - y)
                If (currDistance < closestBubbleDistance) Then
                    closestBubbleIndex = i
                    closestBubbleDistance = currDistance
                End If
            Next
            Return New Tuple(Of Double, Double, Integer)(Bubbles(closestBubbleIndex).X, Bubbles(closestBubbleIndex).Y, closestBubbleIndex)
        End Function

        ''' <summary>
        ''' Return the position and index of the data point nearest the given coordinate.
        ''' </summary>
        ''' <param name="x">Location in coordinate space.</param>
        ''' <param name="y">Location in coordinate space.</param>
        ''' <param name="xyRatio">Ratio of pixels per unit (X/Y) when rendered.</param>
        Public Function GetPointNearest(x As Double, y As Double, Optional xyRatio As Double = 1) As Tuple(Of Double, Double, Integer)
            If (Bubbles.Count = 0) Then
                Throw New InvalidOperationException("BubblePlot is empty.")
            End If

            Dim xyRatioSquared As Double = xyRatio * xyRatio
            Dim pointDistanceSquared = Function(x1 As Double, y1 As Double)
                                           Return (x1 - x) * (x1 - x) * xyRatioSquared + (y1 - y) * (y1 - y)
                                       End Function

            Dim closestBubbleDistance As Double = Double.PositiveInfinity
            Dim closestBubbleIndex As Integer = 0
            For i As Integer = 0 To Bubbles.Count - 1
                Dim currDistance As Double = pointDistanceSquared(Bubbles(i).X, Bubbles(i).Y)
                If (currDistance < closestBubbleDistance) Then
                    closestBubbleIndex = i
                    closestBubbleDistance = currDistance
                End If
            Next

            Return New Tuple(Of Double, Double, Integer)(Bubbles(closestBubbleIndex).X, Bubbles(closestBubbleIndex).Y, closestBubbleIndex)
        End Function

#End Region '/METHODS

    End Class

End Namespace