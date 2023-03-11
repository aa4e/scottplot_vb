Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A colorbar translates numeric intensity values to colors.
    ''' The Colorbar plot type displays a Colorbar along an edge of the plot.
    ''' </summary>
    Public Class Colorbar
        Implements IPlottable, IStylable

#Region "PROPS, FIELDS"

        Public Property Edge As Renderable.Edge = Renderable.Edge.Right

        Private Colormap As Drawing.Colormap
        Private BmpScale As Bitmap

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        ''' <summary>
        ''' Width of the colorbar rectangle.
        ''' </summary>
        Public Property Width As Integer = 20

        Public ReadOnly TickLabelFont As New ScottPlot.Drawing.Font()
        Public Property TickMarkColor As Color = Color.Black
        Public Property TickMarkLength As Single = 3
        Public Property TickMarkWidth As Single = 1

        Private ReadOnly ManualTicks As New List(Of Ticks.Tick)()
        Private Property AutomaticTickEnable As Boolean = True
        Private Property AutomaticTickMinimumSpacing As Integer = 40
        Private Property AutomaticTickFormatter As Func(Of Double, String) = Function(position) $"{position:F2}"

        Public Property DataAreaPadding As Single = 10

        ''' <summary>
        ''' If populated, this object holds the plottable containing the heatmap and value data this colorbar represents.
        ''' </summary>
        Private Plottable As IHasColormap

        Public Property MinValue As Double
            Get
                Dim plottable As IHasColormap = Me.Plottable
                Return If(plottable is Nothing, _MinValue, plottable.ColormapMin)
            End Get
            Set(value As Double)
                _MinValue = value
            End Set
        End Property
        Private Property _MinValue As Double

        Public Property MaxValue As Double
            Get
                Dim plottable As IHasColormap = Me.Plottable
                Return If(plottable is Nothing, _MaxValue, plottable.ColormapMax)
            End Get
            Set(value As Double)
                _MaxValue = value
            End Set
        End Property
        Private _MaxValue As Double

        Public Property MinIsClipped As Boolean
            Get
                Dim plottable As IHasColormap = Me.Plottable
                Return If(plottable is Nothing, _MinIsClipped, plottable.ColormapMinIsClipped)
            End Get
            Set(value As Boolean)
                _MinIsClipped = value
            End Set
        End Property
        Private _MinIsClipped As Boolean

        Public Property MaxIsClipped As Boolean
            Get
                Dim plottable As IHasColormap = Me.Plottable
                Return If(plottable is Nothing, _MaxIsClipped, plottable.ColormapMaxIsClipped)
            End Get
            Set(value As Boolean)
                _MaxIsClipped = value
            End Set
        End Property
        Private _MaxIsClipped As Boolean

        Public Property MinColor As Double
            Get
                Return _MinColor
            End Get
            Set(value As Double)
                _MinColor = value
                UpdateBitmap()
            End Set
        End Property
        Private _MinColor As Double = 0

        Public Property MaxColor As Double
            Get
                Return _MaxColor
            End Get
            Set(value As Double)
                _MaxColor = value
                UpdateBitmap()
            End Set
        End Property
        Private _MaxColor As Double = 1

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(Optional colormap As Drawing.Colormap = Nothing)
            UpdateColormap(If(colormap IsNot Nothing, colormap, Drawing.Colormap.Viridis))
        End Sub

        Public Sub New(plottable As IHasColormap)
            Me.Plottable = plottable
            UpdateColormap(plottable.Colormap)
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return AxisLimits.NoLimits
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Public Sub SetStyle(tickMarkColor As Color?, tickFontColor As Color?) Implements ScottPlot.Plottable.IStylable.SetStyle
            Me.TickMarkColor = If(tickMarkColor, Me.TickMarkColor)
            Me.TickLabelFont.Color = If(tickFontColor, Me.TickLabelFont.Color)
        End Sub

        ''' <summary>
        ''' Configure ticks that are automatically generated in the absense of manually-added ticks.
        ''' </summary>
        ''' <param name="minimumSpacing">Minimum number of vertical pixels between tick positions.</param>
        ''' <param name="formatter">Optional custom string formatter to translate tick positions to labels.</param>
        Public Sub AutomaticTicks(Optional enable As Boolean = True, Optional minimumSpacing As Integer? = Nothing, Optional formatter As Func(Of Double, String) = Nothing)
            If enable Then
                ManualTicks.Clear()
            End If
            AutomaticTickEnable = enable
            AutomaticTickMinimumSpacing = If(minimumSpacing, AutomaticTickMinimumSpacing)
            AutomaticTickFormatter = If(formatter, AutomaticTickFormatter)
        End Sub

        ''' <summary>
        ''' Clear the list of manually-defined ticks.
        ''' To enable automatic tick placement call.
        ''' </summary>
        Public Sub ClearTicks()
            ManualTicks.Clear()
        End Sub

        ''' <summary>
        ''' Add a tick to the list of manually-defined ticks (disabling automatic tick placement).
        ''' </summary>
        ''' <param name="fraction">From 0 (darkest) to 1 (brightest).</param>
        ''' <param name="label">String displayed beside the tick.</param>
        Public Sub AddTick(fraction As Double, label As String)
            ManualTicks.Add(New Ticks.Tick(fraction, label, True, False))
        End Sub

        ''' <summary>
        ''' Manually define ticks (disabling automatic tick placement).
        ''' </summary>
        ''' <param name="fractions">From 0 (darkest) to 1 (brightest).</param>
        ''' <param name="labels">Strings displayed beside the ticks.</param>
        Public Sub AddTicks(fractions As Double(), labels As String())
            If (fractions.Length <> labels.Length) Then
                Throw New Exception("Fractions and labels must have the same length.")
            End If
            For i As Integer = 0 To fractions.Length - 1
                ManualTicks.Add(New Ticks.Tick(fractions(i), labels(i), True, False))
            Next
        End Sub

        ''' <summary>
        ''' Manually define ticks as a fraction from 0 to 1 (disabling automatic tick placement).
        ''' </summary>
        ''' <param name="fractions">From 0 (darkest) to 1 (brightest).</param>
        ''' <param name="labels">Strings displayed beside the ticks.</param>
        Public Sub SetTicks(fractions As Double(), labels As String())
            If (fractions.Length <> labels.Length) Then
                Throw New Exception("Fractions and labels must have the same length.")
            End If
            ClearTicks()
            AddTicks(fractions, labels)
        End Sub

        ''' <summary>
        ''' Manually define ticks by value within a range (disabling automatic tick placement).
        ''' </summary>
        ''' <param name="values">Position for each tick.</param>
        ''' <param name="labels">Label for each tick.</param>
        ''' <param name="min">Colorbar range minimum.</param>
        ''' <param name="max">Colorbar range maximum.</param>
        Public Sub SetTicks(values As Double(), labels As String(), min As Double, max As Double)
            If (values.Length <> labels.Length) Then
                Throw New Exception("Fractions and labels must have the same length.")
            End If
            Dim span As Double = max - min
            Dim fractions As Double() = values.Select(Function(x As Double) (x - min) / span).ToArray()
            SetTicks(fractions, labels)
        End Sub

        ''' <summary>
        ''' Re-Render the colorbar using a new colormap.
        ''' </summary>
        Public Sub UpdateColormap(newColormap As Drawing.Colormap)
            Colormap = If(newColormap, Drawing.Colormap.Viridis)
            UpdateBitmap()
        End Sub

        Private Sub UpdateBitmap()
            BmpScale?.Dispose()
            BmpScale = GetBitmap()
        End Sub

        ''' <summary>
        ''' Return a Bitmap of just the color portion of the colorbar.
        ''' The width is defined by the Width field. The height will be 256.
        ''' </summary>
        Public Function GetBitmap() As Bitmap
            Return Drawing.Colormap.Colorbar(Colormap, Width, 256, True, MinColor, MaxColor)
        End Function

        ''' <summary>
        ''' Return a Bitmap of just the color portion of the colorbar.
        ''' </summary>
        ''' <param name="width">Width of the Bitmap.</param>
        ''' <param name="height">Height of the Bitmap.</param>
        ''' <param name="vertical">If true, colormap will be vertically oriented (tall and skinny).</param>
        Public Function GetBitmap(width As Integer, height As Integer, Optional vertical As Boolean = True) As Bitmap
            Return Drawing.Colormap.Colorbar(Colormap, width, height, vertical, MinColor, MaxColor)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If (BmpScale is Nothing) Then
                UpdateBitmap()
            End If
            Dim colorbarRect As RectangleF = RenderColorbar(dims, bmp)
            RenderTicks(dims, bmp, lowQuality, colorbarRect)
        End Sub

        ''' <summary>
        ''' Return a list of ticks evenly spaced between the min and max values.
        ''' </summary>
        ''' <param name="height">Height of the vertical colorbar.</param>
        ''' <param name="tickSpacing">Minimum pixel distance between adjacent ticks.</param>
        Private Function GetEvenlySpacedTicks(height As Single, tickSpacing As Double) As List(Of Ticks.Tick)
            Dim ticks As New List(Of Ticks.Tick)()
            Dim tickCount As Integer = CInt(height / tickSpacing)
            tickCount = Math.Max(tickCount, 1)
            Dim tickSpacingFraction As Double = 1 / tickCount
            Dim valueSpan As Double = MaxValue - MinValue
            For i As Integer = 0 To tickCount
                Dim colorbarFraction As Double = tickSpacingFraction * i
                Dim tickPosition As Double = MinValue + colorbarFraction * valueSpan

                Dim tickLabel As New System.Text.StringBuilder(AutomaticTickFormatter(tickPosition))
                If MinIsClipped AndAlso (i = 0) Then
                    tickLabel = tickLabel.Insert(0, "≤")
                End If
                If MaxIsClipped AndAlso (i = tickCount) Then
                    tickLabel = tickLabel.Insert(0, "≥")
                End If
                Dim tick As New Ticks.Tick(colorbarFraction, tickLabel.ToString(), True, False)
                ticks.Add(tick)
            Next
            Return ticks
        End Function

        Private Function RenderColorbar(dims As PlotDimensions, bmp As Bitmap) As RectangleF
            Dim size As SizeF = New SizeF(CSng(Width), dims.DataHeight)
            Dim locationY As Single = dims.DataOffsetY
            Dim locationX As Single
            If (Edge = ScottPlot.Renderable.Edge.Right) Then
                locationX = dims.DataOffsetX + dims.DataWidth + DataAreaPadding
            ElseIf (Edge = ScottPlot.Renderable.Edge.Left) Then
                locationX = DataAreaPadding
            Else
                Throw New InvalidOperationException($"Unsupported {NameOf(Edge)}: {Edge}.")
            End If

            Dim location As New PointF(locationX, locationY)
            Dim rect As New RectangleF(location, size)

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, True, False),
                pen As Pen = Drawing.GDI.Pen(TickMarkColor)
                gfx.DrawImage(BmpScale, location.X, location.Y, size.Width, size.Height + 1)
                gfx.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
            End Using
            Return rect
        End Function

        Private Sub RenderTicks(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean, colorbarRect As RectangleF)
            Dim tickLeftPx As Single = colorbarRect.Right
            Dim tickRightPx As Single = tickLeftPx + TickMarkLength
            Dim tickLabelPx As Single = tickRightPx + 2

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, False),
                tickMarkPen As Pen = Drawing.GDI.Pen(TickMarkColor, TickMarkWidth),
                tickLabelBrush As Brush = Drawing.GDI.Brush(TickLabelFont.Color),
                tickFont As System.Drawing.Font = Drawing.GDI.Font(TickLabelFont),
                sf As New StringFormat() With {.LineAlignment = StringAlignment.Center}

                Dim useManualTicks As Boolean = (ManualTicks.Count > 0) OrElse (Not AutomaticTickEnable)
                Dim ticks As List(Of Ticks.Tick) = If(useManualTicks, ManualTicks, GetEvenlySpacedTicks(colorbarRect.Height, AutomaticTickMinimumSpacing))

                For Each tick As Ticks.Tick In ticks
                    Dim y As Single = colorbarRect.Top + CSng((1 - tick.Position) * colorbarRect.Height)
                    gfx.DrawLine(tickMarkPen, tickLeftPx, y, tickRightPx, y)
                    gfx.DrawString(tick.Label, tickFont, tickLabelBrush, tickLabelPx, y, sf)
                Next
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace