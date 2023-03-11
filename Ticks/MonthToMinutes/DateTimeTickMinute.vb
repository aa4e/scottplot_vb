Imports System.Globalization

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Public Class DateTimeTickMinute
        Inherits DateTimeTickUnitBase

        Public Sub New(culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?)
            MyBase.New(culture, maxTickCount, manualSpacing)
            Kind = DateTimeUnit.Minute
            If (manualSpacing is Nothing) Then
                Deltas = New Integer() {1, 2, 5, 10, 15, 30}
            End If
        End Sub

        Protected Overrides Function Floor(value As DateTime) As DateTime
            Return New DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0)
        End Function

        Protected Overrides Function Increment(value As DateTime, delta As Integer) As DateTime
            Return value.AddMinutes(delta)
        End Function

        Protected Overrides Function GetTickLabel(value As DateTime) As String
            Dim dt As String = value.ToString("d", Culture)
            Dim tm As String = value.ToString("t", Culture)
            Return (dt & vbLf & tm)
        End Function

    End Class

End Namespace