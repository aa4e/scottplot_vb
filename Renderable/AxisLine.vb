Imports System.Drawing

Namespace ScottPlot.Renderable

    Public Class AxisLine
        Implements IRenderable

        Public Color As Color = Color.Black
        Public Width As Single = 1
        Public Edge As Edge
        Public PixelOffset As Single

        Public Property IsVisible As Boolean = True Implements ScottPlot.Renderable.IRenderable.IsVisible

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Renderable.IRenderable.Render
            If (Not IsVisible) Then
                Return
            End If

            Dim left As Single = dims.DataOffsetX
            Dim right As Single = dims.DataOffsetX + dims.DataWidth
            Dim top As Single = dims.DataOffsetY
            Dim bot As Single = dims.DataOffsetY + dims.DataHeight

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, False),
                pen As Pen = Drawing.GDI.Pen(Color, Width)
                Select Case Edge
                    Case Edge.Bottom
                        gfx.DrawLine(pen, left, bot + PixelOffset, right, bot + PixelOffset)
                    Case Edge.Left
                        gfx.DrawLine(pen, left - PixelOffset, bot, left - PixelOffset, top)
                    Case Edge.Right
                        gfx.DrawLine(pen, right + PixelOffset, bot, right + PixelOffset, top)
                    Case Edge.Top
                        gfx.DrawLine(pen, left, top - PixelOffset, right, top - PixelOffset)
                    Case Else
                        Throw New NotImplementedException()
                End Select
            End Using
        End Sub

    End Class

End Namespace