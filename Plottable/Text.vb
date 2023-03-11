Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Display a text label at an X/Y position in coordinate space.
    ''' </summary>
    Public Class Text
        Implements IPlottable, IHasPixelOffset, IDraggable, IHasColor

#Region "PROPS, FIELDS"

        Public X As Double
        Public Y As Double
        Public Label As String
        Public BackgroundFill As Boolean = False
        Public BackgroundColor As Color
        Public Font As New ScottPlot.Drawing.Font()

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return Font.Color
            End Get
            Set(value As Color)
                Font.Color = value
            End Set
        End Property

        Public Property FontName As String
            Get
                Return Font.Name
            End Get
            Set(value As String)
                Font.Name = value
            End Set
        End Property

        Public Property FontSize As Single
            Get
                Return Font.Size
            End Get
            Set(value As Single)
                Font.Size = value
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

        Public Property Alignment As Alignment
            Get
                Return Font.Alignment
            End Get
            Set(value As Alignment)
                Font.Alignment = value
            End Set
        End Property

        Public Property Rotation As Single
            Get
                Return Font.Rotation
            End Get
            Set(value As Single)
                Font.Rotation = value
            End Set
        End Property

        Public Property BorderSize As Single = 0
        Public Property BorderColor As Color = Color.Black
        Public Property PixelOffsetX As Single = 0 Implements ScottPlot.Plottable.IHasPixelOffset.PixelOffsetX
        Public Property PixelOffsetY As Single = 0 Implements ScottPlot.Plottable.IHasPixelOffset.PixelOffsetY
        Private Property LastRenderRectangleCoordinates As RectangleF
        Private Property DeltaCX As Double = 0
        Private Property DeltaCY As Double = 0

        Public Property DragSnap As SnapLogic.ISnap2D = New SnapLogic.NoSnap2D() Implements ScottPlot.Plottable.IDraggable.DragSnap

        ''' <summary>
        ''' Indicates whether this marker is draggable in user controls.
        ''' </summary>
        Public Property DragEnabled As Boolean Implements ScottPlot.Plottable.IDraggable.DragEnabled

        ''' <summary>
        ''' Cursor to display while hovering over this marker if dragging is enabled.
        ''' </summary>
        Public Property DragCursor As Cursor = Cursor.Hand Implements ScottPlot.Plottable.IDraggable.DragCursor

        ''' <summary>
        ''' If dragging is enabled the marker cannot be dragged more negative than this position.
        ''' </summary>
        Public DragXLimitMin As Double = Double.NegativeInfinity

        ''' <summary>
        ''' If dragging is enabled the marker cannot be dragged more positive than this position.
        ''' </summary>
        Public DragXLimitMax As Double = Double.PositiveInfinity

        ''' <summary>
        ''' If dragging is enabled the marker cannot be dragged more negative than this position.
        ''' </summary>
        Public DragYLimitMin As Double = Double.NegativeInfinity

        ''' <summary>
        ''' If dragging is enabled the marker cannot be dragged more positive than this position.
        ''' </summary>
        Public DragYLimitMax As Double = Double.PositiveInfinity

        ''' <summary>
        ''' This event is invoked after the marker is dragged.
        ''' </summary>
        Public Event Dragged As EventHandler Implements ScottPlot.Plottable.IDraggable.Dragged

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New()
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Overrides Function ToString() As String
            Return $"PlottableText ""{Label}"" at ({X}, {Y})."
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return New AxisLimits(X, X, Y, Y)
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If Double.IsNaN(X) OrElse Double.IsNaN(Y) Then
                Throw New InvalidOperationException("X and Y cannot be NaN.")
            End If
            If Double.IsInfinity(X) OrElse Double.IsInfinity(Y) Then
                Throw New InvalidOperationException("X and Y cannot be Infinity.")
            End If
            If String.IsNullOrWhiteSpace(Label) Then
                Throw New InvalidOperationException("Text cannot be null or whitespace.")
            End If
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible AndAlso (Not String.IsNullOrWhiteSpace(Label)) Then
                Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                    fnt As System.Drawing.Font = Drawing.GDI.Font(Font),
                    fontBrush As New SolidBrush(Font.Color),
                    frameBrush As New SolidBrush(BackgroundColor),
                    outlinePen As New Pen(BorderColor, BorderSize),
                    redPen As New Pen(Color.Red, BorderSize)

                    Dim pixelX As Single = dims.GetPixelX(Me.X) + PixelOffsetX
                    Dim pixelY As Single = dims.GetPixelY(Me.Y) - PixelOffsetY
                    Dim stringSize As SizeF = Drawing.GDI.MeasureString(gfx, Label, fnt)

                    gfx.TranslateTransform(pixelX, pixelY)
                    gfx.RotateTransform(Font.Rotation)

                    Dim valueTuple As Tuple(Of Single, Single) = Drawing.GDI.TranslateString(gfx, Label, Font)
                    Dim dX As Single = valueTuple.Item1
                    Dim dY As Single = valueTuple.Item2
                    gfx.TranslateTransform(-dX, -dY)

                    If BackgroundFill Then
                        Dim stringRect As New RectangleF(0, 0, stringSize.Width, stringSize.Height)
                        gfx.FillRectangle(frameBrush, stringRect)
                        If (BorderSize > 0) Then
                            gfx.DrawRectangle(outlinePen, stringRect.X, stringRect.Y, stringRect.Width, stringRect.Height)
                        End If
                    End If

                    gfx.DrawString(Label, fnt, fontBrush, New PointF(0, 0))
                    Drawing.GDI.ResetTransformPreservingScale(gfx, dims)

                    Dim degangle As Double = CDbl(Font.Rotation) * Math.PI / 180
                    Dim xA As Single = pixelX - dX
                    Dim yA As Single = pixelY - dY
                    Dim xC As Single = CSng(xA + stringSize.Width * Math.Cos(degangle) - stringSize.Height * Math.Sin(degangle))
                    Dim yC As Single = CSng(yA + stringSize.Height * Math.Cos(degangle) + stringSize.Width * Math.Sin(degangle))

                    Dim pointA As New PointF(xA, yA)
                    Dim pointC As New PointF(xC, yC)
                    LastRenderRectangleCoordinates = RectangleF.FromLTRB(CSng(dims.GetCoordinateX(pointA.X)),
                                                                         CSng(dims.GetCoordinateY(pointC.Y)),
                                                                         CSng(dims.GetCoordinateX(pointC.X)),
                                                                         CSng(dims.GetCoordinateY(pointA.Y)))
                End Using
            End If
        End Sub

        ''' <summary>
        ''' Move the marker to a new coordinate in plot space.
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

                If (coordinateX < DragXLimitMin) Then
                    coordinateX = DragXLimitMin
                End If
                If (coordinateX > DragXLimitMax) Then
                    coordinateX = DragXLimitMax
                End If
                If (coordinateX < DragYLimitMin) Then
                    coordinateY = DragYLimitMin
                End If
                If (coordinateX > DragYLimitMax) Then
                    coordinateY = DragYLimitMax
                End If
                X = coordinateX + DeltaCX
                Y = coordinateY + DeltaCY
                RaiseEvent Dragged(Me, EventArgs.Empty)
            End If
        End Sub

        ''' <summary>
        ''' Return True if the marker is within a certain number of pixels (snap) to the mouse.
        ''' </summary>
        ''' <param name="coordinateX">Mouse position (coordinate space).</param>
        ''' <param name="coordinateY">Mouse position (coordinate space).</param>
        ''' <param name="snapX">Snap distance (pixels).</param>
        ''' <param name="snapY">Snap distance (pixels).</param>
        Public Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean Implements ScottPlot.Plottable.IDraggable.IsUnderMouse
            Dim pt As New PointF(CSng(coordinateX), CSng(coordinateY))
            If Text.IsPointInsideRectangle(pt, LastRenderRectangleCoordinates) Then
                DeltaCX = X - coordinateX
                DeltaCY = Y - coordinateY
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' https://swharden.com/blog/2022-02-01-point-in-rectangle/
        ''' </remarks>
        Private Shared Function IsPointInsideRectangle(pt As PointF, rect As RectangleF) As Boolean
            Dim x1 As Double = rect.Left
            Dim x2 As Double = rect.Right
            Dim x3 As Double = rect.Right
            Dim x4 As Double = rect.Left

            Dim y1 As Double = rect.Top
            Dim y2 As Double = rect.Top
            Dim y3 As Double = rect.Bottom
            Dim y4 As Double = rect.Bottom

            Dim a1 As Double = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2))
            Dim a2 As Double = Math.Sqrt((x2 - x3) * (x2 - x3) + (y2 - y3) * (y2 - y3))
            Dim a3 As Double = Math.Sqrt((x3 - x4) * (x3 - x4) + (y3 - y4) * (y3 - y4))
            Dim a4 As Double = Math.Sqrt((x4 - x1) * (x4 - x1) + (y4 - y1) * (y4 - y1))

            Dim b1 As Double = Math.Sqrt((x1 - pt.X) * (x1 - pt.X) + (y1 - pt.Y) * (y1 - pt.Y))
            Dim b2 As Double = Math.Sqrt((x2 - pt.X) * (x2 - pt.X) + (y2 - pt.Y) * (y2 - pt.Y))
            Dim b3 As Double = Math.Sqrt((x3 - pt.X) * (x3 - pt.X) + (y3 - pt.Y) * (y3 - pt.Y))
            Dim b4 As Double = Math.Sqrt((x4 - pt.X) * (x4 - pt.X) + (y4 - pt.Y) * (y4 - pt.Y))

            Dim u1 As Double = (a1 + b1 + b2) / 2
            Dim u2 As Double = (a2 + b2 + b3) / 2
            Dim u3 As Double = (a3 + b3 + b4) / 2
            Dim u4 As Double = (a4 + b4 + b1) / 2

            Dim aa1 As Double = Math.Sqrt(u1 * (u1 - a1) * (u1 - b1) * (u1 - b2))
            Dim aa2 As Double = Math.Sqrt(u2 * (u2 - a2) * (u2 - b2) * (u2 - b3))
            Dim aa3 As Double = Math.Sqrt(u3 * (u3 - a3) * (u3 - b3) * (u3 - b4))
            Dim aa4 As Double = Math.Sqrt(u4 * (u4 - a4) * (u4 - b4) * (u4 - b1))

            Dim difference As Double = aa1 + aa2 + aa3 + aa4 - a1 * a2
            Return (difference < 0.001 * (rect.Height + rect.Width))
        End Function

#End Region '/METHODS

    End Class

End Namespace