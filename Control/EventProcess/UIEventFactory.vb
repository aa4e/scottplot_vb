Namespace ScottPlot.Control.EventProcess

    ''' <summary>
    ''' This class takes details about interactions and builds them into event objects which can 
    ''' be passed into the event processor for processing/rendering when the render queue is free.
    ''' </summary>
    Public Class UIEventFactory

        Public ReadOnly Plot As Plot
        Public ReadOnly Configuration As Configuration
        Public ReadOnly Settings As Settings

#Region "CTOR"

        Public Sub New(config As Configuration, settings As Settings, plt As Plot)
            Me.Configuration = config
            Me.Settings = settings
            Me.Plot = plt
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function CreateBenchmarkToggle() As IUIEvent
            Return New Events.BenchmarkToggleEvent(Plot, Configuration)
        End Function

        Public Function CreateApplyZoomRectangleEvent(x As Single, y As Single) As IUIEvent
            Return New Events.ApplyZoomRectangleEvent(x, y, Configuration, Settings, Plot)
        End Function

        Public Function CreateMouseAutoAxis() As IUIEvent
            Return New Events.MouseAxisAutoEvent(Configuration, Settings, Plot)
        End Function

        Public Function CreateMouseMovedToZoomRectangle(x As Single, y As Single) As IUIEvent
            Return New Events.MouseMovedToZoomRectangle(x, y, Settings, Configuration)
        End Function

        Public Function CreateMousePan(input As InputState) As IUIEvent
            Return New Events.MousePanEvent(input, Configuration, Settings)
        End Function

        Public Function CreateMouseScroll(x As Single, y As Single, scroolUp As Boolean) As IUIEvent
            Return New Events.MouseScrollEvent(x, y, scroolUp, Configuration, Settings)
        End Function

        Public Function CreateMouseUpClearRender() As IUIEvent
            Return New Events.MouseUpClearRenderEvent(Configuration)
        End Function

        Public Function CreateMouseZoom(input As InputState) As IUIEvent
            Return New Events.MouseZoomEvent(input, Configuration, Settings, Plot)
        End Function

        Public Function CreatePlottableDrag(x As Single, y As Single, shiftDown As Boolean, draggable As Plottable.IDraggable) As IUIEvent
            Return New Events.PlottableDragEvent(x, y, shiftDown, draggable, Plot, Configuration)
        End Function

        Public Function CreateManualLowQualityRender() As IUIEvent
            Return New Events.RenderLowQuality()
        End Function

        Public Function CreateManualHighQualityRender() As IUIEvent
            Return New Events.RenderHighQuality()
        End Function

        Public Function CreateManualDelayedHighQualityRender() As IUIEvent
            Return New Events.RenderDelayedHighQuality()
        End Function

#End Region '/METHODS

    End Class

End Namespace