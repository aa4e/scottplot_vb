Imports System.Drawing

Namespace ScottPlot.Plottable

    Public MustInherit Class RepeatingAxisLine
        Implements IDraggable, IPlottable, IHasColor

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Location of the reference line (Y position if horizontal line, X position if vertical line).
        ''' </summary>
        Protected Position As Double

        ''' <summary>
        ''' Indicates whether the line is horizontal (position in Y units) or vertical (position in X units).
        ''' </summary>
        Private ReadOnly IsHorizontal As Boolean

        ''' <summary>
        ''' Total number of plotted lines.
        ''' </summary>
        Protected Property Count As Integer = 2

        ''' <summary>
        ''' Offset about Position (in Y position if horizontal line, in X position if vertical line), this offset should be negative.
        ''' </summary>
        Protected Property Offset As Integer = 0

        ''' <summary>
        ''' Shift between lines (in Y if horizontal line, in X if vertical line).
        ''' </summary>
        Protected Property Shift As Double = 1

        ''' <summary>
        ''' If RelativePosition is true, then the Shift is interpreted as a ratio of Position, otherwise it is an absolute shift along the axis.
        ''' </summary>
        Protected Property RelativePosition As Boolean = True

        ''' <summary>
        ''' If True, the position will be labeled on the axis using the PositionFormatter.
        ''' </summary>
        Public Property PositionLabel As Boolean

        ''' <summary>
        ''' If True, the first line (positioned at the specified X or Y) will be thicker.
        ''' </summary>
        Public Property HighlightReferenceLine As Boolean = True

        ''' <summary>
        ''' Font to use for position labels (labels drawn over the axis).
        ''' </summary>
        Public ReadOnly PositionLabelFont As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' Color to use behind the position labels.
        ''' </summary>
        Public Property PositionLabelBackground As Color = Color.Black

        ''' <summary>
        ''' If true the position label will be drawn on the right or top of the data area.
        ''' </summary>
        Public Property PositionLabelOppositeAxis As Boolean

        ''' <summary>
        ''' Position of the axis line in DateTime (OADate) units.
        ''' </summary>
        Public Property DateTime As DateTime
            Get
                Return DateTime.FromOADate(Me.Position)
            End Get
            Set(value As DateTime)
                Me.Position = value.ToOADate()
            End Set
        End Property

        ''' <summary>
        ''' If true, AxisAuto() will ignore the position of this line when determining axis limits.
        ''' </summary>
        Public Property IgnoreAxisAuto As Boolean = False

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property LineStyle As LineStyle = LineStyle.Solid
        Public Property LineWidth As Single = 1
        Public Property Color As Color = Color.Black Implements ScottPlot.Plottable.IHasColor.Color

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

        ''' <summary>
        ''' This method generates the position label text for numeric (non-DateTime) axes.
        ''' For DateTime axes assign your own format string that uses DateTime.FromOADate(position).
        ''' </summary>
        Public Property PositionFormatter As Func(Of Double, String) = Function(position) position.ToString("F2")

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(isHorizontal As Boolean)
            Me.IsHorizontal = isHorizontal
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If IgnoreAxisAuto Then
                Return AxisLimits.NoLimits
            End If
            If IsHorizontal Then
                Return AxisLimits.VerticalLimitsOnly(Position, Position)
            End If
            Return AxisLimits.HorizontalLimitsOnly(Position, Position)
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If Double.IsNaN(Position) OrElse Double.IsInfinity(Position) Then
                Throw New InvalidOperationException("Position must be a valid number.")
            End If
            If (PositionFormatter is Nothing) Then
                Throw New NullReferenceException("PositionFormatter")
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
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality),
            pen As Pen =Drawing.GDI.Pen(Color, 2 * LineWidth, LineStyle, True),
            pen2 As Pen =Drawing.GDI.Pen(Color, LineWidth, LineStyle, True)
                If IsHorizontal Then
                    For i As Integer = 0 To Count - 1
                        Dim pixelY As Single = dims.GetPixelY(ComputePosition(Position, Offset, Shift, i, RelativePosition))
                        Dim xMin As Double = Math.Max(Min, dims.XMin)
                        Dim xMax As Double = Math.Min(Max, dims.XMax)
                        Dim pixelX1 As Single = dims.GetPixelX(xMin)
                        Dim pixelX2 As Single = dims.GetPixelX(xMax)

                        If HighlightReferenceLine AndAlso (i + Offset = 0) Then
                            gfx.DrawLine(pen, pixelX1, pixelY, pixelX2, pixelY)
                        Else
                            gfx.DrawLine(pen2, pixelX1, pixelY, pixelX2, pixelY)
                        End If
                    Next
                    Return
                End If

                For i As Integer = 0 To Count - 1
                    Dim pixelX As Single = dims.GetPixelX(ComputePosition(Position, Offset, Shift, i, RelativePosition))
                    Dim yMin As Double = Math.Max(Min, dims.YMin)
                    Dim yMax As Double = Math.Min(Max, dims.YMax)
                    Dim pixelY1 As Single = dims.GetPixelY(yMin)
                    Dim pixelY2 As Single = dims.GetPixelY(yMax)

                    If HighlightReferenceLine AndAlso (i + Offset = 0) Then
                        gfx.DrawLine(pen, pixelX, pixelY1, pixelX, pixelY2)
                    Else
                        gfx.DrawLine(pen2, pixelX, pixelY1, pixelX, pixelY2)
                    End If
                Next
            End Using
        End Sub

        Private Sub RenderPositionLabel(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, False),
            pen =Drawing.GDI.Pen(Color, LineWidth, LineStyle, True),
            fnt As System.Drawing.Font =Drawing.GDI.Font(PositionLabelFont),
            fillBrush As Brush =Drawing.GDI.Brush(PositionLabelBackground),
            fontBrush As Brush =Drawing.GDI.Brush(PositionLabelFont.Color)

                If IsHorizontal Then
                    For i As Integer = 0 To Count - 1
                        Dim lineposition As Double = ComputePosition(Position, Offset, Shift, i, RelativePosition)
                        If (lineposition <= dims.YMax) AndAlso (lineposition >= dims.YMin) Then
                            Dim pixelY As Single = dims.GetPixelY(lineposition)
                            Dim yLabel As String = PositionFormatter(lineposition)
                            Dim yLabelSize As SizeF =Drawing.GDI.MeasureString(gfx, yLabel, PositionLabelFont)
                            Dim xPos As Single = If(PositionLabelOppositeAxis,
                                dims.DataOffsetX + dims.DataWidth,
                                dims.DataOffsetX - yLabelSize.Width)
                            Dim yPos As Single = pixelY - yLabelSize.Height / 2
                            Dim xLabelRect As New RectangleF(xPos, yPos, yLabelSize.Width, yLabelSize.Height)
                            gfx.FillRectangle(fillBrush, xLabelRect)
                            Dim sf As StringFormat =Drawing.GDI.StringFormat(HorizontalAlignment.Left, VerticalAlignment.Middle)
                            gfx.DrawString(yLabel, fnt, fontBrush, xPos, pixelY, sf)
                        End If
                    Next
                Else
                    For i As Integer = 0 To Count - 1
                        Dim lineposition As Double = ComputePosition(Position, Offset, Shift, i, RelativePosition)
                        If (lineposition <= dims.XMax) AndAlso (lineposition >= dims.XMin) Then
                            Dim pixelX As Single = dims.GetPixelX(lineposition)
                            Dim xLabel As String = PositionFormatter(lineposition)
                            Dim xLabelSize As SizeF =Drawing.GDI.MeasureString(gfx, xLabel, PositionLabelFont)
                            Dim xPos As Single = pixelX - xLabelSize.Width / 2
                            Dim yPos As Single = If(PositionLabelOppositeAxis,
                                dims.DataOffsetY - xLabelSize.Height,
                                dims.DataOffsetY + dims.DataHeight)
                            Dim xLabelRect As New RectangleF(xPos, yPos, xLabelSize.Width, xLabelSize.Height)
                            gfx.FillRectangle(fillBrush, xLabelRect)
                            Dim sf As StringFormat =Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Upper)
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
                Dim original As New Coordinate(coordinateX, coordinateY)
                Dim snapped As Coordinate = DragSnap.Snap(original)
                coordinateX = snapped.X
                coordinateY = snapped.Y

                If IsHorizontal Then
                    If (coordinateY < DragLimitMin) Then coordinateY = DragLimitMin
                    If (coordinateY > DragLimitMax) Then coordinateY = DragLimitMax
                    Position = coordinateY
                Else
                    If (coordinateX < DragLimitMin) Then coordinateX = DragLimitMin
                    If (coordinateX > DragLimitMax) Then coordinateX = DragLimitMax
                    Position = coordinateX
                End If
                RaiseEvent Dragged(Me, EventArgs.Empty)
            End If
        End Sub

        ''' <summary>
        ''' Return True if the reference line is within a certain number of pixels (snap) to the mouse
        ''' </summary>
        ''' <param name="coordinateX">Mouse position (coordinate space).</param>
        ''' <param name="coordinateY">Mouse position (coordinate space).</param>
        ''' <param name="snapX">Snap distance (pixels).</param>
        ''' <param name="snapY">Snap distance (pixels).</param>
        Public Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean Implements ScottPlot.Plottable.IDraggable.IsUnderMouse
            If IsHorizontal Then
                Return Math.Abs(Position - coordinateY) <= snapY
            End If
            Return Math.Abs(Position - coordinateX) <= snapX
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

        Private Function ComputePosition(Position As Double, Offset As Integer, Shift As Double, i As Integer, RelativePosition As Boolean) As Double
            If RelativePosition Then
                Return Position * (1 + (i + Offset) * Shift)
            End If
            Return Position + (i + Offset) * Shift
        End Function

#End Region '/METHODS

    End Class

    ''' <summary>
    ''' Repeating Vertical lines with refernce at an X position.
    ''' </summary>
    Public Class RepeatingVLine
        Inherits RepeatingAxisLine

        ''' <summary>
        ''' X position to render the line.
        ''' </summary>
        Public Property X As Double
            Get
                Return Position
            End Get
            Set(value As Double)
                Position = value
            End Set
        End Property

        Public Overloads Property Count As Integer
            Get
                Return MyBase.Count
            End Get
            Set(value As Integer)
                MyBase.Count = value
            End Set
        End Property

        Public Overloads Property Offset As Integer
            Get
                Return MyBase.Offset
            End Get
            Set(value As Integer)
                MyBase.Offset = value
            End Set
        End Property

        Public Overloads Property Shift As Double
            Get
                Return MyBase.Shift
            End Get
            Set(value As Double)
                MyBase.Shift = value
            End Set
        End Property

        Public Overloads Property Relativeposition As Boolean
            Get
                Return MyBase.RelativePosition
            End Get
            Set(value As Boolean)
                MyBase.RelativePosition = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"{Count} equispaced lines starting at X={X}, with an offset of {Offset} and {(If(Relativeposition, "a relative", "an absolute"))} shift of {Shift}."
        End Function

        Public Sub New()
            MyBase.New(False)
        End Sub

    End Class

    ''' <summary>
    ''' Repeating horizontHorizontal line at an Y position.
    ''' </summary>
    Public Class RepeatingHLine
        Inherits RepeatingAxisLine

        ''' <summary>
        ''' Y position to render the line.
        ''' </summary>
        Public Property Y As Double
            Get
                Return Position
            End Get
            Set(value As Double)
                Position = value
            End Set
        End Property

        Public Overloads Property Count As Integer
            Get
                Return MyBase.Count
            End Get
            Set(value As Integer)
                MyBase.Count = value
            End Set
        End Property

        Public Overloads Property Offset As Integer
            Get
                Return MyBase.Offset
            End Get
            Set(value As Integer)
                MyBase.Offset = value
            End Set
        End Property

        Public Overloads Property Shift As Double
            Get
                Return MyBase.Shift
            End Get
            Set(value As Double)
                MyBase.Shift = value
            End Set
        End Property

        Public Overloads Property Relativeposition As Boolean
            Get
                Return MyBase.RelativePosition
            End Get
            Set(value As Boolean)
                MyBase.RelativePosition = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"{Count} equispaced lines starting at Y={Y}, with an offset of {Offset} and {(If(Relativeposition, "a relative", "an absolute"))} shift of {Shift}."
        End Function

        Public Sub New()
            MyBase.New(True)
        End Sub

    End Class

End Namespace
