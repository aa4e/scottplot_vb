Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' Matrix equation solver using the Gaussian elimination algorithm.
    ''' </summary>
    ''' <remarks>
    ''' Work in this file is derived from code originally written by Hans-Peter Moser:
    ''' http//www.mosismath.com/Basics/Basics.html
    ''' http://www.mosismath.com/Matrix_Gauss/MatrixGauss.html
    ''' It is included in ScottPlot under a MIT license with permission from the original author.
    ''' </remarks>
    <Obsolete("This class has been deprecated. Use ScottPlot.Statistics.Interpolation.Cubic.InterpolateXY()")>
    Public Class MatrixSolver

        Public ReadOnly M As Matrix
        Public ReadOnly MaxOrder As Integer
        Public CalcError As Boolean

        Public Sub New(size As Integer, mi As Matrix)
            MaxOrder = size
            M = mi
        End Sub

        Private Sub SwitchRows(n As Integer)
            For i As Integer = n To MaxOrder - 2
                Dim tempD As Double
                For j As Integer = 0 To MaxOrder - 1
                    tempD = M.A(i, j)
                    M.A(i, j) = M.A(i + 1, j)
                    M.A(i + 1, j) = tempD
                Next
                tempD = M.Y(i)
                M.Y(i) = M.Y(i + 1)
                M.Y(i + 1) = tempD
            Next
        End Sub

        Public Function Eliminate() As Boolean
            CalcError = False
            For i As Integer = 0 To MaxOrder - 2
                For j As Integer = i To MaxOrder - 2
                    If (Math.Abs(M.A(j + 1, j)) < 0.00000001) Then
                        SwitchRows(j + 1)
                    End If
                    If (M.A(j + 1, i) <> 0) Then
                        For k As Integer = i + 1 To MaxOrder - 1
                            If (Not CalcError) Then
                                M.A(j + 1, k) = M.A(j + 1, k) * M.A(i, i) - M.A(i, k) * M.A(j + 1, i)
                                If (M.A(j + 1, k) > 1.0E+261) Then
                                    M.A(j + 1, i) = 0
                                    CalcError = True
                                End If
                            End If
                        Next
                        M.Y(j + 1) = M.Y(j + 1) * M.A(i, i) - M.Y(i) * M.A(j + 1, i)
                        M.A(j + 1, i) = 0
                    End If
                Next
            Next
            Return (Not CalcError)
        End Function

        Public Sub Solve()
            For i As Integer = MaxOrder - 1 To 0 Step -1
                For j As Integer = MaxOrder - 1 To i Step -1
                    M.Y(i) = M.Y(i) - M.X(j) * M.A(i, j)
                Next
                If (M.A(i, i) <> 0) Then
                    M.X(i) = M.Y(i) / M.A(i, i)
                Else
                    M.X(i) = 0
                End If
            Next
        End Sub

    End Class

End Namespace
