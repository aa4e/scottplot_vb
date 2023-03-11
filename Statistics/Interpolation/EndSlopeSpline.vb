Imports System

Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' The End Slope Spline is a Natural Spline whose first and last point slopes can be defined.
    ''' </summary>
    ''' <remarks>
    ''' Work in this file is derived from code originally written by Hans-Peter Moser:
    ''' http//www.mosismath.com/AngleSplines/EndSlopeSplines.html
    ''' It is included in ScottPlot under a MIT license with permission from the original author.
    ''' </remarks>
    <Obsolete("This class has been deprecated. Use ScottPlot.Statistics.Interpolation.Cubic.InterpolateXY()")>
    Public Class EndSlopeSpline
        Inherits SplineInterpolator

        Public Sub New(xs As Double(), ys As Double(),
                       Optional resolution As Integer = 10, Optional firstSlopeDegrees As Double = 0, Optional lastSlopeDegrees As Double = 0)
            MyBase.New(xs, ys, resolution)
            M = New Matrix(N)
            Gauss = New MatrixSolver(N, M)

            A = New Double(N - 1) {}
            B = New Double(N - 1) {}
            C = New Double(N - 1) {}
            D = New Double(N - 1) {}
            H = New Double(N - 1) {}

            CalcParameters(firstSlopeDegrees, lastSlopeDegrees)
            MyBase.Integrate()
            MyBase.Interpolate()
        End Sub

        Public Sub CalcParameters(alpha As Double, beta As Double)
            For i As Integer = 0 To N - 1
                A(i) = givenYs(i)
            Next
            For j As Integer = 0 To N - 1 - 1
                H(j) = givenXs(j + 1) - givenXs(j)
            Next

            M.a(0, 0) = 2 * H(0)
            M.a(0, 1) = H(0)
            M.y(0) = 3 * ((A(1) - A(0)) / H(0) - Math.Tan(alpha * Math.PI / 180))

            For i As Integer = 0 To N - 2 - 1
                M.a(i + 1, i) = H(i)
                M.a(i + 1, i + 1) = 2 * (H(i) + H(i + 1))
                If (i < N - 2) Then
                    M.a(i + 1, i + 2) = H(i + 1)
                End If
                If (H(i) <> 0) AndAlso (H(i + 1) <> 0) Then
                    M.y(i + 1) = ((A(i + 2) - A(i + 1)) / H(i + 1) - (A(i + 1) - A(i)) / H(i)) * 3
                Else
                    M.y(i + 1) = 0
                End If
            Next

            M.a(N - 1, N - 2) = H(N - 2)
            M.a(N - 1, N - 1) = 2.0 * H(N - 2)
            M.y(N - 1) = 3 * (Math.Tan(beta * Math.PI / 180) - (A(N - 1) - A(N - 2)) / H(N - 2))

            If (Not Gauss.Eliminate()) Then
                Throw New InvalidOperationException()
            End If

            Gauss.Solve()

            For i As Integer = 0 To N - 1
                C(i) = M.x(i)
            Next

            For i As Integer = 0 To N - 1
                If (H(i) <> 0) Then
                    D(i) = 1 / 3 / H(i) * (C(i + 1) - C(i))
                    B(i) = 1 / H(i) * (A(i + 1) - A(i)) - H(i) / 3 * (C(i + 1) + 2 * C(i))
                End If
            Next
        End Sub

    End Class

End Namespace