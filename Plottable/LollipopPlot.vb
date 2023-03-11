Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Lollipop plots display a series of "Lollipops" in place of bars. 
    ''' Positions are defined by Xs.
    ''' Heights are defined by Ys (relative to BaseValue and YOffsets).
    ''' </summary>
    Public Class LollipopPlot
        Inherits BarPlotBase
        Implements IPlottable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Name for this series of values that will appear in the legend.
        ''' </summary>
        Public Property Label As String = ""

        ''' <summary>
        ''' Color of all lollipop components (the stick and the circle).
        ''' </summary>
        Public Property LollipopColor As Color

        ''' <summary>
        ''' Size of the circle at the end of each lollipop.
        ''' </summary>
        Public Property LollipopRadius As Single = 5

        Public Property IsVisible As Boolean = True Implements IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements IPlottable.YAxisIndex

#End Region '/PROPS, FIELDS

#Region "CTOR"

        ''' <summary>
        ''' Create a lollipop plot from arrays of positions and sizes.
        ''' </summary>
        ''' <param name="positions">Position of each lollipop.</param>
        ''' <param name="values">Height of each lollipop.</param>
        Public Sub New(positions As Double(), values As Double())
            If (positions is Nothing) OrElse (positions.Length = 0) OrElse (values is Nothing) OrElse (values.Length = 0) Then
                Throw New InvalidOperationException("Xs and Ys must be arrays that contains elements.")
            End If
            If (values.Length <> positions.Length) Then
                Throw New InvalidOperationException("Xs and Ys must have the same number of elements.")
            End If
            ValueErrors = DataGen.Zeros(values.Length)
            ValueOffsets = DataGen.Zeros(values.Length)
            Me.Values = values
            Me.Positions = If(positions, DataGen.Consecutive(values.Length, 1.0, 0.0))
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function GetAxisLimits() As AxisLimits Implements IPlottable.GetAxisLimits
            Return MyBase.GetAxisLimits()
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = LollipopColor,
                .LineWidth = 10,
                .MarkerShape = MarkerShape.None,
                .BorderColor = BorderColor,
                .BorderWith = 1}
            Return New LegendItem() {leg}
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality)
                For barIndex As Integer = 0 To Values.Length - 1
                    If (Orientation = Orientation.Vertical) Then
                        RenderBarVertical(dims, gfx, Positions(barIndex) + PositionOffset, Values(barIndex), ValueErrors(barIndex), ValueOffsets(barIndex))
                    Else
                        RenderBarHorizontal(dims, gfx, Positions(barIndex) + PositionOffset, Values(barIndex), ValueErrors(barIndex), ValueOffsets(barIndex))
                    End If
                Next
            End Using
        End Sub

        Private Sub RenderBarFromRect(rect As RectangleF, negative As Boolean, gfx As Graphics)
            Dim centerPx As Single = If(Orientation = Orientation.Horizontal,
                rect.Y + rect.Height / 2,
                rect.X + rect.Width / 2)

            Using fillPen As New Pen(LollipopColor),
                fillBrush As Brush =Drawing.GDI.Brush(LollipopColor)
                If (Orientation = Orientation.Horizontal) Then
                    gfx.FillEllipse(fillBrush, If(negative, rect.X, rect.X + rect.Width), centerPx - LollipopRadius / 2, LollipopRadius, LollipopRadius)
                    gfx.DrawLine(fillPen, rect.X, centerPx, rect.X + rect.Width, centerPx)
                Else
                    gfx.FillEllipse(fillBrush, centerPx - LollipopRadius / 2, If(Not negative, rect.Y, rect.Y + rect.Height), LollipopRadius, LollipopRadius)
                    gfx.DrawLine(fillPen, centerPx, rect.Y, centerPx, rect.Y + rect.Height)
                End If
            End Using
        End Sub

        Private Sub RenderBarVertical(dims As PlotDimensions, gfx As Graphics, position As Double, value As Double, valueError As Double, yOffset As Double)
            'bar body
            Dim centerPx As Single = dims.GetPixelX(position)
            Dim edge1 As Double = position - BarWidth / 2
            Dim value1 As Double = Math.Min(ValueBase, value) + yOffset
            Dim value2 As Double = Math.Max(ValueBase, value) + yOffset
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
                Using valueTextFont As System.Drawing.Font =Drawing.GDI.Font(Font),
                    valueTextBrush As Brush =Drawing.GDI.Brush(Font.Color),
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
                Using valueTextFont As System.Drawing.Font =Drawing.GDI.Font(Font),
                    valueTextBrush As Brush =Drawing.GDI.Brush(Font.Color),
                    sf As New StringFormat() With {.LineAlignment = StringAlignment.Center, .Alignment = StringAlignment.Near}
                    gfx.DrawString(ValueFormatter(value), valueTextFont, valueTextBrush, rect.X + rect.Width, centerPx, sf)
                End Using
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace