Imports System.Drawing

Namespace ScottPlot

    Public Interface IMarker

        Sub Draw(gfx As Graphics, center As PointF, radius As Single, brush As Brush, pen As Pen)

    End Interface

End Namespace