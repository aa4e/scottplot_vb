Namespace ScottPlot.Control

    ''' <summary>
    ''' Interactive ScottPlot controls implement this interface.
    ''' </summary>
    Public Interface IPlotControl

        ''' <summary>
        ''' The plot displayed by this control
        ''' </summary>
        ReadOnly Property Plot As Plot

        ''' <summary>
        ''' Redraw the plot and display it in the control
        ''' </summary>
        Sub Refresh()

        ''' <summary>
        ''' Configuration object holding advanced control options
        ''' </summary>
        ReadOnly Property Configuration As Configuration

    End Interface

End Namespace