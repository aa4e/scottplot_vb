Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' This plot type is essentially the same as an Axis line, but it contains an array of positions.
    ''' All lines are styled the same, but they can be positioned (And dragged) independently.
    ''' </summary>
    Public MustInherit Class AxisLineVector
        Implements IPlottable, IDraggable, IHasLine, IHasColor

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Location of the line (Y position if horizontal line, X position if vertical line).
        ''' </summary>
        Protected Positions As Double()
        Public Property CurrentIndex As Integer = 0

        ''' <summary>
        ''' Add this value to each datapoint value before plotting (axis units).
        ''' </summary>
        Protected Property Offset As Integer = 0

        Public ReadOnly Property PointCount As Integer
            Get
                Return Positions.Length
            End Get
        End Property

        ''' <summary>
        ''' If True, the position will be labeled on the axis using the <see cref="PositionFormatter"/>.
        ''' </summary>
        Public Property PositionLabel As Boolean = False

        ''' <summary>
        ''' Font to use for position labels (labels drawn over the axis).
        ''' </summary>
        Public ReadOnly PositionLabelFont As New ScottPlot.Drawing.Font() With {.Color = Color.White, .Bold = True}

        ''' <summary>
        ''' Color to use behind the position labels.
        ''' </summary>
        Public Property PositionLabelBackground As Color = Color.Black

        ''' <summary>
        ''' If true the position label will be drawn on the right or top of the data area.
        ''' </summary>
        Public Property PositionLabelOppositeAxis As Boolean = False

        ''' <summary>
        ''' This method generates the position label text for numeric (non-DateTime) axes.
        ''' For DateTime axes assign your own format string that uses DateTime.FromOADate(position).
        ''' </summary>
        Public Property PositionFormatter As Func(Of Double, String)

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property Color As Color = Color.Black Implements ScottPlot.Plottable.IHasColor.Color

        Public Property LineColor As Color Implements ScottPlot.Plottable.IHasLine.LineColor
            Get
                Return Color
            End Get
            Set(value As Color)
                Color = value
            End Set
        End Property

        Public Property LineStyle As LineStyle = LineStyle.Solid Implements ScottPlot.Plottable.IHasLine.LineStyle
        Public Property MarkerShape As MarkerShape = MarkerShape.FilledCircle
        Public Property LineWidth As Double = 1 Implements ScottPlot.Plottable.IHasLine.LineWidth
        Public Property ErrorLineWidth As Single = 1
        Public Property ErrorCapSize As Single = 3
        Public Property MarkerSize As Single = 5
        Public Property StepDisplay As Boolean = False

        ''' <summary>
        ''' Indicates whether the line is horizontal (position in Y units) or vertical (position in X units).
        ''' </summary>
        Private ReadOnly IsHorizontal As Boolean

        ''' <summary>
        ''' If true, AxisAuto() will ignore the position of this line when determining axis limits.
        ''' </summary>
        Public Property IgnoreAxisAuto As Boolean = False

        ''' <summary>
        ''' Text that appears in the legend.
        ''' </summary>
        Public Property Label As String = String.Empty

        ''' <summary>
        ''' Indicates whether this line is draggable in user controls.
        ''' </summary>
        Public Property DragEnabled As Boolean = False Implements ScottPlot.Plottable.IDraggable.DragEnabled

        ''' <summary>
        ''' Cursor to display while hovering over this line if dragging is enabled.
        ''' </summary>
        Public ReadOnly Property DragCursor As Cursor Implements ScottPlot.Plottable.IDraggable.DragCursor
            Get
                Return If(IsHorizontal, Cursor.NS, Cursor.WE)
            End Get
        End Property

        ''' <summary>
        ''' If dragging is enabled the line cannot be dragged more negative than this position.
        ''' </summary>
        Public Property DragLimitMin As Double = Double.NegativeInfinity

        ''' <summary>
        ''' If dragging is enabled the line cannot be dragged more positive than this position.
        ''' </summary>
        Public Property DragLimitMax As Double = Double.PositiveInfinity

        ''' <summary>
        ''' This event is invoked after the line is dragged.
        ''' </summary>
        Public Event Dragged As EventHandler Implements ScottPlot.Plottable.IDraggable.Dragged

        ''' <summary>
        ''' The lower bound of the axis line.
        ''' </summary>
        Public Property Min As Double = Double.NegativeInfinity

        ''' <summary>
        ''' The upper bound of the axis line.
        ''' </summary>
        Public Property Max As Double = Double.PositiveInfinity

        ''' <summary>
        ''' This function applies snapping logic while dragging.
        ''' </summary>
        Public Property DragSnap As SnapLogic.ISnap2D = New SnapLogic.NoSnap2D() Implements ScottPlot.Plottable.IDraggable.DragSnap

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(isHorizontal As Boolean)
            Me.IsHorizontal = isHorizontal
            Me.PositionFormatter = Function(pos) pos.ToString("F2")
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If IgnoreAxisAuto Then
                Return AxisLimits.NoLimits
            End If
            If IsHorizontal Then
                Return AxisLimits.VerticalLimitsOnly(Positions.Min(), Positions.Max())
            End If
            Return AxisLimits.HorizontalLimitsOnly(Positions.Min(), Positions.Max())
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            Validate.AssertHasElements("Positions", Positions)
            If deep Then
                Validate.AssertAllReal("Positions", Positions)
            End If
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                RenderLine(dims, bmp, lowQuality)
                If PositionLabel Then
                    RenderPositionLabel(dims, bmp, lowQuality)
                End If
            End If
        End Sub

        Public Sub RenderLine(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Dim gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                pen As Pen = Drawing.GDI.Pen(Color, LineWidth, LineStyle, True)

            If IsHorizontal Then
                For i As Integer = 0 To PointCount - 1
                    Dim pixelY As Single = dims.GetPixelY(Positions(i) + Offset)
                    Dim xMin As Double = Math.Max(Min, dims.XMin)
                    Dim xMax As Double = Math.Min(Max, dims.XMax)
                    Dim pixelX1 As Single = dims.GetPixelX(xMin)
                    Dim pixelX2 As Single = dims.GetPixelX(xMax)
                    gfx.DrawLine(pen, pixelX1, pixelY, pixelX2, pixelY)
                Next
            Else
                For i As Integer = 0 To PointCount - 1
                    Dim pixelX As Single = dims.GetPixelX(Positions(i) + Offset)
                    Dim yMin As Double = Math.Max(Min, dims.YMin)
                    Dim yMax As Double = Math.Min(Max, dims.YMax)
                    Dim pixelY1 As Single = dims.GetPixelY(yMin)
                    Dim pixelY2 As Single = dims.GetPixelY(yMax)
                    gfx.DrawLine(pen, pixelX, pixelY1, pixelX, pixelY2)
                Next
            End If
        End Sub

        Private Sub RenderPositionLabel(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, False),
                pen = Drawing.GDI.Pen(Color, LineWidth, LineStyle, True),
                fnt As System.Drawing.Font = Drawing.GDI.Font(PositionLabelFont),
                fillBrush As Brush = Drawing.GDI.Brush(PositionLabelBackground, Nothing, Drawing.HatchStyle.None),
                fontBrush As Brush = Drawing.GDI.Brush(PositionLabelFont.Color, Nothing, Drawing.HatchStyle.None)

                If IsHorizontal Then
                    For i As Integer = 0 To PointCount - 1
                        Dim lineposition As Double = Positions(i) + Offset
                        If (lineposition <= dims.YMax) AndAlso (lineposition >= dims.YMin) Then
                            Dim pixelY As Single = dims.GetPixelY(lineposition)
                            Dim yLabel As String = PositionFormatter(lineposition)
                            Dim yLabelSize As SizeF = Drawing.GDI.MeasureString(gfx, yLabel, PositionLabelFont)
                            Dim xPos As Single = If(PositionLabelOppositeAxis, (dims.DataOffsetX + dims.DataWidth), (dims.DataOffsetX - yLabelSize.Width))
                            Dim yPos As Single = pixelY - yLabelSize.Height / 2.0F
                            Dim xLabelRect As New RectangleF(xPos, yPos, yLabelSize.Width, yLabelSize.Height)
                            gfx.FillRectangle(fillBrush, xLabelRect)
                            Dim sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Left, VerticalAlignment.Middle)
                            gfx.DrawString(yLabel, fnt, fontBrush, xPos, pixelY, sf)
                        End If
                    Next
                Else
                    For i As Integer = 0 To PointCount - 1
                        Dim lineposition As Double = Positions(i) + Offset
                        If (lineposition <= dims.XMax) AndAlso (lineposition >= dims.XMin) Then
                            Dim pixelX As Single = dims.GetPixelX(lineposition)
                            Dim xLabel As String = PositionFormatter(lineposition)
                            Dim xLabelSize As SizeF = Drawing.GDI.MeasureString(gfx, xLabel, PositionLabelFont)
                            Dim xPos As Single = pixelX - xLabelSize.Width / 2.0F
                            Dim yPos As Single = If(PositionLabelOppositeAxis, (dims.DataOffsetY - xLabelSize.Height), (dims.DataOffsetY + dims.DataHeight))
                            Dim xLabelRect As New RectangleF(xPos, yPos, xLabelSize.Width, xLabelSize.Height)
                            gfx.FillRectangle(fillBrush, xLabelRect)
                            Dim sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Upper)
                            gfx.DrawString(xLabel, fnt, fontBrush, pixelX, yPos, sf)
                        End If
                    Next
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Move the reference line to a New coordinate in plot space.
        ''' </summary>
        ''' <param name="coordinateX">New X position.</param>
        ''' <param name="coordinateY">New Y position.</param>
        ''' <param name="fixedSize">This argument is ignored.</param>
        Public Sub DragTo(coordinateX As Double, coordinateY As Double, fixedSize As Boolean) Implements ScottPlot.Plottable.IDraggable.DragTo
            If DragEnabled Then
                Dim original As Coordinate = New Coordinate(coordinateX, coordinateY)
                Dim snapped As Coordinate = Me.DragSnap.Snap(original)
                coordinateX = snapped.X
                coordinateY = snapped.Y

                If IsHorizontal Then
                    If (coordinateY < DragLimitMin) Then
                        coordinateY = DragLimitMin
                    End If
                    If (coordinateY > DragLimitMax) Then
                        coordinateY = DragLimitMax
                    End If
                    Positions(CurrentIndex) = coordinateY
                Else
                    If (coordinateX < DragLimitMin) Then
                        coordinateX = DragLimitMin
                    End If
                    If (coordinateX > DragLimitMax) Then
                        coordinateX = DragLimitMax
                    End If
                    Positions(CurrentIndex) = coordinateX
                End If
                RaiseEvent Dragged(Me, EventArgs.Empty)
            End If
        End Sub

        ''' <summary>
        ''' Return True if the reference line is within a certain number of pixels (snap) to the mouse.
        ''' </summary>
        ''' <param name="coordinateX">Mouse position (coordinate space).</param>
        ''' <param name="coordinateY">Mouse position (coordinate space).</param>
        ''' <param name="snapX">Snap distance (pixels).</param>
        ''' <param name="snapY">Snap distance (pixels).</param>
        Public Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean Implements ScottPlot.Plottable.IDraggable.IsUnderMouse
            If IsHorizontal Then
                For i As Integer = 0 To PointCount - 1
                    If (Math.Abs(Positions(i) - coordinateY) <= snapY) Then
                        CurrentIndex = i
                        Return True
                    End If
                Next
            Else
                For i As Integer = 0 To PointCount - 1
                    If (Math.Abs(Positions(i) - coordinateX) <= snapX) Then
                        CurrentIndex = i
                        Return True
                    End If
                Next
            End If
            Return False
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = Color,
                .LineStyle = LineStyle,
                .LineWidth = LineWidth,
                .MarkerShape = MarkerShape.None}
            Return New LegendItem() {leg}
        End Function

#End Region '/METHODS

    End Class


    ''' <summary>
    ''' Vertical line at an X position.
    ''' </summary>
    Public Class VLineVector
        Inherits AxisLineVector

        ''' <summary>
        ''' X position to render the line.
        ''' </summary>
        Public Property Xs As Double()
            Get
                Return Positions
            End Get
            Set(value As Double())
                Positions = value
            End Set
        End Property

        Public Shadows Property Offset As Integer
            Get
                Return MyBase.Offset
            End Get
            Set(value As Integer)
                MyBase.Offset = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"{PointCount} lines positions X={Xs}, with an offset of {Offset}."
        End Function

        Public Sub New()
            MyBase.New(False)
        End Sub

    End Class

    ''' <summary>
    ''' Horizontal line at an Y position.
    ''' </summary>
    Public Class HLineVector
        Inherits AxisLineVector

        ''' <summary>
        ''' Y position to render the line.
        ''' </summary>
        Public Property Ys As Double()
            Get
                Return Positions
            End Get
            Set(value As Double())
                Positions = value
            End Set
        End Property

        Public Shadows Property Offset As Integer
            Get
                Return MyBase.Offset
            End Get
            Set(value As Integer)
                MyBase.Offset = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"{PointCount} lines positions Y={Ys}, with an offset of {Offset}."
        End Function

        Public Sub New()
            MyBase.New(True)
        End Sub

    End Class

End Namespace