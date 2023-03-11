Namespace ScottPlot

    ''' <summary>
    ''' This object describes the 4 edges of a rectangular view in 2D space.
    ''' Values may contain NaN to describe undefined or uninitialized edges.
    ''' </summary>
    Public Structure AxisLimits
        Implements IEquatable(Of AxisLimits)

#Region "CTOR"

        Public ReadOnly XMin As Double
        Public ReadOnly XMax As Double
        Public ReadOnly YMin As Double
        Public ReadOnly YMax As Double

        Public ReadOnly XSpan As Double
        Public ReadOnly YSpan As Double
        Public ReadOnly XCenter As Double
        Public ReadOnly YCenter As Double

        Public Sub New(xMin As Double, xMax As Double, yMin As Double, yMax As Double)
            Me.XMin = xMin
            Me.XMax = xMax
            Me.YMin = yMin
            Me.YMax = yMax
            Me.XSpan = Me.XMax - Me.XMin
            Me.YSpan = Me.YMax - Me.YMin
            XCenter = xMin + XSpan / 2
            YCenter = yMin + YSpan / 2
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"AxisLimits: x=[{XMin}, {XMax}] y=[{YMin}, {YMax}]"
        End Function

        ''' <summary>
        ''' AxisLimits representing uninitialized or "no data" limits (all limits are NaN).
        ''' </summary>
        Public Shared ReadOnly Property NoLimits As AxisLimits
            Get
                Return New AxisLimits(Double.NaN, Double.NaN, Double.NaN, Double.NaN)
            End Get
        End Property

        ''' <summary>
        ''' AxisLimits with finite vertical limits and undefined (NaN) horizontal limits.
        ''' </summary>
        Public Shared Function VerticalLimitsOnly(yMin As Double, yMax As Double) As AxisLimits
            Return New AxisLimits(Double.NaN, Double.NaN, yMin, yMax)
        End Function

        ''' <summary>
        ''' AxisLimits with finite horizontal limits and undefined (NaN) vertical limits.
        ''' </summary>
        Public Shared Function HorizontalLimitsOnly(xMin As Double, xMax As Double) As AxisLimits
            Return New AxisLimits(xMin, xMax, Double.NaN, Double.NaN)
        End Function

        ''' <summary>
        ''' Return the maximum boundary for both sets of axis limits.
        ''' </summary>
        Public Function Expand(limits As AxisLimits) As AxisLimits
            Return New AxisLimits(
                If(Double.IsNaN(Me.XMin), limits.XMin, Math.Min(Me.XMin, limits.XMin)),
                If(Double.IsNaN(Me.XMax), limits.XMax, Math.Max(Me.XMax, limits.XMax)),
                If(Double.IsNaN(Me.YMin), limits.YMin, Math.Min(Me.YMin, limits.YMin)),
                If(Double.IsNaN(Me.YMax), limits.YMax, Math.Max(Me.YMax, limits.YMax)))
        End Function

        ''' <summary>
        ''' Returns True if the coordinate is contained inside these axis limits.
        ''' </summary>
        Public Function Contains(coordinate As Coordinate) As Boolean
            Return (coordinate.X >= Me.XMin) _
                AndAlso (coordinate.X <= Me.XMax) _
                AndAlso (coordinate.Y >= Me.YMin) _
                AndAlso (coordinate.Y <= Me.YMax)
        End Function

        Public Function Equals(other As AxisLimits) As Boolean Implements IEquatable(Of AxisLimits).Equals
            Return (other.XMin = XMin) _
                AndAlso (other.XMax = XMax) _
                AndAlso (other.YMin = YMin) _
                AndAlso (other.YMax = YMax)
        End Function

#End Region '/METHODS

    End Structure

End Namespace