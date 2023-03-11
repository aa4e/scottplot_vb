Imports System

Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Work in this file is derived from code originally written by Hans-Peter Moser:
    ''' http//www.mosismath.com/NaturalSplines/NaturalSplines.html
    ''' It is included in ScottPlot under a MIT license with permission from the original author.
    ''' </remarks>
    <Obsolete("This class has been deprecated. Use ScottPlot.Statistics.Interpolation.Cubic.InterpolateXY()")>
    Public MustInherit Class SplineInterpolator

        Public GivenYs As Double()
        Public GivenXs As Double()
        Public InterpolatedXs As Double()
        Public InterpolatedYs As Double()

        Protected M As Matrix
        Protected Gauss As MatrixSolver
        Protected ReadOnly N As Integer
        Protected A As Double()
        Protected B As Double()
        Protected C As Double()
        Protected D As Double()
        Protected H As Double()

        Protected Sub New(xs As Double(), ys As Double(), Optional resolution As Integer = 10)
            If (xs is Nothing) OrElse (ys is Nothing) Then
                Throw New ArgumentException("Xs and Ys cannot be null.")
            End If
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must have the same length.")
            End If
            If (xs.Length < 4) Then
                Throw New ArgumentException("Xs and Ys must have a length of 4 or greater.")
            End If
            If (resolution < 1) Then
                Throw New ArgumentException("Resolution must be 1 or greater.")
            End If

            GivenXs = xs
            GivenYs = ys
            N = xs.Length

            InterpolatedXs = New Double(N * resolution - 1) {}
            InterpolatedYs = New Double(N * resolution - 1) {}
        End Sub

        Public Sub Interpolate()
            Dim resolution As Integer = CInt(InterpolatedXs.Length / N)
            For i As Integer = 0 To H.Length - 1
                For j As Integer = 0 To resolution - 1
                    Dim deltaX As Double = j / resolution * H(i)
                    Dim termA As Double = A(i)
                    Dim termB As Double = B(i) * deltaX
                    Dim termC As Double = C(i) * deltaX * deltaX
                    Dim termD As Double = D(i) * deltaX * deltaX * deltaX
                    Dim interpolatedIndex As Integer = i * resolution + j
                    InterpolatedXs(interpolatedIndex) = deltaX + GivenXs(i)
                    InterpolatedYs(interpolatedIndex) = termA + termB + termC + termD
                Next
            Next

            ' After interpolation the last several values of the interpolated arrays contain uninitialized data.
            ' This section identifies the values which are populated with values and copies just the useful data into new arrays.
            Dim pointsToKeep As Integer = resolution * (N - 1) + 1
            Dim interpolatedXsCopy As Double() = New Double(pointsToKeep - 1) {}
            Dim interpolatedYsCopy As Double() = New Double(pointsToKeep - 1) {}
            Array.Copy(InterpolatedXs, 0, interpolatedXsCopy, 0, pointsToKeep - 1)
            Array.Copy(InterpolatedYs, 0, interpolatedYsCopy, 0, pointsToKeep - 1)
            InterpolatedXs = interpolatedXsCopy
            InterpolatedYs = interpolatedYsCopy
            InterpolatedXs(pointsToKeep - 1) = GivenXs(N - 1)
            InterpolatedYs(pointsToKeep - 1) = GivenYs(N - 1)
        End Sub

        Public Function Integrate() As Double
            Dim integral As Double = 0
            For i As Integer = 0 To H.Length - 1
                Dim termA As Double = A(i) * H(i)
                Dim termB As Double = B(i) * Math.Pow(H(i), 2) / 2
                Dim termC As Double = C(i) * Math.Pow(H(i), 3) / 3
                Dim termD As Double = D(i) * Math.Pow(H(i), 4) / 4
                integral += termA + termB + termC + termD
            Next
            Return integral
        End Function

    End Class

End Namespace