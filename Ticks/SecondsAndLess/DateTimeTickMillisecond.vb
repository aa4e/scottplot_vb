Imports System.Globalization

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Public Class DateTimeTickMillisecond
        Inherits DateTimeTickUnitBase

        Public Sub New(culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?)
            MyBase.New(culture, maxTickCount, manualSpacing)
            Kind = DateTimeUnit.Millisecond
            If (manualSpacing is Nothing) Then
                Deltas = New Integer() {1, 2, 5}
            End If
        End Sub

        Protected Overrides Function Floor(value As DateTime) As DateTime
            Return New DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second)
        End Function

        Protected Overrides Function Increment(value As DateTime, delta As Integer) As DateTime
            Return value.AddMilliseconds(delta)
        End Function

        Protected Overrides Function GetTickLabel(value As DateTime) As String
            Dim dt As String = value.ToString("d", Culture)
            Dim hourSpecifier As String = If(Tools.Uses24HourClock(Culture), "HH", "hh")
            Dim time As String = value.ToString(hourSpecifier & ":mm:ss.fff", Culture)
            Return (dt & vbLf & time)
        End Function

    End Class

End Namespace