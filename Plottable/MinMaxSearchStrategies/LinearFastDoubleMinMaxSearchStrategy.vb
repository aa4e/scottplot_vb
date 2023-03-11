Namespace ScottPlot.MinMaxSearchStrategies

    Public Class LinearFastDoubleMinMaxSearchStrategy(Of T As {Structure, IComparable})
        Inherits LinearMinMaxSearchStrategy(Of T)

        Public Overrides Property SourceArray As T()
            Get
                Return MyBase.SourceArray
            End Get
            Set(value As T())
                SourceArrayDouble = TryCast(value, Double())
                MyBase.SourceArray = value
            End Set
        End Property
        Private SourceArrayDouble As Double()

        Public Overrides Sub MinMaxRangeQuery(l As Integer, r As Integer, <System.Runtime.InteropServices.OutAttribute()> ByRef lowestValue As Double, <System.Runtime.InteropServices.OutAttribute()> ByRef highestValue As Double)
            If (SourceArrayDouble IsNot Nothing) Then
                lowestValue = SourceArrayDouble(l)
                highestValue = SourceArrayDouble(l)
                For i As Integer = l To r
                    If (SourceArrayDouble(i) < lowestValue) Then
                        lowestValue = SourceArrayDouble(i)
                    End If
                    If (SourceArrayDouble(i) > highestValue) Then
                        highestValue = SourceArrayDouble(i)
                    End If
                Next
            Else
                MyBase.MinMaxRangeQuery(l, r, lowestValue, highestValue)
            End If
        End Sub

        Public Overrides Function SourceElement(index As Integer) As Double
            If (SourceArrayDouble IsNot Nothing) Then
                Return SourceArrayDouble(index)
            End If
            Return NumericConversion.GenericToDouble(Of T)(Me.SourceArray(index))
        End Function

    End Class

End Namespace