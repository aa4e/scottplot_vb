Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Namespace ScottPlot

    Partial Class Plot

#Region "PRIMARY RENDER SYSTEM"

        ''' <summary>
        ''' Render the plot onto an existing bitmap.
        ''' </summary>
        ''' <param name="bmp">An existing bitmap to render onto.</param>
        ''' <param name="scale">Scale the size of the output image by this fraction (without resizing the plot).</param>
        ''' <returns>The same bitmap that was passed in (but was rendered onto).</returns>
        Public Function Render(bmp As Bitmap, Optional lowQuality As Boolean = False, Optional scale As Double = 1) As Bitmap
            SyncLock Me.RenderLocker
                Settings.BenchmarkMessage.Restart()
                Settings.Resize(CSng(bmp.Width / scale), CSng(bmp.Height / scale))
                Settings.CopyPrimaryLayoutToAllAxes()
                Settings.AxisAutoUnsetAxes()
                Settings.EnforceEqualAxisScales()
                Settings.LayoutAuto()
                If (Settings.EqualScaleMode <> EqualScaleMode.Disabled) Then
                    Settings.EnforceEqualAxisScales()
                    Settings.LayoutAuto()
                End If

                Dim plotDimensions As PlotDimensions = Settings.GetPlotDimensions(0, 0, scale)
                RenderClear(bmp, lowQuality, plotDimensions)

                If (plotDimensions.DataWidth > 0) AndAlso (plotDimensions.DataHeight > 0) Then
                    RenderBeforePlottables(bmp, lowQuality, plotDimensions)
                    RenderPlottables(bmp, lowQuality, scale)
                    RenderAfterPlottables(bmp, lowQuality, plotDimensions)
                End If
            End SyncLock

            Return bmp
        End Function

        Private Sub RenderClear(bmp As Bitmap, lowQuality As Boolean, primaryDims As PlotDimensions)
            Settings.FigureBackground.Render(primaryDims, bmp, lowQuality)
        End Sub

        Private Sub RenderBeforePlottables(bmp As Bitmap, lowQuality As Boolean, dims As PlotDimensions)
            Settings.DataBackground.Render(dims, bmp, lowQuality)
            If (Not Settings.DrawGridAbovePlottables) Then
                RenderAxes(bmp, lowQuality, dims)
            End If
        End Sub

        Private Sub RenderPlottables(bmp As Bitmap, lowQuality As Boolean, scaleFactor As Double)
            For Each plottable As Plottable.IPlottable In Settings.Plottables
                If plottable.IsVisible Then
                    plottable.ValidateData(False)

                    Dim dims As PlotDimensions
                    If (plottable IsNot Nothing) Then
                        dims = Settings.GetPlotDimensions(plottable.XAxisIndex, plottable.YAxisIndex, scaleFactor)
                    Else
                        dims = Settings.GetPlotDimensions(0, 0, scaleFactor)
                    End If

                    If Double.IsInfinity(dims.XSpan) OrElse Double.IsInfinity(dims.YSpan) Then
                        Throw New InvalidOperationException($"Axis limits cannot be infinite: {dims}.")
                    End If
                    Try
                        plottable.Render(dims, bmp, lowQuality)
                    Catch ex As OverflowException
                        'This exception is commonly thrown by System.Drawing when drawing to extremely large pixel locations.
                        If Settings.IgnoreOverflowExceptionsDuringRender Then
                            Diagnostics.Debug.WriteLine($"OverflowException plotting: {plottable}.")
                        Else
                            Throw New OverflowException("Overflow during render", ex)
                        End If
                    End Try
                End If
            Next
        End Sub

        Private Sub RenderAfterPlottables(bmp As Bitmap, lowQuality As Boolean, dims As PlotDimensions)
            If Settings.DrawGridAbovePlottables Then
                RenderAxes(bmp, lowQuality, dims)
            End If

            Settings.CornerLegend.UpdateLegendItems(Me, False)

            If Settings.CornerLegend.IsVisible Then
                Settings.CornerLegend.Render(dims, bmp, lowQuality)
            End If

            Settings.BenchmarkMessage.Stop()

            Settings.ZoomRectangle.Render(dims, bmp, lowQuality)
            Settings.BenchmarkMessage.Render(dims, bmp, lowQuality)
            Settings.ErrorMessage.Render(dims, bmp, lowQuality)
        End Sub

        Private Sub RenderAxes(bmp As Bitmap, lowQuality As Boolean, dims As PlotDimensions)
            For Each axis As Renderable.Axis In Settings.Axes
                Dim dims2 As PlotDimensions = If(axis.IsHorizontal,
                    Settings.GetPlotDimensions(axis.AxisIndex, 0, dims.ScaleFactor),
                    Settings.GetPlotDimensions(0, axis.AxisIndex, dims.ScaleFactor))
                Try
                    axis.Render(dims2, bmp, lowQuality)
                Catch ex As OverflowException
                    Throw New InvalidOperationException("Data cannot contain Infinity.")
                End Try
            Next
        End Sub

#End Region '/PRIMARY RENDER SYSTEM

#Region "RENDER LOCK"

        ''' <summary>
        ''' This object is locked by the render loop.
        ''' Locking this externally will prevent renders until it is released.
        ''' </summary>
        Private ReadOnly RenderLocker As New Object()

        ''' <summary>
        ''' Wait for the current render to finish, then prevent future renders until <see cref="RenderUnlock()"/> is called.
        ''' Locking rendering is required if you intend to modify plottables while rendering is occurring in another thread.
        ''' </summary>
        Public Sub RenderLock()
            Threading.Monitor.Enter(RenderLocker)
        End Sub

        ''' <summary>
        ''' Release the render lock, allowing renders to proceed.
        ''' </summary>
        Public Sub RenderUnlock()
            Threading.Monitor.Exit(RenderLocker)
        End Sub

#End Region '/RENDER LOCK

#Region "RENDER HELPER METHODS"

        ''' <summary>
        ''' Render the plot onto a new bitmap (using the size given when the plot was created or resized).
        ''' </summary>
        ''' <param name="lowQuality">if true, anti-aliasing will be disabled for this render.</param>
        Public Function Render(Optional lowQuality As Boolean = False) As Bitmap
            Return Render(Settings.Width, Settings.Height, lowQuality)
        End Function

        ''' <summary>
        ''' Render the plot onto a new bitmap of the given dimensions.
        ''' </summary>
        ''' <param name="width">Resize the plot to this width (pixels) before rendering.</param>
        ''' <param name="height">Resize the plot to this height (pixels) before rendering.</param>
        ''' <param name="lowQuality">If true, anti-aliasing will be disabled for this render.</param>
        ''' <param name="scale">Scale the size of the output image by this fraction (without resizing the plot).</param>
        Public Function Render(width As Integer, height As Integer,
                               Optional lowQuality As Boolean = False, Optional scale As Double = 1) As Bitmap
            'allow a bitmap to always be created even if invalid dimensions are provided
            width = Math.Max(1, CInt(width * scale))
            height = Math.Max(1, CInt(height * scale))
            Dim bitmap As New Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
            Render(bitmap, lowQuality, scale)
            Return bitmap
        End Function

        ''' <summary>
        ''' Create a new Bitmap, render the plot onto it, and return it.
        ''' </summary>
        Public Function GetBitmap(Optional lowQuality As Boolean = False, Optional scale As Double = 1) As Bitmap
            Return Render(Settings.Width, Settings.Height, lowQuality, scale)
        End Function

        ''' <summary>
        ''' Render the plot and return the bytes for a PNG file.
        ''' This method is useful for rendering in stateless cloud environments that do Not use a traditional filesystem.
        ''' </summary>
        Public Function GetImageBytes(Optional lowQuality As Boolean = False, Optional scale As Double = 1) As Byte()
            Dim imageBytes As Byte()
            Using ms As New IO.MemoryStream()
                Dim bmp As Bitmap = GetBitmap(lowQuality, scale)
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                imageBytes = ms.ToArray()
            End Using
            Return imageBytes
        End Function

        ''' <summary>
        ''' Render the plot and return the image as a Bas64-encoded PNG.
        ''' </summary>
        Public Function GetImageBase64(Optional lowQuality As Boolean = False, Optional scale As Double = 1) As String
            Return Convert.ToBase64String(Me.GetImageBytes(lowQuality, scale))
        End Function

        ''' <summary>
        ''' Render the plot and return an HTML img element containing a Base64-encoded PNG.
        ''' </summary>
        Public Function GetImageHTML(Optional lowQuality As Boolean = False, Optional scale As Double = 1) As String
            Dim b64 As String = GetImageBase64(lowQuality, scale)
            Return $"<img src=""data:image/png;base64,{b64}"" />"
        End Function

        ''' <summary>
        ''' Return a new Bitmap containing only the legend.
        ''' </summary>
        Public Function RenderLegend(Optional lowQuality As Boolean = False, Optional scale As Double = 1) As Bitmap
            Render(lowQuality)
            Dim originalEdgeColor As Color = Settings.CornerLegend.OutlineColor
            Settings.CornerLegend.OutlineColor = Color.Transparent
            Dim bmp As Bitmap = Settings.CornerLegend.GetBitmap(lowQuality, scale)
            Settings.CornerLegend.OutlineColor = originalEdgeColor
            Return bmp
        End Function

        <Obsolete("Use RenderLegend()", True)>
        Public Function GetLegendBitmap(Optional lowQuality As Boolean = False) As Bitmap
            Return RenderLegend()
        End Function

        ''' <summary>
        ''' Save the plot as an image.
        ''' </summary>
        ''' <param name="filePath">File path for the images (existing files will be overwritten).</param>
        ''' <param name="width">Resize the plot to this width (pixels) before rendering.</param>
        ''' <param name="height">Resize the plot to this height (pixels) before rendering.</param>
        ''' <param name="lowQuality">If true, anti-aliasing will be disabled for this render. Default false.</param>
        ''' <param name="scale">Scale the size of the output image by this fraction (without resizing the plot).</param>
        ''' <returns>Full path for the image that was saved.</returns>
        Public Function SaveFig(filePath As String, Optional width As Integer? = Nothing, Optional height As Integer? = Nothing, Optional lowQuality As Boolean = False, Optional scale As Double = 1.0) As String
            Dim bmp As System.Drawing.Image = Render(If(width, Settings.Width),
                                                     If(height, Settings.Height),
                                                     lowQuality, scale)
            filePath = IO.Path.GetFullPath(filePath)
            Dim fileFolder As String = IO.Path.GetDirectoryName(filePath)
            If (Not IO.Directory.Exists(fileFolder)) Then
                Throw New Exception($"ERROR: folder does not exist: {fileFolder}.")
            End If

            Dim format As ImageFormat
            Dim extension As String = Path.GetExtension(filePath).ToUpper()
            Select Case extension
                Case ".JPG", ".JPEG"
                    format = ImageFormat.Jpeg
                Case ".PNG"
                    format = ImageFormat.Png
                Case ".TIF", ".TIFF"
                    format = ImageFormat.Tiff
                Case ".BMP"
                    format = ImageFormat.Bmp
                Case Else
                    Throw New NotImplementedException($"Extension not supported: {extension}.")
            End Select

            bmp.Save(filePath, format)
            Return filePath
        End Function

#End Region '/RENDER HELPER METHODS

    End Class

End Namespace