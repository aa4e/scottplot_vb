Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Namespace ScottPlot.Plottable

    <Obsolete("This plot type has been deprecated. Min/max and interpolation settings are exposed in the regular Heatmap.", True)>
    Public Class CoordinatedHeatmap
        Inherits Heatmap

        Protected Overrides Sub RenderHeatmap(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False)
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, True)

                gfx.InterpolationMode = Interpolation
                gfx.PixelOffsetMode = PixelOffsetMode.Half

                Dim drawFromX As Integer = CInt(Math.Round(dims.GetPixelX(XMin)))
                Dim drawFromY As Integer = CInt(Math.Round(dims.GetPixelY(YMax)))
                Dim drawWidth As Integer = CInt(Math.Round(dims.GetPixelX(XMax) - drawFromX))
                Dim drawHeight As Integer = CInt(Math.Round(dims.GetPixelY(YMin) - drawFromY))
                Dim destRect As New Rectangle(drawFromX, drawFromY, drawWidth, drawHeight)

                Dim attr As ImageAttributes = New ImageAttributes()
                attr.SetWrapMode(WrapMode.TileFlipXY)

                If (BackgroundImage IsNot Nothing) AndAlso (Not DisplayImageAbove) Then
                    gfx.DrawImage(BackgroundImage, destRect, 0, 0, BackgroundImage.Width, BackgroundImage.Height, GraphicsUnit.Pixel, attr)
                End If
                gfx.DrawImage(BmpHeatmap, destRect, 0, 0, BmpHeatmap.Width, BmpHeatmap.Height, GraphicsUnit.Pixel, attr)

                If (BackgroundImage IsNot Nothing) AndAlso DisplayImageAbove Then
                    gfx.DrawImage(BackgroundImage, destRect, 0, 0, BackgroundImage.Width, BackgroundImage.Height, GraphicsUnit.Pixel, attr)
                End If
            End Using
        End Sub

        Public Overrides Function GetAxisLimits() As AxisLimits
            Return New AxisLimits(XMin, XMax, YMin, YMax)
        End Function

    End Class

End Namespace