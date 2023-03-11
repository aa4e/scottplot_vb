Namespace ScottPlot

    ''' <summary>
    ''' Describes the location and size of a rectangle in coordinate space.
    ''' </summary>
    Public Class CoordinateRect

#Region "PROPS"

        Public ReadOnly XMin As Double
        Public ReadOnly XMax As Double
        Public ReadOnly YMin As Double
        Public ReadOnly YMax As Double

        Public ReadOnly Property Width As Double
            Get
                Return (XMax - XMin)
            End Get
        End Property

        Public ReadOnly Property Height As Double
            Get
                Return (YMax - YMin)
            End Get
        End Property

        Public ReadOnly Property Area As Double
            Get
                Return Math.Abs(Width * Height)
            End Get
        End Property

        Public ReadOnly Property HasArea As Boolean
            Get
                Return (Area > 0)
            End Get
        End Property

#End Region '/PROPS

#Region "CTOR"

        Public Sub New(x1 As Double, x2 As Double, y1 As Double, y2 As Double)
            If Double.IsNaN(x1) OrElse Double.IsInfinity(x1) Then
                Throw New ArgumentOutOfRangeException($"{NameOf(x1)} must be a real number.")
            End If
            If Double.IsNaN(x2) OrElse Double.IsInfinity(x2) Then
                Throw New ArgumentOutOfRangeException($"{NameOf(x2)} must be a real number.")
            End If
            If Double.IsNaN(y1) OrElse Double.IsInfinity(y1) Then
                Throw New ArgumentOutOfRangeException($"{NameOf(y1)} must be a real number.")
            End If
            If Double.IsNaN(y2) OrElse Double.IsInfinity(y2) Then
                Throw New ArgumentOutOfRangeException($"{NameOf(y2)} must be a real number.")
            End If
            XMin = Math.Min(x1, x2)
            XMax = Math.Max(x1, x2)
            YMin = Math.Min(y1, y2)
            YMax = Math.Max(y1, y2)
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function Contains(coord As Coordinate) As Boolean
            Return Contains(coord.X, coord.Y)
        End Function

        Public Function Contains(x As Double, y As Double) As Boolean
            Return (x >= XMin) AndAlso (x <= XMax) AndAlso (y >= YMin) AndAlso (y <= YMax)
        End Function

        Public Overrides Function ToString() As String
            Return $"CoordinateRect: x=[{XMin}, {XMax}] y=[{YMin}, {YMax}]."
        End Function

        Public Shared Function BoundingBox(coordinates As Coordinate()) As CoordinateRect
            If (coordinates is Nothing) Then
                Throw New ArgumentNullException($"{NameOf(coordinates)}.")
            End If
            If (coordinates.Length = 0) Then
                Throw New ArgumentException($"{NameOf(coordinates)} must not be empty.")
            End If

            Dim x1 As Double = coordinates(0).X
            Dim x2 As Double = coordinates(0).X
            Dim y1 As Double = coordinates(0).Y
            Dim y2 As Double = coordinates(0).Y

            For Each c As Coordinate In coordinates
                x1 = Math.Min(x1, c.X)
                x2 = Math.Max(x2, c.X)
                y1 = Math.Min(y1, c.Y)
                y2 = Math.Max(y2, c.Y)
            Next
            Return New CoordinateRect(x1, x2, y1, y2)
        End Function

#End Region '/METHODS

    End Class

End Namespace