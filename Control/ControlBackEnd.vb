Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing

Namespace ScottPlot.Control

    ''' <summary>
    ''' The control back end module contains all the logic required to manage a mouse-interactive
    ''' plot to display in a user control. However, this module contains no control-specific dependencies.
    ''' User controls can instantiate this object, pass mouse And resize event information in, and have renders triggered using events.
    ''' </summary>
    ''' <remarks>
    ''' This file describes the ScottPlot back-end control module.
    '''
    ''' Goals for this module:
    '''   - Interact with the Plot object so controls don't have to.
    '''   - Wrap/abstract mouse interaction logic so controls don't have to implement it.
    '''   - Use events to tell controls when to update the image Or change the mouse cursor.
    '''   - Render calls are non-blocking so GUI/controls aren't slowed by render requests.
    '''   - Delayed high quality renders are possible after mouse interaction stops.
    '''  
    '''  Default Controls:
    '''  
    '''   - Left-click-drag: pan
    '''   - Right-click-drag: zoom
    '''   - Middle-click-drag: zoom region
    '''   - ALT+Left-click-drag: zoom region
    '''   - Scroll wheel: zoom to cursor
    '''  
    '''   - Right-click: show menu
    '''   - Middle-click: auto-axis
    '''   - Double-click: show benchmark
    '''  
    '''   - CTRL+Left-click-drag to pan horizontally
    '''   - SHIFT+Left-click-drag to pan vertically
    '''   - CTRL+Right-click-drag to zoom horizontally
    '''   - SHIFT+Right-click-drag to zoom vertically
    '''   - CTRL+SHIFT+Right-click-drag to zoom evenly
    '''   - SHIFT+click-drag draggables for fixed-size dragging
    '''   
    ''' Configurable options:
    ''' 
    '''   - left-click-drag pan
    '''   - right-click-drag zoom
    '''   - lock vertical Or horizontal axis
    '''   - middle-click auto-axis margin
    '''   - double-click benchmark toggle
    '''   - scrollwheel zoom
    '''   - quality (anti-aliasing on/off) for various actions
    '''   - delayed high quality render after low-quality interactive renders.
    ''' </remarks>
    Public Class ControlBackEnd

#Region "CTOR"

        Public Sub New()
            Configuration = New Configuration(Me)
            EventFactory = New EventProcess.UIEventFactory(Configuration, Settings, Plot)
            EventsProcessor = New EventProcess.EventsProcessor(Sub(lowQuality) Render(lowQuality), CInt(Configuration.ScrollWheelZoomHighQualityDelay))
            _Cursor = Configuration.DefaultCursor
            OldBitmaps = New Queue(Of Bitmap)
        End Sub

        ''' <summary>
        ''' Create a back-end for a user control.
        ''' </summary>
        ''' <param name="width">Initial bitmap size (pixels).</param>
        ''' <param name="height">Initial bitmap size (pixels).</param>
        ''' <param name="name">Variable name of the user control using this backend.</param>
        Public Sub New(width As Single, height As Single, Optional name As String = "UnnamedControl")
            Me.New()
            ControlName = name
            Reset(width, height)
        End Sub

#End Region '/CTOR

#Region "EVENTS"

        ''' <summary>
        ''' This Event is invoked when an existing Bitmap is redrawn.
        ''' E.g., after rendering following a click-drag-pan mouse Event.
        ''' </summary>
        Public Event BitmapUpdated As EventHandler

        ''' <summary>
        ''' This event is invoked after a new Bitmap was created.
        ''' E.g., after resizing the control, requiring a new Bitmap of a different size.
        ''' </summary>
        Public Event BitmapChanged As EventHandler

        ''' <summary>
        ''' This event is invoked when the cursor is supposed to change.
        ''' Cursor changes may be required when hovering over draggable plottable objects.
        ''' </summary>
        Public Event CursorChanged As EventHandler

        ''' <summary>
        ''' This event Is invoked when the axis limts change.
        ''' This is typically the result of a pan or zoom operation.
        ''' </summary>
        Public Event AxesChanged As EventHandler

        ''' <summary>
        ''' This event Is invoked when the user right-clicks the control with the mouse.
        ''' It is typically used to deploy a context menu.
        ''' </summary>
        Public Event RightClicked As EventHandler

        ''' <summary>
        ''' This event Is invoked when the user left-clicks the control with the mouse.
        ''' It is typically used to interact with custom plot types.
        ''' </summary>
        Public Event LeftClicked As EventHandler

        ''' <summary>
        ''' This event is invoked when the user left-clicks a plottable control with the mouse.
        ''' </summary>
        Public Event LeftClickedPlottable As EventHandler

        ''' <summary>
        ''' This event is invoked after the mouse moves while dragging a draggable plottable.
        ''' </summary>
        Public Event PlottableDragged As EventHandler

        ''' <summary>
        ''' This event is invoked after the mouse moves while dragging a draggable plottable.
        ''' </summary>
        Public Event PlottableDropped As EventHandler

#End Region '/EVENTS

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' The plot underlying this control.
        ''' </summary>
        Public ReadOnly Property Plot As Plot

        ''' <summary>
        ''' The style of cursor the control should display.
        ''' </summary>
        Public ReadOnly Property Cursor As Cursor

        ''' <summary>
        ''' Total number of renders performed.
        ''' Note that at least one render occurs before the first request to measure the layout And calculate data area.
        ''' This means the first render increments this number twice.
        ''' </summary>
        Public ReadOnly Property RenderCount As Integer = 0

        ''' <summary>
        ''' True if the mouse was dragged (with a button down) long enough to quality as a drag instead of a click.
        ''' </summary>
        Public ReadOnly Property MouseDownDragged As Boolean
            Get
                Return (MouseDownTravelDistance > Configuration.IgnoreMouseDragDistance)
            End Get
        End Property

        ''' <summary>
        ''' The control configuration object stores advanced customization And behavior settings for mouse-interactive plots.
        ''' </summary>
        Public ReadOnly Configuration As Configuration

        ''' <summary>
        ''' True if the middle mouse button is pressed.
        ''' </summary>
        Private IsMiddleDown As Boolean

        ''' <summary>
        ''' True if the right mouse button is pressed.
        ''' </summary>
        Private IsRightDown As Boolean

        ''' <summary>
        ''' True if the left mouse button is pressed.
        ''' </summary>
        Private IsLeftDown As Boolean

        ''' <summary>
        ''' Current position of the mouse in pixels.
        ''' </summary>
        Private MouseLocationX As Single

        ''' <summary>
        ''' Current position of the mouse in pixels.
        ''' </summary>
        Private MouseLocationY As Single

        ''' <summary>
        ''' Holds the plottable actively being dragged with the mouse. Contains null if no plottable is being dragged.
        ''' </summary>
        Private PlottableBeingDragged As Plottable.IDraggable

        ''' <summary>
        ''' True when a zoom rectangle is being drawn and the mouse button is still down.
        ''' </summary>
        Private IsZoomingRectangle As Boolean

        ''' <summary>
        ''' The settings object underlying the plot.
        ''' </summary>
        Private Settings As Settings

        ''' <summary>
        ''' The latest render is stored in this bitmap.
        ''' New renders may be performed on this existing bitmap.
        ''' When a new bitmap is created, this bitmap will be stored in <see cref="OldBitmaps"/> and eventually disposed.
        ''' </summary>
        Private Bmp As Bitmap

        ''' <summary>
        ''' Bitmaps that are created are stored here so they can be kept track of and disposed properly when new bitmaps are created.
        ''' </summary>
        Private OldBitmaps As Queue(Of Bitmap)

        ''' <summary>
        ''' Store last render limits so New renders can know whether the axis limits
        ''' have changed and decide whether to invoke the <see cref="AxesChanged"/> event or not.
        ''' </summary>
        Private LimitsOnLastRender As AxisLimits

        ''' <summary>
        ''' Unique identifier of the plottables list that was last rendered.
        ''' This value is used to determine if the plottables list was modified (requiring a re-render).
        ''' </summary>
        Private PlottablesIdentifierAtLastRender As Integer = 0

        ''' <summary>
        ''' This is set to true while the render loop is running.
        ''' This prevents multiple renders from occurring at the same ti
        ''' </summary>
        Private CurrentlyRendering As Boolean = False

        ''' <summary>
        ''' The events processor invokes renders in response to custom events.
        ''' </summary>
        Private EventsProcessor As EventProcess.EventsProcessor

        ''' <summary>
        ''' The event factor creates event objects to be handled by the event processor.
        ''' </summary>
        Private EventFactory As EventProcess.UIEventFactory

        ''' <summary>
        ''' Number of times the current bitmap has been rendered on.
        ''' </summary>
        Private BitmapRenderCount As Integer = 0

        ''' <summary>
        ''' Tracks the total distance the mouse was click-dragged (rectangular pixel units).
        ''' </summary>
        Private MouseDownTravelDistance As Single

        ''' <summary>
        ''' Indicates whether <see cref="Render"/> has been explicitly called by the user.
        ''' Renders requested by resize events do not count.
        ''' </summary>
        Public WasManuallyRendered As Boolean

        ''' <summary>
        ''' Variable name for the user control tied to this backend.
        ''' </summary>
        Public ControlName As String

        ''' <summary>
        ''' Plots whose axes And layout will be updated when this plot changes
        ''' </summary>
        Private ReadOnly LinkedPlotControls As New List(Of LinkedPlotControl)

#End Region '/PROPS, FIELDS

#Region "METHODS"

        ''' <summary>
        ''' The host control may instantiate the back-end And start sending it events
        ''' before it has fully connected its event handlers. To prevent processing events before
        ''' the host Is control Is ready, the processor will be stopped until Is called by the host control.
        ''' </summary>
        Public Sub StartProcessingEvents()
            EventsProcessor.Enable = True
        End Sub

        ''' <summary>
        ''' Reset the back-end by creating an entirely new plot of the given dimensions.
        ''' </summary>
        Public Sub Reset(width As Single, height As Single)
            Reset(width, height, New Plot(800, 600))
        End Sub

        ''' <summary>
        ''' Reset the back-end by replacing the existing plot with one that has already been created.
        ''' </summary>
        Public Sub Reset(width As Single, height As Single, newPlot As Plot)
            _Plot = newPlot
            Settings = Plot.GetSettings(False)
            EventFactory = New EventProcess.UIEventFactory(Configuration, Settings, Plot)
            WasManuallyRendered = False
            Resize(width, height, False)
        End Sub

        ''' <summary>
        ''' Return a multi-line string describing the default mouse interactions.
        ''' This can be useful for displaying a help message in a user control.
        ''' </summary>
        Public Shared Function GetHelpMessage() As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("Left-click-drag: pan")
            sb.AppendLine("Right-click-drag: zoom")
            sb.AppendLine("Middle-click-drag: zoom region")
            sb.AppendLine("ALT+Left-click-drag: zoom region")
            sb.AppendLine("Scroll wheel: zoom to cursor")
            sb.AppendLine("")
            sb.AppendLine("Right-click: show menu")
            sb.AppendLine("Middle-click: auto-axis")
            sb.AppendLine("Double-click: show benchmark")
            sb.AppendLine("")
            sb.AppendLine("CTRL+Left-click-drag to pan horizontally")
            sb.AppendLine("SHIFT+Left-click-drag to pan vertically")
            sb.AppendLine("CTRL+Right-click-drag to zoom horizontally")
            sb.AppendLine("SHIFT+Right-click-drag to zoom vertically")
            sb.AppendLine("CTRL+SHIFT+Right-click-drag to zoom evenly")
            sb.AppendLine("SHIFT+click-drag draggables for fixed-size dragging")
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Return the most recently rendered Bitmap. This method also disposes old Bitmaps if they exist.
        ''' </summary>
        Public Function GetLatestBitmap() As Bitmap
            While (OldBitmaps.Count > 3)
                OldBitmaps.Dequeue()?.Dispose()
            End While
            Return Bmp
        End Function

        ''' <summary>
        ''' Render onto the existing Bitmap. Quality describes whether anti-aliasing will be used.
        ''' </summary>
        Public Sub Render(Optional lowQuality As Boolean = False, Optional skipIfCurrentlyRendering As Boolean = False)
            If (Bmp Is Nothing) Then
                Return
            End If

            If (CurrentlyRendering AndAlso skipIfCurrentlyRendering) Then
                Return
            End If
            CurrentlyRendering = True

            Select Case Configuration.Quality
                Case QualityMode.High
                    lowQuality = False
                Case QualityMode.Low
                    lowQuality = True
            End Select

            Plot.Render(Bmp, lowQuality)
            BitmapRenderCount += 1
            _RenderCount += 1
            PlottablesIdentifierAtLastRender = Settings.PlottablesIdentifier

            If (WasManuallyRendered = False AndAlso Settings.Plottables.Count > 0 AndAlso Configuration.WarnIfRenderNotCalledManually AndAlso Debugger.IsAttached) Then
                Dim message As String = $"ScottPlot {Plot.Version} WARNING:{vbNewLine}{ControlName}.Refresh() must be called{vbNewLine}after modifying the plot or its data."
                Debug.WriteLine(message.Replace("\n", " "))
                AddErrorMessage(Bmp, message)
            End If

            Dim newLimits As AxisLimits = Plot.GetAxisLimits()
            If (Not newLimits.Equals(LimitsOnLastRender)) AndAlso Configuration.AxesChangedEventEnabled Then
                RaiseEvent AxesChanged(Nothing, EventArgs.Empty)
                UpdateLinkedPlotControls()
            End If

            LimitsOnLastRender = newLimits
            If (BitmapRenderCount = 1) Then
                RaiseEvent BitmapChanged(Me, EventArgs.Empty) 'a new bitmap was rendered on for the first time
            Else
                RaiseEvent BitmapUpdated(Nothing, EventArgs.Empty) 'an existing bitmap was re-rendered onto
            End If
            CurrentlyRendering = False
        End Sub

        ''' <summary>
        ''' Add error text on top of the rendered plot.
        ''' </summary>
        Private Shared Sub AddErrorMessage(bmp As Bitmap, message As String)
            Dim foreColor As Color = System.Drawing.Color.Red
            Dim backColor As Color = System.Drawing.Color.Yellow
            Dim padding As Integer = 10

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp),
                sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Middle),
                font As New System.Drawing.Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold)

                Dim messageSize As SizeF = gfx.MeasureString(message, font)
                Dim messageRect As New RectangleF(
                    CSng(bmp.Width / 2 - messageSize.Width / 2 - padding),
                    CSng(bmp.Height / 2 - messageSize.Height / 2 - padding),
                    messageSize.Width + padding * 2,
                    messageSize.Height + padding * 2)

                Using foreBrush As New SolidBrush(foreColor)
                    If (messageSize.Width > bmp.Width) OrElse (messageSize.Height > bmp.Height) Then
                        Dim plotRect As New RectangleF(0, 0, bmp.Width, bmp.Height)
                        sf.Alignment = StringAlignment.Near
                        sf.LineAlignment = StringAlignment.Near
                        message = message.Replace(vbNewLine, " ")

                        Using fontSmall As New System.Drawing.Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold)
                            gfx.Clear(backColor)
                            gfx.DrawString(message, fontSmall, foreBrush, plotRect, sf)
                        End Using
                    Else
                        Dim shadowColor As Color = Color.FromArgb(50, Color.Black)
                        Dim shadowOffset As Integer = 7
                        Dim shadowRect As New RectangleF(messageRect.X + shadowOffset, messageRect.Y + shadowOffset, messageRect.Width, messageRect.Height)
                        Using forePen As New Pen(foreColor, 5), backBrush As New SolidBrush(backColor), shadowBrush As New SolidBrush(shadowColor)
                            gfx.FillRectangle(shadowBrush, shadowRect)
                            gfx.FillRectangle(backBrush, messageRect)
                            gfx.DrawRectangle(forePen, Rectangle.Round(messageRect))
                            gfx.DrawString(message, font, foreBrush, messageRect, sf)
                        End Using
                    End If
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Request a render using the render queue. This method does not block the calling thread.
        ''' </summary>
        Public Sub RenderRequest(renderType As RenderType)
            Select Case renderType
                Case RenderType.LowQuality
                    ProcessEvent(EventFactory.CreateManualLowQualityRender())

                Case RenderType.HighQuality
                    ProcessEvent(EventFactory.CreateManualHighQualityRender())

                Case RenderType.HighQualityDelayed
                    ProcessEvent(EventFactory.CreateManualDelayedHighQualityRender())

                Case RenderType.LowQualityThenHighQuality
                    ProcessEvent(EventFactory.CreateManualLowQualityRender())
                    ProcessEvent(EventFactory.CreateManualHighQualityRender())

                Case RenderType.LowQualityThenHighQualityDelayed
                    ProcessEvent(EventFactory.CreateManualDelayedHighQualityRender())

                Case RenderType.ProcessMouseEventsOnly

                Case Else
                    Throw New InvalidOperationException($"Unsupported render type {renderType}.")
            End Select
        End Sub

        ''' <summary>
        ''' Check if the number of plottibles has changed And if so request a render.
        ''' This is typically called by a continuously running timer in the user control.
        ''' </summary>
        <Obsolete("Automatic render timer has been removed. Call Render() manually.", True)>
        Public Sub RenderIfPlottableListChanged()
            If (Not Configuration.RenderIfPlottableListChanges) Then
                Return
            End If

            If (Bmp Is Nothing) Then
                Return
            End If

            If (Settings.PlottablesIdentifier <> PlottablesIdentifierAtLastRender) Then
                Render()
            End If
        End Sub

        ''' <summary>
        ''' Resize the control (creates a new bitmap and requests a render).
        ''' </summary>
        ''' <param name="width">New width (pixels).</param>
        ''' <param name="height">New height (pixels).</param>
        ''' <param name="useDelayedRendering">Render using the queue (best for mouse events), otherwise render immediately.</param>
        Public Sub Resize(width As Single, height As Single, useDelayedRendering As Boolean)
            'don't render if the requested size cannot produce a valid bitmap
            If (width < 1) OrElse (height < 1) Then
                Return
            End If

            'don't render if the request is so early that the processor hasn't started
            If (EventsProcessor Is Nothing) Then
                Return
            End If

            ' Disposing a Bitmap the GUI is displaying will cause an exception.
            ' Keep track of old bitmaps so they can be disposed of later.
            OldBitmaps.Enqueue(Bmp)
            Bmp = New Bitmap(CInt(width), CInt(height))
            BitmapRenderCount = 0

            If useDelayedRendering Then
                RenderRequest(RenderType.HighQualityDelayed)
            Else
                Render()
            End If
        End Sub

        ''' <summary>
        ''' Indicate a mouse button has just been pressed.
        ''' </summary>
        Public Sub MouseDown(input As InputState)
            If (Not Settings.AllAxesHaveBeenSet) Then
                Plot.SetAxisLimits(Plot.GetAxisLimits())
            End If
            IsMiddleDown = input.MiddleWasJustPressed
            IsRightDown = input.RightWasJustPressed
            IsLeftDown = input.LeftWasJustPressed
            If IsLeftDown Then
                PlottableBeingDragged = Plot.GetDraggable(input.X, input.Y)
            Else
                PlottableBeingDragged = Nothing
            End If
            Settings.MouseDown(input.X, input.Y)
            MouseDownTravelDistance = 0
        End Sub

        ''' <summary>
        ''' Return the mouse position on the plot (in coordinate space) for the latest X and Y coordinates.
        ''' </summary>
        Public Function GetMouseCoordinates(Optional xAxisIndex As Integer = 0, Optional yAxisIndex As Integer = 0) As Tuple(Of Double, Double)
            Dim coordinate As Tuple(Of Double, Double) = Plot.GetCoordinate(MouseLocationX, MouseLocationY, xAxisIndex, yAxisIndex)
            Return New Tuple(Of Double, Double)(
                If(Double.IsNaN(coordinate.Item1), 0, coordinate.Item1),
                If(Double.IsNaN(coordinate.Item2), 0, coordinate.Item2))
        End Function

        ''' <summary>
        ''' Return the mouse position (in pixel space) for the last observed mouse position.
        ''' </summary>
        Public Function GetMousePixel() As Tuple(Of Single, Single)
            Return New Tuple(Of Single, Single)(MouseLocationX, MouseLocationY)
        End Function

        ''' <summary>
        ''' Indicate the mouse has moved to a new position.
        ''' </summary>
        Public Sub MouseMove(input As InputState)
            Dim isZoomingRectangleWithAltLeft As Boolean = IsLeftDown AndAlso input.AltDown
            Dim isZoomingRectangleWithMiddle As Boolean = IsMiddleDown AndAlso Configuration.MiddleClickDragZoom

            Dim wasZoomingRectangle As Boolean = IsZoomingRectangle
            IsZoomingRectangle = (isZoomingRectangleWithAltLeft OrElse isZoomingRectangleWithMiddle)

            If wasZoomingRectangle AndAlso (Not IsZoomingRectangle) Then
                Settings.ZoomRectangle.Clear()
            End If

            MouseDownTravelDistance += Math.Abs(input.X - MouseLocationX)
            MouseDownTravelDistance += Math.Abs(input.Y - MouseLocationY)

            MouseLocationX = input.X
            MouseLocationY = input.Y

            Dim mouseMoveEvent As EventProcess.IUIEvent = Nothing
            If (PlottableBeingDragged IsNot Nothing) Then
                mouseMoveEvent = EventFactory.CreatePlottableDrag(input.X, input.Y, input.ShiftDown, PlottableBeingDragged)

            ElseIf IsLeftDown AndAlso (Not input.AltDown) AndAlso Configuration.LeftClickDragPan Then
                mouseMoveEvent = EventFactory.CreateMousePan(input)

            ElseIf IsRightDown AndAlso Configuration.RightClickDragZoom Then
                mouseMoveEvent = EventFactory.CreateMouseZoom(input)

            ElseIf IsZoomingRectangle Then
                mouseMoveEvent = EventFactory.CreateMouseMovedToZoomRectangle(input.X, input.Y)
            End If

            If (IsRightDown OrElse IsMiddleDown) AndAlso (Cursor <> Configuration.DefaultCursor) Then
                'cursor was originally over a draggable but is now zooming
                _Cursor = Configuration.DefaultCursor
                RaiseEvent CursorChanged(Me, EventArgs.Empty)
            End If

            If (mouseMoveEvent IsNot Nothing) Then
                ProcessEvent(mouseMoveEvent)
            Else
                MouseMovedWithoutInteraction(input)
            End If
        End Sub

        ''' <summary>
        ''' Process an event using the render queue (non-blocking) or traditional rendering (blocking) based on the <see cref="UseRenderQueue"/> flag in the <see cref="Configuration"/> module.
        ''' </summary>
        Private Sub ProcessEvent(uiEvent As EventProcess.IUIEvent)
            If Configuration.UseRenderQueue Then
                'TODO: refactor to better support async
                'TODO: document that draggable events aren't supported by the render queue
                EventsProcessor.ProcessAsync(uiEvent)
            Else
                uiEvent.ProcessEvent()

                If (uiEvent.RenderType = RenderType.ProcessMouseEventsOnly) Then
                    Return
                End If

                Dim uiEventType As Type = uiEvent.GetType()

                Dim lowQuality As Boolean = TypeOf uiEvent Is EventProcess.Events.MouseMovedToZoomRectangle _
                    OrElse TypeOf uiEvent Is EventProcess.Events.MousePanEvent _
                    OrElse TypeOf uiEvent Is EventProcess.Events.MouseZoomEvent _
                    OrElse TypeOf uiEvent Is EventProcess.Events.PlottableDragEvent _
                    OrElse TypeOf uiEvent Is EventProcess.Events.RenderLowQuality 'TEST 

                Dim allowSkip As Boolean = lowQuality AndAlso Configuration.AllowDroppedFramesWhileDragging

                Render(lowQuality, allowSkip)

                If (TypeOf uiEvent Is EventProcess.Events.PlottableDragEvent) Then 'TEST 
                    RaiseEvent PlottableDragged(PlottableBeingDragged, EventArgs.Empty)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Call this when the mouse moves without any buttons being down.
        ''' It will only update the cursor based on what's beneath the cursor.
        ''' </summary>
        Private Sub MouseMovedWithoutInteraction(input As InputState)
            UpdateCursor(input)
        End Sub

        ''' <summary>
        ''' Set the cursor based on whether a draggable is engaged or not, then invoke the <see cref="CursorChanged"/> event.
        ''' </summary>
        Private Sub UpdateCursor(input As InputState)
            Dim newCursor As Cursor = Configuration.DefaultCursor
            Dim draggableUnderCursor As Plottable.IDraggable = Plot.GetDraggable(input.X, input.Y)
            If (draggableUnderCursor IsNot Nothing) Then
                newCursor = draggableUnderCursor.DragCursor
            End If

            Dim hittable As Plottable.IPlottable = Plot.GetHittable(input.X, input.Y)
            If (hittable IsNot Nothing) Then
                newCursor = CType(hittable, Plottable.IHittable).HitCursor
            End If

            If (newCursor <> newCursor) Then
                newCursor = newCursor
                RaiseEvent CursorChanged(Nothing, EventArgs.Empty)
            End If
        End Sub

        ''' <summary>
        ''' Indicate a mouse button has been released. This may initiate a render (and/or a delayed render).
        ''' </summary>
        Public Sub MouseUp(input As InputState)
            Dim droppedPlottable As Plottable.IDraggable = PlottableBeingDragged

            Dim uiEvent As EventProcess.IUIEvent
            If IsZoomingRectangle AndAlso MouseDownDragged AndAlso Configuration.MiddleClickDragZoom Then
                uiEvent = EventFactory.CreateApplyZoomRectangleEvent(input.X, input.Y)
            ElseIf IsMiddleDown AndAlso Configuration.MiddleClickAutoAxis AndAlso Not MouseDownDragged Then
                uiEvent = EventFactory.CreateMouseAutoAxis()
            Else
                uiEvent = EventFactory.CreateMouseUpClearRender()
            End If
            ProcessEvent(uiEvent)

            If IsRightDown AndAlso (Not MouseDownDragged) Then
                RaiseEvent RightClicked(Nothing, EventArgs.Empty)
            End If

            If IsLeftDown AndAlso (Not MouseDownDragged) Then
                RaiseEvent LeftClicked(Nothing, EventArgs.Empty)

                Dim plottableHit As Plottable.IPlottable = Plot.GetHittable(input.X, input.Y)
                If (plottableHit IsNot Nothing) Then
                    RaiseEvent LeftClickedPlottable(plottableHit, EventArgs.Empty)
                End If
            End If

            IsMiddleDown = False
            IsRightDown = False
            IsLeftDown = False

            UpdateCursor(input)

            If (droppedPlottable IsNot Nothing) Then
                RaiseEvent PlottableDropped(droppedPlottable, EventArgs.Empty)
            End If

            PlottableBeingDragged = Nothing
            If (droppedPlottable IsNot Nothing) Then
                ProcessEvent(EventFactory.CreateMouseUpClearRender())
            End If
        End Sub

        ''' <summary>
        ''' Indicate the left mouse button has been double-clicked. 
        ''' The default action of a double-click Is to toggle the benchmark.
        ''' </summary>
        Public Sub DoubleClick()
            If Configuration.DoubleClickBenchmark Then
                Dim mouseEvent As EventProcess.IUIEvent = EventFactory.CreateBenchmarkToggle()
                ProcessEvent(mouseEvent)
            End If
        End Sub

        ''' <summary>
        ''' Apply a scroll wheel action, perform a low quality render, and later re-render in high quality.
        ''' </summary>
        Public Sub MouseWheel(input As InputState)
            If (Not Settings.AllAxesHaveBeenSet) Then
                Plot.SetAxisLimits(Plot.GetAxisLimits())
            End If

            If Configuration.ScrollWheelZoom Then
                Dim mouseEvent As EventProcess.IUIEvent = EventFactory.CreateMouseScroll(input.X, input.Y, input.WheelScrolledUp)
                ProcessEvent(mouseEvent)
            End If
        End Sub

        ''' <summary>
        ''' Add a plot which will have its axes And layout updated when this plot changes
        ''' </summary>
        Public Sub AddLinkedControl(otherPlot As IPlotControl, Optional horizontal As Boolean = True, Optional vertical As Boolean = True, Optional layout As Boolean = True)
            Dim linkedControl As New LinkedPlotControl(otherPlot, horizontal, vertical, layout)
            LinkedPlotControls.Add(linkedControl)
            UpdateLinkedPlotControls()
        End Sub

        Public Sub ClearLinkedControls()
            LinkedPlotControls.Clear()
        End Sub

        Private Sub UpdateLinkedPlotControls()
            If (Not Configuration.EmitLinkedControlUpdateSignals) Then
                Return
            End If

            For Each linkedPlotControl In LinkedPlotControls
                linkedPlotControl.PlotControl.Configuration.EmitLinkedControlUpdateSignals = False

                linkedPlotControl.PlotControl.Plot.MatchAxis(Me.Plot, linkedPlotControl.LinkHorizontal, linkedPlotControl.LinkVertical)

                If (linkedPlotControl.LinkLayout) Then
                    linkedPlotControl.PlotControl.Plot.MatchLayout(Me.Plot, linkedPlotControl.LinkHorizontal, linkedPlotControl.LinkVertical)
                End If

                linkedPlotControl.PlotControl.Refresh()
                linkedPlotControl.PlotControl.Configuration.EmitLinkedControlUpdateSignals = True
            Next
        End Sub

#End Region '/METHODS

    End Class

End Namespace