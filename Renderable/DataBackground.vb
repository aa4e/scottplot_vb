Imports System.Drawing

Namespace ScottPlot.Renderable

    Public Class DataBackground
        Implements IRenderable

        Public Property Color As Color = Color.White
        Public Property IsVisible As Boolean = True Implements ScottPlot.Renderable.IRenderable.IsVisible
        Public Property Bitmap As Bitmap = Nothing

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Renderable.IRenderable.Render
            If (Bitmap is Nothing) Then
                RenderSolidColorBackground(dims, bmp)
            Else
                RenderImageBackground(dims, bmp)
            End If
        End Sub

        Private Sub RenderSolidColorBackground(dims As PlotDimensions, bmp As Bitmap)
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, True, False),
                brush As Brush =Drawing.GDI.Brush(Color)
                Dim dataRect As New RectangleF(dims.DataOffsetX, dims.DataOffsetY, dims.DataWidth, dims.DataHeight)
                gfx.FillRectangle(brush, dataRect)
            End Using
        End Sub

        Private Sub RenderImageBackground(dims As PlotDimensions, bmp As Bitmap)
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, False, True)
                Dim x As Single = dims.DataOffsetX
                Dim y As Single = dims.DataOffsetY
                Dim width As Single = dims.DataWidth
                Dim height As Single = dims.DataHeight

                'NOTE: increase size by 1 source pixel to prevent anti-aliasing transparency at edges
                x -= width / Bitmap.Width
                y -= height / Bitmap.Height
                width += 2 * width / Bitmap.Width
                height += 2 * height / Bitmap.Height

                gfx.DrawImage(Bitmap, x, y, width, height)
            End Using
        End Sub

    End Class

End Namespace