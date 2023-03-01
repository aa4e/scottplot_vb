Imports System.Drawing

Namespace ScottPlot.MarkerShapes

    Public Structure Asterisk
        Implements IMarker

        Public Sub Draw(gfx As Graphics, center As PointF, radius As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
            Dim rect As New RectangleF(center.X - radius, center.Y - radius, radius + radius, radius + radius)
            Dim centerX As Single = rect.Left + rect.Width / 2
            Dim centerY As Single = rect.Top + rect.Height / 2
            Dim dsize As Single = 0.707F * rect.Width / 2

            gfx.DrawLine(pen, centerX, rect.Bottom, centerX, rect.Top)
            gfx.DrawLine(pen, rect.Left, centerY, rect.Right, centerY)
            gfx.DrawLine(pen, centerX - dsize, centerY - dsize, centerX + dsize, centerY + dsize)
            gfx.DrawLine(pen, centerX - dsize, centerY + dsize, centerX + dsize, centerY - dsize)
        End Sub

    End Structure

End Namespace