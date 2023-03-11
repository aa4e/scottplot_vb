Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A pie plot displays a collection of values as a circle.
    ''' Pie plots with a hollow center are donut plots.
    ''' </summary>
    Public Class PiePlot
        Implements IPlottable

        Public Property Values As Double()
        Public Property Label As String
        Public Property SliceLabels As String()

        ''' <summary>
        ''' Defines how large the pie is relative to the pixel size of the smallest axis.
        ''' </summary>
        Public Property Size As Double = 0.9

        Public Property SliceFillColors As Color()
        Public Property SliceLabelColors As Color()
        Public Property BackgroundColor As Color
        Public Property HatchOptions As HatchOptions()

        Public Property Explode As Boolean
        Public Property ShowValues As Boolean
        Public Property ShowPercentages As Boolean
        Public Property ShowLabels As Boolean

        Public Property DonutSize As Double
        Public Property DonutLabel As String
        Public ReadOnly CenterFont As New ScottPlot.Drawing.Font()
        Public ReadOnly SliceFont As New ScottPlot.Drawing.Font()

        Public Property OutlineSize As Single = 0
        Public Property OutlineColor As Color = Color.Black

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        Public ReadOnly Property PointCount As Integer
            Get
                Return Values.Length
            End Get
        End Property

        Public Sub New(values As Double(), groupNames As String(), colors As Color())
            Me.Values = values
            Me.SliceLabels = groupNames
            Me.SliceFillColors = colors
            Me.SliceFont.Size = 18
            Me.SliceFont.Bold = True
            Me.SliceFont.Color = Color.White
            Me.CenterFont.Size = 48
            Me.CenterFont.Bold = True
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottablePie{lbl} with {PointCount} points."
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            If (SliceLabels Is Nothing) Then
                Return {}
            End If

            Return Enumerable.Range(0, Values.Length) _
                .Select(Function(i As Integer)
                            Return New LegendItem(Me) With {
                            .Label = SliceLabels(i),
                            .Color = SliceFillColors(i),
                            .LineWidth = 10,
                            .HatchStyle = If(HatchOptions?(i).Pattern IsNot Nothing, HatchOptions(i).Pattern, Drawing.HatchStyle.None),
                            .HatchColor = If(HatchOptions?(i).Color IsNot Nothing, HatchOptions(i).Color, Color.Black)
                            }
                        End Function).ToArray()
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return New AxisLimits(-0.5, 0.5, -1, 1)
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            Validate.AssertHasElements("Values", Values)
            Validate.AssertHasElements("Colors", SliceFillColors)
            Validate.AssertAllReal("Values", Values)
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                backgroundPen As Pen = Drawing.GDI.Pen(BackgroundColor),
                outlinePen As Pen = Drawing.GDI.Pen(OutlineColor, OutlineSize),
                slcFont As System.Drawing.Font = Drawing.GDI.Font(SliceFont),
                sliceFontBrush As SolidBrush = CType(Drawing.GDI.Brush(SliceFont.Color), SolidBrush),
                centFont As System.Drawing.Font = Drawing.GDI.Font(CenterFont),
                centerFontBrush As Brush = Drawing.GDI.Brush(CenterFont.Color),
                sfCenter As New StringFormat() With {.LineAlignment = StringAlignment.Center, .Alignment = StringAlignment.Center}

                Dim proportions As Double() = Values.Select(Function(x As Double) x / Values.Sum()).ToArray()

                Dim centreX As Double = 0
                Dim centreY As Double = 0
                Dim diameterPixels As Single = CSng(Size * Math.Min(dims.DataWidth, dims.DataHeight))

                'record label details and draw them after slices to prevent cover-ups
                Dim labelXs As Double() = New Double(Values.Length - 1) {}
                Dim labelYs As Double() = New Double(Values.Length - 1) {}
                Dim labelStrings As String() = New String(Values.Length - 1) {}

                Dim boundingRectangle As New RectangleF(
                    dims.GetPixelX(centreX) - diameterPixels / 2,
                    dims.GetPixelY(centreY) - diameterPixels / 2,
                    diameterPixels,
                    diameterPixels)

                If (DonutSize > 0) Then
                    Dim graphicsPath As New GraphicsPath()
                    Dim donutDiameterPixels As Single = CSng(DonutSize * diameterPixels)
                    Dim donutHoleBoundingRectangle As New RectangleF(
                        dims.GetPixelX(centreX) - donutDiameterPixels / 2,
                        dims.GetPixelY(centreY) - donutDiameterPixels / 2,
                        donutDiameterPixels,
                        donutDiameterPixels)
                    graphicsPath.AddEllipse(donutHoleBoundingRectangle)
                    Dim excludedRegion As New Region(graphicsPath)
                    gfx.ExcludeClip(excludedRegion)
                End If

                Dim start As Double = -90
                For i As Integer = 0 To Values.Length - 1
                    'determine where the slice is to be drawn
                    Dim sweep As Double = proportions(i) * 360.0
                    Dim sweepOffset As Double = If(Explode, -1, 0)
                    Dim angle As Double = (Math.PI / 180) * ((sweep + 2 * start) / 2)
                    Dim xOffset As Double = If(Explode, 3 * Math.Cos(angle), 0)
                    Dim yOffset As Double = If(Explode, 3 * Math.Sin(angle), 0)

                    'record where and what to label the slice
                    Dim sliceLabelR As Double = 0.35 * diameterPixels
                    labelXs(i) = (boundingRectangle.X + diameterPixels / 2) + xOffset + Math.Cos(angle) * sliceLabelR
                    labelYs(i) = (boundingRectangle.Y + diameterPixels / 2) + yOffset + Math.Sin(angle) * sliceLabelR
                    Dim sliceLabelValue As String = If(ShowValues, $"{Values(i)}", "")
                    Dim sliceLabelPercentage As String = If(ShowPercentages, $"{(proportions(i) * 100):f1}%", "")
                    Dim sliceLabelName As String = If(ShowLabels AndAlso SliceLabels IsNot Nothing, SliceLabels(i), "")
                    labelStrings(i) = String.Concat(New String() {sliceLabelValue, vbLf, sliceLabelPercentage, vbLf, sliceLabelName}).Trim()

                    Using sliceFillBrush = Drawing.GDI.Brush(SliceFillColors(i),
                                                     HatchOptions?(i).Color,
                                                     If(HatchOptions?(i).Pattern IsNot Nothing, HatchOptions(i).Pattern, Drawing.HatchStyle.None))

                        Dim offsetRectangle As New Rectangle(CInt(boundingRectangle.X + xOffset),
                                                             CInt(boundingRectangle.Y + yOffset),
                                                             CInt(boundingRectangle.Width),
                                                             CInt(boundingRectangle.Height))
                        If (sweep <> 360) Then
                            gfx.FillPie(sliceFillBrush, offsetRectangle, CSng(start), CSng(sweep + sweepOffset))
                        Else
                            gfx.FillEllipse(sliceFillBrush, offsetRectangle)
                        End If

                        If Explode AndAlso (sweep <> 360) Then
                            gfx.DrawPie(backgroundPen, offsetRectangle, CSng(start), CSng(sweep + sweepOffset))
                        End If
                    End Using
                    start += sweep
                Next

                Dim useCustomLabelColors As Boolean = (SliceLabelColors IsNot Nothing) AndAlso (SliceLabelColors.Length = Values.Length)
                For i As Integer = 0 To Values.Length - 1
                    If (Not String.IsNullOrWhiteSpace(labelStrings(i))) Then
                        If useCustomLabelColors Then
                            sliceFontBrush.Color = SliceLabelColors(i)
                        End If
                        gfx.DrawString(labelStrings(i), slcFont, sliceFontBrush, CSng(labelXs(i)), CSng(labelYs(i)), sfCenter)
                    End If
                Next

                If (OutlineSize > 0) Then
                    gfx.DrawEllipse(outlinePen,
                                    boundingRectangle.X, boundingRectangle.Y,
                                    boundingRectangle.Width, boundingRectangle.Height)
                End If

                gfx.ResetClip()

                If (DonutLabel IsNot Nothing) Then
                    gfx.DrawString(DonutLabel, centFont, centerFontBrush,
                                   dims.GetPixelX(0.0), dims.GetPixelY(0.0),
                                   sfCenter)
                End If

                If Explode Then
                    'draw a background-colored circle around the perimeter to make it look like all pieces are the same size
                    backgroundPen.Width = 20
                    gfx.DrawEllipse(backgroundPen,
                                    boundingRectangle.X, boundingRectangle.Y,
                                    boundingRectangle.Width, boundingRectangle.Height)
                End If
            End Using
        End Sub

    End Class

End Namespace