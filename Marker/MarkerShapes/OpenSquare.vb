Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure OpenSquare
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, radius As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			Dim rectangleF As New RectangleF(center.X - radius, center.Y - radius, radius + radius, radius + radius)
			gfx.DrawRectangle(pen, rectangleF.Left, rectangleF.Top, rectangleF.Width, rectangleF.Height)
		End Sub

	End Structure

End Namespace