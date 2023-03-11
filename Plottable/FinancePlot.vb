Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Finance plots display open/high/low/close (OHLC) data.
    ''' </summary>
    Public Class FinancePlot
        Implements IPlottable

#Region "CTOR"

        ''' <summary>
        ''' Create an empty finance plot. 
        ''' Call <see cref="Add(Double, Double, Double, Double)"/> and <see cref="AddRange(OHLC())"/> to add data.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Create a finance plot from existing OHLC data.
        ''' </summary>
        Public Sub New(ohlcs As OHLC())
            AddRange(ohlcs)
        End Sub

#End Region '/CTOR

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Returns the last element of OHLCs so users can modify FinancePlots in real time.
        ''' </summary>
        Public ReadOnly OHLCs As New List(Of OHLC)()

        ''' <summary>
        ''' Display prices as filled candlesticks (otherwise display as OHLC lines).
        ''' </summary>
        Public Property Candle As Boolean

        ''' <summary>
        ''' If True, OHLC timestamps are ignored and candles are placed at consecutive integers and all given a width of 1.
        ''' </summary>
        Public Property Sequential As Boolean

        ''' <summary>
        ''' Color of the candle if it closes at or above its open value.
        ''' </summary>
        Public Property ColorUp As Color = Color.LightGreen

        ''' <summary>
        ''' Color of the candle if it closes below its open value.
        ''' </summary>
        Public Property ColorDown As Color = Color.LightCoral

        ''' <summary>
        ''' This field controls the color of the wick and rectangular candle border.
        ''' If null, the wick is the same color as the candle and no border is applied.
        ''' </summary>
        Public Property WickColor As Color?

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Function Last() As OHLC
            Return OHLCs.Last()
        End Function

        Public Overrides Function ToString() As String
            Return $"FinancePlot with {OHLCs.Count} OHLC indicators."
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        ''' <summary>
        ''' Add a single candle representing a defined time span.
        ''' </summary>
        Public Sub Add(open As Double, high As Double, low As Double, close As Double, timeStart As DateTime, timeSpan As TimeSpan)
            Add(New OHLC(open, high, low, close, timeStart, timeSpan))
        End Sub

        ''' <summary>
        ''' Add a single candle to the end of the list assuming each candle is spaced 1 horizontal unit apart.
        ''' </summary>
        Public Sub Add(open As Double, high As Double, low As Double, close As Double)
            Add(New OHLC(open, high, low, close, OHLCs.Count))
        End Sub

        ''' <summary>
        ''' Add a single OHLC to the plot.
        ''' </summary>
        Public Sub Add(ohlc As OHLC)
            If (ohlc is Nothing) Then
                Throw New ArgumentNullException()
            End If
            OHLCs.Add(ohlc)
        End Sub

        ''' <summary>
        ''' Add multiple OHLCs to the plot.
        ''' </summary>
        Public Sub AddRange(ohlcs As OHLC())
            If (ohlcs is Nothing) Then
                Throw New ArgumentNullException()
            End If
            For i As Integer = 0 To ohlcs.Length - 1
                If (ohlcs(i) is Nothing) Then
                    Throw New ArgumentNullException("No OHLCs may be null.")
                End If
            Next
            Me.OHLCs.AddRange(ohlcs)
        End Sub

        ''' <summary>
        ''' Clear all OHLCs.
        ''' </summary>
        Public Sub Clear()
            OHLCs.Clear()
        End Sub

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If (OHLCs.Count() = 0) Then
                Return AxisLimits.NoLimits
            End If

            Dim xMin As Double = OHLCs(0).DateTime.ToOADate()
            Dim xMax As Double = OHLCs(0).DateTime.ToOADate()
            Dim yMin As Double = OHLCs(0).Low
            Dim yMax As Double = OHLCs(0).High

            For i As Integer = 1 To OHLCs.Count - 1
                If (OHLCs(i).DateTime.ToOADate() < xMin) Then
                    xMin = OHLCs(i).DateTime.ToOADate()
                End If
                If (OHLCs(i).DateTime.ToOADate() > xMax) Then
                    xMax = OHLCs(i).DateTime.ToOADate()
                End If
                If (OHLCs(i).Low < yMin) Then
                    yMin = OHLCs(i).Low
                End If
                If (OHLCs(i).High > yMax) Then
                    yMax = OHLCs(i).High
                End If
            Next
            Return If(Sequential,
                New AxisLimits(0, OHLCs.Count - 1, yMin, yMax),
                New AxisLimits(xMin, xMax, yMin, yMax))
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If Candle Then
                RenderCandles(dims, bmp, lowQuality)
            Else
                RenderOhlc(dims, bmp, lowQuality)
            End If
        End Sub

        Public Sub ValidateData(Optional deepValidation As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If (OHLCs is Nothing) Then
                Throw New InvalidOperationException("OHLCs cannot be null.")
            End If
            For i As Integer = 0 To OHLCs.Count - 1
                If (OHLCs(i) is Nothing) Then
                    Throw New InvalidOperationException($"OHLCs[{i}] cannot be null.")
                End If
                If (Not OHLCs(i).IsValid) Then
                    Throw New InvalidOperationException($"OHLCs[{i}] does not contain valid data.")
                End If
            Next
        End Sub

        Private Sub RenderCandles(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Dim fractionalTickWidth As Double = 0.7

            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                pen As New Pen(Color.Magenta),
                brush As New SolidBrush(Color.Magenta)

                For i As Integer = 0 To Me.OHLCs.Count - 1
                    Dim ohlc As OHLC = Me.OHLCs(i)
                    Dim closedHigher As Boolean = (ohlc.Close >= ohlc.Open)
                    Dim highestOpenClose As Double = Math.Max(ohlc.Open, ohlc.Close)
                    Dim lowestOpenClose As Double = Math.Min(ohlc.Open, ohlc.Close)

                    Dim ohlcTime As Double = If(Sequential, i, ohlc.DateTime.ToOADate())
                    Dim ohlcSpan As Single = If(Sequential, 1, CSng(ohlc.TimeSpan.TotalDays))
                    Dim pixelX As Single = dims.GetPixelX(ohlcTime)

                    Dim boxWidth As Single = CSng(ohlcSpan * dims.PxPerUnitX / 2 * fractionalTickWidth)

                    Dim priceChangeColor As Color = If(closedHigher, ColorUp, ColorDown)
                    pen.Color = If(WickColor, priceChangeColor)
                    pen.Width = If(boxWidth >= 2, 2, 1)

                    'draw the wick below the box
                    Dim wickLowBot As New PointF(pixelX, dims.GetPixelY(ohlc.Low))
                    Dim wickLowTop As New PointF(pixelX, dims.GetPixelY(lowestOpenClose))
                    gfx.DrawLine(pen, wickLowBot, wickLowTop)

                    'draw the wick above the box
                    Dim wickHighBot As New PointF(pixelX, dims.GetPixelY(highestOpenClose))
                    Dim wickHighTop As New PointF(pixelX, dims.GetPixelY(ohlc.High))
                    gfx.DrawLine(pen, wickHighBot, wickHighTop)

                    'draw the candle body
                    Dim boxLowerLeft As New PointF(pixelX, dims.GetPixelY(lowestOpenClose))
                    Dim boxUpperRight As New PointF(pixelX, dims.GetPixelY(highestOpenClose))

                    If (ohlc.Open = ohlc.Close) Then
                        'draw OHLC (non-filled) candle
                        gfx.DrawLine(pen, boxLowerLeft.X - boxWidth, boxLowerLeft.Y, boxLowerLeft.X + boxWidth, boxLowerLeft.Y)
                    Else
                        brush.Color = priceChangeColor
                        gfx.FillRectangle(brush,
                                          boxLowerLeft.X - boxWidth,
                                          boxUpperRight.Y, boxWidth * 2,
                                          boxLowerLeft.Y - boxUpperRight.Y)
                        If (WickColor IsNot Nothing) Then
                            gfx.DrawRectangle(pen,
                                              boxLowerLeft.X - boxWidth,
                                              boxUpperRight.Y, boxWidth * 2,
                                              boxLowerLeft.Y - boxUpperRight.Y)
                        End If
                    End If
                Next
            End Using
        End Sub

        Private Sub RenderOhlc(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Dim fractionalTickWidth As Double = 0.7

            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                pen As Pen = New Pen(Color.Magenta)
                For i As Integer = 0 To OHLCs.Count - 1
                    Dim ohlc As OHLC = OHLCs(i)
                    Dim closedHigher As Boolean = (ohlc.Close >= ohlc.Open)

                    Dim ohlcTime As Double = If(Sequential, i, ohlc.DateTime.ToOADate())
                    Dim ohlcSpan As Single = If(Sequential, 1, CSng(ohlc.TimeSpan.TotalDays))
                    Dim pixelX As Single = dims.GetPixelX(ohlcTime)

                    Dim boxWidth As Single = CSng(ohlcSpan * dims.PxPerUnitX / 2 * fractionalTickWidth)

                    pen.Color = If(closedHigher, ColorUp, ColorDown)
                    pen.Width = If(boxWidth >= 2, 2, 1)

                    'the main line
                    Dim wickTop As New PointF(pixelX, dims.GetPixelY(ohlc.Low))
                    Dim wickBot As New PointF(pixelX, dims.GetPixelY(ohlc.High))
                    gfx.DrawLine(pen, wickBot, wickTop)

                    'open and close lines
                    Dim xPx As Single = wickTop.X
                    Dim yPxOpen As Single = dims.GetPixelY(ohlc.Open)
                    Dim yPxClose As Single = dims.GetPixelY(ohlc.Close)
                    gfx.DrawLine(pen, xPx - boxWidth, yPxOpen, xPx, yPxOpen)
                    gfx.DrawLine(pen, xPx + boxWidth, yPxClose, xPx, yPxClose)
                Next
            End Using
        End Sub

        ''' <summary>
        ''' Return the simple moving average (SMA) of the OHLC closing prices.
        ''' The returned ys are SMA where each point is the average of N points.
        ''' The returned xs are times in OATime units.
        ''' The returned xs and ys arrays will be the length of the OHLC data minus N.
        ''' </summary>
        ''' <param name="N">Each returned value represents the average of N points.</param>
        ''' <returns>Times and averages of the OHLC closing prices.</returns>
        Public Function GetSMA(N As Integer) As Tuple(Of Double(), Double())
            If (N >= OHLCs.Count) Then
                Throw New ArgumentException("can not analyze more points than are available in the OHLCs")
            End If
            Dim sortedOHLCs = GetSortedOHLCs()
            Dim xs As Double() = sortedOHLCs.Skip(N).Select(Function(x) x.DateTime.ToOADate()).ToArray()
            Dim ys As Double() = Statistics.Finance.SMA(sortedOHLCs.ToArray(), N)
            Return New Tuple(Of Double(), Double())(xs, ys)
        End Function

        ''' <summary>
        ''' Return Bollinger bands (mean +/- 2*SD) for the OHLC closing prices.
        ''' The returned xs are times in OATime units.
        ''' The returned xs and ys arrays will be the length of the OHLC data minus N (points).
        ''' </summary>
        ''' <param name="N">Each returned value represents the average of N points.</param>
        ''' <returns>Times, averages, and both Bollinger bands for the OHLC closing prices.</returns>
        Public Function GetBollingerBands(N As Integer) As Tuple(Of Double(), Double(), Double(), Double())
            If (N >= OHLCs.Count) Then
                Throw New ArgumentException("Can not analyze more points than are available in the OHLCs.")
            End If

            Dim sortedOHLCs = GetSortedOHLCs()
            Dim xs As Double() = sortedOHLCs.Skip(N).Select(Function(x) x.DateTime.ToOADate()).ToArray()
            Dim t = Statistics.Finance.Bollinger(sortedOHLCs.ToArray(), N)
            Dim sma As Double() = t.Item1
            Dim lower As Double() = t.Item2
            Dim upper As Double() = t.Item3
            Return New Tuple(Of Double(), Double(), Double(), Double())(xs, sma, lower, upper)
        End Function

        Private Function GetSortedOHLCs() As List(Of OHLC)
            If OHLCsAreSorted() Then
                Return OHLCs
            End If
            Return OHLCs.OrderBy(Function(ohlc) ohlc.DateTime).ToList()
        End Function

        Private Function OHLCsAreSorted() As Boolean
            For i As Integer = 0 To OHLCs.Count - 1 - 1
                If (OHLCs(i).DateTime > OHLCs(i + 1).DateTime) Then
                    Return False
                End If
            Next
            Return True
        End Function

#End Region '/METHODS

    End Class

End Namespace