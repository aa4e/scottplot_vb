Namespace ScottPlot.Renderable

    Public Class DefaultBottomAxis
        Inherits Axis

        Public Sub New()
            Edge = Edge.Bottom
            Grid(True)
        End Sub

    End Class

    Public Class DefaultTopAxis
        Inherits Axis

        Public Sub New()
            Edge = Edge.Top
            AxisIndex = 1
            Grid(False)
            Ticks(False)
        End Sub

    End Class

    Public Class DefaultLeftAxis
        Inherits Axis

        Public Sub New()
            Edge = Edge.Left
            Grid(True)
        End Sub

    End Class

    Public Class DefaultRightAxis
        Inherits Axis

        Public Sub New()
            Edge = Edge.Right
            AxisIndex = 1
            Grid(False)
            Ticks(False)
        End Sub
    End Class

    Public Class AdditionalRightAxis
        Inherits Axis

        Public Sub New(yAxisIndex As Integer, title As String)
            Edge = Edge.Right
            AxisIndex = yAxisIndex
            Grid(False)
            Ticks(True)
            Label(title)
        End Sub
    End Class

    Public Class AdditionalLeftAxis
        Inherits Axis

        Public Sub New(yAxisIndex As Integer, title As String)
            Edge = Edge.Left
            AxisIndex = yAxisIndex
            Grid(False)
            Ticks(True)
            Label(title)
        End Sub
    End Class

    Public Class AdditionalTopAxis
        Inherits Axis

        Public Sub New(xAxisIndex As Integer, title As String)
            Edge = Edge.Top
            AxisIndex = xAxisIndex
            Grid(False)
            Ticks(True)
            Label(title)
        End Sub
    End Class

    Public Class AdditionalBottomAxis
        Inherits Axis

        Public Sub New(xAxisIndex As Integer, title As String)
            Edge = Edge.Bottom
            AxisIndex = xAxisIndex
            Grid(False)
            Ticks(True)
            Label(title)
        End Sub

    End Class

End Namespace