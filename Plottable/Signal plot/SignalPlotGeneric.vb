Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A signal plot displays evenly-spaced data. 
    ''' Instead of X/Y pairs, signal plots take Y values and a sample rate.
    ''' Optional X and Y offsets can further customize the data.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class SignalPlotGeneric(Of T As {Structure, IComparable})
        Inherits SignalPlotBase(Of T)

        Public Sub New()
            Me.Strategy = New MinMaxSearchStrategies.LinearMinMaxSearchStrategy(Of T)()
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableSignal{lbl} with {PointCount} points."
        End Function

    End Class

End Namespace