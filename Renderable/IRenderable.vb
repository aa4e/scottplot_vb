Imports System.Drawing

Namespace ScottPlot.Renderable

    ''' <summary>
    ''' A "renderable" is any object which can be drawn on the figure.
    ''' </summary>
    Public Interface IRenderable

        Property IsVisible As Boolean
        Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False)

    End Interface

End Namespace