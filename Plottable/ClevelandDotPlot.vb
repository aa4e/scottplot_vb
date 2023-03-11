Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Cleveland Dot plots display a series of paired points. 
    ''' Positions are defined by Xs.
    ''' Heights are defined by Ys1 and Ys2 (internally done with Ys and YOffsets).
    ''' </summary>
    Public Class ClevelandDotPlot
        Inherits BarPlotBase
        Implements IPlottable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Color for the line.
        ''' </summary>
        ''' <returns></returns>
        Public Property StemColor As Color = Color.Gray

        ''' <summary>
        ''' Size of the markers at the ends of each line.
        ''' </summary>
        Public Property DotRadius As Single = 5

        ''' <summary>
        ''' Text to display in the legend associated with the series 1 data.
        ''' </summary>
        Private Property Label As String

        ''' <summary>
        ''' Color for one of the markers.
        ''' </summary>
        Private Property Color1 As Color = Color.Green

        ''' <summary>
        ''' Marker to use for the series 1 data.
        ''' </summary>
        Private Property MarkerShape1 As MarkerShape = MarkerShape.FilledCircle

        ''' <summary>
        ''' Text to display in the legend associated with the series 2 data.
        ''' </summary>
        Private Property Label2 As String

        ''' <summary>
        ''' Color for one of the markers.
        ''' </summary>
        Private Property Color2 As Color = Color.Red

        ''' <summary>
        ''' Marker to use for the series 2 data.
        ''' </summary>
        Private Property MarkerShape2 As MarkerShape = MarkerShape.FilledCircle

        Private Shadows Property IsVisible As Boolean = True Implements IPlottable.IsVisible
        Private Shadows Property XAxisIndex As Integer = 0 Implements IPlottable.XAxisIndex
        Private Shadows Property YAxisIndex As Integer = 0 Implements IPlottable.YAxisIndex

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(xs As Double(), ys1 As Double(), ys2 As Double())
            Me.Ys1 = ys1
            Me.Ys2 = ys2
            Positions = xs
            ValueErrors = DataGen.Zeros(ys1.Length)
        End Sub

#End Region '/CTOR

#Region "METHODS"

        ''' <summary>
        ''' TODO: don't expose these, instead put them behind an Update() method
        ''' that lets the user update one Or both arrays. This can also perform length checking.
        ''' </summary>
        Public Property Ys1 As Double()
            Get
                Return ValueOffsets
            End Get
            Set(value As Double())
                Dim diff As Double() = If(Ys1, DataGen.Zeros(value.Length)).Zip(value, Function(y, v) y - v).ToArray()
                ValueOffsets = value

                If (Ys2 IsNot Nothing) Then
                    Ys2 = If(Ys2, DataGen.Zeros(value.Length)).Zip(diff, Function(y, v) y + v).ToArray()
                End If
            End Set
        End Property

        Public Property Ys2 As Double()
            Get
                If (Values is Nothing) Then
                    Return Nothing
                End If
                Dim offsets As Double() = If(Ys1, DataGen.Zeros(Values.Length))
                Return Values.Select(Function(y As Double, i As Integer) y + offsets(i)).ToArray()
            End Get
            Set(value As Double())
                Dim offsets As Double() = If(Ys1, DataGen.Zeros(value.Length))
                Values = value.Select(Function(y As Double, i As Integer) y - offsets(i)).ToArray()
            End Set
        End Property

        ''' <summary>
        ''' Allows customizing the first point (set by ys1).
        ''' </summary>
        ''' <param name="color">The color of the dot, null for no change.</param>
        ''' <param name="markerShape">The shape of the dot, null for no change.</param>
        ''' <param name="label">The label of the dot in the legend, null for no change</param>
        Public Sub SetPoint1Style(Optional color As Color? = Nothing, Optional markerShape As MarkerShape? = Nothing, Optional label As String = Nothing)
            Me.Label = If(label, Me.Label)
            Me.MarkerShape1 = If(markerShape, MarkerShape1)
            Me.Color1 = If(color, Me.Color1)
        End Sub

        ''' <summary>
        ''' Allows customizing the second point (set by ys2).
        ''' </summary>
        ''' <param name="color">The color of the dot, null for no change.</param>
        ''' <param name="markerShape">The shape of the dot, null for no change.</param>
        ''' <param name="label">The label of the dot in the legend, null for no change.</param>
        Public Sub SetPoint2Style(Optional color As Color? = Nothing, Optional markerShape As MarkerShape? = Nothing, Optional label As String = Nothing)
            Me.Label2 = If(label, Me.Label2)
            Me.MarkerShape2 = If(markerShape, Me.MarkerShape2)
            Me.Color2 = If(color, Me.Color2)
        End Sub

        Public Overrides Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Dim valueMin As Double = Double.PositiveInfinity
            Dim valueMax As Double = Double.NegativeInfinity
            Dim positionMin As Double = Double.PositiveInfinity
            Dim positionMax As Double = Double.NegativeInfinity

            For i As Integer = 0 To Positions.Length - 1
                valueMin = New Double() {valueMin, Values(i) - ValueErrors(i) + ValueOffsets(i), ValueOffsets(i)}.Min()
                valueMax = New Double() {valueMax, Values(i) + ValueErrors(i) + ValueOffsets(i), ValueOffsets(i)}.Max()
                positionMin = Math.Min(positionMin, Positions(i))
                positionMax = Math.Max(positionMax, Positions(i))
            Next

            valueMin = Math.Min(valueMin, ValueBase)
            valueMax = Math.Max(valueMax, ValueBase)

            If ShowValuesAboveBars Then
                valueMax += (valueMax - valueMin) * 0.1 'increase by 10% to accomodate label
            End If

            positionMin -= BarWidth / 2
            positionMax += BarWidth / 2
            positionMin += PositionOffset
            positionMax += PositionOffset

            If (Orientation = Orientation.Vertical) Then
                Return New AxisLimits(positionMin, positionMax, valueMin, valueMax)
            End If
            Return New AxisLimits(valueMin, valueMax, positionMin, positionMax)
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim firstDot As New LegendItem(Me) With {
                .Label = Label,
                .Color = Color1,
                .LineStyle = LineStyle.None,
                .MarkerShape = MarkerShape1,
                .MarkerSize = 5}
            Dim secondDot As New LegendItem(Me) With {
                .Label = Label2,
                .Color = Color2,
                .LineStyle = LineStyle.None,
                .MarkerShape = MarkerShape2,
                .MarkerSize = 5}
            Return New LegendItem() {firstDot, secondDot}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality)
                For i As Integer = 0 To Values.Length - 1
                    If (Orientation = Orientation.Vertical) Then
                        RenderBarVertical(dims, gfx, Positions(i) + PositionOffset, Values(i), ValueErrors(i), ValueOffsets(i))
                    Else
                        RenderBarHorizontal(dims, gfx, Positions(i) + PositionOffset, Values(i), ValueErrors(i), ValueOffsets(i))
                    End If
                Next
            End Using
        End Sub

        Private Sub RenderBarFromRect(rect As RectangleF, negative As Boolean, gfx As Graphics)
            Dim centerPx As Single = If(Orientation = Orientation.Horizontal,
                rect.Y + rect.Height / 2,
                rect.X + rect.Width / 2)

            Using stemPen As New Pen(StemColor),
                dot1Brush As Brush = Drawing.GDI.Brush(Color1),
                dot2Brush As Brush = Drawing.GDI.Brush(Color2)

                Dim points As PointF() = New PointF(1) {}
                If (Orientation = Orientation.Horizontal) Then
                    points(0) = New PointF(If(negative, rect.X + rect.Width, rect.X), centerPx - DotRadius / 2)
                    points(1) = New PointF(If(negative, rect.X, rect.X + rect.Width), centerPx - DotRadius / 2)
                Else
                    points(0) = New PointF(centerPx - DotRadius / 2, If(Not negative, rect.Y + rect.Height, rect.Y))
                    points(1) = New PointF(centerPx - DotRadius / 2, If(Not negative, rect.Y, rect.Y + rect.Height))
                End If
                gfx.DrawLine(stemPen, points(0), points(1))
                MarkerTools.DrawMarker(gfx, points(1), MarkerShape2, DotRadius, Color2, 1)
                MarkerTools.DrawMarker(gfx, points(0), MarkerShape1, DotRadius, Color1, 1) 'First point should be drawn overtop the second.
            End Using
        End Sub

        Private Sub RenderBarVertical(dims As PlotDimensions, gfx As Graphics, position As Double, value As Double, valueError As Double, yOffset As Double)
            'bar body
            Dim centerPx As Single = dims.GetPixelX(position)
            Dim edge1 As Double = position - MyBase.BarWidth / 2.0
            Dim value1 As Double = Math.Min(MyBase.ValueBase, value) + yOffset
            Dim value2 As Double = Math.Max(MyBase.ValueBase, value) + yOffset
            Dim valueSpan As Double = value2 - value1

            Dim rect As New RectangleF(dims.GetPixelX(edge1),
                                       dims.GetPixelY(value2),
                                       CSng(BarWidth * dims.PxPerUnitX),
                                       CSng(valueSpan * dims.PxPerUnitY))

            'errorbar
            Dim error1 As Double = If(value > 0, value2 - Math.Abs(valueError), value1 - Math.Abs(valueError))
            Dim error2 As Double = If(value > 0, value2 + Math.Abs(valueError), value1 + Math.Abs(valueError))
            Dim capPx1 As Single = dims.GetPixelX(position - ErrorCapSize * BarWidth / 2)
            Dim capPx2 As Single = dims.GetPixelX(position + ErrorCapSize * BarWidth / 2)
            Dim errorPx2 As Single = dims.GetPixelY(error2)
            Dim errorPx1 As Single = dims.GetPixelY(error1)

            RenderBarFromRect(rect, value < 0, gfx)

            If (ErrorLineWidth > 0) AndAlso (valueError > 0) Then
                Using errorPen As New Pen(ErrorColor, ErrorLineWidth)
                    gfx.DrawLine(errorPen, centerPx, errorPx1, centerPx, errorPx2)
                    gfx.DrawLine(errorPen, capPx1, errorPx1, capPx2, errorPx1)
                    gfx.DrawLine(errorPen, capPx1, errorPx2, capPx2, errorPx2)
                End Using
            End If

            If ShowValuesAboveBars Then
                Using valueTextFont As System.Drawing.Font = Drawing.GDI.Font(Font),
                    valueTextBrush As Brush = Drawing.GDI.Brush(Font.Color),
                    sf As New StringFormat() With {.LineAlignment = StringAlignment.Far, .Alignment = StringAlignment.Center}
                    gfx.DrawString(ValueFormatter(value), valueTextFont, valueTextBrush, centerPx, rect.Y, sf)
                End Using
            End If
        End Sub

        Private Sub RenderBarHorizontal(dims As PlotDimensions, gfx As Graphics, position As Double, value As Double, valueError As Double, yOffset As Double)
            'bar body
            Dim centerPx As Single = dims.GetPixelY(position)
            Dim edge2 As Double = position + BarWidth / 2
            Dim value1 As Double = Math.Min(ValueBase, value) + yOffset
            Dim value2 As Double = Math.Max(ValueBase, value) + yOffset
            Dim valueSpan As Double = value2 - value1
            Dim rect As New RectangleF(dims.GetPixelX(value1),
                                       dims.GetPixelY(edge2),
                                       CSng(valueSpan * dims.PxPerUnitX),
                                       CSng(BarWidth * dims.PxPerUnitY))
            RenderBarFromRect(rect, value < 0, gfx)

            'errorbar
            Dim error1 As Double = If(value > 0, value2 - Math.Abs(valueError), value1 - Math.Abs(valueError))
            Dim error2 As Double = If(value > 0, value2 + Math.Abs(valueError), value1 + Math.Abs(valueError))
            Dim capPx1 As Single = dims.GetPixelY(position - ErrorCapSize * BarWidth / 2)
            Dim capPx2 As Single = dims.GetPixelY(position + ErrorCapSize * BarWidth / 2)
            Dim errorPx2 As Single = dims.GetPixelX(error2)
            Dim errorPx1 As Single = dims.GetPixelX(error1)

            If (ErrorLineWidth > 0) AndAlso (valueError > 0) Then
                Using errorPen As New Pen(ErrorColor, ErrorLineWidth)
                    gfx.DrawLine(errorPen, errorPx1, centerPx, errorPx2, centerPx)
                    gfx.DrawLine(errorPen, errorPx1, capPx2, errorPx1, capPx1)
                    gfx.DrawLine(errorPen, errorPx2, capPx2, errorPx2, capPx1)
                End Using
            End If

            If ShowValuesAboveBars Then
                Using valueTextFont As System.Drawing.Font = Drawing.GDI.Font(Font),
                    valueTextBrush As Brush = Drawing.GDI.Brush(Font.Color),
                    sf As New StringFormat() With {.LineAlignment = StringAlignment.Center, .Alignment = StringAlignment.Near}
                    gfx.DrawString(ValueFormatter(value), valueTextFont, valueTextBrush, rect.X + rect.Width, centerPx, sf)
                End Using
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace