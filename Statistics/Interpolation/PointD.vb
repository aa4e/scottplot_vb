Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Original Author: Reinoud Veenhof (http://www.veetools.xyz)
    ''' Original Article https://www.codeproject.com/Articles/1093960/D-Polyline-Vertex-Smoothing
    ''' Original License: CPOL (https://www.codeproject.com/info/cpol10.aspx)
    ''' Code here was translated to C# from VB by Scott W Harden 1/24/2022 and is released under MIT license.
    ''' </remarks>
    Friend Class PointD

        Public ReadOnly X As Double = 0
        Public ReadOnly Y As Double = 0

        Public Sub New()
        End Sub

        Public Sub New(nx As Double, ny As Double)
            Me.X = nx
            Me.Y = ny
        End Sub

        Public Sub New(p As PointD)
            Me.X = p.X
            Me.Y = p.Y
        End Sub

        Public Shared Operator +(p1 As PointD, p2 As PointD) As PointD
            Return New PointD(p1.X + p2.X, p1.Y + p2.Y)
        End Operator

        Public Shared Operator +(p As PointD, d As Double) As PointD
            Return New PointD(p.X + d, p.Y + d)
        End Operator

        Public Shared Operator +(d As Double, p As PointD) As PointD
            Return p + d
        End Operator

        Public Shared Operator -(p1 As PointD, p2 As PointD) As PointD
            Return New PointD(p1.X - p2.X, p1.Y - p2.Y)
        End Operator

        Public Shared Operator -(p As PointD, d As Double) As PointD
            Return New PointD(p.X - d, p.Y - d)
        End Operator

        Public Shared Operator -(d As Double, p As PointD) As PointD
            Return p - d
        End Operator

        Public Shared Operator *(p1 As PointD, p2 As PointD) As PointD
            Return New PointD(p1.X * p2.X, p1.Y * p2.Y)
        End Operator

        Public Shared Operator *(p As PointD, d As Double) As PointD
            Return New PointD(p.X * d, p.Y * d)
        End Operator

        Public Shared Operator *(d As Double, p As PointD) As PointD
            Return p * d
        End Operator

        Public Shared Operator /(p1 As PointD, p2 As PointD) As PointD
            Return New PointD(p1.X / p2.X, p1.Y / p2.Y)
        End Operator

        Public Shared Operator /(p As PointD, d As Double) As PointD
            Return New PointD(p.X / d, p.Y / d)
        End Operator

        Public Shared Operator /(d As Double, p As PointD) As PointD
            Return New PointD(d / p.X, d / p.Y)
        End Operator

    End Class

End Namespace