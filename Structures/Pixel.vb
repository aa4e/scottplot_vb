Imports System.Drawing

Namespace ScottPlot

    ''' <summary>
    ''' Describes an X/Y position in pixel space.
    ''' </summary>
    Public Structure Pixel

#Region "CTOR"

        Public X As Single
        Public Y As Single

        Public Sub New(x As Single, y As Single)
            Me.X = x
            Me.Y = y
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"[X={X}, Y={Y}]"
        End Function

        ''' <summary>
        ''' True as lone as neither coordinate is NaN or Infinity.
        ''' </summary>
        Public Function IsFinite() As Boolean
            Return (Not Double.IsNaN(X)) _
                AndAlso (Not Double.IsNaN(Y)) _
                AndAlso (Not Double.IsInfinity(X)) _
                AndAlso (Not Double.IsInfinity(Y))
        End Function

        ''' <summary>
        ''' Return the distance to another pixel (in pixel units).
        ''' </summary>
        Public Function Distance(other As Pixel) As Single
            Dim dX As Double = Math.Abs(other.X - X)
            Dim dY As Double = Math.Abs(other.Y - Y)
            Return CSng(Math.Sqrt(dX * dX + dY * dY))
        End Function

        ''' <summary>
        ''' Shift the pixel location by the given deltas.
        ''' </summary>
        Public Sub Translate(deltaX As Single, deltaY As Single)
            X += deltaX
            Y += deltaY
        End Sub

        ''' <summary>
        ''' Return a new pixel translated by the given deltas.
        ''' </summary>
        Public Function WithTranslation(deltaX As Single, deltaY As Single) As Pixel
            Return New Pixel(X + deltaX, Y + deltaY)
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            'If obj.GetType().Equals(GetType(Pixel)) Then
            '    Dim px As Pixel = CType(obj, Pixel)
            '    Return (X = px.X) AndAlso (Y = px.Y)
            'Else
            '    Return False
            'End If

            If (TypeOf obj is Pixel) Then 'TEST 
                Dim px As Pixel = CType(obj, Pixel)
                Return (X = px.X) AndAlso (Y = px.Y)
            End If
            Return False
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim x As Integer = BitConverter.ToInt32(BitConverter.GetBytes(Me.X), 0)
            Dim y As Integer = BitConverter.ToInt32(BitConverter.GetBytes(Me.Y), 0)
            Return x * 12345 + y
        End Function

        Public Function ToPointF() As PointF
            Return New PointF(X, Y)
        End Function

        Public Shared Function Clamp(value As Single, min As Single, max As Single) As Single
            If (min > max) Then
                Throw New ArgumentException($"{NameOf(min)} must be <= {NameOf(max)}.")
            End If
            If (value < min) Then
                Return min
            ElseIf (value > max) Then
                Return max
            Else
                Return value
            End If
        End Function

        Public Function Clamp(rect As PixelRect) As Pixel
            Dim x As Single = Pixel.Clamp(Me.X, rect.X, rect.X + rect.Width)
            Dim y As Single = Pixel.Clamp(Me.Y, rect.Y, rect.Y + rect.Height)
            Return New Pixel(x, y)
        End Function

#End Region '/METHODS

#Region "OPERATORS"

        Public Shared Operator =(a As Pixel, b As Pixel) As Boolean
            Return (a.X = b.X) AndAlso (a.Y = b.Y)
        End Operator

        Public Shared Operator <>(a As Pixel, b As Pixel) As Boolean
            Return (a.X <> b.X) OrElse (a.Y <> b.Y)
        End Operator

#End Region '/OPERATORS

    End Structure

End Namespace