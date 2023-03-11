Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' The scatter plot renders X/Y pairs as points and/or connected lines.
    ''' Scatter plots can be extremely slow for large datasets, so use Signal plots in these situations.
    ''' </summary>
    Public Class ScatterPlot
        Implements IPlottable, IHasPoints, IHasPointsGeneric(Of Double, Double), IHasPointsGenericX(Of Double, Double), IHasPointsGenericY(Of Double, Double), IHasLine, IHasMarker, IHighlightable, IHasColor

#Region "PROPS, FIELDS"

        'data
        Public ReadOnly Property Xs As Double()
        Public ReadOnly Property Ys As Double()
        Public Property XError As Double()
        Public Property YError As Double()
        Public Property DataPointLabels As String()
        Public Property DataPointLabelFont As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' Add this value to each X value before plotting (axis units).
        ''' </summary>
        Public Property OffsetX As Double = 0

        ''' <summary>
        ''' Add this value to each Y value before plotting (axis units).
        ''' </summary>
        Public Property OffsetY As Double = 0

        Public ReadOnly Property PointCount As Integer
            Get
                Return Ys.Length
            End Get
        End Property

        ''' <summary>
        ''' If enabled, points will be connected by smooth lines instead of straight diagnal lines.
        ''' <see cref="SmoothTension"/> adjusts the smoothnes of the lines.
        ''' </summary>
        Public Smooth As Boolean = False

        ''' <summary>
        ''' Tension to use for smoothing when <see cref="Smooth"/> is enabled.
        ''' </summary>
        Public SmoothTension As Double = 0.5

        ''' <summary>
        ''' Defines behavior when <see cref="Xs"/> or <see cref="Ys"/> contains <see cref="Double.NaN"/>.
        ''' </summary>
        Public OnNaN As ScatterPlot.NanBehavior = NanBehavior.Throw

        <Obsolete("Scatter plot arrowheads have been deprecated. Use the Arrow plot type instead.", True)>
        Public ArrowheadWidth As Single

        <Obsolete("Scatter plot arrowheads have been deprecated. Use the Arrow plot type instead.", True)>
        Public ArrowheadLength As Single

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        Public Property Label As String = String.Empty

        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return LineColor
            End Get
            Set(value As Color)
                LineColor = value
                MarkerColor = value
            End Set
        End Property

        Public Property LineColor As Color = Color.Black Implements ScottPlot.Plottable.IHasLine.LineColor
        Public Property MarkerColor As Color = Color.Black Implements ScottPlot.Plottable.IHasMarker.MarkerColor
        Public Property LineStyle As LineStyle = LineStyle.Solid Implements ScottPlot.Plottable.IHasLine.LineStyle
        Public Property MarkerShape As MarkerShape = MarkerShape.FilledCircle Implements ScottPlot.Plottable.IHasMarker.MarkerShape

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

        Public Property ErrorLineWidth As Double
            Get
                If IsHighlighted Then
                    Return _ErrorLineWidth * HighlightCoefficient
                End If
                Return _ErrorLineWidth
            End Get
            Set(value As Double)
                _ErrorLineWidth = value
            End Set
        End Property
        Private _ErrorLineWidth As Double = 1

        Public ErrorCapSize As Single = 3

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
                    Return CSng(_LineWidth * HighlightCoefficient)
                End If
                Return _MarkerLineWidth
            End Get
            Set(value As Single)
                _MarkerLineWidth = value
            End Set
        End Property
        Private _MarkerLineWidth As Single = 1

        ''' <summary>
        ''' If enabled, scatter plot points will be connected by square corners rather than straight diagnal lines.
        ''' </summary>
        Public Property StepDisplay As Boolean = False

        ''' <summary>
        ''' Describes orientation of steps if <see cref="StepDisplay"/> is enabled.
        ''' If true, lines will extend to the right before ascending or descending to the level of the following point.
        ''' </summary>
        ''' <returns></returns>
        Public Property StepDisplayRight As Boolean = True

        Public Property IsHighlighted As Boolean = False Implements ScottPlot.Plottable.IHighlightable.IsHighlighted
        Public Property HighlightCoefficient As Single = 2 Implements ScottPlot.Plottable.IHighlightable.HighlightCoefficient

        <Obsolete("Scatter plot arrowheads have been deprecated. Use the Arrow plot type instead.", True)>
        Public ReadOnly Property IsArrow As Boolean
            Get
                Return (ArrowheadWidth > 0) AndAlso (ArrowheadLength > 0)
            End Get
        End Property

        Public Property MinRenderIndex As Integer?
        Public Property MaxRenderIndex As Integer?

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New()
        End Sub

        Public Sub New(xs As Double(), ys As Double(), Optional errorX As Double() = Nothing, Optional errorY As Double() = Nothing)
            Me.Xs = xs
            Me.Ys = ys
            Me.XError = errorX
            Me.YError = errorY
        End Sub

#End Region '/CTOR

#Region "METHODS"

        ''' <summary>
        ''' Replace the Xs array with a new one.
        ''' </summary>
        Public Sub UpdateX(xs As Double())
            If (xs is Nothing) Then
                Throw New ArgumentException("Xs must not be null.")
            End If
            If (xs.Length <> Ys.Length) Then
                Throw New ArgumentException("Xs and Ys must have the same length.")
            End If
            _Xs = xs
        End Sub

        ''' <summary>
        ''' Replace the Ys array with a new one.
        ''' </summary>
        Public Sub UpdateY(ys As Double())
            If (ys is Nothing) Then
                Throw New ArgumentException("Ys must not be null.")
            End If
            If (Xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must have the same length")
            End If
            _Ys = ys
        End Sub

        ''' <summary>
        ''' Replace Xs and Ys arrays with new ones.
        ''' </summary>
        Public Sub Update(xs As Double(), ys As Double())
            If (xs is Nothing) Then
                Throw New ArgumentException("Xs must not be null.")
            End If
            If (ys is Nothing) Then
                Throw New ArgumentException("Ys must not be null.")
            End If
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must have the same length.")
            End If
            _Xs = xs
            _Ys = ys
        End Sub

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            Validate.AssertHasElements("xs", Xs)
            Validate.AssertHasElements("ys", Ys)
            Validate.AssertEqualLength(Of Double, Double)("xs and ys", Xs, Ys)

            If (MaxRenderIndex IsNot Nothing) Then
                If (MaxRenderIndex > Ys.Length - 1) OrElse (MaxRenderIndex < 0) Then
                    Throw New IndexOutOfRangeException($"{NameOf(MaxRenderIndex)} must be a valid index for ys[].")
                End If
            End If

            If (MinRenderIndex IsNot Nothing) Then
                If (MinRenderIndex < 0) Then
                    Throw New IndexOutOfRangeException($"{NameOf(MinRenderIndex)} must be between 0 and {MaxRenderIndex}.")
                End If
                If (MaxRenderIndex IsNot Nothing) AndAlso (MinRenderIndex > MaxRenderIndex) Then
                    Throw New IndexOutOfRangeException($"{NameOf(MinRenderIndex)} must be between 0 and {MaxRenderIndex}.")
                End If
            End If

            If (XError IsNot Nothing) Then
                Validate.AssertHasElements("ErrorX", Xs)
                Validate.AssertEqualLength(Of Double, Double)("Xs and errorX", Xs, XError)
            End If

            If (YError IsNot Nothing) Then
                Validate.AssertHasElements("ErrorY", Ys)
                Validate.AssertEqualLength(Of Double, Double)("Ys and errorY", Ys, YError)
            End If

            If deep Then
                Validate.AssertAllReal("Xs", Xs)
                Validate.AssertAllReal("Ys", Ys)

                If (XError IsNot Nothing) Then
                    Validate.AssertAllReal("ErrorX", XError)
                End If
                If (YError IsNot Nothing) Then
                    Validate.AssertAllReal("ErrorY", YError)
                End If
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableScatter{lbl} with {PointCount} points"
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Select Case OnNaN
                Case ScatterPlot.NanBehavior.Throw
                    Return GetAxisLimitsThrowOnNaN()
                Case ScatterPlot.NanBehavior.Ignore
                    Return GetAxisLimitsIgnoreNaN()
                Case ScatterPlot.NanBehavior.Gap
                    Return GetAxisLimitsIgnoreNaN()
                Case Else
                    Throw New NotImplementedException($"{NameOf(OnNaN)} behavior not yet supported: {OnNaN}.")
            End Select
        End Function

        Private Function GetAxisLimitsIgnoreNaN() As AxisLimits
            ValidateData(False)
            Dim from As Integer = MinRenderIndex.GetValueOrDefault()
            Dim [to] As Integer = If(MaxRenderIndex, Xs.Length - 1)

            'TODO: don't use an array for this
            Dim limits As Double() = New Double() {Double.NaN, Double.NaN, Double.NaN, Double.NaN}

            If (XError is Nothing) Then
                Dim xsRange = Xs.Skip(from).Take([to] - from + 1).Where(Function(x) Not Double.IsNaN(x))
                If xsRange.Any() Then
                    limits(0) = xsRange.Min()
                    limits(1) = xsRange.Max()
                End If
            Else
                Dim xsAndError = Xs.Zip(XError, Function(x, e) New Tuple(Of Double, Double)(x, e)) _
                    .Skip(from) _
                    .Take([to] - from + 1) _
                    .Where(Function(p) Not Double.IsNaN(p.Item1 + p.Item2))
                If (xsAndError.Any()) Then
                    limits(0) = xsAndError.Min(Function(p As Tuple(Of Double, Double)) p.Item1 - p.Item2)
                    limits(1) = xsAndError.Max(Function(p As Tuple(Of Double, Double)) p.Item1 + p.Item2)
                End If
            End If

            If (YError is Nothing) Then
                Dim ysRange = Ys.Skip(from).Take([to] - from + 1).Where(Function(x) Not Double.IsNaN(x))
                If (ysRange.Any()) Then
                    limits(2) = ysRange.Min()
                    limits(3) = ysRange.Max()
                End If
            Else
                Dim ysAndError = Ys.Zip(YError, Function(y, e) New Tuple(Of Double, Double)(y, e)) _
                    .Skip(from) _
                    .Take([to] - from + 1) _
                    .Where(Function(p) Not Double.IsNaN(p.Item1 + p.Item2))
                If ysAndError.Any() Then
                    limits(2) = ysAndError.Min(Function(p) p.Item1 - p.Item2)
                    limits(3) = ysAndError.Max(Function(p) p.Item1 + p.Item2)
                End If
            End If

            If Double.IsInfinity(limits(0)) OrElse Double.IsInfinity(limits(1)) Then
                Throw New InvalidOperationException("X data must not contain Infinity.")
            End If
            If Double.IsInfinity(limits(2)) OrElse Double.IsInfinity(limits(3)) Then
                Throw New InvalidOperationException("Y data must not contain Infinity.")
            End If

            Return New AxisLimits(limits(0) + OffsetX,
                                  limits(1) + OffsetX,
                                  limits(2) + OffsetY,
                                  limits(3) + OffsetY)
        End Function

        Private Function GetAxisLimitsThrowOnNaN() As AxisLimits
            Me.ValidateData(False)
            Dim from As Integer = Me.MinRenderIndex.GetValueOrDefault()
            Dim [to] As Integer = If(Me.MaxRenderIndex, (Me.Xs.Length - 1))

            'TODO: don't use an array for this
            Dim limits As Double() = New Double(3) {}

            If (XError is Nothing) Then
                Dim xsRange As IEnumerable(Of Double) = Xs.Skip(from).Take([to] - from + 1)
                limits(0) = xsRange.Min()
                limits(1) = xsRange.Max()
            Else
                Dim xsAndError = Xs.Zip(XError, Function(x, e) New Tuple(Of Double, Double)(x, e)).Skip(from).Take([to] - from + 1)
                limits(0) = xsAndError.Min(Function(p) p.Item1 - p.Item2)
                limits(1) = xsAndError.Max(Function(p) p.Item1 + p.Item2)
            End If

            If (YError is Nothing) Then
                Dim source4 As IEnumerable(Of Double) = Ys.Skip(from).Take([to] - from + 1)
                limits(2) = source4.Min()
                limits(3) = source4.Max()
            Else
                Dim ysAndError = Ys.Zip(YError, Function(y, e) New Tuple(Of Double, Double)(y, e)).Skip(from).Take([to] - from + 1)
                limits(2) = ysAndError.Min(Function(p) p.Item1 - p.Item2)
                limits(3) = ysAndError.Max(Function(p) p.Item1 + p.Item2)
            End If

            If Double.IsNaN(limits(0)) OrElse Double.IsNaN(limits(1)) Then
                Throw New InvalidOperationException("X data must not contain NaN.")
            End If
            If Double.IsNaN(limits(2)) OrElse Double.IsNaN(limits(3)) Then
                Throw New InvalidOperationException("Y data must not contain NaN.")
            End If

            If Double.IsInfinity(limits(0)) OrElse Double.IsInfinity(limits(1)) Then
                Throw New InvalidOperationException("X data must not contain Infinity.")
            End If
            If Double.IsInfinity(limits(2)) OrElse Double.IsInfinity(limits(3)) Then
                Throw New InvalidOperationException("Y data must not contain Infinity.")
            End If

            Return New AxisLimits(
                limits(0) + OffsetX,
                limits(1) + OffsetX,
                limits(2) + OffsetY,
                limits(3) + OffsetY)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                    penLine As Pen = Drawing.GDI.Pen(LineColor, LineWidth, LineStyle, True),
                    penLineError As Pen = Drawing.GDI.Pen(LineColor, ErrorLineWidth, LineStyle.Solid, True)

                    Dim from As Integer = MinRenderIndex.GetValueOrDefault()
                    Dim [to] As Integer = If(MaxRenderIndex, (Xs.Length - 1))
                    Dim len As Integer = [to] - from + 1
                    If (len > 0) Then
                        Dim points As PointF() = New PointF(len - 1) {}

                        For i As Integer = from To [to]
                            Dim x As Single = dims.GetPixelX(Xs(i) + OffsetX)
                            Dim y As Single = dims.GetPixelY(Ys(i) + OffsetY)
                            points(i - from) = New PointF(x, y)
                        Next

                        If (YError IsNot Nothing) Then
                            For i As Integer = 0 To points.Count() - 1
                                Dim yWithOffset As Double = Ys(i) + OffsetY
                                Dim yBot As Single = dims.GetPixelY(yWithOffset - YError(i + from))
                                Dim yTop As Single = dims.GetPixelY(yWithOffset + YError(i + from))
                                gfx.DrawLine(penLineError, points(i).X, yBot, points(i).X, yTop)
                                gfx.DrawLine(penLineError, points(i).X - ErrorCapSize, yBot, points(i).X + ErrorCapSize, yBot)
                                gfx.DrawLine(penLineError, points(i).X - ErrorCapSize, yTop, points(i).X + ErrorCapSize, yTop)
                            Next
                        End If

                        If (XError IsNot Nothing) Then
                            For k As Integer = 0 To points.Length - 1
                                Dim xWithOffset As Double = Xs(k) + OffsetX
                                Dim xLeft As Single = dims.GetPixelX(xWithOffset - XError(k + from))
                                Dim xRight As Single = dims.GetPixelX(xWithOffset + XError(k + from))
                                gfx.DrawLine(penLineError, xLeft, points(k).Y, xRight, points(k).Y)
                                gfx.DrawLine(penLineError, xLeft, points(k).Y - ErrorCapSize, xLeft, points(k).Y + ErrorCapSize)
                                gfx.DrawLine(penLineError, xRight, points(k).Y - ErrorCapSize, xRight, points(k).Y + ErrorCapSize)
                            Next
                        End If

                        If (OnNaN = ScatterPlot.NanBehavior.Throw) Then
                            For Each pointF As PointF In points
                                If (Single.IsNaN(pointF.X) OrElse Single.IsNaN(pointF.Y)) Then
                                    Throw New NotImplementedException($"Data must not contain NaN if {NameOf(OnNaN)} is {OnNaN}.")
                                End If
                            Next
                            DrawLines(points, gfx, penLine)
                        ElseIf OnNaN = ScatterPlot.NanBehavior.Ignore Then
                            DrawLinesIngoringNaN(points, gfx, penLine)
                        ElseIf OnNaN = ScatterPlot.NanBehavior.Gap Then
                            DrawLinesWithGaps(points, gfx, penLine)
                        End If

                        If (DataPointLabels IsNot Nothing) Then
                            For m As Integer = 0 To DataPointLabels.Length - 1
                                Dim text As String = DataPointLabels(m)
                                If (Not String.IsNullOrEmpty(text)) Then
                                    gfx.TranslateTransform(points(m).X, points(m).Y)
                                    gfx.RotateTransform(DataPointLabelFont.Rotation)

                                    Dim t As Tuple(Of Single, Single) = Drawing.GDI.TranslateString(gfx, text, DataPointLabelFont)
                                    Dim dx As Single = t.Item1
                                    Dim dy As Single = t.Item2
                                    gfx.TranslateTransform(-dx, -dy)

                                    Using fnt As System.Drawing.Font = Drawing.GDI.Font(DataPointLabelFont),
                                    fontBrush As New SolidBrush(DataPointLabelFont.Color)
                                        gfx.DrawString(text, fnt, fontBrush, New PointF(0, 0))
                                        Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
                                    End Using
                                End If
                            Next
                        End If

                        'draw a marker at each point
                        If (MarkerSize > 0) AndAlso (MarkerShape <> MarkerShape.None) Then
                            MarkerTools.DrawMarkers(gfx, points, MarkerShape, MarkerSize, MarkerColor, MarkerLineWidth)
                        End If
                    Else
                        Diagnostics.Debug.WriteLine("len=0")
                    End If
                End Using
            End If
        End Sub

        Private Sub DrawLines(points As PointF(), gfx As Graphics, penLine As Pen)
            Dim isLineVisible As Boolean = (LineWidth > 0) AndAlso (points.Length > 1) AndAlso (LineStyle <> LineStyle.None)
            If isLineVisible Then
                If StepDisplay Then
                    Dim pointsStep As PointF() = ScatterPlot.GetStepDisplayPoints(points, StepDisplayRight)
                    gfx.DrawLines(penLine, pointsStep)
                ElseIf Smooth Then
                    gfx.DrawCurve(penLine, points, CSng(SmoothTension))
                Else
                    gfx.DrawLines(penLine, points)
                End If
            End If
        End Sub

        Private Sub DrawLinesIngoringNaN(points As PointF(), gfx As Graphics, penLine As Pen)
            Dim pointsWithoutNaNs As PointF() = points.Where(Function(pt) Not Double.IsNaN(pt.X) AndAlso Not Double.IsNaN(pt.Y)).ToArray()
            DrawLines(pointsWithoutNaNs, gfx, penLine)
        End Sub

        Private Sub DrawLinesWithGaps(points As PointF(), gfx As Graphics, penLine As Pen)
            Dim segment As New List(Of PointF)()
            For i As Integer = 0 To points.Length - 1
                If Double.IsNaN(points(i).X) OrElse Double.IsNaN(points(i).Y) Then
                    If segment.Any() Then
                        DrawLines(segment.ToArray(), gfx, penLine)
                        segment.Clear()
                    End If
                Else
                    segment.Add(points(i))
                End If
            Next
            If segment.Any() Then
                DrawLines(segment.ToArray(), gfx, penLine)
            End If
        End Sub

        ''' <summary>
        ''' Convert scatter plot points (connected by diagnal lines) to step plot points (connected by right angles)
        ''' by inserting an extra point between each of the original data points to result in L-shaped steps.
        ''' </summary>
        ''' <param name="points">Array of corner positions.</param>
        ''' <param name="right">Indicates that a line will extend to the right before rising or falling.</param>
        Public Shared Function GetStepDisplayPoints(points As PointF(), right As Boolean) As PointF()
            Dim pointsStep As PointF() = New PointF(points.Length * 2 - 1 - 1) {}
            Dim offsetX As Integer = If(right, 1, 0)
            Dim offsetY As Integer = If(right, 0, 1)
            For i As Integer = 0 To points.Length - 1 - 1
                pointsStep(i * 2) = points(i)
                pointsStep(i * 2 + 1) = New PointF(points(i + offsetX).X, points(i + offsetY).Y)
            Next
            pointsStep(pointsStep.Length - 1) = points(points.Length - 1)
            Return pointsStep
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = LineColor,
                .LineStyle = LineStyle,
                .LineWidth = LineWidth,
                .MarkerShape = MarkerShape,
                .MarkerSize = MarkerSize}
            Return New LegendItem() {leg}
        End Function

        ''' <summary>
        ''' Return the X/Y coordinates of the point nearest the X position.
        ''' </summary>
        ''' <param name="x">X position in plot space.</param>
        Public Function GetPointNearestX(x As Double) As Tuple(Of Double, Double, Integer) Implements ScottPlot.Plottable.IHasPointsGenericX(Of Double, Double).GetPointNearestX
            Dim from As Integer = Me.MinRenderIndex.GetValueOrDefault()
            Dim [to] As Integer = If(Me.MaxRenderIndex, (Me.Xs.Length - 1))
            Dim minDistance As Double = Double.PositiveInfinity
            Dim minIndex As Integer = 0
            For i As Integer = from To [to]
                Dim currDistance As Double = Math.Abs(Xs(i) - x)
                If (currDistance < minDistance) Then
                    minIndex = i
                    minDistance = currDistance
                End If
            Next
            Return New Tuple(Of Double, Double, Integer)(Xs(minIndex), Ys(minIndex), minIndex)
        End Function

        ''' <summary>
        ''' Return the X/Y coordinates of the point nearest the Y position.
        ''' </summary>
        ''' <param name="y">Y position in plot space.</param>
        Public Function GetPointNearestY(y As Double) As Tuple(Of Double, Double, Integer) Implements ScottPlot.Plottable.IHasPointsGenericY(Of Double, Double).GetPointNearestY
            Dim from As Integer = Me.MinRenderIndex.GetValueOrDefault()
            Dim [to] As Integer = If(Me.MaxRenderIndex, (Me.Ys.Length - 1))
            Dim minDistance As Double = Double.PositiveInfinity
            Dim minIndex As Integer = 0
            For i As Integer = from To [to]
                Dim currDistance As Double = Math.Abs(Ys(i) - y)
                If (currDistance < minDistance) Then
                    minIndex = i
                    minDistance = currDistance
                End If
            Next
            Return New Tuple(Of Double, Double, Integer)(Xs(minIndex), Ys(minIndex), minIndex)
        End Function

        ''' <summary>
        ''' Return the position and index of the data point nearest the given coordinate.
        ''' </summary>
        ''' <param name="x">Location in coordinate space.</param>
        ''' <param name="y">Location in coordinate space.</param>
        ''' <param name="xyRatio">Ratio of pixels per unit (X/Y) when rendered. Default 1.</param>
        Public Function GetPointNearest(x As Double, y As Double, xyRatio As Double) As Tuple(Of Double, Double, Integer) Implements ScottPlot.Plottable.IHasPointsGeneric(Of Double, Double).GetPointNearest
            Dim from As Integer = If(MinRenderIndex IsNot Nothing, MinRenderIndex.Value, 0)
            Dim [to] As Integer = If(MaxRenderIndex IsNot Nothing, MaxRenderIndex.Value, Ys.Length - 1)

            Dim points = Xs.Zip(Ys, Function(first, second) New Tuple(Of Double, Double)(first, second)) _
                .Skip(from).Take([to] - from + 1).ToList()

            If (xyRatio = 0) Then
                Throw New ArgumentException($"{NameOf(xyRatio)} must be not zero.")
            End If

            Dim xyRatioSquared As Double = xyRatio * xyRatio
            Dim pointDistanceSquared = Function(x1 As Double, y1 As Double)
                                           Return (x1 - x) * (x1 - x) * xyRatioSquared + (y1 - y) * (y1 - y)
                                       End Function

            Dim minDistance As Double = Double.PositiveInfinity
            Dim minIndex As Integer = 0

            For i As Integer = 0 To points.Count - 1
                If (Double.IsNaN(points(i).Item1) OrElse Double.IsNaN(points(i).Item2)) Then
                    Continue For
                End If
                Dim currDistance As Double = pointDistanceSquared(points(i).Item1, points(i).Item2)
                If (currDistance < minDistance) Then
                    minIndex = i
                    minDistance = currDistance
                End If
            Next

            Return New Tuple(Of Double, Double, Integer)(Xs(minIndex), Ys(minIndex), minIndex)
        End Function

        ''' <summary>
        ''' Return the vertical limits of the data between horizontal positions (inclusive).
        ''' </summary>
        Public Function GetYDataRange(xMin As Double, xMax As Double) As Tuple(Of Double, Double) Implements ScottPlot.Plottable.IHasPointsGenericX(Of Double, Double).GetYDataRange
            Dim includedYs As IEnumerable(Of Double) = Ys.Where(Function(y As Double, i As Integer)
                                                                    Return (Xs(i) >= xMin) AndAlso (Xs(i) <= xMax)
                                                                End Function)
            Return New Tuple(Of Double, Double)(includedYs.Min(), includedYs.Max())
        End Function

#End Region '/METHODS

#Region "NESTED TYPES"

        Public Enum NanBehavior
            ''' <summary>
            ''' Throw a <see cref="NotImplementedException"/> if <see cref="Xs"/> or <see cref="Ys"/> contains <see cref="Double.NaN"/>.
            ''' </summary>
            [Throw]
            ''' <summary>
            ''' Ignore points where X or Y is <see cref="Double.NaN"/>, drawing a line between adjacent non-NaN points.
            ''' </summary>
            Ignore
            ''' <summary>
            ''' Treat points where X or Y is <see cref="Double.NaN"/> as missing data and render the scatter plot as a broken line with gaps indicating NaN points.
            ''' </summary>
            Gap
        End Enum

#End Region '/NESTED TYPES

    End Class

End Namespace