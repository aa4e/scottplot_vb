Namespace ScottPlot

    ''' <summary>
    ''' Defines if/how axis scales (units per pixel) are matched between horizontal and vertical axes.
    ''' </summary>
    Public Enum EqualScaleMode
        ''' <summary>
        ''' Horizontal and vertical axes can be scaled independently. 
        ''' Squares And circles may stretch to rectangles And ovals.
        ''' </summary>
        Disabled
        ''' <summary>
        ''' Axis scales are locked so geometry of squares and circles is preserved.
        ''' After axes are set, the vertical scale (units per pixel) Is applied to the horizontal axis.
        ''' </summary>
        PreserveY
        ''' <summary>
        ''' Axis scales are locked so geometry of squares and circles is preserved.
        ''' After axes are set, the horizontal scale (units per pixel) Is applied to the vertical axis.
        ''' </summary>
        PreserveX
        ''' <summary>
        ''' Axis scales are locked so geometry of squares and circles is preserved.
        ''' After axes are set, the largest scale (most units per pixel) Is applied to both axes.
        ''' Apply the most zoomed-out scale to both axes.
        ''' </summary>
        ZoomOut
        ''' <summary>
        ''' Apply the scale of the larger axis to both axes.
        ''' </summary>
        PreserveLargest
        ''' <summary>
        ''' Apply the scale of the smaller axis to both axes.
        ''' </summary>
        PreserveSmallest
    End Enum

End Namespace