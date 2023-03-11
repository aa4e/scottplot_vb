Namespace ScottPlot.MinMaxSearchStrategies

    Public Class SegmentedTreeMinMaxSearchStrategy(Of T As {Structure, IComparable})
        Implements IMinMaxSearchStrategy(Of T)

#Region "PROPS, FIELDS"

        Private SegmentedTree As DataStructures.SegmentedTree(Of T)

        Public Property SourceArray As T() Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).SourceArray
            Get
                Return SegmentedTree.SourceArray
            End Get
            Set(value As T())
                SegmentedTree.SourceArray = value
            End Set
        End Property

        Public ReadOnly Property TreesReady As Boolean
            Get
                Return SegmentedTree.TreesReady
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTORs"

        Public Sub New()
            SegmentedTree = New DataStructures.SegmentedTree(Of T)()
        End Sub

        Public Sub New(data As T())
            Me.New()
            SourceArray = data
        End Sub

#End Region '/CTORs

#Region "METHODS"

        Public Sub MinMaxRangeQuery(l As Integer, r As Integer, <System.Runtime.InteropServices.OutAttribute()> ByRef lowestValue As Double, <System.Runtime.InteropServices.OutAttribute()> ByRef highestValue As Double) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).MinMaxRangeQuery
            Me.SegmentedTree.MinMaxRangeQuery(l, r, lowestValue, highestValue)
        End Sub

        Public Function SourceElement(index As Integer) As Double Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).SourceElement
            Return NumericConversion.GenericToDouble(Of T)(Me.SourceArray(index))
        End Function

        Public Sub UpdateElement(index As Integer, newValue As T) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).UpdateElement
            SegmentedTree.UpdateElement(index, newValue)
        End Sub

        Public Sub UpdateRange(from As Integer, [to] As Integer, newData As T(), Optional fromData As Integer = 0) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).UpdateRange
            SegmentedTree.UpdateRange(from, [to], newData, fromData)
        End Sub

#End Region '/METHODS

    End Class

End Namespace