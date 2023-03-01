Namespace ScottPlot.Control.EventProcess.Events

    ''' <summary>
    ''' This event Is called after the mouse button is lifted (typically following panning and zooming).
    ''' It assumes all the axis manipulation (panning/zooming) has already been performed,
    ''' and the purpose of this event Is only to request an immediate high quality render.
    ''' </summary>
    Public Class MouseUpClearRenderEvent
        Implements IUIEvent

        Private ReadOnly Configuration As Configuration

        Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
            Get
                If (Configuration IsNot Nothing) Then
                    Return Configuration.QualityConfiguration.MouseInteractiveDropped
                End If
                Return RenderType.LowQuality
            End Get
        End Property

        Public Sub New(config As Configuration)
            Me.Configuration = config
        End Sub

        Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
        End Sub

    End Class

End Namespace