Imports System.Collections.Generic
Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices

Namespace ScottPlot

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' WARNING: DO NOT MODIFY THIS API! IT IS USED BY SCOTTPLOT 4 AND SCOTTPLOT 5 
    ''' </remarks>
    Public Module NumericConversion

        Private Const ImplOptions As MethodImplOptions = MethodImplOptions.AggressiveInlining

        ''' <summary>
        ''' Returns the double value of a <typeparamref name="T"/> using a conversion technique optimized for the platform.
        ''' </summary>
        <MethodImpl(ImplOptions)>
        Public Function GenericToDouble(Of T)(ByRef value As T) As Double
            Return Convert.ToDouble(value)
        End Function

        ''' <summary>
        ''' Returns the double value of the <typeparamref name="T"/> at position <paramref name="i"/> in <paramref name="list"/>
        ''' using a conversion technique optimized for the platform.
        ''' </summary>
        <MethodImpl(ImplOptions)>
        Public Function GenericToDouble(Of T)(list As List(Of T), i As Integer) As Double
            Dim v As T = list(i)
            Return NumericConversion.GenericToDouble(Of T)(v)
        End Function

        ''' <summary>
        ''' Returns the double value of the <typeparamref name="T"/> at position <paramref name="i"/> in <paramref name="array"/>
        ''' using a conversion technique optimized for the platform.
        ''' </summary>
        <MethodImpl(ImplOptions)>
        Public Function GenericToDouble(Of T)(array As T(), i As Integer) As Double
            Dim v As T = array(i)
            Return NumericConversion.GenericToDouble(Of T)(v)
        End Function

        ''' <summary>
        ''' Creates a <typeparamref name="T"/> for a given double <paramref name="value"/>
        ''' using a conversion technique optimized for the platform.
        ''' </summary>
        <MethodImpl(ImplOptions)>
        Public Sub DoubleToGeneric(Of T)(value As Double, <System.Runtime.InteropServices.OutAttribute()> ByRef v As T)
            v = CType(Convert.ChangeType(value, GetType(T)), T)
        End Sub

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ToGenericArray(Of T)(input As Double()) As T()
            Dim result As T() = New T(input.Length - 1) {}
            For i As Integer = 0 To input.Length - 1
                NumericConversion.DoubleToGeneric(Of T)(input(i), result(i))
            Next
            Return result
        End Function

        Public Function AddBytes(a As Byte, b As Byte) As Byte
            Return (a + b)
        End Function

        Public Function SubtractBytes(a As Byte, b As Byte) As Byte
            Return (a - b)
        End Function

        Public Function LessThanOrEqualBytes(a As Byte, b As Byte) As Boolean
            Return (a <= b)
        End Function

        Public Function CreateAddFunction(Of T)() As Func(Of T, T, T)
            Dim paramA As ParameterExpression = Expression.Parameter(GetType(T), "a")
            Dim paramB As ParameterExpression = Expression.Parameter(GetType(T), "b")

            Dim body As BinaryExpression
            If (Type.GetTypeCode(GetType(T)) = TypeCode.[Byte]) Then 'TEST
                body = Expression.Add(paramA, paramB, GetType(NumericConversion).GetMethod("AddBytes"))
            Else
                body = Expression.Add(paramA, paramB)
            End If

            Return Expression.Lambda(Of Func(Of T, T, T))(body, New ParameterExpression() {paramA, paramB}).Compile()
        End Function

        Public Function CreateSubtractFunction(Of T)() As Func(Of T, T, T)
            Dim paramA As ParameterExpression = Expression.Parameter(GetType(T), "a")
            Dim paramB As ParameterExpression = Expression.Parameter(GetType(T), "b")

            Dim body As BinaryExpression
            If (Type.GetTypeCode(GetType(T)) = TypeCode.[Byte]) Then 'TEST
                body = Expression.Subtract(paramA, paramB, GetType(NumericConversion).GetMethod("SubtractBytes"))
            Else
                body = Expression.Subtract(paramA, paramB)
            End If

            Return Expression.Lambda(Of Func(Of T, T, T))(body, New ParameterExpression() {paramA, paramB}).Compile()
        End Function

        Public Function CreateLessThanOrEqualFunction(Of T)() As Func(Of T, T, Boolean)
            Dim paramA As ParameterExpression = Expression.Parameter(GetType(T), "a")
            Dim paramB As ParameterExpression = Expression.Parameter(GetType(T), "b")

            Dim body As BinaryExpression
            If (Type.GetTypeCode(GetType(T)) = TypeCode.[Byte]) Then 'TEST
                body = Expression.LessThanOrEqual(paramA, paramB, False, GetType(NumericConversion).GetMethod("LessThanOrEqualBytes"))
            Else
                body = Expression.LessThanOrEqual(paramA, paramB)
            End If

            Return Expression.Lambda(Of Func(Of T, T, Boolean))(body, New ParameterExpression() {paramA, paramB}).Compile()
        End Function

    End Module

End Namespace