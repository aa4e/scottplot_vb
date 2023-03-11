Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Horizontal line at a Y position.
    ''' </summary>
    Public Class HLine
        Inherits AxisLine

        Public Property Y As Double
            Get
                Return MyBase.Position
            End Get
            Set(value As Double)
                MyBase.Position = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"Horizontal line at Y={Y}."
        End Function

        Public Sub New()
            MyBase.New(True)
        End Sub

    End Class

End Namespace