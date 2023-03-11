Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Public Class DateTimeUnitFactory

        Public Function Create(kind As DateTimeUnit, culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?) As IDateTimeUnit
            Select Case kind
                Case DateTimeUnit.ThousandYear
                    Return New DateTimeTickThousandYear(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.HundredYear
                    Return New DateTimeTickHundredYear(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.TenYear
                    Return New DateTimeTickTenYear(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Year
                    Return New DateTimeTickYear(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Month
                    Return New DateTimeTickMonth(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Day
                    Return New DateTimeTickDay(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Hour
                    Return New DateTimeTickHour(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Minute
                    Return New DateTimeTickMinute(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Second
                    Return New DateTimeTickSecond(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Decisecond
                    Return New DateTimeTickDecisecond(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Centisecond
                    Return New DateTimeTickCentisecond(culture, maxTickCount, manualSpacing)
                Case DateTimeUnit.Millisecond
                    Return New DateTimeTickMillisecond(culture, maxTickCount, manualSpacing)
                Case Else
                    Throw New NotImplementedException("Unrecognized TickUnit.")
            End Select
        End Function

        Public Function CreateBestUnit(from As DateTime, toDate As DateTime, culture As CultureInfo, maxTickCount As Integer) As IDateTimeUnit
            Dim daysApart As Double = toDate.ToOADate() - from.ToOADate()
            Dim halfDensity As Double = maxTickCount / 2

            Dim tickUnitBorders As New List(Of Tuple(Of DateTimeUnit, Double)) From {
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.ThousandYear, 365 * 1000 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.HundredYear, 365 * 100 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.TenYear, 365 * 10 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Year, 365 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Month, 30 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Day, halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Hour, 1 / 24 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Minute, 1 / 24 / 60 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Second, 1 / 24 / 3600 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Decisecond, 1 / 24 / 3600 / 10 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Centisecond, 1 / 24 / 3600 / 100 * halfDensity)),
                (New Tuple(Of DateTimeUnit, Double)(DateTimeUnit.Millisecond, 1 / 24 / 3600 / 1000 * halfDensity))
            } 'tick unit borders in days

            Dim bestTickUnitKind = tickUnitBorders.FirstOrDefault(Function(tr) daysApart > tr.Item2)
            bestTickUnitKind = If(bestTickUnitKind, tickUnitBorders.Last()) ' last tickUnit if not found best
            Return Create(bestTickUnitKind.Item1, culture, maxTickCount, Nothing)
        End Function

        Public Function CreateUnit(from As DateTime, toDate As DateTime, culture As CultureInfo, maxTickCount As Integer, manualUnits As DateTimeUnit?, manualSpacing As Integer?) As IDateTimeUnit
            If (manualUnits is Nothing) Then
                Return CreateBestUnit(from, toDate, culture, maxTickCount)
            Else
                Return Create(manualUnits.Value, culture, maxTickCount, manualSpacing)
            End If
        End Function

    End Class

End Namespace