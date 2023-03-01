Imports System.Drawing

Namespace ScottPlot.MarkerShapes

    Public Structure None
        Implements IMarker

        Public Sub Draw(gfx As Graphics, center As PointF, size As Single, brush As Brush, pen As Pen) Implements ScottPlot.IMarker.Draw
        End Sub

    End Structure

End Namespace