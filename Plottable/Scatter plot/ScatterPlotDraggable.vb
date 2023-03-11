Namespace ScottPlot.Plottable

    ''' <summary>
    ''' The scatter plot renders X/Y pairs as points and/or connected lines.
    ''' Scatter plots can be extremely slow for large datasets, so use Signal plots in these situations.
    ''' </summary>
    Public Class ScatterPlotDraggable
        Inherits ScatterPlot
        Implements IDraggable

        Public Overloads ReadOnly Property Xs As Double()
        Public Overloads ReadOnly Property Ys As Double()
        Public Property CurrentIndex As Integer = 0

        ''' <summary>
        ''' Indicates whether scatter points are draggable in user controls.
        ''' </summary>
        Public Property DragEnabled As Boolean = False Implements ScottPlot.Plottable.IDraggable.DragEnabled

        ''' <summary>
        ''' Indicates whether scatter points are horizontally draggable in user controls.
        ''' </summary>
        Public Property DragEnabledX As Boolean = True

        ''' <summary>
        ''' Indicates whether scatter points are vertically draggable in user controls.
        ''' </summary>
        Public Property DragEnabledY As Boolean = True

        ''' <summary>
        ''' Cursor to display while hovering over the scatter points if dragging is enabled.
        ''' </summary>
        Public Property DragCursor As Cursor = Cursor.Crosshair Implements ScottPlot.Plottable.IDraggable.DragCursor

        ''' <summary>
        ''' If dragging is enabled the points cannot be dragged more negative than this position.
        ''' </summary>
        Public Property DragXLimitMin As Double = Double.NegativeInfinity

        ''' <summary>
        ''' If dragging is enabled the points cannot be dragged more positive than this position.
        ''' </summary>
        Public Property DragXLimitMax As Double = Double.PositiveInfinity

        ''' <summary>
        ''' If dragging is enabled the points cannot be dragged more negative than this position.
        ''' </summary>
        Public Property DragYLimitMin As Double = Double.NegativeInfinity

        ''' <summary>
        ''' If dragging is enabled the points cannot be dragged more positive than this position.
        ''' </summary>
        Public Property DragYLimitMax As Double = Double.PositiveInfinity

        ''' <summary>
        ''' This event is invoked after the plot is dragged.
        ''' </summary>
        Public Event Dragged As EventHandler Implements ScottPlot.Plottable.IDraggable.Dragged

        ''' <summary>
        ''' This function applies snapping logic while dragging.
        ''' </summary>
        Public Property DragSnap As SnapLogic.ISnap2D = New SnapLogic.NoSnap2D() Implements ScottPlot.Plottable.IDraggable.DragSnap

        ''' <summary>
        ''' Move a scatter point to a New coordinate in plot space.
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
                If DragEnabledX Then Xs(CurrentIndex) = coordinateX
                If DragEnabledY Then Ys(CurrentIndex) = coordinateY

                RaiseEvent Dragged(Me, EventArgs.Empty)
            End If
        End Sub

        ''' <summary>
        ''' Return True if a scatter point is within a certain number of pixels (snap) to the mouse
        ''' </summary>
        ''' <param name="coordinateX">Mouse position (coordinate space).</param>
        ''' <param name="coordinateY">Mouse position (coordinate space).</param>
        ''' <param name="snapX">Snap distance (pixels).</param>
        ''' <param name="snapY">Snap distance (pixels).</param>
        Public Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean Implements ScottPlot.Plottable.IDraggable.IsUnderMouse
            Dim test As Boolean = False
            For i As Integer = 0 To MyBase.PointCount - 1
                test = (Math.Abs(Ys(i) - coordinateY) <= snapY) AndAlso (Math.Abs(Xs(i) - coordinateX) <= snapX)
                If test Then
                    CurrentIndex = i
                    Return test
                End If
            Next
            Return test
        End Function

        Public Sub New(xs As Double(), ys As Double(), Optional errorX As Double() = Nothing, Optional errorY As Double() = Nothing)
            MyBase.New(xs, ys, errorX, errorY)
        End Sub

    End Class

End Namespace