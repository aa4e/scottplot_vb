Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot

    ''' <summary>
    ''' This module holds state for figure dimensions, axis limits, plot contents, and styling options.
    ''' A plot can be duplicated by copying the full state of this settings module.
    ''' </summary>
    Public Class Settings

#Region "CTOR"

        Public Sub New()
            AddHandler Plottables.CollectionChanged, Sub(sender As Object, e As NotifyCollectionChangedEventArgs)
                                                         _PlottablesIdentifier += 1
                                                     End Sub
        End Sub

#End Region '/CTOR

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' This List contains all plottables managed by this Plot. Render order is from lowest (first) to highest (last).
        ''' </summary>
        Public ReadOnly Plottables As New ObservableCollection(Of Plottable.IPlottable)()

        Public ReadOnly Property MouseDownX As Single = 0
        Public ReadOnly Property MouseDownY As Single = 0

        Public ReadOnly FigureBackground As New Renderable.FigureBackground()
        Public ReadOnly DataBackground As New Renderable.DataBackground()
        Public ReadOnly BenchmarkMessage As New Renderable.BenchmarkMessage()
        Public ReadOnly ErrorMessage As New Renderable.ErrorMessage()
        Public ReadOnly CornerLegend As New Renderable.Legend()
        Public ReadOnly ZoomRectangle As New Renderable.ZoomRectangle()
        Public PlottablePalette As IPalette = Palette.Category10

        ''' <summary>
        ''' Unique value that changes any time the list of plottables is modified.
        ''' </summary>
        Public ReadOnly Property PlottablesIdentifier As Integer = 0

        ''' <summary>
        ''' List of all axes used in this plot. Axes can be added, but existing ones should not be removed.
        ''' </summary>
        Public Axes As New List(Of Renderable.Axis)() From {
            New Renderable.DefaultLeftAxis(),
            New Renderable.DefaultRightAxis(),
            New Renderable.DefaultBottomAxis(),
            New Renderable.DefaultTopAxis()
        }

        ''' <summary>
        ''' Default padding to use when AxisAuto() or Margins() is called without a specified margin.
        ''' </summary>
        Public MarginsX As Double = 0.05

        ''' <summary>
        ''' Default padding to use when AxisAuto() or Margins() is called without a specified margin.
        ''' </summary>
        Public MarginsY As Double = 0.1

        ''' <summary>
        ''' Controls whether OferflowException is ignored in the Render() method.
        ''' This exception is commonly thrown by System.Drawing when drawing to extremely large pixel locations.
        ''' </summary>
        Public IgnoreOverflowExceptionsDuringRender As Boolean = True

        ''' <summary>
        ''' Get an array containing just horizontal axes.
        ''' </summary>
        Public ReadOnly Property HorizontalAxes As Renderable.Axis()
            Get
                Return Axes.Where(Function(x) x.IsHorizontal).Distinct().ToArray()
            End Get
        End Property

        ''' <summary>
        ''' Get an array containing just vertical axes.
        ''' </summary>
        Public ReadOnly Property VerticalAxes As Renderable.Axis()
            Get
                Return Axes.Where(Function(x) x.IsVertical).Distinct().ToArray()
            End Get
        End Property

        ''' <summary>
        ''' Indicates whether unset axes are present.
        ''' If true, the user may want to call <see cref="ScottPlot.Plot.AxisAuto"/> or <see cref="ScottPlot.Plot.SetAxisLimits"/>.
        ''' </summary>
        Public ReadOnly Property AllAxesHaveBeenSet As Boolean
            Get
                Return Axes.All(Function(x) x.Dims.HasBeenSet)
            End Get
        End Property

        ''' <summary>
        ''' Controls relationship between X and Y axis scales.
        ''' See documentation for enumeration members.
        ''' </summary>
        Public Property EqualScaleMode As EqualScaleMode = EqualScaleMode.Disabled

        ''' <summary>
        ''' Determines whether the grid lines should be drawn above the plottables.
        ''' </summary>
        Public Property DrawGridAbovePlottables As Boolean = False

        ''' <summary>
        ''' If defined, the data area will use this rectangle and not be adjusted depending on axis labels or ticks.
        ''' </summary>
        Public Property ManualDataPadding As PixelPadding? = Nothing

        ''' <summary>
        ''' Primary vertical axis (on the left of the plot).
        ''' </summary>
        Public ReadOnly Property YAxis As Renderable.Axis
            Get
                Return Axes(0)
            End Get
        End Property

        ''' <summary>
        ''' Secondary vertical axis (on the right of the plot).
        ''' </summary>
        Public ReadOnly Property YAxis2 As Renderable.Axis
            Get
                Return Axes(1)
            End Get
        End Property

        ''' <summary>
        ''' Primary horizontal axis (on the bottom of the plot).
        ''' </summary>
        Public ReadOnly Property XAxis As Renderable.Axis
            Get
                Return Axes(2)
            End Get
        End Property

        ''' <summary>
        ''' Secondary horizontal axis (on the top of the plot).
        ''' </summary>
        Public ReadOnly Property XAxis2 As Renderable.Axis
            Get
                Return Axes(3)
            End Get
        End Property

        ''' <summary>
        ''' Width of the figure (in pixels).
        ''' </summary>
        Public ReadOnly Property Width As Integer
            Get
                Return CInt(XAxis.Dims.FigureSizePx)
            End Get
        End Property

        ''' <summary>
        ''' Height of the figure (in pixels).
        ''' </summary>
        Public ReadOnly Property Height As Integer
            Get
                Return CInt(YAxis.Dims.FigureSizePx)
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "METHODS"

        ''' <summary>
        ''' Return the next color from PlottablePalette based on the current number of plottables.
        ''' </summary>
        Public Function GetNextColor() As Color
            Return PlottablePalette.GetColor(Me.Plottables.Count)
        End Function

        ''' <summary>
        ''' Return the first horizontal axis with the given axis index.
        ''' </summary>
        Public Function GetXAxis(xAxisIndex As Integer) As Renderable.Axis
            Dim array As Renderable.Axis() = Axes.Where(Function(x) x.IsHorizontal AndAlso (x.AxisIndex = xAxisIndex)).ToArray()
            If (array.Length = 0) Then
                Throw New InvalidOperationException($"There no X axes with an axis index of {xAxisIndex}.")
            End If
            Return array(0)
        End Function

        ''' <summary>
        ''' Return the first vertical axis with the given axis index.
        ''' </summary>
        Public Function GetYAxis(yAxisIndex As Integer) As Renderable.Axis
            Dim array As Renderable.Axis() = Axes.Where(Function(x) x.IsVertical AndAlso (x.AxisIndex = yAxisIndex)).ToArray()
            If (array.Length = 0) Then
                Throw New InvalidOperationException($"There no Y axes with an axis index of {yAxisIndex}.")
            End If
            Return array(0)
        End Function

        ''' <summary>
        ''' Return figure dimensions for the specified X and Y axes.
        ''' </summary>
        Public Function GetPlotDimensions(xAxisIndex As Integer, yAxisIndex As Integer, scaleFactor As Double) As PlotDimensions
            Dim xAxis = GetXAxis(xAxisIndex)
            Dim yAxis = GetYAxis(yAxisIndex)

            'Determine figure dimensions based on primary X and Y axis
            Dim figureSize As New SizeF(xAxis.Dims.FigureSizePx, yAxis.Dims.FigureSizePx)
            Dim dataSize As New SizeF(xAxis.Dims.DataSizePx, yAxis.Dims.DataSizePx)
            Dim dataOffset As New PointF(xAxis.Dims.DataOffsetPx, yAxis.Dims.DataOffsetPx)

            'Manual override if manual padding is enabled
            If (ManualDataPadding IsNot Nothing) Then
                dataOffset = New PointF(ManualDataPadding.Value.Left, ManualDataPadding.Value.Top)
                dataSize = New SizeF(figureSize.Width - ManualDataPadding.Value.Left - ManualDataPadding.Value.Right,
                                     figureSize.Height - ManualDataPadding.Value.Top - ManualDataPadding.Value.Bottom)
            End If

            'Determine axis limits based on specific X and Y axes:
            Dim xs = xAxis.Dims.RationalLimits()
            Dim ys = yAxis.Dims.RationalLimits()
            Dim xMin As Double = xs.Item1
            Dim xMax As Double = xs.Item2
            Dim yMin As Double = ys.Item1
            Dim yMax As Double = ys.Item2
            Dim limits As New AxisLimits(xMin, xMax, yMin, yMax)

            Return New PlotDimensions(figureSize, dataSize, dataOffset, limits, scaleFactor)
        End Function

        ''' <summary>
        ''' Set the default size for rendering images.
        ''' </summary>
        Public Sub Resize(width As Single, height As Single)
            For Each axis As Renderable.Axis In Axes
                axis.Dims.Resize(If(axis.IsHorizontal, width, height))
            Next
        End Sub

        ''' <summary>
        ''' Reset axis limits to their defauts.
        ''' </summary>
        Public Sub ResetAxisLimits()
            For Each axis As Renderable.Axis In Axes
                axis.Dims.ResetLimits()
            Next
        End Sub

        ''' <summary>
        ''' Define axis limits for a particuar axis.
        ''' </summary>
        Public Sub AxisSet(xMin As Double?, xMax As Double?, yMin As Double?, yMax As Double?, Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0)
            GetXAxis(xAxisIndex).Dims.SetAxis(xMin, xMax)
            GetYAxis(yAxisIndex).Dims.SetAxis(yMin, yMax)
        End Sub

        ''' <summary>
        ''' Define axis limits for a particuar axis.
        ''' </summary>
        Public Sub AxisSet(limits As AxisLimits, Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0)
            GetXAxis(xAxisIndex).Dims.SetAxis(limits.XMin, limits.XMax)
            GetYAxis(yAxisIndex).Dims.SetAxis(limits.YMin, limits.YMax)
        End Sub

        ''' <summary>
        ''' Return X and Y axis limits.
        ''' </summary>
        Public Function AxisLimits(xAxisIndex As Integer, yAxisIndex As Integer) As AxisLimits
            Dim xAxis As Renderable.Axis = GetXAxis(xAxisIndex)
            Dim yAxis As Renderable.Axis = GetYAxis(yAxisIndex)
            Return New AxisLimits(xAxis.Dims.Min, xAxis.Dims.Max, yAxis.Dims.Min, yAxis.Dims.Max)
        End Function

        ''' <summary>
        ''' Pan all axes by the given pixel distance.
        ''' </summary>
        Public Sub AxesPanPx(dxPx As Single, dyPx As Single)
            For Each axis As Renderable.Axis In Axes
                If axis.IsHorizontal Then
                    axis.Dims.PanPx(dxPx)
                Else
                    axis.Dims.PanPx(dyPx)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Zoom all axes by the given pixel distance.
        ''' </summary>
        Public Sub AxesZoomPx(xPx As Single, yPx As Single, Optional lockRatio As Boolean = False)
            If lockRatio Then
                Dim xMax As Single = Math.Max(xPx, yPx)
                Dim yMax As Single = Math.Max(xPx, yPx) 'same?
                xPx = xMax
                yPx = yMax
            End If
            For Each axis As Renderable.Axis In Axes
                Dim deltaPx As Double = If(axis.IsHorizontal, xPx, yPx)
                Dim delta As Double = deltaPx * axis.Dims.UnitsPerPx
                Dim deltaFrac As Double = delta / (Math.Abs(delta) + axis.Dims.Span)
                axis.Dims.Zoom(Math.Pow(10, deltaFrac))
            Next
        End Sub

        ''' <summary>
        ''' Zoom all axes by the given fraction.
        ''' </summary>
        Public Sub AxesZoomTo(xFrac As Double, yFrac As Double, xPixel As Single, yPixel As Single)
            For Each axis As Renderable.Axis In Axes
                Dim frac As Double = If(axis.IsHorizontal, xFrac, yFrac)
                Dim centerPixel As Single = If(axis.IsHorizontal, xPixel, yPixel)
                Dim center As Double = axis.Dims.GetUnit(centerPixel)
                axis.Dims.Zoom(frac, center)
            Next
        End Sub

        ''' <summary>
        ''' Automatically adjust X and Y axis limits of all axes to fit the data.
        ''' </summary>
        Public Sub AxisAutoAll(Optional horizontalMargin As Double? = Nothing, Optional verticalMargin As Double? = Nothing)
            AxisAutoAllX(horizontalMargin)
            AxisAutoAllY(verticalMargin)
        End Sub

        ''' <summary>
        ''' Automatically adjust axis limits for all axes which have not yet been set.
        ''' </summary>
        Public Sub AxisAutoUnsetAxes(Optional horizontalMargin As Double? = Nothing, Optional verticalMargin As Double? = Nothing)
            Dim unsetAxesX As Renderable.Axis() = HorizontalAxes _
                .Where(Function(x) Not x.Dims.HasBeenSet AndAlso x.Dims.IsNan) _
                .Select(Function(x) x.AxisIndex) _
                .Distinct() _
                .Select(Function(x) GetXAxis(x)).ToArray()

            Dim unsetAxesY As Renderable.Axis() = VerticalAxes _
                .Where(Function(x) (Not x.Dims.HasBeenSet) AndAlso x.Dims.IsNan) _
                .Select(Function(x) x.AxisIndex) _
                .Distinct() _
                .Select(Function(x) GetYAxis(x)).ToArray()

            For Each xa In unsetAxesX
                AxisAutoX(xa.AxisIndex, horizontalMargin)
            Next
            For Each yA In unsetAxesY
                AxisAutoY(yA.AxisIndex, verticalMargin)
            Next
        End Sub

        ''' <summary>
        ''' If a scale lock mode is in use, modify the axis limits accordingly.
        ''' </summary>
        Public Sub EnforceEqualAxisScales()
            Dim mode As EqualScaleMode = EqualScaleMode

            If (mode = EqualScaleMode.PreserveLargest) Then
                mode = If(XAxis.Dims.DataSizePx > YAxis.Dims.DataSizePx,
                    EqualScaleMode.PreserveX,
                    EqualScaleMode.PreserveY)
            End If

            If (mode = EqualScaleMode.PreserveSmallest) Then
                mode = If(XAxis.Dims.DataSizePx < YAxis.Dims.DataSizePx,
                    EqualScaleMode.PreserveX,
                    EqualScaleMode.PreserveY)
            End If

            Select Case mode
                Case EqualScaleMode.Disabled
                    Return

                Case EqualScaleMode.PreserveX
                    Dim yHalfSize As Double = (YAxis.Dims.DataSizePx / 2) * XAxis.Dims.UnitsPerPx
                    AxisSet(Nothing, Nothing, YAxis.Dims.Center - yHalfSize, YAxis.Dims.Center + yHalfSize)

                Case EqualScaleMode.PreserveY
                    Dim xHalfSize As Double = (XAxis.Dims.DataSizePx / 2) * YAxis.Dims.UnitsPerPx
                    AxisSet(XAxis.Dims.Center - xHalfSize, XAxis.Dims.Center + xHalfSize, Nothing, Nothing)

                Case EqualScaleMode.ZoomOut
                    Dim maxUnitsPerPx As Double = Math.Max(XAxis.Dims.UnitsPerPx, YAxis.Dims.UnitsPerPx)
                    Dim halfX As Double = (XAxis.Dims.DataSizePx / 2) * maxUnitsPerPx
                    Dim halfY As Double = (YAxis.Dims.DataSizePx / 2) * maxUnitsPerPx
                    AxisSet(XAxis.Dims.Center - halfX, XAxis.Dims.Center + halfX, YAxis.Dims.Center - halfY, YAxis.Dims.Center + halfY)

                Case EqualScaleMode.PreserveLargest
                    Throw New InvalidOperationException("This mode should have been converted to preserve X or Y.")

                Case EqualScaleMode.PreserveSmallest
                    Throw New InvalidOperationException("This mode should have been converted to preserve X or Y.")

                Case Else
                    Throw New InvalidOperationException("Unknown scale lock mode.")
            End Select
        End Sub

        ''' <summary>
        ''' Automatically adjust X axis limits to fit the data.
        ''' </summary>
        Public Sub AxisAutoAllX(Optional margin As Double? = Nothing)
            Dim xAxisIndexes As Integer() = Axes.Where(Function(x) x.IsHorizontal).Select(Function(x) x.AxisIndex).Distinct().ToArray()
            For Each i As Integer In xAxisIndexes
                AxisAutoX(i, margin)
            Next
        End Sub

        ''' <summary>
        ''' Automatically adjust Y axis limits to fit the data.
        ''' </summary>
        Public Sub AxisAutoAllY(Optional margin As Double? = Nothing)
            Dim yAxisIndexes As Integer() = Axes.Where(Function(x) x.IsVertical).Select(Function(x) x.AxisIndex).Distinct().ToArray()
            For Each i As Integer In yAxisIndexes
                AxisAutoY(i, margin)
            Next
        End Sub

        Public Sub AxisAutoX(xAxisIndex As Integer, Optional margin As Double? = Nothing)
            If (margin < 0) OrElse (margin >= 1) Then
                Throw New ArgumentException("Margins must be greater than 0 and less than 1.")
            End If

            If margin.HasValue Then
                MarginsX = margin.Value
            End If

            Dim plottableLimits = Plottables.Where(Function(x) x.IsVisible) _
                .Where(Function(x) x.XAxisIndex = xAxisIndex) _
                .Select(Function(x) x.GetAxisLimits()).ToArray()

            Dim min As Double = Double.NaN
            Dim max As Double = Double.NaN
            For Each limits In plottableLimits
                If (Not Double.IsNaN(limits.XMin)) Then
                    min = If(Double.IsNaN(min), limits.XMin, Math.Min(min, limits.XMin))
                End If
                If (Not Double.IsNaN(limits.XMax)) Then
                    max = If(Double.IsNaN(max), limits.XMax, Math.Max(max, limits.XMax))
                End If
            Next

            If (Double.IsNaN(min) AndAlso Double.IsNaN(max)) Then
                Return
            End If

            Dim xAxis = GetXAxis(xAxisIndex)
            xAxis.Dims.SetAxis(min, max)

            Dim zoomFrac As Double = 1 - MarginsX
            xAxis.Dims.Zoom(zoomFrac)
        End Sub

        Public Sub AxisAutoY(yAxisIndex As Integer, Optional margin As Double? = Nothing)
            If (margin < 0 OrElse margin >= 1) Then
                Throw New ArgumentException("Margins must be greater than 0 and less than 1.")
            End If
            If margin.HasValue Then
                MarginsY = margin.Value
            End If

            Dim plottableLimits = Plottables.Where(Function(x) x.IsVisible) _
                .Where(Function(x) x.YAxisIndex = yAxisIndex) _
                .Select(Function(x) x.GetAxisLimits()).ToArray()

            Dim min As Double = Double.NaN
            Dim max As Double = Double.NaN
            For Each limits In plottableLimits
                If (Not Double.IsNaN(limits.YMin)) Then
                    min = If(Double.IsNaN(min), limits.YMin, Math.Min(min, limits.YMin))
                End If
                If (Not Double.IsNaN(limits.YMax)) Then
                    max = If(Double.IsNaN(max), limits.YMax, Math.Max(max, limits.YMax))
                End If
            Next

            If (Double.IsNaN(min) AndAlso Double.IsNaN(max)) Then
                Return
            End If

            Dim yAxis = GetYAxis(yAxisIndex)
            yAxis.Dims.SetAxis(min, max)

            Dim zoomFrac As Double = 1 - MarginsY
            yAxis.Dims.Zoom(zoomFrac)
        End Sub

        ''' <summary>
        ''' Store axis limits (useful for storing state upon a MouseDown event).
        ''' </summary>
        Public Sub RememberAxisLimits()
            AxisAutoUnsetAxes()
            For Each axis As Renderable.Axis In Axes
                axis.Dims.Remember()
            Next
        End Sub

        ''' <summary>
        ''' Recall axis limits (useful for recalling state from a previous MouseDown event).
        ''' </summary>
        Public Sub RecallAxisLimits()
            For Each axis As Renderable.Axis In Axes
                axis.Dims.Recall()
            Next
        End Sub

        ''' <summary>
        ''' Remember mouse position (do this before calling <see cref="MousePan(Single, Single)"/> or <see cref="MouseZoom(Single, Single)"/>).
        ''' </summary>
        Public Sub MouseDown(mouseDownX As Single, mouseDownY As Single)
            RememberAxisLimits()
            _MouseDownX = mouseDownX
            _MouseDownY = mouseDownY
        End Sub

        ''' <summary>
        ''' Pan all axes based on the mouse position now vs that last given to <see cref="MouseDown(Single, Single)"/>.
        ''' </summary>
        Public Sub MousePan(mouseNowX As Single, mouseNowY As Single)
            RecallAxisLimits()
            AxesPanPx(MouseDownX - mouseNowX, mouseNowY - MouseDownY)
        End Sub

        ''' <summary>
        ''' Zoom all axes based on the mouse position now vs that last given to <see cref="MouseDown(Single, Single)"/>().
        ''' </summary>
        Public Sub MouseZoom(mouseNowX As Single, mouseNowY As Single)
            RecallAxisLimits()
            AxesZoomPx(mouseNowX - MouseDownX, MouseDownY - mouseNowY)
        End Sub

        Public Sub MouseZoomRect(mouseNowX As Single, mouseNowY As Single, Optional finalize As Boolean = False)
            Dim left As Single = Math.Min(MouseDownX, mouseNowX)
            Dim right As Single = Math.Max(MouseDownX, mouseNowX)
            Dim top As Single = Math.Min(MouseDownY, mouseNowY)
            Dim bottom As Single = Math.Max(MouseDownY, mouseNowY)
            Dim width As Single = right - left
            Dim height As Single = bottom - top

            If finalize Then
                ZoomRectangle.Clear()
                For Each axis In Axes
                    If axis.IsHorizontal Then
                        Dim x1 As Double = axis.Dims.GetUnit(left)
                        Dim x2 As Double = axis.Dims.GetUnit(right)
                        axis.Dims.SetAxis(x1, x2)
                    Else
                        Dim y1 As Double = axis.Dims.GetUnit(bottom)
                        Dim y2 As Double = axis.Dims.GetUnit(top)
                        axis.Dims.SetAxis(y1, y2)
                    End If
                Next
            Else
                'TODO: don't require data offset shifting prior to calling this
                ZoomRectangle.Set(left - XAxis.Dims.DataOffsetPx, top - YAxis.Dims.DataOffsetPx, width, height)
            End If
        End Sub

        ''' <summary>
        ''' Ensure all axes have the same size and offset as the primary X and Y axis.
        ''' </summary>
        Public Sub CopyPrimaryLayoutToAllAxes()
            For Each axis As Renderable.Axis In Axes
                If axis.IsHorizontal Then
                    axis.Dims.Resize(Width, XAxis.Dims.DataSizePx, XAxis.Dims.DataOffsetPx)
                Else
                    axis.Dims.Resize(Height, YAxis.Dims.DataSizePx, YAxis.Dims.DataOffsetPx)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Automatically adjust the layout for every axis.
        ''' </summary>
        Public Sub LayoutAuto()
            Dim xIndexes As Integer() = HorizontalAxes.Select(Function(x) x.AxisIndex).Distinct().ToArray()
            Dim yIndexes As Integer() = VerticalAxes.Select(Function(x) x.AxisIndex).Distinct().ToArray()

            For Each xAxisIndex In xIndexes
                LayoutAuto(xAxisIndex, 0)
            Next
            For Each yAxisIndex In yIndexes
                LayoutAuto(0, yAxisIndex)
            Next
        End Sub

        Private Sub LayoutAuto(xAxisIndex As Integer, yAxisIndex As Integer)
            ' TODO: separate this into distinct X and Y functions (requires refactoring plottable interface)
            Dim atLeastOneAxisIsZero As Boolean = (xAxisIndex = 0) OrElse (yAxisIndex = 0)
            If (Not atLeastOneAxisIsZero) Then
                Throw New InvalidOperationException()
            End If

            ' Adjust padding around the data area to accommodate title and tick labels.
            ' This is a chicken-and-egg problem:
            '   * TICK DENSITY depends on the DATA AREA SIZE
            '   * DATA AREA SIZE depends on LAYOUT PADDING
            '   * LAYOUT PADDING depends on MAXIMUM LABEL SIZE
            '   * MAXIMUM LABEL SIZE depends on TICK DENSITY
            ' To solve this, start by assuming data area size == figure size and layout padding == 0,
            ' then calculate ticks, then set padding based on the largest tick, then re-calculate ticks.

            'Axis limits shall not change
            Dim dims = GetPlotDimensions(xAxisIndex, yAxisIndex, 1)
            Dim figSize As New SizeF(Width, Height)

            'first-pass tick calculation based on full image size 
            Dim dimsFull As New PlotDimensions(figSize, figSize, New PointF(0, 0), dims.AxisLimits, 1)

            For Each axis In Axes
                Dim isMatchingXAxis As Boolean = axis.IsHorizontal AndAlso (axis.AxisIndex = xAxisIndex)
                Dim isMatchingYAxis As Boolean = axis.IsVertical AndAlso (axis.AxisIndex = yAxisIndex)
                If (isMatchingXAxis OrElse isMatchingYAxis) Then
                    axis.RecalculateTickPositions(dimsFull)
                    axis.RecalculateAxisSize()
                End If
            Next

            'Now adjust our layout based on measured axis sizes
            RecalculateDataPadding()

            'Now recalculate ticks based on new layout
            Dim dataSize As New SizeF(XAxis.Dims.DataSizePx, YAxis.Dims.DataSizePx)
            Dim dataOffset As New PointF(XAxis.Dims.DataOffsetPx, YAxis.Dims.DataOffsetPx)

            Dim dims3 As New PlotDimensions(figSize, dataSize, dataOffset, dims.AxisLimits, 1)
            For Each axis In Axes
                Dim isMatchingXAxis As Boolean = axis.IsHorizontal AndAlso (axis.AxisIndex = xAxisIndex)
                Dim isMatchingYAxis As Boolean = axis.IsVertical AndAlso (axis.AxisIndex = yAxisIndex)
                If (isMatchingXAxis OrElse isMatchingYAxis) Then
                    axis.RecalculateTickPositions(dims3)
                End If
            Next

            'Adjust the layout based on measured tick label sizes
            RecalculateDataPadding()
        End Sub

        Private Sub RecalculateDataPadding()
            Dim edges As Renderable.Edge() = {Renderable.Edge.Left, Renderable.Edge.Right, Renderable.Edge.Top, Renderable.Edge.Bottom}
            For Each edge In edges
                Dim offset As Single = 0
                For Each axis In Axes.Where(Function(x) x.Edge = edge)
                    axis.SetOffset(offset)
                    offset += axis.GetSize()
                Next
            Next

            Dim padLeft As Single
            Dim padRight As Single
            Dim padBottom As Single
            Dim padTop As Single

            If (ManualDataPadding is Nothing) Then
                padLeft = Axes.Where(Function(x) x.Edge = Renderable.Edge.Left).Select(Function(x) x.GetSize()).Sum()
                padRight = Axes.Where(Function(x) x.Edge = Renderable.Edge.Right).Select(Function(x) x.GetSize()).Sum()
                padBottom = Axes.Where(Function(x) x.Edge = Renderable.Edge.Bottom).Select(Function(x) x.GetSize()).Sum()
                padTop = Axes.Where(Function(x) x.Edge = Renderable.Edge.Top).Select(Function(x) x.GetSize()).Sum()
            Else
                padLeft = ManualDataPadding.Value.Left
                padRight = ManualDataPadding.Value.Right
                padBottom = ManualDataPadding.Value.Bottom
                padTop = ManualDataPadding.Value.Top
            End If

            For Each axis In Axes
                If (axis.IsHorizontal) Then
                    axis.Dims.SetPadding(padLeft, padRight)
                Else
                    axis.Dims.SetPadding(padTop, padBottom)
                End If
            Next
        End Sub

#End Region '/METHODS

    End Class

End Namespace