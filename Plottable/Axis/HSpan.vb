Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Shaded horizontal region between two X values.
    ''' </summary>
    Public Class HSpan
        Inherits AxisSpan

        Public Property X1 As Double
            Get
                Return Position1
            End Get
            Set(value As Double)
                Position1 = value
            End Set
        End Property

        Public Property X2 As Double
            Get
                Return Position2
            End Get
            Set(value As Double)
                Position2 = value
            End Set
        End Property

        Public Sub New()
            MyBase.New(True)
        End Sub

        Public Overrides Function ToString() As String
            Return $"Horizontal span between Y1={X1} and Y2={X2}."
        End Function

    End Class

End Namespace