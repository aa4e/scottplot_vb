Namespace ScottPlot

    ''' <summary>
    ''' Represents direction and magnitude in coordinate space.
    ''' </summary>
    Public Structure CoordinateVector

        Public Property X As Double
        Public Property Y As Double

        Public Sub New(x As Double, y As Double)
            Me.X = x
            Me.Y = y
        End Sub

        Public ReadOnly Property Magnitude As Double
            Get
                Return Math.Sqrt(X * X + Y * Y)
            End Get
        End Property

        Public Shared Operator +(coordinate As Coordinate, vector As CoordinateVector) As Coordinate
            Return New Coordinate(coordinate.X + vector.X, coordinate.Y + vector.Y)
        End Operator

        Public Shared Operator -(coordinate As Coordinate, vector As CoordinateVector) As Coordinate
            Return New Coordinate(coordinate.X - vector.X, coordinate.Y - vector.Y)
        End Operator

    End Structure

End Namespace