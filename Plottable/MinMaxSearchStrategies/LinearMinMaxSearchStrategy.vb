Imports System.Linq.Expressions

Namespace ScottPlot.MinMaxSearchStrategies

    Public Class LinearMinMaxSearchStrategy(Of T As {Structure, IComparable})
        Implements IMinMaxSearchStrategy(Of T)

#Region "PROPS, FIELDS"

        ' precompiled lambda expressions for fast math on generic
        Private Shared LessThanExp As Func(Of T, T, Boolean)
        Private Shared GreaterThanExp As Func(Of T, T, Boolean)

        Public Overridable Property SourceArray As T() Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).SourceArray
            Get
                Return _SourceArray
            End Get
            Set(value As T())
                _SourceArray = value
            End Set
        End Property
        Private _SourceArray As T()

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New()
            InitExp()
        End Sub

        Private Sub InitExp()
            Dim paramA As ParameterExpression = Expression.Parameter(GetType(T), "a")
            Dim paramB As ParameterExpression = Expression.Parameter(GetType(T), "b")
            'add the parameters together
            Dim bodyLessThan As Expression = Expression.LessThan(paramA, paramB)
            Dim bodyGreaterThan As BinaryExpression = Expression.GreaterThan(paramA, paramB)
            'compile it
            LessThanExp = Expression.Lambda(Of Func(Of T, T, Boolean))(bodyLessThan, paramA, paramB).Compile()
            GreaterThanExp = Expression.Lambda(Of Func(Of T, T, Boolean))(bodyGreaterThan, paramA, paramB).Compile()
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overridable Sub MinMaxRangeQuery(l As Integer, r As Integer, <System.Runtime.InteropServices.OutAttribute()> ByRef lowestValue As Double, <System.Runtime.InteropServices.OutAttribute()> ByRef highestValue As Double) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).MinMaxRangeQuery
            Dim lowestValueT As T = SourceArray(l)
            Dim highestValueT As T = SourceArray(l)
            For i As Integer = l To r
                If LinearMinMaxSearchStrategy(Of T).LessThanExp(SourceArray(i), lowestValueT) Then
                    lowestValueT = SourceArray(i)
                End If
                If LinearMinMaxSearchStrategy(Of T).GreaterThanExp(SourceArray(i), highestValueT) Then
                    highestValueT = SourceArray(i)
                End If
            Next
            lowestValue = NumericConversion.GenericToDouble(Of T)(lowestValueT)
            highestValue = NumericConversion.GenericToDouble(Of T)(highestValueT)
        End Sub

        Public Overridable Function SourceElement(index As Integer) As Double Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).SourceElement
            Return NumericConversion.GenericToDouble(Of T)(SourceArray(index))
        End Function

        Public Sub UpdateElement(index As Integer, newValue As T) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).UpdateElement
            SourceArray(index) = newValue
        End Sub

        Public Sub UpdateRange(from As Integer, [to] As Integer, newData As T(), Optional fromData As Integer = 0) Implements ScottPlot.MinMaxSearchStrategies.IMinMaxSearchStrategy(Of T).UpdateRange
            For i As Integer = from To [to] - 1
                SourceArray(i) = newData(i - from + fromData)
            Next
        End Sub

#End Region '/METHODS

    End Class

End Namespace