Imports System.Collections.Generic
Imports System.Linq

Namespace ScottPlot.Statistics

    <Obsolete("This module is obsolete. Use ScottPlot.Statistics.Common methods instead.", False)>
    Public Class Histogram

#Region "PROPS"

        ''' <summary>
        ''' Number of values counted for each bin.
        ''' </summary>
        Public ReadOnly Property Counts As Integer()

        ''' <summary>
        ''' Lower edge for each bin.
        ''' </summary>
        Public ReadOnly BinEdges As Double()

        ''' <summary>
        ''' Number of values that were smaller than the lower edge of the smallest bin.
        ''' </summary>
        Public ReadOnly Property MinOutlierCount As Integer = 0

        ''' <summary>
        ''' Number of values that were greater than the upper edge of the smallest bin.
        ''' </summary>
        Public ReadOnly Property MaxOutlierCount As Integer = 0

        ''' <summary>
        ''' Default behavior is that outlier values are Not counted.
        ''' If this is enabled, min/max outliers will be counted in the first/last bin.
        ''' </summary>
        Public Property AddOutliersToEdgeBins As Boolean = False

#End Region '/PROPS

#Region "CTOR"

        ''' <summary>
        ''' Create a histogram which will count values supplied by <see cref="Add"/> and <see cref="AddRange"/>
        ''' </summary>
        Public Sub New(firstBin As Double, binSize As Double, binCount As Integer)
            Counts = New Integer(binCount - 1) {}
            BinEdges = New Double(binCount - 1) {}
            For i As Integer = 0 To binCount - 1
                BinEdges(i) = firstBin + binSize * i
            Next
        End Sub

#End Region '/CTOR

#Region "METHODS"

        ''' <summary>
        ''' Add a single value to the histogram.
        ''' </summary>
        Public Sub Add(value As Double)
            Throw New NotImplementedException()
        End Sub

        ''' <summary>
        ''' Add multiple values to the histogram.
        ''' </summary>
        Public Sub AddRange(values As IEnumerable(Of Double))
            For Each value As Double In values
                Add(value)
            Next
        End Sub

        ''' <summary>
        ''' Reset the histogram, setting all counts to zero.
        ''' </summary>
        Public Sub Clear()
            _MinOutlierCount = 0
            _MaxOutlierCount = 0
            For i As Integer = 0 To Counts.Length - 1
                Counts(i) = 0
            Next
        End Sub

#End Region '/METHODS

    End Class

End Namespace