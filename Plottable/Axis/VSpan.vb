Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Shade the region between two Y values.
    ''' </summary>
    Public Class VSpan
        Inherits AxisSpan

        Public Property Y1 As Double
            Get
                Return Position1
            End Get
            Set(value As Double)
                Position1 = value
            End Set
        End Property

        Public Property Y2 As Double
            Get
                Return Position2
            End Get
            Set(value As Double)
                Position2 = value
            End Set
        End Property

        Public Sub New()
            MyBase.New(False)
        End Sub

        Public Overrides Function ToString() As String
            Return $"Vertical span between X1={Y1} and X2={Y2}."
        End Function

    End Class

End Namespace