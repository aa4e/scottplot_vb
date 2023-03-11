Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

Namespace ScottPlot.Ticks

    Public Enum AxisOrientation
        Vertical
        Horizontal
    End Enum

    Public Enum TickLabelFormat
        Numeric
        DateTime
    End Enum

    Public Enum MinorTickDistribution
        Even
        Log
    End Enum

    ''' <summary>
    ''' Class creates pretty tick labels (with offset and exponent) uses graph settings
    ''' to inspect the tick font and ensure tick labels will Not overlap.
    ''' It also respects manually defined tick spacing settings set via plt.Grid().
    ''' </summary>
    Public Class TickCollection

#Region "PROPS, FIELDS"

        'TODO: store these in a class
        Public TickPositionsMajor As Double()
        Public TickPositionsMinor As Double()
        Public TickLabels As String()

        'When populated, manual ticks are the ONLY ticks shown
        Public ManualTickPositions As Double()
        Public ManualTickLabels As String()

        'When populated, additionalTicks are shown in addition to automatic ticks
        Public AdditionalTickPositions As Double()
        Public AdditionalTickLabels As String()

        ''' <summary>
        ''' Controls how to translate positions to strings.
        ''' </summary>
        Public LabelFormat As TickLabelFormat = TickLabelFormat.Numeric

        ''' <summary>
        ''' This is used to determine whether tick density should be based on tick label width or height.
        ''' </summary>
        Public Orientation As AxisOrientation

        ''' <summary>
        ''' If True, the sign of numeric tick labels will be inverted. 
        ''' This is used to give the appearance of descending ticks.
        ''' </summary>
        Public LabelUsingInvertedSign As Boolean

        ''' <summary>
        ''' Define how minor ticks are distributed (evenly vs. log scale).
        ''' </summary>
        Public MinorTickDistribution As MinorTickDistribution

        Public NumericFormatString As String
        Public DateTimeFormatString As String

        ''' <summary>
        ''' If defined, this function will be used to generate tick labels from positions.
        ''' </summary>
        Public ManualTickFormatter As Func(Of Double, String)

        Public Radix As Integer = 10
        Public Prefix As String

        Public ManualSpacingX As Double
        Public ManualSpacingY As Double

        Public ManualDateTimeSpacingUnitX As DateTimeUnit? = Nothing
        Public ManualDateTimeSpacingUnitY As DateTimeUnit? = Nothing

        Public Culture As CultureInfo = CultureInfo.DefaultThreadCurrentCulture
        Public UseMultiplierNotation As Boolean
        Public UseOffsetNotation As Boolean
        Public UseExponentialNotation As Boolean = True

        ''' <summary>
        ''' Optimally packed tick labels have a density 1.0 and lower densities space ticks farther apart.
        ''' </summary>
        Public TickDensity As Single = 1

        ''' <summary>
        ''' Defines the minimum distance (in coordinate units) for major ticks.
        ''' </summary>
        Public MinimumTickSpacing As Double

        ''' <summary>
        ''' If True, non-integer tick positions will not be used. 
        ''' This may be desired for log10-scaled axes so tick marks are even powers of 10.
        ''' </summary>
        Public IntegerPositionsOnly As Boolean

        ''' <summary>
        ''' If minor tick distribution is log-scaled, place this many minor ticks.
        ''' </summary>
        Public LogScaleMinorTickCount As Integer = 10

        ''' <summary>
        ''' Number of minor ticks per major tick.
        ''' </summary>
        Public MinorTickCount As Integer = 5

        ''' <summary>
        ''' Determine tick density using a fixed formula to estimate label size instead of MeasureString(). 
        ''' This is less accurate, but is consistent across operating systems, and is independent of font.
        ''' </summary>
        Public MeasureStringManually As Boolean

        ''' <summary>
        ''' Controls how to translate exponential part of a number to strings
        ''' </summary>
        Public Property CornerLabelFormat As String = "E{0}"

        ''' <summary>
        ''' Measured size of the largest tick label.
        ''' </summary>
        Public ReadOnly Property CornerLabel As String = ""

        ''' <summary>
        ''' Measured size of the largest tick label.
        ''' </summary>
        Public ReadOnly Property LargestLabelWidth As Single = 15

        ''' <summary>
        ''' Measured size of the largest tick label.
        ''' </summary>
        Public ReadOnly Property LargestLabelHeight As Single = 12

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Sub Recalculate(dims As PlotDimensions, tickFont As ScottPlot.Drawing.Font)
            If (ManualTickPositions is Nothing) Then
                'first pass uses forced density with manual label sizes to consistently approximate labels
                If (LabelFormat = TickLabelFormat.DateTime) Then
                    RecalculatePositionsAutomaticDatetime(dims, 20, 24, CInt(10 * TickDensity))
                Else
                    RecalculatePositionsAutomaticNumeric(dims, 15, 12, CInt(10 * TickDensity))
                End If

                'second pass calculates density using measured labels produced by the first pass
                Dim t = MaxLabelSize(tickFont)
                _LargestLabelWidth = t.Item1
                _LargestLabelHeight = t.Item2
                If (LabelFormat = TickLabelFormat.DateTime) Then
                    RecalculatePositionsAutomaticDatetime(dims, LargestLabelWidth, LargestLabelHeight, Nothing)
                Else
                    RecalculatePositionsAutomaticNumeric(dims, LargestLabelWidth, LargestLabelHeight, Nothing)
                End If
            Else
                If (ManualTickPositions.Length <> ManualTickLabels.Length) Then
                    Throw New InvalidOperationException("ManualTickPositions() must have the same length as manualTickLabels.")
                End If

                Dim min As Double = If(Orientation = AxisOrientation.Vertical, dims.YMin, dims.XMin)
                Dim max As Double = If(Orientation = AxisOrientation.Vertical, dims.YMax, dims.XMax)

                Dim visibleIndexes = Enumerable.Range(0, ManualTickPositions.Count()) _
                    .Where(Function(i) ManualTickPositions(i) >= min) _
                    .Where(Function(i) ManualTickPositions(i) <= max)
                TickPositionsMajor = visibleIndexes.Select(Function(x) ManualTickPositions(x)).ToArray()
                TickPositionsMinor = Nothing
                TickLabels = visibleIndexes.Select(Function(x) ManualTickLabels(x)).ToArray()
                _CornerLabel = Nothing

                Dim t = MaxLabelSize(tickFont)
                _LargestLabelWidth = t.Item1
                _LargestLabelHeight = t.Item2
            End If
        End Sub

        Public Sub SetCulture(Optional shortDatePattern As String = Nothing,
                              Optional decimalSeparator As String = Nothing,
                              Optional numberGroupSeparator As String = Nothing,
                              Optional decimalDigits As Integer? = Nothing,
                              Optional numberNegativePattern As Integer? = Nothing,
                              Optional numberGroupSizes As Integer() = Nothing)
            ' Culture may be null if the thread culture is the same is the system culture.
            ' If it is null, assigning it to a clone of the current culture solves this and also makes it mutable.
            Culture = If(Culture, CType(CultureInfo.CurrentCulture.Clone(), CultureInfo)) 'TEST 
            With Culture
                .DateTimeFormat.ShortDatePattern = If(shortDatePattern, .DateTimeFormat.ShortDatePattern)
                .NumberFormat.NumberDecimalDigits = If(decimalDigits, .NumberFormat.NumberDecimalDigits)
                .NumberFormat.NumberDecimalSeparator = If(decimalSeparator, .NumberFormat.NumberDecimalSeparator)
                .NumberFormat.NumberGroupSeparator = If(numberGroupSeparator, .NumberFormat.NumberGroupSeparator)
                .NumberFormat.NumberGroupSizes = If(numberGroupSizes, .NumberFormat.NumberGroupSizes)
                .NumberFormat.NumberNegativePattern = If(numberNegativePattern, .NumberFormat.NumberNegativePattern)
            End With
        End Sub

        Private Function MaxLabelSize(tickFont As ScottPlot.Drawing.Font) As Tuple(Of Single, Single)
            If (TickLabels is Nothing) OrElse (TickLabels.Length = 0) Then
                Return New Tuple(Of Single, Single)(0, 0)
            End If

            Dim largestString As String = ""
            For Each s As String In TickLabels.Where(Function(x) Not String.IsNullOrEmpty(x))
                If (s.Length > largestString.Length) Then
                    largestString = s
                End If
            Next

            If (LabelFormat = TickLabelFormat.DateTime) Then
                'widen largest string based on the longest month name
                For Each s As String In New DateTimeFormatInfo().MonthGenitiveNames
                    Dim s2 As String = s & "\n" & "1985"
                    If (s2.Length > largestString.Length) Then
                        largestString = s2
                    End If
                Next
            End If

            If MeasureStringManually Then
                Dim width As Single = largestString.Trim().Length * tickFont.Size * 0.75F
                Dim height As Single = tickFont.Size
                Return New Tuple(Of Single, Single)(width, height)
            Else
                Dim maxLabSize As System.Drawing.SizeF = Drawing.GDI.MeasureStringUsingTemporaryGraphics(largestString.Trim(), tickFont)
                Return New Tuple(Of Single, Single)(maxLabSize.Width, maxLabSize.Height)
            End If
        End Function

        Private Sub RecalculatePositionsAutomaticDatetime(dims As PlotDimensions, labelWidth As Single, labelHeight As Single, forcedTickCount As Integer?)
            If (MinimumTickSpacing > 0) Then
                Throw New InvalidOperationException("Minimum tick spacing does not support DateTime ticks.")
            End If

            Dim low As Double
            Dim high As Double
            Dim tickCount As Integer

            If (Orientation = AxisOrientation.Vertical) Then
                low = dims.YMin - dims.UnitsPerPxY 'add an extra pixel to capture the edge tick
                high = dims.YMax + dims.UnitsPerPxY 'add an extra pixel to capture the edge tick
                tickCount = CInt(dims.DataHeight / labelHeight * TickDensity)
                tickCount = If(forcedTickCount, tickCount)
            Else
                low = dims.XMin - dims.UnitsPerPxX 'add an extra pixel to capture the edge tick
                high = dims.XMax + dims.UnitsPerPxX 'add an extra pixel to capture the edge tick
                tickCount = CInt(dims.DataWidth / labelWidth * TickDensity)
                tickCount = If(forcedTickCount, tickCount)
            End If

            If (low < high) Then
                low = Math.Max(low, New DateTime(100, 1, 1, 0, 0, 0).ToOADate()) 'minimum OADate value
                high = Math.Min(high, DateTime.MaxValue.ToOADate())

                Dim dtManualUnits = If(Orientation = AxisOrientation.Vertical, ManualDateTimeSpacingUnitY, ManualDateTimeSpacingUnitX)
                Dim dtManualSpacing As Double = If(Orientation = AxisOrientation.Vertical, ManualSpacingY, ManualSpacingX)

                Try
                    Dim from As DateTime = DateTime.FromOADate(low)
                    Dim [to] As DateTime = DateTime.FromOADate(high)

                    Dim unitFactory As New DateTimeTickUnits.DateTimeUnitFactory()
                    Dim tickUnit As DateTimeTickUnits.IDateTimeUnit = unitFactory.CreateUnit(from, [to], Culture, tickCount, dtManualUnits, CInt(dtManualSpacing))
                    Dim t = tickUnit.GetTicksAndLabels(from, [to], DateTimeFormatString)
                    TickLabels = TickLabels.Select(Function(x) x.Trim()).ToArray()
                Catch
                    TickPositionsMajor = New Double() {} 'far zoom out can produce FromOADate() exception
                End Try
            Else
                TickPositionsMajor = New Double() {}
            End If

            'dont forget to set all the things
            TickPositionsMinor = Nothing
            _CornerLabel = Nothing
        End Sub

        Private Sub RecalculatePositionsAutomaticNumeric(dims As PlotDimensions, labelWidth As Single, labelHeight As Single, forcedTickCount As Integer?)
            Dim low As Double
            Dim high As Double
            Dim tickSpacing As Double

            If (Orientation = AxisOrientation.Vertical) Then
                low = dims.YMin - dims.UnitsPerPxY 'add an extra pixel to capture the edge tick
                high = dims.YMax + dims.UnitsPerPxY 'add an extra pixel to capture the edge tick
                Dim maxTickCount As Integer = CInt(dims.DataHeight / labelHeight * TickDensity)
                maxTickCount = If(forcedTickCount, maxTickCount)
                tickSpacing = If(ManualSpacingY <> 0, ManualSpacingY, TickCollection.GetIdealTickSpacing(low, high, maxTickCount, Radix))
                tickSpacing = Math.Max(tickSpacing, MinimumTickSpacing)
            Else
                low = dims.XMin - dims.UnitsPerPxX 'add an extra pixel to capture the edge tick
                high = dims.XMax + dims.UnitsPerPxX 'add an extra pixel to capture the edge tick
                Dim maxTickCount As Integer = CInt(dims.DataWidth / labelWidth * TickDensity)
                maxTickCount = If(forcedTickCount, maxTickCount)
                tickSpacing = If(ManualSpacingX <> 0, ManualSpacingX, TickCollection.GetIdealTickSpacing(low, high, maxTickCount, Radix))
                tickSpacing = Math.Max(tickSpacing, MinimumTickSpacing)
            End If

            'now that tick spacing is known, populate the list of ticks and labels
            Dim firstTickOffset As Double = low Mod tickSpacing
            Dim tickCount As Integer = CInt(((high - low) / tickSpacing) + 2)
            tickCount = If(tickCount > 1000, 1000, tickCount)
            tickCount = If(tickCount < 1, 1, tickCount)
            TickPositionsMajor = Enumerable.Range(0, tickCount) _
                .Select(Function(x) low - firstTickOffset + tickSpacing * x) _
                .Where(Function(x) low <= x AndAlso x <= high).ToArray()

            If (TickPositionsMajor.Length < 2) Then
                Dim tickBelow As Double = low - firstTickOffset
                Dim firstTick As Double = If(TickPositionsMajor.Length <> 0, TickPositionsMajor(0), tickBelow)
                Dim nextTick As Double = tickBelow + tickSpacing
                TickPositionsMajor = New Double() {firstTick, nextTick}
            End If

            If IntegerPositionsOnly Then
                Dim firstTick As Integer = CInt(TickPositionsMajor(0))
                TickPositionsMajor = TickPositionsMajor.Where(Function(x) x = CInt(x)).Distinct().ToArray()
                If (TickPositionsMajor.Length < 2) Then
                    TickPositionsMajor = New Double() {firstTick - 1, firstTick, firstTick + 1}
                End If
            End If

            Dim prettyTickLabels = GetPrettyTickLabels(
                TickPositionsMajor,
                UseMultiplierNotation,
                UseOffsetNotation,
                UseExponentialNotation,
                LabelUsingInvertedSign,
                Culture)
            TickLabels = prettyTickLabels.Item1
            _CornerLabel = prettyTickLabels.Item2

            If (MinorTickDistribution = MinorTickDistribution.Log) Then
                TickPositionsMinor = MinorFromMajorLog(TickPositionsMajor, low, high, LogScaleMinorTickCount)
            Else
                TickPositionsMinor = MinorFromMajor(TickPositionsMajor, MinorTickCount, low, high)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim allTickLabels As String = String.Join(", ", TickLabels)
            Return $"Tick Collection: [{allTickLabels}] {CornerLabel}"
        End Function

        Private Shared Function GetIdealTickSpacing(low As Double, high As Double, maxTickCount As Integer, Optional radix As Integer = 10) As Double
            Dim range As Double = high - low
            If (Not Double.IsInfinity(range)) Then
                Dim exponent As Integer = CInt(Math.Log(range, radix))
                Dim tickSpacings As New List(Of Double)() From {Math.Pow(radix, exponent)}
                tickSpacings.Add(tickSpacings.Last())
                tickSpacings.Add(tickSpacings.Last())

                Dim divBy As Double()
                If (radix = 10) Then
                    divBy = New Double() {2, 2, 2.5} '10, 5, 2.5, 1
                ElseIf (radix <> 16) Then
                    divBy = New Double() {2, 2, 2, 2} '16, 8, 4, 2, 1
                Else
                    Throw New ArgumentException($"Radix {radix} is not supported.")
                End If

                Dim divisions As Integer = 0
                Dim tickCount As Integer = 0
                While (tickCount < maxTickCount) AndAlso (tickSpacings.Count < 1000)
                    tickSpacings.Add(tickSpacings.Last() / divBy(divisions Mod divBy.Length))
                    tickCount = CInt(range / tickSpacings.Last())
                    divisions += 1
                End While

                Dim startIndex As Integer = 3
                Dim maxSpacing As Double = range / 2
                Dim idealSpacing As Double = tickSpacings(tickSpacings.Count - startIndex)

                While (idealSpacing > maxSpacing AndAlso startIndex >= 1)
                    idealSpacing = tickSpacings(tickSpacings.Count - startIndex)
                    startIndex -= 1
                End While

                Return idealSpacing
            Else
                Diagnostics.Debug.WriteLine("Fake Tick spacing")
                Return 100
            End If
        End Function

        Private Function FormatLocal(value As Double, culture As CultureInfo) As String
            'if a custom format string exists use it
            If (NumericFormatString IsNot Nothing) Then
                Return value.ToString(NumericFormatString, culture)
            End If

            'If the number is round or large, use the numeric format
            'https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-numeric-n-format-specifier

            Dim isRoundNumber As Boolean = (Math.Round(value, 0) - value < 0.0001)
            Dim isLargeNumber As Boolean = (Math.Abs(value) > 1000)
            If (isRoundNumber OrElse isLargeNumber) Then
                Return value.ToString("N0", culture)
            End If

            'Otherwise the number is probably small or very precise to use the general format (with slight rounding)
            'https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-general-g-format-specifier
            Return Math.Round(value, 10).ToString("G", culture)
        End Function

        Public Function GetPrettyTickLabels(positions As Double(),
                                            useMultiplierNotation As Boolean,
                                            useOffsetNotation As Boolean,
                                            useExponentialNotation As Boolean,
                                            invertSign As Boolean,
                                            culture As CultureInfo) As Tuple(Of String(), String)
            'given positions returns nicely-formatted labels (with offset and multiplier)
            Dim labels As String() = New String(positions.Length - 1) {}
            Dim cornerLabel As New System.Text.StringBuilder()

            If (positions.Length = 0) Then
                Return New Tuple(Of String(), String)(labels, cornerLabel.ToString())
            End If

            Dim range As Double = positions.Last() - positions.First()
            Dim exponent As Double = Math.Log10(range)

            Dim multiplier As Double = 1.0
            If useMultiplierNotation AndAlso (Math.Abs(exponent) > 2) Then
                multiplier = Math.Pow(10, exponent)
            End If

            Dim offset As Double = 0.0
            If useOffsetNotation Then
                offset = positions.First()
                If (Math.Abs(offset / range) < 10) Then
                    offset = 0
                End If
            End If

            For i As Integer = 0 To positions.Length - 1
                Dim adjustedPosition As Double = (positions(i) - offset) / multiplier
                If invertSign Then
                    adjustedPosition *= -1
                End If
                labels(i) = If(ManualTickFormatter is Nothing,
                    FormatLocal(adjustedPosition, culture),
                    ManualTickFormatter(adjustedPosition))
                If (labels(i) = "-0") Then
                    labels(i) = "0"
                End If
            Next

            If useExponentialNotation Then
                If (multiplier <> 1) Then
                    cornerLabel.Append(String.Format(CornerLabelFormat, exponent))
                End If
                If (offset <> 0) Then
                    cornerLabel.Append(Tools.ScientificNotation(offset))
                End If
            Else
                If (multiplier <> 1) Then
                    cornerLabel.Append(FormatLocal(multiplier, culture))
                End If
                If (offset <> 0) Then
                    cornerLabel.Append(" +")
                    cornerLabel.Append(FormatLocal(offset, culture))
                End If
                cornerLabel = cornerLabel.Replace("+-", "-")
            End If
            Return New Tuple(Of String(), String)(labels, cornerLabel.ToString())
        End Function

        Public Function MinorFromMajor(majorTicks As Double(), minorTicksPerMajorTick As Double, lowerLimit As Double, upperLimit As Double) As Double()
            If (majorTicks is Nothing) OrElse (majorTicks.Length < 2) Then
                Return Nothing
            End If

            Dim majorTickSpacing As Double = majorTicks(1) - majorTicks(0)
            Dim minorTickSpacing As Double = majorTickSpacing / minorTicksPerMajorTick

            Dim majorTicksWithPadding As New List(Of Double)()
            majorTicksWithPadding.Add(majorTicks(0) - majorTickSpacing)
            majorTicksWithPadding.AddRange(majorTicks)

            Dim minorTicks As New List(Of Double)()
            For Each majorTickPosition As Double In majorTicksWithPadding
                For i As Integer = 1 To CInt(minorTicksPerMajorTick - 1)
                    Dim minorTickPosition As Double = majorTickPosition + minorTickSpacing * i
                    If (minorTickPosition > lowerLimit) AndAlso (minorTickPosition < upperLimit) Then
                        minorTicks.Add(minorTickPosition)
                    End If
                Next
            Next
            Return minorTicks.ToArray()
        End Function

        ''' <summary>
        ''' Return an array of log-spaced minor tick marks for the given range
        ''' </summary>
        ''' <param name="majorTickPositions">Locations of visible major ticks. Must be evenly spaced.</param>
        ''' <param name="min">Do not include minor ticks less than this value.</param>
        ''' <param name="max">Do not include minor ticks greater than this value.</param>
        ''' <param name="divisions">Number of minor ranges to divide each major range into. (A range is the space between tick marks).</param>
        ''' <returns>Array of minor tick positions (empty at positions occupied by major ticks).</returns>
        Public Function MinorFromMajorLog(majorTickPositions As Double(), min As Double, max As Double, divisions As Integer) As Double()
            'if too few major ticks are visible, don't attempt to render minor ticks
            If (majorTickPositions is Nothing) OrElse (majorTickPositions.Length < 2) Then
                Return Nothing
            End If

            Dim majorTickSpacing As Double = majorTickPositions(1) - majorTickPositions(0)
            If (majorTickSpacing = 0) Then
                Return Nothing
            End If

            Dim lowerBound As Double = majorTickPositions.First() - majorTickSpacing
            Dim upperBound As Double = majorTickPositions.Last() + majorTickSpacing

            Dim minorTicks As New List(Of Double)()
            For majorTick As Double = lowerBound To upperBound Step majorTickSpacing
                Dim positions As Double() = GetLogDistributedPoints(divisions, majorTick, majorTick + majorTickSpacing, False)
                minorTicks.AddRange(positions)
            Next
            Return minorTicks.Where(Function(x) x >= min AndAlso x <= max).ToArray()
        End Function

        Public Shared Function GetDateLabels(ticksOADate As Double(), culture As CultureInfo) As String()
            Dim dtFmt As String = String.Empty
            Try
                Dim dtTickSep As TimeSpan = DateTime.FromOADate(ticksOADate(1)) - DateTime.FromOADate(ticksOADate(0))
                If (dtTickSep.TotalDays > 365 * 5) Then
                    dtFmt = "{0:yyyy}"
                ElseIf (dtTickSep.TotalDays > 365) Then
                    dtFmt = "{0:yyyy-MM}"
                ElseIf (dtTickSep.TotalDays > 0.5) Then
                    dtFmt = "{0:yyyy-MM-dd}"
                ElseIf (dtTickSep.TotalMinutes > 0.5) Then
                    dtFmt = "{0:yyyy-MM-dd" & Environment.NewLine & "H:mm}"
                Else
                    dtFmt = "{0:yyyy-MM-dd" & Environment.NewLine & "H:mm:ss}"
                End If
            Catch ex As Exception
                Diagnostics.Debug.WriteLine(ex)
            End Try

            Dim labels As String() = New String(ticksOADate.Length - 1) {}
            For i As Integer = 0 To ticksOADate.Length - 1
                Try
                    Dim dt As DateTime = DateTime.FromOADate(ticksOADate(i))
                    labels(i) = String.Format(culture, dtFmt, dt)
                Catch ex As Exception
                    labels(i) = "?"
                End Try
            Next
            Return labels
        End Function

        Private Function GetMajorTicks(min As Double, max As Double) As Tick()
            If (TickPositionsMajor is Nothing) OrElse (TickPositionsMajor.Length = 0) Then
                Return New Tick() {}
            End If

            Dim ticks As New List(Of Tick)
            For i As Integer = 0 To TickPositionsMajor.Length - 1
                Dim tick As New Tick(TickPositionsMajor(i), TickLabels(i), True, LabelFormat = TickLabelFormat.DateTime)
                ticks.Add(tick)
            Next

            If (AdditionalTickPositions IsNot Nothing) Then
                Dim sign As Double = If(LabelUsingInvertedSign, -1, 1)
                For i As Integer = 0 To AdditionalTickPositions.Length - 1
                    Dim tick As New Tick(AdditionalTickPositions(i) * sign, AdditionalTickLabels(i), True, LabelFormat = TickLabelFormat.DateTime)
                    ticks.Add(tick)
                Next
            End If
            Return ticks.Where(Function(x) x.Position >= min AndAlso x.Position <= max).OrderBy(Function(x) x.Position).ToArray()
        End Function

        Private Function GetMinorTicks() As Tick()
            If (TickPositionsMinor is Nothing) OrElse (TickPositionsMinor.Length = 0) Then
                Return New Tick() {}
            End If
            Dim ticks As Tick() = New Tick(TickPositionsMinor.Length - 1) {}
            For i As Integer = 0 To ticks.Length - 1
                ticks(i) = New Tick(TickPositionsMinor(i), Nothing, False, LabelFormat = TickLabelFormat.DateTime)
            Next
            Return ticks
        End Function

        Public Function GetTicks(min As Double, max As Double) As Tick()
            Return GetMajorTicks(min, max).Concat(GetMinorTicks()).ToArray()
        End Function

        Public Function GetVisibleMajorTicks(dims As PlotDimensions) As Tick()
            Dim low As Double
            Dim high As Double
            If (Orientation = AxisOrientation.Vertical) Then
                low = dims.YMin - dims.UnitsPerPxY 'add an extra pixel to capture the edge tick
                high = dims.YMax + dims.UnitsPerPxY  'add an extra pixel to capture the edge tick
            Else
                low = dims.XMin - dims.UnitsPerPxX 'add an extra pixel to capture the edge tick
                high = dims.XMax + dims.UnitsPerPxX  'add an extra pixel to capture the edge tick
            End If
            Return GetMajorTicks(low, high)
        End Function

        Public Function GetVisibleMinorTicks(dims As PlotDimensions) As Tick()
            Dim low As Double
            Dim high As Double
            If (Orientation = AxisOrientation.Vertical) Then
                low = dims.YMin - dims.UnitsPerPxY  'add an extra pixel to capture the edge tick
                high = dims.YMax + dims.UnitsPerPxY 'add an extra pixel to capture the edge tick
            Else
                low = dims.XMin - dims.UnitsPerPxX  'add an extra pixel to capture the edge tick
                high = dims.XMax + dims.UnitsPerPxX 'add an extra pixel to capture the edge tick
            End If
            Return GetMinorTicks().Where(Function(t) t.Position >= low AndAlso t.Position <= high).ToArray()
        End Function

        Public Function GetVisibleTicks(dims As PlotDimensions) As Tick()
            Return GetVisibleMajorTicks(dims).Concat(GetVisibleMinorTicks(dims)).ToArray()
        End Function

        ''' <summary>
        ''' Return log-distributed points between the min/max values.
        ''' </summary>
        ''' <param name="count">Number of divisions.</param>
        ''' <param name="min">Lowest value.</param>
        ''' <param name="max">Highest value.</param>
        ''' <param name="inclusive">If true, returned values will contain the min/max values themselves.</param>
        Public Shared Function GetLogDistributedPoints(count As Integer, min As Double, max As Double, inclusive As Boolean) As Double()
            Dim range As Double = max - min
            Dim values = DataGen.Range(1, 10, 10 / count).Select(Function(x) Math.Log10(x)).Select(Function(x) x * range + min)
            Return If(inclusive, values.ToArray(), values.Skip(1).Take(count - 2).ToArray())
        End Function

#End Region '/METHODS

    End Class

End Namespace