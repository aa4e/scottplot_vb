Imports System.Globalization

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Public Class DateTimeTickHour
        Inherits DateTimeTickUnitBase

        Public Sub New(culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?)
            MyBase.New(culture, maxTickCount, manualSpacing)
            Kind = DateTimeUnit.Hour
            If (manualSpacing is Nothing) Then
                Deltas = New Integer() {1, 2, 4, 8, 12}
            End If
        End Sub

        Protected Overrides Function Floor(value As DateTime) As DateTime
            Return New DateTime(value.Year, value.Month, value.Day)
        End Function

        Protected Overrides Function Increment(value As DateTime, delta As Integer) As DateTime
            Return value.AddHours(delta)
        End Function

        Protected Overrides Function GetTickLabel(value As DateTime) As String
            Dim dt As String = value.ToString("d", Culture)
            Dim time As String = value.ToString("t", Culture)
            Return (dt & vbLf & time)
        End Function

    End Class

End Namespace