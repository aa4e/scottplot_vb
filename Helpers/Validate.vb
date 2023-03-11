Imports System.Drawing

Namespace ScottPlot

    Public Module Validate

        Private Function ValidLabel(label As String) As String
            Return If(String.IsNullOrWhiteSpace(label), "[unknown variable]", label)
        End Function

        ''' <summary>
        ''' Throw an exception if the value is NaN or infinity.
        ''' </summary>
        Public Sub AssertIsReal(label As String, value As Double)
            label = Validate.ValidLabel(label)

            If Double.IsNaN(value) Then
                Throw New InvalidOperationException($"{label} is NaN.")
            End If
            If Double.IsInfinity(value) Then
                Throw New InvalidOperationException($"{label} is infinity.")
            End If
        End Sub

        ''' <summary>
        ''' Throw an exception if the array is null or contains NaN or infinity.
        ''' </summary>
        Public Sub AssertAllReal(label As String, values As Double())
            label = Validate.ValidLabel(label)
            If (values is Nothing) Then
                Throw New InvalidOperationException($"{label} must not be null.")
            End If

            For i As Integer = 0 To values.Length - 1
                If Double.IsNaN(values(i)) OrElse Double.IsInfinity(values(i)) Then
                    Throw New InvalidOperationException($"{label} index {i} is invalid ({values(i)}).")
                End If
            Next
        End Sub

        ''' <summary>
        ''' Throw an exception if the array is null or contains NaN or infinity.
        ''' </summary>
        Public Sub AssertAllReal(Of T)(label As String, values As T())
            If (GetType(T) is GetType(Double)) Then 'TEST 
                Validate.AssertAllReal(label, CType(CObj(values), Double()))
            ElseIf (GetType(T) is GetType(Single)) Then 'TEST 
                Validate.AssertAllReal(Of Single)(label, CType(CObj(values), Single()))
            Else
                Throw New InvalidOperationException("Values must be Single() or Double().")
            End If
        End Sub

        ''' <summary>
        ''' Throw an exception if an element is less than the previous element.
        ''' </summary>
        Public Sub AssertDoesNotDescend(values As Double())
            Validate.AssertDoesNotDescend(Of Double)("values", values)
        End Sub

        ''' <summary>
        ''' Throw an exception if an element is less than the previous element.
        ''' </summary>
        Public Sub AssertDoesNotDescend(Of T)(values As T())
            Validate.AssertDoesNotDescend(Of T)("values", values)
        End Sub

        ''' <summary>
        ''' Throw an exception if an element is less than the previous element.
        ''' </summary>
        Public Sub AssertDoesNotDescend(Of T)(label As String, values As T(), Optional minIndex As Integer = 0, Optional maxIndex As Integer? = Nothing)
            If (values is Nothing) Then
                Throw New InvalidOperationException($"{label } must not be null.")
            End If

            If (maxIndex is Nothing) Then
                maxIndex = values.Length - 1
            End If

            label = Validate.ValidLabel(label)

            For i As Integer = minIndex To maxIndex.Value - 1
                If (NumericConversion.GenericToDouble(values(i)) > NumericConversion.GenericToDouble(values(i + 1))) Then
                    Throw New InvalidOperationException($"{label} must not descend: {label}[{i}]={values(i)} but {label}[{i + 1}]={values(i + 1)}.")
                End If
            Next
        End Sub

        ''' <summary>
        ''' Throw an exception if the array does not contain at least one element.
        ''' </summary>
        Public Sub AssertHasElements(label As String, values As Double())
            label = Validate.ValidLabel(label)
            If (values is Nothing) Then
                Throw New InvalidOperationException($"{label} must not be null.")
            End If
            If (values.Length = 0) Then
                Throw New InvalidOperationException($"{label} must contain at least one element.")
            End If
        End Sub

        ''' <summary>
        ''' Throw an exception if the array does not contain at least one element.
        ''' </summary>
        Public Sub AssertHasElements(Of T)(label As String, values As T())
            label = Validate.ValidLabel(label)
            If (values is Nothing) Then
                Throw New InvalidOperationException($"{label} must not be null.")
            End If
            If (values.Length = 0) Then
                Throw New InvalidOperationException($"{label} must contain at least one element.")
            End If
        End Sub

        ''' <summary>
        ''' Throw an exception if the array does not contain at least one element.
        ''' </summary>
        Public Sub AssertHasElements(label As String, values As Color())
            label = Validate.ValidLabel(label)
            If (values is Nothing) Then
                Throw New InvalidOperationException($"{label} must not be null.")
            End If
            If (values.Length = 0) Then
                Throw New InvalidOperationException($"{label} must contain at least one element.")
            End If
        End Sub

        ''' <summary>
        ''' Throw an exception if the array does not contain at least one element.
        ''' </summary>
        Public Sub AssertHasElements(label As String, values As String())
            label = Validate.ValidLabel(label)
            If (values is Nothing) Then
                Throw New InvalidOperationException($"{label} must not be null.")
            End If
            If (values.Length = 0) Then
                Throw New InvalidOperationException($"{label} must contain at least one element.")
            End If
        End Sub

        ''' <summary>
        ''' Throw an exception if non-null arrays have different lengths.
        ''' </summary>
        Public Sub AssertEqualLength(label As String, a As Double(),
                                     Optional b As Double() = Nothing, Optional c As Double() = Nothing,
                                     Optional d As Double() = Nothing, Optional e As Double() = Nothing,
                                     Optional f As Double() = Nothing)
            label = Validate.ValidLabel(label)
            If (Not Validate.IsEqualLength(a, b, c, d, e, f)) Then
                Throw New InvalidOperationException($"{label} must all have same length.")
            End If
        End Sub

        ''' <summary>
        ''' Throw an exception if non-null arrays have different lengths.
        ''' </summary>
        Public Sub AssertEqualLength(Of T1, T2)(label As String, a As T1(), b As T2())
            label = Validate.ValidLabel(label)
            If (a.Length <> b.Length) Then
                Throw New InvalidOperationException($"{label } must all have same length.")
            End If
        End Sub

        ''' <summary>
        ''' Returns true if all non-null arguments have equal length.
        ''' </summary>
        Public Function IsEqualLength(a As Double(),
                                      Optional b As Double() = Nothing, Optional c As Double() = Nothing,
                                      Optional d As Double() = Nothing, Optional e As Double() = Nothing,
                                      Optional f As Double() = Nothing) As Boolean
            If (a is Nothing) Then
                Throw New InvalidOperationException("First array must not be null.")
            End If
            If (b IsNot Nothing) AndAlso (b.Length <> a.Length) Then Return False
            If (c IsNot Nothing) AndAlso (b.Length <> a.Length) Then Return False
            If (d IsNot Nothing) AndAlso (b.Length <> a.Length) Then Return False
            If (e IsNot Nothing) AndAlso (b.Length <> a.Length) Then Return False
            If (f IsNot Nothing) AndAlso (b.Length <> a.Length) Then Return False
            Return True
        End Function

        ''' <summary>
        ''' Throws an exception if the string is null, empty, or only contains whitespace.
        ''' </summary>
        Public Sub AssertHasText(label As String, value As String)
            label = Validate.ValidLabel(label)
            If String.IsNullOrWhiteSpace(value) Then
                Throw New InvalidOperationException($"{label} must contain text.")
            End If
        End Sub

    End Module

End Namespace