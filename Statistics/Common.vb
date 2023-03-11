Imports System.Linq
Imports System.Security.Cryptography

Namespace ScottPlot.Statistics

    Public Module Common

        Private ReadOnly Rand As New RNGCryptoServiceProvider()

        ''' <summary>
        ''' Return the minimum, and maximum, and sum of a given array.
        ''' </summary>
        Public Function MinMaxSum(values As Double()) As Tuple(Of Double, Double, Double)
            If (values Is Nothing) Then
                Throw New ArgumentNullException("values")
            End If
            If (values.Length = 0) Then
                Throw New ArgumentException("Input cannot be empty.")
            End If

            Dim min As Double = Double.MaxValue
            Dim max As Double = Double.MinValue
            Dim sum As Double = 0.0
            For i As Integer = 0 To values.Length - 1
                min = Math.Min(min, values(i))
                max = Math.Max(max, values(i))
                sum += values(i)
            Next
            Return New Tuple(Of Double, Double, Double)(min, max, sum)
        End Function

        ''' <summary>
        ''' Return the standard deviation of the given values.
        ''' </summary>
        Public Function StDev(values As Double()) As Double
            Return Common.StDev(values, Common.Mean(values))
        End Function

        ''' <summary>
        ''' Return the standard deviation of the given values.
        ''' The overload is faster because the mean of the values is provided.
        ''' </summary>
        Public Function StDev(values As Double(), mean As Double) As Double
            Dim sumVariancesSquared As Double = 0
            For i As Integer = 0 To values.Length - 1
                Dim pointVariance As Double = Math.Abs(mean - values(i))
                Dim pointVarianceSquared As Double = Math.Pow(pointVariance, 2)
                sumVariancesSquared += pointVarianceSquared
            Next
            Return Math.Sqrt(sumVariancesSquared / values.Length)
        End Function

        ''' <summary>
        ''' Return the standard error of the given values.
        ''' </summary>
        Public Function StdErr(values As Double()) As Double
            Return Common.StDev(values) / Math.Sqrt(values.Length)
        End Function

        ''' <summary>
        ''' Return the mean of the given values.
        ''' </summary>
        Public Function Mean(values As Double()) As Double
            Dim sum As Double = 0
            For i As Integer = 0 To values.Length - 1
                sum += values(i)
            Next
            Return sum / values.Length
        End Function

        ''' <summary>
        ''' Return the N-th smallest value in the given array.
        ''' </summary>
        Public Function NthOrderStatistic(values As Double(), n As Integer) As Double
            If (n < 1) OrElse (n > values.Length) Then
                Throw New ArgumentException("N must be a number from 1 to the length of the array.")
            End If
            Dim valuesCopy As Double() = New Double(values.Length - 1) {}
            Array.Copy(values, valuesCopy, values.Length)
            Return Common.QuickSelect(valuesCopy, 0, values.Length - 1, n - 1)
        End Function

        ''' <summary>
        ''' Return the value of the N-th quantile.
        ''' </summary>
        Public Function Quantile(values As Double(), n As Integer, quantileCount As Integer) As Double
            If (n = 0) Then
                Return values.Min()
            End If
            If (n = quantileCount) Then
                Return values.Max()
            End If
            Return Common.NthOrderStatistic(values, CInt(n * values.Length / quantileCount))
        End Function

        ''' <summary>
        ''' Return the value of the N-th quartile.
        ''' </summary>
        ''' <param name="quart">Quartile 1, 2, or 3.</param>
        Public Function Quartile(values As Double(), quart As Integer) As Double
            Return Common.Quantile(values, quart, 4)
        End Function

        ''' <summary>
        ''' Return the percentile of the given array.
        ''' </summary>
        ''' <param name="percent">Number from 0 to 100.</param>
        Public Function Percentile(values As Double(), percent As Integer) As Double
            Return Common.Quantile(values, percent, 100)
        End Function

        ''' <summary>
        ''' Return the percentile of the given array.
        ''' </summary>
        ''' <param name="percent">Number from 0 to 100.</param>
        Public Function Percentile(values As Double(), percent As Double) As Double
            If (percent = 0) Then
                Return values.Min()
            End If
            If (percent = 100) Then
                Return values.Max()
            End If
            Dim percentileIndex As Integer = CInt(Percentile / 100 * (values.Length - 1))
            Dim copiedValues As Double() = New Double(values.Length - 1) {}
            Array.Copy(values, copiedValues, values.Length)
            Return Common.NthOrderStatistic(values, percentileIndex + 1)
        End Function

        ''' <summary>
        ''' Return the median of the given array.
        ''' If the length of the array is even, this value is the mean of the upper and lower medians.
        ''' </summary>
        Public Function Median(values As Double()) As Double
            If (values.Length Mod 2 = 1) Then
                Return Common.NthOrderStatistic(values, CInt(values.Length / 2 + 1))
            End If
            Dim lowerMedian As Double = Common.NthOrderStatistic(values, CInt(values.Length / 2))
            Dim upperMedian As Double = Common.NthOrderStatistic(values, CInt(values.Length / 2 + 1))
            Return (lowerMedian + upperMedian) / 2
        End Function

        ''' <summary>
        ''' Return the k-th smallest value from a range of the given array. 
        ''' WARNING: values will be mutated.
        ''' </summary>
        ''' <param name="leftIndex">Inclusive lower bound.</param>
        ''' <param name="rightIndex">Inclusive upper bound.</param>
        ''' <param name="k">Number starting at 0.</param>
        ''' <remarks>
        ''' QuickSelect (aka Hoare's Algorithm) is a selection algorithm:
        ''' - Given an integer k it returns the kth smallest element in a sequence) with O(n) expected time.
        ''' - In the worst case it is O(n^2), i.e. when the chosen pivot is always the max Or min at each call.
        ''' - The use of a random pivot virtually assures linear time performance.
        ''' - https://en.wikipedia.org/wiki/Quickselect
        ''' </remarks>
        Private Function QuickSelect(values As Double(), leftIndex As Integer, rightIndex As Integer, k As Integer) As Double
            If (leftIndex = rightIndex) Then
                Return values(leftIndex)
            End If
            If (k = 0) Then
                Dim min As Double = values(leftIndex)
                For i As Integer = leftIndex To rightIndex
                    If (values(i) < min) Then
                        min = values(i)
                    End If
                Next
                Return min
            End If

            If (k = rightIndex - leftIndex) Then
                Dim max As Double = values(leftIndex)
                For i As Integer = leftIndex To rightIndex
                    If (values(i) > max) Then
                        max = values(i)
                    End If
                Next
                Return max
            End If

            Dim partitionIndex As Integer = Common.Partition(values, leftIndex, rightIndex)
            Dim pivotIndex As Integer = partitionIndex - leftIndex

            If (k = pivotIndex) Then
                Return values(partitionIndex)
            ElseIf (k < pivotIndex) Then
                Return Common.QuickSelect(values, leftIndex, partitionIndex - 1, k)
            End If
            Return Common.QuickSelect(values, partitionIndex + 1, rightIndex, k - pivotIndex - 1)
        End Function

        ''' <summary>
        ''' Return a random integer from within the given range
        ''' </summary>
        ''' <param name="min">Inclusive lower bound.</param>
        ''' <param name="max">Exclusive upper bound.</param>
        Public Function GetRandomInt(min As Integer, max As Integer) As Integer
            Dim randomBytes As Byte() = New Byte(3) {}
            Common.Rand.GetBytes(randomBytes)
            Dim randomInt As Integer = BitConverter.ToInt32(randomBytes, 0)
            Return Math.Abs(randomInt Mod (max - min + 1)) + min
        End Function

        ''' <summary>
        ''' Partition the array between the defined bounds according to elements above and below a randomly chosen pivot value.
        ''' </summary>
        ''' <returns>Index of the pivot used.</returns>
        Private Function Partition(values As Double(), leftIndex As Integer, rightIndex As Integer) As Integer
            'Moving the pivot to the end is far easier than handling it where it is
            'This also allows you to turn this into the non-randomized Partition
            Dim initialPivotIndex As Integer = Common.GetRandomInt(leftIndex, rightIndex)
            Dim swap As Double = values(initialPivotIndex)
            values(initialPivotIndex) = values(rightIndex)
            values(rightIndex) = swap

            Dim pivotValue As Double = values(rightIndex)
            Dim pivotIndex As Integer = leftIndex - 1
            For i As Integer = leftIndex To rightIndex - 1
                If (values(i) <= pivotValue) Then
                    pivotIndex += 1
                    Dim tmp As Double = values(i)
                    values(i) = values(pivotIndex)
                    values(pivotIndex) = tmp
                End If
            Next
            pivotIndex += 1
            Dim tmp2 As Double = values(rightIndex)
            values(rightIndex) = values(pivotIndex)
            values(pivotIndex) = tmp2
            Return pivotIndex
        End Function

        ''' <summary>
        ''' Given a dataset of values return the probability density function.
        ''' The returned function is a Gaussian curve from 0 to 1 (not normalized).
        ''' </summary>
        ''' <param name="values">Original dataset.</param>
        ''' <returns>Function to return Y for a given X.</returns>
        Public Function ProbabilityDensityFunction(values As Double()) As Func(Of Double, Double?)
            Dim stats As BasicStats = New BasicStats(values)
            Return Function(x As Double) Math.Exp(-0.5 * Math.Pow((x - stats.Mean) / stats.StDev, 2))
        End Function

        ''' <summary>
        ''' Given a dataset of values return the probability density function at specific X positions.
        ''' Returned values will be normalized such that their integral is 1.
        ''' </summary>
        ''' <param name="values">Original dataset.</param>
        ''' <param name="xs">Positions (Xs) for which probabilities (Ys) will be returned.</param>
        ''' <param name="percent">If True, output will be multiplied by 100.</param>
        ''' <returns>Densities (Ys) for each of the given Xs.</returns>
        Public Function ProbabilityDensity(values As Double(), xs As Double(), Optional percent As Boolean = False) As Double()
            Dim f As Func(Of Double, Double?) = Common.ProbabilityDensityFunction(values)
            Dim ys As Double() = xs.Select(Function(x As Double) f(x).Value).ToArray()
            Dim sum As Double = ys.Sum()
            If percent Then
                sum /= 100.0
            End If
            For i As Integer = 0 To ys.Length - 1
                ys(i) /= sum
            Next
            Return ys
        End Function

        ''' <summary>
        ''' Return the cumulative sum of the given data.
        ''' </summary>
        Public Function CumulativeSum(values As Double()) As Double()
            Dim sum As Double() = New Double(values.Length - 1) {}
            sum(0) = values(0)
            For i As Integer = 1 To values.Length - 1
                sum(i) = sum(i - 1) + values(i)
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Compute the histogram of a dataset.
        ''' </summary>
        ''' <param name="values">Input data.</param>
        ''' <param name="min">Lower edge of the first bin (inclusive). If NaN, minimum of input values will be used.</param>
        ''' <param name="max">High edge of the largest bin (inclusive). If NaN, maximum of input values will be used.</param>
        ''' <param name="binSize">Width of each bin.</param>
        ''' <param name="density">If False, the result will contain the number of samples in each bin. 
        ''' If True, the result is the value of the probability density function at the bin (the sum of all values will be 1 if the bin size is 1).</param>
        Public Function Histogram(values As Double(), min As Double, max As Double, binSize As Double, Optional density As Boolean = False) As Tuple(Of Double(), Double())
            Dim binCount As Integer = CInt((max - min) / binSize)
            Return Common.Histogram(values, binCount, density, min, max)
        End Function

        ''' <summary>
        ''' Compute the histogram of a dataset.
        ''' </summary>
        ''' <param name="values">Input data.</param>
        ''' <param name="binCount">Number of equal-width bins.</param>
        ''' <param name="density">If False, the result will contain the number of samples in each bin. 
        ''' If True, the result is the value of the probability density function at the bin (the sum of all values will be 1 if the bin size is 1).</param>
        ''' <param name="min">Lower edge of the first bin (inclusive). If NaN, minimum of input values will be used.</param>
        ''' <param name="max">High edge of the largest bin (inclusive). If NaN, maximum of input values will be used.</param>
        ''' <remarks>
        ''' Note: function signature loosely matches numpy: 
        ''' https//numpy.org/doc/stable/reference/generated/numpy.histogram.html
        ''' </remarks>
        Public Function Histogram(values As Double(), binCount As Integer, Optional density As Boolean = False, Optional min As Double = Double.NaN, Optional max As Double = Double.NaN) As Tuple(Of Double(), Double())
            'determine min/max based on the data (if not provided)
            If Double.IsNaN(min) OrElse Double.IsNaN(max) Then
                Dim stats As New BasicStats(values)
                If Double.IsNaN(min) Then
                    min = stats.Min
                End If
                If Double.IsNaN(max) Then
                    max = stats.Max
                End If
            End If

            'create evenly sized bins
            Dim binWidth As Double = (max - min) / binCount
            Dim binEdges As Double() = New Double(binCount + 1 - 1) {}
            For i As Integer = 0 To binEdges.Length - 1
                binEdges(i) = min + binWidth * i
            Next

            'place values in histogram
            Dim hist As Double() = New Double(binCount - 1) {}
            For i As Integer = 0 To values.Length - 1
                If (values(i) < min OrElse values(i) > max) Then
                    Continue For
                End If

                If (values(i) = max) Then
                    hist(hist.Length - 1) += 1
                    Continue For
                End If

                Dim distanceFromMin As Double = values(i) - min
                Dim binsFromMin As Integer = CInt(distanceFromMin / binWidth)
                hist(binsFromMin) += 1
            Next

            'optionally normalize the data
            If density Then
                Dim binScale As Double = hist.Sum() * binWidth
                For i As Integer = 0 To hist.Length - 1
                    hist(i) /= binScale
                Next
            End If
            Return New Tuple(Of Double(), Double())(hist, binEdges)
        End Function

    End Module

End Namespace