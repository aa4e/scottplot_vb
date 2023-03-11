Imports System.Drawing

Namespace ScottPlot.Plottable

    Public MustInherit Class AxisSpan
        Implements IPlottable, IDraggable, IHasColor, IHasArea

#Region "PROPS, FIELDS"

        'Location and orientation
        Protected Position1 As Double
        Protected Position2 As Double
        Private ReadOnly IsHorizontal As Boolean
        Private EdgeUnderMouse As AxisSpan.Edge

        Private ReadOnly Property Min As Double
            Get
                Return Math.Min(Position1, Position2)
            End Get
        End Property

        Private ReadOnly Property Max As Double
            Get
                Return Math.Max(Position1, Position2)
            End Get
        End Property

        ''' <summary>
        ''' If true, AxisAuto() will ignore the position of this span when determining axis limits.
        ''' </summary>
        Public Property IgnoreAxisAuto As Boolean = False

        'Configuration
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property Color As Color = Color.FromArgb(128, Color.Magenta) Implements ScottPlot.Plottable.IHasColor.Color
        Public Property BorderColor As Color = Color.Transparent Implements ScottPlot.Plottable.IHasArea.BorderColor
        Public Property BorderLineWidth As Single = 0 Implements ScottPlot.Plottable.IHasArea.BorderLineWidth
        Public Property BorderLineStyle As LineStyle = LineStyle.None Implements ScottPlot.Plottable.IHasArea.BorderLineStyle
        Public Property HatchColor As Color = Color.Transparent Implements ScottPlot.Plottable.IHasArea.HatchColor
        Public Property HatchStyle As Drawing.HatchStyle = Drawing.HatchStyle.None Implements ScottPlot.Plottable.IHasArea.HatchStyle

        Public Property Label As String = String.Empty

        'Mouse interaction
        Public Property DragEnabled As Boolean Implements ScottPlot.Plottable.IDraggable.DragEnabled
        Public Property DragFixedSize As Boolean
        Public Property DragLimitMin As Double = Double.NegativeInfinity
        Public Property DragLimitMax As Double = Double.PositiveInfinity

        Public ReadOnly Property DragCursor As Cursor Implements ScottPlot.Plottable.IDraggable.DragCursor
            Get
                Return If(IsHorizontal, Cursor.WE, Cursor.NS)
            End Get
        End Property

        ''' <summary>
        ''' This event is invoked after the line is dragged.
        ''' </summary>
        Public Event Dragged As EventHandler Implements ScottPlot.Plottable.IDraggable.Dragged

        ''' <summary>
        ''' This event is invoked after the Edge1 is dragged.
        ''' </summary>
        Public Event Edge1Dragged As EventHandler(Of Double)

        ''' <summary>
        ''' This event is invoked after the Edge2 is dragged.
        ''' </summary>
        Public Event Edge2Dragged As EventHandler(Of Double)

        ''' <summary>
        ''' This event is invoked after the min edge is dragged.
        ''' </summary>
        Public Event MinDragged As EventHandler(Of Double)

        ''' <summary>
        ''' This event is invoked after the max edge is dragged.
        ''' </summary>
        Public Event MaxDragged As EventHandler(Of Double)

        ''' <summary>
        ''' This function applies snapping logic while dragging.
        ''' </summary>
        Public Property DragSnap As SnapLogic.ISnap2D = New SnapLogic.NoSnap2D() Implements ScottPlot.Plottable.IDraggable.DragSnap

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(isHorizontal As Boolean)
            Me.IsHorizontal = isHorizontal
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If Double.IsNaN(Position1) OrElse Double.IsInfinity(Position1) Then
                Throw New InvalidOperationException($"{NameOf(Position1)} must be a valid number.")
            End If
            If Double.IsNaN(Position2) OrElse Double.IsInfinity(Position2) Then
                Throw New InvalidOperationException($"{NameOf(Position2)} must be a valid number.")
            End If
        End Sub

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = Color,
                .BorderWith = Math.Min(BorderLineWidth, 3),
                .BorderColor = BorderColor,
                .BorderLineStyle = BorderLineStyle,
                .HatchColor = HatchColor,
                .HatchStyle = HatchStyle}
            Return New LegendItem() {leg}
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If IgnoreAxisAuto Then
                Return AxisLimits.NoLimits
            End If
            If IsHorizontal Then
                Return AxisLimits.HorizontalLimitsOnly(Min, Max)
            End If
            Return AxisLimits.VerticalLimitsOnly(Min, Max)
        End Function

        ''' <summary>
        ''' Return True if either span edge is within a certain number of pixels (snap) to the mouse.
        ''' </summary>
        ''' <param name="coordinateX">Mouse position (coordinate space).</param>
        ''' <param name="coordinateY">Mouse position (coordinate space).</param>
        ''' <param name="snapX">Snap distance (pixels).</param>
        ''' <param name="snapY">Snap distance (pixels).</param>
        Public Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean Implements ScottPlot.Plottable.IDraggable.IsUnderMouse
            If IsHorizontal Then
                If (Math.Abs(Position1 - coordinateX) <= snapX) Then
                    EdgeUnderMouse = AxisSpan.Edge.Edge1
                ElseIf (Math.Abs(Position2 - coordinateX) <= snapX) Then
                    EdgeUnderMouse = AxisSpan.Edge.Edge2
                Else
                    EdgeUnderMouse = AxisSpan.Edge.Neither
                End If
            Else
                If (Math.Abs(Position1 - coordinateY) <= snapY) Then
                    EdgeUnderMouse = AxisSpan.Edge.Edge1
                ElseIf (Math.Abs(Position2 - coordinateY) <= snapY) Then
                    EdgeUnderMouse = AxisSpan.Edge.Edge2
                Else
                    EdgeUnderMouse = AxisSpan.Edge.Neither
                End If
            End If
            Return (EdgeUnderMouse <> AxisSpan.Edge.Neither)
        End Function

        ''' <summary>
        ''' Move the span to a new coordinate in plot space.
        ''' </summary>
        ''' <param name="coordinateX">New X position.</param>
        ''' <param name="coordinateY">New Y position.</param>
        ''' <param name="fixedSize">if True, both edges will be moved to maintain the size of the span.</param>
        Public Sub DragTo(coordinateX As Double, coordinateY As Double, fixedSize As Boolean) Implements ScottPlot.Plottable.IDraggable.DragTo
            If DragEnabled Then
                Dim original As New Coordinate(coordinateX, coordinateY)
                Dim snapped As Coordinate = DragSnap.Snap(original)
                coordinateX = snapped.X
                coordinateY = snapped.Y

                Dim position1Changed As Boolean = False
                Dim position2Changed As Boolean = False

                If IsHorizontal Then
                    coordinateX = Math.Max(coordinateX, DragLimitMin)
                    coordinateX = Math.Min(coordinateX, DragLimitMax)
                Else
                    coordinateY = Math.Max(coordinateY, DragLimitMin)
                    coordinateY = Math.Min(coordinateY, DragLimitMax)
                End If

                Dim sizeBeforeDrag As Double = Position2 - Position1
                If (EdgeUnderMouse = AxisSpan.Edge.Edge1) Then
                    Position1 = If(IsHorizontal, coordinateX, coordinateY)
                    position1Changed = True
                    If DragFixedSize OrElse fixedSize Then
                        Position2 = Position1 + sizeBeforeDrag
                        position2Changed = True
                    End If
                ElseIf (EdgeUnderMouse = AxisSpan.Edge.Edge2) Then
                    Position2 = If(IsHorizontal, coordinateX, coordinateY)
                    position2Changed = True
                    If DragFixedSize OrElse fixedSize Then
                        Position1 = Position2 - sizeBeforeDrag
                        position1Changed = True
                    End If
                Else
                    Diagnostics.Debug.WriteLine("DragTo() called but no side selected. Call IsUnderMouse() to select a side.")
                End If

                'ensure fixed-width spans stay entirely inside the allowable range
                Dim belowLimit As Double = DragLimitMin - Position1
                Dim aboveLimit As Double = Position2 - DragLimitMax
                If (belowLimit > 0) Then
                    Position1 += belowLimit
                    Position2 += belowLimit
                    position1Changed = True
                    position2Changed = True
                End If
                If (aboveLimit > 0) Then
                    Position1 -= aboveLimit
                    Position2 -= aboveLimit
                    position1Changed = True
                    position2Changed = True
                End If

                RaiseEvent Dragged(Me, EventArgs.Empty)

                If position1Changed Then
                    RaiseEvent Edge1Dragged(Me, Position1)
                    If (Position1 <= Position2) Then
                        RaiseEvent MinDragged(Me, Position1)
                    Else
                        RaiseEvent MaxDragged(Me, Position1)
                    End If
                End If

                If position2Changed Then
                    RaiseEvent Edge2Dragged(Me, Position2)
                    If (Position2 <= Position1) Then
                        RaiseEvent MinDragged(Me, Position2)
                    Else
                        RaiseEvent MaxDragged(Me, Position2)
                    End If
                End If
            End If
        End Sub

        Private Function GetClippedRectangle(dims As PlotDimensions) As RectangleF
            'clip the rectangle to the size of the data area to avoid Drawing.GDI rendering errors
            Dim clippedPixelX = Function(x As Double)
                                    Return dims.GetPixelX(Math.Max(dims.XMin, Math.Min(x, dims.XMax)))
                                End Function
            Dim ClippedPixelY = Function(y As Double)
                                    Return dims.GetPixelY(Math.Max(dims.YMin, Math.Min(y, dims.YMax)))
                                End Function

            Dim left As Single = If(IsHorizontal, clippedPixelX(Min), dims.DataOffsetX)
            Dim right As Single = If(IsHorizontal, clippedPixelX(Max), dims.DataOffsetX + dims.DataWidth)
            Dim top = If(IsHorizontal, dims.DataOffsetY, ClippedPixelY(Max))
            Dim bottom = If(IsHorizontal, dims.DataOffsetY + dims.DataHeight, ClippedPixelY(Min))

            Dim width As Single = right - left + 1
            Dim height As Single = bottom - top + 1

            'expand slightly so anti-aliasing transparency is not observed at the edges of frameless plots
            If IsHorizontal Then
                top -= 1
                height += 2
            Else
                left -= 1
                width += 2
            End If
            Return New RectangleF(left, top, width, height)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                brush As Brush = Drawing.GDI.Brush(Color, HatchColor, HatchStyle),
                pen As Pen = Drawing.GDI.Pen(BorderColor, BorderLineWidth, BorderLineStyle)

                Dim rect As RectangleF = GetClippedRectangle(dims)
                gfx.FillRectangle(brush, rect)
                If (BorderLineWidth > 0) AndAlso (BorderColor <> Color.Transparent) AndAlso (BorderLineStyle <> LineStyle.None) Then
                    gfx.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
                End If
            End Using
        End Sub

#End Region '/METHODS

#Region "NESTED TYPES"

        Private Enum Edge
            Edge1
            Edge2
            Neither
        End Enum

#End Region '/NESTED TYPES

    End Class

End Namespace