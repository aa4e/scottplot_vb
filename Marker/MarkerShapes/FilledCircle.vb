Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure FilledCircle
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, radius As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			Dim rect As New RectangleF(center.X - radius, center.Y - radius, radius + radius, radius + radius)
			gfx.FillEllipse(brush, rect)
		End Sub

	End Structure

End Namespace