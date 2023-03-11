Imports System.Linq

Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' Parabolic Cubic Spline Interpolation Module.
    ''' </summary>
    ''' <remarks>
    '''  Original Author: Ryan Seghers
    '''  Original Copyright (C) 2013-2014 Ryan Seghers
    '''  Original License: MIT https : //opensource.org/licenses/MIT
    '''  Original Source Code: https://github.com/SCToolsfactory/SCJMapper-V2/tree/master/OGL
    '''  Related Article: https://www.codeproject.com/Articles/560163/Csharp-Cubic-Spline-Interpolation
    '''  Modified by: Scott W Harden In 2022 (released under MIT license)
    '''  Related Article: https://swharden.com/blog/2022-01-22-spline-interpolation/
    '''  Related Source Code: https://github.com/swharden/Csharp-Data-Visualization
    ''' </remarks>
    Public Module Cubic

        ''' <summary>
        ''' Generate a smooth (interpolated) curve that follows the path of the given X/Y points.
        ''' </summary>
        Public Function InterpolateXY(xs As Double(), ys As Double(), count As Integer) As Tuple(Of Double(), Double())
            If (xs is Nothing) OrElse (ys is Nothing) OrElse (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must have same length.")
            End If

            Dim inputPointCount As Integer = xs.Length
            Dim inputDistances As Double() = New Double(inputPointCount - 1) {}
            For i As Integer = 1 To inputPointCount - 1
                Dim dx As Double = xs(i) - xs(i - 1)
                Dim dy As Double = ys(i) - ys(i - 1)
                Dim distance As Double = Math.Sqrt(dx * dx + dy * dy)
                inputDistances(i) = inputDistances(i - 1) + distance
            Next

            Dim meanDistance As Double = inputDistances.Last() / (count - 1)
            Dim evenDistances As Double() = Enumerable.Range(0, count).Select(Function(x) x * meanDistance).ToArray()
            Dim xsOut As Double() = Cubic.Interpolate(inputDistances, xs, evenDistances)
            Dim ysOut As Double() = Cubic.Interpolate(inputDistances, ys, evenDistances)
            Return New Tuple(Of Double(), Double())(xsOut, ysOut)
        End Function

        Private Function Interpolate(xOrig As Double(), yOrig As Double(), xInterp As Double()) As Double()
            Dim tp As Tuple(Of Double(), Double()) = Cubic.FitMatrix(xOrig, yOrig)
            Dim a As Double() = tp.Item1
            Dim b As Double() = tp.Item2
            Dim yInterp As Double() = New Double(xInterp.Length - 1) {}
            For i As Integer = 0 To yInterp.Length - 1
                Dim j As Integer = 0
                While (j < xOrig.Length - 2) AndAlso (xInterp(i) > xOrig(j + 1)) 'TEST 
                    j += 1
                End While
                Dim dx As Double = xOrig(j + 1) - xOrig(j)
                Dim t As Double = (xInterp(i) - xOrig(j)) / dx
                Dim y As Double = (1 - t) * yOrig(j) + t * yOrig(j + 1) + t * (1 - t) * (a(j) * (1 - t) + b(j) * t)
                yInterp(i) = y
            Next
            Return yInterp
        End Function

        Private Function FitMatrix(x As Double(), y As Double()) As Tuple(Of Double(), Double())
            Dim n As Integer = x.Length
            Dim a As Double() = New Double(n - 1 - 1) {}
            Dim b As Double() = New Double(n - 1 - 1) {}
            Dim r As Double() = New Double(n - 1) {}
            Dim aa As Double() = New Double(n - 1) {}
            Dim bb As Double() = New Double(n - 1) {}
            Dim cc As Double() = New Double(n - 1) {}

            Dim dx1 As Double = x(1) - x(0)
            cc(0) = 1 / dx1
            bb(0) = 2 * cc(0)
            r(0) = 3 * (y(1) - y(0)) / (dx1 * dx1)

            Dim dy1 As Double
            For i As Integer = 1 To n - 1 - 1
                dx1 = x(i) - x(i - 1)
                Dim dx2 As Double = x(i + 1) - x(i)
                aa(i) = 1 / dx1
                cc(i) = 1 / dx2
                bb(i) = 2 * (aa(i) + cc(i))
                dy1 = y(i) - y(i - 1)
                Dim dy2 As Double = y(i + 1) - y(i)
                r(i) = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2))
            Next

            dx1 = x(n - 1) - x(n - 2)
            dy1 = y(n - 1) - y(n - 2)
            aa(n - 1) = 1 / dx1
            bb(n - 1) = 2 * aa(n - 1)
            r(n - 1) = 3 * (dy1 / (dx1 * dx1))

            Dim cPrime As Double() = New Double(n - 1) {}
            cPrime(0) = cc(0) / bb(0)
            For i As Integer = 1 To n - 1
                cPrime(i) = cc(i) / (bb(i) - cPrime(i - 1) * aa(i))
            Next

            Dim dPrime As Double() = New Double(n - 1) {}
            dPrime(0) = r(0) / bb(0)
            For i As Integer = 1 To n - 1
                dPrime(i) = (r(i) - dPrime(i - 1) * aa(i)) / (bb(i) - cPrime(i - 1) * aa(i))
            Next

            Dim k As Double() = New Double(n - 1) {}
            k(n - 1) = dPrime(n - 1)
            For i As Integer = n - 2 To 0 Step -1
                k(i) = dPrime(i) - cPrime(i) * k(i + 1)
            Next

            For i As Integer = 1 To n - 1
                dx1 = x(i) - x(i - 1)
                dy1 = y(i) - y(i - 1)
                a(i - 1) = k(i - 1) * dx1 - dy1
                b(i - 1) = -k(i) * dx1 + dy1
            Next
            Return New Tuple(Of Double(), Double())(a, b)
        End Function

    End Module

End Namespace