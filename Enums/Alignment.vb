Namespace ScottPlot

    ''' <summary>
    ''' Vertical (upper/middle/lower) and Horizontal (left/center/right) alignment.
    ''' </summary>
    Public Enum Alignment
        UpperLeft
        UpperRight
        UpperCenter
        MiddleLeft
        MiddleCenter
        MiddleRight
        LowerLeft
        LowerRight
        LowerCenter
    End Enum

    <Obsolete("use Alignment", True)>
    Public Enum TextAlignment
        None
    End Enum

    <Obsolete("use Alignment", True)>
    Public Enum ImageAlignment
        None
    End Enum

    <Obsolete("use Alignment", True)>
    Public Enum LegendLocation
        None
    End Enum

    <Obsolete("use Alignment", True)>
    Public Enum ShadowDirection
        None
    End Enum

End Namespace