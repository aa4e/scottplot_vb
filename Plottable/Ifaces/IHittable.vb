Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Describes a Plottable that can report whether or not it is beneath the mouse cursor.
    ''' </summary>
    Public Interface IHittable

        ''' <summary>
        ''' Cursor to display when the Plottable is under the mouse.
        ''' </summary>
        Property HitCursor As Cursor

        ''' <summary>
        ''' Returns true if the Plottable is at the given coordinate.
        ''' </summary>
        Function HitTest(coord As Coordinate) As Boolean

        ''' <summary>
        ''' Controls whether logic inside <see cref="HitTest(Coordinate)"/> will run (or always return false).
        ''' </summary>
        Property HitTestEnabled As Boolean

    End Interface

End Namespace