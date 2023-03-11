Imports System.Linq.Expressions
Imports System.Threading.Tasks

Namespace ScottPlot.DataStructures

    Public Class SegmentedTree(Of T As {Structure, IComparable})

#Region "PROPS, FIELDS"

        Private TreeMin As T()
        Private TreeMax As T()
        ''' <summary>
        ''' Size of each Tree.
        ''' </summary>
        Private N As Integer
        Public TreesReady As Boolean = False
        'precompiled lambda expressions for fast math on generic
        Private Shared MinExp As Func(Of T, T, T)
        Private Shared MaxExp As Func(Of T, T, T)
        Private Shared EqualExp As Func(Of T, T, Boolean)
        Private Shared MaxValue As Func(Of T)
        Private Shared MinValue As Func(Of T)
        Private Shared LessThanExp As Func(Of T, T, Boolean)
        Private Shared GreaterThanExp As Func(Of T, T, Boolean)

        Public Property SourceArray As T()
            Get
                Return _SourceArray
            End Get
            Set(value As T())
                If (value is Nothing) Then
                    Throw New Exception("Source Array cannot be null.")
                End If
                _SourceArray = value
                UpdateTrees()
            End Set
        End Property
        Private _SourceArray As T()

#End Region '/PROPS, FIELDS


#Region "CTORs"

        Public Sub New()
            Try 'runtime check
                Dim t As T = Activator.CreateInstance(Of T)()
                NumericConversion.GenericToDouble(Of T)(t)
            Catch
                Throw New ArgumentOutOfRangeException("Unsupported data type, provide convertable to double data types.")
            End Try
            InitExp()
        End Sub

        Public Sub New(data As T())
            UpdateTreesInBackground()
        End Sub

        Private Sub InitExp()
            Dim paramA As ParameterExpression = Expression.Parameter(GetType(T), "a")
            Dim paramB As ParameterExpression = Expression.Parameter(GetType(T), "b")
            'add the parameters together
            Dim bodyMin As Expression = Expression.Condition(Expression.LessThanOrEqual(paramA, paramB), paramA, paramB)
            Dim bodyMax As ConditionalExpression = Expression.Condition(Expression.GreaterThanOrEqual(paramA, paramB), paramA, paramB)
            Dim bodyEqual As BinaryExpression = Expression.Equal(paramA, paramB)
            Dim bodyMaxValue As MemberExpression = Expression.MakeMemberAccess(Nothing, GetType(T).GetField("MaxValue"))
            Dim bodyMinValue As MemberExpression = Expression.MakeMemberAccess(Nothing, GetType(T).GetField("MinValue"))
            Dim bodyLessThan As BinaryExpression = Expression.LessThan(paramA, paramB)
            Dim bodyGreaterThan As BinaryExpression = Expression.GreaterThan(paramA, paramB)
            'compile it
            MinExp = Expression.Lambda(Of Func(Of T, T, T))(bodyMin, paramA, paramB).Compile()
            MaxExp = Expression.Lambda(Of Func(Of T, T, T))(bodyMax, paramA, paramB).Compile()
            EqualExp = Expression.Lambda(Of Func(Of T, T, Boolean))(bodyEqual, paramA, paramB).Compile()
            MaxValue = Expression.Lambda(Of Func(Of T))(bodyMaxValue).Compile()
            MinValue = Expression.Lambda(Of Func(Of T))(bodyMinValue).Compile()
            LessThanExp = Expression.Lambda(Of Func(Of T, T, Boolean))(bodyLessThan, paramA, paramB).Compile()
            GreaterThanExp = Expression.Lambda(Of Func(Of T, T, Boolean))(bodyGreaterThan, paramA, paramB).Compile()
        End Sub

#End Region '/CTORs

#Region "METHODS"

        Public Async Function SetSourceAsync(data As T()) As Task
            If (data is Nothing) Then
                Throw New ArgumentNullException("Data cannot be null.")
            End If
            SourceArray = data
            Await Task.Run(Sub() UpdateTrees())
        End Function

        Public Sub UpdateElement(index As Integer, newValue As T)
            SourceArray(index) = newValue

            'update Tree, can be optimized
            Dim ind2 As Integer = CInt(N / 2 + index / 2)
            If (index = SourceArray.Length - 1) Then 'last elem haven't pair
                TreeMin(ind2) = SourceArray(index)
                TreeMax(ind2) = SourceArray(index)

            ElseIf (index Mod 2 = 0) Then 'even elem have right pair
                TreeMin(ind2) = MinExp(SourceArray(index), SourceArray(index + 1))
                TreeMax(ind2) = MaxExp(SourceArray(index), SourceArray(index + 1))

            Else 'odd elem have left pair           
                TreeMin(ind2) = MinExp(SourceArray(index), SourceArray(index - 1))
                TreeMax(ind2) = MaxExp(SourceArray(index), SourceArray(index - 1))
            End If

            Dim startIndex As Integer = CInt((N / 2 + index / 2) / 2)
            For i As Integer = startIndex To 1
                'for (int i = (n / 2 + index / 2) / 2; i > 0; i /= 2)
                Dim candidate As T = MinExp(TreeMin(i * 2), TreeMin(i * 2 + 1))
                If EqualExp(TreeMin(i), candidate) Then 'if node same then new value don't need to recalc all upper
                    Exit For
                End If
                TreeMin(i) = candidate
                i \= 2 'TEST
            Next

            For i As Integer = startIndex To 1
                'for (int i = (n / 2 + index / 2) / 2; i > 0; i /= 2)
                Dim candidate As T = MaxExp(TreeMax(i * 2), TreeMax(i * 2 + 1))
                If EqualExp(TreeMax(i), candidate) Then 'if node same then new value don't need to recalc all upper
                    Exit For
                End If
                TreeMax(i) = candidate
                i \= 2 'TEST
            Next
        End Sub

        Public Sub UpdateRange(from As Integer, [to] As Integer, newData As T(), Optional fromData As Integer = 0)
            'update source signal
            For i As Integer = from To [to] - 1
                SourceArray(i) = newData(i - from + fromData)
            Next

            For i As Integer = CInt(N / 2 + from / 2) To CInt(N / 2 + [to] / 2 - 1)
                TreeMin(i) = MinExp(SourceArray(i * 2 - N), SourceArray(i * 2 + 1 - N))
                TreeMax(i) = MaxExp(SourceArray(i * 2 - N), SourceArray(i * 2 + 1 - N))
            Next

            Dim ind1 As Integer = CInt(N / 2 + [to] / 2)
            If ([to] = SourceArray.Length) Then 'last elem haven't pair
                TreeMin(ind1) = SourceArray([to] - 1)
                TreeMax(ind1) = SourceArray([to] - 1)
            ElseIf ([to] Mod 2 = 1) Then 'last elem even(to-1) and not last
                TreeMin(ind1) = MinExp(SourceArray([to] - 1), SourceArray([to]))
                TreeMax(ind1) = MaxExp(SourceArray([to] - 1), SourceArray([to]))
            End If

            from = CInt((N / 2 + from / 2) / 2)
            [to] = CInt((N / 2 + [to] / 2) / 2)

            While (from <> 0) 'up to root elem, that is [1], [0] - is free elem
                If (from = [to]) Then 'Left == rigth, so no need more from To Loop
                    For i As Integer = from To 1
                        Dim candidate As T = MinExp(TreeMin(i * 2), TreeMin(i * 2 + 1))
                        If EqualExp(TreeMin(i), candidate) Then 'if node same then new value don't need to recalc all upper
                            Exit For
                        End If
                        TreeMin(i) = candidate
                        i = CInt(i / 2) 'TEST
                    Next
                    For i As Integer = from To 1
                        Dim candidate As T = MaxExp(TreeMax(i * 2), TreeMax(i * 2 + 1))
                        If EqualExp(TreeMax(i), candidate) Then 'if node same then new value don't need to recalc all upper
                            Exit For
                        End If
                        TreeMax(i) = candidate
                        i = CInt(i / 2) 'TEST
                    Next
                    Exit While 'all work done exit while loop

                Else 'Recalc all level nodes in range 
                    For i As Integer = from To [to]
                        TreeMin(i) = MinExp(TreeMin(i * 2), TreeMin(i * 2 + 1))
                        TreeMax(i) = MaxExp(TreeMax(i * 2), TreeMax(i * 2 + 1))
                    Next
                End If

                'level up
                from = CInt(from / 2)
                [to] = CInt([to] / 2)
            End While
        End Sub

        Public Sub UpdateData(from As Integer, newData As T())
            UpdateRange(from, newData.Length, newData, 0)
        End Sub

        Public Sub UpdateData(newData As T())
            UpdateRange(0, newData.Length, newData, 0)
        End Sub

        Public Sub UpdateTreesInBackground()
            Task.Run(Sub()
                         UpdateTrees()
                     End Sub)
        End Sub

        Public Sub UpdateTrees()
            'O(n) to build trees
            TreesReady = False
            Try
                If SourceArray.Length = 0 Then
                    Throw New ArgumentOutOfRangeException("Array cant't be empty.")
                End If
                'Size up to pow2
                If (SourceArray.Length > &H40_00_00_00) Then 'pow 2 must be more then int.MaxValue
                    Throw New ArgumentOutOfRangeException($"Array higher than {&H40_00_00_00} not supported by SignalConst.")
                End If
                Dim pow2 As Integer = 1
                While (pow2 < &H40_00_00_00) AndAlso (pow2 < SourceArray.Length)
                    pow2 <<= 1
                End While
                N = pow2
                TreeMin = New T(N - 1) {}
                TreeMax = New T(N - 1) {}
                Dim maxValue As T = SegmentedTree(Of T).MaxValue()
                Dim minValue As T = SegmentedTree(Of T).MinValue()

                'fill bottom layer of tree
                For i As Integer = 0 To CInt(SourceArray.Length / 2 - 1) 'with source array pairs min/max
                    Dim ti As Integer = CInt(N / 2 + i)
                    TreeMin(ti) = SegmentedTree(Of T).MinExp(SourceArray(i * 2), SourceArray(i * 2 + 1))
                    TreeMax(ti) = SegmentedTree(Of T).MaxExp(SourceArray(i * 2), SourceArray(i * 2 + 1))
                Next
                If (SourceArray.Length Mod 2 = 1) Then 'if array size odd, last element haven't pair to compare
                    Dim ti As Integer = CInt(N / 2 + SourceArray.Length / 2)
                    TreeMin(ti) = SourceArray(SourceArray.Length - 1)
                    TreeMax(ti) = SourceArray(SourceArray.Length - 1)
                End If
                For i As Integer = CInt(N / 2 + (SourceArray.Length + 1) / 2) To N - 1 'min/max for pairs of nonexistent elements
                    TreeMin(i) = minValue
                    TreeMax(i) = maxValue
                Next
                'fill other layers
                For i As Integer = CInt(N / 2 - 1) To 1 Step -1
                    TreeMin(i) = MinExp(TreeMin(2 * i), TreeMin(2 * i + 1))
                    TreeMax(i) = MaxExp(TreeMax(2 * i), TreeMax(2 * i + 1))
                Next
                TreesReady = True
            Catch ex As OutOfMemoryException
                TreeMin = Nothing
                TreeMax = Nothing
                TreesReady = False
            End Try
        End Sub

        Public Sub MinMaxRangeQuery(l As Integer, r As Integer, <System.Runtime.InteropServices.OutAttribute()> ByRef lowestValue As Double, <System.Runtime.InteropServices.OutAttribute()> ByRef highestValue As Double)
            'O(log(n)) for each range min/max query
            Dim lowestValueT As T
            Dim highestValueT As T
            'if the tree calculation isn't finished or if it crashed
            If (Not TreesReady) Then
                'use the original (slower) min/max calculated method
                lowestValueT = SourceArray(l)
                highestValueT = SourceArray(l)
                For i As Integer = l To r - 1
                    If LessThanExp(SourceArray(i), lowestValueT) Then
                        lowestValueT = SourceArray(i)
                    End If
                    If GreaterThanExp(SourceArray(i), highestValueT) Then
                        highestValueT = SourceArray(i)
                    End If
                Next
                lowestValue = NumericConversion.GenericToDouble(Of T)(lowestValueT)
                highestValue = NumericConversion.GenericToDouble(Of T)(highestValueT)
                Return
            End If

            lowestValueT = SegmentedTree(Of T).MaxValue()
            highestValueT = SegmentedTree(Of T).MinValue()
            If (l = r) Then
                Dim tmp As Double = NumericConversion.GenericToDouble(Of T)(SourceArray(l))
                highestValue = tmp
                lowestValue = tmp
                Return
            End If
            'first iteration on source array that virtualy bottom of tree
            If ((l and 1) = 1) Then 'l is right child
                lowestValueT = SegmentedTree(Of T).MinExp(lowestValueT, SourceArray(l))
                highestValueT = SegmentedTree(Of T).MaxExp(highestValueT, SourceArray(l))
            End If
            If ((r and 1) <> 1) Then 'r is left child
                lowestValueT = SegmentedTree(Of T).MinExp(lowestValueT, SourceArray(r))
                highestValueT = SegmentedTree(Of T).MaxExp(highestValueT, SourceArray(r))
            End If
            'go up from array to bottom of Tree
            l = CInt((l + N + 1) / 2)
            r = CInt((r + N - 1) / 2)
            'next iterations on tree
            While (l <= r)
                If ((l and 1) = 1) Then 'l is right child
                    lowestValueT = SegmentedTree(Of T).MinExp(lowestValueT, TreeMin(l))
                    highestValueT = SegmentedTree(Of T).MaxExp(highestValueT, TreeMax(l))
                End If
                If ((r and 1) <> 1) Then 'r is left child
                    lowestValueT = SegmentedTree(Of T).MinExp(lowestValueT, TreeMin(r))
                    highestValueT = SegmentedTree(Of T).MaxExp(highestValueT, TreeMax(r))
                End If
                'go up one level
                l = CInt((l + 1) / 2)
                r = CInt((r - 1) / 2)
            End While
            lowestValue = NumericConversion.GenericToDouble(Of T)(lowestValueT)
            highestValue = NumericConversion.GenericToDouble(Of T)(highestValueT)
        End Sub

#End Region '/METHODS

    End Class

End Namespace