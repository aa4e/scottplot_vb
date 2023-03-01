Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure Hashtag
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, radius As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			Dim rectangleF As New RectangleF(center.X - radius, center.Y - radius, radius + radius, radius + radius)
			Dim centerX1 As Single = rectangleF.Left + rectangleF.Width * 0.33F
			Dim centerX2 As Single = rectangleF.Left + rectangleF.Width * 0.66F
			Dim centerY1 As Single = rectangleF.Top + rectangleF.Height * 0.33F
			Dim centerY2 As Single = rectangleF.Top + rectangleF.Height * 0.66F

			gfx.DrawLine(pen, centerX1, rectangleF.Bottom, centerX1, rectangleF.Top)
			gfx.DrawLine(pen, centerX2, rectangleF.Bottom, centerX2, rectangleF.Top)
			gfx.DrawLine(pen, rectangleF.Left, centerY1, rectangleF.Right, centerY1)
			gfx.DrawLine(pen, rectangleF.Left, centerY2, rectangleF.Right, centerY2)
		End Sub

	End Structure

End Namespace