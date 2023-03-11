Namespace ScottPlot.Statistics.Interpolation

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' *  Work in this file is derived from code originally written by Hans-Peter Moser:
    '''  http//www.mosismath.com/Basics/Basics.html
    '''  http://www.mosismath.com/Matrix_Gauss/MatrixGauss.html
    '''  It is included in ScottPlot under a MIT license with permission from the original author.
    ''' </remarks>
    <Obsolete("This class has been deprecated. Use ScottPlot.Statistics.Interpolation.Cubic.InterpolateXY()")>
    Public Class Matrix

        Public A As Double(,)
        Public Y As Double()
        Public X As Double()

        Public Sub New(size As Integer)
            A = New Double(size - 1, size - 1) {}
            Y = New Double(size - 1) {}
            X = New Double(size - 1) {}
        End Sub

    End Class

End Namespace