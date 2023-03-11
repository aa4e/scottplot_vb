Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' An L-shaped scalebar rendered in the corner of the data area.
    ''' </summary>
    Public Class ScaleBar
        Implements IPlottable, IStylable, IHasColor

#Region "PROPS, FIELDS"

        Public ReadOnly Font As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' Width of the scalebar in cooridinate units.
        ''' </summary>
        Public Property Width As Double

        ''' <summary>
        ''' Height of the scalebar in cooridinate units.
        ''' </summary>
        Public Property Height As Double

        ''' <summary>
        ''' Distance in pixels from the edge of the data area.
        ''' </summary>
        Public Property Padding As Single = 10

        Public Property HorizontalLabel As String
        Public Property VerticalLabel As String
        Public Property LineWidth As Single = 2
        Public Property LineColor As Color = Color.Black

        Public Property FontSize As Single
            Get
                Return Font.Size
            End Get
            Set(value As Single)
                Font.Size = value
            End Set
        End Property

        Public Property FontColor As Color
            Get
                Return Font.Color
            End Get
            Set(value As Color)
                Font.Color = value
            End Set
        End Property

        Public Property FontBold As Boolean
            Get
                Return Font.Bold
            End Get
            Set(value As Boolean)
                Font.Bold = value
            End Set
        End Property

        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return LineColor
            End Get
            Set(value As Color)
                LineColor = value
                FontColor = value
            End Set
        End Property

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return AxisLimits.NoLimits
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Public Overrides Function ToString() As String
            Return $"PlottableScaleBar ({HorizontalLabel}={Width}, {VerticalLabel}={Height})"
        End Function

        Public Sub SetStyle(tickMarkColor As Color?, tickFontColor As Color?) Implements ScottPlot.Plottable.IStylable.SetStyle
            LineColor = If(tickMarkColor, LineColor)
            FontColor = If(tickFontColor, Font.Color)
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                fnt As System.Drawing.Font =Drawing.GDI.Font(Font),
                fontBrush As New SolidBrush(Font.Color),
                linePen As New Pen(LineColor, LineWidth),
                sfNorth As New StringFormat() With {.LineAlignment = StringAlignment.Near, .Alignment = StringAlignment.Center},
                sfWest As New StringFormat() With {.LineAlignment = StringAlignment.Center, .Alignment = StringAlignment.Near}

                'determine where the corner of the scalebar will be
                Dim widthPx As Single = CSng((Width * dims.PxPerUnitX))
                Dim heightPx As Single = CSng((Height * dims.PxPerUnitY))
                Dim cornerPoint As New PointF(dims.GetPixelX(dims.XMax) - Padding, dims.GetPixelY(dims.YMin) - Padding)

                'move the corner point away from the edge to accommodate label size
                Dim xLabelSize As SizeF =Drawing.GDI.MeasureString(gfx, HorizontalLabel, fnt)
                Dim yLabelSize As SizeF =Drawing.GDI.MeasureString(gfx, VerticalLabel, fnt)
                cornerPoint.X -= yLabelSize.Width * 1.2F
                cornerPoint.Y -= yLabelSize.Height

                'determine all other points relative to the corner point
                Dim horizPoint As New PointF(cornerPoint.X - widthPx, cornerPoint.Y)
                Dim vertPoint As New PointF(cornerPoint.X, cornerPoint.Y - heightPx)
                Dim horizMidPoint As New PointF((cornerPoint.X + horizPoint.X) / 2, cornerPoint.Y)
                Dim vertMidPoint As New PointF(cornerPoint.X, (cornerPoint.Y + vertPoint.Y) / 2)

                'draw the scalebar
                gfx.DrawLines(linePen, New PointF() {horizPoint, cornerPoint, vertPoint})
                gfx.DrawString(HorizontalLabel, fnt, fontBrush, horizMidPoint.X, cornerPoint.Y, sfNorth)
                gfx.DrawString(Me.VerticalLabel, fnt, fontBrush, cornerPoint.X, vertMidPoint.Y, sfWest)
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace