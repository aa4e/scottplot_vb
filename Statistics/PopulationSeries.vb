Imports System.Drawing

Namespace ScottPlot.Statistics

    ''' <summary>
    ''' A population series is a collection of similar PopulationStats objects.
    ''' </summary>
    Public Class PopulationSeries

        Public Populations As Population()
        Public SeriesLabel As String
        Public Color As Color

        Public Sub New(populations As Population(), Optional seriesLabel As String = Nothing, Optional color As Color? = Nothing)
            Me.Populations = populations
            Me.SeriesLabel = seriesLabel
            Me.Color = If(color is Nothing, System.Drawing.Color.LightGray, color.Value)
        End Sub

    End Class

End Namespace