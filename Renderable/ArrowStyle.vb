Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace ScottPlot.Renderable

    Public Class ArrowStyle

        ''' <summary>
        ''' Describes which part of the vector line will be placed at the data coordinates.
        ''' </summary>
        Public Anchor As ArrowAnchor = ArrowAnchor.Center

        ''' <summary>
        ''' If enabled arrowheads will be drawn as lines scaled to each vector's magnitude.
        ''' </summary>
        Public ScaledArrowheads As Boolean

        ''' <summary>
        ''' When using scaled arrowheads this defines the width of the arrow relative to the vector line's length.
        ''' </summary>
        Public ScaledArrowheadWidth As Double = 0.15

        ''' <summary>
        ''' When using scaled arrowheads this defines length of the arrowhead relative to the vector line's length.
        ''' </summary>
        Public ScaledArrowheadLength As Double = 0.5

        ''' <summary>
        ''' Size of the arrowhead if custom/scaled arrowheads are not in use.
        ''' </summary>
        Public NonScaledArrowheadWidth As Single = 2

        ''' <summary>
        ''' Size of the arrowhead if custom/scaled arrowheads are not in use.
        ''' </summary>
        Public NonScaledArrowheadLength As Single = 2

        ''' <summary>
        ''' Marker drawn at each coordinate.
        ''' </summary>
        Public MarkerShape As MarkerShape = MarkerShape.FilledCircle

        ''' <summary>
        ''' Size of markers to be drawn at each coordinate.
        ''' </summary>
        Public MarkerSize As Single = 0

        ''' <summary>
        ''' Thickness of the arrow lines.
        ''' </summary>
        Public LineWidth As Single = 1

        ''' <summary>
        ''' Length of the scaled arrowhead.
        ''' </summary>
        Private ReadOnly Property ScaledTipLength As Double
            Get
                Return Math.Sqrt(ScaledArrowheadLength * ScaledArrowheadLength + ScaledArrowheadWidth * ScaledArrowheadWidth)
            End Get
        End Property

        ''' <summary>
        ''' Width of the scaled arrowhead.
        ''' </summary>
        Private ReadOnly Property ScaledHeadAngle As Double
            Get
                Return Math.Atan2(ScaledArrowheadWidth, ScaledArrowheadLength)
            End Get
        End Property

        ''' <summary>
        ''' Render an evenly-spaced 2D vector field.
        ''' </summary>
        Public Sub Render(dims As PlotDimensions, gfx As Graphics, xs As Double(), ys As Double(), vectors As Statistics.Vector2(,), colors As Color())
            For i As Integer = 0 To xs.Length - 1
                For j As Integer = 0 To ys.Length - 1
                    Dim coordinate As New Coordinate(xs(i), ys(j))
                    Dim vector As New CoordinateVector(vectors(i, j).X, vectors(i, j).Y)
                    Dim color As Color = colors(i * ys.Length + j)
                    RenderArrow(dims, gfx, coordinate, vector, color)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Render a single arrow placed anywhere in coordinace space.
        ''' </summary>
        Public Sub RenderArrow(dims As PlotDimensions, gfx As Graphics, pt As Coordinate, vec As CoordinateVector, color As Color)
            Dim x As Double = pt.X
            Dim y As Double = pt.Y
            Dim xVector As Double = vec.X
            Dim yVector As Double = vec.Y
            Dim tailX As Single
            Dim tailY As Single
            Dim endX As Single
            Dim endY As Single

            Select Case Anchor
                Case ArrowAnchor.Base
                    tailX = dims.GetPixelX(x)
                    tailY = dims.GetPixelY(y)
                    endX = dims.GetPixelX(x + xVector)
                    endY = dims.GetPixelY(y + yVector)

                Case ArrowAnchor.Center
                    tailX = dims.GetPixelX(x - xVector / 2)
                    tailY = dims.GetPixelY(y - yVector / 2)
                    endX = dims.GetPixelX(x + xVector / 2)
                    endY = dims.GetPixelY(y + yVector / 2)

                Case ArrowAnchor.Tip
                    tailX = dims.GetPixelX(x - xVector)
                    tailY = dims.GetPixelY(y - yVector)
                    endX = dims.GetPixelX(x)
                    endY = dims.GetPixelY(y)
                Case Else
                    Throw New NotImplementedException("Unsupported anchor type.")
            End Select

            Using pen As Pen = Drawing.GDI.Pen(color, LineWidth, LineStyle.Solid, True)
                If ScaledArrowheads Then
                    DrawFancyArrow(gfx, pen, tailX, tailY, endX, endY)
                Else
                    DrawStandardArrow(gfx, pen, tailX, tailY, endX, endY)
                End If
                DrawMarker(dims, gfx, pen, x, y)
            End Using
        End Sub

        Private Sub DrawMarker(dims As PlotDimensions, gfx As Graphics, pen As Pen, x As Double, y As Double)
            If (MarkerShape <> MarkerShape.None) AndAlso (MarkerSize > 0) Then
                Dim markerPoint As New PointF(dims.GetPixelX(x), dims.GetPixelY(y))
                MarkerTools.DrawMarker(gfx, markerPoint, MarkerShape, MarkerSize, pen.Color)
            End If
        End Sub

        Private Sub DrawStandardArrow(gfx As Graphics, pen As Pen, x1 As Single, y1 As Single, x2 As Single, y2 As Single)
            pen.CustomEndCap = New AdjustableArrowCap(NonScaledArrowheadWidth, NonScaledArrowheadLength)
            gfx.DrawLine(pen, x1, y1, x2, y2)
        End Sub

        Private Sub DrawFancyArrow(gfx As Graphics, pen As Pen, x1 As Single, y1 As Single, x2 As Single, y2 As Single)
            Dim dx As Single = x2 - x1
            Dim dy As Single = y2 - y1
            Dim arrowAngle = Math.Atan2(dy, dx)
            Dim sinA1 = Math.Sin(ScaledHeadAngle - arrowAngle)
            Dim cosA1 = Math.Cos(ScaledHeadAngle - arrowAngle)
            Dim sinA2 = Math.Sin(ScaledHeadAngle + arrowAngle)
            Dim cosA2 = Math.Cos(ScaledHeadAngle + arrowAngle)
            Dim len = Math.Sqrt(dx * dx + dy * dy)
            Dim hypLen = Len * ScaledTipLength

            Dim corner1X = x2 - hypLen * cosA1
            Dim corner1Y = y2 + hypLen * sinA1
            Dim corner2X = x2 - hypLen * cosA2
            Dim corner2Y = y2 - hypLen * sinA2

            Dim arrowPoints As PointF() = {
                New PointF(x1, y1),
                New PointF(x2, y2),
                New PointF(CSng(corner1X), CSng(corner1Y)),
                New PointF(x2, y2),
                New PointF(CSng(corner2X), CSng(corner2Y))
            }
            gfx.DrawLines(pen, arrowPoints)
        End Sub

    End Class

End Namespace