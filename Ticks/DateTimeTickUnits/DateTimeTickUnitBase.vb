Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

Namespace ScottPlot.Ticks.DateTimeTickUnits

    ''' <summary>
    ''' Base class implements Seconds Unit.
    ''' </summary>
    Public Class DateTimeTickUnitBase
        Implements IDateTimeUnit

        Protected Kind As DateTimeUnit = DateTimeUnit.Second
        Protected Culture As CultureInfo
        Protected Deltas As Integer() = {1, 2, 5, 10, 15, 30}
        Protected MaxTickCount As Integer

        Public Sub New(culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?)
            Me.Culture = If(culture, CultureInfo.CurrentCulture)
            Me.MaxTickCount = maxTickCount
            If (manualSpacing IsNot Nothing) Then
                Me.Deltas = New Integer() {manualSpacing.Value}
            End If
        End Sub

        Protected Overridable Function Floor(value As DateTime) As DateTime
            Return New DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0)
        End Function

        Protected Overridable Function Increment(value As DateTime, delta As Integer) As DateTime
            Return value.AddSeconds(delta)
        End Function

        Protected Overridable Function GetTickLabel(value As DateTime) As String
            Dim dt As String = value.ToString("d", Culture) 'short time
            Dim time As String = value.ToString("T", Culture) 'long time
            Return dt & Environment.NewLine & time
        End Function

        Public Function GetTicksAndLabels(from As DateTime, toDate As DateTime, format As String) As Tuple(Of Double(), String()) Implements ScottPlot.Ticks.DateTimeTickUnits.IDateTimeUnit.GetTicksAndLabels
            Dim ticks As DateTime() = GetTicks(from, toDate, Deltas, MaxTickCount)
            Dim labels As String() = If(format is Nothing,
                ticks.Select(Function(t) GetTickLabel(t)).ToArray(),
                ticks.Select(Function(t) t.ToString(format, Culture)).ToArray())
            Return New Tuple(Of Double(), String())(ticks.Select(Function(t) t.ToOADate()).ToArray(), labels)
        End Function

        Protected Function GetTicks(from As DateTime, toDate As DateTime, deltas As Integer(), maxTickCount As Integer) As DateTime()
            Dim result As DateTime() = New DateTime() {}
            For Each delta As Integer In deltas
                result = GetTicks(from, toDate, delta)
                If (result.Length <= maxTickCount) Then
                    Return result
                End If
            Next
            Return result
        End Function

        Protected Overridable Function GetTicks(from As DateTime, toDate As DateTime, delta As Integer) As DateTime()
            Dim dates As New List(Of DateTime)()
            Dim dt As DateTime = Me.Floor(from)
            While (dt <= toDate)
                If (dt >= from) Then
                    dates.Add(dt)
                End If
                Try
                    dt = Increment(dt, delta)
                Catch ex As Exception
                    Diagnostics.Debug.WriteLine(ex)
                    Exit While 'our date is larger than possible
                End Try
            End While
            Return dates.ToArray()
        End Function

    End Class

End Namespace