Namespace ScottPlot

    ''' <summary>
    ''' This class holds open/high/low/close (OHLC) price data over a time range.
    ''' </summary>
    Public Class OHLC

#Region "PROPS, FIELDS"

        Public Open As Double
        Public High As Double
        Public Low As Double
        Public Close As Double
        Public Volume As Double
        Public DateTime As DateTime
        Public TimeSpan As TimeSpan

        Public ReadOnly Property IsValid As Boolean
            Get
                Return (Not IsNanOrInfinity(Open)) _
                    AndAlso (Not IsNanOrInfinity(High)) _
                    AndAlso (Not IsNanOrInfinity(Low)) _
                    AndAlso (Not IsNanOrInfinity(Close)) _
                    AndAlso (Not IsNanOrInfinity(Volume))
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTORs"

        ''' <summary>
        ''' OHLC price over a specific period of time.
        ''' </summary>
        ''' <param name="open">Opening price.</param>
        ''' <param name="high">Maximum price.</param>
        ''' <param name="low">Minimum price.</param>
        ''' <param name="close">Closing price</param>
        ''' <param name="timeStart">Open time.</param>
        ''' <param name="timeSpan">Width of the OHLC.</param>
        ''' <param name="volume">Transaction volume for this time span.</param>
        Public Sub New(open As Double, high As Double, low As Double, close As Double, timeStart As DateTime, timeSpan As TimeSpan, Optional volume As Double = 0.0)
            Me.Open = open
            Me.High = high
            Me.Low = low
            Me.Close = close
            Me.DateTime = timeStart
            Me.TimeSpan = timeSpan
            Me.Volume = volume
        End Sub

        ''' <summary>
        ''' OHLC price over a specific period of time.
        ''' </summary>
        ''' <param name="open">Opening price.</param>
        ''' <param name="high">Maximum price.</param>
        ''' <param name="low">Minimum price.</param>
        ''' <param name="close">Closing price.</param>
        ''' <param name="timeStart">Open time (DateTime.ToOADate() units).</param>
        ''' <param name="days">Width of the OHLC in days.</param>
        ''' <param name="volume">Transaction volume for this time span.</param>
        Public Sub New(open As Double, high As Double, low As Double, close As Double, timeStart As Double, Optional days As Double = 1, Optional volume As Double = 0.0)
            Me.Open = open
            Me.High = high
            Me.Low = low
            Me.Close = close
            Me.DateTime = DateTime.FromOADate(timeStart)
            Me.TimeSpan = TimeSpan.FromDays(days)
            Me.Volume = volume
        End Sub

#End Region '/CTORs

#Region "METHODS"

        Private Function IsNanOrInfinity(val As Double) As Boolean
            Return (Double.IsInfinity(val) OrElse Double.IsNaN(val))
        End Function

        Public Overrides Function ToString() As String
            Return $"OHLC: open={Open}, high={High}, low={Low}, close={Close}, start={DateTime}, span={TimeSpan}, volume={Volume}"
        End Function

#End Region '/METHODS

    End Class

End Namespace