Imports System.Collections.Generic
Imports System.Linq

Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Original Author: Kenneth Haugland
    ''' Original Article https://www.codeproject.com/Articles/747928/Spline-Interpolation-history-theory-And-implementa
    ''' Original License: CPOL (https://www.codeproject.com/info/cpol10.aspx)
    ''' Code here was adapted by Scott W Harden 1/24/2022 and is released under MIT license.
    ''' </remarks>
    Public Class Bezier

        Public Shared Function InterpolateXY(xs As Double(), ys As Double(), stepSize As Double) As Tuple(Of Double(), Double())
            Dim points As List(Of PointD) = New List(Of PointD)()
            For i As Integer = 0 To xs.Length - 1
                points.Add(New PointD(xs(i), ys(i)))
            Next
            Dim weights As Double() = Enumerable.Range(0, points.Count).Select(Function(x) 1 / points.Count).ToArray()
            Dim smooth = RationalBezierFunction(points, weights, stepSize)
            Dim xs2 As Double() = smooth.Select(Function(s) s.X).ToArray()
            Dim ys2 As Double() = smooth.Select(Function(s) s.Y).ToArray()
            Return New Tuple(Of Double(), Double())(xs2, ys2)
        End Function

        Private Shared Function RationalBezierFunction(p As List(Of PointD), weight As Double(), stepSize As Double) As List(Of PointD)
            Dim result As New List(Of PointD)()
            For k As Double = 0 To 1 Step stepSize
                Dim b As Double() = RationalBasisFunction(p.Count, k, weight)
                Dim cx As Double = 0
                Dim cy As Double = 0
                For j As Integer = 0 To p.Count - 1
                    cx += b(j) * p(j).X
                    cy += b(j) * p(j).Y
                Next
                result.Add(New PointD(cx, cy))
            Next

            If (Not result.Contains(p(p.Count - 1))) Then
                result.Add(p(p.Count - 1))
            End If
            Return result
        End Function

        Private Shared Function RationalBasisFunction(n As Integer, u As Double, weight As Double()) As Double()
            If (weight.Length <> n) Then
                Throw New ArgumentException("Weight length must match n.")
            End If
            Dim b As Double() = Bezier.AllBernstein(n, u)

            Dim test As Double = 0
            For i As Integer = 0 To n - 1
                test += b(i) * weight(i)
            Next

            Dim result As Double() = New Double(n - 1) {}
            For i As Integer = 0 To n - 1
                result(i) = b(i) * weight(i) / test
            Next
            Return result
        End Function

        Private Shared Function AllBernstein(n As Integer, u As Double) As Double()
            Dim u1 As Double = 1 - u
            Dim b As Double() = New Double(n - 1) {}
            b(0) = 1
            For i As Integer = 1 To n - 1
                Dim saved As Double = 0
                For j As Integer = 0 To i - 1
                    Dim tmp As Double = b(j)
                    b(j) = saved + u1 * tmp
                    saved = u * tmp
                Next
                b(i) = saved
            Next
            Return b
        End Function

    End Class

End Namespace