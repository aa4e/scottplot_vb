Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure OpenTriangleDown
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, size As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			Dim points As PointF() = MarkerTools.TriangleDownPoints(center, size)
			gfx.DrawPolygon(pen, points)
		End Sub

	End Structure

End Namespace