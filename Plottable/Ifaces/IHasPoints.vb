Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Indicates a plottable has data distributed along both axes and can return the X/Y location of the point nearest a given X/Y location.
    ''' </summary>
    Public Interface IHasPoints
        Inherits IHasPointsGeneric(Of Double, Double), IHasPointsGenericX(Of Double, Double), IHasPointsGenericY(Of Double, Double)

    End Interface

    ''' <summary>
    ''' Indicates a plottable has data distributed along both axes and can return the X/Y location of the point nearest a given X/Y location.
    ''' </summary>
    Public Interface IHasPointsGeneric(Of TX, TY)
        Inherits IHasPointsGenericX(Of TX, TY), IHasPointsGenericY(Of TX, TY)

        Function GetPointNearest(x As TX, y As TY, xyRatio As TX) As Tuple(Of TX, TY, Integer)

    End Interface

    ''' <summary>
    ''' Indicates a plottable has data distributed along the vertical axis and can return the X/Y location of the point nearest a given Y value.
    ''' </summary>
    Public Interface IHasPointsGenericY(Of TX, TY)

        Function GetPointNearestY(y As TY) As Tuple(Of TX, TY, Integer)

    End Interface

    ''' <summary>
    ''' Indicates a plottable has data distributed along the horizontal axis and can return the X/Y location of the point nearest a given X value.
    ''' </summary>
    Public Interface IHasPointsGenericX(Of TX, TY)

        Function GetPointNearestX(x As TX) As Tuple(Of TX, TY, Integer)
        Function GetYDataRange(xMin As TX, xMax As TX) As Tuple(Of TY, TY)

    End Interface

End Namespace