Imports System.Linq

Namespace ScottPlot.Statistics

    Public Module Finance

        ''' <summary>
        ''' Simple moving average.
        ''' </summary>
        ''' <param name="values">Number of values to use for each calculation.</param>
        ''' <param name="period">Number of values to use for each calculation.</param>
        ''' <param name="trimNan">Only return data where values are present for the whole period.</param>
        Public Function SMA(values As Double(), period As Integer, Optional trimNan As Boolean = True) As Double()
            If (period < 2) Then
                Throw New ArgumentException("Period must be 2 or greater.")
            End If
            If (period > values.Length) Then
                Throw New ArgumentException("Period cannot be longer than number of values.")
            End If

            Dim smas As Double() = New Double(values.Length - 1) {}
            For i As Integer = 0 To values.Length - 1
                If (i < period) Then
                    smas(i) = Double.NaN
                Else
                    Dim periodValues As Double() = New Double(period - 1) {}
                    Array.Copy(values, i - period + 1, periodValues, 0, period)
                    smas(i) = Common.Mean(periodValues)
                End If
            Next

            Return If(trimNan, smas.Skip(period).ToArray(), smas)
        End Function

        ''' <summary>
        ''' Simple moving standard deviation.
        ''' </summary>
        ''' <param name="period">Number of values to use for each calculation.</param>
        Public Function SMStDev(values As Double(), period As Integer) As Double()
            If (period < 2) Then
                Throw New ArgumentException("Period must be 2 or greater.")
            End If
            If (period > values.Length) Then
                Throw New ArgumentException("Period cannot be longer than number of values.")
            End If

            Dim stDev As Double() = New Double(values.Length - 1) {}
            For i As Integer = 0 To values.Length - 1
                If i < period Then
                    stDev(i) = Double.NaN
                Else
                    Dim periodValues As Double() = New Double(period - 1) {}
                    Array.Copy(values, i - period + 1, periodValues, 0, period)
                    stDev(i) = Common.StDev(periodValues)
                End If
            Next
            Return stDev
        End Function

        ''' <summary>
        ''' Return the simple moving average (SMA) of the OHLC closing prices.
        ''' The returned data will be shorter than the input data by N points.
        ''' </summary>
        ''' <param name="ohlcs">Price data to analyze.</param>
        ''' <param name="n">Each returned price represents the average of N prices.</param>
        Public Function SMA(ohlcs As OHLC(), n As Integer) As Double()
            Dim closingPrices As Double() = New Double(ohlcs.Length - 1) {}
            For i As Integer = 0 To ohlcs.Length - 1
                closingPrices(i) = ohlcs(i).Close
            Next
            Return Finance.SMA(closingPrices, n, True)
        End Function

        ''' <summary>
        ''' Return the SMA and upper/lower Bollinger bands for the given price data.
        ''' The returned data will not be shorter than the input data. It will contain NaN values at the front.
        ''' </summary>
        ''' <param name="prices">Price data to use for analysis.</param>
        ''' <param name="n">Each returned price represents the average of N prices.</param>
        ''' <param name="sdCoeff">Number of standard deviations from the mean to use for the Bollinger bands.</param>
        Public Function Bollinger(prices As Double(), n As Integer, Optional sdCoeff As Double = 2) As Tuple(Of Double(), Double(), Double())
            Dim sma As Double() = Finance.SMA(prices, n, False)
            Dim smstd As Double() = Finance.SMStDev(prices, n)
            Dim bolU As Double() = New Double(prices.Length - 1) {}
            Dim bol As Double() = New Double(prices.Length - 1) {}
            For i As Integer = 0 To prices.Length - 1
                bol(i) = sma(i) - sdCoeff * smstd(i)
                bolU(i) = sma(i) + sdCoeff * smstd(i)
            Next
            Return New Tuple(Of Double(), Double(), Double())(sma, bol, bolU)
        End Function

        ''' <summary>
        ''' Return the SMA and upper/lower Bollinger bands for the closing price of the given OHLCs.
        ''' The returned data will be shorter than the input data by N values.
        ''' </summary>
        ''' <param name="ohlcs">Price data to use for analysis</param>
        ''' <param name="N">Each returned price represents the average of N prices</param>
        ''' <param name="sdCoeff">Number of standard deviations from the mean to use for the Bollinger bands.</param>
        Public Function Bollinger(ohlcs As OHLC(), N As Integer, Optional sdCoeff As Double = 2.0) As Tuple(Of Double(), Double(), Double())
            Dim closingPrices As Double() = New Double(ohlcs.Length - 1) {}
            For i As Integer = 0 To ohlcs.Length - 1
                closingPrices(i) = ohlcs(i).Close
            Next

            Dim t As Tuple(Of Double(), Double(), Double()) = Finance.Bollinger(closingPrices, N, sdCoeff)
            Dim sma As Double() = t.Item1
            Dim lower As Double() = t.Item2
            Dim upper As Double() = t.Item3

            'skip the first points which all contain NaN
            sma = sma.Skip(N).ToArray()
            lower = lower.Skip(N).ToArray()
            upper = upper.Skip(N).ToArray()

            Return New Tuple(Of Double(), Double(), Double())(sma, lower, upper)
        End Function

    End Module

End Namespace