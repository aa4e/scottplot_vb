Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A heatmap displays a 2D array of intensities as small rectangles on the plot
    ''' colored according to their intensity value according to a colormap.
    ''' </summary>
    Public Class Heatmap
        Implements IPlottable, IHasColormap

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Minimum heatmap value.
        ''' </summary>
        Private Min As Double

        ''' <summary>
        ''' Maximum heatmap value.
        ''' </summary>
        Private Max As Double

        ''' <summary>
        ''' Number of columns in the heatmap data.
        ''' </summary>
        Private DataWidth As Integer

        ''' <summary>
        ''' Number of rows in the heatmap data.
        ''' </summary>
        Private DataHeight As Integer

        ''' <summary>
        ''' Pre-rendered heatmap image.
        ''' </summary>
        Protected BmpHeatmap As Bitmap

        ''' <summary>
        ''' Horizontal location of the lower-left cell.
        ''' </summary>
        Public Property OffsetX As Double = 0

        ''' <summary>
        ''' Vertical location of the lower-left cell.
        ''' </summary>
        Public Property OffsetY As Double = 0

        ''' <summary>
        ''' Width of each cell composing the heatmap.
        ''' </summary>
        Public Property CellWidth As Double = 1

        ''' <summary>
        ''' Width of each cell composing the heatmap.
        ''' </summary>
        Public Property CellHeight As Double = 1

        ''' <summary>
        ''' Position of the left edge of the heatmap.
        ''' </summary>
        Public Property XMin As Double
            Get
                Return OffsetX
            End Get
            Set(value As Double)
                OffsetX = value
            End Set
        End Property

        ''' <summary>
        ''' Position of the right edge of the heatmap.
        ''' </summary>
        Public Property XMax As Double
            Get
                Return OffsetX + DataWidth * CellWidth
            End Get
            Set(value As Double)
                CellWidth = (value - OffsetX) / DataWidth
            End Set
        End Property

        Public Property YMin As Double
            Get
                Return OffsetY
            End Get
            Set(value As Double)
                OffsetY = value
            End Set
        End Property

        Public Property YMax As Double
            Get
                Return OffsetY + DataHeight * CellHeight
            End Get
            Set(value As Double)
                CellHeight = (value - OffsetY) / DataHeight
            End Set
        End Property

        ''' <summary>
        ''' Indicates whether the heatmap's size or location has been modified by the user.
        ''' </summary>
        Public ReadOnly Property IsDefaultSizeAndLocation As Boolean
            Get
                Return (OffsetX = 0) AndAlso (OffsetY = 0) AndAlso (CellHeight = 1) AndAlso (CellWidth = 1)
            End Get
        End Property

        ''' <summary>
        ''' Text to appear in the legend.
        ''' </summary>
        Public Property Label As String = String.Empty

        ''' <summary>
        ''' Colormap used to translate heatmap values to colors.
        ''' </summary>
        Public ReadOnly Property Colormap As ScottPlot.Drawing.Colormap = ScottPlot.Drawing.Colormap.Viridis Implements ScottPlot.Plottable.IHasColormap.Colormap

        ''' <summary>
        ''' If defined, colors will be "clipped" to this value such that lower values (lower colors) will not be shown.
        ''' </summary>
        Public Property ScaleMin As Double?

        ''' <summary>
        ''' If defined, colors will be "clipped" to this value such that greater values (higher colors) will not be shown.
        ''' </summary>
        Public Property ScaleMax As Double?

        ''' <summary>
        ''' Heatmap values below this number (if defined) will be made transparent.
        ''' </summary>
        Public Property TransparencyThreshold As Double?

        <Obsolete("This feature has been deprecated. Use AddImage() to place a bitmap beneath or above the heatmap.", True)>
        Public Property BackgroundImage As Bitmap

        <Obsolete("This feature has been deprecated. Use AddImage() to place a bitmap beneath or above the heatmap.", True)>
        Public Property DisplayImageAbove As Boolean

        <Obsolete("This feature has been deprecated. Use Plot.AddText() to add text to the plot.", True)>
        Public ShowAxisLabels As Boolean

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        ''' <summary>
        ''' Value of the the lower edge of the colormap.
        ''' </summary>
        Public ReadOnly Property ColormapMin As Double Implements ScottPlot.Plottable.IHasColormap.ColormapMin
            Get
                Return If(Me.ScaleMin, Min)
            End Get
        End Property

        ''' <summary>
        ''' Value of the the upper edge of the colormap.
        ''' </summary>
        Public ReadOnly Property ColormapMax As Double Implements ScottPlot.Plottable.IHasColormap.ColormapMax
            Get
                Return If(Me.ScaleMax, Max)
            End Get
        End Property

        ''' <summary>
        ''' Indicates whether values extend beyond the lower edge of the colormap.
        ''' </summary>
        Public ReadOnly Property ColormapMinIsClipped As Boolean = False Implements ScottPlot.Plottable.IHasColormap.ColormapMinIsClipped

        ''' <summary>
        ''' Indicates whether values extend beyond the upper edge of the colormap.
        ''' </summary>
        Public ReadOnly Property ColormapMaxIsClipped As Boolean = False Implements ScottPlot.Plottable.IHasColormap.ColormapMaxIsClipped

        ''' <summary>
        ''' If true, heatmap squares will be smoothed using high quality bicubic interpolation.
        ''' If false, heatmap squares will look like sharp rectangles (nearest neighbor interpolation).
        ''' </summary>
        Public Property Smooth As Boolean
            Get
                Return (Interpolation <> InterpolationMode.NearestNeighbor)
            End Get
            Set(value As Boolean)
                Interpolation = If(value, InterpolationMode.HighQualityBicubic, InterpolationMode.NearestNeighbor)
            End Set
        End Property

        ''' <summary>
        ''' Controls which interpolation mode is used when zooming into the heatmap.
        ''' </summary>
        Public Property Interpolation As InterpolationMode = InterpolationMode.NearestNeighbor

        ''' <summary>
        ''' If true the Heatmap will be drawn from the bottom left corner of the plot. Otherwise it will be drawn from the top left corner. Defaults to false.
        ''' </summary>
        Public Property FlipVertically As Boolean = False

        Public Property ClippingPoints As Coordinate() = {}

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        ''' <summary>
        ''' This method analyzes the intensities and colormap to create a bitmap with a single pixel for every intensity value. 
        ''' The bitmap is stored and displayed (without anti-alias interpolation) when <see cref="Render(PlotDimensions, Bitmap, Boolean)"/> is called.
        ''' </summary>
        ''' <param name="intensities">2D array of data for the heatmap (null values are not shown).</param>
        ''' <param name="colormap">Update the Colormap to use this colormap.</param>
        ''' <param name="min">Minimum intensity (according to the colormap).</param>
        ''' <param name="max">Maximum intensity (according to the colormap).</param>
        ''' <param name="opacity">If defined, this mask indicates the opacity of each cell in the heatmap from 0 (transparent) to 1 (opaque).
        ''' If defined, this array must have the same dimensions as the heatmap array. Null values are not shown.</param>
        Public Sub Update(intensities As Double?(,),
                          Optional colormap As ScottPlot.Drawing.Colormap = Nothing,
                          Optional min As Double? = Nothing,
                          Optional max As Double? = Nothing,
                          Optional opacity As Double?(,) = Nothing)
            ' limit edge size due to System.Drawing rendering artifacts
            ' https://github.com/ScottPlot/ScottPlot/issues/2119
            Dim maxEdgeLength As Integer = 1 << 15
            If (intensities.GetLength(1) > maxEdgeLength) OrElse (intensities.GetLength(0) > maxEdgeLength) Then
                Throw New ArgumentException($"Due to limitations in rendering large bitmaps, heatmaps cannot have more than {maxEdgeLength:N0} rows or columns.")
            End If

            ' limit total size due to System.Drawing rendering artifacts
            ' https://github.com/ScottPlot/ScottPlot/issues/772
            Dim maxTotalValues As Integer = 10_000_000
            If (intensities.GetLength(1) * intensities.GetLength(0) > maxTotalValues) Then
                Throw New ArgumentException($"Heatmaps may be unreliable for 2D arrays with more than {maxTotalValues:N0} values.")
            End If

            DataWidth = intensities.GetLength(1)
            DataHeight = intensities.GetLength(0)

            Me._Colormap = If(colormap, Me.Colormap)
            ScaleMin = min
            ScaleMax = max

            Dim intensitiesFlattened As Double?() = intensities.Cast(Of Double?).ToArray()
            Dim opacityFlattened As Double?() = Nothing
            If (opacity IsNot Nothing) Then
                opacityFlattened = opacity.Cast(Of Double?).ToArray()
            End If
            Me.Min = Double.PositiveInfinity
            Me.Max = Double.NegativeInfinity

            For Each curr As Double? In intensitiesFlattened
                If curr.HasValue AndAlso Double.IsNaN(curr.Value) Then
                    Throw New ArgumentException("Heatmaps do not support intensities of NaN.")
                End If
                If curr.HasValue AndAlso (curr.Value < Me.Min) Then
                    Me.Min = curr.Value
                End If
                If curr.HasValue AndAlso (curr.Value > Me.Max) Then
                    Me.Max = curr.Value
                End If
            Next

            _ColormapMinIsClipped = (ScaleMin.HasValue AndAlso (ScaleMin.Value > Me.Min))
            _ColormapMaxIsClipped = (ScaleMax.HasValue AndAlso (ScaleMax.Value < Me.Max))

            Dim normalizeMin As Double = If(ScaleMin.HasValue AndAlso (ScaleMin.Value < Me.Min), ScaleMin.Value, Me.Min)
            Dim normalizeMax As Double = If(ScaleMax.HasValue AndAlso (ScaleMax.Value > Me.Max), ScaleMax.Value, Me.Max)

            Dim minimumIntensity As Double = If(ScaleMin, Me.Min)
            Dim maximumIntensity As Double = If(ScaleMax, Me.Max)

            If TransparencyThreshold.HasValue Then
                TransparencyThreshold = Normalize(TransparencyThreshold, Me.Min, Me.Max, ScaleMin, ScaleMax)
                minimumIntensity = TransparencyThreshold.Value
            End If

            Dim normalizedIntensities As Double?() = Normalize(intensitiesFlattened, minimumIntensity, maximumIntensity, ScaleMin, ScaleMax)

            Dim flatARGB As Integer()
            If (opacity IsNot Nothing) Then
                flatARGB = ScottPlot.Drawing.Colormap.GetRGBAs(normalizedIntensities, opacityFlattened, Me.Colormap)
            ElseIf (TransparencyThreshold.HasValue) Then
                flatARGB = ScottPlot.Drawing.Colormap.GetRGBAs(normalizedIntensities, Me.Colormap, minimumIntensity)
            Else
                flatARGB = ScottPlot.Drawing.Colormap.GetRGBAs(normalizedIntensities, Me.Colormap, Double.NegativeInfinity)
            End If

            Dim pixelValues As Double?() = Enumerable.Range(0, 256).Select(Function(i) CType(i, Double?)).Reverse().ToArray()
            Dim normalizedValues = Me.Normalize(pixelValues, minimumIntensity, maximumIntensity, ScaleMin, ScaleMax)

            BmpHeatmap?.Dispose()
            BmpHeatmap = New Bitmap(DataWidth, DataHeight, PixelFormat.Format32bppArgb)
            Dim rect As New Rectangle(0, 0, BmpHeatmap.Width, BmpHeatmap.Height)
            Dim bmpData As BitmapData = BmpHeatmap.LockBits(rect, ImageLockMode.ReadWrite, BmpHeatmap.PixelFormat)
            Marshal.Copy(flatARGB, 0, bmpData.Scan0, flatARGB.Length)
            BmpHeatmap.UnlockBits(bmpData)
        End Sub

        ''' <summary>
        ''' This method analyzes the intensities and colormap to create a bitmap with a single pixel for every intensity value. 
        ''' The bitmap is stored and displayed (without anti-alias interpolation) when <see cref="Render(PlotDimensions, Bitmap, Boolean)"/> is called.
        ''' </summary>
        ''' <param name="intensities">2D array of data for the heatmap (all values are shown).</param>
        ''' <param name="colormap">Update the Colormap to use this colormap.</param>
        ''' <param name="min">Minimum intensity (according to the colormap).</param>
        ''' <param name="max">Maximum intensity (according to the colormap).</param>
        ''' <param name="opacity">If defined, this mask indicates the opacity of each cell in the heatmap from 0 (transparent) to 1 (opaque).
        ''' If defined, this array must have the same dimensions as the heatmap array.</param>
        Public Sub Update(intensities As Double(,),
                          Optional colormap As ScottPlot.Drawing.Colormap = Nothing,
                          Optional min As Double? = Nothing,
                          Optional max As Double? = Nothing,
                          Optional opacity As Double?(,) = Nothing)
            Dim finalIntensity As Double?(,) = New Double?(intensities.GetLength(0) - 1, intensities.GetLength(1) - 1) {}
            Dim finalOpacity As Double?(,) = Nothing

            If (opacity IsNot Nothing) Then
                finalOpacity = New Double?(opacity.GetLength(0) - 1, opacity.GetLength(1) - 1) {}
            End If

            For i As Integer = 0 To intensities.GetLength(0) - 1
                For j As Integer = 0 To intensities.GetLength(1) - 1
                    finalIntensity(i, j) = intensities(i, j)

                    If (opacity IsNot Nothing) Then
                        finalOpacity(i, j) = opacity(i, j)
                    End If
                Next
            Next

            Update(finalIntensity, colormap, min, max, finalOpacity)
        End Sub

        ''' <summary>
        ''' Update the heatmap where every cell is given the same color, but with various opacities.
        ''' </summary>
        ''' <param name="color">Single color used for all cells</param>
        ''' <param name="opacity">Opacities (ranging 0-1) for all cells</param>
        Public Sub Update(color As Color, opacity As Double?(,))
            ' limit edge size due to System.Drawing rendering artifacts
            ' https://github.com/ScottPlot/ScottPlot/issues/2119
            Dim maxEdgeLength As Integer = 1 << 15
            If (opacity.GetLength(1) > maxEdgeLength) OrElse (opacity.GetLength(0) > maxEdgeLength) Then
                Throw New ArgumentException($"Due to limitations in rendering large bitmaps, heatmaps cannot have more than {maxEdgeLength:N0} rows or columns.")
            End If

            ' limit total size due to System.Drawing rendering artifacts
            ' https://github.com/ScottPlot/ScottPlot/issues/772
            Dim maxTotalValues As Integer = 10_000_000
            If (opacity.GetLength(1) * opacity.GetLength(0) > maxTotalValues) Then
                Throw New ArgumentException($"Heatmaps may be unreliable for 2D arrays with more than {maxTotalValues:N0} values.")
            End If

            DataWidth = opacity.GetLength(1)
            DataHeight = opacity.GetLength(0)
            Dim opacityFlattened As Double?() = opacity.Cast(Of Double?).ToArray()

            Dim flatARGB As Integer() = ScottPlot.Drawing.Colormap.GetRGBAs(opacityFlattened, color)

            BmpHeatmap?.Dispose()
            BmpHeatmap = New Bitmap(DataWidth, DataHeight, PixelFormat.Format32bppArgb)
            Dim rect As New Rectangle(0, 0, BmpHeatmap.Width, BmpHeatmap.Height)
            Dim bmpData As BitmapData = BmpHeatmap.LockBits(rect, ImageLockMode.ReadWrite, BmpHeatmap.PixelFormat)
            Marshal.Copy(flatARGB, 0, bmpData.Scan0, flatARGB.Length)
            BmpHeatmap.UnlockBits(bmpData)
        End Sub

        Private Function Normalize(input As Double?, Optional min As Double? = Nothing, Optional max As Double? = Nothing, Optional scaleMin As Double? = Nothing, Optional scaleMax As Double? = Nothing) As Double?
            Return Normalize(New Double?() {input}, min, max, scaleMin, scaleMax)(0)
        End Function

        Private Function Normalize(input As Double?(), Optional min As Double? = Nothing, Optional max As Double? = Nothing, Optional scaleMin As Double? = Nothing, Optional scaleMax As Double? = Nothing) As Double?()

            min = If(min, input.Min())
            max = If(max, input.Max())

            min = If(scaleMin.HasValue AndAlso (scaleMin.Value < min), scaleMin.Value, min)
            max = If(scaleMax.HasValue AndAlso (scaleMax.Value > max), scaleMax.Value, max)

            Dim normalized As Double?() = input.AsParallel().AsOrdered().Select(Function(i As Double?)
                                                                                    If i.HasValue Then
                                                                                        Return New Double?((i.Value - min.Value) / (max.Value - min.Value))
                                                                                    End If
                                                                                    Return Nothing
                                                                                End Function).ToArray()

            If scaleMin.HasValue Then
                Dim threshold As Double = (scaleMin.Value - min.Value) / (max.Value - min.Value)
                normalized = normalized.AsParallel().AsOrdered().Select(Function(i) If(i < threshold, threshold, i)).ToArray()
            End If

            If scaleMax.HasValue Then
                Dim threshold As Double = (scaleMax.Value - min.Value) / (max.Value - min.Value)
                normalized = normalized.AsParallel().AsOrdered().Select(Function(i) If(i > threshold, threshold, i)).ToArray()
            End If

            Return normalized
        End Function

        Public Overridable Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return If(BmpHeatmap IsNot Nothing,
                New AxisLimits(OffsetX, OffsetX + DataWidth * CellWidth, OffsetY, OffsetY + DataHeight * CellHeight),
                AxisLimits.NoLimits)
        End Function

        ''' <summary>
        ''' Return the position in the 2D array corresponding to the given coordinate. Returns null if the coordinate is not over the heatmap.
        ''' </summary>
        Public Function GetCellIndexes(x As Double, y As Double) As Tuple(Of Integer?, Integer?)
            Dim xIndex As Integer? = CType((x - OffsetX) / CellWidth, Integer?)
            Dim yIndex As Integer? = CType((y - OffsetY) / CellHeight, Integer?)

            If (xIndex < 0) OrElse (xIndex >= DataWidth) Then
                xIndex = Nothing
            End If

            If (yIndex < 0) OrElse (yIndex >= DataHeight) Then
                yIndex = Nothing
            End If

            Return New Tuple(Of Integer?, Integer?)(xIndex, yIndex)
        End Function

        Public Sub ValidateData(Optional deepValidation As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If (BmpHeatmap is Nothing) Then
                Throw New InvalidOperationException("Update() was not called prior to rendering.")
            End If
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            RenderHeatmap(dims, bmp, lowQuality)
        End Sub

        Protected Overridable Sub RenderHeatmap(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False)
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality)
               Drawing.GDI.ClipIntersection(gfx, dims, ClippingPoints)

                gfx.InterpolationMode = Interpolation
                gfx.PixelOffsetMode = PixelOffsetMode.Half

                Dim fromX As Integer = CInt(Math.Round(dims.GetPixelX(OffsetX)))
                Dim fromY As Integer = CInt(Math.Round(dims.GetPixelY(OffsetY + DataHeight * CellHeight)))
                Dim width As Integer = CInt(Math.Round(dims.GetPixelX(OffsetX + DataWidth * CellWidth) - fromX))
                Dim height As Integer = CInt(Math.Round(dims.GetPixelY(OffsetY) - fromY))

                Dim attr As New ImageAttributes()
                attr.SetWrapMode(WrapMode.TileFlipXY)

                gfx.TranslateTransform(fromX, fromY)

                If FlipVertically Then
                    gfx.ScaleTransform(1, -1)
                End If

                Dim destRect As Rectangle = If(FlipVertically,
                    New Rectangle(0, -height, width, height),
                    New Rectangle(0, 0, width, height))

                gfx.DrawImage(BmpHeatmap,
                              destRect,
                              0,
                              0,
                              BmpHeatmap.Width,
                              BmpHeatmap.Height,
                              GraphicsUnit.Pixel,
                              attr)
            End Using
        End Sub

        Public Overrides Function ToString() As String
            Return $"PlottableHeatmap ({BmpHeatmap.Size})."
        End Function

#End Region '/METHODS

    End Class

End Namespace