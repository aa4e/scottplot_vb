Imports System.Linq

Namespace ScottPlot.Statistics

    Public Class PopulationMultiSeries

        Public MultiSeries As PopulationSeries()

        Public ReadOnly Property SeriesLabels As String()
            Get
                Return MultiSeries.Select(Function(x) x.SeriesLabel).ToArray()
            End Get
        End Property

        Public ReadOnly Property SeriesCount As Integer
            Get
                Return MultiSeries.Length
            End Get
        End Property

        Public ReadOnly Property GroupCount As Integer
            Get
                Return MultiSeries(0).Populations.Length
            End Get
        End Property

        Public Sub New(multiSeries As PopulationSeries())
            If (multiSeries is Nothing) Then
                Throw New ArgumentException("GroupedSeries cannot be null.")
            End If
            For Each series In multiSeries
                If (series.Populations.Length <> multiSeries(0).Populations.Length) Then
                    Throw New ArgumentException("All series must have the same number of populations.")
                End If
            Next
            Me.MultiSeries = multiSeries
        End Sub

    End Class

End Namespace