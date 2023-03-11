Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Linq

' Code here extends Plot module with methods to construct plottables.
'  - Plottables created here are added to the plottables list and returned.
'  - Long lists of optional arguments (matplotlib style) are permitted.
'  - Use one line per argument to simplify the tracking of changes.

Namespace ScottPlot

    Partial Class Plot

        <Obsolete("Use AddHeatmapCoordinated (note capitalization)")>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Function AddHeatMapCoordinated(intensities As Double?(,),
                                              Optional xMin As Double? = Nothing,
                                              Optional xMax As Double? = Nothing,
                                              Optional yMin As Double? = Nothing,
                                              Optional yMax As Double? = Nothing,
                                              Optional colormap As ColorMap = Nothing) As Plottable.CoordinatedHeatmap
            Return AddHeatMapCoordinated(intensities, xMin, xMax, yMin, yMax, colormap)
        End Function

        <Obsolete("Use AddHeatmapCoordinated (note capitalization)")>
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Function AddHeatMapCoordinated(intensities As Double(,),
                                              Optional xMin As Double? = Nothing,
                                              Optional xMax As Double? = Nothing,
                                              Optional yMin As Double? = Nothing,
                                              Optional yMax As Double? = Nothing,
                                              Optional colormap As ColorMap = Nothing) As Plottable.CoordinatedHeatmap
            Return AddHeatMapCoordinated(intensities, xMin, xMax, yMin, yMax, colormap)
        End Function

        <Obsolete("Use AddAnnotation() and customize the object it returns")>
        Public Function PlotAnnotation(label As String,
                                       Optional xPixel As Double = 10,
                                       Optional yPixel As Double = 10,
                                       Optional fontSize As Double = 12,
                                       Optional fontName As String = "Segoe UI",
                                       Optional fontColor As Color? = Nothing,
                                       Optional fontAlpha As Double = 1,
                                       Optional fill As Boolean = True,
                                       Optional fillColor As Color? = Nothing,
                                       Optional fillAlpha As Double = 0.2,
                                       Optional lineWidth As Double = 1,
                                       Optional lineColor As Color? = Nothing,
                                       Optional lineAlpha As Double = 1,
                                       Optional shadow As Boolean = False) As Plottable.Annotation

            fontColor = If(fontColor, Color.Black)
            lineColor = If(lineColor, Color.Black)
            fillColor = If(fillColor, Color.Yellow)

            fontColor = Color.FromArgb(CInt(255 * fontAlpha), fontColor.Value.R, fontColor.Value.G, fontColor.Value.B)
            lineColor = Color.FromArgb(CInt(255 * lineAlpha), lineColor.Value.R, lineColor.Value.G, lineColor.Value.B)
            fillColor = Color.FromArgb(CInt(255 * fillAlpha), fillColor.Value.R, fillColor.Value.G, fillColor.Value.B)

            Dim annotation As New Plottable.Annotation() With {
                .X = xPixel,
                .Y = yPixel,
                .Label = label,
                .Background = fill,
                .BackgroundColor = fillColor.Value,
                .BorderWidth = CSng(lineWidth),
                .BorderColor = lineColor.Value,
                .Shadow = shadow
            }
            annotation.Font.Size = CSng(fontSize)
            annotation.Font.Name = fontName
            annotation.Font.Color = fontColor.Value
            Add(annotation)
            Return annotation
        End Function

        <Obsolete("Use AddArrow() and customize the object it returns")>
        Public Function PlotArrow(tipX As Double, tipY As Double, baseX As Double, baseY As Double,
                                  Optional lineWidth As Double = 5,
                                  Optional arrowheadWidth As Single = 3,
                                  Optional arrowheadLength As Single = 3,
                                  Optional color As Color? = Nothing,
                                  Optional label As String = Nothing) As Plottable.ScatterPlot
            Dim scatterPlot As Plottable.ScatterPlot = PlotScatter(New Double() {baseX, tipX},
                                                         New Double() {baseY, tipY},
                                                         color, lineWidth, 0, label)
            scatterPlot.ArrowheadLength = arrowheadLength
            scatterPlot.ArrowheadWidth = arrowheadWidth
            Return scatterPlot
        End Function

        <Obsolete("Use AddBar() and customize the object it returns")>
        Public Function PlotBar(xs As Double(), ys As Double(),
                                Optional errorY As Double() = Nothing,
                                Optional label As String = Nothing,
                                Optional barWidth As Double = 0.8,
                                Optional xOffset As Double = 0,
                                Optional fill As Boolean = True,
                                Optional fillColor As Color? = Nothing,
                                Optional outlineWidth As Double = 1,
                                Optional outlineColor As Color? = Nothing,
                                Optional errorLineWidth As Double = 1,
                                Optional errorCapSize As Double = 0.38,
                                Optional errorColor As Color? = Nothing,
                                Optional horizontal As Boolean = False,
                                Optional showValues As Boolean = False,
                                Optional valueColor As Color? = Nothing,
                                Optional autoAxis As Boolean = True,
                                Optional yOffsets As Double() = Nothing,
                                Optional negativeColor As Color? = Nothing) As Plottable.BarPlot
            Dim nextColor As Color = Settings.GetNextColor()
            Dim barPlot As New Plottable.BarPlot(xs, ys, errorY, yOffsets) With {
                .BarWidth = barWidth,
                .XOffset = xOffset,
                .FillColor = If(fillColor, nextColor),
                .Label = label,
                .ErrorLineWidth = CSng(errorLineWidth),
                .ErrorCapSize = errorCapSize,
                .ErrorColor = If(errorColor, Color.Black),
                .BorderLineWidth = CSng(outlineWidth),
                .BorderColor = If(outlineColor, Color.Black),
                .VerticalOrientation = Not horizontal,
                .ShowValuesAboveBars = showValues,
                .FillColorNegative = If(negativeColor, nextColor)
            }
            barPlot.Font.Color = (If(valueColor, Color.Black))
            Add(barPlot)

            If autoAxis Then
                'perform a tight axis adjustment
                AxisAuto(0, 0)
                Dim tightAxisLimits As AxisLimits = GetAxisLimits(0, 0)

                'now loosen it up a bit
                AxisAuto()

                'now set one of the axis edges to zero
                If horizontal Then
                    If (tightAxisLimits.XMin = 0) Then
                        SetAxisLimits(xMin:=0)
                    ElseIf (tightAxisLimits.XMax = 0) Then
                        SetAxisLimits(xMax:=0)
                    End If

                ElseIf (tightAxisLimits.YMin = 0) Then
                    SetAxisLimits(yMin:=0)
                ElseIf (tightAxisLimits.YMax = 0) Then
                    SetAxisLimits(yMax:=0)
                End If
            End If
            Return barPlot
        End Function

        ''' <summary>
        ''' Create a series of bar plots given a 2D dataset.
        ''' </summary>
        <Obsolete("Use AddBarGroups() and customize the object it returns")>
        Public Function PlotBarGroups(groupLabels As String(), seriesLabels As String(), ys As Double()(),
                                      Optional yErr As Double()() = Nothing,
                                      Optional groupWidthFraction As Double = 0.8,
                                      Optional barWidthFraction As Double = 0.8,
                                      Optional errorCapSize As Double = 0.38,
                                      Optional showValues As Boolean = False) As Plottable.BarPlot()
            If (groupLabels is Nothing) OrElse (seriesLabels is Nothing) OrElse (ys is Nothing) Then
                Throw New ArgumentException("Labels and Ys cannot be null.")
            End If
            If (seriesLabels.Length <> ys.Length) Then
                Throw New ArgumentException("GroupLabels and ys must be the same length.")
            End If
            For Each subArray In ys
                If (subArray.Length <> groupLabels.Length) Then
                    Throw New ArgumentException("All arrays inside ys must be the same length as groupLabels.")
                End If
            Next

            Dim seriesCount As Integer = ys.Length
            Dim barWidth As Double = groupWidthFraction / seriesCount
            Dim bars As Plottable.BarPlot() = New Plottable.BarPlot(seriesCount - 1) {}
            Dim containsNegativeY As Boolean = False
            For i As Integer = 0 To seriesCount - 1
                Dim offset As Double = i * barWidth
                Dim barYs As Double() = ys(i)
                Dim barYerr As Double() = yErr?(i)
                Dim barXs As Double() = DataGen.Consecutive(barYs.Length)
                containsNegativeY = containsNegativeY OrElse barYs.Where(Function(y) y < 0).Any()
                bars(i) = PlotBar(barXs,
                                  barYs,
                                  barYerr,
                                  seriesLabels(i),
                                  barWidth * barWidthFraction,
                                  offset,
                                  errorCapSize:=errorCapSize,
                                  showValues:=showValues)
            Next

            If containsNegativeY Then
                AxisAuto()
            End If

            Dim groupPositions As Double() = DataGen.Consecutive(groupLabels.Length, offset:=(groupWidthFraction - barWidth) / 2)
            XTicks(groupPositions, groupLabels)
            Return bars
        End Function

        <Obsolete("Use AddImage() and customize the object it returns.")>
        Public Function PlotBitmap(bitmap As Bitmap, x As Double, y As Double,
                                   Optional label As String = Nothing,
                                   Optional alignment As Alignment = Alignment.MiddleLeft,
                                   Optional rotation As Double = 0,
                                   Optional frameColor As Color? = Nothing,
                                   Optional frameSize As Integer = 0) As ScottPlot.Plottable.Image
            Dim bmp As New ScottPlot.Plottable.Image() With {
                .Bitmap = bitmap,
                .X = x,
                .Y = y,
                .Label = label,
                .Alignment = alignment,
                .Rotation = rotation,
                .BorderColor = If(frameColor, Color.White),
                .BorderSize = CSng(frameSize)
            }
            Settings.Plottables.Add(bmp)
            Return bmp
        End Function

        <Obsolete("Use AddCandlesticks() and customize the object it returns")>
        Public Function PlotCandlestick(ohlcs As OHLC(),
                                        Optional colorUp As Color? = Nothing,
                                        Optional colorDown As Color? = Nothing,
                                        Optional autoWidth As Boolean = True,
                                        Optional sequential As Boolean = False) As Plottable.FinancePlot
            Dim financePlot As New Plottable.FinancePlot(ohlcs) With {
                .Candle = True,
                .Sequential = sequential,
                .ColorUp = If(colorUp, ColorTranslator.FromHtml("#26a69a")),
                .ColorDown = If(colorDown, ColorTranslator.FromHtml("#ef5350"))}
            Add(financePlot)
            Return financePlot
        End Function

        <Obsolete("Use AddScatter() and customize it for no line, no marker, and errorbars as desired")>
        Public Sub PlotErrorBars(xs As Double(), ys As Double(),
                                 Optional xPositiveError As Double() = Nothing,
                                 Optional xNegativeError As Double() = Nothing,
                                 Optional yPositiveError As Double() = Nothing,
                                 Optional yNegativeError As Double() = Nothing,
                                 Optional color As Color? = Nothing,
                                 Optional lineWidth As Double = 1,
                                 Optional capWidth As Double = 3,
                                 Optional label As String = Nothing)
            Throw New NotImplementedException()
        End Sub

        <Obsolete("Use AddFill() and customize the object it returns")>
        Public Function PlotFill(xs As Double(), ys As Double(),
                                 Optional label As String = Nothing,
                                 Optional lineWidth As Double = 0,
                                 Optional lineColor As Color? = Nothing,
                                 Optional fill As Boolean = True,
                                 Optional fillColor As Color? = Nothing,
                                 Optional fillAlpha As Double = 1,
                                 Optional baseline As Double = 0) As Plottable.Polygon
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must all have the same length.")
            End If
            Dim xs2 As Double() = Tools.Pad(xs, cloneEdges:=True)
            Dim ys2 As Double() = Tools.Pad(ys, padWithLeft:=baseline, padWithRight:=baseline)
            Return PlotPolygon(xs2, ys2, label, lineWidth, lineColor, fill, fillColor, fillAlpha)
        End Function

        <Obsolete("Use AddFill() and customize the object it returns")>
        Public Function PlotFill(xs1 As Double(), ys1 As Double(), xs2 As Double(), ys2 As Double(),
                                 Optional label As String = Nothing,
                                 Optional lineWidth As Double = 0,
                                 Optional lineColor As Color? = Nothing,
                                 Optional fill As Boolean = True,
                                 Optional fillColor As Color? = Nothing,
                                 Optional fillAlpha As Double = 1,
                                 Optional baseline As Double = 0) As Plottable.Polygon
            If (xs1.Length <> ys1.Length) OrElse (xs2.Length <> ys2.Length) Then
                Throw New ArgumentException("Xs and Ys for each dataset must have the same length.")
            End If

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
            Return PlotPolygon(bothX, bothY, label, lineWidth, lineColor, fill, fillColor, fillAlpha)
        End Function

        <Obsolete("Use AddFill() and customize the object it returns")>
        Public Function PlotFillAboveBelow(xs As Double(), ys As Double(),
                                           Optional labelAbove As String = Nothing,
                                           Optional labelBelow As String = Nothing,
                                           Optional lineWidth As Double = 1,
                                           Optional lineColor As Color? = Nothing,
                                           Optional fill As Boolean = True,
                                           Optional fillColorAbove As Color? = Nothing,
                                           Optional fillColorBelow As Color? = Nothing,
                                           Optional fillAlpha As Double = 1,
                                           Optional baseline As Double = 0) As Tuple(Of Plottable.Polygon, Plottable.Polygon)
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must all have the same length.")
            End If

            Dim xs2 As Double() = Tools.Pad(xs, cloneEdges:=True)
            Dim ys2 As Double() = Tools.Pad(ys, padWithLeft:=baseline, padWithRight:=baseline)

            Dim ys2below As Double() = New Double(ys2.Length - 1) {}
            Dim ys2above As Double() = New Double(ys2.Length - 1) {}
            For i As Integer = 0 To ys2.Length - 1
                If (ys2(i) < baseline) Then
                    ys2below(i) = ys2(i)
                    ys2above(i) = baseline
                Else
                    ys2above(i) = ys2(i)
                    ys2below(i) = baseline
                End If
            Next

            If (fillColorAbove is Nothing) Then fillColorAbove = Color.Green
            If (fillColorBelow is Nothing) Then fillColorBelow = Color.Red
            If (lineColor is Nothing) Then lineColor = Color.Black

            Dim polyAbove As Plottable.Polygon = PlotPolygon(xs2, ys2above, labelAbove, lineWidth, lineColor, fill, fillColorAbove, fillAlpha)
            Dim polyBelow As Plottable.Polygon = PlotPolygon(xs2, ys2below, labelBelow, lineWidth, lineColor, fill, fillColorBelow, fillAlpha)

            Return New Tuple(Of Plottable.Polygon, Plottable.Polygon)(polyBelow, polyAbove)
        End Function

        <Obsolete("Use AddFill() and customize the object it returns")>
        Public Function PlotFillRightLeft(xs As Double(), ys As Double(),
                                          Optional labelRight As String = Nothing,
                                          Optional labelLeft As String = Nothing,
                                          Optional lineWidth As Double = 1,
                                          Optional lineColor As Color? = Nothing,
                                          Optional fill As Boolean = True,
                                          Optional fillColorRight As Color? = Nothing,
                                          Optional fillColorLeft As Color? = Nothing,
                                          Optional fillAlpha As Double = 1,
                                          Optional baseline As Double = 0) As Tuple(Of Plottable.Polygon, Plottable.Polygon)
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must all have the same length.")
            End If

            Dim xs2 As Double() = Tools.Pad(xs, padWithLeft:=baseline, padWithRight:=baseline)
            Dim ys2 As Double() = Tools.Pad(ys, cloneEdges:=True)

            Dim xs2below As Double() = New Double(xs2.Length - 1) {}
            Dim xs2above As Double() = New Double(xs2.Length - 1) {}

            For i As Integer = 0 To xs2.Length - 1
                If (xs2(i) < baseline) Then
                    xs2below(i) = xs2(i)
                    xs2above(i) = baseline
                Else
                    xs2above(i) = xs2(i)
                    xs2below(i) = baseline
                End If
            Next

            If (fillColorRight is Nothing) Then fillColorRight = Color.Green
            If (fillColorLeft is Nothing) Then fillColorLeft = Color.Red
            If (lineColor is Nothing) Then lineColor = Color.Black

            Dim polyRight As Plottable.Polygon = PlotPolygon(xs2above, ys2, labelRight, lineWidth, lineColor, fill, fillColorRight, fillAlpha)
            Dim polyLeft As Plottable.Polygon = PlotPolygon(xs2below, ys2, labelLeft, lineWidth, lineColor, fill, fillColorLeft, fillAlpha)

            Return New Tuple(Of Plottable.Polygon, Plottable.Polygon)(polyLeft, polyRight)
        End Function

        <Obsolete("Use AddFunction() and customize the object it returns")>
        Public Function PlotFunction(f As Func(Of Double, Double?),
                                     Optional color As Color? = Nothing,
                                     Optional lineWidth As Double = 1,
                                     Optional markerSize As Double = 0,
                                     Optional label As String = "f(x)",
                                     Optional markerShape As MarkerShape = MarkerShape.None,
                                     Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.FunctionPlot
            If (markerShape <> MarkerShape.None) OrElse (markerSize <> 0) Then
                Throw New ArgumentException("Function plots do not use markers.")
            End If
            Dim functionPlot As New Plottable.FunctionPlot(f) With {
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .LineStyle = lineStyle,
                .Label = label
            }
            Add(functionPlot)
            Return functionPlot
        End Function

        <Obsolete("Use AddHeatmap() and customize the object it returns")>
        Public Function PlotHeatmap(intensities As Double(,),
                                    Optional colormap As ScottPlot.Drawing.Colormap = Nothing,
                                    Optional label As String = Nothing,
                                    Optional axisOffsets As Double() = Nothing,
                                    Optional axisMultipliers As Double() = Nothing,
                                    Optional scaleMin As Double? = Nothing,
                                    Optional scaleMax As Double? = Nothing,
                                    Optional transparencyThreshold As Double? = Nothing,
                                    Optional backgroundImage As Bitmap = Nothing,
                                    Optional displayImageAbove As Boolean = False,
                                    Optional drawAxisLabels As Boolean = True) As Plottable.Heatmap
            Dim tmp As Double?(,) = New Double?(intensities.GetLength(0) - 1, intensities.GetLength(1) - 1) {}
            For i As Integer = 0 To intensities.GetLength(0) - 1
                For j As Integer = 0 To intensities.GetLength(1) - 1
                    tmp(i, j) = intensities(i, j)
                Next
            Next
            Return PlotHeatmap(tmp, colormap, label, axisOffsets, axisMultipliers, scaleMin, scaleMax,
                               transparencyThreshold, backgroundImage, displayImageAbove, drawAxisLabels)
        End Function

        <Obsolete("Create this plottable manually with new, then Add() it to the plot.")>
        Public Function PlotHeatmap(
            intensities As Double?(,),
            Optional colormap As ScottPlot.Drawing.Colormap = Nothing,
            Optional label As String = Nothing,
            Optional axisOffsets As Double() = Nothing,
            Optional axisMultipliers As Double() = Nothing,
            Optional scaleMin As Double? = Nothing,
            Optional scaleMax As Double? = Nothing,
            Optional transparencyThreshold As Double? = Nothing,
            Optional backgroundImage As Bitmap = Nothing,
            Optional displayImageAbove As Boolean = False,
            Optional drawAxisLabels As Boolean = True) As Plottable.Heatmap

            Dim heatmap As New Plottable.Heatmap() With {
                .Label = label,
                .TransparencyThreshold = transparencyThreshold,
                .BackgroundImage = backgroundImage,
                .DisplayImageAbove = displayImageAbove,
                .ShowAxisLabels = drawAxisLabels
            }
            heatmap.Update(intensities, If(colormap, ScottPlot.Drawing.Colormap.Viridis), scaleMin, scaleMax)

            Add(heatmap)
            Layout(top:=180)

            Return heatmap
        End Function

        <Obsolete("Use AddHorizontalLine() and customize the object it returns")>
        Public Function PlotHLine(y As Double,
                                  Optional color As Color? = Nothing,
                                  Optional lineWidth As Double = 1,
                                  Optional label As String = Nothing,
                                  Optional draggable As Boolean = False,
                                  Optional dragLimitLower As Double = Double.NegativeInfinity,
                                  Optional dragLimitUpper As Double = Double.PositiveInfinity,
                                  Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.HLine
            Dim hline As New Plottable.HLine() With {
                .Y = y,
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .Label = label,
                .DragEnabled = draggable,
                .LineStyle = lineStyle,
                .DragLimitMin = dragLimitLower,
                .DragLimitMax = dragLimitUpper
            }
            Add(hline)
            Return hline
        End Function

        <Obsolete("Use AddHorizontalSpan() and customize the object it returns")>
        Public Function PlotHSpan(x1 As Double, x2 As Double,
                                  Optional color As Color? = Nothing,
                                  Optional alpha As Double = 0.5,
                                  Optional label As String = Nothing,
                                  Optional draggable As Boolean = False,
                                  Optional dragFixedSize As Boolean = False,
                                  Optional dragLimitLower As Double = Double.NegativeInfinity,
                                  Optional dragLimitUpper As Double = Double.PositiveInfinity) As Plottable.HSpan
            Dim hspan As New Plottable.HSpan() With {
                .X1 = x1,
                .X2 = x2,
                .Color = If(color, GetNextColor(alpha)),
                .Label = label,
                .DragEnabled = draggable,
                .DragFixedSize = dragFixedSize,
                .DragLimitMin = dragLimitLower,
                .DragLimitMax = dragLimitUpper
            }
            Add(hspan)
            Return hspan
        End Function

        <Obsolete("use AddLine() and customize the object it returns")>
        Public Function PlotLine(x1 As Double, y1 As Double, x2 As Double, y2 As Double,
                                 Optional color As Color? = Nothing,
                                 Optional lineWidth As Double = 1,
                                 Optional label As String = Nothing,
                                 Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.ScatterPlot
            Return PlotScatter(New Double() {x1, x2},
                               New Double() {y1, y2},
                               color:=color,
                               lineWidth:=lineWidth,
                               label:=label,
                               lineStyle:=lineStyle,
                               markerSize:=0)
        End Function

        <Obsolete("use AddLine() and customize the object it returns")>
        Public Function PlotLine(slope As Double, offset As Double, xLimits As Tuple(Of Double, Double),
                                 Optional color As Color? = Nothing,
                                 Optional lineWidth As Double = 1,
                                 Optional label As String = Nothing,
                                 Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.ScatterPlot
            Dim y1 As Double = xLimits.Item1 * slope + offset
            Dim y2 As Double = xLimits.Item2 * slope + offset
            Return PlotScatter(New Double() {xLimits.Item1, xLimits.Item2},
                               New Double() {y1, y2},
                               color:=color,
                               lineWidth:=lineWidth,
                               label:=label,
                               lineStyle:=lineStyle,
                               markerSize:=0)
        End Function

        <Obsolete("use AddOHLC() and customize the object it returns")>
        Public Function PlotOHLC(ohlcs As OHLC(),
                                 Optional colorUp As Color? = Nothing,
                                 Optional colorDown As Color? = Nothing,
                                 Optional autoWidth As Boolean = True,
                                 Optional sequential As Boolean = False) As Plottable.FinancePlot
            Dim financePlot As New Plottable.FinancePlot(ohlcs) With {
                .Candle = False,
                .Sequential = sequential,
                .ColorUp = If(colorUp, ColorTranslator.FromHtml("#26a69a")),
                .ColorDown = If(colorDown, ColorTranslator.FromHtml("#ef5350"))}
            Add(financePlot)
            Return financePlot
        End Function

        <Obsolete("use AddPie() and customize the object it returns")>
        Public Function PlotPie(values As Double(),
                                Optional sliceLabels As String() = Nothing,
                                Optional colors As Color() = Nothing,
                                Optional explodedChart As Boolean = False,
                                Optional showValues As Boolean = False,
                                Optional showPercentages As Boolean = False,
                                Optional showLabels As Boolean = True,
                                Optional label As String = Nothing) As Plottable.PiePlot
            If (colors is Nothing) Then
                colors = Enumerable.Range(0, values.Length) _
                    .Select(Function(i As Integer) Settings.PlottablePalette.GetColor(i)).ToArray()
            End If
            Dim piePlot As New Plottable.PiePlot(values, sliceLabels, colors) With {
                .Explode = explodedChart,
                .ShowValues = showValues,
                .ShowPercentages = showPercentages,
                .ShowLabels = showLabels,
                .Label = label
            }
            Add(piePlot)
            Return piePlot
        End Function

        <Obsolete("Use AddPoint() and customize the object it returns")>
        Public Function PlotPoint(x As Double, y As Double,
                                  Optional color As Color? = Nothing,
                                  Optional markerSize As Double = 5,
                                  Optional label As String = Nothing,
                                  Optional errorX As Double? = Nothing,
                                  Optional errorY As Double? = Nothing,
                                  Optional errorLineWidth As Double = 1,
                                  Optional errorCapSize As Double = 3,
                                  Optional markerShape As MarkerShape = MarkerShape.FilledCircle,
                                  Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.ScatterPlot
            Throw New NotImplementedException()
        End Function

        <Obsolete("Use AddScatter() and customize the object it returns")>
        Public Function PlotScatter(xs As Double(), ys As Double(),
                                    Optional color As Color? = Nothing,
                                    Optional lineWidth As Double = 1,
                                    Optional markerSize As Double = 5,
                                    Optional label As String = Nothing,
                                    Optional errorX As Double() = Nothing,
                                    Optional errorY As Double() = Nothing,
                                    Optional errorLineWidth As Double = 1,
                                    Optional errorCapSize As Double = 3,
                                    Optional markerShape As MarkerShape = MarkerShape.FilledCircle,
                                    Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.ScatterPlot
            Dim scatterPlot As New Plottable.ScatterPlot(xs, ys, errorX, errorY) With {
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = CSng(markerSize),
                .Label = label,
                .ErrorLineWidth = errorLineWidth,
                .ErrorCapSize = CSng(errorCapSize),
                .StepDisplay = False,
                .MarkerShape = markerShape,
                .LineStyle = lineStyle
            }
            Add(scatterPlot)
            Return scatterPlot
        End Function

        <Obsolete("AddScatter() then AddPoint() and move the point around")>
        Public Function PlotScatterHighlight(xs As Double(), ys As Double(),
                                             Optional color As Color? = Nothing,
                                             Optional lineWidth As Double = 1,
                                             Optional markerSize As Double = 5,
                                             Optional label As String = Nothing,
                                             Optional errorX As Double() = Nothing,
                                             Optional errorY As Double() = Nothing,
                                             Optional errorLineWidth As Double = 1,
                                             Optional errorCapSize As Double = 3,
                                             Optional markerShape As MarkerShape = MarkerShape.FilledCircle,
                                             Optional lineStyle As LineStyle = LineStyle.Solid,
                                             Optional highlightedShape As MarkerShape = MarkerShape.OpenCircle,
                                             Optional highlightedColor As Color? = Nothing,
                                             Optional highlightedMarkerSize As Double? = Nothing) As Plottable.ScatterPlotHighlight
            If (color is Nothing) Then
                color = Settings.GetNextColor()
            End If
            If (highlightedColor is Nothing) Then
                highlightedColor = System.Drawing.Color.Red
            End If
            If (highlightedMarkerSize is Nothing) Then
                highlightedMarkerSize = 2 * markerSize
            End If
            Dim scatterPlotHighlight As New Plottable.ScatterPlotHighlight(xs, ys, errorX, errorY) With {
                .Color = color.Value,
                .LineWidth = lineWidth,
                .MarkerSize = CSng(markerSize),
                .Label = label,
                .ErrorLineWidth = errorLineWidth,
                .ErrorCapSize = CSng(errorCapSize),
                .StepDisplay = False,
                .MarkerShape = markerShape,
                .LineStyle = lineStyle,
                .HighlightedShape = highlightedShape,
                .HighlightedColor = highlightedColor.Value,
                .HighlightedMarkerSize = CSng(highlightedMarkerSize.Value)
            }
            Add(scatterPlotHighlight)
            Return scatterPlotHighlight
        End Function

        <Obsolete("Use AddSignal() and customize the object it returns")>
        Public Function PlotSignal(ys As Double(),
                                   Optional sampleRate As Double = 1,
                                   Optional xOffset As Double = 0,
                                   Optional yOffset As Double = 0,
                                   Optional color As Color? = Nothing,
                                   Optional lineWidth As Double = 1,
                                   Optional markerSize As Double = 5,
                                   Optional label As String = Nothing,
                                   Optional colorByDensity As Color() = Nothing,
                                   Optional minRenderIndex As Integer? = Nothing,
                                   Optional maxRenderIndex As Integer? = Nothing,
                                   Optional lineStyle As LineStyle = LineStyle.Solid,
                                   Optional useParallel As Boolean = True) As Plottable.SignalPlot
            Dim signalPlot As New Plottable.SignalPlot() With {
                .Ys = ys,
                .SampleRate = sampleRate,
                .OffsetX = xOffset,
                .OffsetY = yOffset,
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = CSng(markerSize),
                .Label = label,
                .DensityColors = colorByDensity,
                .MinRenderIndex = minRenderIndex.GetValueOrDefault(),
                .MaxRenderIndex = If(maxRenderIndex, ys.Length - 1),
                .LineStyle = lineStyle,
                .UseParallel = useParallel
            }
            Add(signalPlot)
            Return signalPlot
        End Function

        <Obsolete("Use AddSignalConst() and customize the object it returns")>
        Public Function PlotSignalConst(Of T As {Structure, IComparable})(ys As T(),
                                                                          Optional sampleRate As Double = 1,
                                                                          Optional xOffset As Double = 0,
                                                                          Optional yOffset As T = Nothing,
                                                                          Optional color As Color? = Nothing,
                                                                          Optional lineWidth As Double = 1,
                                                                          Optional markerSize As Double = 5,
                                                                          Optional label As String = Nothing,
                                                                          Optional colorByDensity As Color() = Nothing,
                                                                          Optional minRenderIndex As Integer? = Nothing,
                                                                          Optional maxRenderIndex As Integer? = Nothing,
                                                                          Optional lineStyle As LineStyle = LineStyle.Solid,
                                                                          Optional useParallel As Boolean = True) As Plottable.SignalPlotConst(Of T)
            Dim signalPlotConst As New Plottable.SignalPlotConst(Of T)() With {
                .Ys = ys,
                .SampleRate = sampleRate,
                .OffsetX = xOffset,
                .OffsetY = yOffset,
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = CSng(markerSize),
                .Label = label,
                .DensityColors = colorByDensity,
                .MinRenderIndex = minRenderIndex.GetValueOrDefault(),
                .MaxRenderIndex = If(maxRenderIndex, ys.Length - 1),
                .LineStyle = lineStyle,
                .UseParallel = useParallel
            }
            Add(signalPlotConst)
            Return signalPlotConst
        End Function

        <Obsolete("Use AddSignalXY() and customize the object it returns")>
        Public Function PlotSignalXY(xs As Double(), ys As Double(),
                                     Optional color As Color? = Nothing,
                                     Optional lineWidth As Double = 1,
                                     Optional markerSize As Double = 5,
                                     Optional label As String = Nothing,
                                     Optional minRenderIndex As Integer? = Nothing,
                                     Optional maxRenderIndex As Integer? = Nothing,
                                     Optional lineStyle As LineStyle = LineStyle.Solid,
                                     Optional useParallel As Boolean = True) As Plottable.SignalPlotXY
            Dim signalPlotXY As New Plottable.SignalPlotXY() With {
                .Xs = xs,
                .Ys = ys,
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = CSng(markerSize),
                .Label = label,
                .MinRenderIndex = minRenderIndex.GetValueOrDefault(),
                .MaxRenderIndex = If(maxRenderIndex, ys.Length - 1),
                .LineStyle = lineStyle,
                .UseParallel = useParallel
            }
            Add(signalPlotXY)
            Return signalPlotXY
        End Function

        <Obsolete("Use AddSignalXYConst() and customize the object it returns")>
        Public Function PlotSignalXYConst(Of TX As {Structure, IComparable},
                                              TY As {Structure, IComparable})(xs As TX(), ys As TY(),
                                                                              Optional color As Color? = Nothing,
                                                                              Optional lineWidth As Double = 1,
                                                                              Optional markerSize As Double = 5,
                                                                              Optional label As String = Nothing,
                                                                              Optional minRenderIndex As Integer? = Nothing,
                                                                              Optional maxRenderIndex As Integer? = Nothing,
                                                                              Optional lineStyle As LineStyle = LineStyle.Solid,
                                                                              Optional useParallel As Boolean = True) As Plottable.SignalPlotXYConst(Of TX, TY)
            Dim sigPlot As New Plottable.SignalPlotXYConst(Of TX, TY)() With {
                .Xs = xs,
                .Ys = ys,
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .MarkerSize = CSng(markerSize),
                .Label = label,
                .MinRenderIndex = minRenderIndex.GetValueOrDefault(),
                .MaxRenderIndex = If(maxRenderIndex, ys.Length - 1),
                .LineStyle = lineStyle,
                .UseParallel = useParallel
            }
            Add(sigPlot)
            Return sigPlot
        End Function

        <Obsolete("Use AddScatterStep() and customize the object it returns")>
        Public Function PlotStep(xs As Double(), ys As Double(),
                                 Optional color As Color? = Nothing,
                                 Optional lineWidth As Double = 1,
                                 Optional label As String = Nothing) As Plottable.ScatterPlot
            If (color is Nothing) Then
                color = Settings.GetNextColor()
            End If
            Dim scatterPlot As New Plottable.ScatterPlot(xs, ys, Nothing, Nothing) With {
                .Color = color.Value,
                .LineWidth = lineWidth,
                .MarkerSize = 0,
                .Label = label,
                .ErrorLineWidth = 0,
                .ErrorCapSize = 0,
                .StepDisplay = True,
                .MarkerShape = MarkerShape.None,
                .LineStyle = LineStyle.Solid
            }
            Add(scatterPlot)
            Return scatterPlot
        End Function

        <Obsolete("Use AddPolygon() and customize the object it returns")>
        Public Function PlotPolygon(xs As Double(), ys As Double(),
                                    Optional label As String = Nothing,
                                    Optional lineWidth As Double = 0,
                                    Optional lineColor As Color? = Nothing,
                                    Optional fill As Boolean = True,
                                    Optional fillColor As Color? = Nothing,
                                    Optional fillAlpha As Double = 1) As Plottable.Polygon
            Dim polygon As New Plottable.Polygon(xs, ys) With {
                .Label = label,
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, Color.Black),
                .Fill = fill,
                .FillColor = If(fillColor, GetNextColor(fillAlpha))
            }
            Add(polygon)
            Return polygon
        End Function

        <Obsolete("Use AddPolygons() and customize the object it returns")>
        Public Function PlotPolygons(polys As List(Of List(Of Tuple(Of Double, Double))),
                                     Optional label As String = Nothing,
                                     Optional lineWidth As Double = 0,
                                     Optional lineColor As Color? = Nothing,
                                     Optional fill As Boolean = True,
                                     Optional fillColor As Color? = Nothing,
                                     Optional fillAlpha As Double = 1) As Plottable.Polygons
            Dim polygons As New Plottable.Polygons(polys) With {
                .Label = label,
                .LineWidth = lineWidth,
                .LineColor = If(lineColor, Color.Black),
                .Fill = fill,
                .FillColor = Color.FromArgb(CInt(255 * fillAlpha), If(fillColor, GetNextColor()))
            }
            Add(polygons)
            Return polygons
        End Function

        <Obsolete("Use AddPopulation() and customize the object it returns")>
        Public Function PlotPopulations(population As Statistics.Population, Optional label As String = Nothing) As Plottable.PopulationPlot
            Dim populationPlot As New Plottable.PopulationPlot(population, label, Settings.GetNextColor())
            Add(populationPlot)
            Return populationPlot
        End Function

        <Obsolete("Use AddPopulations() and customize the object it returns")>
        Public Function PlotPopulations(populations As Statistics.Population(), Optional label As String = Nothing) As Plottable.PopulationPlot
            Dim populationPlot As New Plottable.PopulationPlot(populations, label)
            Add(populationPlot)
            Return populationPlot
        End Function

        <Obsolete("Use AddPopulations() and customize the object it returns")>
        Public Function PlotPopulations(series As Statistics.PopulationSeries, Optional label As String = Nothing) As Plottable.PopulationPlot
            series.Color = Settings.GetNextColor()
            If (label IsNot Nothing) Then
                series.SeriesLabel = label
            End If
            Dim populationPlot As New Plottable.PopulationPlot(series)
            Add(populationPlot)
            Return populationPlot
        End Function

        <Obsolete("Use AddPopulations() and customize the object it returns")>
        Public Function PlotPopulations(multiSeries As Statistics.PopulationMultiSeries) As Plottable.PopulationPlot
            For i As Integer = 0 To multiSeries.MultiSeries.Length - 1
                multiSeries.MultiSeries(i).Color = Settings.PlottablePalette.GetColor(i)
            Next
            Dim populationPlot As New Plottable.PopulationPlot(multiSeries)
            Add(populationPlot)
            Return populationPlot
        End Function

        <Obsolete("Use AddRader() and customize the object it returns")>
        Public Function PlotRadar(values As Double(,),
                                  Optional categoryNames As String() = Nothing,
                                  Optional groupNames As String() = Nothing,
                                  Optional fillColors As Color() = Nothing,
                                  Optional fillAlpha As Double = 0.4,
                                  Optional webColor As Color? = Nothing,
                                  Optional independentAxes As Boolean = False,
                                  Optional maxValues As Double() = Nothing) As Plottable.RadarPlot
            Dim colors As Color() = If(fillColors,
                Enumerable.Range(0, values.Length).Select(Function(i As Integer)
                                                              Return Settings.PlottablePalette.GetColor(i)
                                                          End Function).ToArray())
            Dim colorsAlpha As Color() = colors.Select(Function(x As Color)
                                                           Return Color.FromArgb(CInt(255 * fillAlpha), x)
                                                       End Function).ToArray()
            Dim radarPlot As New Plottable.RadarPlot(values, colors, If(fillColors, colorsAlpha), independentAxes, maxValues) With {
                .CategoryLabels = categoryNames,
                .GroupLabels = groupNames,
                .WebColor = If(webColor, Color.Gray)
            }
            Add(radarPlot)
            Return radarPlot
        End Function

        <Obsolete("use AddScalebar() and customize the object it returns")>
        Public Function PlotScaleBar(sizeX As Double, sizeY As Double,
                                     Optional labelX As String = Nothing,
                                     Optional labelY As String = Nothing,
                                     Optional thickness As Double = 2,
                                     Optional fontSize As Double = 12,
                                     Optional color As Color? = Nothing,
                                     Optional padPx As Double = 10) As Plottable.ScaleBar
            Dim scaleBar As New Plottable.ScaleBar() With {
                .Width = sizeX,
                .Height = sizeY,
                .HorizontalLabel = labelX,
                .VerticalLabel = labelY,
                .LineWidth = CSng(thickness),
                .FontSize = CSng(fontSize),
                .FontColor = If(color, System.Drawing.Color.Black),
                .LineColor = If(color, System.Drawing.Color.Black),
                .Padding = CSng(padPx)
            }
            Add(scaleBar)
            Return scaleBar
        End Function

        <Obsolete("Use AddText() and customize the object it returns")>
        Public Function PlotText(text As String, x As Double, y As Double,
                                 Optional color As Color? = Nothing,
                                 Optional fontName As String = Nothing,
                                 Optional fontSize As Double = 12,
                                 Optional bold As Boolean = False,
                                 Optional label As String = Nothing,
                                 Optional alignment As Alignment = Alignment.MiddleLeft,
                                 Optional rotation As Double = 0,
                                 Optional frame As Boolean = False,
                                 Optional frameColor As Color? = Nothing) As Plottable.Text
            If String.IsNullOrWhiteSpace(label) Then
                Diagnostics.Debug.WriteLine("WARNING: the PlotText() label argument is ignored.")
            End If
            Dim txt As New Plottable.Text() With {
                .Label = text,
                .X = x,
                .Y = y,
                .Color = If(color, Settings.GetNextColor()),
                .FontName = fontName,
                .FontSize = CSng(fontSize),
                .FontBold = bold,
                .Alignment = alignment,
                .Rotation = CSng(rotation),
                .BackgroundFill = frame,
                .BackgroundColor = If(frameColor, System.Drawing.Color.White)
            }
            Add(txt)
            Return txt
        End Function

        <Obsolete("Create a VectorField manually then call Add()")>
        Public Function PlotVectorField(vectors As Statistics.Vector2(,), xs As Double(), ys As Double(),
                                        Optional label As String = Nothing,
                                        Optional color As Color? = Nothing,
                                        Optional colormap As ScottPlot.Drawing.Colormap = Nothing,
                                        Optional scaleFactor As Double = 1) As Plottable.VectorField
            Dim vectorField As New Plottable.VectorField(vectors,
                                               xs,
                                               ys,
                                               colormap,
                                               scaleFactor,
                                               If(color, Settings.GetNextColor())) With {.Label = label}
            Add(vectorField)
            Return vectorField
        End Function

        <Obsolete("Use AddVerticalLine() and customize the object it returns")>
        Public Function PlotVLine(x As Double,
                                  Optional color As Color? = Nothing,
                                  Optional lineWidth As Double = 1,
                                  Optional label As String = Nothing,
                                  Optional draggable As Boolean = False,
                                  Optional dragLimitLower As Double = Double.NegativeInfinity,
                                  Optional dragLimitUpper As Double = Double.PositiveInfinity,
                                  Optional lineStyle As LineStyle = LineStyle.Solid) As Plottable.VLine
            Dim vline As New Plottable.VLine() With {
                .X = x,
                .Color = If(color, Settings.GetNextColor()),
                .LineWidth = lineWidth,
                .Label = label,
                .DragEnabled = draggable,
                .LineStyle = lineStyle,
                .DragLimitMin = dragLimitLower,
                .DragLimitMax = dragLimitUpper
            }
            Add(vline)
            Return vline
        End Function

        <Obsolete("Use AddVerticalSpan() and customize the object it returns")>
        Public Function PlotVSpan(y1 As Double, y2 As Double,
                                  Optional color As Color? = Nothing,
                                  Optional alpha As Double = 0.5,
                                  Optional label As String = Nothing,
                                  Optional draggable As Boolean = False,
                                  Optional dragFixedSize As Boolean = False,
                                  Optional dragLimitLower As Double = Double.NegativeInfinity,
                                  Optional dragLimitUpper As Double = Double.PositiveInfinity) As Plottable.VSpan
            Dim vspan As New Plottable.VSpan() With {
                .Y1 = y1,
                .Y2 = y2,
                .Color = (If(color, GetNextColor(alpha))),
                .Label = label,
                .DragEnabled = draggable,
                .DragFixedSize = dragFixedSize,
                .DragLimitMin = dragLimitLower,
                .DragLimitMax = dragLimitUpper
            }
            Add(vspan)
            Return vspan
        End Function

        <Obsolete("This method has been replaced by AddBar() and one line of Linq (see cookbook)")>
        Public Function PlotWaterfall(xs As Double(), ys As Double(),
                                      Optional errorY As Double() = Nothing,
                                      Optional label As String = Nothing,
                                      Optional barWidth As Double = 0.8,
                                      Optional xOffset As Double = 0,
                                      Optional fill As Boolean = True,
                                      Optional fillColor As Color? = Nothing,
                                      Optional outlineWidth As Double = 1,
                                      Optional outlineColor As Color? = Nothing,
                                      Optional errorLineWidth As Double = 1,
                                      Optional errorCapSize As Double = 0.38,
                                      Optional errorColor As Color? = Nothing,
                                      Optional horizontal As Boolean = False,
                                      Optional showValues As Boolean = False,
                                      Optional valueColor As Color? = Nothing,
                                      Optional autoAxis As Boolean = True,
                                      Optional negativeColor As Color? = Nothing) As Plottable.BarPlot
            Dim yOffsets As Double() = Enumerable.Range(0, ys.Length).Select(Function(count As Integer)
                                                                                 Return ys.Take(count).Sum()
                                                                             End Function).ToArray()
            Return PlotBar(xs, ys, errorY, label, barWidth, xOffset, fill, fillColor,
                           outlineWidth, outlineColor,
                           errorLineWidth, errorCapSize, errorColor,
                           horizontal, showValues, valueColor, autoAxis, yOffsets, negativeColor)
        End Function

    End Class

End Namespace