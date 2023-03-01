Imports System
Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure Cross
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, radius As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			Dim rect As New RectangleF(center.X - radius, center.Y - radius, radius + radius, radius + radius)
			Dim centerX As Single = rect.Left + rect.Width / 2
			Dim centerY As Single = rect.Top + rect.Height / 2

			gfx.DrawLine(pen, rect.Left, centerY, rect.Right, centerY)
			gfx.DrawLine(pen, centerX, rect.Top, centerX, rect.Bottom)
		End Sub

	End Structure

End Namespace