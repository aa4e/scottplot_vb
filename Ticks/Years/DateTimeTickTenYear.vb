Imports System.Globalization

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Public Class DateTimeTickTenYear
        Inherits DateTimeTickUnitBase

        Public Sub New(culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?)
            MyBase.New(culture, maxTickCount, manualSpacing)
            Kind = DateTimeUnit.TenYear
            If (manualSpacing is Nothing) Then
                Deltas = New Integer() {1, 2, 5}
            End If
        End Sub

        Protected Overrides Function Floor(value As DateTime) As DateTime
            Return New DateTime(value.Year - value.Year Mod 10, 1, 1)
        End Function

        Protected Overrides Function Increment(value As DateTime, delta As Integer) As DateTime
            Return value.AddYears(delta * 10)
        End Function

        Protected Overrides Function GetTickLabel(value As DateTime) As String
            Dim dateTime As New DateTime(value.Year, 1, 1)
            Return (dateTime.ToString("yyyy", Culture) & vbLf & " ")
        End Function

    End Class

End Namespace