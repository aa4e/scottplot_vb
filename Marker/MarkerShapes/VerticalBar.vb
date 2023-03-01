Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure VerticalBar
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, size As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			gfx.DrawLine(pen, center.X, center.Y - size, center.X, center.Y + size)
		End Sub

	End Structure

End Namespace