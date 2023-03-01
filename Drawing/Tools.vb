Imports System.Collections.Generic
Imports System.Linq

Namespace ScottPlot.Drawing

    Public Module Tools

        ''' <summary>
        ''' Return Xs And Ys for 2 polygons representing the input data above and below the given baseline.
        ''' </summary>
        Public Function PolyAboveAndBelow(xs As Double(), ys As Double(), baseline As Double) As Tuple(Of Double(), Double(), Double())
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("xs and ys must have same length.")
            End If

            Dim intersectionsX As Double() = New Double(ys.Length - 1 - 1) {}
            Dim intersectionsY As Double() = New Double(ys.Length - 1 - 1) {}
            For i As Integer = 0 To intersectionsX.Length - 1
                intersectionsX(i) = Double.NaN
                intersectionsY(i) = Double.NaN

                Dim x1 As Double = xs(i)
                Dim y1 As Double = ys(i)
                Dim x2 As Double = xs(i + 1)
                Dim y2 As Double = ys(i + 1)

                If ((y1 <= baseline AndAlso y2 <= baseline) OrElse (y1 >= baseline AndAlso y2 >= baseline)) Then
                    Continue For
                End If

                Dim deltaX As Double = x2 - x1
                Dim deltaY As Double = y2 - y1

                Dim y1diff As Double = baseline - y1
                Dim y2diff As Double = y2 - baseline
                Dim totalDiff As Double = y1diff + y2diff
                Dim frac As Double = y1diff / totalDiff
                intersectionsX(i) = x1 + deltaX * frac
                intersectionsY(i) = y1 + deltaY * frac
            Next

            Dim polyXs As New List(Of Double)()
            Dim polyYs As New List(Of Double)()

            polyXs.Add(xs.First())
            polyYs.Add(baseline)

            For i As Integer = 0 To xs.Length - 1
                polyXs.Add(xs(i))
                polyYs.Add(ys(i))
                If (i < intersectionsX.Length) AndAlso (Not Double.IsNaN(intersectionsX(i))) Then
                    polyXs.Add(intersectionsX(i))
                    polyYs.Add(intersectionsY(i))
                End If
            Next

            polyXs.Add(xs.Last())
            polyYs.Add(baseline)

            Dim xs2 As Double() = polyXs.ToArray()
            Dim ysAbove As Double() = polyYs.[Select](Function(x As Double) Math.Max(x, baseline)).ToArray()
            Dim ysBelow As Double() = polyYs.[Select](Function(x As Double) Math.Min(x, baseline)).ToArray()
            Return New Tuple(Of Double(), Double(), Double())(xs2, ysAbove, ysBelow)
        End Function

    End Module

End Namespace