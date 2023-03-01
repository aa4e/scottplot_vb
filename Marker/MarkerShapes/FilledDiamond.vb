Imports System.Drawing

Namespace ScottPlot.MarkerShapes

	Public Structure FilledDiamond
		Implements IMarker

		Public Sub Draw(gfx As Graphics, center As PointF, size As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
			Dim points As PointF() = MarkerTools.DiamondPoints(center, size)
			gfx.FillPolygon(brush, points)
		End Sub

	End Structure

End Namespace