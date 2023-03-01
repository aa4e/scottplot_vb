Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure Eks
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, size As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			gfx.DrawLine(pen, center.X - size, center.Y - size, center.X + size, center.Y + size)
			gfx.DrawLine(pen, center.X - size, center.Y + size, center.X + size, center.Y - size)
		End Sub

	End Structure

End Namespace