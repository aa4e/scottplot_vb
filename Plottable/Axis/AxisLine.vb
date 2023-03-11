Imports System.Drawing

Namespace ScottPlot.Plottable

    Public MustInherit Class AxisLine
        Implements IDraggable, IPlottable, IHasLine, IHasColor

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Indicates whether the line is horizontal (position in Y units) or vertical (position in X units).
        ''' </summary>
        Private ReadOnly IsHorizontal As Boolean

        ''' <summary>
        ''' Location of the line (Y position if horizontal line, X position if vertical line).
        ''' </summary>
        Protected Property Position As Double

        ''' <summary>
        ''' If True, the position will be labeled on the axis using the <see cref="PositionFormatter"/>.
        ''' </summary>
        Public Property PositionLabel As Boolean

        ''' <summary>
        ''' Font to use for position labels (labels drawn over the axis).
        ''' </summary>
        Public ReadOnly PositionLabelFont As New ScottPlot.Drawing.Font() With {.Color = Color.White, .Bold = True}

        ''' <summary>
        ''' Color to use behind the position labels.
        ''' </summary>
        Public Property PositionLabelBackground As Color = Color.Black

        Public Property PositionLabelAlignmentX As HorizontalAlignment = HorizontalAlignment.Center
        Public Property PositionLabelAlignmentY As VerticalAlignment = VerticalAlignment.Middle

        ''' <summary>
        ''' If true the position label will be drawn on the right or top of the data area.
        ''' </summary>
        Public Property PositionLabelOppositeAxis As Boolean = False

        ''' <summary>
        ''' If provided, the position label will be rendered on this axis.
        ''' </summary>
        Public Property PositionLabelAxis As Renderable.Axis = Nothing

        ''' <summary>
        ''' This method generates the position label text for numeric (non-DateTime) axes.
        ''' For DateTime axes assign your own format string that uses DateTime.FromOADate(position).
        ''' </summary>
        ''' <returns></returns>
        Public Property PositionFormatter As Func(Of Double, String)

        ''' <summary>
        ''' Position of the axis line in DateTime (OADate) units.
        ''' </summary>
        Public Property DateTime As DateTime
            Get
                Return DateTime.FromOADate(Position)
            End Get
            Set(value As DateTime)
                Position = value.ToOADate()
            End Set
        End Property

        ''' <summary>
        ''' If true, <see cref="ScottPlot.Plot.AxisAuto"/> will ignore the position of this line when determining axis limits.
        ''' </summary>
        Public Property IgnoreAxisAuto As Boolean = False

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property LineStyle As LineStyle = LineStyle.Solid Implements ScottPlot.Plottable.IHasLine.LineStyle
        Public Property LineWidth As Double = 1 Implements ScottPlot.Plottable.IHasLine.LineWidth
        Public Property Color As Color = Color.Black Implements ScottPlot.Plottable.IHasColor.Color

        Public Property LineColor As Color Implements ScottPlot.Plottable.IHasLine.LineColor
            Get
                Return Color
            End Get
            Set(value As Color)
                Color = value
            End Set
        End Property

        ''' <summary>
        ''' Text that appears in the legend.
        ''' </summary>
        Public Property Label As String

        ''' <summary>
        ''' Indicates whether this line is draggable in user controls.
        ''' </summary>
        Public Property DragEnabled As Boolean = False Implements ScottPlot.Plottable.IDraggable.DragEnabled

        ''' <summary>
        ''' Cursor to display while hovering over this line if dragging is enabled.
        ''' </summary>
        Public ReadOnly Property DragCursor As Cursor Implements ScottPlot.Plottable.IDraggable.DragCursor
            Get
                If IsHorizontal Then
                    Return Cursor.NS
                End If
                Return Cursor.WE
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
                Return AxisLimits.VerticalLimitsOnly(Position, Position)
            End If
            Return AxisLimits.HorizontalLimitsOnly(Position, Position)
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If Double.IsNaN(Position) OrElse Double.IsInfinity(Position) Then
                Throw New InvalidOperationException("Position must be a valid number.")
            End If
            If (PositionFormatter Is Nothing) Then
                Throw New NullReferenceException("PositionFormatter")
            End If
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                If (LineWidth > 0) Then
                    RenderLine(dims, bmp, lowQuality)
                End If
                If PositionLabel Then
                    RenderPositionLabel(dims, bmp, lowQuality)
                End If
            End If
        End Sub

        Public Sub RenderLine(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
            pen As Pen = Drawing.GDI.Pen(Color, LineWidth, LineStyle, True)

                If IsHorizontal Then
                    Dim pixelY As Single = dims.GetPixelY(Position)
                    Dim xMin As Double = Math.Max(Min, dims.XMin)
                    Dim xMax As Double = Math.Min(Max, dims.XMax)
                    Dim pixelX1 As Single = dims.GetPixelX(xMin)
                    Dim pixelX2 As Single = dims.GetPixelX(xMax)
                    gfx.DrawLine(pen, pixelX1, pixelY, pixelX2, pixelY)
                Else
                    Dim pixelX As Single = dims.GetPixelX(Position)
                    Dim yMin As Double = Math.Max(Min, dims.YMin)
                    Dim yMax As Double = Math.Min(Max, dims.YMax)
                    Dim pixelY1 As Single = dims.GetPixelY(yMin)
                    Dim pixelY2 As Single = dims.GetPixelY(yMax)
                    gfx.DrawLine(pen, pixelX, pixelY1, pixelX, pixelY2)
                End If
            End Using
        End Sub

        Private Sub RenderPositionLabel(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, False),
                pen = Drawing.GDI.Pen(Color, LineWidth, LineStyle, True),
                fnt = Drawing.GDI.Font(PositionLabelFont),
                fillBrush = Drawing.GDI.Brush(PositionLabelBackground),
                fontBrush = Drawing.GDI.Brush(PositionLabelFont.Color)

                Dim axisOffset As Single = If(PositionLabelAxis IsNot Nothing, PositionLabelAxis.PixelOffset, 0)

                If IsHorizontal Then
                    If (Position <= dims.YMax) AndAlso (Position >= dims.YMin) Then
                        If PositionLabelOppositeAxis Then
                            Drawing.GDI.DrawLabel(gfx, PositionFormatter(Position),
                                                  dims.DataOffsetX + dims.DataWidth + axisOffset, dims.GetPixelY(Position),
                                                  PositionLabelFont.Name, PositionLabelFont.Size, PositionLabelFont.Bold,
                                                  HorizontalAlignment.Left, PositionLabelAlignmentY,
                                                  PositionLabelFont.Color, PositionLabelBackground)
                        Else
                            Drawing.GDI.DrawLabel(gfx, PositionFormatter(Position),
                                                  dims.DataOffsetX - axisOffset, dims.GetPixelY(Position),
                                                  PositionLabelFont.Name, PositionLabelFont.Size, PositionLabelFont.Bold,
                                                  HorizontalAlignment.Right, PositionLabelAlignmentY,
                                                  PositionLabelFont.Color, PositionLabelBackground)
                        End If
                    End If
                ElseIf (Position <= dims.XMax) AndAlso (Position >= dims.XMin) Then
                    If PositionLabelOppositeAxis Then
                        Drawing.GDI.DrawLabel(gfx, PositionFormatter(Position),
                                              dims.GetPixelX(Position), dims.DataOffsetY - axisOffset,
                                              PositionLabelFont.Name, PositionLabelFont.Size, PositionLabelFont.Bold,
                                              PositionLabelAlignmentX, VerticalAlignment.Lower,
                                              PositionLabelFont.Color, PositionLabelBackground)
                    Else
                        Drawing.GDI.DrawLabel(gfx, PositionFormatter(Position),
                                              dims.GetPixelX(Position), dims.DataOffsetY + dims.DataHeight + axisOffset,
                                              PositionLabelFont.Name, PositionLabelFont.Size, PositionLabelFont.Bold,
                                              PositionLabelAlignmentX, VerticalAlignment.Upper,
                                              PositionLabelFont.Color, PositionLabelBackground)
                    End If
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Move the line to a new coordinate in plot space.
        ''' </summary>
        ''' <param name="coordinateX">New X position.</param>
        ''' <param name="coordinateY">New Y position.</param>
        ''' <param name="fixedSize">This argument is ignored.</param>
        Public Sub DragTo(coordinateX As Double, coordinateY As Double, fixedSize As Boolean) Implements ScottPlot.Plottable.IDraggable.DragTo
            If DragEnabled Then
                Dim original As Coordinate = New Coordinate(coordinateX, coordinateY)
                Dim snapped As Coordinate = DragSnap.Snap(original)
                coordinateX = snapped.X
                coordinateY = snapped.Y

                If IsHorizontal Then
                    If (coordinateY < DragLimitMin) Then
                        coordinateY = DragLimitMin
                    End If
                    If (coordinateY > DragLimitMax) Then
                        coordinateY = DragLimitMax
                    End If
                    Position = coordinateY
                Else
                    If (coordinateX < DragLimitMin) Then
                        coordinateX = DragLimitMin
                    End If
                    If (coordinateX > DragLimitMax) Then
                        coordinateX = DragLimitMax
                    End If
                    Position = coordinateX
                End If
                RaiseEvent Dragged(Me, EventArgs.Empty)
            End If
        End Sub

        ''' <summary>
        ''' Return True if the line is within a certain number of pixels (snap) to the mouse.
        ''' </summary>
        ''' <param name="coordinateX">Mouse position (coordinate space).</param>
        ''' <param name="coordinateY">Mouse position (coordinate space).</param>
        ''' <param name="snapX">Snap distance (pixels).</param>
        ''' <param name="snapY">Snap distance (pixels).</param>
        Public Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean Implements ScottPlot.Plottable.IDraggable.IsUnderMouse
            Return If(IsHorizontal,
                Math.Abs(Position - coordinateY) <= snapY,
                Math.Abs(Position - coordinateX) <= snapX)
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

End Namespace