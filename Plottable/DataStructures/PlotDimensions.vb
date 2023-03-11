Imports System.Drawing

Namespace ScottPlot

    ''' <summary>
    ''' Supplies figure dimensions and pixel/coordinate lookup methods for a single 2D plane.
    ''' </summary>
    Public Class PlotDimensions

#Region "PROPS, FIELDS"

        'Plot dimensions
        Public ReadOnly Width As Single
        Public ReadOnly Height As Single
        Public ReadOnly DataWidth As Single
        Public ReadOnly DataHeight As Single
        Public ReadOnly DataOffsetX As Single
        Public ReadOnly DataOffsetY As Single

        'Rendering options
        Public ReadOnly ScaleFactor As Double

        'Axis limits
        Public ReadOnly XMin As Double
        Public ReadOnly XMax As Double
        Public ReadOnly YMin As Double
        Public ReadOnly YMax As Double
        Public ReadOnly XSpan As Double
        Public ReadOnly YSpan As Double
        Public ReadOnly XCenter As Double
        Public ReadOnly YCenter As Double
        Public ReadOnly AxisLimits As AxisLimits

        'Pixel/coordinate conversions
        Public ReadOnly PxPerUnitX As Double
        Public ReadOnly PxPerUnitY As Double
        Public ReadOnly UnitsPerPxX As Double
        Public ReadOnly UnitsPerPxY As Double

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(figureSize As SizeF, dataSize As SizeF, dataOffset As PointF, axisLimits As AxisLimits, scaleFactor As Double)
            Me.Width = figureSize.Width
            Me.Height = figureSize.Height
            Me.DataWidth = dataSize.Width
            Me.DataHeight = dataSize.Height
            Me.DataOffsetX = dataOffset.X
            Me.DataOffsetY = dataOffset.Y
            Me.AxisLimits = axisLimits
            Me.XMin = axisLimits.XMin
            Me.XMax = axisLimits.XMax
            Me.YMin = axisLimits.YMin
            Me.YMax = axisLimits.YMax
            Me.XSpan = XMax - XMin
            Me.YSpan = YMax - YMin
            Me.XCenter = (XMin + XMax) / 2
            Me.YCenter = (YMin + YMax) / 2
            Me.PxPerUnitX = DataWidth / XSpan
            Me.PxPerUnitY = DataHeight / YSpan
            Me.UnitsPerPxX = XSpan / DataWidth
            Me.UnitsPerPxY = YSpan / DataHeight
            Me.ScaleFactor = scaleFactor
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetPixel(coordinate As Coordinate) As Pixel
            Return New Pixel(GetPixelX(coordinate.X), GetPixelY(coordinate.Y))
        End Function

        Public Function GetPixelX(position As Double) As Single
            Return CSng(DataOffsetX + (position - XMin) * PxPerUnitX)
        End Function

        Public Function GetPixelY(position As Double) As Single
            Return CSng(DataOffsetY + (YMax - position) * PxPerUnitY)
        End Function

        Public Function GetCoordinate(pixel As Pixel) As Coordinate
            Return New Coordinate(GetCoordinateX(pixel.X), GetCoordinateY(pixel.Y))
        End Function

        Public Function GetCoordinate(xPixel As Single, yPixel As Single) As Coordinate
            Return New Coordinate(GetCoordinateX(xPixel), GetCoordinateY(yPixel))
        End Function

        Public Function GetCoordinateX(pixel As Single) As Double
            Return (pixel - DataOffsetX) / PxPerUnitX + XMin
        End Function

        Public Function GetCoordinateY(pixel As Single) As Double
            Return (YMax - (pixel - DataOffsetY) / PxPerUnitY)
        End Function

        Public Overrides Function ToString() As String
            Return $"Dimensions for figure ({Width}x{Height}), data area ({DataWidth}x{DataHeight}) and axes ({XMin}, {XMax}, {YMin}, {YMax})."
        End Function

#End Region '/METHODS

    End Class

End Namespace