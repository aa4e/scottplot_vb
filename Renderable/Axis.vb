Imports System.Diagnostics
Imports System.Drawing
Imports System.Globalization

Namespace ScottPlot.Renderable

    ''' <summary>
    ''' An Axis stores dimensions (axis limits and pixel/unit conversion methods) 
    ''' and can render itself including axis label, tick marks, tick labels, and grid lines.
    ''' </summary>
    ''' <remarks>
    ''' The Axis module seeks to provide a simple facade to a lot of complex logic.
    ''' Axes have many functions
    ''' - Unit/Pixel conversions;
    ''' - Configuring axis limits and boundaries;
    ''' - Axis labels (XLabel, YLabel, Title, etc);
    ''' - Adding multiple axes;
    ''' - Grid lines;
    ''' - Tick marks;
    ''' - Tick labels.
    ''' </remarks>
    Public Class Axis
        Implements IRenderable

#Region "PROPS"

        ''' <summary>
        ''' Axis dimensions and methods for pixel/unit conversions.
        ''' </summary>
        Public ReadOnly Dims As New AxisDimensions()

        ''' <summary>
        ''' Plottables with this axis index will use pixel/unit conversions from this axis.
        ''' </summary>
        Public AxisIndex As Integer = 0

        Public Property IsVisible As Boolean = True Implements ScottPlot.Renderable.IRenderable.IsVisible

        'private renderable components
        Private AxisLabel As New AxisLabel()
        Private AxisTicks As New AxisTicks()
        Private AxisLine As New AxisLine()

        Public Property Edge As Edge
            Get
                Return _Edge
            End Get
            Set(value As Edge)
                _Edge = value
                AxisLine.Edge = value
                AxisLabel.Edge = value
                AxisTicks.Edge = value
                Dim isVertical As Boolean = (value = Edge.Left) OrElse (value = Edge.Right)
                AxisTicks.TickCollection.Orientation = If(isVertical, ScottPlot.Ticks.AxisOrientation.Vertical, ScottPlot.Ticks.AxisOrientation.Horizontal)
                Dims.IsInverted = isVertical
            End Set
        End Property
        Private _Edge As Edge = Edge.Left

        Public ReadOnly Property IsHorizontal As Boolean
            Get
                Return (Edge = Edge.Top) OrElse (Edge = Edge.Bottom)
            End Get
        End Property

        Public ReadOnly Property IsVertical As Boolean
            Get
                Return (Edge = Edge.Left) OrElse (Edge = Edge.Right)
            End Get
        End Property

        ''' <summary>
        ''' How large this axis is.
        ''' </summary>
        Private PixelSize As Single

        ''' <summary>
        ''' true if axes are hidden.
        ''' </summary>
        Private Collapsed As Boolean = False

        ''' <summary>
        ''' Distance from the data area.
        ''' </summary>
        Public Property PixelOffset As Single
            Get
                Return _PixelOffset
            End Get
            Private Set(value As Single)
                _PixelOffset = value
            End Set
        End Property
        Private _PixelOffset As Single = 0

        'also defined in ResetLayout()
        Private PixelSizeMinimum As Single = 5
        Private PixelSizeMaximum As Single = Single.PositiveInfinity
        Private PixelSizePadding As Single = 3

#End Region '/PROPS

#Region "METHODS"

        ''' <summary>
        ''' Return configuration objects to allow deep customization of axis settings. 
        ''' WARNING: This API may not be stable across future versions.
        ''' </summary>
        Public Function GetSettings(Optional showWarning As Boolean = True) As Tuple(Of AxisLabel, AxisTicks, AxisLine)
            If showWarning Then
                System.Diagnostics.Debug.WriteLine(
                    "WARNING: GetSettings() is only for development and testing. " &
                    "Not all features may be fully implemented. " &
                    "Its API may not be stable across future versions.")
            End If
            Return New Tuple(Of AxisLabel, AxisTicks, AxisLine)(AxisLabel, AxisTicks, AxisLine)
        End Function

        ''' <summary>
        ''' Reset axis padding to the default values.
        ''' </summary>
        Public Sub ResetLayout()
            PixelSizeMinimum = 5
            PixelSizeMaximum = Single.PositiveInfinity
            PixelSizePadding = 3
        End Sub

        ''' <summary>
        ''' Define the minimum and maximum limits for the pixel size of this axis.
        ''' </summary>
        Public Sub SetSizeLimit(Optional min As Single? = Nothing, Optional max As Single? = Nothing, Optional pad As Single? = Nothing)
            PixelSizeMinimum = If(min, PixelSizeMinimum)
            PixelSizeMaximum = If(max, PixelSizeMaximum)
            PixelSizePadding = If(pad, PixelSizePadding)
        End Sub

        ''' <summary>
        ''' Size this axis to an exact number of pixels.
        ''' </summary>
        Public Sub SetSizeLimit(px As Single)
            SetSizeLimit(px, px, 0)
        End Sub

        ''' <summary>
        ''' Define how many pixels away from the data area this axis will be.
        ''' TightenLayout() populates this value (based on other PixelSize values) to stack axes beside each other.
        ''' </summary>
        ''' <param name="pixels"></param>
        Public Sub SetOffset(pixels As Single)
            PixelOffset = pixels
        End Sub

        ''' <summary>
        ''' Returns the number of pixels occupied by this axis.
        ''' </summary>
        Public Function GetSize() As Single
            If (Not IsVisible) OrElse Collapsed Then
                Return 0
            End If
            Return (PixelSize + PixelSizePadding)
        End Function

        Public Overrides Function ToString() As String
            Return $"{Edge} axis from {Dims.Min} to {Dims.Max}."
        End Function

        ''' <summary>
        ''' Use the latest configuration (size, font settings, axis limits) to determine tick mark positions.
        ''' </summary>
        Public Sub RecalculateTickPositions(dims As PlotDimensions)
            Try
                AxisTicks.TickCollection.Recalculate(dims, AxisTicks.TickLabelFont)
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try
        End Sub

        ''' <summary>
        ''' Render all components of this axis onto the given Bitmap.
        ''' </summary>
        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Renderable.IRenderable.Render
            If IsVisible Then
                AxisLabel.PixelSizePadding = PixelSizePadding
                AxisTicks.PixelOffset = PixelOffset
                AxisLabel.PixelOffset = PixelOffset
                AxisLabel.PixelSize = PixelSize
                AxisLine.PixelOffset = PixelOffset
                Using Drawing.GDI.Graphics(bmp, dims, lowQuality, False)
                    AxisTicks.Render(dims, bmp, lowQuality)
                    AxisLabel.Render(dims, bmp, lowQuality)
                    AxisLine.Render(dims, bmp, AxisTicks.SnapPx OrElse lowQuality)
                End Using
            End If
        End Sub

        ''' <summary>
        ''' DateTime format assumes axis represents DateTime.ToOATime() units and displays tick labels accordingly.
        ''' </summary>
        Public Sub DateTimeFormat(enable As Boolean)
            AxisTicks.TickCollection.LabelFormat = If(enable,
                ScottPlot.Ticks.TickLabelFormat.DateTime,
                ScottPlot.Ticks.TickLabelFormat.Numeric)
        End Sub

        ''' <summary>
        ''' Configure the label of this axis.
        ''' </summary>
        Public Function Label(Optional labl As String = Nothing,
                              Optional color As Color? = Nothing,
                              Optional size As Single? = Nothing,
                              Optional bold As Boolean? = Nothing,
                              Optional fontName As String = Nothing) As String
            With AxisLabel
                .IsVisible = True
                .ImageLabel = Nothing
                .Label = If(labl, AxisLabel.Label)
                .Font.Color = If(color, AxisLabel.Font.Color)
                .Font.Size = If(size, AxisLabel.Font.Size)
                .Font.Bold = If(bold, AxisLabel.Font.Bold)
                .Font.Name = If(fontName, AxisLabel.Font.Name)
                Return .Label
            End With
        End Function

        ''' <summary>
        ''' Display a custom image as the axis label instead of text.
        ''' </summary>
        ''' <param name="img">The image to display where the label should go.</param>
        ''' <param name="padInside">Pixels of padding between the inner image edge and the data area.</param>
        ''' <param name="padOutside">Pixels of padding between the outer image edge and the figure edge.</param>
        Public Sub ImageLabel(img As Bitmap, Optional padInside As Single = 5, Optional padOutside As Single = 5)
            IsVisible = True
            AxisLabel.ImageLabel = img
            AxisLabel.ImagePaddingToDataArea = padInside
            AxisLabel.ImagePaddingToFigureEdge = padOutside
        End Sub

        ''' <summary>
        ''' Set color of every component of this axis (label, line, tick marks, and tick labels).
        ''' </summary>
        ''' <param name="color"></param>
        Public Sub Color(color As Color)
            Label(color:=color)
            TickLabelStyle(color:=color)
            AxisTicks.MajorTickColor = color
            AxisTicks.MinorTickColor = color
            AxisLine.Color = color
        End Sub

        ''' <summary>
        ''' Use a custom function to generate tick label strings.
        ''' </summary>
        Public Sub TickLabelFormat(tickFormatter As Func(Of Double, String))
            With AxisTicks.TickCollection
                .ManualTickFormatter = tickFormatter
                'delete existing tick formatter function:
                .NumericFormatString = Nothing
                .DateTimeFormatString = Nothing
                .LabelFormat = ScottPlot.Ticks.TickLabelFormat.Numeric
            End With
        End Sub

        ''' <summary>
        ''' Manually define the string format to use for translating tick positions to tick labels.
        ''' </summary>
        Public Sub TickLabelFormat(format As String, dateTimeFormat As Boolean)
            AxisTicks.TickCollection.ManualTickFormatter = Nothing 'delete existing tick formatter function
            If dateTimeFormat Then
                AxisTicks.TickCollection.DateTimeFormatString = format
                Me.DateTimeFormat(True)
            Else
                AxisTicks.TickCollection.NumericFormatString = format
                Me.DateTimeFormat(False)
            End If
        End Sub

        ''' <summary>
        ''' Manually define the string format to use for translating exponential part of a number to corner label.
        ''' </summary>
        Public Sub CornerLabelFormat(format As String)
            AxisTicks.TickCollection.CornerLabelFormat = format
        End Sub

        ''' <summary>
        ''' Customize string settings for the tick labels.
        ''' </summary>
        Public Sub TickLabelNotation(Optional multiplier As Boolean? = Nothing,
                                     Optional offset As Boolean? = Nothing,
                                     Optional exponential As Boolean? = Nothing,
                                     Optional invertSign As Boolean? = Nothing,
                                     Optional radix As Integer? = Nothing,
                                     Optional prefix As String = Nothing)
            With AxisTicks.TickCollection
                .UseMultiplierNotation = If(multiplier, .UseMultiplierNotation)
                .UseOffsetNotation = If(offset, .UseOffsetNotation)
                .UseExponentialNotation = If(exponential, .UseExponentialNotation)
                .LabelUsingInvertedSign = If(invertSign, .LabelUsingInvertedSign)
                .Radix = If(radix, .Radix)
                .Prefix = If(prefix, .Prefix)
            End With
        End Sub

        ''' <summary>
        ''' Define a manual spacing between major ticks (and major grid lines).
        ''' </summary>
        Public Sub ManualTickSpacing(manualSpacing As Double)
            'TODO: cutt X and Y out of this
            AxisTicks.TickCollection.ManualSpacingX = manualSpacing
            AxisTicks.TickCollection.ManualSpacingY = manualSpacing
        End Sub

        ''' <summary>
        ''' Define a manual spacing between major ticks (and major grid lines) for axes configured to display using DateTime format.
        ''' </summary>
        Public Sub ManualTickSpacing(manualSpacing As Double, manualSpacingDateTimeUnit As Ticks.DateTimeUnit)
            ManualTickSpacing(manualSpacing)
            AxisTicks.TickCollection.ManualDateTimeSpacingUnitX = manualSpacingDateTimeUnit
        End Sub

        ''' <summary>
        ''' Manually define major tick (and grid) positions and labels.
        ''' </summary>
        Public Sub ManualTickPositions(positions As Double(), labels As String())
            If (positions.Length <> labels.Length) Then
                Throw New ArgumentException("Positions must have the same length as labels.")
            End If
            AxisTicks.TickCollection.ManualTickPositions = positions
            AxisTicks.TickCollection.ManualTickLabels = labels
        End Sub

        ''' <summary>
        ''' Reset previously defined manual tick positions and revert to default automatic tick placement.
        ''' </summary>
        Public Sub AutomaticTickPositions()
            With AxisTicks.TickCollection
                .ManualTickPositions = Nothing
                .ManualTickLabels = Nothing
                .AdditionalTickPositions = Nothing
                .AdditionalTickLabels = Nothing
            End With
        End Sub

        ''' <summary>
        ''' Reset previously defined manual tick positions and revert to default automatic tick placement.
        ''' The provided tick positions and labels will be displayed in addition to the automatic ticks.
        ''' </summary>
        Public Sub AutomaticTickPositions(additionalTickPositions As Double(), additionalTickLabels As String())
            If (additionalTickPositions is Nothing) Then
                Throw New ArgumentNullException("additionalTickPositions")
            End If
            If (additionalTickLabels is Nothing) Then
                Throw New ArgumentNullException("additionalTickLabels")
            End If
            If (additionalTickLabels.Length <> additionalTickLabels.Length) Then
                Throw New ArgumentException("Tick positions and labels must be equal length.")
            End If
            With AxisTicks.TickCollection
                .ManualTickPositions = Nothing
                .ManualTickLabels = Nothing
                .AdditionalTickPositions = additionalTickPositions
                .AdditionalTickLabels = additionalTickLabels
            End With
        End Sub

        ''' <summary>
        ''' Ruler mode draws long tick marks and offsets tick labels for a ruler appearance.
        ''' </summary>
        Public Sub RulerMode(enable As Boolean)
            AxisTicks.RulerMode = enable
        End Sub

        ''' <summary>
        ''' Enable this to snap major ticks (and grid lines) to the nearest pixel to avoid anti-aliasing artifacts.
        ''' </summary>
        Public Sub PixelSnap(enable As Boolean)
            AxisTicks.SnapPx = enable
        End Sub

        ''' <summary>
        ''' Apply the same color to major and minor tick marks.
        ''' </summary>
        Public Sub TickMarkColor(color As Color)
            AxisTicks.MajorTickColor = color
            AxisTicks.MinorTickColor = color
        End Sub

        ''' <summary>
        ''' Set colors for major and minor tick marks.
        ''' </summary>
        Public Sub TickMarkColor(majorColor As Color, minorColor As Color)
            AxisTicks.MajorTickColor = majorColor
            AxisTicks.MinorTickColor = minorColor
        End Sub

        ''' <summary>
        ''' Control whether tick marks point outward or inward.
        ''' </summary>
        Public Sub TickMarkDirection(outward As Boolean)
            AxisTicks.TicksExtendOutward = outward
        End Sub

        ''' <summary>
        ''' Set the culture to use for unit-to-string tick mark conversion.
        ''' </summary>
        Public Sub SetCulture(culture As CultureInfo)
            AxisTicks.TickCollection.Culture = culture
        End Sub

        ''' <summary>
        ''' Manually define culture to use for unit-to-string tick mark conversion.
        ''' </summary>
        Public Sub SetCulture(Optional shortDatePattern As String = Nothing,
                              Optional decimalSeparator As String = Nothing,
                              Optional numberGroupSeparator As String = Nothing,
                              Optional decimalDigits As Integer? = Nothing,
                              Optional numberNegativePattern As Integer? = Nothing,
                              Optional numberGroupSizes As Integer() = Nothing)
            AxisTicks.TickCollection.SetCulture(shortDatePattern,
                                                decimalSeparator,
                                                numberGroupSeparator,
                                                decimalDigits,
                                                numberNegativePattern,
                                                numberGroupSizes)
        End Sub

        ''' <summary>
        ''' Customize styling of the tick labels.
        ''' </summary>
        Public Sub TickLabelStyle(Optional color As Color? = Nothing,
                                  Optional fontName As String = Nothing,
                                  Optional fontSize As Single? = Nothing,
                                  Optional fontBold As Boolean? = Nothing,
                                  Optional rotation As Single? = Nothing)
            With AxisTicks
                .TickLabelFont.Color = If(color, .TickLabelFont.Color)
                .TickLabelFont.Name = If(fontName, .TickLabelFont.Name)
                .TickLabelFont.Size = If(fontSize, .TickLabelFont.Size)
                .TickLabelFont.Bold = If(fontBold, .TickLabelFont.Bold)
                .TickLabelRotation = If(rotation, .TickLabelRotation)
            End With
        End Sub

        ''' <summary>
        ''' Customize styling of the label (without changing its content).
        ''' </summary>
        Public Sub LabelStyle(Optional color As Color? = Nothing,
                              Optional fontName As String = Nothing,
                              Optional fontSize As Single? = Nothing,
                              Optional rotation As Single? = Nothing)
            With AxisLabel
                .Font.Color = If(color, .Font.Color)
                .Font.Name = If(fontName, .Font.Name)
                .Font.Size = If(fontSize, .Font.Size)
                .Font.Rotation = If(rotation, .Font.Rotation)
            End With
        End Sub

        ''' <summary>
        ''' Set visibility of all ticks.
        ''' </summary>
        Public Sub Ticks(enable As Boolean)
            AxisTicks.MajorTickVisible = enable
            AxisTicks.TickLabelVisible = enable
            AxisTicks.MinorTickVisible = enable
        End Sub

        ''' <summary>
        ''' Set visibility of individual tick components.
        ''' </summary>
        Public Sub Ticks(major As Boolean, minor As Boolean, Optional majorLabels As Boolean = True)
            AxisTicks.MajorTickVisible = major
            AxisTicks.TickLabelVisible = majorLabels
            AxisTicks.MinorTickVisible = minor
        End Sub

        ''' <summary>
        ''' This value defines the packing density of tick labels.
        ''' A density of 1.0 means labels fit tightly based on measured maximum label size.
        ''' Higher densities place more ticks but tick labels may oberlap.
        ''' </summary>
        Public Sub TickDensity(Optional ratio As Double = 1)
            AxisTicks.TickCollection.TickDensity = CSng(ratio)
        End Sub

        ''' <summary>
        ''' Define the smallest distance between major ticks, grid lines, and tick labels in coordinate units.
        ''' This only works for numeric tick systems (DateTime ticks are not supported).
        ''' </summary>
        Public Sub MinimumTickSpacing(spacing As Double)
            AxisTicks.TickCollection.MinimumTickSpacing = spacing
        End Sub

        ''' <summary>
        ''' Sets whether minor ticks are evenly spaced Or log-distributed between major tick positions.
        ''' </summary>
        ''' <param name="enable">If true, minor tick marks will be logarithmically distributed.</param>
        ''' <param name="roundMajorTicks">If true, log-scaled ticks will only show as even powers of ten.</param>
        ''' <param name="minorTickCount">This many minor ticks will be placed between each major tick.</param>
        Public Sub MinorLogScale(enable As Boolean,
                                 Optional roundMajorTicks As Boolean = True,
                                 Optional minorTickCount As Integer = 10)
            With AxisTicks.TickCollection
                If enable Then
                    .MinorTickDistribution = ScottPlot.Ticks.MinorTickDistribution.Log
                    .IntegerPositionsOnly = roundMajorTicks
                    .LogScaleMinorTickCount = minorTickCount
                Else
                    .MinorTickDistribution = ScottPlot.Ticks.MinorTickDistribution.Even
                    .IntegerPositionsOnly = False
                End If
            End With
        End Sub

        ''' <summary>
        ''' Configure the line drawn along the edge of the axis.
        ''' </summary>
        Public Sub Line(Optional visible As Boolean? = Nothing,
                        Optional color As Color? = Nothing,
                        Optional width As Single? = Nothing)
            AxisLine.IsVisible = If(visible, AxisLine.IsVisible)
            AxisLine.Color = If(color, AxisLine.Color)
            AxisLine.Width = If(width, AxisLine.Width)
        End Sub

        ''' <summary>
        ''' Set the minimum size and padding of the axis.
        ''' </summary>
        Public Sub Layout(Optional padding As Single? = Nothing,
                          Optional minimumSize As Single? = Nothing,
                          Optional maximumSize As Single? = Nothing)
            PixelSizePadding = If(padding, PixelSizePadding)
            PixelSizeMinimum = If(minimumSize, PixelSizeMinimum)
            PixelSizeMaximum = If(maximumSize, PixelSizeMaximum)
        End Sub

        ''' <summary>
        ''' Configure visibility and styling of the major grid.
        ''' </summary>
        Public Sub MajorGrid(Optional enable As Boolean? = Nothing,
                             Optional color As Color? = Nothing,
                             Optional lineWidth As Single? = Nothing,
                             Optional lineStyle As LineStyle? = Nothing)
            AxisTicks.MajorGridVisible = If(enable, AxisTicks.MajorGridVisible)
            AxisTicks.MajorGridColor = If(color, AxisTicks.MajorGridColor)
            AxisTicks.MajorGridWidth = If(lineWidth, AxisTicks.MajorGridWidth)
            AxisTicks.MajorGridStyle = If(lineStyle, AxisTicks.MajorGridStyle)
        End Sub

        ''' <summary>
        ''' Configure visibility and styling of the minor grid.
        ''' </summary>
        Public Sub MinorGrid(Optional enable As Boolean? = Nothing,
                             Optional color As Color? = Nothing,
                             Optional lineWidth As Single? = Nothing,
                             Optional lineStyle As LineStyle? = Nothing,
                             Optional logScale As Boolean? = Nothing)
            AxisTicks.MinorGridVisible = If(enable, AxisTicks.MinorGridVisible)
            AxisTicks.MinorGridColor = If(color, AxisTicks.MinorGridColor)
            AxisTicks.MinorGridWidth = If(lineWidth, AxisTicks.MinorGridWidth)
            AxisTicks.MinorGridStyle = If(lineStyle, AxisTicks.MinorGridStyle)
            If (logScale IsNot Nothing) Then
                AxisTicks.TickCollection.MinorTickDistribution = If(logScale.Value,
                    ScottPlot.Ticks.MinorTickDistribution.Log,
                    ScottPlot.Ticks.MinorTickDistribution.Even)
            End If
        End Sub

        ''' <summary>
        ''' Hide this axis by forcing its size to always be zero.
        ''' </summary>
        Public Sub Hide(Optional hidden As Boolean = True)
            AxisTicks.MajorTickVisible = Not hidden
            AxisTicks.MinorTickVisible = Not hidden
            AxisTicks.TickLabelVisible = Not hidden
            AxisLine.IsVisible = Not hidden
            Collapsed = hidden
        End Sub

        ''' <summary>
        ''' Set visibility for major tick grid lines.
        ''' </summary>
        Public Sub Grid(enable As Boolean)
            AxisTicks.MajorGridVisible = enable
        End Sub

        ''' <summary>
        ''' Set pixel size based on the latest axis label, tick marks, and tick label.
        ''' </summary>
        Public Sub RecalculateAxisSize()
            PixelSize = 0
            If (Not Collapsed) Then
                Using tickFont = Drawing.GDI.Font(AxisTicks.TickLabelFont),
                    titleFont = Drawing.GDI.Font(AxisLabel.Font)

                    If AxisLabel.IsVisible Then
                        PixelSize += AxisLabel.Measure().Height
                    End If

                    If AxisTicks.TickLabelVisible Then
                        'determine how many pixels the largest tick label occupies
                        Dim maxHeight As Single = AxisTicks.TickCollection.LargestLabelHeight
                        Dim maxWidth As Single = AxisTicks.TickCollection.LargestLabelWidth * 1.2F

                        'calculate the width and height of the rotated label
                        Dim largerEdgeLength As Single = Math.Max(maxWidth, maxHeight)
                        Dim shorterEdgeLength As Single = Math.Min(maxWidth, maxHeight)
                        Dim differenceInEdgeLengths As Single = largerEdgeLength - shorterEdgeLength
                        Dim radians As Double = AxisTicks.TickLabelRotation * Math.PI / 180
                        Dim fraction As Double = If(IsHorizontal, Math.Sin(radians), Math.Cos(radians))
                        Dim rotatedSize As Double = shorterEdgeLength + differenceInEdgeLengths * fraction

                        'add the rotated label size to the size of this axis
                        PixelSize += CSng(rotatedSize)
                    End If

                    If AxisTicks.MajorTickVisible Then
                        PixelSize += AxisTicks.MajorTickLength
                    End If

                    PixelSize = Math.Max(PixelSize, PixelSizeMinimum)
                    PixelSize = Math.Min(PixelSize, PixelSizeMaximum)
                    PixelSize += PixelSizePadding
                End Using
            End If
        End Sub

        ''' <summary>
        ''' Lock min/max limits so it cannot be changed (until it's unlocked).
        ''' </summary>
        ''' <param name="locked"></param>
        Public Sub LockLimits(Optional locked As Boolean = True)
            Dims.LockLimits(locked)
        End Sub

        ''' <summary>
        ''' Return the ticks displayed in the previous render.
        ''' </summary>
        Public Function GetTicks(Optional min As Double = Double.NegativeInfinity, Optional max As Double = Double.PositiveInfinity) As Ticks.Tick()
            Return AxisTicks.TickCollection.GetTicks(min, max)
        End Function

        ''' <summary>
        ''' Configure how tick label measurement is performed when calculating ideal tick density.
        ''' </summary>
        Public Sub TickMeasurement(manual As Boolean)
            AxisTicks.TickCollection.MeasureStringManually = manual
        End Sub

#End Region '/METHODS

    End Class

End Namespace