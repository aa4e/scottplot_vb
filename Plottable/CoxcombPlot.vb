Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A Pie chart where the angle of slices is constant but the radii are not.
    ''' </summary>
    Public Class CoxcombPlot
        Implements IPlottable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' The data to be plotted.
        ''' </summary>
        Public Property Values As Double()
            Get
                Return _Values
            End Get
            Set(value As Double())
                _Values = value
                Normalized = CoxcombPlot.Normalize(value)
            End Set
        End Property
        Private _Values As Double()

        ''' <summary>
        ''' The colors of each slice.
        ''' </summary>
        Public Property FillColors As Color()

        ''' <summary>
        ''' Contains options for hatched (patterned) fills for each slice.
        ''' </summary>
        Public Property HatchOptions As HatchOptions()

        ''' <summary>
        ''' The color of the slice outline.
        ''' </summary>
        Public Property Outline As Color = Color.Black

        ''' <summary>
        ''' The width of the slice outline.
        ''' </summary>
        Public Property OutlineWidth As Single = 0

        ''' <summary>
        ''' The color to draw the axis in.
        ''' </summary>
        Public Property WebColor As Color = Color.Gray

        ''' <summary>
        ''' Controls rendering style of the concentric circles (ticks) of the web.
        ''' </summary>
        Public Property AxisType As RadarAxis = RadarAxis.Circle

        ''' <summary>
        ''' If true, each value will be written in text on the plot.
        ''' </summary>
        Public Property ShowAxisValues As Boolean = True

        ''' <summary>
        ''' Labels for each category.
        ''' Length must be equal to the number of columns (categories) in the original data..
        ''' </summary>
        Public Property SliceLabels As String()

        ''' <summary>
        ''' Icons for each category.
        ''' Length must be equal to the number of columns (categories) in the original data. 
        ''' </summary>
        Public Property CategoryImages As System.Drawing.Image()

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        Public Property Label As String

        Private Property Normalized As Double()

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(values As Double(), fillColors As Color())
            Me.Values = values
            Me.FillColors = fillColors
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Dim numCategories As Integer = Normalized.Length
            Dim origin As New PointF(dims.GetPixelX(0), dims.GetPixelY(0))
            Dim sweepAngle As Double = 360 / numCategories
            Dim maxRadiusPixels As Double = New Double() {dims.PxPerUnitX, dims.PxPerUnitX}.Min() * 2

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                outlinePen As Pen = Drawing.GDI.Pen(Outline, OutlineWidth)

                RenderAxis(gfx, dims, bmp, lowQuality)

                Dim start As Double = -90.0
                For i As Integer = 0 To numCategories - 1
                    Using sliceFillBrush As Brush = Drawing.GDI.Brush(FillColors(i), HatchOptions?(i).Color, If(HatchOptions?(i).Pattern, Drawing.HatchStyle.None))
                        Dim angle As Double = (sweepAngle + 2 * start) / 2
                        Dim diameter As Single = CSng(maxRadiusPixels * Normalized(i))

                        Dim pieX As Single = origin.X - diameter / 2
                        Dim pieY As Single = origin.Y - diameter / 2

                        gfx.FillPie(sliceFillBrush, pieX, pieY, diameter, diameter, CSng(start), CSng(sweepAngle))

                        If (OutlineWidth <> 0) Then
                            gfx.DrawPie(outlinePen, pieX, pieY, diameter, diameter, CSng(start), CSng(sweepAngle))
                        End If

                        start += sweepAngle
                    End Using
                Next
            End Using
        End Sub

        Private Sub RenderAxis(gfx As Graphics, dims As PlotDimensions, bmp As Bitmap, lowQuality As Boolean)
            Dim norm As Double(,) = New Double(Me.Normalized.Length - 1, 0) {}
            For i As Integer = 0 To Me.Normalized.Length - 1
                norm(i, 0) = Normalized(i)
            Next
            Dim ticks As StarAxisTick() = New Double() {0.25, 0.5, 1} _
                .Select(Function(x As Double) New StarAxisTick(x, Values.Max())).ToArray()

            Dim axis As New StarAxis() With {
                .Ticks = ticks,
                .CategoryLabels = SliceLabels,
                .AxisType = AxisType,
                .WebColor = WebColor,
                .ShowAxisValues = ShowAxisValues,
                .CategoryImages = CategoryImages,
                .Graphics = gfx,
                .ShowCategoryLabels = False,
                .NumberOfSpokes = Values.Length,
                .ImagePlacement = ImagePlacement.Inside}
            axis.Render(dims, bmp, lowQuality)
        End Sub

        Private Shared Function Normalize(values As Double()) As Double()
            Dim max As Double = values.Max()
            If (max = 0) Then
                Return values.Select(Function(x) 0.0).ToArray() 'TEST 
            End If
            Return values.Select(Function(v As Double) v / max).ToArray()
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            If (SliceLabels is Nothing) Then
                Return {}
            End If

            Return Enumerable.Range(0, Values.Length).Select(Function(i As Integer)
                                                                 Dim leg As New LegendItem(Me) With {
                                                                 .Label = SliceLabels(i),
                                                                 .Color = FillColors(i),
                                                                 .LineWidth = 10,
                                                                 .HatchStyle = If(HatchOptions?(i).Pattern, Drawing.HatchStyle.None),
                                                                 .HatchColor = If(HatchOptions?(i).Color, Color.Black)
                                                                 }
                                                                 Return leg
                                                             End Function).ToArray()
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return New AxisLimits(-2.5, 2.5, -2.5, 2.5)
        End Function

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableCoxcomb {lbl} with {Values.Length} categories."
        End Function

#End Region '/METHODS

    End Class

End Namespace