Imports System.Drawing

Namespace ScottPlot.Plottable

    Public Class ErrorBar
        Implements IPlottable, IHasLine, IHasMarker, IHasColor

#Region "PROPS"

        Public Property Xs As Double()
        Public Property Ys As Double()
        Public Property XErrorsPositive As Double()
        Public Property XErrorsNegative As Double()
        Public Property YErrorsPositive As Double()
        Public Property YErrorsNegative As Double()
        Public Property CapSize As Integer = 3

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property LineWidth As Double = 1 Implements ScottPlot.Plottable.IHasLine.LineWidth
        Public Property Color As Color = Color.Gray Implements ScottPlot.Plottable.IHasColor.Color

        Public Property LineColor As Color Implements ScottPlot.Plottable.IHasLine.LineColor
            Get
                Return Color
            End Get
            Set(value As Color)
                Color = value
            End Set
        End Property

        Public Property LineStyle As LineStyle = LineStyle.Solid Implements ScottPlot.Plottable.IHasLine.LineStyle
        Public Property MarkerShape As MarkerShape = MarkerShape.FilledCircle Implements ScottPlot.Plottable.IHasMarker.MarkerShape
        Public Property MarkerLineWidth As Single = 1 Implements ScottPlot.Plottable.IHasMarker.MarkerLineWidth
        Public Property MarkerSize As Single = 0 Implements ScottPlot.Plottable.IHasMarker.MarkerSize

        Public Property MarkerColor As Color Implements ScottPlot.Plottable.IHasMarker.MarkerColor
            Get
                Return Color
            End Get
            Set(value As Color)
                Color = value
            End Set
        End Property

#End Region '/PROPS

#Region "CTOR"

        Public Sub New(xs As Double(), ys As Double(), xErrorsPositive As Double(), xErrorsNegative As Double(), yErrorsPositive As Double(), yErrorsNegative As Double())
            Me.Xs = xs
            Me.Ys = ys
            Me.XErrorsPositive = xErrorsPositive
            Me.XErrorsNegative = xErrorsNegative
            Me.YErrorsPositive = yErrorsPositive
            Me.YErrorsNegative = yErrorsNegative
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Dim xMin As Double = Double.PositiveInfinity
            Dim xMax As Double = Double.NegativeInfinity
            Dim yMin As Double = Double.PositiveInfinity
            Dim yMax As Double = Double.NegativeInfinity

            For i As Integer = 0 To Xs.Length - 1
                xMin = Math.Min(xMin, Xs(i) - (If(XErrorsNegative?(i) IsNot Nothing, XErrorsNegative(i), 0)))
                xMax = Math.Max(xMax, Xs(i) + (If(XErrorsPositive?(i) IsNot Nothing, XErrorsPositive(i), 0)))
                yMin = Math.Min(yMin, Ys(i) - (If(YErrorsNegative?(i) IsNot Nothing, YErrorsNegative(i), 0)))
                yMax = Math.Max(yMax, Ys(i) + (If(YErrorsPositive?(i) IsNot Nothing, YErrorsPositive(i), 0)))
            Next

            Return New AxisLimits(xMin, xMax, yMin, yMax)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                pen As Pen =Drawing.GDI.Pen(Color, LineWidth, LineStyle, True)

                If (XErrorsPositive IsNot Nothing) AndAlso (XErrorsNegative IsNot Nothing) Then
                    DrawErrorBars(dims, gfx, pen, XErrorsPositive, XErrorsNegative, True)
                End If
                If (YErrorsPositive IsNot Nothing) AndAlso (YErrorsNegative IsNot Nothing) Then
                    DrawErrorBars(dims, gfx, pen, YErrorsPositive, YErrorsNegative, False)
                End If
                If (MarkerSize > 0) AndAlso (MarkerShape <> MarkerShape.None) Then
                    DrawMarkers(dims, gfx)
                End If
            End Using
        End Sub

        Private Sub DrawErrorBars(dims As PlotDimensions, gfx As Graphics, pen As Pen, errorPositive As Double(), errorNegative As Double(), onXAxis As Boolean)
            For i As Integer = 0 To Xs.Length - 1
                If onXAxis Then
                    Dim left As Pixel = dims.GetPixel(New Coordinate(Xs(i) - errorNegative(i), Ys(i)))
                    Dim right As Pixel = dims.GetPixel(New Coordinate(Xs(i) + errorPositive(i), Ys(i)))
                    If (left <> right) Then
                        gfx.DrawLine(pen, left.X, left.Y, right.X, right.Y)
                        gfx.DrawLine(pen, left.X, left.Y - CapSize, left.X, left.Y + CapSize)
                        gfx.DrawLine(pen, right.X, right.Y - CapSize, right.X, right.Y + CapSize)
                    End If
                Else
                    Dim top As Pixel = dims.GetPixel(New Coordinate(Xs(i), Ys(i) - errorNegative(i)))
                    Dim bot As Pixel = dims.GetPixel(New Coordinate(Xs(i), Ys(i) + errorPositive(i)))
                    If (top <> bot) Then
                        gfx.DrawLine(pen, top.X, top.Y, bot.X, bot.Y)
                        gfx.DrawLine(pen, top.X - CapSize, top.Y, top.X + CapSize, top.Y)
                        gfx.DrawLine(pen, bot.X - CapSize, bot.Y, bot.X + CapSize, bot.Y)
                    End If
                End If
            Next
        End Sub

        Private Sub DrawMarkers(dims As PlotDimensions, gfx As Graphics)
            Dim pixels As PointF() = New PointF(Xs.Length - 1) {}
            For i As Integer = 0 To Xs.Length - 1
                Dim pixelX As Single = dims.GetPixelX(Xs(i))
                Dim pixelY As Single = dims.GetPixelY(Ys(i))
                pixels(i) = New PointF(pixelX, pixelY)
            Next
            MarkerTools.DrawMarkers(gfx, pixels, MarkerShape, MarkerSize, Color, MarkerLineWidth)
        End Sub

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            Validate.AssertHasElements("Xs", Xs)
            Validate.AssertHasElements("Ys", Ys)
            Validate.AssertEqualLength(Of Double, Double)("Xs, Ys", Xs, Ys)

            If (XErrorsPositive IsNot Nothing) OrElse (XErrorsNegative IsNot Nothing) Then
                Validate.AssertHasElements("XErrorsPositive", XErrorsPositive)
                Validate.AssertHasElements("XErrorsNegative", XErrorsNegative)
                Validate.AssertEqualLength($"{Xs} {NameOf(XErrorsPositive)}, {NameOf(XErrorsNegative)}", Xs, XErrorsPositive, XErrorsNegative)
            End If

            If YErrorsPositive IsNot Nothing OrElse YErrorsNegative IsNot Nothing Then
                Validate.AssertHasElements("YErrorsPositive", YErrorsPositive)
                Validate.AssertHasElements("YErrorsNegative", YErrorsNegative)
                Validate.AssertEqualLength($"{Xs} {NameOf(YErrorsPositive)}, {NameOf(YErrorsNegative)}", Xs, YErrorsPositive, YErrorsNegative)
            End If

            If deep Then
                Validate.AssertAllReal("Xs", Xs)
                Validate.AssertAllReal("Ys", Ys)

                If (XErrorsPositive IsNot Nothing) AndAlso (XErrorsNegative IsNot Nothing) Then
                    Validate.AssertAllReal(NameOf(XErrorsPositive), XErrorsPositive)
                    Validate.AssertAllReal(NameOf(XErrorsNegative), XErrorsNegative)
                End If
                If (YErrorsPositive IsNot Nothing) AndAlso (YErrorsNegative IsNot Nothing) Then
                    Validate.AssertAllReal(NameOf(YErrorsPositive), YErrorsPositive)
                    Validate.AssertAllReal(NameOf(YErrorsNegative), YErrorsNegative)
                End If
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace