Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure HorizontalBar
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, size As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			gfx.DrawLine(pen, center.X - size, center.Y, center.X + size, center.Y)
		End Sub

	End Structure

End Namespace