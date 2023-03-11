Imports System.Linq

Namespace ScottPlot.Statistics

    Public Class LinearRegressionLine

#Region "PROPS, FIELDS"

        Public ReadOnly Slope As Double
        Public ReadOnly Offset As Double
        Public ReadOnly RSquared As Double
        Private ReadOnly PointCount As Integer
        Private ReadOnly Xs As Double()
        Private ReadOnly Ys As Double()

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(xs As Double(), ys As Double())
            If (xs.Length <> ys.Length) OrElse (xs.Length < 2) Then
                Throw New ArgumentException("Xs and ys must be the same length and have at least 2 points.")
            End If

            Me.PointCount = ys.Length
            Me.Xs = xs
            Me.Ys = ys
            Dim coefficients = LinearRegressionLine.GetCoefficients(xs, ys)
            Me.Slope = coefficients.Item1
            Me.Offset = coefficients.Item2
            Me.RSquared = coefficients.Item3
        End Sub

        ''' <summary>
        ''' This constructor doesn't require an X array to be passed in at all.
        ''' </summary>
        Public Sub New(ys As Double(), firstX As Double, xSpacing As Double)
            Me.PointCount = ys.Length
            Dim xs As Double() = New Double(Me.PointCount - 1) {}
            For i As Integer = 0 To Me.PointCount - 1
                xs(i) = firstX + xSpacing * i
            Next
            Me.Xs = xs
            Me.Ys = ys
            Dim coefficients = LinearRegressionLine.GetCoefficients(xs, ys)
            Me.Slope = coefficients.Item1
            Me.Offset = coefficients.Item2
            Me.RSquared = coefficients.Item3
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"Linear fit for {PointCount} points: Y = {Slope}x + {Offset} (R² = {RSquared})."
        End Function

        Public Function GetValueAt(x As Double) As Double
            Return (Offset + Slope * x)
        End Function

        Public Function GetValues() As Double()
            Dim values As Double() = New Double(PointCount - 1) {}
            For i As Integer = 0 To PointCount - 1
                values(i) = GetValueAt(Xs(i))
            Next
            Return values
        End Function

        ''' <summary>
        ''' The residual is the difference between the actual and predicted value.
        ''' </summary>
        Public Function GetResiduals() As Double()
            Dim residuals As Double() = New Double(Ys.Length - 1) {}
            For i As Integer = 0 To Ys.Length - 1
                residuals(i) = Ys(i) - GetValueAt(Xs(i))
            Next
            Return residuals
        End Function

        Private Shared Function GetCoefficients(xs As Double(), ys As Double()) As Tuple(Of Double, Double, Double)
            Dim sumXYResidual As Double = 0
            Dim sumXSquareResidual As Double = 0

            Dim meanX As Double = xs.Average()
            Dim meanY As Double = ys.Average()

            For i As Integer = 0 To xs.Length - 1
                sumXYResidual += (xs(i) - meanX) * (ys(i) - meanY)
                sumXSquareResidual += (xs(i) - meanX) * (xs(i) - meanX)
            Next

            'Note: least-squares regression line always passes through (x̅,y̅)
            Dim slope As Double = sumXYResidual / sumXSquareResidual
            Dim offset As Double = meanY - slope * meanX

            'calcualte R squared (https://en.wikipedia.org/wiki/Coefficient_of_determination)
            Dim ssTot As Double = 0
            Dim ssRes As Double = 0

            For i As Integer = 0 To ys.Length - 1
                Dim thisY As Double = ys(i)

                Dim distanceFromMeanSquared As Double = Math.Pow(thisY - meanY, 2.0)
                ssTot += distanceFromMeanSquared

                Dim modelY As Double = slope * xs(i) + offset
                Dim distanceFromModelSquared As Double = Math.Pow(thisY - modelY, 2.0)
                ssRes += distanceFromModelSquared
            Next
            Dim rSquared As Double = 1 - ssRes / ssTot
            Return New Tuple(Of Double, Double, Double)(slope, offset, rSquared)
        End Function

#End Region '/METHODS

    End Class

End Namespace