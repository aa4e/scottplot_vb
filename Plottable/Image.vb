Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Display a Bitmap at X/Y coordinates in unit space.
    ''' </summary>
    Public Class Image
        Implements IPlottable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Multiply the size of the image (in pixel units) by this scale factor.
        ''' The primary corner (based on <see cref="Alignment"/>) will remain anchored.
        ''' </summary>
        Public Scale As Double = 1.0

        ''' <summary>
        ''' Position of the primary corner (based on <see cref="Alignment"/>).
        ''' </summary>
        Public Property X As Double

        ''' <summary>
        ''' Position of the primary corner (based on <see cref="Alignment"/>).
        ''' </summary>
        Public Property Y As Double

        ''' <summary>
        ''' If defined, the image will be stretched to be this wide in axis units. If null, the image will use screen/pixel units.
        ''' </summary>
        Public Property WidthInAxisUnits As Double? = Nothing

        ''' <summary>
        ''' If defined, the image will be stretched to be this height in axis units. If null, the image will use screen/pixel units.
        ''' </summary>
        Public Property HeightInAxisUnits As Double? = Nothing

        ''' <summary>
        ''' Rotate the image clockwise around its primary corner (defined by <see cref="Alignment"/>) by this number of degrees.
        ''' </summary>
        Public Property Rotation As Double

        ''' <summary>
        ''' Image to display.
        ''' </summary>
        Public Property Bitmap As System.Drawing.Image

        ''' <summary>
        ''' Indicates which corner of the bitmap is described by X and Y.
        ''' This corner will be the axis of <see cref="Rotation"/>, and the center of <see cref="Scale"/>.
        ''' </summary>
        Public Property Alignment As Alignment

        Public Property BorderColor As Color

        ''' <summary>
        ''' Line width of the border (in pixels).
        ''' </summary>
        Public Property BorderSize As Single

        Public Property Label As String

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        Public Property ClippingPoints As Coordinate() = New Coordinate() {}

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If (Bitmap Is Nothing) Then
                Return AxisLimits.NoLimits
            End If
            Return New AxisLimits(X,
                                  (X + WidthInAxisUnits).GetValueOrDefault(),
                                  (Y - HeightInAxisUnits).GetValueOrDefault(),
                                  Y)
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Overrides Function ToString() As String
            Dim h As String = If(HeightInAxisUnits Is Nothing, $"{Bitmap.Height} pixels", $"{HeightInAxisUnits} axis units")
            Dim w As String = If(WidthInAxisUnits Is Nothing, $"{Bitmap.Width} pixels", $"{WidthInAxisUnits} axis units")
            Return $"Image at ({X}, {Y}), Width = {w}, Height = {h}."
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If Double.IsNaN(X) OrElse Double.IsInfinity(X) Then
                Throw New InvalidOperationException("X must be a real value.")
            End If

            If Double.IsNaN(Y) OrElse Double.IsInfinity(Y) Then
                Throw New InvalidOperationException("Y must be a real value.")
            End If

            Dim w As Double? = WidthInAxisUnits
            If (w IsNot Nothing) Then
                Dim axisWidth As Double = w.GetValueOrDefault()
                If Double.IsNaN(axisWidth) OrElse Double.IsInfinity(axisWidth) Then
                    Throw New InvalidOperationException("Width must be a real value.")
                End If
            End If

            Dim h = HeightInAxisUnits
            If (h IsNot Nothing) Then
                Dim axisHeight As Double = h.GetValueOrDefault()
                If Double.IsNaN(axisHeight) OrElse Double.IsInfinity(axisHeight) Then
                    Throw New InvalidOperationException("Height must be a real value.")
                End If
            End If

            If Double.IsNaN(Scale) OrElse Double.IsInfinity(Scale) Then
                Throw New InvalidOperationException("Scale must be a real value.")
            End If

            If Double.IsNaN(Rotation) OrElse Double.IsInfinity(Rotation) Then
                Throw New InvalidOperationException("Rotation must be a real value.")
            End If

            If (Bitmap Is Nothing) Then
                Throw New InvalidOperationException("Image cannot be null.")
            End If
        End Sub

        Private Function ImageLocationOffset(width As Single, height As Single) As PointF
            Select Case Alignment
                Case Alignment.UpperLeft
                    Return New PointF(0, 0)
                Case Alignment.UpperRight
                    Return New PointF(-width, 0)
                Case Alignment.UpperCenter
                    Return New PointF(-width / 2, 0)
                Case Alignment.MiddleLeft
                    Return New PointF(0, -height / 2)
                Case Alignment.MiddleCenter
                    Return New PointF(-width / 2, -height / 2)
                Case Alignment.MiddleRight
                    Return New PointF(-width, -height / 2)
                Case Alignment.LowerLeft
                    Return New PointF(0, -height)
                Case Alignment.LowerRight
                    Return New PointF(-width, -height)
                Case Alignment.LowerCenter
                    Return New PointF(-width / 2, -height)
                Case Else
                    Throw New InvalidEnumArgumentException()
            End Select
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Dim defaultPoint As New PointF(dims.GetPixelX(X), dims.GetPixelY(Y))

            Dim w As Double? = WidthInAxisUnits
            Dim width As Single
            If (w IsNot Nothing) Then
                width = dims.GetPixelX(X + w.GetValueOrDefault()) - defaultPoint.X
            Else
                width = Bitmap.Width
            End If

            Dim h = HeightInAxisUnits
            Dim height As Single
            If (h IsNot Nothing) Then
                height = dims.GetPixelY(Y - h.GetValueOrDefault()) - defaultPoint.Y
            Else
                height = Bitmap.Height
            End If

            width = CSng(width * Scale)
            height = CSng(height * Scale)

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                pen As New Pen(BorderColor, BorderSize * 2.0F)

                Drawing.GDI.ClipIntersection(gfx, dims, ClippingPoints)
                gfx.PixelOffsetMode = PixelOffsetMode.Half
                gfx.TranslateTransform(defaultPoint.X, defaultPoint.Y)
                gfx.RotateTransform(CSng(Rotation))

                Dim rect As New RectangleF(ImageLocationOffset(width, height), New SizeF(width, height))
                If (BorderSize > 0) Then
                    gfx.DrawRectangle(pen,
                                      Math.Min(rect.X, rect.Right), Math.Min(rect.Y, rect.Bottom),
                                      Math.Abs(rect.Width) - 1, Math.Abs(rect.Height) - 1)
                End If
                gfx.DrawImage(Bitmap, rect)
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace