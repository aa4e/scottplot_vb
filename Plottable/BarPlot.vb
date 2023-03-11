Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Bar plots display a series of bars. 
    ''' Positions are defined by Xs. Heights are defined by Ys (relative to BaseValue and YOffsets).
    ''' </summary>
    Public Class BarPlot
        Inherits BarPlotBase
        Implements IPlottable, IHasColor

#Region "PROPS, FIELDS"

        Public Property Label As String

        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return FillColor
            End Get
            Set(value As Color)
                FillColor = value
            End Set
        End Property

        Public Property FillColor As Color = Color.Green
        Public Property FillColorNegative As Color = Color.Red
        Public Property FillColorHatch As Color = Color.Blue
        Public Property HatchStyle As Drawing.HatchStyle = Drawing.HatchStyle.None
        Public Property BorderLineWidth As Single = 1

        Private Property IsVisible As Boolean Implements IPlottable.IsVisible
        Private Property XAxisIndex As Integer Implements IPlottable.XAxisIndex
        Private Property YAxisIndex As Integer Implements IPlottable.YAxisIndex

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(xs As Double(), ys As Double(), yErr As Double(), yOffsets As Double())
            If (ys is Nothing) OrElse (ys.Length = 0) Then
                Throw New InvalidOperationException("Ys must be an array that contains elements.")
            End If
            If (xs IsNot Nothing) AndAlso (ys.Length <> xs.Length) Then
                Throw New ArgumentException("Bar plot Xs and Ys must have the same number of elements.")
            End If
            Values = ys
            Positions = If(xs, DataGen.Consecutive(ys.Length, 1.0, 0.0)) 'TEST Positions = xs ?? DataGen.Consecutive(ys.Length);
            ValueErrors = If(yErr, DataGen.Zeros(ys.Length))
            ValueOffsets = If(yOffsets, DataGen.Zeros(ys.Length))
        End Sub

#End Region '/CTOR

#Region "METHODS"

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

            'erroebar
            Dim error1 As Double = If(value > 0, value2 - Math.Abs(valueError), value1 - Math.Abs(valueError))
            Dim error2 As Double = If(value > 0, value2 + Math.Abs(valueError), value1 + Math.Abs(valueError))
            Dim capPx1 As Single = dims.GetPixelX(position - ErrorCapSize * BarWidth / 2)
            Dim capPx2 As Single = dims.GetPixelX(position + ErrorCapSize * BarWidth / 2)
            Dim errorPx2 As Single = dims.GetPixelY(error2)
            Dim errorPx1 As Single = dims.GetPixelY(error1)

            RenderBarFromRect(BarPlot.ClipRectToDataArea(rect, dims), value < 0, gfx)

            If (ErrorLineWidth > 0) AndAlso (valueError > 0) Then
                Using errorPen As New Pen(ErrorColor, ErrorLineWidth)
                    gfx.DrawLine(errorPen, centerPx, errorPx1, centerPx, errorPx2)
                    gfx.DrawLine(errorPen, capPx1, errorPx1, capPx2, errorPx1)
                    gfx.DrawLine(errorPen, capPx1, errorPx2, capPx2, errorPx2)
                End Using
            End If

            If ShowValuesAboveBars Then
                Dim belowBar As Boolean = (value < 0)
                Using valueTextFont As System.Drawing.Font = Drawing.GDI.Font(Font),
                    valueTextBrush As Brush = Drawing.GDI.Brush(Font.Color),
                    sf As New StringFormat() With {
                        .LineAlignment = If(belowBar, StringAlignment.Near, StringAlignment.Far),
                        .Alignment = StringAlignment.Center
                }
                    Dim y As Single = If(belowBar, rect.Bottom, rect.Top)
                    gfx.DrawString(ValueFormatter(value), valueTextFont, valueTextBrush, centerPx, y, sf)
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

            RenderBarFromRect(BarPlot.ClipRectToDataArea(rect, dims), value < 0, gfx)

            'errorbar
            Dim error1 As Double = If(value > 0, value2 - Math.Abs(valueError), value1 - Math.Abs(valueError))
            Dim error2 As Double = If(value > 0, value2 + Math.Abs(valueError), value1 + Math.Abs(valueError))
            Dim capPx1 As Single = dims.GetPixelY(position - ErrorCapSize * BarWidth / 2)
            Dim capPx2 As Single = dims.GetPixelY(position + ErrorCapSize * BarWidth / 2)
            Dim erroePx2 As Single = dims.GetPixelX(error2)
            Dim erroePx1 As Single = dims.GetPixelX(error1)

            If (ErrorLineWidth > 0) AndAlso (valueError > 0) Then
                Using errorPen As New Pen(ErrorColor, ErrorLineWidth)
                    gfx.DrawLine(errorPen, erroePx1, centerPx, erroePx2, centerPx)
                    gfx.DrawLine(errorPen, erroePx1, capPx2, erroePx1, capPx1)
                    gfx.DrawLine(errorPen, erroePx2, capPx2, erroePx2, capPx1)
                End Using
            End If

            If ShowValuesAboveBars Then
                Dim belowBar As Boolean = (value < 0)
                Using valueTextFont As System.Drawing.Font = Drawing.GDI.Font(Me.Font),
                    valueTextBrush As Brush = Drawing.GDI.Brush(Font.Color, Nothing, Drawing.HatchStyle.None)
                    Using sf As New StringFormat() With {
                        .LineAlignment = StringAlignment.Center,
                        .Alignment = If(belowBar, StringAlignment.Far, StringAlignment.Near)
                    }
                        Dim x As Single = If(belowBar, rect.Left, rect.Right)
                        gfx.DrawString(ValueFormatter(value), valueTextFont, valueTextBrush, x, centerPx, sf)
                    End Using
                End Using
            End If
        End Sub

        Protected Sub RenderBarFromRect(rect As RectangleF, negative As Boolean, gfx As Graphics)
            Using outlinePen As New Pen(BorderColor, BorderLineWidth),
                fillBrush As Brush = Drawing.GDI.Brush(If(negative, FillColorNegative, FillColor), FillColorHatch, HatchStyle)

                gfx.FillRectangle(fillBrush, rect.X, rect.Y, rect.Width, rect.Height)
                If (BorderLineWidth > 0) Then
                    gfx.DrawRectangle(outlinePen, rect.X, rect.Y, rect.Width, rect.Height)
                End If
            End Using
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableBar{lbl} with {Values.Length} bars."
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = FillColor,
                .LineWidth = 10,
                .MarkerShape = MarkerShape.None,
                .HatchColor = FillColorHatch,
                .HatchStyle = HatchStyle,
                .BorderColor = BorderColor,
                .BorderWith = BorderLineWidth}
            Return New LegendItem() {leg}
        End Function

        Private Shared Function ClipRectToDataArea(rect As RectangleF, dims As PlotDimensions) As RectangleF
            Dim left As Single = Math.Max(rect.Left, dims.DataOffsetX - 1)
            Dim right As Single = Math.Min(rect.Right, dims.DataOffsetX + dims.DataWidth + 1)
            Dim top As Single = Math.Max(rect.Top, dims.DataOffsetY - 1)
            Dim bot As Single = Math.Min(rect.Bottom, dims.DataOffsetY + dims.DataHeight + 1)
            Return New RectangleF(left, top, right - left, bot - top)
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Private Function IPlottable_GetAxisLimits() As AxisLimits Implements IPlottable.GetAxisLimits
            Throw New NotImplementedException()
        End Function

#End Region '/METHODS

    End Class

End Namespace