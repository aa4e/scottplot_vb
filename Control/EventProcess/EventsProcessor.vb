Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Threading.Tasks

Namespace ScottPlot.Control.EventProcess

    ''' <summary>
    ''' This event processor process incoming events And invokes renders as needed.
    ''' This class contains logic to optionally display a fast preview render and a delayed high quality render.
    ''' </summary>
    Public Class EventsProcessor

#Region "CTOR"

        ''' <summary>
        ''' Create a processor to invoke renders in response to incoming events
        ''' </summary>
        ''' <param name="renderAction">Action to invoke to perform a render. Bool argument is <see cref="RenderType.LowQuality"/>.</param>
        ''' <param name="renderDelay">Milliseconds after low-quality render to re-render using high quality.</param>
        Public Sub New(renderAction As Action(Of Boolean), renderDelay As Integer)
            Me.RenderAction = renderAction
            Me.RenderDelayMilliseconds = renderDelay
        End Sub

#End Region '/CTOR

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' List of events that have not yet been processed.
        ''' </summary>
        Private ReadOnly Queue As New Queue(Of IUIEvent)()

        ''' <summary>
        ''' This timer is used for delayed rendering.
        ''' It is restarted whenever an event is processed which requests a delayed render.
        ''' </summary>
        Private ReadOnly RenderDelayTimer As New Stopwatch()

        ''' <summary>
        ''' This is true while the processor is processing events and/or waiting for a delayed render.
        ''' </summary>
        Private QueueProcessorIsRunning As Boolean

        ''' <summary>
        ''' The event processor loop will hang infinitely until this is set to True.
        ''' </summary>
        Public Enable As Boolean

        ''' <summary>
        ''' Time to wait after a low-quality render to invoke a high quality render.
        ''' </summary>
        Public Property RenderDelayMilliseconds As Integer

        ''' <summary>
        ''' When a render is required this Action will be invoked.
        ''' Its argument indicates whether low quality should be used.
        ''' </summary>
        Public Property RenderAction As Action(Of Boolean)

#End Region '/PROPS, FIELDS

#Region "METHODS"

        ''' <summary>
        ''' Perform a high quality render.
        ''' Call this instead of the action itself because this has better-documented arguments.
        ''' </summary>
        Private Sub RenderHighQuality()
            RenderAction?.Invoke(False)
        End Sub

        ''' <summary>
        ''' Perform a low quality render.
        ''' Call this instead of the action itself because this has better-documented arguments.
        ''' </summary>
        Private Sub RenderLowQuality()
            RenderAction?.Invoke(True)
        End Sub

        ''' <summary>
        ''' Add an event to the queue And process it when it is ready.
        ''' After all events are processed a render will be called automatically by the queue processor.
        ''' </summary>
        Public Async Function ProcessAsync(uiEvent As IUIEvent) As Task
            Queue.Enqueue(uiEvent)

            ' start a new processor only if one is not already running
            If (Not QueueProcessorIsRunning) Then
                Await RunQueueProcessor()
            End If
        End Function

        ''' <summary>
        ''' Perform a low quality preview render if the render type allows it.
        ''' </summary>
        Private Sub RenderPreview(renderType As RenderType)
            If (renderType = RenderType.HighQuality) Then
                Return
            End If
            RenderLowQuality()
        End Sub

        ''' <summary>
        ''' Perform a final high quality render if the render type allows it.
        ''' </summary>
        ''' <returns>Return False if the final render needs to happen later.</returns>
        Private Function RenderFinal(renderType As RenderType) As Boolean
            Select Case renderType
                Case RenderType.LowQuality
                    Return True ' we don't need a HQ render if the type is LQ only

                Case RenderType.HighQuality
                    RenderHighQuality() 'a HQ version is always needed
                    Return True

                Case RenderType.LowQualityThenHighQuality
                    RenderHighQuality() 'a LQ version has been rendered, but we need a HQ version now
                    Return True

                Case RenderType.LowQualityThenHighQualityDelayed
                    If RenderDelayTimer.IsRunning Then 'a LQ version has been rendered, but we need a HQ version after a delay
                        RenderDelayTimer.Restart()
                    End If

                    If (RenderDelayTimer.ElapsedMilliseconds > RenderDelayMilliseconds) Then
                        RenderHighQuality()
                        RenderDelayTimer.Stop()
                        Return True
                    Else
                        Return False
                    End If
            End Select
            Throw New ArgumentException($"Unknown quality: {renderType}.")
        End Function

        ''' <summary>
        ''' Process every event in the queue.
        ''' A render will be executed after each event is processed.
        ''' A slight delay will be added between queue checks.
        ''' </summary>
        Private Async Function RunQueueProcessor() As Task
            While (Not Enable)
                Await Task.Delay(1)
            End While

            Dim lastEventRenderType As RenderType = RenderType.ProcessMouseEventsOnly
            While True
                QueueProcessorIsRunning = True
                Dim eventRenderRequested As Boolean = False
                While (Queue.Count > 0)
                    Dim uiEvent = Queue.Dequeue()
                    uiEvent.ProcessEvent()

                    If (uiEvent.RenderType = RenderType.LowQualityThenHighQualityDelayed) Then
                        RenderDelayTimer.Restart()
                    End If

                    If (uiEvent.RenderType <> RenderType.ProcessMouseEventsOnly) Then
                        lastEventRenderType = uiEvent.RenderType
                        eventRenderRequested = True
                    End If
                End While

                If eventRenderRequested Then
                    RenderPreview(lastEventRenderType)
                End If

                Await Task.Delay(1)

                ' if all events were processed, consider performing a final render And exiting
                If (Queue.Count = 0) Then
                    Dim finalRenderExecuted = RenderFinal(lastEventRenderType)
                    If finalRenderExecuted Then
                        Exit While
                    End If
                End If
            End While

            QueueProcessorIsRunning = False
        End Function

#End Region '/METHODS

    End Class

End Namespace