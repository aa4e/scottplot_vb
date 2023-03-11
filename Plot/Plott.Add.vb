Imports System
Imports System.Drawing
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Linq
Imports System.Collections.Generic

' This file contains helper methods for creating plottables, customizing them based on optional arguments, 
' adding them to the plot, then returning them for additional customization all with a single method call.
' 
' Plottable-creating helper methods try to obey these rules:
' 
'   1. Only the most common plot types get helper methods.
'      Uncommon Or experimental plottables can be created by the user and added with Add().
'   
'   2. Only the most common styling options are configurable with optional arguments.
'      This is subjective, but guided by what is in the cookbook and often seen in the wild.
'      Plottables are always returned by helper methods, so users can customize them extensively as desired.

Namespace ScottPlot

    Partial Class Plot

        ''' <summary>
        ''' Display text in the data area at a pixel location (not a X/Y coordinates).
        ''' </summary>
        Public Function AddAnnotation(label As String, x As Double, y As Double) As Plottable.Annotation
            Dim plottable As New Plottable.Annotation() With {.Label = label, .X = x, .Y = y}
            Add(plottable)
            Return plottable
        End Function

        ''' <summary>
        ''' Display an arrow pointing to a spot in coordinate space.
        ''' </summary>
        Public Function AddArrow(xTip As Double, yTip As Double, xBase As Double, yBase As Double, Optional lineWidth As Single = 5.0F, Optional color As Color? = Nothing) As Plottable.ArrowCoordinated
            Dim plottable As New Plottable.ArrowCoordinated(xBase, yBase, xTip, yTip) With {
                .LineWidth = lineWidth,
                .Color = If(color, GetNextColor())
            }
            Add(plottable)
            Return plottable
        End Function

        ''' <summary>
        ''' Add a bracket to highlight a range between two points in coordinate space with an optional label.
        ''' </summary>
        Public Function AddBracket(x1 As Double, y1 As Double, x2 As Double, y2 As Double, Optional label As String = Nothing) As Plottable.Bracket
            Dim bracket As New Plottable.Bracket(x1, y1, x2, y2) With {.Label = label}
            Add(bracket)
            Return bracket
        End Function

        ''' <summary>
        ''' Add a bracket to highlight a range between two points in coordinate space with an optional label.
        ''' </summary>
        Public Function AddBracket(point1 As Coordinate, point2 As Coordinate, Optional label As String = Nothing) As Plottable.Bracket
            Return AddBracket(point1.X, point1.Y, point2.X, point2.Y, label)
        End Function

        ''' <summary>
        ''' Add a Cleveland Dot plot for the given values. Cleveland Dots will be placed at X positions 0, 1, 2, etc.
        ''' </summary>
        Public Function AddClevelandDot(ys1 As Double(), ys2 As Double()) As Plottable.ClevelandDotPlot
            Dim xs As Double() = DataGen.Consecutive(ys1.Length)
            Dim clevelandDotPlot As New Plottable.ClevelandDotPlot(xs, ys1, ys2)
            Add(clevelandDotPlot)
            Return clevelandDotPlot
        End Function

        ''' <summary>
        ''' Add a Cleveland Dot plot for the given values using defined dot positions.
        ''' </summary>
        Public Function AddClevelandDot(ys1 As Double(), ys2 As Double(), positions As Double()) As Plottable.ClevelandDotPlot
            Dim clevelandDotPlot As New Plottable.ClevelandDotPlot(positions, ys1, ys2)
            Add(clevelandDotPlot)
            Return clevelandDotPlot
        End Function

        ''' <summary>
        ''' Add a Lollipop plot for the given values. Lollipops will be placed at X positions 0, 1, 2, etc.
        ''' </summary>
        Public Function AddLollipop(values As Double(), Optional color As Color? = Nothing) As Plottable.LollipopPlot
            Dim xs As Double() = DataGen.Consecutive(values.Length)
            Dim lollipopPlot As New Plottable.LollipopPlot(xs, values) With {
                .LollipopColor = (If(color, GetNextColor()))
            }
            Add(lollipopPlot)
            Return lollipopPlot
        End Function

        ''' <summary>
        ''' Add a lollipop plot for the given values using defined lollipop positions.
        ''' </summary>
        Public Function AddLollipop(values As Double(), positions As Double(), Optional color As Color? = Nothing) As Plottable.LollipopPlot
            Dim lollipopPlot As New Plottable.LollipopPlot(positions, values) With {
                .LollipopColor = If(color, GetNextColor())
            }
            Add(lollipopPlot)
            Return lollipopPlot
        End Function

        ''' <summary>
        ''' Add a bar plot for the given values. Bars will be placed at X positions 0, 1, 2, etc.
        ''' </summary>
        Public Function AddBar(values As Double(), Optional color As Color? = Nothing) As Plottable.BarPlot
            Dim xs As Double() = DataGen.Consecutive(values.Length)
            Dim barPlot As New Plottable.BarPlot(xs, values, Nothing, Nothing) With {
                .FillColor = If(color, GetNextColor())
            }
            Add(barPlot)
            Return barPlot
        End Function

        ''' <summary>
        ''' Add a bar plot for the given values using defined bar positions.
        ''' </summary>
        Public Function AddBar(values As Double(), positions As Double(), Optional color As Color? = Nothing) As Plottable.BarPlot
            Dim barPlot As New Plottable.BarPlot(positions, values, Nothing, Nothing) With {
                .FillColor = If(color, GetNextColor())
            }
            Add(barPlot)
            Return barPlot
        End Function

        ''' <summary>
        ''' Add a bar plot (values +/- errors) using defined positions.
        ''' </summary>
        Public Function AddBar(values As Double(), errors As Double(), positions As Double(), Optional color As Color? = Nothing) As Plottable.BarPlot
            Dim barPlot As New Plottable.BarPlot(positions, values, errors, Nothing) With {
                .FillColor = If(color, GetNextColor()),
                .FillColorNegative = If(color, GetNextColor())
            }
            Add(barPlot)
            Return barPlot
        End Function

        ''' <summary>
        ''' Create a series of bar plots and customize the ticks and legend.
        ''' </summary>
        Public Function AddBarGroups(groupLabels As String(), seriesLabels As String(), ys As Double()(), yErr As Double()()) As Plottable.BarPlot()
            If (groupLabels is Nothing) OrElse (seriesLabels is Nothing) OrElse (ys is Nothing) Then
                Throw New ArgumentException("Labels and ys cannot be null.")
            End If
            If (seriesLabels.Length <> ys.Length) Then
                Throw New ArgumentException("GroupLabels and ys must be the same length.")
            End If
            For i As Integer = 0 To ys.Length - 1
                If (ys(i).Length <> groupLabels.Length) Then
                    Throw New ArgumentException("All arrays inside ys must be the same length as groupLabels.")
                End If
            Next

            Dim groupWidthFraction As Double = 0.8
            Dim barWidthFraction As Double = 0.8
            Dim errorCapSize As Double = 0.38

            Dim seriesCount As Integer = ys.Length
            Dim barWidth As Double = groupWidthFraction / seriesCount
            Dim bars As Plottable.BarPlot() = New Plottable.BarPlot(seriesCount - 1) {}
            Dim containsNegativeY As Boolean = False
            For i As Integer = 0 To seriesCount - 1
                Dim barYs As Double() = ys(i)
                Dim barYerr As Double() = If(yErr IsNot Nothing, yErr(i), Nothing)
                Dim barXs As Double() = DataGen.Consecutive(barYs.Length)

                containsNegativeY = containsNegativeY Or barYs.Where(Function(y) y < 0).Any()

                Dim bar As New Plottable.BarPlot(barXs, barYs, barYerr, Nothing) With {
                    .Label = seriesLabels(i),
                    .BarWidth = barWidth * barWidthFraction,
                    .PositionOffset = i * barWidth,
                    .ErrorCapSize = errorCapSize,
                    .FillColor = GetNextColor()
                }
                bars(i) = bar
                Add(bar)
            Next
            If containsNegativeY Then
                AxisAuto(Nothing, Nothing)
            End If

            Dim groupPositions As Double() = DataGen.Consecutive(groupLabels.Length, 1,
                                                                 (groupWidthFraction - barWidth) / 2)
            XTicks(groupPositions, groupLabels)
            Return bars
        End Function

        ''' <summary>
        ''' Create an empty Plottable. BarSeries, add it to the plot, and return it. Use its Add() method to add bars.
        ''' </summary>
        Public Function AddBarSeries() As Plottable.BarSeries
            Dim barSeries As New Plottable.BarSeries()
            Add(barSeries)
            Return barSeries
        End Function

        ''' <summary>
        ''' Create a Plottable. BarSeries filled with the given bars, add it to the plot, and return it.
        ''' </summary>
        Public Function AddBarSeries(bars As List(Of Plottable.Bar)) As Plottable.BarSeries
            Dim barSeries As New Plottable.BarSeries(bars)
            Add(barSeries)
            Return barSeries
        End Function

        ''' <summary>
        ''' Create a Plottable. BarSeries filled with the given bars, add it to the plot, and return it.
        ''' </summary>
        Public Function AddBarSeries(bars As Plottable.Bar()) As Plottable.BarSeries
            Dim barSeries As New Plottable.BarSeries(bars.ToList())
            Add(barSeries)
            Return barSeries
        End Function

        ''' <summary>
        ''' Add an empty bubble plot. Call it's Add() method to add bubbles with custom position and styling.
        ''' </summary>
        Public Function AddBubblePlot() As Plottable.BubblePlot
            Dim bubblePlot As New Plottable.BubblePlot()
            Add(bubblePlot)
            Return bubblePlot
        End Function

        ''' <summary>
        ''' Add a bubble plot with multiple bubbles at the given positions all styled the same.
        ''' Call the Add() method to add bubbles manually, allowing further customization of size and style.
        ''' </summary>
        Public Function AddBubblePlot(xs As Double(), ys As Double(), Optional radius As Double = 10, Optional fillColor As Color? = Nothing, Optional edgeWidth As Double = 1, Optional edgeColor As Color? = Nothing) As Plottable.BubblePlot
            Dim bubblePlot As New Plottable.BubblePlot()
            bubblePlot.Add(xs, ys, radius, If(fillColor, GetNextColor()), edgeWidth, If(edgeColor, Color.Black))
            Add(bubblePlot)
            Return bubblePlot
        End Function

        ''' <summary>
        ''' Add candlesticks to the chart from OHLC (open, high, low, close) data.
        ''' </summary>
        Public Function AddCandlesticks(ohlcs As OHLC()) As Plottable.FinancePlot
            Dim financePlot As New Plottable.FinancePlot(ohlcs) With {
                .Candle = True,
                .ColorUp = ColorTranslator.FromHtml("#26a69a"),
                .ColorDown = ColorTranslator.FromHtml("#ef5350")}
            Add(financePlot)
            Return financePlot
        End Function

        ''' <summary>
        ''' Add a colorbar to display a colormap beside the data area.
        ''' </summary>
        ''' <param name="colormap">Colormap to display in this colorbar.</param>
        ''' <param name="space">The size of the right axis will be set to this number of pixels to make room for the colorbar.</param>
        ''' <param name="rightSide">If false the colorbar will be displayed on the left edge of the plot.</param>
        ''' <returns>the colorbar that was just created.</returns>
        Public Function AddColorbar(Optional colormap As ScottPlot.Drawing.Colormap = Nothing, Optional space As Integer = 100, Optional rightSide As Boolean = True) As Plottable.Colorbar
            Dim cb As New Plottable.Colorbar(colormap)
            If rightSide Then
                cb.Edge = Renderable.Edge.Right
                YAxis2.SetSizeLimit(space)
            Else
                cb.Edge = Renderable.Edge.Left
                YAxis.SetSizeLimit(space)
            End If
            Add(cb)
            Return cb
        End Function

        ''' <summary>
        ''' Add a colorbar initialized with settings from a heatmap.
        ''' </summary>
        ''' <param name="heatmap">A heatmap-containing plottable to connect with this colorbar</param>
        ''' <param name="space">The size of the right axis will be set to this number of pixels to make room for the colorbar</param>
        ''' <returns>the colorbar that was just created</returns>
        Public Function AddColorbar(heatmap As Plottable.IHasColormap, Optional space As Integer = 100) As Plottable.Colorbar
            Dim cb As New Plottable.Colorbar(heatmap)
            Add(cb)
            YAxis2.SetSizeLimit(space)
            Return cb
        End Function

        ''' <summary>
        ''' Add a crosshair to the plot.
        ''' </summary>
        ''' <param name="x">position of vertical line (axis units).</param>
        ''' <param name="y">position of horizontal line (axis units).</param>
        ''' <returns>the crosshair that was just created.</returns>
        Public Function AddCrosshair(x As Double, y As Double) As Plottable.Crosshair
            Dim cb As New Plottable.Crosshair() With {.X = x, .Y = y}
            Add(cb)
            Return cb
        End Function

        ''' <summary>
        ''' Create a polygon to fill the area between Y values and a baseline.
        ''' </summary>
        Public Function AddFill(xs As Double(), ys As Double(),
                                Optional baseline As Double = 0, Optional color As Color? = Nothing,
                                Optional lineWidth As Double = 0, Optional lineColor As Color? = Nothing) As Plottable.Polygon
            Dim polygon As New Plottable.Polygon(Tools.Pad(xs, cloneEdges:=True),
                                                 Tools.Pad(ys, 1, baseline, baseline)) With {
                                                 .Fill = True,
                                                 .FillColor = If(color, GetNextColor(0.5)),
                                                 .LineWidth = lineWidth,
                                                 .LineColor = If(lineColor, System.Drawing.Color.Black)}
            Add(polygon)
            Return polygon
        End Function

        ''' <summary>
        ''' Create a polygon to fill the area between two Y curves that share the same X positions.
        ''' </summary>
        Public Function AddFill(xs As Double(), ys1 As Double(), ys2 As Double(),
                                Optional color As Color? = Nothing,
                                Optional lineWidth As Double = 0, Optional lineColor As Color? = Nothing) As Plottable.Polygon
            Dim polyXs As Double() = xs.Concat(xs.Reverse()).ToArray()
            Dim polyYs As Double() = ys1.Concat(ys2.Reverse()).ToArray()
            Dim polygon As New Plottable.Polygon(polyXs, polyYs) With {
                .Fill = True,
                .FillColor = If(color, Me.GetNextColor(0.5)),
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, System.Drawing.Color.Black)}
            Add(polygon)
            Return polygon
        End Function

        ''' <summary>
        ''' Create a polygon to fill the area between Y values of two curves.
        ''' </summary>
        Public Function AddFill(xs1 As Double(), ys1 As Double(), xs2 As Double(), ys2 As Double(),
                                Optional color As Color? = Nothing,
                                Optional lineWidth As Double = 0, Optional lineColor As Color? = Nothing) As Plottable.Polygon
            'combine xs and ys to make one big curve
            Dim pointCount As Integer = xs1.Length + xs2.Length
            Dim bothX As Double() = New Double(pointCount - 1) {}
            Dim bothY As Double() = New Double(pointCount - 1) {}

            'copy the first dataset as-is
            Array.Copy(xs1, 0, bothX, 0, xs1.Length)
            Array.Copy(ys1, 0, bothY, 0, ys1.Length)

            'copy the second dataset in reverse order
            For i As Integer = 0 To xs2.Length - 1
                bothX(xs1.Length + i) = xs2(xs2.Length - 1 - i)
                bothY(ys1.Length + i) = ys2(ys2.Length - 1 - i)
            Next

            Dim polygon As New Plottable.Polygon(bothX, bothY) With {
                .Fill = True,
                .FillColor = If(color, GetNextColor(0.5)),
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, System.Drawing.Color.Black)}
            Add(polygon)
            Return polygon
        End Function

        ''' <summary>
        ''' Create a polygon to fill the area above and below a Y curve.
        ''' </summary>
        Public Function AddFillError(xs As Double(), ys As Double(), yError As Double(),
                                     Optional color As Color? = Nothing,
                                     Optional lineWidth As Double = 0, Optional lineColor As Color? = Nothing) As Plottable.Polygon
            Dim polyXs As Double() = xs.Concat(xs.Reverse()).ToArray()

            Dim ysAbove As Double() = Enumerable.Range(0, ys.Length).Select(Function(i As Integer) ys(i) + yError(i)).ToArray()
            Dim ysBelow As Double() = Enumerable.Range(0, ys.Length).Select(Function(i As Integer) ys(i) - yError(i)).ToArray()

            Dim polys As Double() = ysBelow.Concat(ysAbove.Reverse()).ToArray()

            Dim polygon As New Plottable.Polygon(polyXs, ysBelow) With {
                .Fill = True,
                .FillColor = If(color, GetNextColor(0.5)),
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, System.Drawing.Color.Black)}
            Add(polygon)
            Return polygon
        End Function

        ''' <summary>
        ''' Create a polygon to fill the area between Y values and a baseline
        ''' that uses two different colors for area above and area below the baseline.
        ''' </summary>
        Public Function AddFillAboveAndBelow(xs As Double(), ys As Double(),
                                             Optional baseline As Double = 0,
                                             Optional colorAbove As Color? = Nothing,
                                             Optional colorBelow As Color? = Nothing,
                                             Optional lineWidth As Double = 0,
                                             Optional lineColor As Color? = Nothing) As Tuple(Of Plottable.Polygon, Plottable.Polygon)
            Dim t As Tuple(Of Double(), Double(), Double()) = ScottPlot.Drawing.Tools.PolyAboveAndBelow(xs, ys, baseline)
            Dim xs2 As Double() = t.Item1
            Dim ysAbove As Double() = t.Item2
            Dim ysBelow As Double() = t.Item3

            Dim polyAbove As New Plottable.Polygon(xs2, ysAbove) With {
                .FillColor = If(colorAbove, Color.Green),
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, System.Drawing.Color.Black)
            }
            Dim polyBelow As New Plottable.Polygon(xs2, ysBelow) With {
                .FillColor = If(colorBelow, Color.Red),
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, System.Drawing.Color.Black)
            }
            Add(polyAbove)
            Add(polyBelow)
            Return New Tuple(Of Plottable.Polygon, Plottable.Polygon)(polyAbove, polyBelow)
        End Function

        ''' <summary>
        ''' Add a line plot that uses a function (rather than X/Y points) to place the curve.
        ''' </summary>
        Public Function AddFunction(f As Func(Of Double, Double?),
                                    Optional color As Color? = Nothing,
                                    Optional lineWidth As Double = 1,
                                    Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.FunctionPlot
            Dim color2 As Color = If(color, Settings.GetNextColor())
            Dim functionPlot As New Plottable.FunctionPlot(f) With {
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .LineStyle = lineStyle,
                .FillColor = System.Drawing.Color.FromArgb(50, color2)}
            Add(functionPlot)
            Return functionPlot
        End Function

        ''' <summary>
        ''' Add a heatmap to the plot automatically-sized so each cell is 1x1.
        ''' </summary>
        ''' <param name="intensities">2D array of intensities. 
        ''' WARNING: Rendering artifacts may appear for arrays larger than Bitmap can support (~10M total values).</param>
        ''' <param name="lockScales">If true, <see cref="AxisScaleLock"/> will be called to ensure heatmap cells will be square.</param>
        ''' <returns>
        ''' Returns the heatmap that was added to the plot.
        ''' Act on its public fields and methods to customize it or update its data.
        ''' </returns>
        Public Function AddHeatmap(intensities As Double?(,), Optional colormap As Drawing.Colormap = Nothing, Optional lockScales As Boolean? = True) As Plottable.Heatmap
            Dim heatmap As New Plottable.Heatmap()
            heatmap.Update(intensities, colormap, Nothing, Nothing)
            Add(heatmap)

            If (lockScales IsNot Nothing) AndAlso lockScales.Value Then
                AxisScaleLock(True)
            End If
            If (lockScales is Nothing) AndAlso heatmap.IsDefaultSizeAndLocation Then
                AxisScaleLock(True)
            End If
            Return heatmap
        End Function

        ''' <summary>
        ''' Add a heatmap to the plot automatically-sized so each cell is 1x1.
        ''' </summary>
        ''' <param name="intensities">2D array of intensities. 
        ''' WARNING: Rendering artifacts may appear For arrays larger than Bitmap can support (~10M total values).</param>
        ''' <param name="colormap"></param>
        ''' <param name="lockScales">If true, <see cref="AxisScaleLock"/> will be called to ensure heatmap cells will be square.</param>
        ''' <returns>
        ''' Returns the heatmap that was added to the plot.
        ''' Act on its public fields and methods to customize it or update its data.
        ''' </returns>
        Public Function AddHeatmap(intensities As Double(,),
                                   Optional colormap As Drawing.Colormap = Nothing,
                                   Optional lockScales As Boolean? = Nothing) As Plottable.Heatmap
            Dim heatmap As New Plottable.Heatmap()
            heatmap.Update(intensities, colormap, Nothing, Nothing)
            Add(heatmap)

            If (lockScales IsNot Nothing) AndAlso lockScales.Value Then
                AxisScaleLock(True)
            End If
            If (lockScales is Nothing) AndAlso heatmap.IsDefaultSizeAndLocation Then
                AxisScaleLock(True)
            End If
            Return heatmap
        End Function

        ''' <summary>
        ''' Add a single-color heatmap where opacity is defined by a 2D array.
        ''' </summary>
        ''' <param name="color">Single color used for all cells.</param>
        ''' <param name="opacity">Opacities (ranging 0-1) for all cells.</param>
        ''' <param name="lockScales">If true, <see cref="AxisScaleLock"/> will be called to ensure heatmap cells will be square.</param>
        Public Function AddHeatmap(color As Color, opacity As Double?(,),
                                   Optional lockScales As Boolean? = True) As Plottable.Heatmap
            Dim heatmap As New Plottable.Heatmap()
            heatmap.Update(color, opacity)
            Add(heatmap)

            If (lockScales.HasValue AndAlso lockScales.Value) Then
                AxisScaleLock(True)
            End If
            If (lockScales is Nothing) AndAlso heatmap.IsDefaultSizeAndLocation Then
                AxisScaleLock(True)
            End If

            Return heatmap
        End Function

        ''' <summary>
        ''' Add heatmap to the plot stretched to fit the given dimensions.
        ''' Unlike the regular heatmap which gives each cell a size of 1x1 and starts at the axis origin, 
        ''' this heatmap stretches the array so that it covers the defined X and Y spans.
        ''' </summary>
        ''' <param name="intensities">2D array of intensities. 
        ''' WARNING: Rendering artifacts may appear for arrays larger than Bitmap can support (~10M total values).</param>
        ''' <param name="xMin">Position of the left edge of the far left column.</param>
        ''' <param name="xMax">Position of the left edge of the far right column.</param>
        ''' <param name="yMin">Position of the upper edge of the bottom row.</param>
        ''' <param name="yMax">Position of the upper edge of the top row.</param>
        ''' <returns>
        ''' Returns the heatmap that was added to the plot.
        ''' Act on its public fields and methods to customize it or update its data.
        ''' </returns>
        <Obsolete("This plot type has been deprecated. (min/max functionality now exists in Plottable.  Heatmap)")>
        Public Function AddHeatmapCoordinated(intensities As Double?(,), Optional xMin As Double? = Nothing, Optional xMax As Double? = Nothing, Optional yMin As Double? = Nothing, Optional yMax As Double? = Nothing, Optional colormap As Drawing.Colormap = Nothing) As Plottable.CoordinatedHeatmap
            Dim plottable As New Plottable.CoordinatedHeatmap()
            If (xMin is Nothing) AndAlso (xMax is Nothing) Then
                plottable.XMin = 0
                plottable.XMax = intensities.GetLength(0)
            ElseIf (xMin is Nothing) Then
                plottable.XMax = xMax.Value
                plottable.XMin = xMax.Value - intensities.GetLength(0)
            ElseIf (xMax is Nothing) Then
                plottable.XMin = xMin.Value
                plottable.XMax = xMin.Value + intensities.GetLength(0)
            Else
                plottable.XMin = xMin.Value
                plottable.XMax = xMax.Value
            End If

            If (yMin is Nothing) AndAlso (yMax is Nothing) Then
                plottable.YMin = 0
                plottable.YMax = intensities.GetLength(1)
            ElseIf (yMin is Nothing) Then
                plottable.YMax = yMax.Value
                plottable.YMin = yMax.Value - intensities.GetLength(1)
            ElseIf (yMax is Nothing) Then
                plottable.YMin = yMin.Value
                plottable.YMax = yMin.Value + intensities.GetLength(1)
            Else
                plottable.YMin = yMin.Value
                plottable.YMax = yMax.Value
            End If

            plottable.Update(intensities, colormap)
            Add(plottable)

            Return plottable
        End Function

        ''' <summary>
        ''' Add heatmap to the plot stretched to fit the given dimensions.
        ''' Unlike the regular heatmap which gives each cell a size of 1x1 and starts at the axis origin, 
        ''' this heatmap stretches the array so that it covers the defined X and Y spans.
        ''' </summary>
        ''' <param name="intensities">2D array of intensities. 
        ''' WARNING: Rendering artifacts may appear for arrays larger than Bitmap can support (~10M total values).</param>
        ''' <param name="xMin">Position of the left edge of the far left column.</param>
        ''' <param name="xMax">Position of the left edge of the far right column.</param>
        ''' <param name="yMin">Position of the upper edge of the bottom row.</param>
        ''' <param name="yMax">Position of the upper edge of the top row.</param>
        ''' <returns>
        ''' Returns the heatmap that was added to the plot.
        ''' Act on its public fields and methods to customize it or update its data.
        ''' </returns>
        <Obsolete("This plot type has been deprecated. Use a regular heatmap and modify its Offset and CellSize fields.")>
        Public Function AddHeatmapCoordinated(intensities As Double(,), Optional xMin As Double? = Nothing, Optional xMax As Double? = Nothing, Optional yMin As Double? = Nothing, Optional yMax As Double? = Nothing, Optional colormap As Drawing.Colormap = Nothing) As Plottable.CoordinatedHeatmap
            Dim plottable As New Plottable.CoordinatedHeatmap()
            'Solve all possible null combinations, if the boundaries are only partially provided use Step = 1;
            If (xMin is Nothing) AndAlso (xMax is Nothing) Then
                plottable.XMin = 0
                plottable.XMax = intensities.GetLength(0)
            ElseIf (xMin is Nothing) Then
                plottable.XMax = xMax.Value
                plottable.XMin = xMax.Value - intensities.GetLength(0)
            ElseIf (xMax is Nothing) Then
                plottable.XMin = xMin.Value
                plottable.XMax = xMin.Value + intensities.GetLength(0)
            Else
                plottable.XMin = xMin.Value
                plottable.XMax = xMax.Value
            End If

            If (yMin is Nothing) AndAlso (yMax is Nothing) Then
                plottable.YMin = 0
                plottable.YMax = intensities.GetLength(1)
            ElseIf (yMin is Nothing) Then
                plottable.YMax = yMax.Value
                plottable.YMin = yMax.Value - intensities.GetLength(1)
            ElseIf (yMax is Nothing) Then
                plottable.YMin = yMin.Value
                plottable.YMax = yMin.Value + intensities.GetLength(1)
            Else
                plottable.YMin = yMin.Value
                plottable.YMax = yMax.Value
            End If

            plottable.Update(intensities, colormap)
            Add(plottable)

            Return plottable
        End Function

        ''' <summary>
        ''' Add a horizontal axis line at a specific Y position.
        ''' </summary>
        Public Function AddHorizontalLine(y As Double,
                                          Optional color As Color? = Nothing,
                                          Optional width As Single = 1,
                                          Optional style As LineStyle = LineStyle.Solid,
                                          Optional label As String = Nothing) As Plottable.HLine
            Dim hline As New Plottable.HLine() With {
                .Y = y,
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = width,
                .LineStyle = style,
                .Label = label}
            Add(hline)
            Return hline
        End Function

        ''' <summary>
        ''' Add a horizontal span (shades the region between two X positions).
        ''' </summary>
        Public Function AddHorizontalSpan(xMin As Double, xMax As Double,
                                          Optional color As Color? = Nothing,
                                          Optional label As String = Nothing) As Plottable.HSpan
            Dim hspan As New Plottable.HSpan() With {
                .X1 = xMin,
                .X2 = xMax,
                .Color = If(color, GetNextColor(0.5)),
                .Label = label}
            Add(hspan)
            Return hspan
        End Function

        ''' <summary>
        ''' Display an image at a specific coordinate.
        ''' The <paramref name="anchor"/> defines which part of the image is placed at that coordinate.
        ''' By default the image is shown at its original size (in pixel units), but this can be modified with <paramref name="scale"/>.
        ''' </summary>
        ''' <param name="bitmap">Image to display.</param>
        ''' <param name="x">Horizontal position of the image anchor (axis units).</param>
        ''' <param name="y">Vertical position of the image anchor (axis units).</param>
        ''' <param name="rotation">Rotation in degrees.</param>
        ''' <param name="scale">Scale (1.0 = original scale, 2.0 = double size).</param>
        ''' <param name="anchor">Definces which part of the image is placed at the given X and Y coordinates.</param>
        Public Function AddImage(bitmap As Bitmap,
                                 x As Double, y As Double,
                                 Optional rotation As Double = 0,
                                 Optional scale As Double = 1,
                                 Optional anchor As Alignment = Alignment.UpperLeft) As ScottPlot.Plottable.Image
            Dim image As New ScottPlot.Plottable.Image() With {
                .Bitmap = bitmap,
                .X = x,
                .Y = y,
                .Rotation = rotation,
                .Scale = scale,
                .Alignment = anchor
            }
            Settings.Plottables.Add(image)
            Return image
        End Function

        ''' <summary>
        ''' Add a line (a scatter plot with two points) to the plot.
        ''' </summary>
        Public Function AddLine(x1 As Double, y1 As Double,
                                x2 As Double, y2 As Double,
                                Optional color As Color? = Nothing,
                                Optional lineWidth As Single = 1) As Plottable.ScatterPlot
            Return AddScatter(New Double() {x1, x2},
                              New Double() {y1, y2},
                              color,
                              lineWidth,
                              0)
        End Function

        ''' <summary>
        ''' Add a marker at a specific X/Y position.
        ''' </summary>
        Public Function AddLine(slope As Double, offset As Double, xLimits As Tuple(Of Double, Double), Optional color As Color? = Nothing, Optional lineWidth As Single = 1.0F) As Plottable.ScatterPlot
            Dim y1 As Double = xLimits.Item1 * slope + offset
            Dim y2 As Double = xLimits.Item2 * slope + offset
            Return AddScatter(New Double() {xLimits.Item1, xLimits.Item2},
                              New Double() {y1, y2},
                              color,
                              lineWidth,
                              0)
        End Function

        ''' <summary>
        ''' Add a marker at a specific X/Y position.
        ''' </summary>
        Public Function AddMarker(x As Double, y As Double,
                                  Optional shape As MarkerShape = MarkerShape.FilledCircle,
                                  Optional size As Double = 10,
                                  Optional color As Color? = Nothing,
                                  Optional label As String = Nothing) As Plottable.MarkerPlot
            Dim markerPlot As New Plottable.MarkerPlot() With {
                .X = x,
                .Y = y,
                .MarkerShape = shape,
                .MarkerSize = CSng(size),
                .Color = If(color, GetNextColor()),
                .Label = label
            }
            Add(markerPlot)
            Return markerPlot
        End Function

        ''' <summary>
        ''' Add a draggable marker at a specific X/Y position.
        ''' </summary>
        Public Function AddMarkerDraggable(x As Double, y As Double,
                                           Optional shape As MarkerShape = MarkerShape.FilledCircle,
                                           Optional size As Double = 10,
                                           Optional color As Color? = Nothing,
                                           Optional label As String = Nothing) As Plottable.DraggableMarkerPlot
            Dim draggableMarkerPlot As New Plottable.DraggableMarkerPlot() With {
                .X = x,
                .Y = y,
                .MarkerShape = shape,
                .MarkerSize = CSng(size),
                .Color = If(color, GetNextColor()),
                .Label = label
            }
            Add(draggableMarkerPlot)
            Return draggableMarkerPlot
        End Function

        ''' <summary>
        ''' Add OHLC (open, high, low, close) data to the plot.
        ''' </summary>
        Public Function AddOHLCs(ohlcs As OHLC()) As Plottable.FinancePlot
            Dim financePlot As New Plottable.FinancePlot(ohlcs) With {
                .Candle = False,
                .ColorUp = ColorTranslator.FromHtml("#26a69a"),
                .ColorDown = ColorTranslator.FromHtml("#ef5350")
            }
            Add(financePlot)
            Return financePlot
        End Function

        ''' <summary>
        ''' Add a pie chart to the plot.
        ''' </summary>
        Public Function AddPie(values As Double(), Optional hideGridAndFrame As Boolean = True) As Plottable.PiePlot
            Dim colors As Color() = Enumerable.Range(0, values.Length).Select(Function(i As Integer)
                                                                                  Return Settings.PlottablePalette.GetColor(i)
                                                                              End Function).ToArray()
            Dim piePlot As New Plottable.PiePlot(values, Nothing, colors)
            Add(piePlot)

            If hideGridAndFrame Then
                Grid(False)
                Frameless()
            End If
            Return piePlot
        End Function

        ''' <summary>
        ''' Add a point (a scatter plot with a single marker).
        ''' </summary>
        ''' <param name="color">Color of the marker</param>
        ''' <param name="size">Size of the marker</param>
        ''' <param name="shape">Maker shape</param>
        ''' <param name="label">Text to appear in the legend</param>
        ''' <returns>
        ''' The scatter plot that was created and added to the plot. 
        ''' Interact with its public fields and methods to customize style and update data.
        ''' </returns>
        Public Function AddPoint(x As Double, y As Double,
                                 Optional color As Color? = Nothing,
                                 Optional size As Single = 5,
                                 Optional shape As MarkerShape = MarkerShape.FilledCircle,
                                 Optional label As String = Nothing) As Plottable.MarkerPlot
            Dim markerPlot As New Plottable.MarkerPlot() With {
                .X = x,
                .Y = y,
                .MarkerShape = shape,
                .MarkerSize = size,
                .Color = If(color, GetNextColor()),
                .Label = label
            }
            Add(markerPlot)
            Return markerPlot
        End Function

        ''' <summary>
        ''' Add a polygon to the plot.
        ''' </summary>
        Public Function AddPolygon(xs As Double(), ys As Double(),
                                   Optional fillColor As Color? = Nothing,
                                   Optional lineWidth As Double = 0,
                                   Optional lineColor As Color? = Nothing) As Plottable.Polygon
            Dim polygon As New Plottable.Polygon(xs, ys) With {
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, Color.Black),
                .FillColor = If(fillColor, Settings.GetNextColor())}
            Add(polygon)
            Return polygon
        End Function

        ''' <summary>
        ''' Add many polygons using an optimized rendering method.
        ''' </summary>
        Public Function AddPolygons(polys As List(Of List(Of Tuple(Of Double, Double))),
                                    Optional fillColor As Color? = Nothing,
                                    Optional lineWidth As Double = 0,
                                    Optional lineColor As Color? = Nothing) As Plottable.Polygons
            Dim polygons As New Plottable.Polygons(polys) With {
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, Color.Black),
                .FillColor = If(fillColor, Settings.GetNextColor())}
            Add(polygons)
            Return polygons
        End Function

        ''' <summary>
        ''' Add a population to the plot.
        ''' </summary>
        Public Function AddPopulation(population As Statistics.Population, Optional label As String = Nothing) As Plottable.PopulationPlot
            Dim populationPlot As New Plottable.PopulationPlot(population, label, Settings.GetNextColor())
            Add(populationPlot)
            Return populationPlot
        End Function

        ''' <summary>
        ''' Add multiple populations to the plot as a single series.
        ''' </summary>
        Public Function AddPopulations(populations As Statistics.Population(), Optional label As String = Nothing) As Plottable.PopulationPlot
            Dim populationPlot As New Plottable.PopulationPlot(populations, label, Settings.GetNextColor())
            Add(populationPlot)
            Return populationPlot
        End Function

        ''' <summary>
        ''' Add multiple populations to the plot as a single series.
        ''' </summary>
        Public Function AddPopulations(multiSeries As Statistics.PopulationMultiSeries) As Plottable.PopulationPlot
            For i As Integer = 0 To multiSeries.MultiSeries.Length - 1
                multiSeries.MultiSeries(i).Color = Settings.PlottablePalette.GetColor(i)
            Next
            Dim populationPlot As New Plottable.PopulationPlot(multiSeries)
            Add(populationPlot)
            Return populationPlot
        End Function

        ''' <summary>
        ''' Add a radar plot (a two-dimensional chart of three or more quantitative variables 
        ''' represented on axes starting from the same point).
        ''' </summary>
        ''' <param name="values">2D array containing categories (columns) and groups (rows).</param>
        ''' <param name="independentAxes">If true, axis (category) values are scaled independently.</param>
        ''' <param name="maxValues">If provided, each category (column) is normalized to these values.</param>
        ''' <param name="disableFrameAndGrid">Also make the plot frameless and disable its grid.</param>
        ''' <returns>The radar plot that was just created and added to the plot.</returns>
        Public Function AddRadar(values As Double(,), Optional independentAxes As Boolean = False, Optional maxValues As Double() = Nothing, Optional disableFrameAndGrid As Boolean = True) As Plottable.RadarPlot
            Dim colors As Color() = Enumerable.Range(0, values.Length) _
                .Select(Function(i) Settings.PlottablePalette.GetColor(i)).ToArray()

            Dim fills As Color() = colors.Select(Function(x) Color.FromArgb(50, x)).ToArray()
            Dim radarPlot As New Plottable.RadarPlot(values, colors, fills, independentAxes, maxValues)
            Add(radarPlot)

            If (disableFrameAndGrid) Then
                Frameless()
                Grid(False)
            End If

            Return radarPlot
        End Function

        ''' <summary>
        ''' Add a radial gauge plot (a chart where data is represented by concentric circular gauges).
        ''' </summary>
        ''' <param name="values">Array of gauge values.</param>
        ''' <param name="disableFrameAndGrid">Also make the plot frameless and disable its grid.</param>
        ''' <returns>The radial gaugle plot that was just created and added to the plot.</returns>
        Public Function AddRadialGauge(values As Double(), Optional disableFrameAndGrid As Boolean = True) As Plottable.RadialGaugePlot
            Dim colors As Color() = Palette.GetColors(values.Length)
            Dim radialGaugePlot As New Plottable.RadialGaugePlot(values, colors)
            Add(radialGaugePlot)

            If disableFrameAndGrid Then
                Frameless()
                Grid(False)
            End If
            Return radialGaugePlot
        End Function

        ''' <summary>
        ''' A Pie chart where the angle of slices is constant but the radii are not.
        ''' </summary>
        ''' <param name="values">The data to plot.</param>
        ''' <param name="hideGridAndFrame">Whether to make the plot frameless and disable the grid.</param>
        Public Function AddCoxcomb(values As Double(), Optional hideGridAndFrame As Boolean = True) As Plottable.CoxcombPlot
            Dim colors As Color() = Enumerable.Range(0, values.Length) _
                .Select(Function(i As Integer) Settings.PlottablePalette.GetColor(i)).ToArray()
            Dim coxcombPlot As New Plottable.CoxcombPlot(values, colors)
            Add(coxcombPlot)

            If hideGridAndFrame Then
                Grid(False)
                Frameless()
            End If
            Return coxcombPlot
        End Function

        ''' <summary>
        ''' Add error bars to the plot with custom dimensions in all 4 directions.
        ''' </summary>
        ''' <param name="xs">Horizontal center of the errorbar.</param>
        ''' <param name="ys">Vertical center of each errorbar.</param>
        ''' <param name="xErrorsPositive">Magnitude of positive vertical error.</param>
        ''' <param name="xErrorsNegative">Magnitude of positive horizontal error.</param>
        ''' <param name="yErrorsPositive">Magnitude of negative vertical error.</param>
        ''' <param name="yErrorsNegative">Magnitude of negative horizontal error.</param>
        ''' <param name="color">Color (null for next color in palette).</param>
        ''' <param name="markerSize">Size (in pixels) to draw a marker at the center of each errorbar</param>
        Public Function AddErrorBars(xs As Double(), ys As Double(),
                                     xErrorsPositive As Double(), xErrorsNegative As Double(),
                                     yErrorsPositive As Double(), yErrorsNegative As Double(),
                                     Optional color As Color? = Nothing, Optional markerSize As Single = 0) As Plottable.ErrorBar
            Dim errorBar As New Plottable.ErrorBar(xs, ys, xErrorsPositive, xErrorsNegative, yErrorsPositive, yErrorsNegative) With {
                .Color = If(color, GetNextColor()),
                .MarkerSize = markerSize
            }
            Add(errorBar)
            Return errorBar
        End Function

        ''' <summary>
        ''' Add error bars to the plot which have symmetrical positive/negative errors.
        ''' </summary>
        ''' <param name="xs">Horizontal center of the errorbar.</param>
        ''' <param name="ys">Vertical center of each errorbar.</param>
        ''' <param name="xErrors">Magnitude of vertical error.</param>
        ''' <param name="yErrors">Magnitude of horizontal error.</param>
        ''' <param name="color">Color (null for next color in palette).</param>
        ''' <param name="markerSize">Size (in pixels) to draw a marker at the center of each errorbar.</param>
        Public Function AddErrorBars(xs As Double(), ys As Double(), xErrors As Double(), yErrors As Double(),
                                     Optional color As Color? = Nothing, Optional markerSize As Single = 0) As Plottable.ErrorBar
            Return AddErrorBars(xs, ys, xErrors, xErrors, yErrors, yErrors, color, markerSize)
        End Function

        ''' <summary>
        ''' Add an L-shaped scalebar to the corner of the plot.
        ''' </summary>
        Public Function AddScaleBar(width As Double, height As Double,
                                    Optional xLabel As String = Nothing, Optional yLabel As String = Nothing) As Plottable.ScaleBar
            Dim scaleBar As New Plottable.ScaleBar() With {
                .Width = width,
                .Height = height,
                .HorizontalLabel = xLabel,
                .VerticalLabel = yLabel}
            Add(scaleBar)
            Return scaleBar
        End Function

        ''' <summary>
        ''' Add a scatter plot from X/Y pairs. 
        ''' Lines and markers are shown by default. Scatter plots are slower than Signal plots.
        ''' </summary>
        Public Function AddScatter(xs As Double(), ys As Double(),
                                   Optional color As Color? = Nothing,
                                   Optional lineWidth As Single = 1,
                                   Optional markerSize As Single = 5,
                                   Optional markerShape As MarkerShape = MarkerShape.FilledCircle,
                                   Optional lineStyle As LineStyle = LineStyle.Solid,
                                   Optional label As String = Nothing) As Plottable.ScatterPlot
            Dim scatterPlot As New Plottable.ScatterPlot(xs, ys, Nothing, Nothing) With {
                .Color = If(color, GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = markerSize,
                .Label = label,
                .MarkerShape = markerShape,
                .LineStyle = lineStyle}
            Add(scatterPlot)
            Return scatterPlot
        End Function

        ''' <summary>
        ''' Add a scatter plot from X/Y pairs connected by lines (no markers).
        ''' Scatter plots are slower than Signal plots.
        ''' </summary>
        Public Function AddScatterLines(xs As Double(), ys As Double(),
                                        Optional color As Color? = Nothing,
                                        Optional lineWidth As Single = 1,
                                        Optional lineStyle As LineStyle = LineStyle.Solid,
                                        Optional label As String = Nothing) As Plottable.ScatterPlot
            Dim scatterPlot As New Plottable.ScatterPlot(xs, ys, Nothing, Nothing) With {
                .Color = If(color, GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = 0,
                .Label = label,
                .LineStyle = lineStyle}
            Add(scatterPlot)
            Return scatterPlot
        End Function

        ''' <summary>
        ''' Add a scatter plot from X/Y pairs using markers at points (no lines).
        ''' Scatter plots are slower than Signal plots.
        ''' </summary>
        Public Function AddScatterPoints(xs As Double(), ys As Double(),
                                         Optional color As Color? = Nothing,
                                         Optional markerSize As Single = 5,
                                         Optional markerShape As MarkerShape = MarkerShape.FilledCircle,
                                         Optional label As String = Nothing) As Plottable.ScatterPlot
            Dim scatterPlot As New Plottable.ScatterPlot(xs, ys, Nothing, Nothing) With {
                .Color = If(color, GetNextColor()),
                .LineWidth = 0,
                .MarkerSize = markerSize,
                .Label = label,
                .MarkerShape = markerShape
            }
            Add(scatterPlot)
            Return scatterPlot
        End Function

        ''' <summary>
        ''' Add a step plot is a type of line plot where points are connected with right angles instead of straight lines.
        ''' </summary>
        Public Function AddScatterStep(xs As Double(), ys As Double(),
                                       Optional color As Color? = Nothing,
                                       Optional lineWidth As Single = 1,
                                       Optional label As String = Nothing) As Plottable.ScatterPlot
            Dim scatterPlot As New Plottable.ScatterPlot(xs, ys, Nothing, Nothing) With {
                .Color = If(color, GetNextColor()),
                .LineWidth = lineWidth,
                .Label = label,
                .MarkerSize = 0,
                .StepDisplay = True
            }
            Add(scatterPlot)
            Return scatterPlot
        End Function

        ''' <summary>
        ''' Scatter plot with Add() and Clear() methods for updating data.
        ''' </summary>
        Public Function AddScatterList(Optional color As Color? = Nothing,
                                       Optional lineWidth As Single = 1,
                                       Optional markerSize As Single = 5,
                                       Optional label As String = Nothing,
                                       Optional markerShape As MarkerShape = MarkerShape.FilledCircle,
                                       Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.ScatterPlotList(Of Double)
            Dim spl As New Plottable.ScatterPlotList(Of Double)() With {
                .Color = If(color, GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = markerSize,
                .Label = label,
                .MarkerShape = markerShape,
                .LineStyle = lineStyle
            }
            Add(spl)
            Return spl
        End Function

        ''' <summary>
        ''' Generic Plottable.  ScatterPlotList using generic types (as long as they can be converted to double).
        ''' </summary>
        Public Function AddScatterList(Of T)(Optional color As Color? = Nothing,
                                             Optional lineWidth As Single = 1,
                                             Optional markerSize As Single = 5,
                                             Optional label As String = Nothing,
                                             Optional markerShape As MarkerShape = MarkerShape.FilledCircle,
                                             Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.ScatterPlotList(Of T)
            Dim spl As New Plottable.ScatterPlotList(Of T)() With {
                .Color = If(color, GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = markerSize,
                .Label = label,
                .MarkerShape = markerShape,
                .LineStyle = lineStyle
            }
            Add(spl)
            Return spl
        End Function

        ''' <summary>
        ''' Signal plots have evenly-spaced X points and render very fast.
        ''' </summary>
        ''' <param name="sampleRate">Measurements per second, Hz.</param>
        Public Function AddSignal(ys As Double(),
                                  Optional sampleRate As Double = 1,
                                  Optional color As Color? = Nothing,
                                  Optional label As String = Nothing) As Plottable.SignalPlot
            Dim signalPlot As New Plottable.SignalPlot() With {
                .Ys = ys,
                .SampleRate = sampleRate,
                .Color = If(color, Settings.GetNextColor()),
                .Label = label,
                .MinRenderIndex = 0,
                .MaxRenderIndex = ys.Length - 1
            }
            Add(signalPlot)
            Return signalPlot
        End Function

        ''' <summary>
        ''' Signal plots have evenly-spaced X points and render very fast.
        ''' </summary>
        ''' <param name="sampleRate">Measurements per second, Hz.</param>
        Public Function AddSignal(Of T As {Structure, IComparable})(ys As T(),
                                                                    Optional sampleRate As Double = 1,
                                                                    Optional color As Color? = Nothing,
                                                                    Optional label As String = Nothing) As Plottable.SignalPlotGeneric(Of T)
            Dim sp As New Plottable.SignalPlotGeneric(Of T)() With {
                .Ys = ys,
                .SampleRate = sampleRate,
                .Color = If(color, Settings.GetNextColor()),
                .Label = label,
                .MinRenderIndex = 0,
                .MaxRenderIndex = ys.Length - 1
            }
            Add(sp)
            Return sp
        End Function

        ''' <summary>
        ''' SignalConts plots have evenly-spaced X points and render faster than Signal plots
        ''' but data in source arrays cannot be changed after it is loaded.
        ''' Methods can be used to update all or portions of the data.
        ''' </summary>
        ''' <param name="sampleRate">Measurements per second, Hz.</param>
        Public Function AddSignalConst(Of T As {Structure, IComparable})(ys As T(),
                                                                         Optional sampleRate As Double = 1,
                                                                         Optional color As Color? = Nothing,
                                                                         Optional label As String = Nothing) As Plottable.SignalPlotConst(Of T)
            Dim sp As New Plottable.SignalPlotConst(Of T)() With {
                .Ys = ys,
                .SampleRate = sampleRate,
                .Color = (If(color, Settings.GetNextColor())),
                .Label = label,
                .MinRenderIndex = 0,
                .MaxRenderIndex = ys.Length - 1
            }
            Add(sp)
            Return sp
        End Function

        ''' <summary>
        ''' Speed-optimized plot for Ys with unevenly-spaced ascending Xs.
        ''' </summary>
        Public Function AddSignalXY(xs As Double(), ys As Double(),
                                    Optional color As Color? = Nothing, Optional label As String = Nothing) As Plottable.SignalPlotXY
            Dim sp As New Plottable.SignalPlotXY() With {
                .Xs = xs,
                .Ys = ys,
                .Color = If(color, Settings.GetNextColor()),
                .Label = label,
                .MinRenderIndex = 0,
                .MaxRenderIndex = ys.Length - 1}
            Add(sp)
            Return sp
        End Function

        ''' <summary>
        ''' Speed-optimized plot for Ys with unevenly-spaced ascending Xs.
        ''' </summary>
        ''' <remarks>
        ''' Faster than SignalXY but values cannot be modified after loading.
        ''' </remarks>
        Public Function AddSignalXYConst(Of TX As {Structure, IComparable},
                                             TY As {Structure, IComparable})(xs As TX(),
                                                                             ys As TY(),
                                                                             Optional color As Color? = Nothing,
                                                                             Optional label As String = Nothing) As Plottable.SignalPlotXYConst(Of TX, TY)
            Dim sp As New Plottable.SignalPlotXYConst(Of TX, TY)() With {
                .Xs = xs,
                .Ys = ys,
                .Color = If(color, Settings.GetNextColor()),
                .Label = label,
                .MinRenderIndex = 0,
                .MaxRenderIndex = ys.Length - 1
            }
            Add(sp)
            Return sp
        End Function

        ''' <summary>
        ''' Display text at specific X/Y coordinates.
        ''' </summary>
        Public Function AddText(label As String, x As Double, y As Double, Optional size As Single = 12.0F, Optional color As Color? = Nothing) As Plottable.Text
            Return AddText(label, x, y, New ScottPlot.Drawing.Font() With {.Size = size, .Color = If(color, GetNextColor())})
        End Function

        ''' <summary>
        ''' Display text at specific X/Y coordinates.
        ''' </summary>
        Public Function AddText(label As String, x As Double, y As Double, font As ScottPlot.Drawing.Font) As Plottable.Text
            Dim text As New Plottable.Text() With {.Label = label, .X = x, .Y = y, .Font = font}
            Add(text)
            Return text
        End Function

        ''' <summary>
        ''' Display a text bubble that points to an X/Y location on the plot.
        ''' </summary>
        Public Function AddTooltip(label As String, x As Double, y As Double) As Plottable.Tooltip
            Dim tt As New Plottable.Tooltip() With {.Label = label, .X = x, .Y = y}
            Add(tt)
            Return tt
        End Function

        ''' <summary>
        ''' Add a 2D vector field to the plot.
        ''' </summary>
        Public Function AddVectorField(vectors As Statistics.Vector2(,), xs As Double(), ys As Double(),
                                       Optional label As String = Nothing,
                                       Optional color As Color? = Nothing,
                                       Optional colormap As Drawing.Colormap = Nothing,
                                       Optional scaleFactor As Double = 1) As Plottable.VectorField
            Dim vf As New Plottable.VectorField(vectors, xs, ys, colormap, scaleFactor,
                                      If(color, Settings.GetNextColor())) With {.Label = label}
            Add(vf)
            Return vf
        End Function

        ''' <summary>
        ''' Add a 2D vector field to the plot.
        ''' </summary>
        Public Function AddVectorFieldList() As Plottable.VectorFieldList
            Dim vfl As New Plottable.VectorFieldList()
            Add(vfl)
            Return vfl
        End Function

        ''' <summary>
        ''' Add a vertical axis line at a specific Y position.
        ''' </summary>
        Public Function AddVerticalLine(x As Double,
                                        Optional color As Color? = Nothing,
                                        Optional width As Single = 1,
                                        Optional style As LineStyle = LineStyle.Solid,
                                        Optional label As String = Nothing) As Plottable.VLine
            Dim vline As New Plottable.VLine() With {
                .X = x,
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = width,
                .LineStyle = style,
                .Label = label
            }
            Add(vline)
            Return vline
        End Function

        ''' <summary>
        ''' Add a horizontal span (shades the region between two X positions).
        ''' </summary>
        Public Function AddVerticalSpan(yMin As Double, yMax As Double,
                                        Optional color As Color? = Nothing,
                                        Optional label As String = Nothing) As Plottable.VSpan
            Dim vspan As New Plottable.VSpan() With {
                .Y1 = yMin,
                .Y2 = yMax,
                .Color = If(color, GetNextColor(0.5)),
                .Label = label
            }
            Add(vspan)
            Return vspan
        End Function

    End Class

End Namespace