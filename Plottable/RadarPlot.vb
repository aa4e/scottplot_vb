Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A radar chart is a graphical method of displaying multivariate data in the form of 
    ''' a two-dimensional chart of three Or more quantitative variables represented on axes 
    ''' starting from the same point.
    ''' </summary>
    Public Class RadarPlot
        Implements IPlottable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Values for every group (rows) and category (columns) normalized from 0 to 1.
        ''' </summary>
        Private Norm As Double(,)

        ''' <summary>
        ''' Single value to normalize all values against for all groups/categories.
        ''' </summary>
        Private NormMax As Double

        ''' <summary>
        ''' Individual values (one per category) to use for normalization.
        ''' Length must be equal to the number of columns (categories) in the original data.
        ''' </summary>
        Private NormMaxes As Double()

        ''' <summary>
        ''' Font used for labeling values on the plot.
        ''' </summary>
        Public ReadOnly Font As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' If enabled, the plot will fill using a curve instead of polygon.
        ''' </summary>
        Public Smooth As Boolean = False

        ''' <summary>
        ''' Labels for each category. Length must be equal to the number of columns (categories) in the original data.
        ''' </summary>
        ''' <remarks>
        ''' If showing icons, labels will be ignored.
        ''' </remarks>
        Public Property CategoryLabels As String()

        ''' <summary>
        ''' Icons for each category. Length must be equal to the number of columns (categories) in the original data.
        ''' </summary>
        ''' <remarks>
        ''' If showing icons, labels will be ignored.
        ''' </remarks>
        Public Property CategoryImages As System.Drawing.Image()

        ''' <summary>
        ''' Labels for each group. Length must be equal to the number of rows (groups) in the original data.
        ''' </summary>
        Public Property GroupLabels As String()

        ''' <summary>
        ''' Colors (typically semi-transparent) to shade the inner area of each group.
        ''' Length must be equal to the number of rows (groups) in the original data.
        ''' </summary>
        Public Property FillColors As Color()

        ''' <summary>
        ''' Colors to outline the shape for each group.
        ''' Length must be equal to the number of rows (groups) in the original data.
        ''' </summary>
        Public Property LineColors As Color()

        ''' <summary>
        ''' Color of the axis lines and concentric circles representing ticks.
        ''' </summary>
        Public Property WebColor As Color = Color.Gray

        ''' <summary>
        ''' Contains options for hatched (patterned) fills for each slice.
        ''' </summary>
        Public Property HatchOptions As HatchOptions()

        ''' <summary>
        ''' Controls if values along each category axis are scaled independently or uniformly across all axes.
        ''' </summary>
        Public Property IndependentAxes As Boolean

        ''' <summary>
        ''' If true, each value will be written in text on the plot.
        ''' </summary>
        Public Property ShowAxisValues As Boolean = True

        ''' <summary>
        ''' If true, each category name will be written in text at every corner of the radar.
        ''' </summary>
        Public Property ShowCategoryLabels As Boolean = True

        ''' <summary>
        ''' Controls rendering style of the concentric circles (ticks) of the web.
        ''' </summary>
        Public Property AxisType As RadarAxis = RadarAxis.Circle

        ''' <summary>
        ''' Determines the width of each spoke and the axis lines.
        ''' </summary>
        Public Property LineWidth As Integer = 1

        ''' <summary>
        ''' Determines the width of the line at the edge of each area polygon.
        ''' </summary>
        Public Property OutlineWidth As Single = 1

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        Public ReadOnly Property PointCount As Integer
            Get
                Return Norm.Length
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(values As Double(,), lineColors As Color(), fillColors As Color(), independentAxes As Boolean, Optional maxValues As Double() = Nothing)
            Me.LineColors = lineColors
            Me.FillColors = fillColors
            Me.IndependentAxes = independentAxes
            Me.Update(values, independentAxes, maxValues)
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"PlottableRadar with {PointCount} points and {Norm.GetUpperBound(1) + 1} categories."
        End Function

        ''' <summary>
        ''' Replace the data values with New ones.
        ''' </summary>
        ''' <param name="values">2D array of groups (rows) of values for each category (columns).</param>
        ''' <param name="independentAxes">Controls if values along each category axis are scaled independently or uniformly across all axes.</param>
        ''' <param name="maxValues">If provided, these values will be used to normalize each category (columns).</param>
        Public Sub Update(values As Double(,), Optional independentAxes As Boolean = False, Optional maxValues As Double() = Nothing)
            Me.IndependentAxes = independentAxes
            Norm = New Double(values.GetLength(0) - 1, values.GetLength(1) - 1) {}
            Array.Copy(values, 0, Me.Norm, 0, values.Length)

            If Me.IndependentAxes Then
                NormMaxes = NormalizeSeveralInPlace(Norm, maxValues)
            End If
            NormMax = NormalizeInPlace(Norm, maxValues)
        End Sub

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If (GroupLabels IsNot Nothing) AndAlso (GroupLabels.Length <> Norm.GetLength(0)) Then
                Throw New InvalidOperationException("Group names must match size of values.")
            End If
            If (CategoryLabels IsNot Nothing) AndAlso (CategoryLabels.Length <> Norm.GetLength(1)) Then
                Throw New InvalidOperationException("Category names must match size of values.")
            End If
        End Sub

        ''' <summary>
        ''' Normalize a 2D array by dividing all values by the maximum value.
        ''' </summary>
        ''' <returns>Maximum value in the array before normalization.</returns>
        Private Function NormalizeInPlace(input As Double(,), Optional maxValues As Double() = Nothing) As Double
            Dim max As Double
            If (maxValues IsNot Nothing) AndAlso (maxValues.Length = 1) Then
                max = maxValues(0)
            Else
                max = input(0, 0)
                For i As Integer = 0 To input.GetLength(0) - 1
                    For j As Integer = 0 To input.GetLength(1) - 1
                        max = Math.Max(max, input(i, j))
                    Next
                Next
            End If

            For i As Integer = 0 To input.GetLength(0) - 1
                For j As Integer = 0 To input.GetLength(1) - 1
                    input(i, j) /= max
                Next
            Next
            Return max
        End Function

        ''' <summary>
        ''' Normalize each row of a 2D array independently by dividing all values by the maximum value.
        ''' </summary>
        ''' <returns>Maximum value in each row of the array before normalization.</returns>
        Private Function NormalizeSeveralInPlace(input As Double(,), Optional maxValues As Double() = Nothing) As Double()
            Dim maxes As Double()
            If (maxValues IsNot Nothing) AndAlso (input.GetLength(1) = maxValues.Length) Then
                maxes = maxValues
            Else
                maxes = New Double(input.GetLength(1) - 1) {}
                For i As Integer = 0 To input.GetLength(1) - 1
                    Dim max As Double = input(0, i)
                    For j As Integer = 0 To input.GetLength(0) - 1
                        max = Math.Max(input(j, i), max)
                    Next
                    maxes(i) = max
                Next
            End If

            For i As Integer = 0 To input.GetLength(0) - 1
                For j As Integer = 0 To input.GetLength(1) - 1
                    If (maxes(j) = 0) Then
                        input(i, j) = 0
                    Else
                        input(i, j) /= maxes(j)
                    End If
                Next
            Next
            Return maxes
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            If (GroupLabels is Nothing) Then
                Return {}
            End If
            Dim legendItems As New List(Of LegendItem)()
            For i As Integer = 0 To GroupLabels.Length - 1
                Dim legendItem As New LegendItem(Me) With {
                    .Label = GroupLabels(i),
                    .Color = FillColors(i),
                    .LineWidth = 10,
                    .MarkerShape = MarkerShape.None,
                    .HatchStyle = If(Me.HatchOptions?(i) IsNot Nothing, Me.HatchOptions(i).Pattern, Drawing.HatchStyle.None),
                    .HatchColor = If((Me.HatchOptions?(i) IsNot Nothing), Me.HatchOptions(i).Color, Color.Black)
                }
                legendItems.Add(legendItem)
            Next
            Return legendItems.ToArray()
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If (GroupLabels is Nothing) Then
                Return New AxisLimits(-2.5, 2.5, -2.5, 2.5)
            End If
            Return New AxisLimits(-3.5, 3.5, -3.5, 3.5)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Dim numGroups As Integer = Norm.GetUpperBound(0) + 1
            Dim numCategories As Integer = Norm.GetUpperBound(1) + 1
            Dim sweepAngle As Double = 2 * Math.PI / numCategories
            Dim minScale As Double = New Double() {dims.PxPerUnitX, dims.PxPerUnitX}.Min()
            Dim origin As New PointF(dims.GetPixelX(0), dims.GetPixelY(0))

            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, True),
                pen As Pen =Drawing.GDI.Pen(WebColor, OutlineWidth)

                RenderAxis(gfx, dims, bmp, lowQuality)

                For i As Integer = 0 To numGroups - 1
                    Dim points As PointF() = New PointF(numCategories - 1) {}
                    For j As Integer = 0 To numCategories - 1
                        points(j) = New PointF(CSng(Norm(i, j) * Math.Cos(sweepAngle * j - Math.PI / 2) * minScale + origin.X),
                                               CSng(Norm(i, j) * Math.Sin(sweepAngle * j - Math.PI / 2) * minScale + origin.Y))
                    Next

                    Using brush As Brush =Drawing.GDI.Brush(FillColors(i),
                                                     HatchOptions?(i).Color,
                                                     If(HatchOptions?(i) IsNot Nothing, HatchOptions(i).Pattern, Drawing.HatchStyle.None))
                        pen.Color = LineColors(i)

                        If Smooth Then
                            gfx.FillClosedCurve(brush, points)
                            gfx.DrawClosedCurve(pen, points)
                        Else
                            gfx.FillPolygon(brush, points)
                            gfx.DrawPolygon(pen, points)
                        End If
                    End Using
                Next
            End Using
        End Sub

        Private Function GetTick(location As Double) As StarAxisTick
            If IndependentAxes Then
                Return New StarAxisTick(location, NormMaxes.Select(Function(x As Double) x * location).ToArray())
            End If
            Return New StarAxisTick(location, NormMax)
        End Function

        Private Sub RenderAxis(gfx As Graphics, dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Dim tickLocations As Double() = New Double() {0.25, 0.5, 1}
            Dim ticks As StarAxisTick() = tickLocations.Select(Function(x) GetTick(x)).ToArray()

            Dim axis As StarAxis = New StarAxis() With {
                .Ticks = ticks,
                .CategoryLabels = CategoryLabels,
                .CategoryImages = CategoryImages,
                .NumberOfSpokes = Norm.GetLength(1),
                .AxisType = AxisType,
                .WebColor = WebColor,
                .LineWidth = LineWidth,
                .ShowCategoryLabels = ShowCategoryLabels,
                .LabelEachSpoke = IndependentAxes,
                .ShowAxisValues = ShowAxisValues,
                .Graphics = gfx,
                .ImagePlacement = ImagePlacement.Outside
            }
            axis.Render(dims, bmp, lowQuality)
        End Sub

#End Region '/METHODS

    End Class

End Namespace