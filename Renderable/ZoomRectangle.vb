Imports System.Drawing

Namespace ScottPlot.Renderable

    Public Class ZoomRectangle
        Implements IRenderable

        Private X As Single
        Private Y As Single
        Private Width As Single
        Private Height As Single

        Public Property FillColor As Color = Color.FromArgb(50, Color.Red)
        Public Property BorderColor As Color = Color.FromArgb(100, Color.Red)
        Public Property IsVisible As Boolean = True Implements ScottPlot.Renderable.IRenderable.IsVisible

        Public Sub Clear()
            IsVisible = False
        End Sub

        Public Sub [Set](x As Single, y As Single, width As Single, height As Single)
            Me.X = x
            Me.Y = y
            Me.Width = width
            Me.Height = height
            Me.IsVisible = True
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Renderable.IRenderable.Render
            If (Not IsVisible) Then
                Return
            End If
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, True, False),
                fillBrush As Brush = Drawing.GDI.Brush(FillColor),
                borderPen As Pen = Drawing.GDI.Pen(BorderColor)

                gfx.FillRectangle(fillBrush, X + dims.DataOffsetX, Y + dims.DataOffsetY, Width, Height)
                gfx.DrawRectangle(borderPen, X + dims.DataOffsetX, Y + dims.DataOffsetY, Width, Height)
            End Using
        End Sub

    End Class

End Namespace