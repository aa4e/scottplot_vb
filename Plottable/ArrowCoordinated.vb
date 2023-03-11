Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' An arrow with X/Y coordinates for the base and the tip.
    ''' </summary>
    Public Class ArrowCoordinated
        Implements IPlottable, IHasPixelOffset, IHasLine, IHasColor

#Region "PROPS"

        ''' <summary>
        ''' Location of the arrow base in coordinate space.
        ''' </summary>
        Public ReadOnly Base As New Coordinate(0, 0)

        ''' <summary>
        ''' Location of the arrow base in coordinate space.
        ''' </summary>
        Public ReadOnly Tip As New Coordinate(0, 0)

        ''' <summary>
        ''' Color of the arrow and arrowhead.
        ''' </summary>
        Public Property Color As Color = Color.Black Implements ScottPlot.Plottable.IHasColor.Color

        ''' <summary>
        ''' Color of the arrow and arrowhead.
        ''' </summary>
        Public Property LineColor As Color Implements ScottPlot.Plottable.IHasLine.LineColor
            Get
                Return Me.Color
            End Get
            Set(value As Color)
                Me.Color = value
            End Set
        End Property

        ''' <summary>
        ''' Thickness of the arrow line.
        ''' </summary>
        Public Property LineWidth As Double = 2 Implements ScottPlot.Plottable.IHasLine.LineWidth

        ''' <summary>
        ''' Style of the arrow line.
        ''' </summary>
        Public Property LineStyle As LineStyle = LineStyle.Solid Implements ScottPlot.Plottable.IHasLine.LineStyle

        ''' <summary>
        ''' Label to appear in the legend.
        ''' </summary>
        Public Property Label As String

        ''' <summary>
        ''' Width of the arrowhead (pixels).
        ''' </summary>
        Public Property ArrowheadWidth As Double = 3

        ''' <summary>
        ''' Height of the arrowhead (pixels).
        ''' </summary>
        Public Property ArrowheadLength As Double = 3

        ''' <summary>
        ''' The arrow will be lengthened to ensure it is at least this size on the screen.
        ''' </summary>
        Public Property MinimumLengthPixels As Single = 0

        ''' <summary>
        ''' Marker to be drawn at the base (if <see cref="MarkerSize"/> > 0).
        ''' </summary>
        Public Property MarkerShape As MarkerShape = MarkerShape.FilledCircle

        ''' <summary>
        ''' Size of marker (in pixels) to draw at the base.
        ''' </summary>
        Public Property MarkerSize As Single = 0

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property PixelOffsetX As Single Implements ScottPlot.Plottable.IHasPixelOffset.PixelOffsetX
        Public Property PixelOffsetY As Single Implements ScottPlot.Plottable.IHasPixelOffset.PixelOffsetY

#End Region '/PROPS

#Region "CTORs"

        Public Sub New(arrowBase As Coordinate, arrowTip As Coordinate)
            Me.Base.X = arrowBase.X
            Me.Base.Y = arrowTip.Y
            Me.Tip.X = arrowTip.X
            Me.Tip.Y = arrowTip.Y
        End Sub

        Public Sub New(xBase As Double, yBase As Double, xTip As Double, yTip As Double)
            Me.Base.X = xBase
            Me.Base.Y = yBase
            Me.Tip.X = xTip
            Me.Tip.Y = yTip
        End Sub

#End Region '/CTORs

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Dim xMin As Double = Math.Min(Base.X, Tip.X)
            Dim xMax As Double = Math.Max(Base.X, Tip.X)
            Dim yMin As Double = Math.Min(Base.Y, Tip.Y)
            Dim yMax As Double = Math.Max(Base.Y, Tip.Y)
            Return New AxisLimits(xMin, xMax, yMin, yMax)
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return LegendItem.SingleItem(Me, Label)
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If (Not Base.IsFinite()) OrElse (Not Tip.IsFinite()) Then
                Throw New InvalidOperationException("Base and Tip coordinates must be finite.")
            End If
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                    pen As Pen = Drawing.GDI.Pen(Color, LineWidth, LineStyle, True)

                    Dim basePixel As Pixel = dims.GetPixel(Base)
                    Dim tipPixel As Pixel = dims.GetPixel(Tip)

                    basePixel.Translate(PixelOffsetX, -PixelOffsetY)
                    tipPixel.Translate(PixelOffsetX, -PixelOffsetY)

                    Dim lengthPixels As Single = basePixel.Distance(tipPixel)
                    If (lengthPixels < MinimumLengthPixels) Then
                        Dim expandBy As Single = MinimumLengthPixels / lengthPixels
                        Dim dx As Single = tipPixel.X - basePixel.X
                        Dim dy As Single = tipPixel.Y - basePixel.Y
                        basePixel.X = tipPixel.X - dx * expandBy
                        basePixel.Y = tipPixel.Y - dy * expandBy
                    End If

                    MarkerTools.DrawMarker(gfx, New PointF(basePixel.X, basePixel.Y), MarkerShape, MarkerSize, Color)

                    pen.CustomEndCap = New AdjustableArrowCap(CSng(ArrowheadWidth), CSng(ArrowheadLength), True)
                    pen.StartCap = LineCap.Flat
                    gfx.DrawLine(pen, basePixel.X, basePixel.Y, tipPixel.X, tipPixel.Y)
                End Using
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace