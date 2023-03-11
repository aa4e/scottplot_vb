Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Linq

Namespace ScottPlot.Renderable

    Public Class Legend
        Implements IRenderable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' List of items appearing in the legend during the last render.
        ''' </summary>
        Private LegendItems As Plottable.LegendItem() = New Plottable.LegendItem() {}

        ''' <summary>
        ''' Number of items appearing in the legend during the last render.
        ''' </summary>
        Public ReadOnly Property Count As Integer
            Get
                If (LegendItems IsNot Nothing) Then
                    Return LegendItems.Length
                End If
                Return 0
            End Get
        End Property

        ''' <summary>
        ''' Returns true if the legend contained items during the last render.
        ''' </summary>
        Public ReadOnly Property HasItems As Boolean
            Get
                Return (LegendItems IsNot Nothing) AndAlso (LegendItems.Length <> 0)
            End Get
        End Property

        Public Property Location As Alignment = Alignment.LowerRight
        Public Property FixedLineWidth As Boolean = False
        Public Property ReverseOrder As Boolean = False
        Public Property AntiAlias As Boolean = True
        Public Property IsVisible As Boolean = False Implements ScottPlot.Renderable.IRenderable.IsVisible
        Public Property IsDetached As Boolean = False

        Public Property FillColor As Color = Color.White
        Public Property OutlineColor As Color = Color.Black
        Public Property ShadowColor As Color = Color.FromArgb(50, Color.Black)
        Public Property ShadowOffsetX As Single = 2
        Public Property ShadowOffsetY As Single = 2

        Public Property Orientation As Orientation = Orientation.Vertical

        Public ReadOnly Property Font As New ScottPlot.Drawing.Font()

        Public WriteOnly Property FontName As String
            Set(value As String)
                Font.Name = value
            End Set
        End Property

        Public WriteOnly Property FontSize As Single
            Set(value As Single)
                Font.Size = value
            End Set
        End Property

        Public WriteOnly Property FontColor As Color
            Set(value As Color)
                Font.Color = value
            End Set
        End Property

        Public WriteOnly Property FontBold As Boolean
            Set(value As Boolean)
                Font.Bold = value
            End Set
        End Property

        Public Property Padding As Single = 5

        Private ReadOnly Property SymbolWidth As Single
            Get
                Return (40 * Font.Size / 12)
            End Get
        End Property

        Private ReadOnly Property SymbolPad As Single
            Get
                Return (Font.Size / 3)
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Renderable.IRenderable.Render
            If (Not IsVisible) OrElse (LegendItems is Nothing) OrElse (LegendItems.Length = 0) Then
                Return
            End If
            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, False),
                fnt As System.Drawing.Font =Drawing.GDI.Font(Font)
                Dim dimensions = GetDimensions(gfx, LegendItems, fnt)
                Dim locationPx = GetLocationPx(dims, dimensions.Item3, dimensions.Item4)
                Render(gfx, LegendItems, fnt, locationPx.Item1, locationPx.Item2, dimensions.Item3, dimensions.Item4, dimensions.Item2)
            End Using
        End Sub

        ''' <summary>
        ''' Creates and returns a Bitmap containing all legend items displayed at the last render.
        ''' This will be 1px transparent Bitmap if no render was performed previously Or if there are no legend items.
        ''' </summary>
        Public Function GetBitmap(Optional lowQuality As Boolean = False, Optional scale As Double = 1) As Bitmap
            If (LegendItems.Length = 0) Then
                Return New Bitmap(1, 1)
            End If

            'use a temporary bitmap and graphics (without scaling) to measure how large the final image should be
            Using tempBmp As New Bitmap(1, 1),
                tempGfx As Graphics =Drawing.GDI.Graphics(tempBmp, lowQuality, scale),
                legendFont As System.Drawing.Font =Drawing.GDI.Font(Font)
                Dim dimensions As Tuple(Of Single, Single, Single, Single) = GetDimensions(tempGfx, LegendItems, legendFont)

                'create the actual legend bitmap based on the scaled measured size
                Dim width As Integer = CInt(dimensions.Item3 * scale)
                Dim height As Integer = CInt(dimensions.Item4 * scale)

                Dim bmp As New Bitmap(width, height, PixelFormat.Format32bppPArgb)
                Using gfx As Graphics =Drawing.GDI.Graphics(bmp, lowQuality, scale)
                    Render(gfx, LegendItems, legendFont, 0, 0, dimensions.Item3, dimensions.Item4, dimensions.Item2)
                End Using
                Return bmp
            End Using
        End Function

        Private Function GetDimensions(gfx As Graphics, items As Plottable.LegendItem(), font As System.Drawing.Font) As Tuple(Of Single, Single, Single, Single)
            'determine maximum label size and use it to define legend size
            Dim maxLabelWidth As Single = 0
            Dim maxLabelHeight As Single = 0
            Dim totalLegendWidthWhenHorizontal As Single = 0

            For i As Integer = 0 To items.Length - 1
                Dim sizeF As SizeF = gfx.MeasureString(items(i).Label, font)
                maxLabelWidth = Math.Max(maxLabelWidth, sizeF.Width)
                totalLegendWidthWhenHorizontal += Me.SymbolWidth + sizeF.Width + Me.SymbolPad
                maxLabelHeight = Math.Max(maxLabelHeight, sizeF.Height)
            Next

            Dim width As Single = 0
            Dim height As Single = 0
            If (Orientation = Orientation.Vertical) Then
                width = SymbolWidth + maxLabelWidth + SymbolPad
                height = maxLabelHeight * items.Length
            ElseIf (Orientation = Orientation.Horizontal) Then
                width = totalLegendWidthWhenHorizontal
                height = maxLabelHeight
            End If
            Return New Tuple(Of Single, Single, Single, Single)(maxLabelWidth, maxLabelHeight, width, height)
        End Function

        Private Sub Render(gfx As Graphics, items As Plottable.LegendItem(), font As System.Drawing.Font,
                           locationX As Single, locationY As Single, width As Single, height As Single,
                           maxLabelHeight As Single, Optional shadow As Boolean = True, Optional outline As Boolean = True)
            Using fillBrush As New SolidBrush(FillColor),
                shadowBrush As New SolidBrush(ShadowColor),
                textBrush As New SolidBrush(Me.Font.Color),
                outlinePen As New Pen(OutlineColor),
                legendItemHideBrush As Brush = Drawing.GDI.Brush(FillColor, 1)

                Dim rectShadow As New RectangleF(locationX + ShadowOffsetX, locationY + ShadowOffsetY, width, height)
                Dim rectFill As New RectangleF(locationX, locationY, width, height)

                If shadow Then
                    gfx.FillRectangle(shadowBrush, rectShadow)
                End If

                gfx.FillRectangle(fillBrush, rectFill)

                If outline Then
                    gfx.DrawRectangle(outlinePen, Rectangle.Round(rectFill))
                End If

                Dim offsetX As Single = 0
                Dim offsetY As Single = 0
                For i As Integer = 0 To items.Length - 1
                    Dim labelSize As SizeF = gfx.MeasureString(items(i).Label, font)
                    Dim legendItem As Plottable.LegendItem = items(i)

                    Dim itemStartXLocation As Single = locationX + offsetX
                    Dim itemStartYLocation As Single = locationY + offsetY

                    legendItem.Render(gfx, itemStartXLocation, itemStartYLocation,
                                      labelSize.Width, maxLabelHeight, font,
                                      SymbolWidth, SymbolPad, outlinePen, textBrush, legendItemHideBrush)

                    If (Orientation = Orientation.Vertical) Then
                        offsetY += maxLabelHeight
                    ElseIf (Orientation = Orientation.Horizontal) Then
                        offsetX += SymbolWidth + labelSize.Width + SymbolPad
                    End If
                Next
            End Using
        End Sub

        Public Sub UpdateLegendItems(plot As Plot, Optional includeHidden As Boolean = False)
            LegendItems = plot.GetPlottables() _
                .Where(Function(x) x.IsVisible OrElse includeHidden) _
                .Where(Function(x) x.GetLegendItems() IsNot Nothing) _
                .SelectMany(Function(x) x.GetLegendItems()) _
                .Where(Function(x) Not String.IsNullOrWhiteSpace(x.label)).ToArray()

            If ReverseOrder Then
                Array.Reverse(LegendItems)
            End If
        End Sub

        ''' <summary>
        ''' Returns an array of legend items displayed in the last render.
        ''' </summary>
        Public Function GetItems() As Plottable.LegendItem()
            Return LegendItems.ToArray()
        End Function

        ''' <summary>
        ''' Returns an array of legend items displayed in the last render.
        ''' </summary>
        Private Function GetLocationPx(dims As PlotDimensions, width As Single, height As Single) As Tuple(Of Single, Single)
            Dim leftX As Single = dims.DataOffsetX + Padding
            Dim rightX As Single = dims.DataOffsetX + dims.DataWidth - Padding - width
            Dim centerX As Single = dims.DataOffsetX + dims.DataWidth / 2 - width / 2

            Dim topY As Single = dims.DataOffsetY + Padding
            Dim bottomY As Single = dims.DataOffsetY + dims.DataHeight - Padding - height
            Dim centerY As Single = dims.DataOffsetY + dims.DataHeight / 2 - height / 2

            Select Case Location
                Case Alignment.UpperLeft
                    Return New Tuple(Of Single, Single)(leftX, topY)
                Case Alignment.UpperRight
                    Return New Tuple(Of Single, Single)(rightX, topY)
                Case Alignment.UpperCenter
                    Return New Tuple(Of Single, Single)(centerX, topY)
                Case Alignment.MiddleLeft
                    Return New Tuple(Of Single, Single)(leftX, centerY)
                Case Alignment.MiddleCenter
                    Return New Tuple(Of Single, Single)(centerX, centerY)
                Case Alignment.MiddleRight
                    Return New Tuple(Of Single, Single)(rightX, centerY)
                Case Alignment.LowerLeft
                    Return New Tuple(Of Single, Single)(leftX, bottomY)
                Case Alignment.LowerRight
                    Return New Tuple(Of Single, Single)(rightX, bottomY)
                Case Alignment.LowerCenter
                    Return New Tuple(Of Single, Single)(centerX, bottomY)
                Case Else
                    Throw New NotImplementedException()
            End Select
        End Function

#End Region '/METHODS

    End Class

End Namespace