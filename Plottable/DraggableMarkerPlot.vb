Namespace ScottPlot.Plottable

    ''' <summary>
    ''' This plot type displays a marker at a point that can be dragged with the mouse.
    ''' </summary>
    Public Class DraggableMarkerPlot
        Inherits MarkerPlot
        Implements IDraggable, IHasMarker, IHasColor

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Indicates whether this marker is draggable in user controls.
        ''' </summary>
        Public Property DragEnabled As Boolean = True Implements ScottPlot.Plottable.IDraggable.DragEnabled

        ''' <summary>
        ''' Cursor to display while hovering over this marker if dragging is enabled.
        ''' </summary>
        Public Property DragCursor As Cursor = Cursor.Hand Implements ScottPlot.Plottable.IDraggable.DragCursor

        ''' <summary>
        ''' If dragging is enabled the marker cannot be dragged more negative than this position.
        ''' </summary>
        Public Property DragXLimitMin As Double = Double.NegativeInfinity

        ''' <summary>
        ''' If dragging is enabled the marker cannot be dragged more positive than this position.
        ''' </summary>
        Public Property DragXLimitMax As Double = Double.PositiveInfinity

        ''' <summary>
        ''' If dragging is enabled the marker cannot be dragged more negative than this position.
        ''' </summary>
        Public Property DragYLimitMin As Double = Double.NegativeInfinity

        ''' <summary>
        ''' If dragging is enabled the marker cannot be dragged more positive than this position.
        ''' </summary>
        Public Property DragYLimitMax As Double = Double.PositiveInfinity

        ''' <summary>
        ''' This function applies snapping logic while dragging.
        ''' </summary>
        Public Property DragSnap As SnapLogic.ISnap2D = New SnapLogic.NoSnap2D() Implements ScottPlot.Plottable.IDraggable.DragSnap

        ''' <summary>
        ''' This event is invoked after the marker is dragged.
        ''' </summary>
        Public Event Dragged As EventHandler Implements ScottPlot.Plottable.IDraggable.Dragged

#End Region '/PROPS, FIELDS

#Region "METHODS"

        ''' <summary>
        ''' Move the marker to a New coordinate in plot space.
        ''' </summary>
        ''' <param name="coordinateX">New X position.</param>
        ''' <param name="coordinateY">New Y position.</param>
        ''' <param name="fixedSize">This argument is ignored.</param>
        Public Sub DragTo(coordinateX As Double, coordinateY As Double, fixedSize As Boolean) Implements ScottPlot.Plottable.IDraggable.DragTo
            If DragEnabled Then
                Dim original As New Coordinate(coordinateX, coordinateY)
                Dim snapped As Coordinate = DragSnap.Snap(original)
                coordinateX = snapped.X
                coordinateY = snapped.Y

                If (coordinateX < DragXLimitMin) Then coordinateX = DragXLimitMin
                If (coordinateX > DragXLimitMax) Then coordinateX = DragXLimitMax
                If (coordinateY < DragYLimitMin) Then coordinateY = DragYLimitMin
                If (coordinateY > DragYLimitMax) Then coordinateY = DragYLimitMax
                X = coordinateX
                Y = coordinateY
                RaiseEvent Dragged(Me, EventArgs.Empty)
            End If
        End Sub

        ''' <summary>
        ''' Return True if the marker is within a certain number of pixels (snap) to the mouse.
        ''' </summary>
        ''' <param name="coordinateX">Mouse position (coordinate space).</param>
        ''' <param name="coordinateY">Mouse position (coordinate space).</param>
        ''' <param name="snapX">Snap distance (pixels).</param>
        ''' <param name="snapY">Snap distance (pixels).</param>
        Public Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean Implements ScottPlot.Plottable.IDraggable.IsUnderMouse
            Return (Math.Abs(Y - coordinateY) <= snapY) AndAlso (Math.Abs(X - coordinateX) <= snapX)
        End Function

#End Region '/METHODS

    End Class

End Namespace