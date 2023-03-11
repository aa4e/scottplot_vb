Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Linq

Namespace ScottPlot.Plottable

    Public MustInherit Class SignalPlotBase(Of T As {Structure, IComparable})
        Implements IPlottable, IHasLine, IHasMarker, IHighlightable, IHasColor, IHasPointsGenericX(Of Double, T)

#Region "PROPS, FIELDS"

        Protected Strategy As MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T) = New MinMaxSearchStrategies.SegmentedTreeMinMaxSearchStrategy(Of T)()

        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible

        Public Property StepDisplay As Boolean = False
        Public Property Smooth As Boolean = False
        Public Property SmoothTension As Double = 0.5

        Private DensityLevelCount As Integer = 0
        Private PenColorsByDensity As Color()

        ''' <summary>
        ''' Describes orientation of steps if <see cref="StepDisplay"/> is enabled. 
        ''' If true, lines will extend to the right before ascending or descending to the level of the following point.
        ''' </summary>
        Public Property StepDisplayRight As Boolean = True

        Public Property OffsetX As Double

        Public Property OffsetY As T

        Public ReadOnly Property OffsetYAsDouble As Double
            Get
                Dim offsY As T = Me.OffsetY
                Return NumericConversion.GenericToDouble(Of T)(offsY)
            End Get
        End Property

        Public Property Label As String = "Signal plot"

        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return _Color
            End Get
            Set(value As Color)
                _Color = value
            End Set
        End Property
        Private _Color As Color = Color.Green

        Public Property LineColor As Color Implements ScottPlot.Plottable.IHasLine.LineColor
            Get
                Return _LineColor
            End Get
            Set(value As Color)
                _LineColor = value
            End Set
        End Property
        Private _LineColor As Color = Tools.GetRandomColor()

        Public Property MarkerShape As MarkerShape = MarkerShape.FilledCircle Implements ScottPlot.Plottable.IHasMarker.MarkerShape

        Public Property MarkerColor As Color Implements ScottPlot.Plottable.IHasMarker.MarkerColor
            Get
                Return _MarkerColor
            End Get
            Set(value As Color)
                _MarkerColor = value
            End Set
        End Property
        Private _MarkerColor As Color

        Public Property MarkerSize As Single Implements ScottPlot.Plottable.IHasMarker.MarkerSize
            Get
                If IsHighlighted Then
                    Return _MarkerSize * HighlightCoefficient
                End If
                Return _MarkerSize
            End Get
            Set(value As Single)
                _MarkerSize = value
            End Set
        End Property
        Private _MarkerSize As Single = 5

        Public Property MarkerLineWidth As Single Implements ScottPlot.Plottable.IHasMarker.MarkerLineWidth
            Get
                If IsHighlighted Then
                    Return _MarkerLineWidth * HighlightCoefficient
                End If
                Return _MarkerLineWidth
            End Get
            Set(value As Single)
                _MarkerLineWidth = value
            End Set
        End Property
        Private _MarkerLineWidth As Single

        Public Property LineWidth As Double Implements ScottPlot.Plottable.IHasLine.LineWidth
            Get
                If IsHighlighted Then
                    Return _LineWidth * HighlightCoefficient
                End If
                Return _LineWidth
            End Get
            Set(value As Double)
                _LineWidth = value
            End Set
        End Property
        Private _LineWidth As Double = 1

        Public Property LineStyle As LineStyle = LineStyle.Solid Implements ScottPlot.Plottable.IHasLine.LineStyle
        Public Property IsHighlighted As Boolean Implements ScottPlot.Plottable.IHighlightable.IsHighlighted
        Public Property HighlightCoefficient As Single = 2 Implements ScottPlot.Plottable.IHighlightable.HighlightCoefficient

        ''' <summary>
        ''' If enabled, parallel processing will be used to calculate pixel positions for high density datasets.
        ''' </summary>
        Public Property UseParallel As Boolean = True

        ''' <summary>
        ''' If fill above and/or below is enabled, this defines the baseline level.
        ''' </summary>
        Public Property BaselineY As Double = 0

        ''' <summary>
        ''' If fill is enabled, a baseline will be drawn using this color.
        ''' </summary>
        Public Property BaselineColor As Color = Color.Black

        ''' <summary>
        ''' If fill is enabled, a baseline will be drawn using this width.
        ''' </summary>
        Public Property BaselineWidth As Single = 1

        ''' <summary>
        ''' When markers are visible on the line (low density mode) this is True.
        ''' </summary>
        Protected Property ShowMarkersInLegend As Boolean = False

        Public Overridable Property Ys As T()
            Get
                Return _Ys
            End Get
            Set(value As T())
                If (value is Nothing) Then
                    Throw New Exception("Y data cannot be null.")
                End If
                _Ys = value
                Strategy.SourceArray = _Ys
            End Set
        End Property
        Protected _Ys As T()

        Public ReadOnly Property PointCount As Integer
            Get
                Return _Ys.Length
            End Get
        End Property

        ''' <summary>
        ''' Measurements per second, Hz.
        ''' </summary>
        Public Property SampleRate As Double
            Get
                Return _SampleRate
            End Get
            Set(value As Double)
                If (value <= 0) Then
                    Throw New Exception("SampleRate must be greater then zero.")
                End If
                _SampleRate = value
                _SamplePeriod = 1 / value
            End Set
        End Property
        Private _SampleRate As Double = 1

        Public Property SamplePeriod As Double
            Get
                Return _SamplePeriod
            End Get
            Set(value As Double)
                If (value <= 0) Then
                    Throw New Exception("SamplePeriod must be greater then zero.")
                End If
                _SamplePeriod = value
                _SampleRate = 1.0 / value
            End Set
        End Property
        Private _SamplePeriod As Double = 1

        Public Property MinRenderIndex As Integer
            Get
                Return _MinRenderIndex
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("MinRenderIndex must be positive.")
                End If
                _MinRenderIndex = value
            End Set
        End Property
        Protected _MinRenderIndex As Integer = 0

        Public Property MaxRenderIndex As Integer
            Get
                Return _MaxRenderIndex
            End Get
            Set(value As Integer)
                If value < 0 Then
                    Throw New ArgumentException("MaxRenderIndex must be positive.")
                End If
                _MaxRenderIndex = value
            End Set
        End Property
        Protected _MaxRenderIndex As Integer = 0

        Public WriteOnly Property DensityColors As Color()
            Set(value As Color())
                If (value IsNot Nothing) Then
                    'turn the ramp into a pen triangle
                    DensityLevelCount = value.Length * 2 - 1
                    PenColorsByDensity = New Color(DensityLevelCount - 1) {}
                    For i As Integer = 0 To value.Length - 1
                        PenColorsByDensity(i) = value(i)
                        PenColorsByDensity(DensityLevelCount - 1 - i) = value(i)
                    Next
                End If
            End Set
        End Property

        '''' <summary>
        '''' If fill is enabled, this color will be used to fill the area below the curve above BaselineY.
        '''' </summary>
        '<Obsolete("Use the Fill() methods of this object to configure this setting")>
        'Public Property GradientFillColor1 As Color?
        '    Get
        '        Return _GradientFillColor1
        '    End Get
        '    Set(value As Color?)
        '        _GradientFillColor1 = value
        '    End Set
        'End Property
        Private _GradientFillColor1 As Color? = Nothing

        '''' <summary>
        '''' If fill is enabled, this color will be used to fill the area above the curve below BaselineY.
        '''' </summary>
        '<Obsolete("Use the Fill() methods of this object to configure this setting")>
        'Public Property GradientFillColor2 As Color?
        '    Get
        '        Return _GradientFillColor2
        '    End Get
        '    Set(value As Color?)
        '        _GradientFillColor2 = value
        '    End Set
        'End Property
        Private _GradientFillColor2 As Color? = Nothing

        '<Obsolete("Use the Fill() methods of this object to configure this setting")>
        'Public Property FillType As FillType
        '    Get
        '        Return _FillType
        '    End Get
        '    Set(value As FillType)
        '        _FillType = value
        '    End Set
        'End Property
        Protected _FillType As FillType = FillType.NoFill

        '<Obsolete("Use the Fill() methods of this object to configure this setting")>
        'Public Property FillColor1 As Color?
        '    Get
        '        Return _FillColor1
        '    End Get
        '    Set(value As Color?)
        '        _FillColor1 = value
        '    End Set
        'End Property
        Protected _FillColor1 As Color? = Nothing

        '<Obsolete("Use the Fill() methods of this object to configure this setting")>
        'Public Property FillColor2 As Color?
        '    Get
        '        Return _FillColor2
        '    End Get
        '    Set(value As Color?)
        '        _FillColor2 = value
        '    End Set
        'End Property
        Protected _FillColor2 As Color? = Nothing

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New()
            'Strategy = New SegmentedTreeMinMaxSearchStrategy < T > ()
        End Sub

#End Region '/CTOR

#Region "METHODS"

        ''' <summary>
        ''' This expression adds two parameters of the generic type used by this signal plot.
        ''' </summary>
        Private ReadOnly AddYsGenericExpression As Func(Of T, T, T) = NumericConversion.CreateAddFunction(Of T)()

        ''' <summary>
        ''' Add two Y values (of the generic type used by this signal plot) and return the result as a double.
        ''' </summary>
        Private Function AddYs(y1 As T, y2 As T) As Double
            Dim t As T = AddYsGenericExpression(y1, y2)
            Return NumericConversion.GenericToDouble(Of T)(t)
        End Function

        ''' <summary>
        ''' Add two Y values (of the generic type used by this signal plot) and return the result as a the same type.
        ''' </summary>
        Private Function AddYsGeneric(y1 As T, y2 As T) As T
            Return AddYsGenericExpression(y1, y2)
        End Function

        ''' <summary>
        ''' Replace a single Y value.
        ''' </summary>
        ''' <param name="index">Array index to replace.</param>
        ''' <param name="newValue">New value.</param>
        Public Sub Update(index As Integer, newValue As T)
            Strategy.UpdateElement(index, newValue)
        End Sub

        ''' <summary>
        ''' Replace a range of Y values.
        ''' </summary>
        ''' <param name="firstIndex">Index to begin replacing.</param>
        ''' <param name="lastIndex">Last index to replace.</param>
        ''' <param name="newData">Source for new data.</param>
        ''' <param name="fromData">Source data offset.</param>
        Public Sub Update(firstIndex As Integer, lastIndex As Integer, newData As T(), Optional fromData As Integer = 0)
            If (firstIndex < 0 OrElse firstIndex > Ys.Length - 1) Then
                Throw New InvalidOperationException($"{NameOf(firstIndex)} cannot exceed the dimensions of the existing {NameOf(Ys)} array.")
            End If
            If (lastIndex > Ys.Length - 1) Then
                Throw New InvalidOperationException($"{NameOf(lastIndex)} cannot exceed the dimensions of the existing {NameOf(Ys)} array.")
            End If
            Strategy.UpdateRange(firstIndex, lastIndex, newData, fromData)
        End Sub

        ''' <summary>
        ''' Replace all Y values from the given index through the end of the array.
        ''' </summary>
        ''' <param name="firstIndex">first index to begin replacing.</param>
        ''' <param name="newData">New values.</param>
        Public Sub Update(firstIndex As Integer, newData As T())
            If (firstIndex < 0 OrElse firstIndex > Ys.Length - 1) Then
                Throw New InvalidOperationException($"{NameOf(firstIndex)} cannot exceed the dimensions of the existing {NameOf(Ys)} array.")
            End If
            Update(firstIndex, firstIndex + newData.Length, newData)
        End Sub

        ''' <summary>
        ''' Replace all Y values with new ones.
        ''' </summary>
        ''' <param name="newData">New Y values.</param>
        Public Sub Update(newData As T())
            If (newData.Length > Ys.Length) Then
                Throw New InvalidOperationException($"{NameOf(newData)} cannot exceed the dimensions of the existing {NameOf(Ys)} array.")
            End If
            Update(0, newData.Length, newData)
        End Sub

        Public Overridable Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If (Ys.Length = 0) Then
                Return AxisLimits.NoLimits
            End If

            Dim xMin As Double = _SamplePeriod * MinRenderIndex
            Dim xMax As Double = _SamplePeriod * MaxRenderIndex
            Dim yMin As Double
            Dim yMax As Double
            Strategy.MinMaxRangeQuery(MinRenderIndex, MaxRenderIndex, yMin, yMax)

            If (Double.IsNaN(yMin) OrElse Double.IsNaN(yMax)) Then
                Throw New InvalidOperationException("Signal data must not contain NaN.")
            End If
            If (Double.IsInfinity(yMin) OrElse Double.IsInfinity(yMax)) Then
                Return AxisLimits.NoLimits
                Throw New InvalidOperationException("Signal data must not contain Infinity.")
            End If

            Dim offsetY As Double = OffsetYAsDouble
            Return New AxisLimits(xMin + OffsetX, xMax + OffsetX, yMin + offsetY, yMax + offsetY)
        End Function

        ''' <summary>
        ''' Render when the data is zoomed out so much that it just looks like a vertical line.
        ''' </summary>
        Private Sub RenderSingleLine(dims As PlotDimensions, gfx As Graphics, penHD As Pen)
            'This function is for when the graph is zoomed so far out its entire display is a single vertical pixel column
            Dim yMin As Double
            Dim yMax As Double
            Strategy.MinMaxRangeQuery(MinRenderIndex, MaxRenderIndex, yMin, yMax)
            Dim offsetY As Double = Me.OffsetYAsDouble
            Dim pt1 As New PointF(dims.GetPixelX(OffsetX), dims.GetPixelY(yMin + offsetY))
            Dim pt2 As New PointF(dims.GetPixelX(OffsetX), dims.GetPixelY(yMax + offsetY))
            gfx.DrawLine(penHD, pt1, pt2)
        End Sub

        ''' <summary>
        ''' Render when the data is zoomed in such that there is more than 1 column per data point.
        ''' Rendering is accomplished by drawing a straight line from point to point.
        ''' </summary>
        Private Sub RenderLowDensity(dims As PlotDimensions, gfx As Graphics, visibleIndex1 As Integer, visibleIndex2 As Integer, penLD As Pen)
            Dim capacity As Integer = visibleIndex2 - visibleIndex1 + 2
            Dim linePoints As New List(Of PointF)(visibleIndex2 - visibleIndex1 + 2)
            If (visibleIndex2 > _Ys.Length - 2) Then
                visibleIndex2 = _Ys.Length - 2
            End If
            If (visibleIndex2 > MaxRenderIndex - 1) Then
                visibleIndex2 = MaxRenderIndex - 1
            End If
            If (visibleIndex1 < 0) Then
                visibleIndex1 = 0
            End If
            If (visibleIndex1 < MinRenderIndex) Then
                visibleIndex1 = MinRenderIndex
            End If

            For i As Integer = visibleIndex1 To visibleIndex2 + 1
                Dim yCoordinateWithOffset As Double = AddYs(Ys(i), OffsetY)
                Dim pixelY As Single = dims.GetPixelY(yCoordinateWithOffset)
                Dim pixelX As Single = dims.GetPixelX(_SamplePeriod * i + OffsetX)
                Dim linePoint As New PointF(pixelX, pixelY)
                linePoints.Add(linePoint)
            Next

            If (linePoints.Count > 1) Then
                Dim pointsArray As PointF() = linePoints.ToArray()
                ValidatePoints(pointsArray)

                If StepDisplay Then
                    pointsArray = ScatterPlot.GetStepDisplayPoints(pointsArray, StepDisplayRight)
                End If

                If (LineWidth > 0) AndAlso (LineStyle <> LineStyle.None) Then
                    If Smooth Then
                        gfx.DrawCurve(penLD, pointsArray, CSng(SmoothTension))
                    Else
                        gfx.DrawLines(penLD, pointsArray)
                    End If
                End If

                Select Case _FillType
                    Case FillType.NoFill

                    Case FillType.FillAbove
                        FillToInfinity(dims, gfx, linePoints(0).X, linePoints(linePoints.Count - 1).X, pointsArray, True)

                    Case FillType.FillBelow
                        FillToInfinity(dims, gfx, linePoints(0).X, linePoints(linePoints.Count - 1).X, pointsArray, False)

                    Case FillType.FillAboveAndBelow
                        FillToBaseline(dims, gfx, linePoints(0).X, linePoints(linePoints.Count - 1).X, pointsArray, BaselineY)

                    Case Else
                        Throw New InvalidOperationException("Unsupported fill type.")
                End Select
            End If

            If (MarkerSize > 0) AndAlso (MarkerShape <> MarkerShape.None) Then
                'make markers transition away smoothly by making them smaller as the user zooms out
                Dim pixelsBetweenPoints As Single = CSng(If(Ys.Length > 1, _SamplePeriod, 1) * dims.DataWidth / dims.XSpan)
                Dim zoomTransitionScale As Single = Math.Min(1, pixelsBetweenPoints / 10)
                Dim markerPxDiameter As Single = MarkerSize * zoomTransitionScale
                Dim markerPxRadius = markerPxDiameter / 2
                If (markerPxRadius > 0.25) Then
                    ShowMarkersInLegend = True
                    MarkerTools.DrawMarkers(gfx, linePoints, MarkerShape, markerPxDiameter, MarkerColor, MarkerLineWidth)
                Else
                    ShowMarkersInLegend = False
                End If
            End If
        End Sub

        Private Function CalcInterval(xPx As Integer, offsetPoints As Double, columnPointCount As Double, dims As PlotDimensions) As SignalPlotBase(Of T).IntervalMinMax
            Dim index1 As Integer = CInt(offsetPoints + columnPointCount * xPx)
            Dim index2 As Integer = CInt(offsetPoints + columnPointCount * (xPx + 1))

            If (index1 < 0) Then
                index1 = 0
            End If
            If (index1 < MinRenderIndex) Then
                index1 = MinRenderIndex
            End If
            If (index2 > _Ys.Length - 1) Then
                index2 = _Ys.Length - 1
            End If
            If (index2 > MaxRenderIndex) Then
                index2 = MaxRenderIndex
            End If

            'Get the min and max value for this column
            Dim lowestValue As Double
            Dim highestValue As Double
            Strategy.MinMaxRangeQuery(index1, index2, lowestValue, highestValue)
            Dim yPxHigh As Single = dims.GetPixelY(lowestValue + OffsetYAsDouble)
            Dim yPxLow As Single = dims.GetPixelY(highestValue + OffsetYAsDouble)

            Return New IntervalMinMax(xPx, yPxLow, yPxHigh)
        End Function

        ''' <summary>
        ''' Render the data when there is more than one data point per pixel column.
        ''' Each pixel column therefore represents multiple data points.
        ''' Rendering is optimized by determining the min/max for each pixel column, then a single line is drawn connecting those values.
        ''' </summary>
        Private Sub RenderHighDensity(dims As PlotDimensions, gfx As Graphics, offsetPoints As Double, columnPointCount As Double, penHD As Pen)
            Dim dataColumnFirst As Integer = CInt(Math.Ceiling((-1 - offsetPoints + MinRenderIndex) / columnPointCount - 1))
            Dim dataColumnLast As Integer = CInt(Math.Ceiling((MaxRenderIndex - offsetPoints) / columnPointCount))
            dataColumnFirst = Math.Max(0, dataColumnFirst)
            dataColumnLast = Math.Min(CInt(dims.DataWidth), dataColumnLast)

            If (dataColumnFirst >= dataColumnLast) Then
                Return
            End If

            Dim columns = Enumerable.Range(dataColumnFirst, dataColumnLast - dataColumnFirst)
            Dim xPixelStart As Single = CSng(dataColumnFirst + dims.DataOffsetX)
            Dim xPixelEnd As Single = CSng(dataColumnLast + dims.DataOffsetX)

            Dim intervals As IEnumerable(Of IntervalMinMax)
            If UseParallel Then
                intervals = columns _
                    .AsParallel() _
                    .AsOrdered() _
                    .Select(Function(xPx As Integer) CalcInterval(xPx, offsetPoints, columnPointCount, dims)) _
                    .AsSequential()
            Else
                intervals = columns _
                    .Select(Function(xPx As Integer) CalcInterval(xPx, offsetPoints, columnPointCount, dims))
            End If

            Dim linePoints As PointF() = intervals _
                .SelectMany(Function(c As IntervalMinMax) c.GetPoints()) _
                .ToArray()

            'Adjust order of points to enhance anti-aliasing
            Dim buf As PointF
            For i As Integer = 1 To linePoints.Length \ 2 - 1
                If (linePoints(i * 2).Y >= linePoints(i * 2 - 1).Y) Then
                    buf = linePoints(i * 2)
                    linePoints(i * 2) = linePoints(i * 2 + 1)
                    linePoints(i * 2 + 1) = buf
                End If
            Next

            For i As Integer = 0 To linePoints.Length - 1
                linePoints(i).X += dims.DataOffsetX
            Next

            'TODO Откуда здесь бесконечность?

            If (linePoints.Length > 0) Then
                ValidatePoints(linePoints)
                gfx.DrawLines(penHD, linePoints)
            End If

            Select Case _FillType
                Case FillType.NoFill

                Case FillType.FillAbove
                    FillToInfinity(dims, gfx, xPixelStart, xPixelEnd, linePoints, True)

                Case FillType.FillBelow
                    FillToInfinity(dims, gfx, xPixelStart, xPixelEnd, linePoints, False)

                Case FillType.FillAboveAndBelow
                    FillToBaseline(dims, gfx, xPixelStart, xPixelEnd, linePoints, BaselineY)

                Case Else
                    Throw New InvalidOperationException("Unsupported fill type.")
            End Select
        End Sub

        ''' <summary>
        ''' Shade the region abvove or below the curve (to infinity) by drawing a polygon to the edge of the visible plot area.
        ''' </summary>
        Friend Sub FillToInfinity(dims As PlotDimensions, gfx As Graphics, xPxStart As Single, xPxEnd As Single, linePoints As PointF(), fillToPositiveInfinity As Boolean)
            If (xPxEnd - xPxStart = 0) OrElse (dims.Height = 0) Then
                Return
            End If
            Dim minVal As Single = 0
            Dim maxVal As Single = dims.DataHeight * If(fillToPositiveInfinity, -1, 1) + dims.DataOffsetY

            Dim first As New PointF(xPxStart, maxVal)
            Dim last As New PointF(xPxEnd, maxVal)

            Dim points As PointF() = New PointF() _
                {first} _
                .Concat(linePoints) _
                .Concat(New PointF() {last}).ToArray()

            Using brsh As New LinearGradientBrush(New Rectangle(CInt(first.X),
                                                                CInt(minVal) - If(fillToPositiveInfinity, 2, 0),
                                                                CInt((last.X - first.X)),
                                                                CInt(dims.Height)),
                                                                _FillColor1.Value,
                                                                If(_GradientFillColor1, _FillColor1.Value),
                                                                LinearGradientMode.Vertical)
                gfx.FillPolygon(brsh, points)
            End Using
        End Sub

        Private Function GetIntersection(point1 As PointF, point2 As PointF, baselineStart As PointF, baselineEnd As PointF) As PointF?
            Dim a1 As Double = point2.Y - point1.Y
            Dim b1 As Double = point1.X - point2.X
            Dim c1 As Double = a1 * point1.X + b1 * point1.Y

            Dim a2 As Double = baselineEnd.Y - baselineStart.Y
            Dim b2 As Double = baselineStart.X - baselineEnd.X
            Dim c2 As Double = a2 * baselineStart.X + b2 * baselineStart.Y

            Dim d As Double = a1 * b2 - a2 * b1
            If (d = 0) Then 'Lines do not intersect. This could also be the case if the plot is zoomed out too much.
                Return Nothing
            Else
                Dim x As Double = (b2 * c1 - b1 * c2) / d
                Dim y As Double = (a1 * c2 - a2 * c1) / d
                Return New PointF(CSng(x), CSng(y))
            End If
        End Function

        ''' <summary>
        ''' Shade the region abvove and below the curve (to the baseline level) by drawing two polygons.
        ''' </summary>
        Friend Sub FillToBaseline(dims As PlotDimensions, gfx As Graphics, xPxStart As Single, xPxEnd As Single, linePoints As PointF(), baselineY As Double)
            Dim baseline As Integer = CInt(dims.GetPixelY(baselineY))

            Dim first As New PointF(xPxStart, baseline)
            Dim last As New PointF(xPxEnd, baseline)

            Dim points As PointF() = New PointF() {first}.Concat(linePoints).Concat(New PointF() {last}).ToArray()
            Dim pointList As List(Of PointF) = points.ToList()

            Dim baselinePointStart As New PointF(linePoints(0).X, baseline)
            Dim baselinePointEnd As New PointF(linePoints(linePoints.Length - 1).X, baseline)

            Dim newlyAddedItems As Integer = 0
            For i As Integer = 1 To points.Length + newlyAddedItems - 1
                If (pointList(i - 1).Y > baseline AndAlso pointList(i).Y < baseline) _
                    OrElse (pointList(i - 1).Y < baseline AndAlso pointList(i).Y > baseline) Then

                    Dim intersection As PointF? = GetIntersection(pointList(i), pointList(i - 1), baselinePointStart, baselinePointEnd)
                    If (intersection IsNot Nothing) Then
                        pointList.Insert(i, intersection.Value)
                        newlyAddedItems += 1
                        i += 1 'TEST 
                    End If
                End If
            Next

            Dim dataAreaRect As New Rectangle(0, 0, CInt(dims.Width), CInt(dims.Height))

            'Above graph
            If (dataAreaRect.Height > 0) AndAlso (dataAreaRect.Width > 0) Then
                Dim color As Color = If(_GradientFillColor1, _FillColor1.Value)
                Dim edgeColor As Color = _FillColor1.Value
                Using brsh As New LinearGradientBrush(dataAreaRect, color, edgeColor, LinearGradientMode.Vertical)
                    gfx.FillPolygon(brsh, {first}.Concat(pointList.Where(Function(p As PointF) p.Y <= baseline).ToArray()) _
                                    .Concat({last}).ToArray())
                End Using
            End If

            'Below graph
            If dataAreaRect.Height > 0 AndAlso dataAreaRect.Width > 0 Then
                Dim color As Color = _FillColor2.Value
                Dim edgeColor As Color = If(_GradientFillColor2, _FillColor2.Value)
                Using brsh As New LinearGradientBrush(dataAreaRect, color, edgeColor, LinearGradientMode.Vertical)
                    gfx.FillPolygon(brsh, {first}.Concat(pointList.Where(Function(p As PointF) p.Y >= baseline).ToArray()) _
                                    .Concat({last}).ToArray())
                End Using
            End If

            'Draw baseline
            Using baselinePen As Pen = Drawing.GDI.Pen(BaselineColor, BaselineWidth)
                gfx.DrawLine(baselinePen, baselinePointStart, baselinePointEnd)
            End Using
        End Sub

        ''' <summary>
        ''' Render similar to high density mode except use multiple colors to represent density distributions.
        ''' </summary>
        Private Sub RenderHighDensityDistributionParallel(dims As PlotDimensions, gfx As Graphics, offsetPoints As Double, columnPointCount As Double)
            Dim xPxStart As Integer = CInt(Math.Ceiling((-1 - offsetPoints) / columnPointCount - 1))
            Dim xPxEnd As Integer = CInt(Math.Ceiling((_Ys.Length - offsetPoints) / columnPointCount))
            xPxStart = Math.Max(0, xPxStart)
            xPxEnd = Math.Min(CInt(dims.DataWidth), xPxEnd)
            If (xPxStart >= xPxEnd) Then
                Return
            End If

            Dim capacity As Integer = (xPxEnd - xPxStart) * 2 + 1
            Dim linePoints As New List(Of PointF)(capacity)

            Dim levelValues = Enumerable.Range(xPxStart, xPxEnd - xPxStart).AsParallel().AsOrdered() _
                .Select(Function(xPx As Integer) 'determine data indexes for this pixel column
                            Dim index1 As Integer = CInt(offsetPoints + columnPointCount * xPx)
                            Dim index2 As Integer = CInt(offsetPoints + columnPointCount * (xPx + 1))

                            If (index1 < 0) Then index1 = 0
                            If (index1 > _Ys.Length - 1) Then index1 = _Ys.Length - 1
                            If (index2 > _Ys.Length - 1) Then index2 = _Ys.Length - 1

                            Dim indexes = Enumerable.Range(0, DensityLevelCount + 1).Select(Function(x As Integer) x * (index2 - index1 - 1) / DensityLevelCount)
                            Dim levelsValues As T() = New ArraySegment(Of T)(_Ys, index1, index2 - index1).OrderBy(Function(x) x).Where(Function(y As T, i As Integer) indexes.Contains(i)).ToArray()
                            Return New Tuple(Of Integer, T())(xPx, levelsValues)
                        End Function).ToArray()

            Dim linePointsLevels As List(Of PointF()) = levelValues _
                .Select(Function(x)
                            Return x.Item2.Select(Function(y)
                                                      Return New PointF(x.Item1 + dims.DataOffsetX, dims.GetPixelY(AddYs(y, OffsetY)))
                                                  End Function).ToArray()
                        End Function).ToList() 'TEST !!

            For i As Integer = 0 To DensityLevelCount - 1
                linePoints.Clear()
                For j As Integer = 0 To linePointsLevels.Count - 1
                    If (i + 1 < linePointsLevels(j).Length) Then
                        linePoints.Add(linePointsLevels(j)(i))
                        linePoints.Add(linePointsLevels(j)(i + 1))
                    End If
                Next

                Dim pointsArray As PointF() = linePoints.ToArray()
                ValidatePoints(pointsArray)

                Using densityPen As Pen = Drawing.GDI.Pen(PenColorsByDensity(i))
                    gfx.DrawLines(densityPen, pointsArray)
                End Using

                Select Case _FillType
                    Case FillType.NoFill
                        'do nothing
                    Case FillType.FillAbove
                        FillToInfinity(dims, gfx, xPxStart, xPxEnd, pointsArray, True)
                    Case FillType.FillBelow
                        FillToInfinity(dims, gfx, xPxStart, xPxEnd, pointsArray, False)
                    Case FillType.FillAboveAndBelow
                        FillToBaseline(dims, gfx, xPxStart, xPxEnd, pointsArray, BaselineY)
                    Case Else
                        Throw New InvalidOperationException("Unsupported fill type.")
                End Select
            Next
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableSignalBase{lbl} with {PointCount} points ({GetType(T).Name})."
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = Color,
                .LineStyle = LineStyle,
                .LineWidth = LineWidth,
                .MarkerShape = If(ShowMarkersInLegend, MarkerShape, MarkerShape.None),
                .MarkerSize = If(ShowMarkersInLegend, MarkerSize, 0)}
            Return New LegendItem() {leg}
        End Function

        Public Overridable Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If (Ys.Length > 0) Then
                Using gfx = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                penLD = Drawing.GDI.Pen(LineColor, CSng(LineWidth), LineStyle, True),
                penHD = Drawing.GDI.Pen(LineColor, CSng(LineWidth), LineStyle.Solid, True)

                    Dim dataSpanUnits As Double = _Ys.Length * _SamplePeriod
                    Dim columnSpanUnits As Double = dims.XSpan / dims.DataWidth
                    Dim columnPointCount As Double = (columnSpanUnits / dataSpanUnits) * _Ys.Length
                    Dim offsetUnits As Double = dims.XMin - OffsetX
                    Dim offsetPoints As Double = offsetUnits / _SamplePeriod
                    Dim visibleIndex1 As Integer = CInt(offsetPoints)
                    Dim visibleIndex2 As Integer = CInt(offsetPoints + columnPointCount * (dims.DataWidth + 1))
                    Dim visiblePointCount As Integer = visibleIndex2 - visibleIndex1
                    Dim pointsPerPixelColumn As Double = visiblePointCount / dims.DataWidth
                    Dim dataWidthPx2 As Double = visibleIndex2 - visibleIndex1 + 2
                    Dim densityLevelsAvailable As Boolean = (DensityLevelCount > 0) AndAlso (pointsPerPixelColumn > DensityLevelCount)
                    Dim firstPointX As Double = dims.GetPixelX(OffsetX)
                    Dim lastPointX As Double = dims.GetPixelX(_SamplePeriod * (_Ys.Length - 1) + OffsetX)
                    Dim dataWidthPx As Double = lastPointX - firstPointX
                    Dim columnsWithData As Double = Math.Min(dataWidthPx, dataWidthPx2)

                    If (columnsWithData < 1) AndAlso (Ys.Length > 1) Then
                        RenderSingleLine(dims, gfx, penHD)
                    ElseIf (pointsPerPixelColumn > 1) AndAlso (Ys.Length > 1) Then
                        If (densityLevelsAvailable) Then
                            RenderHighDensityDistributionParallel(dims, gfx, offsetPoints, columnPointCount)
                        Else
                            RenderHighDensity(dims, gfx, offsetPoints, columnPointCount, penHD)
                        End If
                    Else
                        RenderLowDensity(dims, gfx, visibleIndex1, visibleIndex2, penLD)
                    End If
                End Using
            End If
        End Sub

        Protected Sub ValidatePoints(points As IEnumerable(Of PointF))
            For Each pointF As PointF In points
                If Single.IsNaN(pointF.Y) Then
                    Throw New InvalidOperationException("Data must not contain NaN.")
                End If
            Next
        End Sub

        Public Overridable Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            'check Y values
            If (Ys is Nothing) Then
                Throw New InvalidOperationException("Ys cannot be null.")
            End If
            If deep Then
                Validate.AssertAllReal(Of T)("Ys", Ys)
            End If


            ' check render indexes
            If (MinRenderIndex < 0 OrElse MinRenderIndex > MaxRenderIndex) Then
                Throw New IndexOutOfRangeException($"{NameOf(MinRenderIndex)} must be between 0 and maxRenderIndex")
            End If
            If ((MaxRenderIndex > Ys.Length - 1) OrElse MaxRenderIndex < 0) Then
                Throw New IndexOutOfRangeException($"{NameOf(MaxRenderIndex)} must be a valid index for ys[]")
            End If
            If (MaxRenderIndex > Ys.Length - 1) Then
                Throw New IndexOutOfRangeException($"{NameOf(MaxRenderIndex)} must be a valid index for ys[]")
            End If
            If (MinRenderIndex > MaxRenderIndex) Then
                Throw New IndexOutOfRangeException($"{NameOf(MinRenderIndex)} must be lower maxRenderIndex")
            End If

            'check misc styling options
            If (_FillColor1 is Nothing) AndAlso (_FillType <> FillType.NoFill) Then
                Throw New InvalidOperationException($"A Color must be assigned to {NameOf(_FillColor1)} to use fill type '{_FillType}'.")
            End If
            If (_FillColor1 is Nothing) AndAlso (_FillType = FillType.FillAboveAndBelow) Then
                Throw New InvalidOperationException($"A Color must be assigned to {NameOf(_FillColor2)} to use fill type '{_FillType}'.")
            End If
        End Sub

        ''' <summary>
        ''' Return the index for the data point corresponding to the given X coordinate.
        ''' </summary>
        Private Function GetIndexForX(x As Double) As Integer
            Dim index = CInt((x - OffsetX + SamplePeriod / 2) * SampleRate)
            index = Math.Max(index, MinRenderIndex)
            index = Math.Min(index, MaxRenderIndex)
            Return index
        End Function

        ''' <summary>
        ''' Return the X coordinate of the data point at the given index.
        ''' </summary>
        Private Function IndexToX(index As Integer) As Double
            Return index * SampleRate + OffsetX
        End Function

        ''' <summary>
        ''' Return the X/Y coordinates of the point nearest the X position.
        ''' </summary>
        ''' <param name="x">X position in plot space</param>
        Public Function GetPointNearestX(x As Double) As Tuple(Of Double, T, Integer) Implements ScottPlot.Plottable.IHasPointsGenericX(Of Double, T).GetPointNearestX
            Dim index As Integer = GetIndexForX(x)
            Dim ptX As Double = OffsetX + index * SamplePeriod
            Dim t As T = AddYsGeneric(Ys(index), OffsetY)
            Return New Tuple(Of Double, T, Integer)(ptX, t, index)
        End Function

        ''' <summary>
        ''' Configure the signal plot to only show the curve with no filled area above or below it.
        ''' </summary>
        Public Sub FillDisable()
            _FillType = FillType.FillBelow
            _GradientFillColor1 = Nothing
            _GradientFillColor2 = Nothing
        End Sub

        ''' <summary>
        ''' Show a solid color beneath the curve.
        ''' </summary>
        Public Sub FillBelow(Optional color As Color? = Nothing, Optional alpha As Double = 0.2)
            _FillType = FillType.FillBelow
            _FillColor1 = Drawing.GDI.Semitransparent(If(color, Me.Color), alpha)
        End Sub

        ''' <summary>
        ''' Show a two-color gradient beneath the curve.
        ''' </summary>
        Public Sub FillBelow(upperColor As Color, lowerColor As Color, Optional alpha As Double = 0.2)
            _FillType = FillType.FillBelow
            _FillColor1 = Drawing.GDI.Semitransparent(upperColor, alpha)
            _GradientFillColor1 = Drawing.GDI.Semitransparent(lowerColor, alpha)
        End Sub

        ''' <summary>
        ''' Show a solid color above the curve.
        ''' </summary>
        Public Sub FillAbove(Optional color As Color? = Nothing, Optional alpha As Double = 0.2)
            _FillType = FillType.FillAbove
            _FillColor1 = Drawing.GDI.Semitransparent(If(color, Me.Color), alpha)
        End Sub

        ''' <summary>
        ''' Show a two-color gradient above the curve.
        ''' </summary>
        Public Sub FillAbove(lowerColor As Color, upperColor As Color, Optional alpha As Double = 0.2)
            _FillType = FillType.FillAbove
            _FillColor1 = Drawing.GDI.Semitransparent(upperColor, alpha)
            _GradientFillColor1 = Drawing.GDI.Semitransparent(lowerColor, alpha)
        End Sub

        ''' <summary>
        ''' Fill the area between the curve and the <see cref="BaselineY"/> value.
        ''' </summary>
        Public Sub FillAboveAndBelow(colorAbove As Color, colorBelow As Color, Optional alpha As Double = 0.2)
            _FillType = FillType.FillAboveAndBelow
            _FillColor1 = Drawing.GDI.Semitransparent(colorAbove, alpha)
            _FillColor2 = Drawing.GDI.Semitransparent(colorBelow, alpha)
        End Sub

        ''' <summary>
        ''' Fill the area between the curve and the edge of the display area using two gradients.
        ''' </summary>
        ''' <param name="above1">Color above the line next to the curve.</param>
        ''' <param name="above2">Color above the line next to the upper edge of the plot area.</param>
        ''' <param name="below1">Color below the line next to the curve.</param>
        ''' <param name="below2">Color below the line next to the lower edge of the plot area.</param>
        ''' <param name="alpha">Apply this opacity to all colors.</param>
        Public Sub FillAboveAndBelow(above1 As Color, above2 As Color, below1 As Color, below2 As Color, Optional alpha As Double = 0.2)
            _FillType = FillType.FillAboveAndBelow
            _FillColor1 = Drawing.GDI.Semitransparent(above1, alpha)
            _FillColor2 = Drawing.GDI.Semitransparent(below2, alpha)
            _GradientFillColor1 = Drawing.GDI.Semitransparent(above2, alpha)
            _GradientFillColor2 = Drawing.GDI.Semitransparent(below1, alpha)
        End Sub

        ''' <summary>
        ''' Return the vertical limits of the data between horizontal positions (inclusive).
        ''' </summary>
        Public Function GetYDataRange(xMin As Double, xMax As Double) As Tuple(Of T, T) Implements ScottPlot.Plottable.IHasPointsGenericX(Of Double, T).GetYDataRange
            Dim startIndex As Integer = GetIndexForX(xMin)
            Dim endIndex As Integer = GetIndexForX(xMax)

            If (IndexToX(endIndex) < xMax) Then
                endIndex = Math.Min(endIndex + 1, MaxRenderIndex)
            End If

            Dim yMin As Double
            Dim yMax As Double
            Strategy.MinMaxRangeQuery(startIndex, endIndex, yMin, yMax)

            Dim genericYMin As T = Nothing
            Dim genericYMax As T = Nothing
            NumericConversion.DoubleToGeneric(Of T)(yMin, genericYMin)
            NumericConversion.DoubleToGeneric(Of T)(yMax, genericYMax)

            Return New Tuple(Of T, T)(genericYMin, genericYMax)
        End Function

#End Region '/METHODS

#Region "NESTED TYPES"

        Private Class IntervalMinMax

            Public X As Single
            Public Min As Single
            Public Max As Single

            Public Sub New(x As Single, Min As Single, Max As Single)
                Me.X = x
                Me.Min = Min
                Me.Max = Max
            End Sub

            Public Iterator Function GetPoints() As IEnumerable(Of PointF)
                Yield New PointF(X, Min)
                Yield New PointF(X, Max)
                Return
            End Function
        End Class

#End Region '/NESTED TYPES

    End Class

End Namespace