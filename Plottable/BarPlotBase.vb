Imports System.Drawing

Namespace ScottPlot.Plottable

    Public MustInherit Class BarPlotBase

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Font settings for labels drawn above the bars.
        ''' </summary>
        Public ReadOnly Font As New ScottPlot.Drawing.Font()

        Public Property IsVisible As Boolean = True
        Public Property XAxisIndex As Integer = 0
        Public Property YAxisIndex As Integer = 0

        ''' <summary>
        ''' Orientation of the bars. Default behavior is vertical so values are on the Y axis and positions are on the X axis.
        ''' </summary>
        Public Property Orientation As Orientation = Orientation.Vertical

        ''' <summary>
        ''' The position of each bar defines where the left edge of the bar should be.
        ''' To center the bar at each position, adjust this value to be negative one-half of the BarWidth.
        ''' </summary>
        Public Property PositionOffset As Double

        ''' <summary>
        ''' Size of each bar (along the axis defined by Orientation) relative to ValueBase.
        ''' </summary>
        Public Property Values As Double()

        ''' <summary>
        ''' Location of the left edge of each bar.
        ''' To center bars on these positions, adjust PositionOffset to be negative one-half of the BarWidth.
        ''' </summary>
        Public Property Positions As Double()

        ''' <summary>
        ''' This array defines the base of each bar.
        ''' Unless the user specifically defines it, this will be an array of zeros.
        ''' </summary>
        Public Property ValueOffsets As Double()

        ''' <summary>
        ''' If populated, this array describes the height of errorbars for each bar.
        ''' </summary>
        Public Property ValueErrors As Double()

        ''' <summary>
        ''' If true, errorbars will be drawn according to the values in the YErrors array.
        ''' </summary>
        Public Property ShowValuesAboveBars As Boolean

        ''' <summary>
        ''' Function to generate the strings placed above each bar based on its value.
        ''' </summary>
        Public Property ValueFormatter As Func(Of Double, String) = Function(x) $"{x}"

        ''' <summary>
        ''' Bars are drawn from this level and extend according to the sizes defined in Values[].
        ''' </summary>
        Public Property ValueBase As Double = 0

        ''' <summary>
        ''' Width of bars defined in axis units.
        ''' If bars are evenly spaced, consider setting this to a fraction of the distance between the first two Positions.
        ''' </summary>
        ''' <returns></returns>
        Public Property BarWidth As Double = 0.8

        ''' <summary>
        ''' Width of the errorbar caps defined in axis units.
        ''' </summary>
        Public Property ErrorCapSize As Double = 0.4

        ''' <summary>
        ''' Thickness of the errorbar lines (pixel units).
        ''' </summary>
        Public Property ErrorLineWidth As Single = 1

        ''' <summary>
        ''' Outline each bar with this color. Set this to transparent to disable outlines.
        ''' </summary>
        Public Property BorderColor As Color = Color.Black

        ''' <summary>
        ''' Color of errorbar lines.
        ''' </summary>
        Public Property ErrorColor As Color = Color.Black

        <Obsolete("Reference the 'Orientation' field instead of this field")>
        Public Property VerticalOrientation As Boolean
            Get
                Return (Orientation = Orientation.Vertical)
            End Get
            Set(value As Boolean)
                Orientation = If(value, Orientation.Vertical, Orientation.Horizontal)
            End Set
        End Property

        <Obsolete("Reference the 'Orientation' field instead of this field")>
        Public Property HorizontalOrientation As Boolean
            Get
                Return (Orientation = Orientation.Horizontal)
            End Get
            Set(value As Boolean)
                Orientation = If(value, Orientation.Horizontal, Orientation.Vertical)
            End Set
        End Property

        <Obsolete("Reference the 'Values' field instead of this field")>
        Public Property Ys As Double()
            Get
                Return Values
            End Get
            Set(value As Double())
                Values = value
            End Set
        End Property

        <Obsolete("Reference the 'Positions' field instead of this field")>
        Public Property Xs As Double()
            Get
                Return Positions
            End Get
            Set(value As Double())
                Positions = value
            End Set
        End Property

        <Obsolete("Reference the 'PositionOffset' field instead of this field", True)>
        Public Property XOffset As Double
            Get
                Return PositionOffset
            End Get
            Set(value As Double)
                PositionOffset = value
            End Set
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Protected Sub New()
            'Dim <>9__86_ As Func(Of Double, String) = BarPlotBase.<> c.<> 9__86_0
            'Dim valueFormatter As Func(Of Double, String) = <>9__86_
            'If <>9__86_ is Nothing Then
            '	Dim func As Func(Of Double, String) = Function(x As Double) String.Format("{0}", x)
            '	valueFormatter = func
            '	BarPlotBase.<>c.<>9__86_0 = func
            'End If
            'Me.ValueFormatter = valueFormatter
            'Me.BarWidth = 0.8
            'Me.ErrorCapSize = 0.4
            'Me.ErrorLineWidth = 1F
            'Me.BorderColor = Color.Black
            'Me.ErrorColor = Color.Black
            'Me.Font = New ScottPlot.Drawing.Font()
            'MyBase..ctor()
        End Sub

#End Region '/CTOR

#Region "METHODS"

        ''' <summary>
        ''' Replace the arrays used to define positions and values.
        ''' New error arrays will be created and filled with zeros.
        ''' </summary>
        Public Sub Replace(positions As Double(), values As Double())
            If (positions is Nothing) OrElse (values is Nothing) OrElse (positions.Length <> values.Length) Then
                Throw New ArgumentException()
            End If
            Me.Positions = positions
            Me.Values = values
            Me.ValueErrors = New Double(values.Length - 1) {}
            Me.ValueOffsets = New Double(values.Length - 1) {}
        End Sub

        Public Overridable Function GetAxisLimits() As AxisLimits
            Dim valueMin As Double = Double.PositiveInfinity
            Dim valueMax As Double = Double.NegativeInfinity
            Dim positionMin As Double = Double.PositiveInfinity
            Dim positionMax As Double = Double.NegativeInfinity

            For i As Integer = 0 To Me.Positions.Length - 1
                valueMin = Math.Min(Math.Min(valueMin, ValueOffsets(i) - ValueErrors(i) + Values(i)), ValueBase + ValueOffsets(i))
                valueMax = Math.Max(Math.Max(valueMax, ValueOffsets(i) + ValueErrors(i) + Values(i)), ValueBase + ValueOffsets(i))
                positionMin = Math.Min(positionMin, Positions(i))
                positionMax = Math.Max(positionMax, Positions(i))
            Next

            If ShowValuesAboveBars Then
                Dim span As Double = valueMax - valueMin
                If (valueMax > 0) Then
                    valueMax += span * 0.1
                End If
                If (valueMin < 0) Then
                    valueMin -= span * 0.1
                End If
            End If

            positionMin -= BarWidth / 2
            positionMax += BarWidth / 2

            positionMin += PositionOffset
            positionMax += PositionOffset

            If (Orientation = Orientation.Vertical) Then
                Return New AxisLimits(positionMin, positionMax, valueMin, valueMax)
            End If
            Return New AxisLimits(valueMin, valueMax, positionMin, positionMax)
        End Function

#End Region '/METHODS

    End Class

End Namespace