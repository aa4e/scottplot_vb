Imports System.Drawing

Namespace ScottPlot.Plottable

    Public Class MarkerPlot
        Implements IPlottable, IHasMarker, IHasColor

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Font settings for rendering <see cref="Text"/>.
        ''' Alignment and orientation relative to the marker can be configured here.
        ''' </summary>
        Public TextFont As New ScottPlot.Drawing.Font()

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        ''' <summary>
        ''' Horizontal position in coordinate space.
        ''' </summary>
        Public Property X As Double

        ''' <summary>
        ''' Vertical position in coordinate space.
        ''' </summary>
        Public Property Y As Double

        ''' <summary>
        ''' Marker to draw at this point.
        ''' </summary>
        Public Property MarkerShape As MarkerShape = MarkerShape.FilledCircle Implements ScottPlot.Plottable.IHasMarker.MarkerShape

        ''' <summary>
        ''' Size of the marker in pixel units.
        ''' </summary>
        Public Property MarkerSize As Single = 10 Implements ScottPlot.Plottable.IHasMarker.MarkerSize

        ''' <summary>
        ''' Thickness of the marker lines in pixel units.
        ''' </summary>
        Public Property MarkerLineWidth As Single = 1 Implements ScottPlot.Plottable.IHasMarker.MarkerLineWidth

        ''' <summary>
        ''' Color of the marker to display at this point.
        ''' </summary>
        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color

        Public Property MarkerColor As Color Implements ScottPlot.Plottable.IHasMarker.MarkerColor
            Get
                Return Color
            End Get
            Set(value As Color)
                Color = value
            End Set
        End Property

        ''' <summary>
        ''' Text to appear in the legend (if populated).
        ''' </summary>
        Public Property Label As String

        ''' <summary>
        ''' Text to appear on the graph at the point.
        ''' </summary>
        Public Property Text As String = String.Empty

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return New AxisLimits(X, X, Y, Y)
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .MarkerShape = MarkerShape,
                .MarkerSize = MarkerSize,
                .Color = Color}
            Return New LegendItem() {leg}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            Validate.AssertIsReal("X", X)
            Validate.AssertIsReal("Y", Y)
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                Dim point As New PointF(dims.GetPixelX(X), dims.GetPixelY(Y))
                Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality)
                    MarkerTools.DrawMarker(gfx, point, MarkerShape, MarkerSize, Color, MarkerLineWidth)

                    If (Not String.IsNullOrEmpty(Text)) Then
                       Drawing.GDI.MeasureString(gfx, Text, TextFont.Name, CDbl(TextFont.Size), TextFont.Bold, Nothing)
                        gfx.TranslateTransform(point.X, point.Y)
                        gfx.RotateTransform(TextFont.Rotation)

                        Dim t As Tuple(Of Single, Single) =Drawing.GDI.TranslateString(gfx, Text, TextFont)
                        Dim dx As Single = t.Item1
                        Dim dy As Single = t.Item2
                        gfx.TranslateTransform(-dx, -dy)

                        Using fnt As System.Drawing.Font =Drawing.GDI.Font(TextFont),
                            fontBrush As New SolidBrush(TextFont.Color)
                            gfx.DrawString(Text, fnt, fontBrush, New PointF(0, 0))
                           Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
                        End Using
                    End If
                End Using
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace