Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Vertical line at an X position.
    ''' </summary>
    Public Class VLine
        Inherits AxisLine

        ''' <summary>
        ''' X position to render the line.
        ''' </summary>
        Public Property X As Double
            Get
                Return MyBase.Position
            End Get
            Set(value As Double)
                MyBase.Position = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"Vertical line at X={X}."
        End Function

        Public Sub New()
            MyBase.New(False)
        End Sub

    End Class

End Namespace