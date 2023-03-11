Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' Natural Spline data interpolator.
    ''' </summary>
    ''' <remarks>
    ''' Work in this file is derived from code originally written by Hans-Peter Moser:
    ''' http//www.mosismath.com/NaturalSplines/NaturalSplines.html
    ''' It is included in ScottPlot under a MIT license with permission from the original author.
    ''' </remarks>
    <Obsolete("This class has been deprecated. Use ScottPlot.Statistics.Interpolation.Cubic.InterpolateXY()")>
    Public Class NaturalSpline
        Inherits SplineInterpolator

        Public Sub New(xs As Double(), ys As Double(), Optional resolution As Integer = 10)
            MyBase.New(xs, ys, resolution)
            M = New Matrix(N - 2)
            Gauss = New MatrixSolver(N - 2, M)

            A = New Double(N - 1) {}
            B = New Double(N - 1) {}
            C = New Double(N - 1) {}
            D = New Double(N - 1) {}
            H = New Double(N - 1 - 1) {}

            CalcParameters()
            MyBase.Integrate()
            MyBase.Interpolate()
        End Sub

        Public Sub CalcParameters()
            For i As Integer = 0 To N - 1
                A(i) = GivenYs(i)
            Next
            For j As Integer = 0 To N - 1 - 1
                H(j) = GivenXs(j + 1) - GivenXs(j)
            Next

            For i As Integer = 0 To N - 2 - 1
                For k As Integer = 0 To N - 2 - 1
                    M.A(i, k) = 0
                    M.Y(i) = 0
                    M.X(i) = 0
                Next
            Next

            For i As Integer = 0 To N - 2 - 1
                If (i = 0) Then
                    M.A(i, 0) = 2 * (H(0) + H(1))
                    M.A(i, 1) = H(1)
                Else
                    M.A(i, i - 1) = H(i)
                    M.A(i, i) = 2 * (H(i) + H(i + 1))
                    If (i < N - 3) Then
                        M.A(i, i + 1) = H(i + 1)
                    End If
                End If

                If H(i) <> 0.0 AndAlso H(i + 1) <> 0.0 Then
                    M.Y(i) = ((A(i + 2) - A(i + 1)) / H(i + 1) - (A(i + 1) - A(i)) / H(i)) * 3
                Else
                    M.Y(i) = 0
                End If
            Next

            If (Not Gauss.Eliminate()) Then
                Throw New InvalidOperationException("Error in matrix calculation.")
            End If

            Gauss.Solve()

            C(0) = 0.0
            C(N - 1) = 0.0

            For i As Integer = 1 To i - 1 - 1
                C(i) = M.X(i - 1)
            Next

            For i As Integer = 0 To N - 1 - 1
                If (H(i) <> 0) Then
                    D(i) = 1 / 3 / H(i) * (C(i + 1) - C(i))
                    B(i) = 1 / H(i) * (A(i + 1) - A(i)) - H(i) / 3 * (C(i + 1) + 2 * C(i))
                End If
            Next
        End Sub

    End Class

End Namespace