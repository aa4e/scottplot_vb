Namespace ScottPlot.Control

    ''' <summary>
    ''' This class detects And stores display scale.
    ''' The scale ratio is used to calculate plot size and translate mouse coordinates to real pixel coordinates.
    ''' </summary>
    Public Class DisplayScale

        ''' <summary>
        ''' This event is invoked whenever the display scale is changed.
        ''' </summary>
        Public Event ScaleChanged As EventHandler

        Public Sub New()
            Measure()
        End Sub

        ''' <summary>
        ''' Scale ratio in use by the active display. This ratio is used when scaling is enabled.
        ''' </summary>
        Public ReadOnly Property SystemScaleRatio As Single = 1

        ''' <summary>
        ''' Scale ratio to use if scaling is disabled.
        ''' </summary>
        Public ReadOnly Property ManualScaleRatio As Single = 1

        ''' <summary>
        ''' Control whether the plot bitmap should be stretched if display scaling is active.
        ''' When enabled text will be large but may be blurry.
        ''' When disabled text will be sharp but may be too small to read on high-resolution displays.
        ''' </summary>
        Public Property Enabled As Boolean
            Get
                Return _Enabled
            End Get
            Set(value As Boolean)
                If (value <> _Enabled) Then
                    _Enabled = value
                    RaiseEvent ScaleChanged(Me, EventArgs.Empty)
                End If
            End Set
        End Property
        Private _Enabled As Boolean = True

        ''' <summary>
        ''' Current display scale ratio. 
        ''' </summary>
        Public ReadOnly Property ScaleRatio As Single
            Get
                Return If(Enabled, SystemScaleRatio, ManualScaleRatio)
            End Get
        End Property

        ''' <summary>
        ''' Update the scale ratio using that of the active display. Call this method if you expect the display scale has changed.
        ''' </summary>
        Public Sub Measure()
            Dim ratio As Double = Drawing.GDI.GetScaleRatio()
            If (SystemScaleRatio <> ratio) Then
                _SystemScaleRatio = CSng(ratio)
                RaiseEvent ScaleChanged(Me, EventArgs.Empty)
            End If
        End Sub

    End Class

End Namespace