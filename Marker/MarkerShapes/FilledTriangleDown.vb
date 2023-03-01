Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure FilledTriangleDown
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, size As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			Dim points As PointF() = MarkerTools.TriangleDownPoints(center, size)
			gfx.FillPolygon(brush, points)
		End Sub

	End Structure

End Namespace