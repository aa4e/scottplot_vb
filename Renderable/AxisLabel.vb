Imports System.Drawing

Namespace ScottPlot.Renderable

    Public Class AxisLabel
        Implements IRenderable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Edge of the data area this axis represents.
        ''' </summary>
        Public Edge As Edge

        ''' <summary>
        ''' Axis title.
        ''' </summary>
        Public Label As String = String.Empty

        ''' <summary>
        ''' Font options for the axis title.
        ''' </summary>
        Public Font As New ScottPlot.Drawing.Font() With {.Size = 16}

        ''' <summary>
        ''' Set this field to display a bitmap instead of a text axis label.
        ''' </summary>
        Public ImageLabel As Bitmap = Nothing

        ''' <summary>
        ''' Padding (in pixels) between the image and the edge of the data area.
        ''' </summary>
        Public ImagePaddingToDataArea As Single = 5

        ''' <summary>
        ''' Padding (in pixels) between the image and the edge of the figure.
        ''' </summary>
        Public ImagePaddingToFigureEdge As Single = 5

        ''' <summary>
        ''' Amount of padding (in pixels) to surround the contents of this axis.
        ''' </summary>
        Public PixelSizePadding As Single

        ''' <summary>
        ''' Distance to offset this axis to account for multiple axes.
        ''' </summary>
        Public PixelOffset As Single

        ''' <summary>
        ''' Exact size (in pixels) of the contents of this this axis.
        ''' </summary>
        Public PixelSize As Single

        ''' <summary>
        ''' Controls whether this axis occupies space and is displayed.
        ''' </summary>
        Public Property IsVisible As Boolean = True Implements ScottPlot.Renderable.IRenderable.IsVisible

        ''' <summary>
        ''' Total amount (in pixels) to pad the image when measuring axis size.
        ''' </summary>
        Public ReadOnly Property ImagePadding As Single
            Get
                Return (ImagePaddingToDataArea + ImagePaddingToFigureEdge)
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "METHODS"

        ''' <summary>
        ''' Return the size of the contents of this axis. Returned dimensions are screen-accurate (even if this axis is rotated).
        ''' </summary>
        Public Function Measure() As SizeF
            If (ImageLabel IsNot Nothing) Then
                Return If(Edge = Edge.Bottom OrElse Edge = Edge.Top,
                    New SizeF(ImageLabel.Width, ImageLabel.Height + ImagePadding),
                    New SizeF(ImageLabel.Height, ImageLabel.Width + ImagePadding))
            Else
                Return Drawing.GDI.MeasureStringUsingTemporaryGraphics(Label, Font)
            End If
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Renderable.IRenderable.Render
            If (Not IsVisible) OrElse (String.IsNullOrWhiteSpace(Label) AndAlso ImageLabel is Nothing) Then
                Return
            End If

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, False)
                Dim center = GetAxisCenter(dims)
                If (ImageLabel IsNot Nothing) Then
                    RenderImageLabel(gfx, dims, center.Item1, center.Item2)
                    Return
                End If
                If (Font.Rotation <> 0) Then
                    RenderTextLabelRotated(gfx, dims, center.Item1, center.Item2)
                    Return
                End If
                RenderTextLabel(gfx, dims, center.Item1, center.Item2)
            End Using
        End Sub

        Private Sub RenderImageLabel(gfx As Graphics, dims As PlotDimensions, x As Single, y As Single)
            'TODO: use ImagePadding instead of fractional padding
            Dim xOffset As Single
            Select Case Me.Edge
                Case Edge.Left
                    xOffset = ImagePaddingToFigureEdge
                Case Edge.Right
                    xOffset = -ImageLabel.Width - ImagePaddingToFigureEdge
                Case Edge.Bottom
                    xOffset = -ImageLabel.Width
                Case Edge.Top
                    xOffset = -ImageLabel.Width
                Case Else
                    Throw New NotImplementedException()
            End Select

            Dim yOffset As Single
            Select Case Me.Edge
                Case Edge.Left
                    yOffset = -ImageLabel.Height
                Case Edge.Right
                    yOffset = -ImageLabel.Height
                Case Edge.Bottom
                    yOffset = -ImageLabel.Height - ImagePaddingToFigureEdge
                Case Edge.Top
                    yOffset = 0 + ImagePaddingToFigureEdge
                Case Else
                    Throw New NotImplementedException()
            End Select

            gfx.TranslateTransform(x, y)
            gfx.DrawImage(ImageLabel, xOffset, yOffset)
            Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
        End Sub

        Private Sub RenderTextLabel(gfx As Graphics, dims As PlotDimensions, x As Single, y As Single)
            Dim padding As Single = If(Edge = Edge.Bottom, -PixelSizePadding, PixelSizePadding)

            Dim rotation As Integer
            Select Case Me.Edge
                Case Edge.Left
                    rotation = -90
                Case Edge.Right
                    rotation = 90
                Case Edge.Bottom
                    rotation = 0
                Case Edge.Top
                    rotation = 0
                Case Else
                    Throw New NotImplementedException()
            End Select

            Using fnt = Drawing.GDI.Font(Font),
                brush = Drawing.GDI.Brush(Font.Color),
                sf = Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Lower)
                Select Case Edge
                    Case Edge.Left
                        sf.LineAlignment = StringAlignment.Near
                    Case Edge.Right
                        sf.LineAlignment = StringAlignment.Near
                    Case Edge.Bottom
                        sf.LineAlignment = StringAlignment.Far
                    Case Edge.Top
                        sf.LineAlignment = StringAlignment.Near
                    Case Else
                        Throw New NotImplementedException()
                End Select

                gfx.TranslateTransform(x, y)
                gfx.RotateTransform(rotation)
                gfx.DrawString(Label, fnt, brush, 0, padding, sf)
                Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
            End Using
        End Sub

        Private Sub RenderTextLabelRotated(gfx As Graphics, dims As PlotDimensions, x As Single, y As Single)
            Using fnt As System.Drawing.Font = Drawing.GDI.Font(Font),
                brush As Brush = Drawing.GDI.Brush(Font.Color)

                gfx.TranslateTransform(x, y)

                Select Case Edge
                    Case Edge.Right
                        If (Font.Rotation <> 90) Then
                            Throw New NotImplementedException("Right axis label rotation must be 0 or 90.")
                        End If
                        Using sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Lower)
                            gfx.RotateTransform(-Font.Rotation)
                            gfx.DrawString(Label, fnt, brush, 0, 0, sf)
                        End Using
                    Case Edge.Left
                        If (Font.Rotation <> 90) Then
                            Throw New NotImplementedException("Left axis label rotation must be 0 or 90.")
                        End If
                        Using sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Upper)
                            gfx.RotateTransform(-Font.Rotation)
                            gfx.DrawString(Label, fnt, brush, 0, 0, sf)
                        End Using
                    Case Edge.Bottom
                        If (Font.Rotation <> 180) Then
                            Throw New NotImplementedException("Bottom axis label rotation must be 0 or 180.")
                        End If
                        Using sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Upper)
                            gfx.RotateTransform(-Font.Rotation)
                            gfx.DrawString(Label, fnt, brush, 0, 0, sf)
                        End Using
                    Case Edge.Top
                        If (Font.Rotation <> 180) Then
                            Throw New NotImplementedException("Top axis label rotation must be 0 or 180.")
                        End If
                        Using sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Lower)
                            gfx.RotateTransform(-Font.Rotation)
                            gfx.DrawString(Label, fnt, brush, 0, 0, sf)
                        End Using
                    Case Else
                        Throw New NotImplementedException(Edge.ToString())
                End Select
                Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
            End Using
        End Sub

        ''' <summary>
        ''' Return the point and rotation representing the center of the base of this axis.
        ''' </summary>
        Private Function GetAxisCenter(dims As PlotDimensions) As Tuple(Of Single, Single)
            Dim x As Single
            Select Case Edge
                Case Edge.Left
                    x = dims.DataOffsetX - PixelOffset - PixelSize
                Case Edge.Right
                    x = dims.DataOffsetX + dims.DataWidth + PixelOffset + PixelSize
                Case Edge.Bottom
                    x = dims.DataOffsetX + dims.DataWidth / 2
                Case Edge.Top
                    x = dims.DataOffsetX + dims.DataWidth / 2
                Case Else
                    Throw New NotImplementedException()
            End Select

            Dim y As Single
            Select Case Edge
                Case Edge.Left
                    y = dims.DataOffsetY + dims.DataHeight / 2
                Case Edge.Right
                    y = dims.DataOffsetY + dims.DataHeight / 2
                Case Edge.Bottom
                    y = dims.DataOffsetY + dims.DataHeight + PixelOffset + PixelSize
                Case Edge.Top
                    y = dims.DataOffsetY - PixelOffset - PixelSize
                Case Else
                    Throw New NotImplementedException()
            End Select
            Return New Tuple(Of Single, Single)(x, y)
        End Function

#End Region '/METHODS

    End Class

End Namespace