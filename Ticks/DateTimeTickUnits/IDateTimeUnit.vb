Imports System.Runtime.CompilerServices

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Public Interface IDateTimeUnit

        Function GetTicksAndLabels(from As DateTime, [to] As DateTime, format As String) As Tuple(Of Double(), String())

    End Interface

End Namespace