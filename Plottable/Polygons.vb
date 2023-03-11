Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Polygons are multiple Polygon objects.
    ''' This plot type is faster alternative for rendering a large number of polygons
    ''' compared to adding a bunch of individual Polygon objects to the plot.
    ''' </summary>
    Public Class Polygons
        Implements IPlottable, IHasColor

#Region "PROPS, FIELDS"

        Public ReadOnly Polys As List(Of List(Of Tuple(Of Double, Double)))

        Public Property Label As String
        Public Property LineWidth As Double
        Public Property LineColor As Color
        Public Property Fill As Boolean = True
        Public Property FillColor As Color

        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return FillColor
            End Get
            Set(value As Color)
                FillColor = value
            End Set
        End Property

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property HatchColor As Color = Color.Transparent
        Public Property HatchStyle As Drawing.HatchStyle
        Public Property SkipOffScreenPolygons As Boolean = True
        Public Property RenderSmallPolygonsAsSinglePixels As Boolean = True

        Public ReadOnly Property PointCount As Integer
            Get
                Return Polys.Count
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(polys As List(Of List(Of Tuple(Of Double, Double))))
            Me.Polys = polys
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottablePolygons {lbl} with {PointCount} polygons."
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If deep Then
                For Each poly As List(Of Tuple(Of Double, Double)) In Polys
                    For Each point As Tuple(Of Double, Double) In poly
                        If Double.IsNaN(point.Item1) OrElse Double.IsNaN(point.Item2) Then
                            Throw New InvalidOperationException("Points cannot contain NaN.")
                        End If
                        If Double.IsInfinity(point.Item1) OrElse Double.IsInfinity(point.Item2) Then
                            Throw New InvalidOperationException("Points cannot contain Infinity.")
                        End If
                    Next
                Next
            End If
        End Sub

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Dim xMin As Double = Me.Polys(0)(0).Item1
            Dim xMax As Double = Me.Polys(0)(0).Item1
            Dim yMin As Double = Me.Polys(0)(0).Item2
            Dim yMax As Double = Me.Polys(0)(0).Item2

            For Each poly As List(Of Tuple(Of Double, Double)) In Polys
                For Each point As Tuple(Of Double, Double) In poly
                    xMin = Math.Min(xMin, point.Item1)
                    xMax = Math.Max(xMax, point.Item1)
                    yMin = Math.Min(yMin, point.Item2)
                    yMax = Math.Max(yMax, point.Item2)
                Next
            Next
            Return New AxisLimits(xMin, xMax, yMin, yMax)
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim singleLegendItem As New LegendItem(Me) With {
                .Label = Label,
                .Color = If(Fill, FillColor, LineColor),
                .LineWidth = If(Fill, 10, LineWidth),
                .MarkerShape = MarkerShape.None,
                .HatchColor = HatchColor,
                .HatchStyle = HatchStyle}
            Return New LegendItem() {singleLegendItem}
        End Function

        Private Function IsBiggerThenPixel(poly As List(Of Tuple(Of Double, Double)), UnitsPerPixelX As Double, UnitsPerPixelY As Double) As Boolean
            Dim minX As Double = poly(0).Item1
            Dim maxX As Double = poly(0).Item1
            Dim minY As Double = poly(0).Item2
            Dim maxY As Double = poly(0).Item2

            Dim smallerThenPixelX As Double = 0.5 * UnitsPerPixelX
            Dim smallerThenPixelY As Double = 0.5 * UnitsPerPixelY

            For i As Integer = 1 To poly.Count - 1
                If (poly(i).Item1 < minX) Then
                    minX = poly(i).Item1
                    If (maxX - minX > smallerThenPixelX) Then
                        Return True
                    End If
                End If
                If (poly(i).Item1 > maxX) Then
                    maxX = poly(i).Item1
                    If (maxX - minX > smallerThenPixelX) Then
                        Return True
                    End If
                End If
                If (poly(i).Item2 < minX) Then
                    minY = poly(i).Item2
                    If (maxY - minY > smallerThenPixelY) Then
                        Return True
                    End If
                End If
                If (poly(i).Item2 > maxX) Then
                    maxY = poly(i).Item2
                    If (maxY - minY > smallerThenPixelY) Then
                        Return True
                    End If
                End If
            Next
            Return (maxX - minX > smallerThenPixelX) OrElse (maxY - minY > smallerThenPixelY)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                brush As Brush = Drawing.GDI.Brush(FillColor, HatchColor, HatchStyle),
                pen As Pen = Drawing.GDI.Pen(LineColor, LineWidth)
                For Each poly As List(Of Tuple(Of Double, Double)) In Me.Polys
                    If SkipOffScreenPolygons Then
                        If (poly.Where(Function(pt) _
                                       (pt.Item1 >= dims.XMin) _
                                       AndAlso (pt.Item1 <= dims.XMax) _
                                       AndAlso (pt.Item2 >= dims.YMin) _
                                       AndAlso (pt.Item2 <= dims.YMax)).Count() = 0) Then
                            Continue For
                        End If
                    End If

                    If RenderSmallPolygonsAsSinglePixels Then
                        Dim polyArray =
                            If(Not IsBiggerThenPixel(poly, dims.UnitsPerPxX, dims.UnitsPerPxY),
                            New PointF() {New PointF(dims.GetPixelX(poly(0).Item1), dims.GetPixelY(poly(0).Item2))},
                            poly.Select(Function(point) New PointF(dims.GetPixelX(point.Item1), dims.GetPixelY(point.Item2))).ToArray())

                        If Fill Then
                            If (polyArray.Length >= 3) Then
                                gfx.FillPolygon(brush, polyArray)
                            Else
                                gfx.FillRectangle(brush, polyArray(0).X, polyArray(0).Y, 1, 1)
                            End If
                        End If

                        If (LineWidth > 0) Then
                            gfx.DrawPolygon(pen, polyArray)
                        End If
                    End If
                Next
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace