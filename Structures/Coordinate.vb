Namespace ScottPlot

    ''' <summary>
    ''' Describes an X/Y position in coordinate space.
    ''' </summary>
    Public Structure Coordinate

        Public X As Double
        Public Y As Double

        Public Sub New(x As Double, y As Double)
            Me.X = x
            Me.Y = y
        End Sub

        Public Overrides Function ToString() As String
            Return $"(X={X}, Y={Y})"
        End Function

        ''' <summary>
        ''' True as lone as neither coordinate is NaN or Infinity.
        ''' </summary>
        Public Function IsFinite() As Boolean
            Return (Not Double.IsNaN(X)) _
                AndAlso (Not Double.IsNaN(Y)) _
                AndAlso (Not Double.IsInfinity(X)) _
                AndAlso (Not Double.IsInfinity(Y))
        End Function

        ''' <summary>
        ''' Return the distance to another coordinate (in coordinate units).
        ''' </summary>
        Public Function Distance(other As Coordinate) As Double
            Dim dX As Double = Math.Abs(other.X - Me.X)
            Dim dY As Double = Math.Abs(other.Y - Me.Y)
            Return Math.Sqrt(dX * dX + dY * dY)
        End Function

        Public Shared Function FromGeneric(Of T)(x As T, y As T) As Coordinate
            Return New Coordinate(NumericConversion.GenericToDouble(Of T)(x), NumericConversion.GenericToDouble(Of T)(y))
        End Function

        Public Function ToPixel(dims As PlotDimensions) As Pixel
            Return dims.GetPixel(Me)
        End Function

    End Structure

End Namespace