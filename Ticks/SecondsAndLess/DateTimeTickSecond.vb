Imports System.Globalization

Namespace ScottPlot.Ticks.DateTimeTickUnits

    Friend Class DateTimeTickSecond
        Inherits DateTimeTickUnitBase

        Public Sub New(culture As CultureInfo, maxTickCount As Integer, manualSpacing As Integer?)
            MyBase.New(culture, maxTickCount, manualSpacing)
        End Sub

    End Class

End Namespace