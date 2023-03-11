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
    ''' About Chaikin curves: https://www.cs.unc.edu/~dm/UNC/COMP258/LECTURES/Chaikins-Algorithm.pdf
    ''' </remarks>
    Public Module Chaikin

        Public Function InterpolateXY(xs As Double(), ys As Double(), multiple As Integer, Optional tension As Double = 0.5) As Tuple(Of Double(), Double())
            Dim points As New List(Of PointD)()
            For i As Integer = 0 To xs.Length - 1
                points.Add(New PointD(xs(i), ys(i)))
            Next
            Dim smooth = GetCurveSmoothingChaikin(points, tension, multiple)
            Dim xs2 As Double() = smooth.Select(Function(s) s.X).ToArray()
            Dim ys2 As Double() = smooth.Select(Function(s) s.Y).ToArray()
            Return New Tuple(Of Double(), Double())(xs2, ys2)
        End Function

        Private Function GetCurveSmoothingChaikin(points As List(Of PointD), tension As Double, nrOfIterations As Integer) As List(Of PointD)
            If (points is Nothing) OrElse (points.Count < 3) Then
                Return Nothing
            End If

            If (nrOfIterations < 1) Then
                nrOfIterations = 1
            End If

            If (tension < 0) Then
                tension = 0
            ElseIf (tension > 1) Then
                tension = 1
            End If

            ' The tension factor defines a scale between corner cutting distance in segment half length, i.e. between 0.05 and 0.45.
            ' The opposite corner will be cut by the inverse (i.e. 1-cutting distance) to keep symmetry.
            ' with a tension value of 0.5 this amounts to 0.25 = 1/4 and 0.75 = 3/4, the original Chaikin values
            Dim cuttingDist As Double = 0.05 + tension * 0.4
            Dim nl As New List(Of PointD)()
            For i As Integer = 0 To points.Count - 1
                nl.Add(New PointD(points(i)))
            Next
            For i As Integer = 1 To nrOfIterations
                nl = Chaikin.GetSmootherChaikin(nl, cuttingDist)
            Next
            Return nl
        End Function

        Private Function GetSmootherChaikin(points As List(Of PointD), cuttingDist As Double) As List(Of PointD)
            Dim nl As New List(Of PointD)
            nl.Add(New PointD(points(0)))

            For i As Integer = 0 To points.Count - 2
                Dim q As PointD = (1.0 - cuttingDist) * points(i) + cuttingDist * points(i + 1)
                Dim r As PointD = cuttingDist * points(i) + (1.0 - cuttingDist) * points(i + 1)
                nl.Add(q)
                nl.Add(r)
            Next
            nl.Add(New PointD(points(points.Count - 1)))
            Return nl
        End Function

    End Module

End Namespace