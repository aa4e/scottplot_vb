Imports System.Linq
Imports System.Drawing

Namespace ScottPlot.Renderable

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' The AxisTicks object contains:
    ''' - A TickCollection responsible for calculating tick positions and labels
    ''' - major tick label styling
    ''' - major/minor tick mark styling
    ''' - major/minor grid line styling
    ''' </remarks>
    Public Class AxisTicks
        Implements IRenderable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' The tick collection determines where ticks should go and what tick labels should say.
        ''' </summary>
        ''' <remarks>
        ''' TODO: store the TickCollection in the Axis module, not in the Ticks module.
        ''' https//github.com/ScottPlot/ScottPlot/pull/1647
        ''' </remarks>
        Public ReadOnly TickCollection As New Ticks.TickCollection()

        'tick label styling
        Public TickLabelVisible As Boolean = True
        Public TickLabelRotation As Single
        Public TickLabelFont As New ScottPlot.Drawing.Font() With {.Size = 11}
        Public TicksExtendOutward As Boolean = True

        'major tick/grid styling
        Public MajorTickVisible As Boolean = True
        Public MajorTickLength As Single = 5
        Public MajorTickColor As Color = Color.Black
        Public MajorGridVisible As Boolean
        Public MajorGridStyle As LineStyle = LineStyle.Solid
        Public MajorGridColor As Color = ColorTranslator.FromHtml("#efefef")
        Public MajorGridWidth As Single = 1

        'minor tick/grid styling
        Public MinorTickVisible As Boolean = True
        Public MinorTickLength As Single = 2.0F
        Public MinorTickColor As Color = Color.Black
        Public MinorGridVisible As Boolean
        Public MinorGridStyle As LineStyle = LineStyle.Solid
        Public MinorGridColor As Color = ColorTranslator.FromHtml("#efefef")
        Public MinorGridWidth As Single = 1

        'misc configuration
        Public Edge As Edge
        Public RulerMode As Boolean

        Public ReadOnly Property IsHorizontal As Boolean
            Get
                Return (Edge = Edge.Top) OrElse (Edge = Edge.Bottom)
            End Get
        End Property

        Public ReadOnly Property IsVertical As Boolean
            Get
                Return (Edge = Edge.Left) OrElse (Edge = Edge.Right)
            End Get
        End Property

        Public Property IsVisible As Boolean = True Implements ScottPlot.Renderable.IRenderable.IsVisible

        ''' <summary>
        ''' If true, grid lines will be drawn with anti-aliasing off to give the appearance of "snapping" to the 
        ''' nearest pixel and to avoid blurriness associated with drawing single-pixel anti-aliased lines.
        ''' </summary>
        Public SnapPx As Boolean = True

        Public PixelOffset As Single = 0

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Renderable.IRenderable.Render
            If (Not IsVisible) Then
                Return
            End If

            Dim majorTicks As Double() = TickCollection.GetVisibleMajorTicks(dims).Select(Function(t) t.Position).ToArray()
            Dim minorTicks As Double() = TickCollection.GetVisibleMinorTicks(dims).Select(Function(t) t.Position).ToArray()

            RenderTicksAndGridLines(dims, bmp, lowQuality OrElse SnapPx, majorTicks, minorTicks)
            RenderTickLabels(dims, bmp, lowQuality)
        End Sub

        Private Sub RenderTicksAndGridLines(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean, visibleMajorTicks As Double(), visibleMinorTicks As Double())
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, False)
                If MajorGridVisible Then
                    AxisTicksRender.RenderGridLines(dims, gfx, visibleMajorTicks, MajorGridStyle, MajorGridColor, MajorGridWidth, Edge)
                End If

                If MinorGridVisible Then
                    AxisTicksRender.RenderGridLines(dims, gfx, visibleMinorTicks, MinorGridStyle, MinorGridColor, MinorGridWidth, Edge)
                End If

                If (MinorTickVisible) Then
                    Dim tickLength As Single = If(TicksExtendOutward, MinorTickLength, -MinorTickLength)
                    AxisTicksRender.RenderTickMarks(dims, gfx, visibleMinorTicks, tickLength, MinorTickColor, Edge, PixelOffset)
                End If

                If MajorTickVisible Then
                    Dim tickLength As Single = MajorTickLength
                    If RulerMode Then
                        tickLength *= 4
                    End If
                    tickLength = If(TicksExtendOutward, tickLength, -tickLength)
                    AxisTicksRender.RenderTickMarks(dims, gfx, visibleMajorTicks, tickLength, MajorTickColor, Edge, PixelOffset)
                End If
            End Using

        End Sub

        Private Sub RenderTickLabels(dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, False)
                If TickLabelVisible Then
                    AxisTicksRender.RenderTickLabels(dims, gfx, TickCollection, TickLabelFont, Edge, TickLabelRotation, RulerMode, PixelOffset, MajorTickLength, MinorTickLength)
                End If
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace