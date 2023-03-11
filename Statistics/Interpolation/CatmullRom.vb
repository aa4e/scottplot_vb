Imports System.Collections.Generic
Imports System.Linq

Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Original Author: Reinoud Veenhof (http://www.veetools.xyz)
    ''' Original Article https://www.codeproject.com/Articles/1093960/D-Polyline-Vertex-Smoothing
    ''' Original License: CPOL (https://www.codeproject.com/info/cpol10.aspx)
    ''' Code here was translated to C# from VB by Scott W Harden 1/24/2022 and is released under MIT license.
    ''' About Catmull-Rom splines: https://www.cs.cmu.edu/~fp/courses/graphics/asst5/catmullRom.pdf
    ''' </remarks>
    Public Module CatmullRom

        Public Function InterpolateXY(xs As Double(), ys As Double(), multiple As Integer) As Tuple(Of Double(), Double())
            Dim points As New List(Of PointD)()
            For i As Integer = 0 To xs.Length - 1
                points.Add(New PointD(xs(i), ys(i)))
            Next
            Dim smooth = GetSplineInterpolationCatmullRom(points, multiple)
            Dim xs2 As Double() = smooth.Select(Function(s) s.X).ToArray()
            Dim ys2 As Double() = smooth.Select(Function(s) s.Y).ToArray()
            Return New Tuple(Of Double(), Double())(xs2, ys2)
        End Function

        Private Function GetSplineInterpolationCatmullRom(points As List(Of PointD), nrOfInterpolatedPoints As Integer) As List(Of PointD)
            If (points.Count < 3) Then
                Throw New Exception("Catmull-Rom Spline requires at least 3 points.")
            End If

            Dim spoints As New List(Of PointD)
            For Each p As PointD In points
                spoints.Add(New PointD(p))
            Next

            Dim dx As Double = spoints(1).X - spoints(0).X
            Dim dy As Double = spoints(1).Y - spoints(0).Y
            spoints.Insert(0, New PointD(spoints(0).X - dx, spoints(0).Y - dy))
            dx = spoints(spoints.Count - 1).X - spoints(spoints.Count - 2).X
            dy = spoints(spoints.Count - 1).Y - spoints(spoints.Count - 2).Y
            spoints.Insert(spoints.Count, New PointD(spoints(spoints.Count - 1).X + dx, spoints(spoints.Count - 1).Y + dy))

            nrOfInterpolatedPoints = Math.Max(1, nrOfInterpolatedPoints)

            Dim spline As New List(Of PointD)
            For i As Integer = 0 To spoints.Count - 4
                For intp As Integer = 0 To nrOfInterpolatedPoints - 1
                    Dim t As Double = 1 / nrOfInterpolatedPoints * intp
                    Dim spoint As New PointD()
                    spoint = 2 * spoints(i + 1) _
                        + (-1 * spoints(i) + spoints(i + 2)) * t _
                        + (2 * spoints(i) - 5 * spoints(i + 1) + 4 * spoints(i + 2) - spoints(i + 3)) * Math.Pow(t, 2) _
                        + (-1 * spoints(i) + 3 * spoints(i + 1) - 3 * spoints(i + 2) + spoints(i + 3)) * Math.Pow(t, 3)
                    spline.Add(New PointD(spoint * 0.5))
                Next
            Next
            spline.Add(spoints(spoints.Count - 2))
            Return spline
        End Function

    End Module

End Namespace