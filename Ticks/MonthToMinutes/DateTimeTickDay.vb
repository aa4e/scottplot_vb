Imports System.Globalization

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Public Class DateTimeTickDay
        Inherits DateTimeTickUnitBase

        Public Sub New(culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?)
            MyBase.New(culture, maxTickCount, manualSpacing)
            Kind = DateTimeUnit.Day
            If (manualSpacing is Nothing) Then
                Deltas = New Integer() {1, 2, 5, 10, 20}
            End If
        End Sub

        Protected Overrides Function Floor(value As DateTime) As DateTime
            Return New DateTime(value.Year, value.Month, 1)
        End Function

        Protected Overrides Function Increment(value As DateTime, delta As Integer) As DateTime
            Return value.AddDays(delta)
        End Function

        Protected Overrides Function GetTickLabel(value As DateTime) As String
            Dim dt As New DateTime(value.Year, value.Month, value.Day)
            Dim localizedLabel As New System.Text.StringBuilder(dt.ToString("d", Culture)) 'short date pattern
            localizedLabel = localizedLabel.Replace("T", vbLf)
            localizedLabel.Append(vbLf & " ")
            Return localizedLabel.ToString()
        End Function

    End Class

End Namespace