Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' This interface is for plottable objects that could be styled using the plot's style. 
    ''' This is for things Like frames, tick marks, and text labels.
    ''' </summary>
    Public Interface IStylable

        Sub SetStyle(tickMarkColor As Color?, tickFontColor As Color?)

    End Interface

End Namespace