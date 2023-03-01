Namespace ScottPlot.Control

    Public Class Configuration

#Region "CTOR"

        Private ReadOnly Backend As ControlBackEnd

        Public Sub New(backend As ControlBackEnd)
            Me.Backend = backend
        End Sub

#End Region '/CTOR

#Region "EVENTS"

        ''' <summary>
        ''' Event is invoked whenever the display scale is changed.
        ''' </summary>
        Public Event ScaleChanged As EventHandler

#End Region '/EVENTS

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Control whether panning is enabled.
        ''' </summary>
        Public Property Pan As Boolean
            Get
                Return LeftClickDragPan
            End Get
            Set(value As Boolean)
                LeftClickDragPan = value
            End Set
        End Property

        ''' <summary>
        ''' Control whether zooming is enabled (via left-click-drag, middle-click-drag, and scrollwheel).
        ''' </summary>
        Public Property Zoom As Boolean
            Get
                Return RightClickDragZoom
            End Get
            Set(value As Boolean)
                RightClickDragZoom = value
                MiddleClickDragZoom = value
                ScrollWheelZoom = value
            End Set
        End Property

        ''' <summary>
        ''' Manual override to set anti-aliasing (high quality) behavior for all renders.
        ''' Refer to the <see cref="QualityConfiguration"/> field for more control over quality in response to specific interactions.
        ''' </summary>
        Public Quality As QualityMode = QualityMode.LowWhileDragging

        ''' <summary>
        ''' This module customizes anti-aliasing (high quality) behavior in response to interactive events.
        ''' </summary>
        Public QualityConfiguration As New QualityConfiguration()

        ''' <summary>
        ''' Control whether left-click-drag panning is enabled.
        ''' </summary>
        Public LeftClickDragPan As Boolean = True

        ''' <summary>
        ''' Control whether right-click-drag zooming is enabled.
        ''' </summary>
        Public RightClickDragZoom As Boolean = True

        ''' <summary>
        ''' Control whether scroll wheel zooming is enabled.
        ''' </summary>
        Public ScrollWheelZoom As Boolean = True

        ''' <summary>
        ''' Fractional amount to zoom in or out when the mouse wheel is scrolled.
        ''' Value must be between 0 and 1 (default is 0.15).
        ''' </summary>
        Public Property ScrollWheelZoomFraction As Double
            Get
                Return _ScrollWheelZoomFraction
            End Get
            Set(value As Double)
                If (value <= 0) Then
                    Throw New ArgumentOutOfRangeException(NameOf(ScrollWheelZoomFraction), "must be positive.")
                End If
                If (value >= 1) Then
                    Throw New ArgumentOutOfRangeException(NameOf(ScrollWheelZoomFraction), "must be less than 1.")
                End If
                _ScrollWheelZoomFraction = value
            End Set
        End Property
        Private _ScrollWheelZoomFraction As Double = 0.15

        ''' <summary>
        ''' Number of milliseconds after low quality scroll wheel zoom to re-render using high quality.
        ''' </summary>
        Public ScrollWheelZoomHighQualityDelay As Double = 500

        ''' <summary>
        ''' Control whether middle-click-drag zooming to a rectangle is enabled.
        ''' </summary>
        Public MiddleClickDragZoom As Boolean = True

        ''' <summary>
        ''' Control whether middle-click can be used to reset axis limits.
        ''' </summary>
        Public MiddleClickAutoAxis As Boolean = True

        ''' <summary>
        ''' Horizontal margin between the edge of the data and the edge of the plot when middle-click AutoAxis is called.
        ''' </summary>
        <Obsolete("Set default margins with Plot.Margins()", True)>
        Public MiddleClickAutoAxisMarginX As Double = 0.05

        ''' <summary>
        ''' Vertical margin between the edge of the data and the edge of the plot when middle-click AutoAxis is called.
        ''' </summary>
        <Obsolete("Set default margins with Plot.Margins()", True)>
        Public MiddleClickAutoAxisMarginY As Double = 0.1

        ''' <summary>
        ''' If enabled, double-clicking the plot will toggle benchmark visibility.
        ''' </summary>
        Public DoubleClickBenchmark As Boolean = True

        ''' <summary>
        ''' If enabled, the vertical axis limits cannot be modified by mouse actions.
        ''' </summary>
        Public LockVerticalAxis As Boolean = False

        ''' <summary>
        ''' If enabled, the horizontal axis limits cannot be modified by mouse actions.
        ''' </summary>
        Public LockHorizontalAxis As Boolean = False

        ''' <summary>
        ''' If enabled the control will automatically re-render as plottables are added and removed.
        ''' </summary>
        <Obsolete("Automatic render timer has been removed. Call Render() manually.", True)>
        Public RenderIfPlottableListChanges As Boolean = True

        ''' <summary>
        ''' Controls whether or not a render event will be triggered if a change in the axis limits is detected.
        ''' </summary>
        Public AxesChangedEventEnabled As Boolean = True

        ''' <summary>
        ''' Permitting dropped frames makes interactive mouse manipulation feel faster.
        ''' </summary>
        Public AllowDroppedFramesWhileDragging As Boolean = True

        ''' <summary>
        ''' If true, control interactions will be non-blocking and renders will occur after interactions.
        ''' If false, control interactions will be blocking while renders are drawn.
        ''' </summary>
        Public UseRenderQueue As Boolean

        ''' <summary>
        ''' Distance (in pixels) the mouse can travel with a button held-down for it to be treated as a click (not a drag).
        ''' A number slightly above zero allows middle-click to call AxisAuto() even if it was draged a few pixels by accident.
        ''' </summary>
        Public IgnoreMouseDragDistance As Integer = 5

        ''' <summary>
        ''' Now that the timer-based auto-render functionality has been removed users must manually call Render() at least once.
        ''' This option controls whether a warning message Is shown if the user did Not call Render() manually.
        ''' </summary>
        Public Property WarnIfRenderNotCalledManually As Boolean = True

        ''' <summary>
        ''' Control whether the plot should be stretched when DPI scaling Is in use.
        ''' Enabling stretching may result in blurry plots.
        ''' Disabling stretching may results in plots with text that Is too small.
        ''' </summary>
        Public Property DpiStretch As Boolean
            Get
                Return _DpiStretch
            End Get
            Set(value As Boolean)
                If (_DpiStretch <> value) Then
                    _DpiStretch = value
                    RaiseEvent ScaleChanged(Nothing, Nothing)
                End If
            End Set
        End Property
        Private _DpiStretch As Boolean = True

        ''' <summary>
        ''' DPI scaling ratio to use for plot size and mouse tracking.
        ''' Will return 1.0 if <see cref="DpiStretch"/> is enabled.
        ''' </summary>
        Public Property DpiStretchRatio As Single
            Get
                Return If(DpiStretch, 1.0F, _DpiStretchRatio)
            End Get
            Set(value As Single)
                If (_DpiStretchRatio <> value) Then
                    _DpiStretchRatio = value
                    RaiseEvent ScaleChanged(Nothing, Nothing)
                End If
            End Set
        End Property
        Private _DpiStretchRatio As Single = Drawing.GDI.GetScaleRatio()

        ''' <summary>
        ''' If true, controls that support the plot object editor will display an option to launch it in the right-click menu.
        ''' </summary>
        Public Property EnablePlotObjectEditor As Boolean = False

        ''' <summary>
        ''' Default cursor to use (when not hovering or dragging an interactive plottable).
        ''' </summary>
        Public Property DefaultCursor As Cursor = Cursor.Arrow

        ''' <summary>
        ''' Notify linked plots when axis, size, or layout of this plot changes.
        ''' Temporarially disable this when applying configuration from another linked plot to prevent an infinite circular update loop.
        ''' </summary>
        Public EmitLinkedControlUpdateSignals As Boolean = True

#End Region '/PROPS, FIELDS

#Region "METHODS"

        ''' <summary>
        ''' Set the <see cref="DpiStretchRatio"/> to that of the active display.
        ''' Call this if you suspect DPI scaling has changed.
        ''' </summary>
        Public Sub DpiMeasure()
            Me.DpiStretchRatio = Drawing.GDI.GetScaleRatio()
        End Sub

        Public Sub AddLinkedControl(plotControl As IPlotControl, Optional horizontal As Boolean = True, Optional vertical As Boolean = True, Optional layout As Boolean = True)
            Backend.AddLinkedControl(plotControl, horizontal, vertical, layout)
        End Sub

        Public Sub ClearLinkedControls()
            Backend.ClearLinkedControls()
        End Sub

#End Region '/METHODS

    End Class

End Namespace