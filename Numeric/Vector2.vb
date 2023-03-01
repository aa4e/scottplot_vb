Namespace ScottPlot.Statistics

    ''' <summary>
    ''' </summary>
    ''' <remarks>
    ''' A simpler double-precision floating point alternative to the System.Numerics.Vectors package.
    ''' https//github.com/microsoft/referencesource/blob/master/System.Numerics/System/Numerics/Vector2.cs
    ''' </remarks>
    Public Structure Vector2

#Region "PROPS, FIELDS"

        Public ReadOnly X As Double
        Public ReadOnly Y As Double

        Public Shared ReadOnly Property Zero As Statistics.Vector2
            Get
                Return New Statistics.Vector2()
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(x As Double, y As Double)
            Me.X = x
            Me.Y = y
        End Sub

#End Region '/CTOR

#Region "METHODS"

        ''' <summary>
        ''' Returns a vector with the same direction as the given vector, but with a length of 1.
        ''' </summary>
        ''' <param name="value">The vector to normalize.</param>
        ''' <returns>The normalized vector.</returns>
        Public Shared Function Normalize(value As Statistics.Vector2) As Statistics.Vector2
            If Vector.IsHardwareAccelerated Then
                Dim length As Single = value.Length()
                Return value / length
            Else
                Dim ls As Single = CSng(value.X * value.X + value.Y * value.Y)
                Dim invNorm As Single = 1 / CSng(Math.Sqrt(ls))
                Return New Statistics.Vector2(value.X * invNorm, value.Y * invNorm)
            End If
        End Function

        ''' <summary>
        ''' Returns the length of the vector.
        ''' </summary>
        ''' <returns>The vector's length.</returns>
        Public Function Length() As Single
            If (Vector.IsHardwareAccelerated) Then
                Dim ls As Single = Statistics.Vector2.Dot(Me, Me)
                Return CSng(Math.Sqrt(ls))
            Else
                Dim ls As Single = CSng(X * X + Y * Y)
                Return CSng(Math.Sqrt(ls))
            End If
        End Function

        Public Function LengthSquared() As Double
            Return (X * X + Y * Y)
        End Function

        Public Shared Function Multiply(left As Statistics.Vector2, right As Double) As Statistics.Vector2
            Return New Statistics.Vector2(left.X * right, left.Y * right)
        End Function

        ''' <summary>
        ''' Returns the dot product of two vectors.
        ''' </summary>
        Public Shared Function Dot(value1 As Statistics.Vector2, value2 As Statistics.Vector2) As Single
            Return CSng(value1.X * value2.X + value1.Y * value2.Y)
        End Function

        Public Function Equals(other As Statistics.Vector2) As Boolean
            Return (Me.X = other.X) AndAlso (Me.Y = other.Y)
        End Function

#End Region '/METHODS

#Region "OPERATORS"

        Public Shared Operator +(left As Statistics.Vector2, right As Statistics.Vector2) As Statistics.Vector2
            Return New Statistics.Vector2(left.X + right.X, left.Y + right.Y)
        End Operator

        Public Shared Operator -(left As Statistics.Vector2, right As Statistics.Vector2) As Statistics.Vector2
            Return New Statistics.Vector2(left.X - right.X, left.Y - right.Y)
        End Operator

        ''' <summary>
        ''' Negates a given vector.
        ''' </summary>
        Public Shared Function Negate(value As Statistics.Vector2) As Statistics.Vector2
            Return (Zero - value)
        End Function

        Public Shared Operator *(left As Statistics.Vector2, right As Statistics.Vector2) As Statistics.Vector2
            Return New Statistics.Vector2(left.X * right.X, left.Y * right.Y)
        End Operator

        Public Shared Operator *(left As Statistics.Vector2, right As Single) As Statistics.Vector2
            Return left * New Statistics.Vector2(right, right)
        End Operator

        Public Shared Operator *(left As Single, right As Statistics.Vector2) As Statistics.Vector2
            Return New Statistics.Vector2(left, left) * right
        End Operator

        Public Shared Operator /(left As Statistics.Vector2, right As Statistics.Vector2) As Statistics.Vector2
            Return New Statistics.Vector2(left.X / right.X, left.Y / right.Y)
        End Operator

        Public Shared Operator /(value1 As Statistics.Vector2, value2 As Single) As Statistics.Vector2
            Dim invDiv As Single = 1 / value2
            Return New Statistics.Vector2(value1.X * invDiv, value1.Y * invDiv)
        End Operator

        ''' <summary>
        ''' Returns a boolean indicating whether the two given vectors are equal.
        ''' </summary>
        Public Shared Operator =(left As Statistics.Vector2, right As Statistics.Vector2) As Boolean
            Return left.Equals(right)
        End Operator

        ''' <summary>
        ''' Returns a boolean indicating whether the two given vectors are not equal.
        ''' </summary>
        Public Shared Operator <>(left As Statistics.Vector2, right As Statistics.Vector2) As Boolean
            Return Not (left = right)
        End Operator

#End Region '/OPERATORS

    End Structure

End Namespace