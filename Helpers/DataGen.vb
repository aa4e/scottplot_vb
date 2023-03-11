Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace ScottPlot

    Public Module DataGen

        ''' <summary>
        ''' Generates an array of numbers with constant spacing.
        ''' </summary>
        ''' <param name="pointCount">The number of points.</param>
        ''' <param name="spacing">The space between points. Default 1.</param>
        ''' <param name="offset">The first point. Default 0.</param>
        ''' <returns>An array of numbers with constant spacing.</returns>
        Public Function Consecutive(pointCount As Integer, Optional spacing As Double = 1, Optional offset As Double = 0) As Double()
            Dim ys As Double() = New Double(pointCount - 1) {}
            For i As Integer = 0 To ys.Length - 1
                ys(i) = i * spacing + offset
            Next
            Return ys
        End Function

        Public Function Consecutive(Of T)(pointCount As Integer, spacing As T, offset As T) As T()
            Dim spacingDouble As Double = NumericConversion.GenericToDouble(Of T)(spacing)
            Dim offsetDouble As Double = NumericConversion.GenericToDouble(Of T)(offset)
            Dim values As Double() = Consecutive(pointCount, spacingDouble, offsetDouble)
            Dim valuesGeneric As T() = NumericConversion.ToGenericArray(Of T)(values)
            Return valuesGeneric
        End Function

        ''' <summary>
        ''' Generates an array of sine values of an input array.
        ''' </summary>
        ''' <param name="xs">The arguments to the sine function.</param>
        ''' <param name="mult">A number to multiply the output by. Default 1.</param>
        ''' <returns>An array of sine values.</returns>
        Public Function Sin(xs As Double(), Optional mult As Double = 1) As Double()
            Dim ys As Double() = New Double(xs.Length - 1) {}
            For i As Integer = 0 To xs.Length - 1
                ys(i) = Math.Sin(xs(i)) * mult
            Next
            Return ys
        End Function

        ''' <summary>
        ''' Generates an array of sine values.
        ''' </summary>
        ''' <param name="pointCount">The number of values to generate.</param>
        ''' <param name="oscillations">The number of periods. Default 1.</param>
        ''' <param name="offset">The number to increment the output by. Default 0.</param>
        ''' <param name="mult">The number to multiply the output by. Default 1.</param>
        ''' <param name="phase">The fraction of a period to offset by. Default 0.</param>
        ''' <returns>An array of sine values</returns>
        Public Function Sin(pointCount As Integer, Optional oscillations As Double = 1, Optional offset As Double = 0, Optional mult As Double = 1, Optional phase As Double = 0) As Double()
            Dim sinScale As Double = 2 * Math.PI * oscillations / (pointCount - 1)
            Dim ys As Double() = New Double(pointCount - 1) {}
            For i As Integer = 0 To ys.Length - 1
                ys(i) = Math.Sin(i * sinScale + phase * Math.PI * 2) * mult + offset
            Next
            Return ys
        End Function

        ''' <summary>
        ''' Return data for a sine wave that increases frequency toward the end of an array. 
        ''' This function may be useful for inspecting rendering artifacts when data is displayed at different densities.
        ''' </summary>
        ''' <param name="pointCount">The number of values to generate.</param>
        ''' <param name="density">Increasing this value increases maximum frequency.</param>
        ''' <returns>An array of values.</returns>
        Public Function SinSweep(pointCount As Integer, Optional density As Double = 50) As Double()
            Dim data As Double() = New Double(pointCount - 1) {}
            For i As Integer = 0 To data.Length - 1
                Dim t As Double = i / pointCount * density
                Dim tSquared As Double = Math.Pow(t, 2)
                data(i) = Math.Sin(tSquared)
            Next
            Return data
        End Function

        ''' <summary>
        ''' Generates an array of cosine values of an input array.
        ''' </summary>
        ''' <param name="xs">The arguments to the cosine function.</param>
        ''' <param name="mult">A number to multiply the output by. Default 1.</param>
        ''' <returns>An array of cosine values.</returns>
        Public Function Cos(xs As Double(), Optional mult As Double = 1) As Double()
            Dim ys As Double() = New Double(xs.Length - 1) {}
            For i As Integer = 0 To xs.Length - 1
                ys(i) = Math.Cos(xs(i)) * mult
            Next
            Return ys
        End Function

        ''' <summary>
        ''' Generates an array of cosine values.
        ''' </summary>
        ''' <param name="pointCount">The number of values to generate.</param>
        ''' <param name="oscillations">The number of periods. Default 1.</param>
        ''' <param name="offset">The number to increment the output by. Default 0.</param>
        ''' <param name="mult">The number to multiply the output by. Default 1.</param>
        ''' <param name="phase">The fraction of a period to offset by. Default 0.</param>
        ''' <returns>An array of cosine values.</returns>
        Public Function Cos(pointCount As Integer, Optional oscillations As Double = 1, Optional offset As Double = 0, Optional mult As Double = 1, Optional phase As Double = 0) As Double()
            Dim sinScale As Double = 2 * Math.PI * oscillations / (pointCount - 1)
            Dim ys As Double() = New Double(pointCount - 1) {}
            For i As Integer = 0 To ys.Length - 1
                ys(i) = Math.Cos(i * sinScale + phase * Math.PI * 2) * mult + offset
            Next
            Return ys
        End Function

        ''' <summary>
        ''' Generates an array of tangent values of an input array.
        ''' </summary>
        ''' <param name="xs">The arguments to the tangent function.</param>
        ''' <param name="mult">A number to multiply the output by. Default 1.</param>
        ''' <returns>An array of tangent values.</returns>
        Public Function Tan(xs As Double(), Optional mult As Double = 1) As Double()
            Dim ys As Double() = New Double(xs.Length - 1) {}
            For i As Integer = 0 To xs.Length - 1
                ys(i) = Math.Tan(xs(i)) * mult
            Next
            Return ys
        End Function

        ''' <summary>
        ''' Generates an array of random numbers following a uniform distribution on the interval [offset, multiplier].
        ''' </summary>
        ''' <param name="rand">The Random object to use.</param>
        ''' <param name="pointCount">The number of random points to generate.</param>
        ''' <param name="multiplier">The maximum number above offset that may be generated.</param>
        ''' <param name="offset">The minimum number that may be generated.</param>
        ''' <returns>An array of random numbers.</returns>
        Public Function Random(rand As Random, pointCount As Integer, Optional multiplier As Double = 1, Optional offset As Double = 0) As Double()
            If (rand is Nothing) Then
                rand = New Random()
            End If
            Dim ys As Double() = New Double(pointCount - 1) {}
            For i As Integer = 0 To pointCount - 1
                ys(i) = rand.NextDouble() * multiplier + offset
            Next
            Return ys
        End Function

        Public Function Random(Of T)(rand As Random, pointCount As Integer, multiplier As T, offset As T) As T()
            Dim multDouble As Double = NumericConversion.GenericToDouble(multiplier)
            Dim offsetDouble As Double = NumericConversion.GenericToDouble(offset)
            Dim values As Double() = Random(rand, pointCount, multDouble, offsetDouble)
            Dim valuesGeneric As T() = NumericConversion.ToGenericArray(Of T)(values)
            Return valuesGeneric
        End Function

        ''' <summary>
        ''' Generates a 2D array of random numbers between 0 and 1 (uniform distribution).
        ''' </summary>
        ''' <param name="rand">The Random object to use.</param>
        ''' <param name="rows">Number of rows (dimension 0).</param>
        ''' <param name="columns">Number of columns (dimension 1).</param>
        ''' <param name="multiplier">Multiply values by this number after generation.</param>
        ''' <param name="offset">Add to values after multiplication.</param>
        ''' <returns>2D array filled with random numbers.</returns>
        Public Function Random2D(rand As Random, rows As Integer, columns As Integer, Optional multiplier As Double = 1, Optional offset As Double = 0) As Double(,)
            If (rand is Nothing) Then
                Throw New ArgumentNullException()
            End If
            Dim data As Double(,) = New Double(rows - 1, columns - 1) {}
            For y As Integer = 0 To data.GetLength(0) - 1
                For x As Integer = 0 To data.GetLength(1) - 1
                    data(y, x) = rand.NextDouble() * multiplier + offset
                Next
            Next
            Return data
        End Function

        ''' <summary>
        ''' Generates a 2D array of numbers with constant spacing.
        ''' </summary>
        ''' <param name="rows">Number of rows (dimension 0).</param>
        ''' <param name="columns">Number of columns (dimension 1).</param>
        ''' <param name="spacing">The space between points.</param>
        ''' <param name="offset">The first point.</param>
        Public Function Consecutive2D(rows As Integer, columns As Integer, Optional spacing As Double = 1, Optional offset As Double = 0) As Double(,)
            Dim data As Double(,) = New Double(rows - 1, columns - 1) {}
            Dim count As Double = offset
            For y As Integer = 0 To data.GetLength(0) - 1
                For x As Integer = 0 To data.GetLength(1) - 1
                    data(y, x) = count
                    count += spacing
                Next
            Next
            Return data
        End Function

        ''' <summary>
        ''' Generates a 2D sine pattern.
        ''' </summary>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <param name="xPeriod">Frequency factor in x direction.</param>
        ''' <param name="yPeriod">Frequency factor in y direction.</param>
        ''' <param name="multiple">Intensity factor.</param>
        Public Function Sin2D(width As Integer, height As Integer, Optional xPeriod As Double = 0.2, Optional yPeriod As Double = 0.2, Optional multiple As Double = 100) As Double(,)
            Dim intensities As Double(,) = New Double(height - 1, width - 1) {}
            For y As Integer = 0 To height - 1
                Dim sunY As Double = Math.Cos(y * yPeriod) * multiple

                For x As Integer = 0 To width - 1
                    Dim sinX As Double = Math.Sin(x * xPeriod) * multiple
                    intensities(y, x) = sinX + sunY
                Next
            Next
            Return intensities
        End Function

        ''' <summary>
        ''' Generate a 2D array in a diagonal gradient pattern.
        ''' </summary>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        Public Function Ramp2D(width As Integer, height As Integer, Optional min As Double = 0, Optional max As Double = 1) As Double(,)
            Dim intensities As Double(,) = New Double(height - 1, width - 1) {}
            Dim span As Double = max - min
            For y As Integer = 0 To height - 1
                Dim fracY As Double = y / height
                Dim valY As Double = fracY * span + min

                For x As Integer = 0 To width - 1
                    Dim fracX As Double = x / width
                    Dim valX As Double = fracX * span + min
                    intensities(y, x) = (valX + valY) * 0.5
                Next
            Next
            Return intensities
        End Function

        ''' <summary>
        ''' Generates an array of random numbers following a uniform distribution on the interval [offset, multiplier].
        ''' </summary>
        ''' <param name="rand">The Random object to use.</param>
        ''' <param name="pointCount">The number of random points to generate.</param>
        ''' <param name="multiplier">The maximum number above offset that may be generated.</param>
        ''' <param name="offset">The minimum number that may be generated.</param>
        ''' <returns>An array of random numbers.</returns>
        Public Function RandomInts(rand As Random, pointCount As Integer, Optional multiplier As Double = 1, Optional offset As Double = 0) As Integer()
            If (rand is Nothing) Then
                rand = New Random()
            End If
            Dim ys As Integer() = New Integer(pointCount - 1) {}
            For i As Integer = 0 To pointCount - 1
                ys(i) = CInt(rand.NextDouble() * multiplier + offset)
            Next
            Return ys
        End Function

        ''' <summary>
        ''' Generates a single value from a normal distribution.
        ''' </summary>
        ''' <param name="rand">The Random object to use.</param>
        ''' <param name="mean">The mean of the distribution.</param>
        ''' <param name="stdDev">The standard deviation of the distribution.</param>
        ''' <param name="maxSdMultiple">The maximum distance from the mean to generate, given as a multiple of the standard deviation.</param>
        ''' <returns>A single value from a normal distribution.</returns>
        Public Function RandomNormalValue(rand As Random, mean As Double, stdDev As Double, Optional maxSdMultiple As Double = 10) As Double
            While True
                Dim u1 As Double = UniformOpenInterval(rand)
                Dim u2 As Double = UniformOpenInterval(rand)
                Dim randStdNormal As Double = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2)
                If (Math.Abs(randStdNormal) < maxSdMultiple) Then
                    Return mean + stdDev * randStdNormal
                End If
            End While
            Throw New Exception()
        End Function

        Private Function UniformOpenInterval(rand As Random) As Double
            Dim value As Double = 0
            While (value = 0)
                value = rand.NextDouble()
            End While
            Return value
        End Function

        ''' <summary>
        ''' Generates an array of values from a normal distribution.
        ''' </summary>
        ''' <param name="seed">The number to seed the random number generator with.</param>
        ''' <param name="pointCount">The number of points to generate.</param>
        ''' <param name="mean">The mean of the distribution.</param>
        ''' <param name="stdDev">The standard deviation of the distribution.</param>
        ''' <param name="maxSdMultiple">The maximum distance from the mean to generate, given as a multiple of the standard deviation.</param>
        ''' <returns>An array of values from a normal distribution.</returns>
        Public Function RandomNormal(seed As Integer, pointCount As Integer, Optional mean As Double = 0.5, Optional stdDev As Double = 0.5, Optional maxSdMultiple As Double = 10) As Double()
            Return DataGen.RandomNormal(New Random(seed), pointCount, mean, stdDev, maxSdMultiple)
        End Function

        ''' <summary>
        ''' Generates an array of values from a normal distribution.
        ''' </summary>
        ''' <param name="rand">The Random object to use.</param>
        ''' <param name="pointCount">The number of points to generate.</param>
        ''' <param name="mean">The mean of the distribution.</param>
        ''' <param name="stdDev">The standard deviation of the distribution.</param>
        ''' <param name="maxSdMultiple">The maximum distance from the mean to generate, given as a multiple of the standard deviation.</param>
        ''' <returns>An array of values from a normal distribution.</returns>
        Public Function RandomNormal(rand As Random, pointCount As Integer, Optional mean As Double = 0.5, Optional stdDev As Double = 0.5, Optional maxSdMultiple As Double = 10) As Double()
            If (rand is Nothing) Then
                rand = New Random(0)
            End If
            Dim values As Double() = New Double(pointCount - 1) {}
            For i As Integer = 0 To values.Length - 1
                values(i) = DataGen.RandomNormalValue(rand, mean, stdDev, maxSdMultiple)
            Next
            Return values
        End Function

        ''' <summary>
        ''' Generates an array of data with normally distributed residuals about a line.
        ''' </summary>
        ''' <param name="rand">The Random object to use.</param>
        ''' <param name="pointCount">The number of points to generate. Default 100.</param>
        ''' <param name="slope">The slope of the line. Default 1.</param>
        ''' <param name="offset">The y-intercept of the line. Default 0.</param>
        ''' <param name="noise">The standard deviation of the residuals. Default 0.1</param>
        ''' <returns>An array of approximately linear data.</returns>
        Public Function NoisyLinear(rand As Random, Optional pointCount As Integer = 100, Optional slope As Double = 1, Optional offset As Double = 0, Optional noise As Double = 0.1) As Double()
            If (rand is Nothing) Then
                rand = New Random(0)
            End If
            Dim data As Double() = New Double(pointCount - 1) {}
            For i As Integer = 0 To data.Length - 1
                data(i) = slope * i + offset + DataGen.RandomNormalValue(rand, 0, noise)
            Next
            Return data
        End Function

        ''' <summary>
        ''' Generates an array of data with uniformally distributed residuals about a sinusoidal curve.
        ''' </summary>
        ''' <param name="rand">The Random object to use.</param>
        ''' <param name="pointCount">The number of points to generate.</param>
        ''' <param name="oscillations">The number of periods. Default 1.</param>
        ''' <param name="noiseLevel">Twice the maximum residual, in units of mult. Default 0.5</param>
        ''' <param name="mult">The number to multiply the residuals by. Default 1.</param>
        ''' <returns>An array of approximately sinusoidal data.</returns>
        Public Function NoisySin(rand As Random, pointCount As Integer, Optional oscillations As Double = 1, Optional noiseLevel As Double = 0.5, Optional mult As Double = 1) As Double()
            If (rand is Nothing) Then
                rand = New Random(0)
            End If
            Dim values As Double() = DataGen.Sin(pointCount, oscillations)
            For i As Integer = 0 To values.Length - 1
                values(i) += (rand.NextDouble() - 0.5) * noiseLevel * mult
            Next
            Return values
        End Function

        ''' <summary>
        ''' Generates a random color.
        ''' </summary>
        ''' <param name="rand">The Random object to use.</param>
        ''' <param name="min">The min of each component. Default 0.</param>
        ''' <param name="max">The max of each component. Default 255.</param>
        ''' <returns>A random color.</returns>
        Public Function RandomColor(rand As Random, Optional min As Integer = 0, Optional max As Integer = 255) As Color
            If (rand is Nothing) Then
                rand = New Random()
            End If
            Dim red As Integer = rand.Next(min, max)
            Dim green As Integer = rand.Next(min, max)
            Dim blue As Integer = rand.Next(min, max)
            Return Color.FromArgb(red, green, blue)
        End Function

        ''' <summary>
        ''' Return the cumulative sum of a random set of numbers using a fixed seed
        ''' </summary>
        ''' <param name="pointCount">The number of points to generate.</param>
        ''' <param name="mult">The max difference between points in the walk. Default 1.</param>
        ''' <param name="offset">The first point in the walk. Default 0.</param>
        ''' <returns>The cumulative sum of a random set of numbers.</returns>
        Public Function RandomWalk(pointCount As Integer, Optional mult As Double = 1, Optional offset As Double = 0) As Double()
            Return DataGen.RandomWalk(New Random(0), pointCount, mult, offset)
        End Function

        ''' <summary>
        ''' Return the cumulative sum of a random set of numbers.
        ''' </summary>
        ''' <param name="rand">The random object to use.</param>
        ''' <param name="pointCount">The number of points to generate.</param>
        ''' <param name="mult">The max difference between points in the walk. Default 1.</param>
        ''' <param name="offset">The first point in the walk. Default 0.</param>
        ''' <returns>The cumulative sum of a random set of numbers.</returns>
        Public Function RandomWalk(rand As Random, pointCount As Integer, Optional mult As Double = 1, Optional offset As Double = 0) As Double()
            If (rand is Nothing) Then
                rand = New Random(0)
            End If
            Dim data As Double() = New Double(pointCount - 1) {}
            data(0) = offset
            For i As Integer = 1 To data.Length - 1
                data(i) = data(i - 1) + (rand.NextDouble() * 2 - 1) * mult
            Next
            Return data
        End Function

        Public Function RandomWalk(Of T)(rand As Random, pointCount As Integer, mult As T, offset As T) As T()
            Dim multDouble As Double = NumericConversion.GenericToDouble(mult)
            Dim offsetDouble As Double = NumericConversion.GenericToDouble(offset)
            Dim values As Double() = RandomWalk(rand, pointCount, multDouble, offsetDouble)
            Dim valuesGeneric As T() = NumericConversion.ToGenericArray(Of T)(values)
            Return valuesGeneric
        End Function

        ''' <summary>
        ''' Generate unevenly-spaced X/Y points.
        ''' X values walk upward (by values from 0 to 1);
        ''' Y values walk randomly (by values from -1 to 1).
        ''' </summary>
        Public Function RandomWalk2D(rand As Random, pointCount As Integer) As Tuple(Of Double(), Double())
            Dim ys As Double() = New Double(pointCount - 1) {}
            Dim xs As Double() = New Double(pointCount - 1) {}
            For i As Integer = 1 To ys.Length - 1
                ys(i) = ys(i - 1) + (rand.NextDouble() - 0.5) * 2
                xs(i) = xs(i - 1) + rand.NextDouble()
            Next
            Return New Tuple(Of Double(), Double())(xs, ys)
        End Function

        ''' <summary>
        ''' Return OHLC array with random prices X positions as DateTime.ToOATime() values using the given time delta.
        ''' </summary>
        ''' <param name="rand">The random object to use.</param>
        ''' <param name="pointCount">The number of prices to generate.</param>
        ''' <param name="delta">The difference in time between prices.</param>
        ''' <param name="mult">The max difference between base prices around which that day's prices independently vary. Default 10.</param>
        ''' <param name="startingPrice">The initial base price. Default 123.45</param>
        ''' <returns>OHLC array with random prices.</returns>
        Public Function RandomStockPrices(rand As Random, pointCount As Integer, delta As TimeSpan, Optional mult As Double = 10, Optional startingPrice As Double = 123.45) As OHLC()
            Dim ohlcs As OHLC() = DataGen.RandomStockPrices(rand, pointCount, mult, startingPrice)
            Dim dt As New DateTime(1985, 9, 24, 9, 30, 0)
            For i As Integer = 0 To ohlcs.Length - 1
                dt += delta
                While (dt.DayOfWeek = DayOfWeek.Saturday) OrElse (dt.DayOfWeek = DayOfWeek.Sunday)
                    dt += TimeSpan.FromDays(1)
                End While
                ohlcs(i).DateTime = dt
                ohlcs(i).TimeSpan = delta
            Next
            Return ohlcs
        End Function

        ''' <summary>
        ''' Return OHLC array with random prices X positions as sequential numbers (0, 1, 2, etc.).
        ''' </summary>
        ''' <param name="rand">The random object to use.</param>
        ''' <param name="pointCount">The number of prices to generate.</param>
        ''' <param name="mult">The max difference between base prices around which that day's prices independently vary. Default 10.</param>
        ''' <param name="startingPrice">The initial base price. Default 123.45</param>
        ''' <returns>OHLC array with random prices.</returns>
        Private Function RandomStockPrices(rand As Random, pointCount As Integer, Optional mult As Double = 10, Optional startingPrice As Double = 123.45) As OHLC()
            If (rand is Nothing) Then
                rand = New Random(0)
            End If
            Dim basePrices As Double() = DataGen.RandomWalk(rand, pointCount, mult, startingPrice)
            Dim ohlcs As OHLC() = New OHLC(pointCount - 1) {}

            For i As Integer = 0 To ohlcs.Length - 1
                Dim basePrice As Double = basePrices(i)
                Dim open As Double = rand.NextDouble() * 10 + 50
                Dim close As Double = rand.NextDouble() * 10 + 50
                Dim high As Double = Math.Max(open, close) + rand.NextDouble() * 10
                Dim low As Double = Math.Min(open, close) - rand.NextDouble() * 10

                open += basePrice
                close += basePrice
                high += basePrice
                low += basePrice

                ohlcs(i) = New OHLC(open, high, low, close, i)
            Next
            Return ohlcs
        End Function

        ''' <summary>
        ''' Return OHLC array with random prices X positions as sequential numbers (0, 1, 2, etc.).
        ''' </summary>
        ''' <param name="rand">The random object to use.</param>
        ''' <param name="pointCount">The number of prices to generate.</param>
        ''' <param name="mult">The max difference between base prices around which that day's prices independently vary. Default 10.</param>
        ''' <param name="startingPrice">The initial base price. Default 123.45</param>
        ''' <param name="deltaMinutes">The minutes between prices. Cumulative with deltaDays. Default 0.</param>
        ''' <param name="deltaDays">The days between prices. Cumulative with deltaMinutes. Default 1.</param>
        ''' <param name="sequential">Whether to use TimeSpan Or integer x axis.</param>
        ''' <returns>OHLC array with random prices.</returns>
        Public Function RandomStockPrices(rand As Random, pointCount As Integer, Optional mult As Double = 10, Optional startingPrice As Double = 123.45, Optional deltaMinutes As Integer = 0, Optional deltaDays As Integer = 1, Optional sequential As Boolean = True) As OHLC()
            Dim ts As TimeSpan = TimeSpan.FromMinutes(deltaMinutes) + TimeSpan.FromDays(deltaDays)
            Dim prices As OHLC() = If(sequential,
                DataGen.RandomStockPrices(rand, pointCount, mult, startingPrice),
                DataGen.RandomStockPrices(rand, pointCount, ts, mult, startingPrice))
            If (rand is Nothing) Then
                rand = New Random(0)
            End If

            For Each price As OHLC In prices
                price.Volume = rand.NextDouble() * 900 + 100
            Next
            Return prices
        End Function

        ''' <summary>
        ''' Generates a random span.
        ''' </summary>
        ''' <param name="rand">The random object to use.</param>
        ''' <param name="low">The minimum of the span. Default 0.</param>
        ''' <param name="high">Tge naximum of the span. Default 100.</param>
        ''' <param name="minimumSpacing">The minimum length of the span. Default 10.</param>
        ''' <returns>A random span.</returns>
        Public Function RandomSpan(Optional rand As Random = Nothing, Optional low As Double = 0, Optional high As Double = 100, Optional minimumSpacing As Double = 10) As Tuple(Of Double, Double)
            If (rand is Nothing) Then
                rand = New Random()
            End If
            Dim span As Double = Math.Abs(high - low)

            For attempts As Integer = 0 To 10_000 - 1
                Dim valA As Double = rand.NextDouble() * span + low
                Dim valB As Double = rand.NextDouble() * span + low
                If (Math.Abs(valA - valB) >= minimumSpacing) Then
                    If (valA < valB) Then
                        Return New Tuple(Of Double, Double)(valA, valB)
                    Else
                        Return New Tuple(Of Double, Double)(valB, valA)
                    End If
                End If
            Next
            Throw New ArgumentException()
        End Function

        ''' <summary>
        ''' Generates a range of values starting at 0 and separated by 1.
        ''' </summary>
        ''' <param name="endOfRange">The end of the range.</param>
        ''' <returns>A range of values.</returns>
        Public Function Range(endOfRange As Integer) As Double()
            Return DataGen.Range(0, endOfRange, 1)
        End Function

        ''' <summary>
        ''' Generates a range of values separated by 1.
        ''' </summary>
        ''' <param name="start">The start of the range.</param>
        ''' <param name="endOfRange">The end of the range.</param>
        ''' <returns>A range of values.</returns>
        Public Function Range(start As Integer, endOfRange As Integer) As Double()
            Return DataGen.Range(start, endOfRange, 1)
        End Function

        ''' <summary>
        ''' Generates a range of values.
        ''' </summary>
        ''' <param name="start">The start of the range.</param>
        ''' <param name="endOfRange">The end of the range.</param>
        ''' <param name="step">The space between values.</param>
        ''' <param name="includeStop">Indicates whether to include the stop point in the range. Default false.</param>
        ''' <returns>A range of values.</returns>
        Public Function Range(start As Double, endOfRange As Double, [step] As Double, Optional includeStop As Boolean = False) As Double()
            If ([step] <= 0) Then
                Throw New ArgumentException($"{NameOf([step])} must be >0. To make a descending series make {NameOf(endOfRange)} < {NameOf(start)}.")
            End If

            Dim valueSpan As Double = Math.Abs(start - endOfRange)
            Dim valueCount As Integer = CInt(valueSpan / [step])
            Dim stepSize As Double = If(endOfRange > start, [step], -[step])
            If includeStop Then
                valueSpan += 1
            End If

            Dim values As Double() = New Double(valueCount - 1) {}
            For i As Integer = 0 To valueCount - 1
                values(i) = start + i * stepSize
            Next
            Return values
        End Function

        ''' <summary>
        ''' Generates an array of zeros
        ''' </summary>
        ''' <param name="pointCount">The number of zeroes to generate</param>
        ''' <returns>An array of zeros.</returns>
        Public Function Zeros(pointCount As Integer) As Double()
            Return New Double(pointCount - 1) {}
        End Function

        ''' <summary>
        ''' Generates an array of ones
        ''' </summary>
        ''' <param name="pointCount">The number of ones to generate</param>
        ''' <returns>An array of ones.</returns>
        Public Function Ones(pointCount As Integer) As Double()
            Dim array As Double() = New Double(pointCount - 1) {}
            For i As Integer = 0 To pointCount - 1
                array(i) = 1
            Next
            Return array
        End Function

        ''' <summary>
        ''' Generates a Bitmap from data on the range [0, 255].
        ''' </summary>
        ''' <param name="data">The data to use.</param>
        ''' <param name="cmap">The colormap to use.</param>
        ''' <returns>A Bitmap.</returns>
        Public Function BitmapFrom2dArray(data As Double(,), cmap As ScottPlot.Drawing.Colormap) As Bitmap
            Dim width As Integer = data.GetLength(1)
            Dim height As Integer = data.GetLength(0)

            Dim bmp As New Bitmap(width, height, PixelFormat.Format8bppIndexed)
            Dim rect As New Rectangle(0, 0, width, height)
            Dim bmpData As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat)

            Dim bytes As Byte() = New Byte(bmpData.Stride * height - 1) {}
            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1
                    bytes(y * bmpData.Stride + x) = CByte(data(y, x))
                Next
            Next

            Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length)
            bmp.UnlockBits(bmpData)

            cmap.Apply(bmp)
            Dim bmp2 As New Bitmap(width, height, PixelFormat.Format32bppPArgb)

            Using gfx As Graphics = Graphics.FromImage(bmp2)
                gfx.DrawImage(bmp, 0, 0)
            End Using
            Return bmp2
        End Function

        ''' <summary>
        ''' Generates a sample Bitmap.
        ''' </summary>
        ''' <returns>A sample Bitmap.</returns>
        Public Function SampleImage() As Bitmap
            Return DataGen.BitmapFrom2dArray(DataGen.SampleImageData(), ScottPlot.Drawing.Colormap.Viridis)
        End Function

        ''' <summary>
        ''' Returns a sample 2D array of grayscale values as a nullable array with values scaled from 0 to 1.
        ''' </summary>
        Public Function SampleImageDataNullable() As Double?(,)
            Dim original As Double(,) = SampleImageData()

            Dim maxValue As Double = original(0, 0)
            For i As Integer = 0 To original.GetLength(0) - 1
                For j As Integer = 0 To original.GetLength(1) - 1
                    maxValue = Math.Max(maxValue, original(i, j))
                Next
            Next

            Dim nullable As Double?(,) = New Double?(original.GetLength(0) - 1, original.GetLength(1) - 1) {}
            For i As Integer = 0 To original.GetLength(0) - 1
                For j As Integer = 0 To original.GetLength(1) - 1
                    nullable(i, j) = 1 - original(i, j) / maxValue
                Next
            Next

            Return nullable
        End Function

        ''' <summary>
        ''' Returns a sample 2D array of grayscale values as a nullable array with values scaled from 0 to 1.
        ''' </summary>
        Public Function SampleImageData() As Double(,)
            Return New Double(,) {
                {95, 113, 117, 118, 120, 126, 135, 125, 120, 117, 120, 122, 124, 125, 118, 114, 113, 109, 108, 111, 113, 114, 113, 116, 114, 111, 113, 114, 111, 108, 111, 114, 116, 118, 114, 112, 116, 120, 116, 113, 109, 111, 108, 105, 104, 104, 108, 108, 109, 108, 107, 107, 108, 111, 111, 111, 111, 117, 116, 116, 117, 114, 112, 107, 100},
                {99, 117, 121, 122, 120, 125, 133, 125, 114, 117, 122, 125, 126, 129, 121, 117, 117, 111, 112, 112, 113, 114, 118, 122, 122, 118, 116, 116, 114, 112, 113, 116, 122, 124, 121, 120, 120, 121, 117, 116, 118, 117, 116, 117, 113, 111, 111, 113, 117, 118, 120, 116, 117, 116, 112, 116, 118, 118, 120, 118, 121, 124, 121, 122, 117},
                {100, 116, 126, 126, 124, 128, 125, 125, 124, 126, 130, 133, 126, 121, 121, 121, 116, 111, 114, 113, 112, 112, 118, 121, 121, 121, 120, 113, 113, 114, 116, 118, 120, 118, 117, 121, 122, 125, 125, 125, 128, 124, 125, 124, 116, 117, 117, 118, 121, 122, 122, 117, 121, 116, 116, 117, 126, 121, 120, 121, 124, 121, 114, 114, 107},
                {103, 121, 134, 134, 133, 133, 131, 130, 128, 128, 130, 130, 125, 125, 126, 118, 116, 116, 116, 116, 116, 117, 120, 122, 126, 129, 124, 121, 121, 121, 120, 120, 126, 125, 124, 125, 126, 125, 125, 130, 134, 134, 134, 134, 126, 129, 129, 125, 124, 125, 125, 125, 124, 124, 125, 131, 137, 131, 128, 129, 129, 128, 124, 114, 101},
                {107, 131, 137, 137, 143, 142, 137, 134, 135, 142, 141, 138, 137, 138, 134, 126, 124, 118, 120, 122, 121, 126, 126, 129, 129, 129, 124, 125, 128, 133, 131, 133, 134, 133, 131, 125, 125, 128, 126, 130, 133, 133, 138, 137, 137, 134, 133, 128, 129, 133, 135, 130, 129, 129, 134, 139, 143, 138, 130, 128, 128, 131, 125, 120, 107},
                {108, 133, 134, 141, 142, 139, 141, 144, 146, 152, 152, 150, 141, 135, 133, 129, 134, 129, 129, 130, 126, 129, 126, 135, 135, 131, 129, 130, 134, 137, 141, 144, 147, 151, 143, 137, 137, 133, 129, 130, 133, 138, 148, 144, 141, 137, 135, 131, 137, 139, 138, 133, 134, 134, 141, 142, 141, 138, 135, 133, 129, 129, 124, 120, 108},
                {113, 133, 138, 139, 138, 142, 139, 144, 154, 154, 151, 152, 151, 146, 144, 141, 138, 135, 135, 134, 133, 134, 135, 141, 137, 138, 138, 141, 141, 141, 147, 143, 148, 152, 155, 151, 146, 139, 138, 137, 141, 148, 155, 150, 146, 141, 138, 138, 143, 141, 141, 138, 141, 138, 141, 143, 138, 137, 137, 135, 130, 129, 128, 122, 111},
                {121, 134, 139, 141, 143, 148, 150, 155, 158, 154, 150, 151, 154, 154, 155, 148, 142, 142, 142, 138, 134, 138, 141, 139, 141, 147, 147, 146, 147, 150, 148, 143, 146, 146, 150, 150, 147, 143, 141, 144, 150, 151, 154, 151, 146, 141, 138, 137, 141, 139, 139, 141, 144, 139, 142, 146, 141, 142, 144, 138, 131, 129, 130, 128, 118},
                {126, 139, 147, 154, 158, 156, 155, 159, 156, 152, 151, 154, 152, 150, 148, 150, 150, 151, 143, 142, 138, 139, 139, 143, 150, 155, 154, 150, 142, 122, 103, 84, 79, 83, 95, 111, 126, 137, 141, 143, 147, 150, 148, 150, 147, 139, 138, 135, 141, 143, 139, 141, 144, 144, 143, 150, 143, 146, 151, 142, 134, 128, 130, 126, 124},
                {128, 141, 152, 160, 168, 158, 151, 155, 158, 155, 155, 154, 150, 151, 154, 158, 158, 155, 150, 143, 143, 142, 143, 148, 158, 154, 143, 105, 65, 54, 37, 28, 27, 32, 36, 44, 56, 77, 112, 137, 148, 152, 155, 152, 150, 141, 141, 138, 139, 144, 144, 146, 148, 146, 148, 152, 148, 151, 155, 148, 139, 131, 131, 130, 122},
                {126, 147, 151, 158, 160, 151, 152, 160, 163, 159, 159, 156, 155, 156, 160, 164, 161, 156, 156, 154, 155, 150, 152, 158, 155, 133, 75, 37, 45, 67, 37, 24, 19, 18, 19, 23, 36, 43, 40, 82, 138, 152, 158, 155, 148, 143, 144, 144, 144, 151, 152, 156, 156, 148, 150, 152, 151, 152, 155, 148, 143, 138, 137, 133, 125},
                {125, 144, 151, 155, 159, 160, 161, 169, 172, 164, 163, 163, 163, 164, 167, 164, 159, 152, 155, 159, 158, 158, 163, 164, 138, 75, 45, 52, 67, 73, 45, 37, 27, 23, 22, 16, 14, 23, 27, 23, 71, 141, 158, 161, 160, 155, 148, 146, 148, 158, 159, 163, 164, 156, 155, 155, 152, 151, 151, 150, 144, 146, 139, 135, 122},
                {121, 150, 168, 169, 169, 168, 172, 171, 173, 172, 172, 164, 167, 169, 169, 169, 164, 156, 156, 161, 161, 165, 169, 161, 87, 41, 56, 78, 103, 96, 70, 54, 35, 26, 26, 19, 15, 14, 15, 19, 22, 83, 152, 167, 165, 158, 148, 151, 151, 156, 161, 161, 161, 161, 165, 163, 158, 156, 151, 150, 143, 147, 139, 135, 128},
                {118, 151, 173, 180, 177, 173, 182, 175, 173, 172, 167, 165, 167, 173, 176, 172, 165, 161, 159, 164, 168, 177, 175, 131, 56, 83, 128, 159, 172, 167, 146, 124, 97, 58, 36, 27, 18, 16, 15, 16, 15, 26, 112, 163, 167, 169, 164, 161, 158, 161, 164, 164, 165, 167, 172, 164, 160, 159, 151, 151, 147, 144, 139, 137, 130},
                {118, 147, 165, 177, 176, 176, 181, 177, 175, 175, 172, 171, 175, 178, 176, 173, 167, 165, 164, 171, 181, 182, 159, 80, 82, 143, 186, 205, 207, 205, 190, 169, 142, 101, 70, 44, 23, 18, 14, 15, 18, 16, 48, 146, 173, 175, 171, 171, 167, 168, 169, 171, 172, 167, 167, 165, 161, 160, 156, 154, 148, 148, 142, 139, 129},
                {133, 154, 167, 168, 168, 180, 182, 188, 186, 185, 180, 180, 176, 181, 178, 175, 176, 175, 169, 177, 185, 177, 112, 50, 118, 186, 214, 216, 218, 215, 208, 192, 167, 129, 88, 57, 35, 22, 14, 14, 18, 18, 22, 105, 177, 173, 172, 169, 175, 175, 175, 176, 173, 168, 171, 176, 173, 171, 167, 158, 152, 150, 142, 143, 138},
                {142, 164, 180, 173, 173, 181, 190, 199, 197, 194, 188, 186, 185, 184, 181, 180, 186, 180, 173, 188, 198, 167, 65, 50, 135, 193, 214, 212, 214, 214, 208, 195, 173, 137, 96, 67, 44, 26, 23, 15, 13, 14, 14, 54, 161, 168, 164, 171, 172, 172, 177, 178, 175, 168, 172, 177, 181, 178, 171, 163, 158, 150, 143, 148, 144},
                {139, 164, 182, 178, 168, 171, 193, 199, 201, 202, 199, 201, 194, 185, 178, 181, 185, 176, 171, 188, 189, 130, 53, 60, 141, 189, 203, 202, 203, 206, 201, 186, 159, 135, 111, 86, 61, 37, 22, 15, 14, 10, 10, 27, 125, 143, 152, 159, 151, 158, 168, 164, 168, 171, 171, 172, 176, 180, 173, 168, 163, 152, 147, 152, 144},
                {118, 144, 163, 161, 144, 150, 182, 194, 201, 197, 185, 197, 197, 192, 181, 182, 189, 177, 181, 192, 158, 87, 39, 78, 148, 188, 199, 203, 203, 202, 201, 186, 163, 147, 133, 114, 94, 60, 26, 19, 15, 11, 10, 14, 78, 113, 130, 131, 131, 150, 158, 155, 154, 165, 168, 175, 173, 175, 172, 165, 163, 155, 144, 147, 143},
                {94, 114, 117, 131, 122, 126, 161, 177, 178, 161, 156, 169, 193, 195, 194, 192, 195, 192, 198, 192, 131, 65, 32, 92, 160, 186, 194, 203, 206, 203, 197, 182, 172, 163, 147, 133, 108, 65, 28, 16, 11, 13, 10, 10, 58, 94, 101, 101, 112, 139, 141, 143, 137, 163, 173, 178, 175, 171, 171, 168, 163, 151, 134, 139, 137},
                {79, 94, 87, 105, 117, 114, 128, 141, 141, 139, 130, 133, 150, 163, 190, 199, 198, 198, 203, 184, 111, 53, 27, 90, 156, 165, 167, 190, 199, 188, 146, 129, 134, 138, 129, 112, 91, 56, 22, 13, 9, 11, 13, 10, 45, 75, 82, 99, 111, 114, 118, 126, 117, 148, 168, 175, 171, 172, 180, 177, 155, 138, 121, 121, 101},
                {49, 73, 77, 95, 111, 112, 121, 116, 109, 113, 105, 108, 108, 120, 139, 185, 193, 198, 195, 158, 90, 56, 30, 65, 101, 104, 100, 144, 178, 133, 73, 86, 99, 82, 66, 61, 65, 53, 23, 15, 10, 9, 13, 13, 33, 60, 70, 80, 99, 101, 108, 113, 105, 126, 156, 173, 176, 175, 186, 181, 159, 143, 130, 112, 96},
                {40, 69, 82, 94, 100, 104, 109, 100, 96, 97, 87, 87, 99, 118, 114, 176, 193, 186, 181, 141, 75, 52, 32, 50, 65, 79, 56, 113, 172, 100, 75, 118, 78, 50, 49, 61, 91, 70, 33, 22, 15, 7, 11, 11, 26, 60, 61, 64, 82, 91, 97, 80, 82, 111, 147, 175, 176, 169, 175, 163, 147, 135, 124, 112, 116},
                {41, 71, 90, 99, 97, 107, 97, 95, 97, 95, 74, 82, 92, 104, 111, 163, 148, 114, 133, 131, 67, 47, 26, 90, 125, 131, 104, 143, 180, 105, 109, 154, 133, 105, 112, 143, 131, 82, 32, 16, 14, 10, 11, 13, 22, 53, 57, 61, 71, 78, 77, 73, 77, 94, 141, 176, 177, 172, 168, 155, 141, 131, 111, 109, 111},
                {44, 71, 84, 97, 94, 99, 94, 87, 88, 83, 71, 77, 77, 80, 80, 108, 97, 86, 79, 99, 65, 39, 22, 117, 173, 169, 164, 175, 178, 125, 137, 173, 173, 172, 172, 160, 126, 73, 24, 10, 10, 9, 10, 15, 16, 43, 52, 62, 77, 82, 74, 69, 73, 83, 122, 165, 167, 164, 154, 147, 143, 133, 112, 108, 100},
                {50, 64, 75, 78, 84, 88, 78, 75, 78, 67, 71, 71, 60, 65, 57, 65, 71, 75, 54, 69, 60, 33, 19, 105, 173, 192, 192, 185, 184, 137, 150, 190, 199, 197, 181, 143, 105, 58, 20, 10, 10, 10, 10, 14, 15, 45, 64, 66, 74, 74, 69, 62, 62, 67, 99, 143, 154, 154, 147, 143, 143, 129, 118, 112, 87},
                {50, 43, 57, 64, 64, 69, 66, 70, 82, 70, 69, 62, 53, 53, 53, 56, 58, 57, 44, 48, 45, 40, 26, 86, 176, 192, 186, 186, 184, 134, 141, 178, 189, 185, 163, 120, 84, 45, 18, 13, 11, 11, 9, 13, 18, 47, 71, 86, 99, 97, 96, 96, 96, 100, 105, 122, 134, 134, 129, 121, 113, 107, 107, 99, 75},
                {39, 33, 47, 61, 58, 61, 64, 69, 82, 79, 74, 66, 56, 54, 58, 53, 49, 41, 31, 30, 33, 39, 23, 66, 156, 184, 173, 178, 172, 121, 126, 175, 181, 167, 137, 100, 70, 37, 16, 15, 13, 11, 7, 10, 16, 28, 39, 49, 65, 73, 77, 79, 83, 96, 105, 116, 120, 114, 99, 91, 73, 75, 92, 73, 49},
                {31, 31, 40, 58, 61, 56, 62, 60, 61, 67, 75, 71, 64, 60, 58, 53, 50, 40, 28, 24, 27, 32, 19, 43, 121, 159, 168, 146, 103, 58, 86, 171, 171, 144, 114, 84, 58, 35, 19, 13, 10, 9, 7, 11, 15, 19, 27, 35, 40, 45, 52, 50, 50, 54, 67, 80, 77, 75, 69, 71, 53, 45, 44, 35, 28},
                {24, 28, 35, 52, 60, 57, 61, 57, 58, 54, 66, 67, 57, 56, 57, 53, 48, 37, 33, 28, 24, 23, 19, 31, 97, 131, 171, 168, 82, 58, 97, 137, 135, 128, 104, 80, 60, 33, 19, 13, 10, 7, 11, 15, 16, 22, 30, 35, 41, 45, 47, 41, 43, 47, 49, 49, 50, 56, 60, 62, 52, 41, 40, 30, 27},
                {24, 31, 35, 52, 61, 57, 54, 54, 60, 54, 60, 57, 53, 53, 53, 49, 43, 36, 33, 31, 23, 30, 28, 23, 87, 126, 126, 142, 120, 86, 83, 90, 122, 126, 105, 80, 57, 32, 16, 13, 10, 7, 9, 11, 16, 26, 36, 40, 47, 48, 49, 43, 40, 40, 41, 45, 56, 65, 70, 61, 50, 36, 32, 28, 26},
                {27, 30, 41, 57, 60, 54, 53, 57, 62, 57, 52, 48, 47, 48, 43, 43, 41, 39, 40, 37, 31, 36, 24, 16, 58, 125, 154, 159, 131, 101, 88, 113, 129, 122, 97, 73, 49, 28, 18, 14, 13, 10, 7, 9, 15, 22, 37, 43, 47, 47, 47, 43, 41, 43, 47, 53, 64, 67, 62, 54, 40, 35, 33, 28, 22},
                {24, 28, 43, 49, 49, 48, 50, 52, 49, 47, 44, 45, 44, 44, 47, 47, 49, 50, 50, 44, 37, 31, 22, 18, 23, 75, 135, 160, 134, 113, 108, 108, 112, 100, 77, 60, 44, 28, 16, 15, 13, 9, 7, 9, 13, 24, 43, 44, 43, 47, 45, 44, 41, 41, 45, 48, 52, 47, 40, 43, 44, 45, 37, 28, 27},
                {23, 28, 33, 39, 45, 49, 53, 49, 43, 44, 45, 45, 48, 54, 57, 58, 60, 60, 62, 54, 45, 28, 24, 24, 16, 20, 78, 161, 193, 172, 129, 108, 94, 73, 56, 49, 36, 23, 18, 16, 11, 7, 7, 7, 13, 23, 45, 48, 44, 50, 54, 54, 44, 41, 47, 48, 48, 44, 44, 52, 50, 50, 39, 24, 23},
                {20, 28, 27, 28, 27, 41, 53, 50, 43, 44, 40, 43, 43, 49, 48, 52, 54, 57, 61, 57, 41, 33, 33, 33, 22, 10, 20, 91, 150, 138, 107, 78, 58, 49, 44, 37, 26, 19, 18, 16, 14, 11, 10, 9, 14, 22, 36, 49, 52, 58, 61, 54, 45, 49, 49, 54, 53, 47, 47, 52, 45, 41, 30, 22, 23},
                {22, 28, 36, 36, 28, 36, 50, 52, 49, 43, 36, 40, 43, 52, 52, 60, 67, 80, 87, 66, 37, 36, 26, 23, 15, 9, 7, 19, 52, 60, 52, 44, 39, 33, 28, 23, 27, 24, 19, 19, 16, 15, 11, 9, 13, 16, 27, 40, 40, 43, 48, 47, 43, 47, 45, 45, 45, 37, 37, 44, 43, 50, 43, 30, 22},
                {22, 27, 32, 22, 32, 33, 48, 54, 54, 54, 54, 58, 64, 71, 74, 84, 88, 95, 94, 75, 39, 27, 22, 24, 18, 9, 6, 6, 10, 35, 31, 26, 22, 20, 26, 33, 37, 37, 35, 23, 16, 11, 10, 10, 14, 13, 20, 33, 36, 41, 48, 48, 41, 39, 39, 40, 32, 28, 30, 35, 43, 40, 35, 35, 22},
                {22, 23, 22, 16, 33, 30, 40, 54, 58, 61, 66, 73, 75, 84, 88, 99, 103, 100, 91, 77, 45, 26, 26, 24, 20, 11, 10, 7, 13, 69, 74, 54, 37, 36, 44, 52, 60, 61, 49, 28, 19, 11, 10, 13, 22, 16, 26, 39, 43, 44, 52, 54, 47, 43, 41, 40, 31, 31, 39, 40, 40, 35, 24, 31, 27},
                {23, 24, 27, 22, 23, 33, 48, 53, 71, 78, 80, 84, 86, 92, 90, 94, 99, 95, 90, 73, 50, 28, 27, 24, 20, 13, 11, 9, 11, 80, 116, 94, 71, 62, 64, 70, 82, 83, 65, 39, 23, 15, 11, 13, 22, 18, 23, 37, 47, 61, 73, 62, 65, 60, 60, 61, 60, 62, 71, 77, 66, 44, 32, 37, 31},
                {26, 32, 39, 44, 48, 58, 61, 67, 77, 69, 70, 70, 64, 78, 84, 83, 87, 86, 79, 58, 43, 26, 24, 20, 16, 14, 14, 11, 14, 70, 128, 122, 105, 91, 91, 95, 105, 104, 86, 50, 31, 19, 15, 19, 27, 15, 14, 27, 36, 49, 54, 58, 70, 77, 79, 71, 86, 94, 91, 86, 66, 58, 39, 28, 30},
                {27, 44, 54, 62, 67, 78, 80, 84, 82, 67, 52, 50, 52, 40, 53, 62, 53, 57, 43, 37, 23, 15, 15, 14, 14, 14, 14, 19, 44, 91, 126, 135, 126, 118, 117, 121, 125, 122, 101, 64, 33, 27, 26, 27, 33, 13, 10, 11, 11, 19, 28, 27, 49, 50, 32, 60, 101, 107, 90, 74, 67, 62, 32, 16, 14},
                {28, 49, 60, 62, 64, 77, 71, 74, 77, 61, 52, 54, 64, 32, 22, 26, 24, 28, 18, 18, 15, 14, 15, 14, 13, 16, 31, 67, 113, 142, 148, 150, 141, 139, 142, 146, 143, 141, 121, 79, 41, 35, 41, 35, 33, 13, 10, 10, 13, 18, 24, 26, 30, 28, 26, 45, 91, 103, 79, 69, 69, 64, 32, 11, 15},
                {23, 40, 58, 54, 44, 56, 66, 73, 64, 48, 54, 57, 54, 30, 18, 15, 14, 24, 23, 11, 10, 15, 16, 18, 19, 50, 96, 126, 159, 176, 175, 169, 161, 163, 169, 171, 164, 154, 137, 94, 53, 40, 50, 43, 32, 15, 14, 13, 18, 30, 40, 53, 73, 82, 87, 95, 109, 97, 65, 62, 61, 53, 27, 15, 22},
                {27, 36, 43, 41, 35, 57, 69, 66, 60, 56, 66, 71, 54, 22, 23, 27, 9, 26, 22, 14, 14, 16, 16, 19, 48, 114, 152, 175, 188, 190, 192, 186, 182, 186, 190, 185, 175, 159, 133, 86, 49, 49, 62, 45, 27, 16, 18, 15, 15, 26, 41, 56, 90, 124, 121, 104, 94, 83, 75, 69, 58, 47, 37, 31, 23},
                {28, 44, 40, 37, 36, 64, 69, 67, 67, 75, 84, 71, 50, 24, 32, 35, 11, 23, 26, 18, 15, 19, 19, 37, 87, 158, 182, 195, 203, 203, 206, 199, 192, 194, 197, 190, 178, 160, 138, 91, 62, 53, 58, 39, 19, 15, 20, 19, 22, 20, 27, 32, 60, 104, 109, 108, 96, 94, 86, 77, 71, 79, 64, 41, 22},
                {26, 43, 47, 58, 49, 69, 78, 79, 75, 82, 77, 61, 49, 39, 37, 44, 26, 33, 32, 15, 14, 22, 36, 64, 125, 182, 195, 205, 214, 211, 205, 199, 197, 201, 203, 194, 184, 165, 137, 96, 67, 54, 56, 33, 14, 15, 22, 33, 31, 24, 22, 28, 49, 73, 86, 95, 96, 97, 86, 87, 90, 86, 60, 44, 24},
                {27, 47, 47, 64, 48, 61, 99, 97, 94, 95, 82, 70, 53, 48, 50, 52, 40, 33, 19, 10, 13, 37, 67, 91, 151, 194, 207, 211, 212, 210, 205, 202, 201, 207, 205, 194, 184, 167, 134, 83, 61, 53, 53, 28, 13, 19, 41, 62, 53, 44, 26, 19, 37, 52, 73, 88, 94, 96, 83, 96, 91, 87, 82, 67, 26},
                {32, 37, 40, 53, 37, 47, 74, 97, 114, 107, 94, 88, 64, 54, 61, 57, 32, 15, 11, 13, 18, 66, 103, 120, 175, 201, 208, 215, 215, 212, 207, 206, 203, 208, 205, 192, 178, 165, 133, 92, 74, 56, 50, 32, 35, 53, 69, 74, 65, 52, 33, 15, 16, 31, 49, 62, 78, 96, 101, 107, 117, 122, 121, 96, 50},
                {31, 44, 47, 45, 36, 47, 52, 47, 73, 103, 99, 87, 77, 61, 60, 43, 16, 13, 15, 13, 30, 94, 133, 163, 194, 206, 208, 212, 212, 215, 216, 214, 208, 208, 205, 198, 186, 167, 134, 92, 70, 52, 58, 71, 84, 92, 96, 95, 83, 65, 45, 20, 15, 10, 16, 27, 32, 50, 64, 60, 69, 79, 84, 71, 61},
                {37, 52, 60, 67, 60, 56, 52, 40, 48, 66, 91, 86, 90, 86, 57, 19, 11, 15, 16, 14, 48, 118, 163, 189, 202, 210, 210, 211, 212, 215, 222, 219, 215, 212, 211, 205, 186, 163, 128, 95, 78, 70, 78, 90, 108, 114, 99, 83, 62, 45, 28, 14, 10, 9, 7, 11, 24, 26, 24, 27, 40, 60, 75, 80, 66},
                {32, 50, 60, 66, 57, 57, 62, 56, 53, 64, 73, 67, 87, 101, 41, 11, 13, 16, 15, 18, 50, 116, 185, 205, 210, 207, 205, 205, 210, 220, 224, 227, 222, 216, 215, 210, 192, 164, 131, 104, 83, 84, 103, 100, 80, 57, 43, 33, 23, 18, 13, 10, 6, 7, 9, 6, 18, 26, 22, 19, 23, 52, 86, 109, 80},
                {35, 49, 49, 52, 57, 67, 77, 67, 71, 82, 87, 95, 96, 69, 20, 13, 14, 20, 16, 27, 53, 83, 152, 192, 207, 206, 194, 198, 211, 223, 228, 228, 225, 222, 219, 211, 194, 159, 125, 91, 80, 79, 60, 39, 31, 30, 23, 15, 11, 9, 6, 6, 6, 6, 6, 6, 7, 22, 23, 23, 22, 27, 41, 61, 48},
                {31, 50, 57, 73, 80, 92, 96, 83, 91, 103, 101, 88, 53, 23, 15, 13, 18, 24, 23, 37, 60, 75, 92, 111, 150, 169, 168, 197, 216, 219, 222, 222, 220, 218, 208, 198, 163, 116, 88, 67, 47, 33, 27, 24, 18, 13, 11, 10, 6, 6, 5, 5, 6, 6, 5, 7, 7, 18, 36, 36, 32, 27, 32, 28, 20},
                {26, 52, 69, 83, 95, 87, 79, 79, 83, 77, 70, 50, 19, 18, 15, 13, 22, 20, 35, 50, 64, 64, 71, 77, 84, 86, 103, 137, 168, 185, 194, 202, 206, 201, 185, 148, 100, 75, 66, 40, 28, 24, 20, 14, 10, 15, 15, 16, 10, 6, 5, 5, 6, 6, 6, 7, 9, 10, 35, 45, 50, 37, 31, 31, 14},
                {23, 52, 71, 78, 90, 86, 75, 80, 86, 79, 65, 30, 16, 19, 15, 14, 18, 20, 44, 60, 61, 56, 52, 64, 65, 73, 78, 83, 88, 97, 105, 112, 121, 124, 108, 84, 67, 53, 48, 28, 19, 13, 13, 23, 31, 35, 24, 20, 13, 5, 5, 5, 6, 6, 5, 7, 7, 7, 20, 35, 44, 32, 27, 35, 19},
                {16, 39, 67, 74, 86, 94, 94, 95, 101, 103, 90, 30, 19, 16, 14, 13, 15, 23, 47, 49, 53, 41, 47, 53, 52, 69, 73, 70, 75, 78, 77, 75, 74, 78, 73, 64, 43, 41, 24, 11, 10, 10, 15, 32, 49, 32, 26, 18, 10, 6, 6, 6, 6, 5, 6, 5, 5, 6, 10, 18, 24, 22, 19, 31, 22},
                {14, 22, 45, 58, 70, 82, 88, 87, 84, 80, 67, 24, 16, 15, 14, 13, 14, 20, 44, 39, 48, 32, 39, 33, 50, 65, 69, 66, 69, 66, 61, 57, 56, 61, 60, 43, 31, 19, 13, 13, 14, 19, 24, 35, 45, 28, 30, 22, 26, 27, 22, 22, 11, 7, 9, 5, 5, 6, 10, 28, 30, 27, 24, 18, 7},
                {13, 18, 36, 53, 60, 67, 70, 64, 64, 65, 49, 20, 13, 13, 11, 11, 13, 23, 36, 28, 30, 31, 32, 28, 44, 58, 64, 64, 60, 50, 45, 45, 52, 50, 37, 20, 13, 11, 11, 15, 19, 26, 27, 31, 28, 32, 39, 58, 60, 43, 39, 31, 22, 13, 10, 9, 7, 9, 11, 28, 48, 43, 32, 13, 5},
                {15, 19, 37, 54, 60, 61, 58, 60, 65, 58, 44, 18, 11, 9, 7, 9, 11, 22, 28, 27, 20, 31, 31, 26, 40, 43, 50, 49, 45, 35, 36, 47, 40, 32, 20, 10, 7, 7, 7, 13, 16, 23, 19, 19, 26, 36, 52, 64, 57, 41, 40, 33, 23, 10, 7, 10, 7, 7, 9, 15, 41, 48, 30, 9, 5},
                {18, 23, 57, 77, 86, 86, 80, 80, 82, 54, 31, 14, 11, 7, 7, 9, 13, 19, 22, 22, 16, 28, 23, 20, 32, 23, 31, 35, 33, 28, 39, 37, 33, 28, 15, 9, 6, 6, 7, 10, 14, 15, 10, 14, 32, 39, 56, 62, 57, 52, 44, 22, 11, 6, 7, 9, 10, 7, 7, 6, 11, 15, 15, 6, 5},
                {19, 37, 79, 92, 97, 96, 94, 99, 96, 69, 33, 18, 11, 7, 9, 10, 13, 14, 15, 16, 16, 24, 18, 16, 19, 19, 20, 22, 22, 35, 36, 36, 27, 11, 9, 7, 6, 7, 9, 9, 14, 14, 10, 20, 37, 44, 60, 61, 58, 41, 20, 6, 6, 5, 6, 6, 13, 11, 9, 7, 20, 32, 30, 10, 5},
                {20, 61, 88, 96, 101, 103, 105, 105, 99, 50, 19, 15, 10, 9, 10, 9, 11, 13, 13, 15, 16, 22, 13, 15, 16, 20, 15, 13, 22, 35, 37, 28, 15, 11, 9, 5, 6, 9, 7, 7, 10, 10, 13, 30, 39, 43, 60, 52, 43, 19, 6, 5, 5, 6, 6, 6, 15, 18, 10, 9, 19, 40, 43, 27, 7},
                {39, 70, 79, 87, 90, 91, 101, 94, 78, 28, 18, 14, 13, 11, 11, 11, 9, 11, 13, 11, 13, 19, 11, 13, 14, 15, 10, 11, 24, 31, 27, 18, 16, 14, 7, 5, 6, 7, 6, 7, 10, 11, 20, 35, 40, 40, 49, 36, 14, 9, 6, 5, 5, 6, 5, 5, 13, 18, 11, 7, 14, 41, 44, 43, 23},
                {28, 41, 48, 58, 62, 65, 67, 58, 36, 18, 15, 15, 19, 22, 16, 11, 7, 9, 11, 9, 10, 16, 11, 10, 10, 13, 10, 16, 20, 23, 18, 18, 15, 11, 5, 3, 5, 6, 6, 7, 7, 11, 22, 37, 31, 27, 23, 14, 7, 10, 9, 6, 6, 6, 5, 5, 9, 13, 10, 6, 6, 19, 23, 20, 16},
                {20, 28, 39, 49, 56, 50, 41, 30, 19, 15, 11, 15, 22, 28, 23, 16, 11, 9, 9, 9, 9, 14, 10, 9, 9, 9, 10, 16, 19, 20, 18, 16, 14, 7, 6, 5, 5, 6, 6, 6, 6, 9, 20, 22, 18, 14, 10, 7, 6, 13, 9, 6, 6, 5, 5, 5, 6, 7, 6, 5, 3, 6, 9, 9, 10},
                {24, 36, 44, 50, 54, 44, 30, 19, 18, 15, 14, 20, 28, 32, 31, 20, 15, 13, 13, 10, 9, 13, 11, 10, 9, 7, 13, 15, 20, 18, 19, 14, 9, 9, 7, 6, 6, 5, 5, 5, 6, 9, 19, 18, 11, 7, 6, 6, 9, 11, 9, 6, 6, 6, 5, 3, 6, 6, 6, 6, 7, 13, 13, 11, 9},
                {24, 36, 41, 47, 40, 22, 16, 18, 19, 16, 16, 26, 35, 36, 37, 28, 18, 15, 14, 10, 10, 14, 13, 13, 9, 6, 11, 15, 20, 19, 18, 7, 7, 11, 11, 9, 7, 5, 5, 5, 6, 6, 18, 13, 5, 5, 6, 7, 13, 13, 7, 6, 5, 5, 3, 5, 6, 7, 6, 7, 13, 15, 20, 18, 11},
                {22, 28, 37, 30, 16, 14, 14, 19, 19, 24, 26, 33, 41, 44, 39, 26, 19, 16, 14, 11, 13, 14, 13, 13, 9, 9, 11, 20, 23, 14, 9, 6, 7, 10, 11, 9, 6, 5, 5, 5, 6, 6, 9, 5, 3, 3, 6, 10, 11, 9, 6, 5, 3, 3, 3, 5, 6, 6, 6, 6, 13, 14, 18, 15, 7},
                {22, 28, 32, 16, 14, 13, 19, 22, 30, 35, 30, 24, 28, 30, 32, 24, 26, 19, 14, 11, 13, 13, 9, 9, 9, 10, 16, 26, 16, 7, 6, 6, 6, 7, 7, 6, 5, 5, 5, 5, 6, 7, 5, 3, 3, 3, 7, 13, 14, 11, 9, 7, 3, 5, 3, 3, 6, 7, 6, 7, 13, 15, 19, 18, 7},
                {19, 27, 19, 16, 14, 18, 15, 15, 23, 23, 16, 16, 22, 23, 18, 14, 18, 18, 11, 9, 10, 10, 7, 7, 9, 14, 20, 15, 9, 6, 6, 6, 5, 6, 6, 6, 5, 6, 5, 6, 6, 7, 5, 3, 3, 5, 9, 13, 16, 15, 18, 15, 7, 3, 3, 3, 6, 7, 9, 11, 14, 14, 18, 20, 7},
                {14, 18, 14, 16, 22, 20, 11, 14, 16, 19, 16, 20, 24, 19, 14, 10, 18, 15, 10, 7, 9, 9, 6, 6, 10, 13, 11, 9, 7, 7, 6, 6, 5, 5, 5, 5, 5, 5, 5, 5, 6, 5, 5, 3, 5, 5, 7, 10, 13, 14, 22, 18, 11, 5, 3, 5, 3, 7, 11, 14, 14, 11, 14, 20, 9},
                {9, 10, 9, 14, 16, 13, 18, 24, 44, 52, 49, 44, 43, 40, 33, 14, 13, 15, 10, 10, 9, 11, 7, 6, 7, 7, 9, 7, 7, 6, 6, 6, 6, 6, 5, 5, 5, 6, 5, 5, 5, 5, 5, 5, 5, 6, 9, 14, 11, 13, 14, 13, 7, 3, 3, 5, 3, 6, 10, 13, 11, 7, 9, 14, 10},
                {9, 10, 13, 14, 15, 14, 31, 41, 56, 67, 37, 41, 48, 52, 40, 19, 13, 11, 10, 13, 14, 16, 10, 10, 6, 6, 9, 7, 6, 6, 7, 6, 6, 6, 5, 6, 7, 7, 7, 6, 5, 3, 5, 6, 6, 7, 11, 13, 10, 9, 7, 7, 7, 5, 3, 3, 2, 5, 10, 9, 5, 3, 2, 5, 3},
                {11, 9, 11, 16, 16, 15, 33, 43, 52, 53, 74, 75, 56, 61, 50, 52, 39, 16, 22, 35, 37, 32, 16, 13, 9, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 9, 10, 7, 6, 5, 5, 9, 10, 9, 9, 7, 11, 13, 15, 11, 9, 9, 6, 3, 3, 2, 5, 10, 9, 5, 2, 2, 1, 0},
                {11, 9, 13, 16, 14, 19, 33, 52, 74, 87, 82, 50, 69, 65, 67, 77, 54, 84, 137, 165, 158, 133, 92, 52, 23, 10, 6, 6, 6, 7, 6, 6, 6, 7, 9, 9, 7, 7, 6, 6, 6, 6, 7, 6, 6, 7, 7, 10, 15, 18, 13, 6, 6, 3, 3, 2, 2, 5, 7, 10, 10, 3, 2, 1, 0},
                {9, 9, 13, 15, 14, 19, 30, 60, 77, 62, 56, 80, 56, 61, 64, 58, 96, 158, 194, 202, 192, 177, 156, 144, 113, 71, 39, 18, 9, 7, 6, 6, 6, 7, 7, 7, 5, 5, 5, 6, 6, 6, 6, 5, 6, 7, 9, 13, 16, 16, 10, 6, 5, 3, 2, 2, 2, 6, 6, 7, 9, 3, 1, 1, 0},
                {10, 10, 13, 15, 14, 15, 26, 54, 65, 50, 69, 56, 44, 50, 41, 56, 111, 171, 186, 194, 186, 184, 173, 167, 152, 143, 142, 114, 52, 10, 6, 6, 6, 7, 7, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 9, 10, 11, 11, 7, 6, 3, 3, 3, 3, 3, 5, 5, 3, 2, 2, 2, 1, 0},
                {13, 11, 11, 14, 13, 14, 23, 41, 37, 57, 56, 33, 37, 47, 39, 61, 107, 154, 165, 169, 167, 175, 176, 173, 165, 158, 158, 150, 120, 71, 37, 11, 6, 6, 6, 6, 5, 5, 5, 5, 6, 5, 6, 5, 5, 5, 6, 7, 10, 11, 7, 6, 5, 6, 5, 3, 3, 3, 3, 2, 2, 3, 2, 1, 0},
                {15, 15, 15, 14, 13, 14, 18, 26, 26, 49, 36, 35, 40, 48, 39, 50, 86, 126, 135, 139, 142, 158, 168, 169, 171, 165, 161, 158, 151, 148, 128, 86, 36, 11, 7, 6, 5, 6, 5, 6, 6, 5, 6, 5, 5, 5, 5, 5, 7, 11, 7, 6, 5, 5, 3, 5, 3, 3, 3, 3, 2, 3, 2, 1, 0},
                {15, 16, 18, 15, 16, 15, 15, 23, 24, 28, 23, 28, 27, 32, 28, 44, 54, 90, 96, 101, 116, 135, 150, 159, 165, 164, 164, 168, 165, 165, 164, 151, 104, 53, 19, 6, 6, 10, 11, 9, 10, 19, 37, 41, 30, 23, 20, 14, 7, 7, 7, 7, 6, 5, 6, 6, 3, 5, 3, 3, 2, 2, 2, 1, 0},
                {14, 15, 15, 15, 18, 18, 18, 18, 19, 19, 14, 15, 18, 23, 19, 30, 32, 49, 57, 71, 100, 121, 134, 144, 152, 150, 148, 159, 151, 126, 146, 175, 155, 84, 56, 45, 50, 77, 71, 56, 44, 45, 69, 73, 57, 53, 52, 41, 22, 7, 5, 9, 6, 6, 6, 6, 5, 5, 3, 3, 3, 2, 2, 1, 1},
                {13, 14, 13, 11, 13, 15, 14, 13, 14, 14, 10, 10, 19, 20, 18, 18, 26, 30, 44, 73, 95, 114, 124, 131, 138, 135, 134, 146, 143, 100, 70, 113, 143, 126, 87, 75, 77, 90, 90, 67, 70, 62, 58, 69, 67, 48, 40, 33, 31, 14, 6, 5, 5, 5, 6, 6, 5, 3, 3, 3, 3, 3, 2, 1, 1},
                {10, 11, 10, 9, 9, 11, 11, 11, 10, 10, 10, 11, 14, 15, 16, 26, 54, 88, 117, 125, 95, 97, 107, 114, 118, 122, 125, 118, 133, 142, 84, 41, 66, 97, 90, 75, 73, 61, 57, 58, 56, 60, 48, 54, 67, 62, 37, 32, 31, 20, 9, 5, 5, 3, 5, 5, 3, 3, 3, 3, 3, 3, 2, 1, 1},
                {11, 13, 10, 9, 9, 10, 11, 11, 11, 10, 13, 14, 11, 13, 36, 90, 105, 105, 139, 146, 112, 83, 86, 97, 103, 116, 126, 105, 92, 129, 139, 91, 44, 48, 69, 80, 73, 65, 47, 52, 56, 57, 57, 40, 33, 44, 40, 33, 32, 27, 14, 5, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 3, 3},
                {10, 10, 10, 9, 7, 9, 10, 11, 13, 13, 11, 14, 11, 15, 71, 91, 84, 111, 137, 144, 137, 97, 67, 75, 87, 95, 117, 137, 99, 77, 109, 121, 83, 47, 40, 56, 70, 53, 45, 37, 45, 49, 62, 48, 31, 26, 32, 24, 24, 26, 19, 5, 3, 3, 3, 3, 3, 2, 3, 3, 3, 3, 3, 3, 6},
                {9, 9, 10, 9, 10, 11, 11, 11, 14, 13, 11, 13, 14, 19, 73, 74, 96, 118, 118, 150, 141, 116, 79, 61, 75, 78, 74, 109, 138, 96, 61, 86, 99, 73, 39, 32, 33, 35, 31, 32, 32, 23, 45, 44, 24, 27, 26, 20, 14, 14, 9, 3, 3, 2, 3, 2, 3, 3, 3, 5, 3, 5, 3, 3, 7},
                {10, 10, 11, 13, 13, 13, 11, 14, 15, 15, 14, 15, 15, 24, 73, 84, 104, 88, 111, 137, 138, 113, 87, 58, 53, 74, 64, 60, 101, 120, 70, 47, 75, 90, 69, 33, 33, 30, 19, 26, 24, 15, 24, 24, 19, 18, 24, 18, 11, 10, 7, 3, 3, 2, 2, 3, 3, 2, 3, 5, 5, 5, 5, 3, 6},
                {10, 10, 11, 13, 13, 13, 13, 14, 16, 15, 15, 15, 16, 30, 67, 82, 78, 80, 112, 129, 118, 88, 69, 53, 37, 39, 56, 49, 50, 71, 66, 48, 39, 61, 86, 54, 26, 23, 15, 19, 15, 11, 15, 14, 11, 13, 15, 14, 9, 6, 5, 3, 3, 3, 3, 3, 3, 3, 5, 3, 5, 5, 5, 2, 3},
                {10, 10, 11, 11, 11, 11, 11, 13, 13, 13, 14, 16, 18, 24, 48, 64, 69, 91, 114, 113, 79, 58, 41, 32, 24, 20, 26, 32, 33, 31, 35, 43, 37, 24, 41, 48, 15, 11, 10, 14, 10, 9, 10, 9, 7, 7, 9, 9, 6, 5, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 1, 1},
                {9, 10, 11, 13, 11, 11, 11, 10, 11, 13, 14, 15, 18, 20, 28, 47, 64, 94, 109, 73, 39, 31, 31, 32, 31, 35, 35, 28, 30, 33, 30, 24, 22, 15, 10, 11, 6, 7, 7, 7, 6, 6, 7, 6, 6, 6, 6, 5, 5, 3, 3, 3, 3, 3, 5, 5, 3, 3, 3, 5, 6, 3, 3, 1, 1},
                {10, 10, 11, 13, 14, 14, 13, 13, 14, 14, 15, 14, 15, 18, 19, 26, 47, 84, 73, 40, 22, 30, 30, 28, 23, 26, 23, 22, 26, 28, 26, 16, 13, 11, 10, 9, 9, 9, 6, 6, 6, 6, 6, 6, 5, 6, 5, 5, 5, 5, 5, 3, 3, 3, 5, 5, 3, 5, 5, 6, 5, 2, 2, 1, 1},
                {11, 13, 13, 14, 16, 15, 14, 13, 13, 15, 18, 16, 15, 15, 14, 16, 24, 61, 50, 26, 18, 26, 23, 19, 16, 15, 16, 18, 19, 16, 14, 11, 10, 10, 9, 9, 9, 9, 7, 7, 7, 6, 6, 6, 5, 5, 5, 3, 3, 5, 3, 3, 5, 3, 3, 5, 3, 7, 5, 6, 5, 3, 2, 1, 1},
                {11, 13, 14, 15, 16, 13, 11, 11, 11, 15, 18, 16, 15, 14, 14, 16, 20, 47, 40, 24, 20, 23, 19, 18, 18, 20, 23, 20, 18, 14, 13, 11, 11, 13, 11, 10, 9, 9, 7, 7, 6, 6, 6, 7, 9, 6, 5, 3, 3, 3, 3, 3, 3, 3, 5, 3, 3, 5, 5, 5, 2, 3, 2, 1, 1},
                {9, 10, 13, 14, 14, 14, 14, 14, 11, 15, 18, 16, 15, 14, 15, 18, 18, 27, 26, 18, 15, 15, 15, 15, 16, 18, 16, 14, 13, 11, 10, 9, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 9, 11, 9, 6, 5, 3, 3, 3, 2, 3, 3, 5, 3, 3, 7, 5, 2, 2, 5, 3, 1, 0},
                {9, 10, 11, 13, 13, 14, 14, 13, 14, 15, 15, 14, 16, 15, 16, 16, 16, 18, 15, 11, 10, 11, 11, 9, 10, 9, 9, 7, 7, 7, 6, 6, 6, 6, 7, 7, 6, 5, 5, 6, 6, 5, 5, 6, 9, 7, 6, 5, 5, 3, 2, 2, 2, 2, 3, 3, 2, 9, 6, 3, 3, 11, 6, 2, 0},
                {9, 9, 10, 13, 14, 14, 15, 13, 13, 15, 15, 15, 15, 15, 16, 18, 15, 16, 14, 11, 9, 9, 13, 10, 9, 7, 7, 6, 5, 6, 7, 6, 6, 9, 10, 11, 7, 6, 6, 5, 5, 5, 5, 6, 7, 6, 5, 5, 5, 3, 2, 3, 2, 2, 3, 3, 2, 5, 5, 2, 3, 10, 6, 2, 0},
                {9, 9, 11, 13, 13, 14, 14, 13, 11, 13, 14, 13, 14, 14, 14, 14, 10, 11, 11, 10, 9, 10, 14, 15, 15, 9, 9, 6, 6, 6, 9, 7, 6, 7, 7, 7, 6, 5, 5, 6, 6, 6, 6, 7, 7, 5, 5, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 1, 7, 5, 1, 0},
                {10, 10, 11, 11, 11, 11, 13, 13, 13, 14, 15, 18, 16, 15, 14, 13, 11, 10, 9, 9, 10, 11, 11, 14, 15, 10, 9, 9, 9, 9, 11, 9, 6, 6, 5, 6, 6, 6, 5, 6, 7, 6, 6, 6, 5, 3, 3, 3, 2, 3, 2, 2, 2, 2, 2, 2, 1, 2, 2, 3, 1, 3, 3, 0, 0},
                {10, 11, 11, 13, 13, 11, 11, 13, 14, 15, 16, 16, 16, 16, 14, 14, 11, 10, 10, 10, 11, 14, 14, 14, 18, 14, 11, 11, 13, 10, 10, 10, 7, 6, 6, 6, 7, 7, 7, 10, 10, 6, 6, 6, 3, 3, 3, 3, 3, 5, 3, 2, 2, 2, 2, 2, 2, 2, 2, 5, 2, 3, 2, 1, 0},
                {11, 13, 13, 13, 13, 14, 11, 14, 14, 15, 16, 16, 16, 15, 13, 13, 11, 11, 11, 13, 14, 15, 14, 14, 19, 18, 15, 13, 13, 11, 13, 11, 11, 7, 7, 7, 10, 10, 10, 11, 13, 9, 9, 10, 6, 5, 5, 5, 5, 7, 5, 5, 5, 5, 3, 3, 2, 3, 3, 3, 2, 2, 3, 1, 0}}
        End Function

        ''' <summary>
        ''' Recording of a neuronal action potential (100 ms, 20 kHz sample rate, mV units).
        ''' </summary>
        ''' <returns>Recording of a neuronal action potential.</returns>
        Public Function ActionPotential() As Double()
            'originated from 17o05027_ic_ramp.abf as part of the pyABF project.
            Dim firstValue As Double = -40.83252
            Dim raw As SByte() = New SByte() {
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                -1, 1, 0, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, -1, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 1, -1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
                -1, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, -1, 0, 0, 0, 0,
                0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1,
                0, 0, 0, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1, 0, 0, -1, 1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, -1, 0, 1, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0,
                1, 0, 0, 0, 0, 0, 0, 0, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 1, 0, 0, 2,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,
                0, 0, 0, 1, 0, 0, 2, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, -1, 0,
                1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2, -1, 0, 0,
                0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 1,
                0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0,
                0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 1, 0, 0, 1, 0, 0, 0, 0, 2, 1, 0, 0, 0, 2, 1, 1, 0, 1, 0,
                1, 0, 0, 2, 0, 0, 0, 0, 2, 0, 1, 0, 0, 0, 0, 2, 0, 2, 0, 0,
                0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 2, 0, 3, 0, 0, 1, 0, 0, 1, 0,
                3, 0, 0, 1, 2, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 2, 1, 2, 0,
                1, 0, 2, 2, 1, 3, 1, 3, 1, 2, 2, 2, 3, 1, 3, 1, 3, 3, 3, 5,
                3, 4, 5, 6, 8, 12, 17, 25, 30, 38, 43, 47, 56, 65, 75, 86, 99, 104, 106, 100,
                93, 83, 73, 61, 53, 41, 34, 26, 18, 12, 5, -1, -9, -14, -21, -27, -32, -35, -38, -41,
                -45, -47, -49, -50, -53, -52, -51, -52, -54, -54, -53, -54, -51, -51, -49, -49, -48, -48, -44, -43,
                -41, -41, -38, -38, -34, -32, -33, -29, -28, -25, -25, -25, -20, -22, -17, -18, -16, -16, -16, -13,
                -13, -12, -12, -10, -10, -9, -9, -7, -9, -7, -6, -6, -6, -6, -5, -4, -5, -3, -4, -3,
                -3, 0, -4, -3, -1, -3, -1, 0, -2, -1, -1, -2, -1, -1, 0, -1, 0, 0, -1, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, -1,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0,
                1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, -1, 0, 0, 0,
                -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, -1, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, -1, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 1, -1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, -1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                -1, 0, 0, 0, 0, 0, 0, 0, -1, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0,
                0, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0,
                0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, -1, 1, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0,
                -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, -1, 0, 0, 0,
                0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 1, 0, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 1, 0, 0, -1, 1, -1, 0, 1, 0,
                0, 0, -1, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, -1, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, -1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 1, -1, 0, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, -1, 1,
                0, 0, 0, -1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0}

            Dim rawScale As Double = 25
            Dim waveform As Double() = New Double(raw.Length + 1 - 1) {}
            waveform(0) = firstValue
            For i As Integer = 0 To raw.Length - 1
                waveform(i + 1) = waveform(i) + raw(i) / rawScale
            Next

            Return waveform
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="size"></param>
        ''' <param name="normalize"></param>
        ''' <remarks>Lifted from https://github.com/swharden/FftSharp (MIT).</remarks>
        Private Function HanningWindow(size As Integer, Optional normalize As Boolean = False) As Double()
            Dim window As Double() = New Double(size - 1) {}
            For i As Integer = 0 To size - 1
                window(i) = 0.5 - 0.5 * Math.Cos(6.2831853071795862 * CDbl(i) / CDbl(size))
            Next

            If normalize Then
                Dim sum As Double = window.Sum()
                For i As Integer = 0 To window.Length - 1
                    window(i) /= sum
                Next
            End If

            Return window
        End Function

        Public Function NoisyBellCurve(rand As Random, count As Integer, Optional mult As Double = 1, Optional noiseFraction As Double = 0.1) As Double()
            Dim ys As Double() = DataGen.HanningWindow(count, False)
            For i As Integer = 0 To count - 1
                ys(i) += (rand.NextDouble() - 0.5) * noiseFraction
                ys(i) *= mult
            Next
            Return ys
        End Function

        ''' <summary>
        ''' Return a copy of the input array with large spans of NaN.
        ''' The higher the stability, the larger the spans are.
        ''' </summary>
        Public Function InsertNanRanges(values As Double(), rand As Random, Optional stability As Integer = 10) As Double()
            Dim values2 As Double() = New Double(values.Length - 1) {}
            Dim isNan As Boolean = (rand.NextDouble() > 0.5)

            For i As Integer = 0 To values.Length - 1
                values2(i) = If(isNan, Double.NaN, values(i))
                If (rand.Next(stability) = 0) Then
                    isNan = Not isNan
                End If
            Next

            Return values2
        End Function

        ''' <summary>
        ''' Return a New array of given length, filled with <paramref name="fillValue"/>.
        ''' </summary>
        Public Function Full(length As Integer, fillValue As Double) As Double()
            Return DataGen.FullGeneric(Of Double)(length, fillValue)
        End Function

        ''' <summary>
        ''' Return a New array of given length, filled with <paramref name="fillValue"/>.
        ''' </summary>
        Public Function FullGeneric(Of T)(length As Integer, fillValue As T) As T()
            Dim data As T() = New T(length - 1) {}
            For i As Integer = 0 To data.Length - 1
                data(i) = fillValue
            Next
            Return data
        End Function

        ''' <summary>
        ''' Electrocardiogram.
        ''' </summary>
        ''' <remarks>
        ''' The aim of the ECG simulator is to produce the typical ECG waveforms of different leads And
        ''' as many arrhythmias as possible. My ECG simulator is able To produce normal lead II ECG
        ''' waveform. The use Of a simulator has many advantages In the simulation Of ECG waveforms.
        ''' First one is saving Of time and another one is removing the difficulties Of taking real ECG
        ''' signals With invasive and noninvasive methods. The ECG simulator enables us To analyze And
        ''' study normal and abnormal ECG waveforms without actually Using the ECG machine. One can
        ''' simulate any given ECG waveform Using the ECG simulator. The way by which my simulator
        ''' differs from other typical ECG simulators is that i have used the principle Of Fourier
        ''' series. The calculations used and other necessary descriptions are included In the file
        ''' attached.
        ''' https://www.mathworks.com/matlabcentral/fileexchange/10858-ecg-simulation-using-matlab
        ''' (c) 2019 karthik raviprakash. All rights reserved. MIT license.
        ''' </remarks>
        Public Class Electrocardiogram

#Region "CTOR"

            Public Sub New(Optional heartRate As Double = 72)
                Me.HeartRate = heartRate
            End Sub

#End Region '/CTOR

#Region "PROPS"

            Public Property PWaveAmplitude As Double = 0.25
            Public Property PWaveDuration As Double = 9
            Public Property PWavePRInterval As Double = 0.16
            Public Property QWaveAmplitude As Double = 25
            Public Property QwaveDuration As Double = 66
            Public Property QWaveTime As Double = 0.166
            Public Property QRSWaveAmplitude As Double = 1.6
            Public Property QRSwaveDuration As Double = 0.11
            Public Property SWaveAmplitude As Double = 0.25
            Public Property SWaveDuration As Double = 66
            Public Property SWaveTime As Double = 9
            Public Property TWaveAmplitude As Double = 0.35
            Public Property TWaveDuration As Double = 0.142
            Public Property TWaveSTInterval As Double = 0.2
            Public Property UWaveAmplitude As Double = 35
            Public Property UWaveDuration As Double = 476
            Public Property UWaveTime As Double = 0.433

            Public Property HeartRate As Double
                Get
                    Return _HeartRate
                End Get
                Set(value As Double)
                    _HeartRate = value
                    _Period = 60 / value
                End Set
            End Property
            Private _HeartRate As Double

            Public Property Period As Double
                Get
                    Return _Period
                End Get
                Set(value As Double)
                    _Period = value
                    _HeartRate = 60 / value
                End Set
            End Property
            Private _Period As Double

#End Region '/PROPS

#Region "METHODS"

            Public Function GetVoltage(elapsedSeconds As Double) As Double
                elapsedSeconds = elapsedSeconds Mod 2 * Me.Period
                Dim value As Double = -0.9
                value += PWave(elapsedSeconds, PWaveAmplitude, PWaveDuration, PWavePRInterval, Period)
                value += QWave(elapsedSeconds, QWaveAmplitude, QwaveDuration, QWaveTime, Period)
                value += QRSwave(elapsedSeconds, QRSWaveAmplitude, QRSwaveDuration, Period)
                value += SWave(elapsedSeconds, SWaveAmplitude, SWaveDuration, SWaveTime, Period)
                value += TWave(elapsedSeconds, TWaveAmplitude, TWaveDuration, TWaveSTInterval, Period)
                value += UWave(elapsedSeconds, UWaveAmplitude, UWaveDuration, UWaveTime, Period)
                Return value
            End Function

            Private Shared Function QRSwave(x As Double, amplitude As Double, duration As Double, period As Double) As Double
                Dim l As Double = 0.5 * period
                Dim a As Double = amplitude
                Dim b As Double = 2 * l / duration
                Dim qrs1 As Double = a / (2 * b) * (2 - b)
                Dim n As Integer = 100
                Dim qrs2 As Double = 0
                For i As Integer = 1 To n - 1
                    Dim harm As Double = 2 * b * a / (i * i * Math.PI * Math.PI) * (1 - Math.Cos(i * Math.PI / b)) * Math.Cos(i * Math.PI * x / l)
                    qrs2 += harm
                Next
                Return (qrs1 + qrs2)
            End Function

            Private Shared Function PWave(x As Double, amplitude As Double, duration As Double, time As Double, period As Double) As Double
                Dim l As Double = 0.5 * period
                Dim a As Double = amplitude
                Dim b As Double = 2 * l / duration
                x += time
                Dim n As Integer = 100
                Dim p1 As Double = 1 / l
                Dim p2 As Double = 0
                Dim harm1 As Double
                For i As Integer = 1 To n - 1
                    harm1 = (Math.Sin(Math.PI / (2 * b) * (b - 2 * i)) / (b - 2 * i) + Math.Sin(Math.PI / (2 * b) * (b + 2 * i)) / (b + 2 * i)) * (2 / Math.PI) * Math.Cos(i * Math.PI * x / l)
                    p2 += harm1
                Next
                Return a * (p1 + p2)
            End Function

            Private Shared Function QWave(x As Double, amplitude As Double, duration As Double, time As Double, period As Double) As Double
                Dim l As Double = 0.5 * period
                Dim a As Double = amplitude
                Dim b As Double = 2 * l / duration
                x += time
                Dim n As Integer = 100
                Dim q1 As Double = a / (2 * b) * (2 - b)
                Dim q2 As Double = 0
                Dim harm5 As Double
                For i As Integer = 1 To n - 1
                    harm5 = 2 * b * a / (i * i * Math.PI * Math.PI) * (1 - Math.Cos(i * Math.PI / b)) * Math.Cos(i * Math.PI * x / l)
                    q2 += harm5
                Next
                Return -1 * (q1 + q2)
            End Function

            Private Shared Function SWave(x As Double, amplitude As Double, duration As Double, time As Double, period As Double) As Double
                Dim l As Double = 0.5 * period
                Dim a As Double = amplitude
                Dim b As Double = 2 * l / duration
                x -= time
                Dim n As Integer = 100
                Dim s1 As Double = a / (2 * b) * (2 - b)
                Dim s2 As Double = 0
                Dim harm3 As Double
                For i As Integer = 1 To n - 1
                    harm3 = 2 * b * a / (i * i * Math.PI * Math.PI) * (1 - Math.Cos(i * Math.PI / b)) * Math.Cos(i * Math.PI * x / l)
                    s2 += harm3
                Next
                Return -1 * (s1 + s2)
            End Function

            Private Shared Function TWave(x As Double, amplitude As Double, duration As Double, time As Double, period As Double) As Double
                Dim l As Double = 0.5 * period
                Dim a As Double = amplitude
                Dim b As Double = 2 * l / duration
                x = x - time - 0.045
                Dim n As Integer = 100
                Dim t1 As Double = 1 / l
                Dim t2 As Double = 0
                Dim harm2 As Double
                For i As Integer = 1 To n - 1
                    harm2 = (Math.Sin(Math.PI / (2 * b) * (b - 2 * i)) / (b - 2 * i) + Math.Sin(Math.PI / (2 * b) * (b + 2 * i)) / (b + 2 * i)) * (2 / Math.PI) * Math.Cos(i * Math.PI * x / l)
                    t2 += harm2
                Next
                Return a * (t1 + t2)
            End Function

            Private Shared Function UWave(x As Double, amplitude As Double, duration As Double, time As Double, period As Double) As Double
                Dim l As Double = 0.5 * period
                Dim a As Double = amplitude
                Dim b As Double = 2 * l / duration
                x -= time
                Dim n As Integer = 100
                Dim u1 As Double = 1 / l
                Dim u2 As Double = 0
                Dim harm4 As Double
                For i As Integer = 1 To n - 1
                    harm4 = (Math.Sin(Math.PI / (2 * b) * (b - 2 * i)) / (b - 2 * i) + Math.Sin(Math.PI / (2 * b) * (b + 2 * i)) / (b + 2 * i)) * (2 / Math.PI) * Math.Cos(i * Math.PI * x / l)
                    u2 += harm4
                Next
                Return a * (u1 + u2)
            End Function

#End Region '/METHODS

        End Class

    End Module

End Namespace