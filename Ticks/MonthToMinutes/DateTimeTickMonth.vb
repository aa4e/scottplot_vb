Imports System.Globalization

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Public Class DateTimeTickMonth
        Inherits DateTimeTickUnitBase

        Public Sub New(culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?)
            MyBase.New(culture, maxTickCount, manualSpacing)
            Kind = DateTimeUnit.Month
            If (manualSpacing is Nothing) Then
                Deltas = New Integer() {1, 2, 3, 6}
            End If
        End Sub

        Protected Overrides Function Floor(value As DateTime) As DateTime
            Return New DateTime(value.Year, 1, 1)
        End Function

        Protected Overrides Function Increment(value As DateTime, delta As Integer) As DateTime
            Return value.AddMonths(delta)
        End Function

        Protected Overrides Function GetTickLabel(value As DateTime) As String
            Dim dt As DateTime = New DateTime(value.Year, value.Month, 1)
            Dim localizedLabel As String = dt.ToString("Y", Culture) 'year and month pattern
            Dim pos As Integer = localizedLabel.IndexOf(" ")
            If (pos < 0) Then
                Return localizedLabel & vbLf
            End If
            Return (localizedLabel.Substring(0, pos) & vbLf & localizedLabel.Substring(pos + 1, localizedLabel.Length - pos - 1))
        End Function

    End Class

End Namespace