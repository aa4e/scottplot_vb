Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A radial gauge chart is a graphical method of displaying scalar data in the form of 
    ''' a chart made of circular gauges so that each scalar is represented by each gauge.
    ''' </summary>
    ''' <remarks>
    ''' This plot type was inspired by MicroCharts:
    ''' https//github.com/dotnet-ad/Microcharts/blob/main/Sources/Microcharts/Charts/RadialGaugeChart.cs
    ''' </remarks>
    Public Class RadialGaugePlot
        Implements IPlottable

#Region "NESTED TYPES"

#End Region '/NESTED TYPES

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Describes labels drawn on each gauge.
        ''' </summary>
        Public ReadOnly Font As New ScottPlot.Drawing.Font() With {.Bold = True, .Color = Color.White}

        ''' <summary>
        ''' This array holds the original levels passed-in by the user. 
        ''' These levels are used to calculate radial gauge positions on every render.
        ''' </summary>
        Public ReadOnly Property Levels As Double()

        ''' <summary>
        ''' Number of gauges.
        ''' </summary>
        Public ReadOnly Property GaugeCount As Integer
            Get
                Return Levels.Length
            End Get
        End Property

        ''' <summary>
        ''' Maximum size (degrees) for the gauge.
        ''' 180 is a semicircle and 360 is a full circle.
        ''' </summary>
        Public Property MaximumAngle As Double = 360.0

        ''' <summary>
        ''' Controls whether the backgrounds of the gauges are full circles or stop at the maximum angle.
        ''' </summary>
        Public Property CircularBackground As Boolean = True

        ''' <summary>
        ''' Labels that appear in the legend for each gauge.
        ''' Number of labels must equal number of gauges.
        ''' May be null if gauges are not to appear in the legend.
        ''' </summary>
        Public Property Labels As String()

        ''' <summary>
        ''' Colors for each gauge. Number of colors must equal number of gauges.
        ''' </summary>
        Public Property Colors As Color()

        ''' <summary>
        ''' Describes how transparent the unfilled background of each gauge is (0 to 1).
        ''' The larger the number the darker the background becomes.
        ''' </summary>
        Public Property BackgroundTransparencyFraction As Double = 0.15

        ''' <summary>
        ''' Indicates whether gauges fill clockwise as levels increase.
        ''' If false, gauges will fill counter-clockwise (anti-clockwise).
        ''' </summary>
        Public Property Clockwise As Boolean = True

        ''' <summary>
        ''' Determines whether the gauges are drawn stacked (dafault value), sequentially, or as a single gauge (ressembling a pie plot).
        ''' </summary>
        Public Property GaugeMode As RadialGaugeMode = RadialGaugeMode.Stacked

        ''' <summary>
        ''' Controls whether gauges will be dwan inside-out (true) or outside-in (false)
        ''' </summary>
        Public Property OrderInsideOut As Boolean = True

        ''' <summary>
        ''' Defines where the gauge label is written on the gage as a fraction of its length.
        ''' Low values place the label near the base and high values place the label at its tip.
        ''' </summary>
        Public Property LabelPositionFraction As Double = 1.0

        ''' <summary>
        ''' Angle (degrees) at which the gauges start.
        ''' 270° for North (default value), 0° for East, 90° for South, 180° for West, etc.
        ''' Expected values in the range [0°-360°], otherwise unexpected side-effects might happen.
        ''' </summary>
        Public Property StartingAngle As Single = 270

        ''' <summary>
        ''' The empty space between gauges as a fraction of the gauge width.
        ''' </summary>
        Public Property SpaceFraction As Double = 0.5

        ''' <summary>
        ''' Size of the gague label text as a fraction of the gauge width.
        ''' </summary>
        Public Property FontSizeFraction As Double = 0.75

        ''' <summary>
        ''' Controls if value labels are shown inside the gauges.
        ''' </summary>
        Public Property ShowLevels As Boolean = True

        ''' <summary>
        ''' String formatter to use for converting gauge levels to text.
        ''' </summary>
        Public Property LevelTextFormat As String = "0.##"

        ''' <summary>
        ''' Style of the tip of the gauge.
        ''' </summary>
        Public Property EndCap As LineCap = LineCap.Triangle

        ''' <summary>
        ''' Style of the base of the gauge.
        ''' </summary>
        Public Property StartCap As LineCap = LineCap.Round

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(levels As Double(), colors As Color())
            Update(levels, colors)
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"RadialGaugePlot with {GaugeCount} gauges."
        End Function

        ''' <summary>
        ''' Replace gauge levels with new ones.
        ''' </summary>
        Public Sub Update(levels As Double(), Optional colors As Color() = Nothing)
            If (levels is Nothing) OrElse (levels.Length = 0) Then
                Throw New ArgumentException("Values must not be null or empty.")
            End If
            If (Me.Levels is Nothing) OrElse (levels.Length <> Me.Levels.Length) Then
                If (colors is Nothing) OrElse (colors.Length <> levels.Length) Then
                    Throw New ArgumentException("When changing the number of values a new colors array must be provided.")
                End If
                Me.Colors = New Color(colors.Length - 1) {}
                Array.Copy(colors, 0, Me.Colors, 0, colors.Length)
            End If
            _Levels = levels
        End Sub

        ''' <summary>
        ''' Calculate the rotational angles for each gauge from the original data values.
        ''' </summary>
        Private Shared Function GetGaugeAngles(values As Double(), angleStart As Double, angleRange As Double, clockwise As Boolean, mode As RadialGaugeMode) As Tuple(Of Double(), Double(), Double)
            Dim scaleMin As Double = Math.Min(0, values.Min())
            Dim scaleMax As Double = values.Max(Function(x) Math.Abs(x))

            If (mode = RadialGaugeMode.Sequential OrElse mode = RadialGaugeMode.SingleGauge) Then
                scaleMax = values.Sum(Function(x) Math.Abs(x))
                scaleMin = 0
            End If

            Dim scaleRange As Double = scaleMax - scaleMin

            Dim gaugeCount As Integer = values.Length
            Dim startAngles As Double() = New Double(gaugeCount - 1) {}
            Dim sweepAngles As Double() = New Double(gaugeCount - 1) {}

            angleStart = RadialGauge.ReduceAngle(angleStart)
            Dim angleSum As Double = angleStart
            For i As Integer = 0 To gaugeCount - 1
                Dim angleSwept As Double = 0
                If (scaleRange > 0) Then
                    angleSwept = angleRange * values(i) / scaleRange
                End If

                If (Not clockwise) Then
                    angleSwept *= -1
                End If

                Dim initialAngle As Double = If(mode = RadialGaugeMode.Stacked, angleStart, angleSum)
                angleSum += angleSwept

                startAngles(i) = initialAngle
                sweepAngles(i) = angleSwept
            Next

            Dim backOffset As Double = angleRange * scaleMin / scaleRange
            If (Not clockwise) Then
                backOffset *= -1
            End If

            Dim backStartAngle As Double = angleStart + backOffset

            Return New Tuple(Of Double(), Double(), Double)(startAngles, sweepAngles, backStartAngle)
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If (Colors.Length <> GaugeCount) Then
                Throw New InvalidOperationException("Colors must be an array with length equal to number of values.")
            End If
            If (Labels IsNot Nothing) AndAlso (Labels.Length <> GaugeCount) Then
                Throw New InvalidOperationException("If Labels is not null, it must be the same length as the number of values.")
            End If
            If (MaximumAngle < 0) OrElse (MaximumAngle > 360) Then
                Throw New InvalidOperationException("MaximumAngle must be [0-360].")
            End If
            If (LabelPositionFraction < 0) OrElse (LabelPositionFraction > 1) Then
                Throw New InvalidOperationException("LabelPositionFraction must be a value from 0 to 1.")
            End If
            If (SpaceFraction < 0) OrElse (SpaceFraction > 1) Then
                Throw New InvalidOperationException("SpaceFraction must be from 0 to 1.")
            End If
        End Sub

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            If (Labels is Nothing) Then
                Return {}
            End If
            Dim legendItems As New List(Of LegendItem)
            For i As Integer = 0 To Me.Labels.Length - 1
                Dim leg As New LegendItem(Me) With {
                    .Label = Labels(i),
                    .Color = Colors(i),
                    .LineWidth = 10,
                    .MarkerShape = MarkerShape.None}
                legendItems.Add(leg)
            Next
            Return legendItems.ToArray()
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Dim radius As Double = GaugeCount / 4
            Return New AxisLimits(-radius, radius, -radius, radius)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render

            ValidateData(False)

            Dim gaugeAngles = RadialGaugePlot.GetGaugeAngles(Levels, StartingAngle, MaximumAngle, Clockwise, GaugeMode)
            Dim startAngles As Double() = gaugeAngles.Item1
            Dim sweepAngles As Double() = gaugeAngles.Item2
            Dim startingAngleBackGauges As Double = gaugeAngles.Item3

            Dim centerPixel As New PointF(dims.GetPixelX(0), dims.GetPixelY(0))

            Dim pxPerUnit As Double = Math.Min(dims.PxPerUnitX, dims.PxPerUnitY)
            Dim gaugeWidthPx As Single = CSng(pxPerUnit / GaugeCount * SpaceFraction + 1)
            Dim radiusPixels As Single = CSng(gaugeWidthPx * (SpaceFraction + 1))

            Dim backgroundAlpha As Integer = CInt(255 * BackgroundTransparencyFraction)
            backgroundAlpha = Math.Max(0, backgroundAlpha)
            backgroundAlpha = Math.Min(255, backgroundAlpha)

            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality)
                For i As Integer = 0 To GaugeCount - 1
                    Dim index As Integer
                    Dim position As Integer
                    If (GaugeMode = RadialGaugeMode.SingleGauge) Then
                        index = GaugeCount - i - 1
                        position = GaugeCount
                    Else
                        index = i
                        position = If(OrderInsideOut, (i + 1), (GaugeCount - i))
                    End If

                    Dim gauge As New RadialGauge() With {
                        .MaximumSizeAngle = MaximumAngle,
                        .StartAngle = startAngles(index),
                        .SweepAngle = sweepAngles(index),
                        .Color = Colors(index),
                        .BackgroundColor = Color.FromArgb(backgroundAlpha, Colors(index)),
                        .Width = CDbl(gaugeWidthPx),
                        .CircularBackground = CircularBackground,
                        .Clockwise = Clockwise,
                        .BackStartAngle = startingAngleBackGauges,
                        .StartCap = StartCap,
                        .EndCap = EndCap,
                        .Mode = GaugeMode,
                        .Font = Font,
                        .FontSizeFraction = FontSizeFraction,
                        .Label = Levels(index).ToString(LevelTextFormat),
                        .LabelPositionFraction = LabelPositionFraction,
                        .ShowLabels = ShowLevels
                    }

                    Dim radiusPx As Single = position * radiusPixels
                    gauge.Render(gfx, dims, centerPixel, radiusPx)
                Next
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace