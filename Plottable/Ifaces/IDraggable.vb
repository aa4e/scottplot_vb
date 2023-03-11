Namespace ScottPlot.Plottable

    Public Interface IDraggable

        Property DragEnabled As Boolean
        ReadOnly Property DragCursor As Cursor
        Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean
        Sub DragTo(coordinateX As Double, coordinateY As Double, fixedSize As Boolean)
        Property DragSnap As SnapLogic.ISnap2D
        Event Dragged As EventHandler

    End Interface

End Namespace