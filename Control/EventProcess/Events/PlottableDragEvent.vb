Namespace ScottPlot.Control.EventProcess.Events

    ''' <summary>
    ''' This event describes what happens when a draggable plottable (Like an axis line)
    ''' has been moved from its initial position. This event places the plottable of interest at the current mouse position.
    ''' This is typically called on MouseMove events while left-click-dragging a draggable plottable.
    ''' </summary>
    Public Class PlottableDragEvent
        Implements IUIEvent

        Private ReadOnly X As Single
        Private ReadOnly Y As Single
        Private ReadOnly PlottableBeingDragged As Plottable.IDraggable
        Private ReadOnly Plot As Plot
        Private ReadOnly ShiftDown As Boolean
        Private ReadOnly Configuration As Configuration

        Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
            Get
                Return Configuration.QualityConfiguration.MouseInteractiveDragged
            End Get
        End Property

        Public Sub New(x As Single, y As Single, shiftDown As Boolean, plottable As Plottable.IDraggable, plt As Plot, config As Configuration)
            Me.X = x
            Me.Y = y
            Me.ShiftDown = shiftDown
            Me.Plot = plt
            Me.PlottableBeingDragged = plottable
            Me.Configuration = config
        End Sub

        Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
            Dim coordX As Double = Plot.GetCoordinateX(X, CType(PlottableBeingDragged, Plottable.IPlottable).XAxisIndex)
            Dim coordY As Double = Plot.GetCoordinateY(Y, CType(PlottableBeingDragged, Plottable.IPlottable).YAxisIndex)
            PlottableBeingDragged.DragTo(coordX, coordY, ShiftDown)
        End Sub

    End Class

End Namespace