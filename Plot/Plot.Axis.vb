Imports System.Collections.Generic
Imports System.Drawing
Imports System.Globalization
Imports System.Linq

' This file contains code related to Axes including:
' - Unit/Pixel conversions
' - Configuring axis limits and boundaries
' - Axis labels (XLabel, YLabel, Title, etc)
' - Adding multiple axes
' - Grid lines
' - Tick marks
' - Tick labels

Namespace ScottPlot

    Partial Class Plot

#Region "SHORTCUTS: PRIMARY AXES"

        ''' <summary>
        ''' Axis on the bottom edge of the plot.
        ''' </summary>
        Public ReadOnly Property XAxis As Renderable.Axis
            Get
                Return Settings.XAxis
            End Get
        End Property

        ''' <summary>
        ''' Axis on the top edge of the plot.
        ''' </summary>
        Public ReadOnly Property XAxis2 As Renderable.Axis
            Get
                Return Settings.XAxis2
            End Get
        End Property

        ''' <summary>
        ''' Axis on the left edge of the plot.
        ''' </summary>
        Public ReadOnly Property YAxis As Renderable.Axis
            Get
                Return Settings.YAxis
            End Get
        End Property

        ''' <summary>
        ''' Axis on the right edge of the plot.
        ''' </summary>
        Public ReadOnly Property YAxis2 As Renderable.Axis
            Get
                Return Settings.YAxis2
            End Get
        End Property

#End Region '/SHORTCUTS: PRIMARY AXES

#Region "SHORTCUTS: AXIS LABEL, TICK, AND GRID"

        ''' <summary>
        ''' Set the label for the vertical axis to the right of the plot (<see cref="XAxis"/>).
        ''' </summary>
        ''' <param name="label">New text.</param>
        Public Sub XLabel(label As String)
            XAxis.Label(label, Nothing, Nothing, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' Set the label for the vertical axis to the right of the plot (<see cref="YAxis2"/>).
        ''' </summary>
        ''' <param name="label">New text.</param>
        Public Sub YLabel(label As String)
            YAxis.Label(label, Nothing, Nothing, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' Set the label for the horizontal axis above the plot (<see cref="XAxis2"/>).
        ''' </summary>
        Public Sub Title(label As String,
                         Optional bold As Boolean? = True,
                         Optional color As Color? = Nothing,
                         Optional size As Single? = Nothing,
                         Optional fontName As String = Nothing)
            XAxis2.Label(label, color, size, bold, fontName)
        End Sub

        ''' <summary>
        ''' Configure color and visibility of the frame that outlines the data area.
        ''' Note that the axis lines of all 4 primary axes touch each other, giving the appearance of a rectangle framing the data area.
        ''' This method allows the user to customize these lines as a group or individually.
        ''' </summary>
        ''' <param name="visible">Visibility of the frames for the 4 primary axes.</param>
        ''' <param name="color">Color for the 4 primary axis lines.</param>
        ''' <param name="left">Visibility of the left axis (<see cref="YAxis"/>) line.</param>
        ''' <param name="right">Visibility of the right axis (<see cref="YAxis2"/>) line.</param>
        ''' <param name="bottom">Visibility of the bottom axis (<see cref="XAxis"/>) line.</param>
        ''' <param name="top">Visibility of the top axis (<see cref="XAxis2"/>) line.</param>
        <Obsolete("This method has been deprecated. Visibility and customization can be controlled with methods like YAxis.Hide(), YAxis.Line(), etc.", True)>
        Public Sub Frame(Optional visible As Boolean? = Nothing,
                         Optional color As Color? = Nothing,
                         Optional left As Boolean? = Nothing,
                         Optional right As Boolean? = Nothing,
                         Optional bottom As Boolean? = Nothing,
                         Optional top As Boolean? = Nothing)
            Dim primaryAxes As Renderable.Axis() = {XAxis, XAxis2, YAxis, YAxis2}
            For i As Integer = 0 To primaryAxes.Length - 1
                primaryAxes(i).Line(visible, color, Nothing)
            Next
            YAxis.Line(left)
            YAxis2.Line(right)
            XAxis.Line(bottom)
            XAxis2.Line(top)
        End Sub

        ''' <summary>
        ''' Give the plot a frameless appearance by hiding all axes.
        ''' Axes are hidden by making them invisible and setting their size to zero.
        ''' This causes the data area to go right up to the edge of the plot.
        ''' </summary>
        Public Sub Frameless(Optional hideAllAxes As Boolean = True)
            For Each axis As Renderable.Axis In Settings.Axes
                axis.Hide(hideAllAxes)
            Next
        End Sub

        ''' <summary>
        ''' Control visibility of axes.
        ''' </summary>
        <Obsolete("This method is deprecated. Call Frameless() to control axis visibility.")>
        Public Sub Frame(enable As Boolean)
            Frameless(Not enable)
        End Sub

        ''' <summary>
        ''' Customize basic options for the primary X and Y axes. 
        ''' Call XAxis.Grid() and YAxis.Grid() to further customize grid settings.
        ''' </summary>
        ''' <param name="enable">Sets visibility of X and Y grid lines.</param>
        ''' <param name="color">Sets color of of X and Y grid lines.</param>
        ''' <param name="lineStyle">Defines the style for X and Y grid lines.</param>
        ''' <param name="onTop">Defines whether the grid is drawn on top of plottables.</param>
        Public Sub Grid(Optional enable As Boolean? = Nothing,
                        Optional color As Color? = Nothing,
                        Optional lineStyle As LineStyle? = Nothing,
                        Optional onTop As Boolean? = Nothing)
            If enable.HasValue Then
                XAxis.Grid(enable.Value)
                YAxis.Grid(enable.Value)
            End If
            XAxis.MajorGrid(color:=color, lineStyle:=lineStyle)
            YAxis.MajorGrid(color:=color, lineStyle:=lineStyle)
            If onTop.HasValue Then
                Settings.DrawGridAbovePlottables = onTop.Value
            End If
        End Sub

        ''' <summary>
        ''' Set padding around the data area by defining the minimum size and padding for all axes.
        ''' </summary>
        ''' <param name="left">YAxis size (in pixels) that defines the area to the left of the plot.</param>
        ''' <param name="right">YAxis2 size (in pixels) that defines the area to the right of the plot.</param>
        ''' <param name="bottom">XAxis size (in pixels) that defines the area to the bottom of the plot.</param>
        ''' <param name="top">XAxis2 size (in pixels) that defines the area to the top of the plot.</param>
        ''' <param name="padding">Customize the default padding between axes and the edge of the plot.</param>
        Public Sub Layout(Optional left As Single? = Nothing,
                          Optional right As Single? = Nothing,
                          Optional bottom As Single? = Nothing,
                          Optional top As Single? = Nothing,
                          Optional padding As Single? = 5)
            YAxis.Layout(padding, left)
            YAxis2.Layout(padding, right)
            XAxis.Layout(padding, bottom)
            XAxis2.Layout(padding, top)
        End Sub

        ''' <summary>
        ''' Adjust this axis layout based on the layout of a source plot.
        ''' </summary>
        ''' <param name="sourcePlot">Plot to use for layout reference.</param>
        ''' <param name="horizontal">If true, horizontal layout will be matched.</param>
        ''' <param name="vertical">If true, vertical layout will be matched.</param>
        Public Sub MatchLayout(sourcePlot As Plot, Optional horizontal As Boolean = True, Optional vertical As Boolean = True)
            Dim sourceSettings As Settings = sourcePlot.GetSettings(False)
            If horizontal Then
                YAxis.SetSizeLimit(sourceSettings.YAxis.GetSize())
                YAxis2.SetSizeLimit(sourceSettings.YAxis2.GetSize())
            End If
            If vertical Then
                XAxis.SetSizeLimit(sourceSettings.XAxis.GetSize())
                XAxis2.SetSizeLimit(sourceSettings.XAxis2.GetSize())
            End If
        End Sub

        ''' <summary>
        ''' Get the axis limits for the given plot and apply them to this plot.
        ''' </summary>
        Public Sub MatchAxis(sourcePlot As Plot, Optional horizontal As Boolean = True, Optional vertical As Boolean = True)
            Dim sourceLimits As AxisLimits = sourcePlot.GetAxisLimits()
            If horizontal Then
                SetAxisLimitsX(sourceLimits.XMin, sourceLimits.XMax)
            End If
            If vertical Then
                SetAxisLimitsY(sourceLimits.YMin, sourceLimits.YMax)
            End If
        End Sub

        ''' <summary>
        ''' Define the shape of the data area as padding (in pixels) on all 4 sides.
        ''' Once defined, the layout will not be adjusted  As Renderable.Axis labels or ticks change.
        ''' Pass null into this function to disable the manual data area.
        ''' </summary>
        Public Sub ManualDataArea(padding As PixelPadding)
            Settings.ManualDataPadding = New PixelPadding?(padding)
        End Sub

        ''' <summary>
        ''' Manually define X axis tick labels using consecutive integer positions (0, 1, 2, etc).
        ''' </summary>
        ''' <param name="labels">New tick labels for the X axis.</param>
        Public Sub XTicks(labels As String())
            XTicks(DataGen.Consecutive(labels.Length), labels)
        End Sub

        ''' <summary>
        ''' Manually define X axis tick positions and labels.
        ''' </summary>
        ''' <param name="positions">Positions on the X axis.</param>
        ''' <param name="labels">New tick labels for the X axis.</param>
        Public Sub XTicks(Optional positions As Double() = Nothing, Optional labels As String() = Nothing)
            XAxis.ManualTickPositions(positions, labels)
        End Sub

        ''' <summary>
        ''' Manually define Y axis tick labels using consecutive integer positions (0, 1, 2, etc).
        ''' </summary>
        ''' <param name="labels">New tick labels for the Y axis.</param>
        Public Sub YTicks(labels As String())
            YTicks(DataGen.Consecutive(labels.Length), labels)
        End Sub

        ''' <summary>
        ''' Manually define Y axis tick positions and labels.
        ''' </summary>
        ''' <param name="positions">positions on the Y axis.</param>
        ''' <param name="labels">New tick labels for the Y axis.</param>
        Public Sub YTicks(Optional positions As Double() = Nothing, Optional labels As String() = Nothing)
            YAxis.ManualTickPositions(positions, labels)
        End Sub

        ''' <summary>
        ''' Manually define Y axis tick positions and labels.
        ''' </summary>
        ''' <param name="positions">Positions on the Y axis.</param>
        ''' <param name="labels">New tick labels for the Y axis.</param>
        Public Sub SetCulture(culture As CultureInfo)
            For Each axis As Renderable.Axis In Settings.Axes
                axis.SetCulture(culture)
            Next
        End Sub

        ''' <summary>
        ''' Set the culture to use for number-to-string converstion for tick labels of all axes.
        ''' This overload allows you to manually define every format string,
        ''' allowing extensive customization of number and date formatting.
        ''' </summary>
        ''' <param name="decimalSeparator">Separates the decimal digits.</param>
        ''' <param name="numberGroupSeparator">Separates large numbers ito groups of digits for readability.</param>
        ''' <param name="decimalDigits">Number of digits after the numberDecimalSeparator.</param>
        ''' <param name="numberNegativePattern">Appearance of negative numbers.</param>
        ''' <param name="numberGroupSizes">Sizes of decimal groups which are separated by the numberGroupSeparator.</param>
        Public Sub SetCulture(Optional shortDatePattern As String = Nothing,
                              Optional decimalSeparator As String = Nothing,
                              Optional numberGroupSeparator As String = Nothing,
                              Optional decimalDigits As Integer? = Nothing,
                              Optional numberNegativePattern As Integer? = Nothing,
                              Optional numberGroupSizes As Integer() = Nothing)
            For Each axis As Renderable.Axis In Settings.Axes
                axis.SetCulture(shortDatePattern, decimalSeparator, numberGroupSeparator,
                                decimalDigits, numberNegativePattern, numberGroupSizes)
            Next
        End Sub

#End Region '/SHORTCUTS: AXIS LABEL, TICK, AND GRID

#Region "AXIS CREATION"

        Private ReadOnly Property NextAxisIndex As Integer
            Get
                Return Settings.Axes.Select(Function(a) a.AxisIndex).Max() + 1
            End Get
        End Property

        ''' <summary>
        ''' Create and return an additional axis
        ''' </summary>
        ''' <param name="edge">Edge of the plot the new axis will belong to.</param>
        ''' <param name="axisIndex">Only plottables with the same axis index will use this axis. Creates an auto-generated index if null.</param>
        ''' <param name="title">Defualt label to use for the axis.</param>
        ''' <param name="color">Defualt color to use for the axis.</param>
        ''' <returns>The axis that was just created and added to the plot. You can further customize it by interacting with it.</returns>
        Public Function AddAxis(edge As Renderable.Edge, Optional axisIndex As Integer? = Nothing, Optional title As String = Nothing, Optional color As Color? = Nothing) As Renderable.Axis
            axisIndex = If(axisIndex, NextAxisIndex)
            If (axisIndex <= 1) Then
                Throw New ArgumentException("The default axes already occupy indexes 0 and 1. Additional axes require higher indexes.")
            End If

            Dim axis As Renderable.Axis
            Select Case edge
                Case Renderable.Edge.Left
                    axis = New Renderable.AdditionalLeftAxis(axisIndex.Value, title)
                Case Renderable.Edge.Right
                    axis = New Renderable.AdditionalRightAxis(axisIndex.Value, title)
                Case Renderable.Edge.Bottom
                    axis = New Renderable.AdditionalBottomAxis(axisIndex.Value, title)
                Case Renderable.Edge.Top
                    axis = New Renderable.AdditionalTopAxis(axisIndex.Value, title)
                Case Else
                    Throw New NotImplementedException($"Unsupported edge: {edge}.")
            End Select
            If color.HasValue Then
                axis.Color(color.Value)
            End If
            Settings.Axes.Add(axis)
            Return axis
        End Function

        ''' <summary>
        ''' Remove the a specific axis from the plot.
        ''' </summary>
        Public Sub RemoveAxis(axis As Renderable.Axis)
            Settings.Axes.Remove(axis)
        End Sub

        ''' <summary>
        ''' Returns axes matching the given <paramref name="axisIndex"/> and <paramref name="isVertical"/>.
        ''' </summary>
        ''' <param name="axisIndex">The axis index to match, or null to allow any index.</param>
        ''' <param name="isVertical">True to match only Y axes, false to match only X axes, or null to match either.</param>
        ''' <returns>The axes matching the given properties.</returns>
        Public Function GetAxesMatching(Optional axisIndex As Integer? = Nothing, Optional isVertical As Boolean? = Nothing) As IEnumerable(Of Renderable.Axis)
            Dim results As IEnumerable(Of Renderable.Axis) = Settings.Axes
            If axisIndex.HasValue Then
                results = results.Where(Function(axis As Renderable.Axis) axis.AxisIndex = axisIndex.Value)
            End If
            If isVertical.HasValue Then
                results = results.Where(Function(axis As Renderable.Axis) axis.IsVertical = isVertical.Value)
            End If
            Return results
        End Function

#End Region '/AXIS CREATION

#Region "COORDINATE/PIXEL CONVERSIONS"

        ''' <summary>
        ''' Return the coordinate (in coordinate space) for the given pixel.
        ''' </summary>
        ''' <param name="xPixel">Horizontal pixel location.</param>
        ''' <param name="yPixel">Vertical pixel location.</param>
        ''' <param name="xAxisIndex">Index of the horizontal axis to use.</param>
        ''' <param name="yAxisIndex">Index of the vertical axis to use.</param>
        ''' <returns>Point in coordinate space.</returns>
        Public Function GetCoordinate(xPixel As Single, yPixel As Single, Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0) As Tuple(Of Double, Double)
            Dim xCoordinate As Double = Settings.GetXAxis(xAxisIndex).Dims.GetUnit(xPixel)
            Dim yCoordinate As Double = Settings.GetYAxis(yAxisIndex).Dims.GetUnit(yPixel)
            Return New Tuple(Of Double, Double)(xCoordinate, yCoordinate)
        End Function

        ''' <summary>
        ''' Return the X position (in coordinate space) for the given pixel column.
        ''' </summary>
        ''' <param name="xPixel">Horizontal pixel location.</param>
        ''' <param name="xAxisIndex">Index of the horizontal axis to use.</param>
        ''' <returns>Horizontal position in coordinate space.</returns>
        Public Function GetCoordinateX(xPixel As Single, Optional xAxisIndex As Integer = 0) As Double
            Return Settings.GetXAxis(xAxisIndex).Dims.GetUnit(xPixel)
        End Function

        ''' <summary>
        ''' Return the Y position (in coordinate space) for the given pixel row.
        ''' </summary>
        ''' <param name="yPixel">Vertical pixel location.</param>
        ''' <param name="yAxisIndex">Index of the vertical axis to use.</param>
        ''' <returns>Vertical position in coordinate space.</returns>
        Public Function GetCoordinateY(yPixel As Single, Optional yAxisIndex As Integer = 0) As Double
            Return Settings.GetYAxis(yAxisIndex).Dims.GetUnit(yPixel)
        End Function

        ''' <summary>
        ''' Return the pixel for the given point in coordinate space.
        ''' </summary>
        ''' <param name="x">Horizontal coordinate.</param>
        ''' <param name="y">Vertical coordinate.</param>
        ''' <param name="xAxisIndex">Index of the horizontal axis to use.</param>
        ''' <param name="yAxisIndex">Index of the vertical axis to use.</param>
        ''' <returns>Pixel location.</returns>
        Public Function GetPixel(x As Double, y As Double, Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0) As Tuple(Of Single, Single)
            Dim xPixel As Single = Settings.GetXAxis(xAxisIndex).Dims.GetPixel(x)
            Dim yPixel As Single = Settings.GetYAxis(yAxisIndex).Dims.GetPixel(y)
            Return New Tuple(Of Single, Single)(xPixel, yPixel)
        End Function

        ''' <summary>
        ''' Return the horizontal pixel location given position in coordinate space.
        ''' </summary>
        ''' <param name="x">Horizontal coordinate.</param>
        ''' <param name="xAxisIndex">Index of the horizontal axis to use.</param>
        ''' <returns>Horizontal pixel position.</returns>
        Public Function GetPixelX(x As Double, Optional xAxisIndex As Integer = 0) As Single
            Return Settings.GetXAxis(xAxisIndex).Dims.GetPixel(x)
        End Function

        ''' <summary>
        ''' Return the vertical pixel location given position in coordinate space.
        ''' </summary>
        ''' <param name="y">Vertical coordinate.</param>
        ''' <param name="yAxisIndex">Index of the vertical axis to use.</param>
        ''' <returns>Vertical pixel position.</returns>
        Public Function GetPixelY(y As Double, Optional yAxisIndex As Integer = 0) As Single
            Return Settings.GetYAxis(yAxisIndex).Dims.GetPixel(y)
        End Function

#End Region '/COORDINATE/PIXEL CONVERSIONS

#Region "AXIS LIMITS: GET AND SET"

        ''' <summary>
        ''' Return the limits of the data contained by this plot (regardless of the axis limits).
        ''' WARNING: This method iterates all data points in the plot and may be slow for large datasets.
        ''' </summary>
        Public Function GetDataLimits(Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0) As AxisLimits
            Dim xMin As Double = Double.PositiveInfinity
            Dim xMax As Double = Double.NegativeInfinity
            Dim yMin As Double = Double.PositiveInfinity
            Dim yMax As Double = Double.NegativeInfinity

            For Each plottable As Plottable.IPlottable In GetPlottables()
                If plottable.IsVisible Then
                    Dim xAxisMatch As Boolean = plottable.XAxisIndex = xAxisIndex
                    Dim yAxisMatch As Boolean = plottable.YAxisIndex = yAxisIndex

                    If xAxisMatch OrElse yAxisMatch Then
                        Dim limits As AxisLimits = plottable.GetAxisLimits()
                        If xAxisMatch Then
                            If (Not Double.IsNaN(limits.XMin)) Then
                                xMin = Math.Min(xMin, limits.XMin)
                            End If
                            If (Not Double.IsNaN(limits.XMax)) Then
                                xMax = Math.Max(xMax, limits.XMax)
                            End If
                        End If
                        If yAxisMatch Then
                            If (Not Double.IsNaN(limits.YMin)) Then
                                yMin = Math.Min(yMin, limits.YMin)
                            End If
                            If (Not Double.IsNaN(limits.YMax)) Then
                                yMax = Math.Max(yMax, limits.YMax)
                            End If
                        End If
                    End If
                End If
            Next

            If Double.IsPositiveInfinity(xMin) Then xMin = Double.NegativeInfinity
            If Double.IsNegativeInfinity(xMax) Then xMax = Double.PositiveInfinity
            If Double.IsPositiveInfinity(yMin) Then yMin = Double.NegativeInfinity
            If Double.IsNegativeInfinity(yMax) Then yMax = Double.PositiveInfinity

            Return New AxisLimits(xMin, xMax, yMin, yMax)
        End Function

        ''' <summary>
        ''' Returns the current limits for a given pair of axes.
        ''' </summary>
        ''' <param name="xAxisIndex">Which axis index to reference.</param>
        ''' <param name="yAxisIndex">Which axis index to reference.</param>
        ''' <returns>Current limits.</returns>
        Public Function GetAxisLimits(Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0) As AxisLimits
            Dim t1 As Tuple(Of Double, Double) = Settings.GetXAxis(xAxisIndex).Dims.RationalLimits()
            Dim xMin As Double = t1.Item1
            Dim xMax As Double = t1.Item2
            Dim t2 As Tuple(Of Double, Double) = Settings.GetYAxis(yAxisIndex).Dims.RationalLimits()
            Dim yMin As Double = t2.Item1
            Dim yMax As Double = t2.Item2
            Return New AxisLimits(xMin, xMax, yMin, yMax)
        End Function

        ''' <summary>
        ''' Set limits for the a given pair of axes.
        ''' </summary>
        ''' <param name="xMin">Lower limit of the horizontal axis.</param>
        ''' <param name="xMax">Upper limit of the horizontal axis.</param>
        ''' <param name="yMin">Lower limit of the vertical axis.</param>
        ''' <param name="yMax">Upper limit of the vertical axis.</param>
        ''' <param name="xAxisIndex">Index of the axis the horizontal limits apply to.</param>
        ''' <param name="yAxisIndex">Index of the axis the vertical limits apply to.</param>
        Public Sub SetAxisLimits(Optional xMin As Double? = Nothing, Optional xMax As Double? = Nothing, Optional yMin As Double? = Nothing, Optional yMax As Double? = Nothing, Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0)
            If (xMin >= xMax) Then
                Throw New InvalidOperationException($"{NameOf(xMax)} must be greater than {NameOf(xMin)}.")
            End If
            If (yMin >= yMax) Then
                Throw New InvalidOperationException($"{NameOf(yMax)} must be greater than {NameOf(yMin)}.")
            End If

            Dim notAllAxesDefined As Boolean = (xMin is Nothing) OrElse (xMax is Nothing) OrElse (yMin is Nothing) OrElse (yMax is Nothing)
            If notAllAxesDefined Then
                Settings.AxisAutoUnsetAxes(Nothing, Nothing)
            End If
            Settings.AxisSet(xMin, xMax, yMin, yMax, xAxisIndex, yAxisIndex)
        End Sub

        ''' <summary>
        ''' Set limits for the primary X axis.
        ''' </summary>
        ''' <param name="xMin">Lower limit of the horizontal axis.</param>
        ''' <param name="xMax">Upper limit of the horizontal axis.</param>
        Public Sub SetAxisLimitsX(xMin As Double, xMax As Double)
            SetAxisLimits(xMin, xMax, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' Set limits for the primary Y axis.
        ''' </summary>
        ''' <param name="yMin">Lower limit of the vertical axis.</param>
        ''' <param name="yMax">Upper limit of the vertical axis.</param>
        Public Sub SetAxisLimitsY(yMin As Double, yMax As Double)
            SetAxisLimits(Nothing, Nothing, yMin, yMax)
        End Sub

        ''' <summary>
        ''' Set limits for a pair of axes.
        ''' </summary>
        ''' <param name="limits">New limits.</param>
        ''' <param name="xAxisIndex">Index of the axis the horizontal limits apply to.</param>
        ''' <param name="yAxisIndex">Index of the axis the vertical limits apply to.</param>
        Public Sub SetAxisLimits(limits As AxisLimits, Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0)
            Settings.AxisSet(limits, xAxisIndex, yAxisIndex)
        End Sub

        ''' <summary>
        ''' Set maximum outer limits beyond which the plot cannot be zoomed-out or panned.
        ''' </summary>
        <Obsolete("use SetOuterViewLimits() or SetInnerViewLimits()", True)>
        Public Sub SetViewLimits(Optional xMin As Double = Double.NegativeInfinity,
                                 Optional xMax As Double = Double.PositiveInfinity,
                                 Optional yMin As Double = Double.NegativeInfinity,
                                 Optional yMax As Double = Double.PositiveInfinity)
            SetOuterViewLimits(xMin, xMax, yMin, yMax)
        End Sub

        ''' <summary>
        ''' Set maximum outer limits beyond which the plot cannot be zoomed-out or panned.
        ''' </summary>
        Public Sub SetOuterViewLimits(Optional xMin As Double = Double.NegativeInfinity,
                                      Optional xMax As Double = Double.PositiveInfinity,
                                      Optional yMin As Double = Double.NegativeInfinity,
                                      Optional yMax As Double = Double.PositiveInfinity,
                                      Optional xAxisIndex As Integer = 0,
                                      Optional yAxisIndex As Integer = 0)
            Settings.GetXAxis(xAxisIndex).Dims.SetBoundsOuter(xMin, xMax)
            Settings.GetYAxis(yAxisIndex).Dims.SetBoundsOuter(yMin, yMax)
        End Sub

        ''' <summary>
        ''' Set minimum innter limits which will always be visible on the plot.
        ''' </summary>
        Public Sub SetInnerViewLimits(Optional xMin As Double = Double.NegativeInfinity,
                                      Optional xMax As Double = Double.PositiveInfinity,
                                      Optional yMin As Double = Double.NegativeInfinity,
                                      Optional yMax As Double = Double.PositiveInfinity,
                                      Optional xAxisIndex As Integer = 0,
                                      Optional yAxisIndex As Integer = 0)
            Settings.GetXAxis(xAxisIndex).Dims.SetBoundsInner(xMin, xMax)
            Settings.GetYAxis(yAxisIndex).Dims.SetBoundsInner(yMin, yMax)
        End Sub

#End Region '/AXIS LIMITS: GET AND SET

#Region "AXIS LIMITS: FIT TO PLOTTABLE DATA"

        ''' <summary>
        ''' Auto-scale the axis limits to fit the data. This function is an alias for AxisAuto().
        ''' </summary>
        ''' <param name="x">Horizontal margin in the range [0, 1].</param>
        ''' <param name="y">Vertical margin in the range [0, 1].</param>
        ''' <returns>Current default margins for automatic axis scaling.</returns>
        Public Function Margins(Optional x As Double? = Nothing, Optional y As Double? = Nothing) As Tuple(Of Double, Double)
            Return Margins(x, y, 0, 0)
        End Function

        ''' <summary>
        ''' Auto-scale the axis limits to fit the data. This function is an alias for AxisAuto().
        ''' This overload is for multi-axis plots (plots with multiple X and Y axes) and will only adjust the specified axes.
        ''' </summary>
        ''' <param name="x">Horizontal margin in the range [0, 1].</param>
        ''' <param name="y">Vertical margin in the range [0, 1].</param>
        ''' <param name="xAxisIndex">Only adjust the specified axis (for plots with multiple X axes).</param>
        ''' <param name="yAxisIndex">Only adjust the specified axis (for plots with multiple Y axes).</param>
        ''' <returns>Current default margins for automatic axis scaling.</returns>
        Public Function Margins(x As Double?, y As Double?, xAxisIndex As Integer, yAxisIndex As Integer) As Tuple(Of Double, Double)
            AxisAuto(x, y, xAxisIndex, yAxisIndex)
            Return New Tuple(Of Double, Double)(Settings.MarginsX, Settings.MarginsY)
        End Function

        ''' <summary>
        ''' Automatically set axis limits to fit the data.
        ''' </summary>
        ''' <param name="horizontalMargin">Extra space (fraction) to add to the left and right of the limits of the data (typically 0.05).</param>
        ''' <param name="verticalMargin">Extra space (fraction) to add above and below the limits of the data (typically 0.1).</param>
        Public Sub AxisAuto(Optional horizontalMargin As Double? = Nothing, Optional verticalMargin As Double? = Nothing)
            Settings.AxisAutoAll(horizontalMargin, verticalMargin)
        End Sub

        ''' <summary>
        ''' Automatically set axis limits to fit the data.
        ''' This overload is designed for multi-axis plots (with multiple X axes Or multiple Y axes).
        ''' </summary>
        ''' <param name="horizontalMargin">Extra space (fraction) to add to the left and right of the limits of the data (typically 0.05).</param>
        ''' <param name="verticalMargin">Extra space (fraction) to add above and below the limits of the data (typically 0.1).</param>
        ''' <param name="xAxisIndex">Only adjust the specified axis (for plots with multiple X axes).</param>
        ''' <param name="yAxisIndex">Only adjust the specified axis (for plots with multiple Y axes).</param>
        Public Sub AxisAuto(horizontalMargin As Double?, verticalMargin As Double?, xAxisIndex As Integer, yAxisIndex As Integer)
            Settings.AxisAutoX(xAxisIndex, horizontalMargin)
            Settings.AxisAutoY(yAxisIndex, verticalMargin)
        End Sub

        ''' <summary>
        ''' Automatically adjust axis limits to fit the data.
        ''' </summary>
        ''' <param name="margin">Amount of space to the left and right of the data (typically 0.05).</param>
        ''' <param name="xAxisIndex">Only adjust the specified axis (for plots with multiple X axes).</param>
        Public Sub AxisAutoX(Optional margin As Double? = Nothing, Optional xAxisIndex As Integer = 0)
            If (Settings.Plottables.Count = 0) Then
                Dim yMin As Double? = -10
                Dim yMax As Double? = 10
                SetAxisLimits(yMin:=yMin, yMax:=yMax)
            Else
                Settings.AxisAutoX(xAxisIndex, margin)
            End If
        End Sub

        Public Sub AxisAutoY(Optional margin As Double? = Nothing, Optional yAxisIndex As Integer = 0)
            If (Settings.Plottables.Count = 0) Then
                Dim xMin As Double? = -10
                Dim xMax As Double? = 10
                SetAxisLimits(xMin:=xMin, xMax:=xMax)
            Else
                Settings.AxisAutoY(yAxisIndex, margin)
            End If
        End Sub

#End Region '/AXIS LIMITS: FIT TO PLOTTABLE DATA

#Region "AXIS LIMITS: SCALING"

        ''' <summary>
        ''' Adjust axis limits to achieve a certain pixel scale (units per pixel).
        ''' </summary>
        ''' <param name="unitsPerPixelX">Zoom so 1 pixel equals this many horizontal units in coordinate space.</param>
        ''' <param name="unitsPerPixelY">Zoom so 1 pixel equals this many vertical units in coordinate space.</param>
        Public Sub AxisScale(Optional unitsPerPixelX As Double? = Nothing, Optional unitsPerPixelY As Double? = Nothing)
            If (unitsPerPixelX IsNot Nothing) Then
                Dim spanX As Double = unitsPerPixelX.Value * Settings.XAxis.Dims.DataSizePx
                SetAxisLimits(Settings.XAxis.Dims.Center - spanX / 2,
                              Settings.XAxis.Dims.Center + spanX / 2)
            End If
            If (unitsPerPixelY IsNot Nothing) Then
                Dim spanY As Double = unitsPerPixelY.Value * Settings.YAxis.Dims.DataSizePx
                SetAxisLimits(Settings.YAxis.Dims.Center - spanY / 2,
                              Settings.YAxis.Dims.Center + spanY / 2)
            End If
        End Sub

        ''' <summary>
        ''' Lock X and Y axis scales (units per pixel) together to protect symmetry of circles and squares.
        ''' </summary>
        ''' <param name="enable">If true, scales are locked such that zooming one zooms the other.</param>
        ''' <param name="scaleMode">Defines behavior for how to adjust axis limits to achieve equal scales.</param>
        Public Sub AxisScaleLock(enable As Boolean, Optional scaleMode As EqualScaleMode = EqualScaleMode.PreserveSmallest)
            Settings.AxisAutoUnsetAxes()
            Settings.EqualScaleMode = If(enable, scaleMode, EqualScaleMode.Disabled)
            Settings.LayoutAuto()
            Settings.EnforceEqualAxisScales()
        End Sub

#End Region '/AXIS LIMITS: SCALING

#Region "AXIS LIMITS: PAN AND ZOOM"

        ''' <summary>
        ''' Zoom in or out. The amount of zoom is defined as a fraction of the current axis span.
        ''' </summary>
        ''' <param name="xFrac">Horizontal zoom (>1 means zoom in).</param>
        ''' <param name="yFrac">Vertical zoom (>1 means zoom in).</param>
        ''' <param name="zoomToX">If defined, zoom will be centered at this point.</param>
        ''' <param name="zoomToY">If defined, zoom will be centered at this point.</param>
        ''' <param name="xAxisIndex">Index of the axis to zoom.</param>
        ''' <param name="yAxisIndex">Index of the axis to zoom.</param>
        Public Sub AxisZoom(Optional xFrac As Double = 1,
                            Optional yFrac As Double = 1,
                            Optional zoomToX As Double? = Nothing,
                            Optional zoomToY As Double? = Nothing,
                            Optional xAxisIndex As Integer = 0,
                            Optional yAxisIndex As Integer = 0)
            Dim xAxis As Renderable.Axis = Settings.GetXAxis(xAxisIndex)
            Dim yAxis As Renderable.Axis = Settings.GetYAxis(yAxisIndex)

            If (Not xAxis.Dims.HasBeenSet) Then
                Settings.AxisAutoX(xAxis.AxisIndex)
            End If

            If (Not yAxis.Dims.HasBeenSet) Then
                Settings.AxisAutoY(yAxis.AxisIndex)
            End If

            xAxis.Dims.Zoom(xFrac, If(zoomToX, xAxis.Dims.Center))
            yAxis.Dims.Zoom(yFrac, If(zoomToY, yAxis.Dims.Center))
        End Sub

        ''' <summary>
        ''' Pan the primary X and Y axis without affecting zoom.
        ''' </summary>
        ''' <param name="dx">Horizontal distance to pan (in coordinate units).</param>
        ''' <param name="dy">Vertical distance to pan (in coordinate units).</param>
        ''' <param name="xAxisIndex">Index of the axis to act on.</param>
        ''' <param name="yAxisIndex">Index of the axis to act on.</param>
        Public Sub AxisPan(Optional dx As Double = 0, Optional dy As Double = 0,
                           Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0)
            Settings.AxisAutoUnsetAxes()
            Settings.GetXAxis(xAxisIndex).Dims.Pan(dx)
            Settings.GetYAxis(yAxisIndex).Dims.Pan(dy)
        End Sub

        ''' <summary>
        ''' Pan the primary X and Y axes to center the view on the given coordinate
        ''' </summary>
        ''' <param name="x">New horizontal center (coordinate units)</param>
        ''' <param name="y">New vertical center (in coordinate units)</param>
        ''' <param name="xAxisIndex">index of the axis to act on</param>
        ''' <param name="yAxisIndex">index of the axis to act on</param>
        Public Sub AxisPanCenter(Optional x As Double = 0, Optional y As Double = 0,
                                 Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0)
            Settings.AxisAutoUnsetAxes()
            Dim dx As Double = x - Settings.GetXAxis(xAxisIndex).Dims.Center
            Dim dy As Double = y - Settings.GetYAxis(yAxisIndex).Dims.Center
            AxisPan(dx, dy)
        End Sub

#End Region '/AXIS LIMITS: PAN AND ZOOM

#Region "OBSOLETE"

        <Obsolete("use AxisScaleLock()", True)>
        Public EqualAxis As Boolean

        <Obsolete("Use SetAxisLimits() and GetAxisLimits()", True)>
        Public Function AxisLimits(Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0) As AxisLimits
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinateX()", True)>
        Public Function CoordinateFromPixelX(pixelX As Single) As Double
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinateY()", True)>
        Public Function CoordinateFromPixelY(pixelY As Single) As Double
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinateX()", True)>
        Public Function CoordinateFromPixelX(pixelX As Double) As Double
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinateY()", True)>
        Public Function CoordinateFromPixelY(pixelY As Double) As Double
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinate(), GetCoordinateX() or GetCoordinateY()", True)>
        Public Function CoordinateFromPixel(pixelX As Integer, pixelY As Integer) As PointF
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinate(), GetCoordinateX() or GetCoordinateY()", True)>
        Public Function CoordinateFromPixel(pixelX As Single, pixelY As Single) As PointF
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinate(), GetCoordinateX() or GetCoordinateY()", True)>
        Public Function CoordinateFromPixel(pixelX As Double, pixelY As Double) As PointF
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinate(), GetCoordinateX() or GetCoordinateY()", True)>
        Public Function CoordinateFromPixel(pixel As Point) As PointF
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetCoordinate(), GetCoordinateX() or GetCoordinateY()", True)>
        Public Function CoordinateFromPixel(pixel As PointF) As PointF
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetPixel, GetPixelX(), or GetPixelY()", True)>
        Public Function CoordinateToPixel(location As PointF) As PointF
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetPixel, GetPixelX(), or GetPixelY()", True)>
        Public Function CoordinateToPixel(locationX As Double, locationY As Double) As PointF
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetPixelX()", True)>
        Public Function CoordinateToPixelX(locationX As Double) As Single
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetPixelY()", True)>
        Public Function CoordinateToPixelY(locationY As Double) As Single
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetAxisLimits() and SetAxisLimits()", True)>
        Public Function Axis(Optional x1 As Double? = Nothing, Optional x2 As Double? = Nothing,
                             Optional y1 As Double? = Nothing, Optional y2 As Double? = Nothing) As AxisLimits
            Throw New NotImplementedException()
        End Function

        <Obsolete("use GetAxisLimits() and SetAxisLimits()", True)>
        Public Sub Axis(axisLimits As Double(), Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0)
            Throw New NotImplementedException()
        End Sub

        <Obsolete("use GetAxisLimits() and SetAxisLimits()", True)>
        Public Function Axis(Optional x1 As Double? = Nothing, Optional x2 As Double? = Nothing,
                             Optional y1 As Double? = Nothing, Optional y2 As Double? = Nothing,
                             Optional a As Double? = Nothing) As Double()
            Return Nothing
        End Function

        <Obsolete("use GetAxisLimits() and SetAxisLimits()", True)>
        Public Function Axis(axisLimits As Double()) As Double()
            Return Nothing
        End Function

        <Obsolete("use GetAxisLimits() and SetAxisLimits()", True)>
        Public Sub Axis(limits As AxisLimits, Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0)
            Throw New NotImplementedException()
        End Sub

        <Obsolete("Use AxisAuto() or Margins()", True)>
        Public Function AutoAxis() As Double()
            Return Nothing
        End Function

        <Obsolete("Use AxisAuto() or Margins()", True)>
        Public Function AutoScale() As Double()
            Return Nothing
        End Function

        <Obsolete("Individual axes (e.g., XAxis and YAxis) have their own tick configuration methods", True)>
        Public Sub Ticks(Optional displayTicksX As Boolean? = Nothing, Optional displayTicksY As Boolean? = Nothing, Optional displayTicksXminor As Boolean? = Nothing, Optional displayTicksYminor As Boolean? = Nothing, Optional displayTickLabelsX As Boolean? = Nothing, Optional displayTickLabelsY As Boolean? = Nothing, Optional color As Color? = Nothing, Optional useMultiplierNotation As Boolean? = Nothing, Optional useOffsetNotation As Boolean? = Nothing, Optional useExponentialNotation As Boolean? = Nothing, Optional dateTimeX As Boolean? = Nothing, Optional dateTimeY As Boolean? = Nothing, Optional rulerModeX As Boolean? = Nothing, Optional rulerModeY As Boolean? = Nothing, Optional invertSignX As Boolean? = Nothing, Optional invertSignY As Boolean? = Nothing, Optional fontName As String = Nothing, Optional fontSize As Single? = Nothing, Optional xTickRotation As Single? = Nothing, Optional logScaleX As Boolean? = Nothing, Optional logScaleY As Boolean? = Nothing, Optional numericFormatStringX As String = Nothing, Optional numericFormatStringY As String = Nothing, Optional snapToNearestPixel As Boolean? = Nothing, Optional baseX As Integer? = Nothing, Optional baseY As Integer? = Nothing, Optional prefixX As String = Nothing, Optional prefixY As String = Nothing, Optional dateTimeFormatStringX As String = Nothing, Optional dateTimeFormatStringY As String = Nothing)
            Throw New NotImplementedException()
        End Sub

#End Region '/OBSOLETE

    End Class

End Namespace