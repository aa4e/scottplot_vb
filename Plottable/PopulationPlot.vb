Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    '''  Statistics. Population plots are designed to show collections of data.
    '''  A Statistics. Population is a 1D array of values, and statistics are calculated automatically.
    '''  Populations can be displayed as bar plots, box plots, or scatter plots.
    '''  Public methods, fields, and properties allow extensive customization.
    '''  This plottable supports higher-order grouping (groups of groups).
    ''' </summary>
    Public Class PopulationPlot
        Implements IPlottable

#Region "NESTED TYPES"

        Public Enum DisplayItems
            BoxOnly
            BoxAndScatter
            ScatterAndBox
            ScatterOnBox
            ScatterOnly
        End Enum

        Public Enum BoxStyle
            BarMeanStDev
            BarMeanStdErr
            BoxMeanStdevStderr
            BoxMedianQuartileOutlier
            MeanAndStdev
            MeanAndStderr
        End Enum

        Public Enum Position
            Hide
            Center
            Left
            Right
        End Enum

        Public Enum BoxFormat
            StdevStderrMean
            OutlierQuartileMedian
        End Enum

        Public Enum HorizontalAlignment
            Left
            Center
            Right
        End Enum

#End Region '/NESTED TYPES

#Region "PROPS, FIELDS"

        Public ReadOnly MultiSeries As Statistics.PopulationMultiSeries

        Public ReadOnly Property GroupCount As Integer
            Get
                Return MultiSeries.GroupCount
            End Get
        End Property

        Public ReadOnly Property SeriesCount As Integer
            Get
                Return MultiSeries.SeriesCount
            End Get
        End Property

        Public ReadOnly Property SeriesLabels As String()
            Get
                Return MultiSeries.SeriesLabels
            End Get
        End Property

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property DistributionCurve As Boolean = True
        Public Property DistributionCurveLineStyle As LineStyle = LineStyle.Solid
        Public Property DistributionCurveColor As Color = Color.Black
        Public Property ScatterOutlineColor As Color = Color.Black
        Public Property DataFormat As PopulationPlot.DisplayItems = PopulationPlot.DisplayItems.BoxAndScatter
        Public Property DataBoxStyle As PopulationPlot.BoxStyle = PopulationPlot.BoxStyle.BoxMedianQuartileOutlier

        Public ReadOnly Property PointCount As Integer
            Get
                Dim ptsCount As Integer = 0
                Dim multiSeries As Statistics.PopulationSeries() = Me.MultiSeries.MultiSeries
                For i As Integer = 0 To multiSeries.Length - 1
                    For Each population As Statistics.Population In multiSeries(i).Populations
                        ptsCount += population.Count
                    Next
                Next
                Return ptsCount
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(groupedSeries As Statistics.PopulationMultiSeries)
            MultiSeries = groupedSeries
        End Sub

        Public Sub New(populations As Statistics.Population(), Optional label As String = Nothing, Optional color As System.Drawing.Color? = Nothing)
            Dim ps As New Statistics.PopulationSeries(populations, label, If(color, System.Drawing.Color.LightGray))
            MultiSeries = New Statistics.PopulationMultiSeries(New Statistics.PopulationSeries() {ps})
        End Sub

        Public Sub New(populationSeries As Statistics.PopulationSeries)
            MultiSeries = New Statistics.PopulationMultiSeries(New Statistics.PopulationSeries() {populationSeries})
        End Sub

        Public Sub New(population As Statistics.Population, Optional label As String = Nothing, Optional color As System.Drawing.Color? = Nothing)
            Dim ps As New Statistics.PopulationSeries(New Statistics.Population() {population}, label, If(color, System.Drawing.Color.LightGray))
            MultiSeries = New Statistics.PopulationMultiSeries(New Statistics.PopulationSeries() {ps})
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"PlottableSeries with {MultiSeries.GroupCount} groups, {MultiSeries.SeriesCount} series, and {PointCount} total points."
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If (MultiSeries is Nothing) Then
                Throw New InvalidOperationException("Population multi-series cannot be null.")
            End If
        End Sub

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return MultiSeries.MultiSeries.Select(Function(x As Statistics.PopulationSeries)
                                                      Return New LegendItem(Me) With {.Label = x.SeriesLabel, .Color = x.Color, .LineWidth = 10}
                                                  End Function).ToArray()
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Dim minValue As Double = Double.PositiveInfinity
            Dim maxValue As Double = Double.NegativeInfinity

            For Each series In Me.MultiSeries.MultiSeries
                For Each population As Statistics.Population In series.Populations
                    minValue = Math.Min(minValue, population.Min)
                    minValue = Math.Min(minValue, population.Minus3stDev)
                    maxValue = Math.Max(maxValue, population.Max)
                    maxValue = Math.Max(maxValue, population.Plus3stDev)
                Next
            Next

            Dim positionMin As Double = 0
            Dim positionMax As Double = MultiSeries.GroupCount - 1

            'pad slightly
            positionMin -= 0.5
            positionMax += 0.5

            Return New AxisLimits(positionMin, positionMax, minValue, maxValue)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Dim rand As Random = New Random(0)
            Dim groupWidth As Double = 0.8
            Dim popWidth As Double = groupWidth / SeriesCount

            For seriesIndex As Integer = 0 To SeriesCount - 1
                For groupIndex As Integer = 0 To GroupCount - 1

                    Dim series As Statistics.PopulationSeries = MultiSeries.MultiSeries(seriesIndex)
                    Dim population As Statistics.Population = series.Populations(groupIndex)
                    Dim groupLeft As Double = groupIndex - groupWidth / 2
                    Dim popLeft As Double = groupLeft + popWidth * seriesIndex

                    Dim boxPos As PopulationPlot.Position
                    Dim scatterPos As PopulationPlot.Position
                    Dim boxAlpha As Byte = 0

                    Select Case DataFormat
                        Case PopulationPlot.DisplayItems.BoxAndScatter
                            boxPos = PopulationPlot.Position.Left
                            scatterPos = PopulationPlot.Position.Right
                            boxAlpha = 255

                        Case PopulationPlot.DisplayItems.BoxOnly
                            boxPos = PopulationPlot.Position.Center
                            scatterPos = PopulationPlot.Position.Hide
                            boxAlpha = 255

                        Case PopulationPlot.DisplayItems.ScatterAndBox
                            boxPos = PopulationPlot.Position.Right
                            scatterPos = PopulationPlot.Position.Left
                            boxAlpha = 255

                        Case PopulationPlot.DisplayItems.ScatterOnBox
                            boxPos = PopulationPlot.Position.Center
                            scatterPos = PopulationPlot.Position.Center
                            boxAlpha = 128

                        Case PopulationPlot.DisplayItems.ScatterOnly
                            boxPos = PopulationPlot.Position.Hide
                            scatterPos = PopulationPlot.Position.Center

                        Case Else
                            Throw New NotImplementedException()
                    End Select

                    PopulationPlot.Scatter(dims, bmp, lowQuality, population, rand, groupLeft, popWidth, series.Color, ScatterOutlineColor, 128, scatterPos)

                    If DistributionCurve Then
                        PopulationPlot.Distribution(dims, bmp, lowQuality, population, rand, groupLeft, popWidth, DistributionCurveColor, scatterPos, DistributionCurveLineStyle)
                    End If

                    Select Case DataBoxStyle
                        Case PopulationPlot.BoxStyle.BarMeanStdErr
                            PopulationPlot.Bar(dims, bmp, lowQuality, population, rand, groupLeft, popWidth, series.Color, boxAlpha, boxPos, True)

                        Case PopulationPlot.BoxStyle.BarMeanStDev
                            PopulationPlot.Bar(dims, bmp, lowQuality, population, rand, groupLeft, popWidth, series.Color, boxAlpha, boxPos, False)

                        Case PopulationPlot.BoxStyle.BoxMeanStdevStderr
                            PopulationPlot.Box(dims, bmp, lowQuality, population, rand, groupLeft, popWidth, series.Color, boxAlpha, boxPos, PopulationPlot.BoxFormat.StdevStderrMean)

                        Case PopulationPlot.BoxStyle.BoxMedianQuartileOutlier
                            PopulationPlot.Box(dims, bmp, lowQuality, population, rand, groupLeft, popWidth, series.Color, boxAlpha, boxPos, PopulationPlot.BoxFormat.OutlierQuartileMedian)

                        Case PopulationPlot.BoxStyle.MeanAndStderr
                            PopulationPlot.MeanAndError(dims, bmp, lowQuality, population, rand, groupLeft, popWidth, series.Color, boxAlpha, boxPos, True)

                        Case PopulationPlot.BoxStyle.MeanAndStdev
                            PopulationPlot.MeanAndError(dims, bmp, lowQuality, population, rand, groupLeft, popWidth, series.Color, boxAlpha, boxPos, False)

                        Case Else
                            Throw New NotImplementedException()
                    End Select
                Next
            Next
        End Sub

        Private Shared Sub Scatter(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean, pop As Statistics.Population, rand As Random, popLeft As Double, popWidth As Double, fillColor As Color, edgeColor As Color, alpha As Byte, position As PopulationPlot.Position)
            'adjust edges to accomodate special positions
            Select Case position
                Case Position.Hide
                    Return
                Case Position.Left, Position.Right
                    popWidth /= 2
                Case Position.Right '???
                    popLeft += popWidth
            End Select

            'contract edges slightly to encourage padding between elements
            Dim edgePaddingFrac As Double = 0.2
            popLeft += popWidth * edgePaddingFrac
            popWidth -= popWidth * edgePaddingFrac * 2

            Dim radius As Single = 5

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                penEdge As Pen = Drawing.GDI.Pen(Color.FromArgb(CInt(alpha), edgeColor), 1.0, LineStyle.Solid, False),
                brushFill As Brush = Drawing.GDI.Brush(Color.FromArgb(CInt(alpha), fillColor), Nothing, Drawing.HatchStyle.None)

                For Each position2 As Double In pop.Values
                    Dim yPx As Double = dims.GetPixelY(position2)
                    Dim xPx As Double = dims.GetPixelX(popLeft + rand.NextDouble() * popWidth)
                    gfx.FillEllipse(brushFill, CSng(xPx - radius), CSng(yPx - radius), radius * 2, radius * 2)
                    gfx.DrawEllipse(penEdge, CSng(xPx - radius), CSng(yPx - radius), radius * 2, radius * 2)
                Next
            End Using
        End Sub

        Private Shared Sub Distribution(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean, pop As Statistics.Population, rand As Random, popLeft As Double, popWidth As Double, color As Color, position As PopulationPlot.Position, lineStyle As LineStyle)
            'adjust edges to accomodate special positions
            Select Case position
                Case Position.Hide
                    Return
                Case Position.Left, Position.Right
                    popWidth /= 2.0
                Case Position.Right '???
                    popLeft += popWidth
            End Select

            'contract edges slightly to encourage padding between elements
            Dim edgePaddingFrac As Double = 0.2
            popLeft += popWidth * edgePaddingFrac
            popWidth -= popWidth * edgePaddingFrac * 2

            Dim ys As Double() = DataGen.Range(pop.Minus3stDev, pop.Plus3stDev, dims.UnitsPerPxY, False)
            If (ys.Length = 0) Then
                Return
            End If

            Dim ysFrac As Double() = pop.GetDistribution(ys, False)

            Dim points As PointF() = New PointF(ys.Length - 1) {}
            For i As Integer = 0 To ys.Length - 1
                Dim x As Single = dims.GetPixelX(popLeft + popWidth * ysFrac(i))
                Dim y As Single = dims.GetPixelY(ys(i))
                points(i) = New PointF(x, y)
            Next

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                pen As Pen = Drawing.GDI.Pen(color, 1.0, lineStyle, True)
                gfx.DrawLines(pen, points)
            End Using
        End Sub

        Private Shared Sub MeanAndError(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean, pop As Statistics.Population, rand As Random, popLeft As Double, popWidth As Double, color As Color, alpha As Byte, position As PopulationPlot.Position, Optional useStdErr As Boolean = False)
            'adjust edges to accomodate special positions
            Select Case position
                Case Position.Hide
                    Return
                Case Position.Left, Position.Right
                    popWidth /= 2
                Case Position.Right '???
                    popLeft += popWidth
            End Select

            'determine the center point and calculate bounds
            Dim centerX As Double = popLeft + popWidth / 2
            Dim xPx As Double = dims.GetPixelX(centerX)
            Dim yPx As Double = dims.GetPixelY(pop.Mean)

            Dim errorMaxPx As Double
            Dim errorMinPx As Double
            If useStdErr Then
                errorMaxPx = dims.GetPixelY(pop.Mean + pop.StdErr)
                errorMinPx = dims.GetPixelY(pop.Mean - pop.StdErr)
            Else
                errorMaxPx = dims.GetPixelY(pop.Mean + pop.StDev)
                errorMinPx = dims.GetPixelY(pop.Mean - pop.StDev)
            End If

            'make cap width a fraction of available space
            Dim capWidthFrac As Double = 0.38
            Dim capWidth As Double = popWidth * capWidthFrac
            Dim capPx1 As Double = dims.GetPixelX(centerX - capWidth / 2)
            Dim capPx2 As Double = dims.GetPixelX(centerX + capWidth / 2)
            Dim radius As Single = 5.0F

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                pen As Pen = Drawing.GDI.Pen(Color.FromArgb(alpha, color), 2),
                brush As Brush = Drawing.GDI.Brush(Color.FromArgb(alpha, color))

                gfx.FillEllipse(brush, CSng(xPx - radius), CSng(yPx - radius), radius * 2, radius * 2)
                gfx.DrawLine(pen, CSng(xPx), CSng(errorMinPx), CSng(xPx), CSng(errorMaxPx))
                gfx.DrawLine(pen, CSng(capPx1), CSng(errorMinPx), CSng(capPx2), CSng(errorMinPx))
                gfx.DrawLine(pen, CSng(capPx1), CSng(errorMaxPx), CSng(capPx2), CSng(errorMaxPx))
            End Using
        End Sub

        Private Shared Sub Bar(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean, pop As Statistics.Population, rand As Random, popLeft As Double, popWidth As Double, color As Color, alpha As Byte, position As PopulationPlot.Position, Optional useStdErr As Boolean = False)
            'adjust edges to accomodate special positions
            Select Case position
                Case Position.Hide
                    Return
                Case Position.Left, Position.Right
                    popWidth /= 2
                Case Position.Right '???
                    popLeft += popWidth
            End Select

            'determine the center point and calculate bounds
            Dim centerX As Double = popLeft + popWidth / 2
            Dim xPx As Double = dims.GetPixelX(centerX)
            Dim yPxTop As Double = dims.GetPixelY(pop.Mean)
            Dim yPxBase As Double = dims.GetPixelY(0)

            Dim errorMaxPx As Double
            Dim errorMinPx As Double
            If useStdErr Then
                errorMaxPx = dims.GetPixelY(pop.Mean + pop.StdErr)
                errorMinPx = dims.GetPixelY(pop.Mean - pop.StdErr)
            Else
                errorMaxPx = dims.GetPixelY(pop.Mean + pop.StDev)
                errorMinPx = dims.GetPixelY(pop.Mean - pop.StDev)
            End If

            'make cap width a fraction of available space
            Dim capWidthFrac As Double = 0.38
            Dim capWidth As Double = popWidth * capWidthFrac
            Dim capPx1 As Double = dims.GetPixelX(centerX - capWidth / 2)
            Dim capPx2 As Double = dims.GetPixelX(centerX + capWidth / 2)

            'contract edges slightly to encourage padding between elements
            Dim edgePaddingFrac As Double = 0.2
            popLeft += popWidth * edgePaddingFrac
            popWidth -= popWidth * edgePaddingFrac * 2
            Dim leftPx As Double = dims.GetPixelX(popLeft)
            Dim rightPx As Double = dims.GetPixelX(popLeft + popWidth)

            Dim rect As New RectangleF(CSng(leftPx), CSng(yPxTop), CSng(rightPx - leftPx), CSng(yPxBase - yPxTop))

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                pen As Pen = Drawing.GDI.Pen(Color.Black),
                brush As Brush = Drawing.GDI.Brush(Color.FromArgb(alpha, color))

                gfx.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height)
                gfx.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
                gfx.DrawLine(pen, CSng(xPx), CSng(errorMinPx), CSng(xPx), CSng(errorMaxPx))
                gfx.DrawLine(pen, CSng(capPx1), CSng(errorMinPx), CSng(capPx2), CSng(errorMinPx))
                gfx.DrawLine(pen, CSng(capPx1), CSng(errorMaxPx), CSng(capPx2), CSng(errorMaxPx))
            End Using
        End Sub

        Private Shared Sub Box(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean,
                               pop As Statistics.Population, rand As Random,
                               popLeft As Double, popWidth As Double,
                               color As Color, alpha As Byte,
                               position As PopulationPlot.Position, boxFormat As PopulationPlot.BoxFormat,
                               Optional errorAlignment As PopulationPlot.HorizontalAlignment = PopulationPlot.HorizontalAlignment.Right)
            'adjust edges to accomodate special positions
            Select Case position
                Case Position.Hide
                    Return
                Case Position.Left, Position.Right
                    popWidth /= 2
                Case Position.Right '???
                    popLeft += popWidth
            End Select

            Dim errorMaxPx As Double
            Dim errorMinPx As Double
            Dim yPxTop As Double
            Dim yPxBase As Double
            Dim yPx As Double

            If (boxFormat = PopulationPlot.BoxFormat.StdevStderrMean) Then
                errorMaxPx = dims.GetPixelY(pop.Mean + pop.StDev)
                errorMinPx = dims.GetPixelY(pop.Mean - pop.StDev)
                yPxTop = dims.GetPixelY(pop.Mean + pop.StdErr)
                yPxBase = dims.GetPixelY(pop.Mean - pop.StdErr)
                yPx = dims.GetPixelY(pop.Mean)
            ElseIf boxFormat <> PopulationPlot.BoxFormat.OutlierQuartileMedian Then
                errorMaxPx = dims.GetPixelY(pop.MaxNonOutlier)
                errorMinPx = dims.GetPixelY(pop.MinNonOutlier)
                yPxTop = dims.GetPixelY(pop.Q3)
                yPxBase = dims.GetPixelY(pop.Q1)
                yPx = dims.GetPixelY(pop.Median)
            Else
                Throw New NotImplementedException()
            End If

            'make cap width a fraction of available space
            Dim capWidthFrac As Double = 0.38
            Dim capWidth As Double = popWidth * capWidthFrac

            'contract edges slightly to encourage padding between elements
            Dim edgePaddingFrac As Double = 0.2
            popLeft += popWidth * edgePaddingFrac
            popWidth -= popWidth * edgePaddingFrac * 2
            Dim leftPx As Double = dims.GetPixelX(popLeft)
            Dim rightPx As Double = dims.GetPixelX(popLeft + popWidth)
            Dim rect As New RectangleF(CSng(leftPx), CSng(yPxTop), CSng(rightPx - leftPx), CSng(yPxBase - yPxTop))

            'determine location of errorbars and caps
            Dim errorPxX As Double
            Dim capPx1 As Double
            Dim capPx2 As Double
            Select Case errorAlignment
                Case PopulationPlot.HorizontalAlignment.Center
                    Dim centerX As Double = popLeft + popWidth / 2
                    errorPxX = dims.GetPixelX(centerX)
                    capPx1 = dims.GetPixelX(centerX - capWidth / 2)
                    capPx2 = dims.GetPixelX(centerX + capWidth / 2)

                Case PopulationPlot.HorizontalAlignment.Right
                    errorPxX = dims.GetPixelX(popLeft + popWidth)
                    capPx1 = dims.GetPixelX(popLeft + popWidth - capWidth / 2)
                    capPx2 = dims.GetPixelX(popLeft + popWidth)

                Case PopulationPlot.HorizontalAlignment.Left
                    errorPxX = dims.GetPixelX(popLeft)
                    capPx1 = dims.GetPixelX(popLeft)
                    capPx2 = dims.GetPixelX(popLeft + capWidth / 2)

                Case Else
                    Throw New NotImplementedException()
            End Select

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                Pen As Pen = Drawing.GDI.Pen(Color.Black),
                brush As Brush = Drawing.GDI.Brush(Color.FromArgb(alpha, color))

                'draw the box
                gfx.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height)
                gfx.DrawRectangle(Pen, rect.X, rect.Y, rect.Width, rect.Height)

                'draw the line in the center
                gfx.DrawLine(Pen, rect.X, CSng(yPx), rect.X + rect.Width, CSng(yPx))

                'draw errorbars and caps
                gfx.DrawLine(Pen, CSng(errorPxX), CSng(errorMinPx), CSng(errorPxX), rect.Y + rect.Height)
                gfx.DrawLine(Pen, CSng(errorPxX), CSng(errorMaxPx), CSng(errorPxX), rect.Y)
                gfx.DrawLine(Pen, CSng(capPx1), CSng(errorMinPx), CSng(capPx2), CSng(errorMinPx))
                gfx.DrawLine(Pen, CSng(capPx1), CSng(errorMaxPx), CSng(capPx2), CSng(errorMaxPx))
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace