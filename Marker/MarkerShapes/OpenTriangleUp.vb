Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure OpenTriangleUp
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, size As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			Dim points As PointF() = MarkerTools.TriangleUpPoints(center, size)
			gfx.DrawPolygon(pen, points)
		End Sub

	End Structure

End Namespace