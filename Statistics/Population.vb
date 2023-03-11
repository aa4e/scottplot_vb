Imports System.Linq

Namespace ScottPlot.Statistics

    ''' <summary>
    ''' This module holds an array of values and provides popultation statistics (mean, median, standard deviation, etc).
    ''' </summary>
    Public Class Population

#Region "PROPS"

        Public ReadOnly Property Values As Double()
        Public ReadOnly Property SortedValues As Double()
        Public ReadOnly Property Min As Double
        Public ReadOnly Property Max As Double
        Public ReadOnly Property Median As Double
        Public ReadOnly Property Sum As Double
        Public ReadOnly Property Count As Integer
        Public ReadOnly Property Mean As Double
        Public ReadOnly Property StDev As Double
        Public ReadOnly Property Plus3stDev As Double
        Public ReadOnly Property Minus3stDev As Double
        Public ReadOnly Property Plus2stDev As Double
        Public ReadOnly Property Minus2stDev As Double
        Public ReadOnly Property StdErr As Double
        Public ReadOnly Property Q1 As Double
        Public ReadOnly Property Q3 As Double
        Public ReadOnly Property IQR As Double
        Public ReadOnly Property LowOutliers As Double()
        Public ReadOnly Property HighOutliers As Double()
        Public ReadOnly Property MaxNonOutlier As Double
        Public ReadOnly Property MinNonOutlier As Double

        Public ReadOnly Property N As Integer
            Get
                Return Values.Length
            End Get
        End Property

        Public ReadOnly Property Span As Double
            Get
                Return (SortedValues.Last() - SortedValues.First())
            End Get
        End Property

#End Region '/PROPS

#Region "CTOR"

        ''' <summary>
        ''' Generate random values with a normal distribution.
        ''' </summary>
        Public Sub New(rand As Random, pointCount As Integer, Optional mean As Double = 0.5, Optional stdDev As Double = 0.5)
            Values = DataGen.RandomNormal(rand, pointCount, mean, stdDev, 10)
            Recalculate()
        End Sub

        ''' <summary>
        ''' Calculate population stats from the given array of values.
        ''' </summary>
        Public Sub New(values As Double())
            If (values is Nothing) Then
                Throw New ArgumentException("Values cannot be null.")
            End If
            Me.Values = values
            Recalculate()
        End Sub

        <Obsolete("This constructor overload is deprecated. Please remove the fullAnalysis argument.")>
        Public Sub New(values As Double(), Optional fullAnalysis As Boolean = True)
            If (values is Nothing) Then
                Throw New ArgumentException("Values cannot be null.")
            End If
            Me.Values = values
            If fullAnalysis Then
                Recalculate()
            End If
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Sub Recalculate()
            _Count = Values.Length
            Dim qSize As Integer = CInt(Math.Floor(Count / 4))

            _SortedValues = New Double(Count - 1) {}
            Array.Copy(Values, 0, SortedValues, 0, Count)
            Array.Sort(Of Double)(SortedValues)

            _Min = SortedValues.First()
            _Max = SortedValues.Last()

            'median is average of the two values in the middle if value count is even
            _Median = If(Count Mod 2 = 0,
                (SortedValues(CInt(Count / 2) - 1) + SortedValues(CInt(Count / 2))) / 2,
                SortedValues(CInt(Count / 2)))

            _Q1 = SortedValues(qSize)
            _Q3 = SortedValues(SortedValues.Length - qSize - 1)
            _IQR = Q3 - Q1

            Dim lowerBoundary As Double = Q1 - 1.5 * IQR
            Dim upperBoundary As Double = Q3 + 1.5 * IQR

            Dim minNonOutlierIndex As Integer = 0
            For i As Integer = 0 To SortedValues.Length - 1
                If (SortedValues(i) >= lowerBoundary) Then
                    minNonOutlierIndex = i
                    Exit For
                End If
            Next

            Dim maxNonOutlierIndex As Integer = 0
            For i As Integer = SortedValues.Length - 1 To 0 Step -1
                If (SortedValues(i) <= upperBoundary) Then
                    maxNonOutlierIndex = i
                    Exit For
                End If
            Next

            _LowOutliers = New Double(minNonOutlierIndex - 1) {}
            _HighOutliers = New Double(SortedValues.Length - maxNonOutlierIndex - 1 - 1) {}
            Array.Copy(SortedValues, 0, LowOutliers, 0, LowOutliers.Length)
            Array.Copy(SortedValues, maxNonOutlierIndex + 1, HighOutliers, 0, HighOutliers.Length)
            _MinNonOutlier = SortedValues(minNonOutlierIndex)
            _MaxNonOutlier = SortedValues(maxNonOutlierIndex)

            _Sum = Values.Sum()
            _Mean = Sum / Count

            Dim sumVariancesSquared As Double = 0
            For i As Integer = 0 To Values.Length - 1
                Dim pointVariance As Double = Math.Abs(Mean - Values(i))
                Dim pointVarianceSquared As Double = Math.Pow(pointVariance, 2)
                sumVariancesSquared += pointVarianceSquared
            Next

            Dim meanVarianceSquared As Double = sumVariancesSquared / Values.Length
            _StDev = Math.Sqrt(meanVarianceSquared)
            _Plus2stDev = Mean + StDev * 2
            _Minus2stDev = Mean - StDev * 2
            _Plus3stDev = Mean + StDev * 3
            _Minus3stDev = Mean - StDev * 3
            _StdErr = StDev / Math.Sqrt(Count)
        End Sub

        Public Function GetDistribution(xs As Double(), normalize As Boolean) As Double()
            Dim ys As Double() = New Double(xs.Length - 1) {}
            For i As Integer = 0 To xs.Length - 1
                ys(i) = Math.Exp(-0.5 * Math.Pow((xs(i) - Mean) / StDev, 2))
            Next
            If normalize Then
                Dim sum As Double = ys.Sum()
                For j As Integer = 0 To ys.Length - 1
                    ys(j) /= sum
                Next
            End If
            Return ys
        End Function

#End Region '/METHODS

    End Class

End Namespace