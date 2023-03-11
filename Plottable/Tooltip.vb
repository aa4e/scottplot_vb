Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A tooltip displays a text bubble pointing to a specific location in X/Y space.
    ''' The position of the bubble moves according to the axis limits to best display the text in the data area.
    ''' </summary>
    Public Class Tooltip
        Implements IPlottable, IHasColor, IHittable

#Region "PROPS, FIELDS"

        Public ReadOnly Font As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' Bounding box of the tooltip the last time is was rendered (in coordinate units).
        ''' </summary>
        Private LastRenderRect As New CoordinateRect(0, 0, 0, 0)

        Public Property Label As String
        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property BorderColor As Color = Color.DarkGray
        Public Property BorderWidth As Single = 2
        Public Property FillColor As Color = Color.White

        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return FillColor
            End Get
            Set(value As Color)
                FillColor = value
            End Set
        End Property

        Public Property XAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        Public Property ArrowSize As Integer = 5
        Public Property LabelPadding As Integer = 10

        ''' <summary>
        ''' Tooltip position in coordinate space.
        ''' </summary>
        Public Property X As Double

        ''' <summary>
        ''' Tooltip position in coordinate space.
        ''' </summary>
        Public Property Y As Double

        ''' <summary>
        ''' Cursor to display when the tooltip is beneath the mouse.
        ''' </summary>
        Public Property HitCursor As Cursor = Cursor.Hand Implements ScottPlot.Plottable.IHittable.HitCursor

        Public Property HitTestEnabled As Boolean = True Implements ScottPlot.Plottable.IHittable.HitTestEnabled

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return AxisLimits.NoLimits
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If String.IsNullOrEmpty(Label) Then
                Throw New InvalidOperationException("Label may not be empty.")
            End If
            If Double.IsNaN(X) OrElse Double.IsInfinity(X) Then
                Throw New InvalidOperationException("X must be a real number.")
            End If
            If Double.IsNaN(Y) OrElse Double.IsInfinity(Y) Then
                Throw New InvalidOperationException("Y must be a real number.")
            End If
        End Sub

        Public Function HitTest(coord As Coordinate) As Boolean Implements ScottPlot.Plottable.IHittable.HitTest
            Return If(HitTestEnabled, LastRenderRect.Contains(coord), False)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                    fnt As System.Drawing.Font = Drawing.GDI.Font(Font),
                    fillBrush As Brush = Drawing.GDI.Brush(FillColor),
                    fontBrush As Brush = Drawing.GDI.Brush(Font.Color),
                    pen As Pen = Drawing.GDI.Pen(BorderColor, BorderWidth)

                    Dim labelSize As SizeF = gfx.MeasureString(Label, fnt)
                    Dim labelIsOnRight As Boolean = (dims.DataWidth - dims.GetPixelX(X) - labelSize.Width > 0)
                    Dim sign As Integer = If(labelIsOnRight, 1, -1)

                    Dim arrowHeadLocation As New PointF(dims.GetPixelX(X), dims.GetPixelY(Me.Y))

                    Dim contentBoxInsideEdgeX As Single = arrowHeadLocation.X + (sign * ArrowSize)
                    Dim upperArrowVertex As New PointF(contentBoxInsideEdgeX, arrowHeadLocation.Y - ArrowSize)
                    Dim lowerArrowVertex As New PointF(contentBoxInsideEdgeX, arrowHeadLocation.Y + ArrowSize)

                    Dim contentBoxTopEdge = upperArrowVertex.Y - LabelPadding
                    Dim contentBoxBottomEdge = Math.Max(contentBoxTopEdge + labelSize.Height, lowerArrowVertex.Y) + 2 * LabelPadding

                    Dim points As PointF() = New PointF() {
                        arrowHeadLocation,
                        upperArrowVertex,
                        New PointF(contentBoxInsideEdgeX, upperArrowVertex.Y - LabelPadding),
                        New PointF(contentBoxInsideEdgeX + sign * (labelSize.Width + LabelPadding), upperArrowVertex.Y - LabelPadding),
                        New PointF(contentBoxInsideEdgeX + sign * (labelSize.Width + LabelPadding), contentBoxBottomEdge),
                        New PointF(contentBoxInsideEdgeX, contentBoxBottomEdge),
                        lowerArrowVertex,
                        arrowHeadLocation,
                        upperArrowVertex 'add one more point to prevent render artifacts where thick line ends meet
                    }

                    Dim pathPointTypes As Byte() = Enumerable.Range(0, points.Length) _
                        .Select(Function(x) CByte(System.Drawing.Drawing2D.PathPointType.Line)).ToArray()

                    Dim path As New GraphicsPath(points, pathPointTypes)
                    gfx.FillPath(fillBrush, path)
                    gfx.DrawPath(pen, path)

                    Dim labelOffsetX = If(labelIsOnRight, 0, -labelSize.Width)
                    Dim labelX As Single = CSng(contentBoxInsideEdgeX + labelOffsetX + sign * LabelPadding / 2)
                    Dim labelY As Single = upperArrowVertex.Y
                    gfx.DrawString(Label, fnt, fontBrush, labelX, labelY)

                    'calculate where the tooltip is in coordinate units and save it for later hit detection
                    Dim corners As Coordinate() = points.Select(Function(pt As PointF) dims.GetCoordinate(pt.X, pt.Y)).ToArray()
                    LastRenderRect = CoordinateRect.BoundingBox(corners)
                End Using
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace