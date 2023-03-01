Imports System.Linq
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Drawing.Text

Namespace ScottPlot.Drawing

    Public Module GDI

#Const LINUX = False
#Const OSX = False

#If LINUX Then
        Private Const xMultiplierLinux As Single = 1
        Private Const yMultiplierLinux As Single = 27.16 / 22
#End If

#If OSX Then
        Private Const xMultiplierMacOS As Single = 82.82 / 72
        Private Const yMultiplierMacOS As Single = 27.16 / 20
#End If

        Private HighQualityTextRenderingHint As TextRenderingHint = TextRenderingHint.AntiAlias
        Private LowQualityTextRenderingHint As TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit

        ''' <summary>
        ''' Return the display scale ratio being used. A scaling ratio of 1.0 means scaling is nsot active.
        ''' </summary>
        Public Function GetScaleRatio() As Single
            Const DEFAULT_DPI As Integer = 96
            Using bmp As New Bitmap(1, 1), gfx As Graphics = Drawing.GDI.Graphics(bmp)
                Return gfx.DpiX / DEFAULT_DPI
            End Using
        End Function

        ''' <summary>
        ''' Create a Bitmap And Graphics And use it to measure a string.
        ''' Only use this function if an existing Graphics does Not exist.
        ''' </summary>
        Public Function MeasureStringUsingTemporaryGraphics(text As String, font As Font) As SizeF
            Using bmp As New Bitmap(1, 1), gfx As Graphics = Graphics(bmp, True)
                Return MeasureString(gfx, text, Nothing, font.Size, font.Bold, font.Family)
            End Using
        End Function

        ''' <summary>
        ''' Return the size (in pixels) of the given string.
        ''' </summary>
        Public Function MeasureString(gfx As Graphics, text As String, font As Font) As SizeF
            Return MeasureString(gfx, text, Nothing, font.Size, font.Bold, font.Family)
        End Function

        ''' <summary>
        ''' Return the size (in pixels) of the given string.
        ''' If <paramref name="fontFamily"/> Is provided it will be used instead of <paramref name="fontName"/>.
        ''' </summary>
        Public Function MeasureString(gfx As Graphics, text As String, fontName As String, fontSize As Double, Optional bold As Boolean = False, Optional fontFamily As FontFamily = Nothing) As SizeF
            If (fontFamily IsNot Nothing) Then
                fontFamily = InstalledFont.ValidFontFamily(fontName) 'TEST:  fontFamily ??= InstalledFont.ValidFontFamily(fontName);
            End If
            Dim fontStyle As FontStyle = If(bold, FontStyle.Bold, FontStyle.Regular)
            Using font As New System.Drawing.Font(fontFamily, CSng(fontSize), fontStyle, GraphicsUnit.Pixel)
                Return MeasureString(gfx, text, font)
            End Using
        End Function

        ''' <summary>
        ''' Return the size (in pixels) of the given string.
        ''' </summary>
        Public Function MeasureString(gfx As Graphics, text As String, font As System.Drawing.Font) As SizeF
            Dim sz As SizeF = gfx.MeasureString(text, font)

            'Compensate for OS-specific differences in font scaling
#If LINUX Then
            sz.Width *= xMultiplierLinux
            sz.Height *= yMultiplierLinux
#End If
#If OSX Then
            sz.Width *= xMultiplierMacOS
            sz.Height *= yMultiplierMacOS
#End If
            'If RuntimeInformation.IsOSPlatform(OSPlatform.Linux) Then
            '    sz.Width *= xMultiplierLinux
            '    sz.Height *= yMultiplierLinux
            'ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
            '    sz.Width *= xMultiplierMacOS
            '    sz.Height *= yMultiplierMacOS
            'End If

            'ensure the measured height Is at least the font size
            sz.Height = Math.Max(font.Size, sz.Height)

            Return sz
        End Function

        Private Function AlignmentFraction(alignment As Alignment) As Tuple(Of Single, Single)
            Select Case alignment
                Case Alignment.UpperLeft
                    Return New Tuple(Of Single, Single)(0, 0)
                Case Alignment.UpperRight
                    Return New Tuple(Of Single, Single)(1, 0)
                Case Alignment.UpperCenter
                    Return New Tuple(Of Single, Single)(0.5, 0)
                Case Alignment.MiddleLeft
                    Return New Tuple(Of Single, Single)(0, 0.5)
                Case Alignment.MiddleCenter
                    Return New Tuple(Of Single, Single)(0.5, 0.5)
                Case Alignment.MiddleRight
                    Return New Tuple(Of Single, Single)(1, 0.5)
                Case Alignment.LowerLeft
                    Return New Tuple(Of Single, Single)(0, 1)
                Case Alignment.LowerRight
                    Return New Tuple(Of Single, Single)(1, 1)
                Case Alignment.LowerCenter
                    Return New Tuple(Of Single, Single)(0.5, 1)
                Case Else
                    Throw New NotImplementedException()
            End Select
        End Function

        ''' <summary>
        ''' Return the X and Y distance (pixels) necessary to translate the canvas for the given text/font/alignment.
        ''' </summary>
        Public Function TranslateString(gfx As Graphics, text As String, font As Font) As Tuple(Of Single, Single)
            Dim stringSize As SizeF = Drawing.GDI.MeasureString(gfx, text, font.Name, font.Size, font.Bold, font.Family)
            Dim t = AlignmentFraction(font.Alignment)
            Return New Tuple(Of Single, Single)(stringSize.Width * t.Item1, stringSize.Height * t.Item2)
        End Function

        Public Function Mix(colorA As Color, colorB As Color, fracA As Double) As Color
            Dim r As Byte = CByte((colorA.R * (1 - fracA)) + colorB.R * fracA)
            Dim g As Byte = CByte((colorA.G * (1 - fracA)) + colorB.G * fracA)
            Dim b As Byte = CByte((colorA.B * (1 - fracA)) + colorB.B * fracA)
            Return System.Drawing.Color.FromArgb(r, g, b)
        End Function

        Public Function Mix(hexA As String, hexB As String, fracA As Double) As Color
            Dim colorA As Color = ColorTranslator.FromHtml(hexA)
            Dim colorB As Color = ColorTranslator.FromHtml(hexB)
            Return Drawing.GDI.Mix(colorA, colorB, fracA)
        End Function

        ''' <summary>
        ''' Controls whether <see cref="TextRenderingHint.ClearTypeGridFit"/> (instead of the Default <see cref="TextRenderingHint.AntiAlias"/>) hinting will be used.
        ''' ClearType typically appears superior except When rendered above a transparent background.
        ''' </summary>
        Public Sub ClearType(enable As Boolean)
            Drawing.GDI.HighQualityTextRenderingHint = If(enable, TextRenderingHint.ClearTypeGridFit, TextRenderingHint.AntiAlias)
        End Sub

        Public Function Graphics(bmp As Bitmap, Optional lowQuality As Boolean = False, Optional scale As Double = 1.0) As Graphics
            Dim gfx As Graphics = System.Drawing.Graphics.FromImage(bmp)
            gfx.SmoothingMode = If(lowQuality, SmoothingMode.HighSpeed, SmoothingMode.AntiAlias)
            gfx.TextRenderingHint = If(lowQuality, Drawing.GDI.LowQualityTextRenderingHint, Drawing.GDI.HighQualityTextRenderingHint)
            gfx.ScaleTransform(CSng(scale), CSng(scale))
            Return gfx
        End Function

        Public Function Graphics(bmp As Bitmap, dims As PlotDimensions, Optional lowQuality As Boolean = False, Optional clipToDataArea As Boolean = True) As Graphics
            Dim gfx As Graphics = Drawing.GDI.Graphics(bmp, lowQuality, dims.ScaleFactor)
            Dim isFrameless As Boolean = (dims.DataWidth = dims.Width) AndAlso (dims.DataHeight = dims.Height)
            If clipToDataArea AndAlso (Not isFrameless) Then
                'These dimensions are withdrawn by 1 pixel to leave room for a 1px wide data frame.
                'Rounding Is intended to exactly match rounding used when frame placement Is determined.
                Dim left As Single = CSng(Math.Round(dims.DataOffsetX + 1))
                Dim top As Single = CSng(Math.Round(dims.DataOffsetY + 1))
                Dim width As Single = CSng(Math.Round(dims.DataWidth - 1))
                Dim height As Single = CSng(Math.Round(dims.DataHeight - 1))
                gfx.Clip = New Region(New RectangleF(left, top, width, height))
            End If
            Return gfx
        End Function

        Public Function Pen(color As System.Drawing.Color, Optional width As Double = 1.0, Optional lineStyle As LineStyle = LineStyle.Solid, Optional rounded As Boolean = False) As Pen
            Dim p As New System.Drawing.Pen(color, CSng(width))
            If (lineStyle = LineStyle.Solid) OrElse (lineStyle = LineStyle.None) Then
                'WARNING: Do Not apply a solid DashPattern!
                'Setting DashPattern automatically sets a pen's DashStyle to custom.
                'Custom DashStyles are slower And can cause diagonal rendering artifacts.
                'Instead use the solid DashStyle.
                'https://github.com/ScottPlot/ScottPlot/issues/327
                'https://github.com/ScottPlot/ScottPlot/issues/401
                p.DashStyle = DashStyle.Solid

            ElseIf lineStyle = LineStyle.Dash Then
                p.DashPattern = New Single() {8.0F, 4.0F}

            ElseIf lineStyle = LineStyle.DashDot Then
                p.DashPattern = New Single() {8.0F, 4.0F, 2.0F, 4.0F}

            ElseIf lineStyle = LineStyle.DashDotDot Then
                p.DashPattern = New Single() {8.0F, 4.0F, 2.0F, 4.0F, 2.0F, 4.0F}

            ElseIf lineStyle <> LineStyle.Dot Then
                p.DashPattern = New Single() {2.0F, 4.0F}

            Else
                Throw New NotImplementedException("Line style not supported.")
            End If

            If rounded Then
                p.StartCap = System.Drawing.Drawing2D.LineCap.Round
                p.EndCap = System.Drawing.Drawing2D.LineCap.Round
                p.LineJoin = System.Drawing.Drawing2D.LineJoin.Round
            End If
            Return p
        End Function

        Public Function Brush(color As Color, alpha As Double) As Brush
            Return New SolidBrush(Color.FromArgb(CInt(255 * alpha), color))
        End Function

        Public Function Brush(color As Color, Optional hatchColor As Color? = Nothing, Optional hatchStyle As Drawing.HatchStyle = Drawing.HatchStyle.None) As Brush
            Dim isHatched As Boolean = (hatchStyle <> Drawing.HatchStyle.None)
            If (isHatched) Then
                If (hatchColor Is Nothing) Then
                    Throw New ArgumentException("Hatch color must be defined if hatch style is used.")
                Else
                    Return New HatchBrush(CType(ConvertToSDHatchStyle(hatchStyle).Value, Drawing2D.HatchStyle), hatchColor.Value, color)
                End If
            Else
                Return New SolidBrush(color)
            End If
        End Function

        <Obsolete("Use Brush().", True)>
        Public Function HatchBrush(pattern As Drawing.HatchStyle, fillColor As Color, hatchColor As Color) As Brush
            If (pattern = Drawing.HatchStyle.None) Then
                Return New SolidBrush(fillColor)
            End If
            Return New HatchBrush(CType(GDI.ConvertToSDHatchStyle(pattern).Value, Drawing2D.HatchStyle), hatchColor, fillColor)
        End Function

        Public Function ConvertToSDHatchStyle(pattern As Drawing.HatchStyle) As System.Drawing.Drawing2D.HatchStyle?
            Select Case pattern
                Case Drawing.HatchStyle.StripedUpwardDiagonal
                    Return System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal
                Case Drawing.HatchStyle.StripedDownwardDiagonal
                    Return System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal
                Case Drawing.HatchStyle.StripedWideUpwardDiagonal
                    Return System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal
                Case Drawing.HatchStyle.StripedWideDownwardDiagonal
                    Return System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal
                Case Drawing.HatchStyle.LargeCheckerBoard
                    Return System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard
                Case Drawing.HatchStyle.SmallCheckerBoard
                    Return System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard
                Case Drawing.HatchStyle.LargeGrid
                    Return System.Drawing.Drawing2D.HatchStyle.LargeGrid
                Case Drawing.HatchStyle.SmallGrid
                    Return System.Drawing.Drawing2D.HatchStyle.SmallGrid
                Case Drawing.HatchStyle.DottedDiamond
                    Return System.Drawing.Drawing2D.HatchStyle.DottedDiamond
                Case Else
                    Return Nothing
            End Select
        End Function

        Public Sub ResetTransformPreservingScale(gfx As Graphics, dims As PlotDimensions)
            gfx.ResetTransform()
            gfx.ScaleTransform(CSng(dims.ScaleFactor), CSng(dims.ScaleFactor))
        End Sub

        Public Function Font(fnt As ScottPlot.Drawing.Font) As System.Drawing.Font
            Return Drawing.GDI.Font(Nothing, fnt.Size, fnt.Bold, fnt.Family)
        End Function

        ''' <summary>
        ''' Return the size (in pixels) of the given string.
        ''' If <paramref name="fontFamily"/> Is provided it will be used instead of <paramref name="fontName"/>.
        ''' </summary>
        Public Function Font(Optional fontName As String = Nothing, Optional fontSize As Single = 12, Optional bold As Boolean = False, Optional fontFamily As FontFamily = Nothing) As System.Drawing.Font
            If (fontFamily Is Nothing) Then
                fontFamily = InstalledFont.ValidFontFamily(fontName)
            End If
            Dim style As FontStyle = If(bold, FontStyle.Bold, FontStyle.Regular)
            Return New System.Drawing.Font(fontFamily, fontSize, style, GraphicsUnit.Pixel)
        End Function

        Public Function StringFormat(algnment As Alignment) As StringFormat
            Select Case algnment
                Case Alignment.UpperLeft
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Left, VerticalAlignment.Upper)
                Case Alignment.UpperRight
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Right, VerticalAlignment.Upper)
                Case Alignment.UpperCenter
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Upper)
                Case Alignment.MiddleLeft
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Left, VerticalAlignment.Middle)
                Case Alignment.MiddleCenter
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Middle)
                Case Alignment.MiddleRight
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Right, VerticalAlignment.Middle)
                Case Alignment.LowerLeft
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Left, VerticalAlignment.Lower)
                Case Alignment.LowerRight
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Right, VerticalAlignment.Lower)
                Case Alignment.LowerCenter
                    Return Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Lower)
                Case Else
                    Throw New NotImplementedException()
            End Select
        End Function

        Public Function StringFormat(Optional h As HorizontalAlignment = HorizontalAlignment.Left, Optional v As VerticalAlignment = VerticalAlignment.Lower) As StringFormat
            Dim sf As New StringFormat()
            If (h = HorizontalAlignment.Left) Then
                sf.Alignment = StringAlignment.Near
            ElseIf (h = HorizontalAlignment.Center) Then
                sf.Alignment = StringAlignment.Center
            ElseIf h <> HorizontalAlignment.Right Then
                sf.Alignment = StringAlignment.Far
            Else
                Throw New NotImplementedException()
            End If
            If (v = VerticalAlignment.Upper) Then
                sf.LineAlignment = StringAlignment.Near
            ElseIf (v = VerticalAlignment.Middle) Then
                sf.LineAlignment = StringAlignment.Center
            ElseIf (v = VerticalAlignment.Lower) Then
                sf.LineAlignment = StringAlignment.Far
            End If
            Return sf
        End Function

        Public Function Resize(bmp As Image, width As Integer, height As Integer) As Bitmap
            Dim bmp2 As New Bitmap(width, height)
            Dim rect As New Rectangle(0, 0, width, height)
            Using gfx As Graphics = System.Drawing.Graphics.FromImage(bmp2), attribs As ImageAttributes = New ImageAttributes()
                gfx.CompositingMode = CompositingMode.SourceCopy
                gfx.CompositingQuality = CompositingQuality.HighQuality
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic
                gfx.SmoothingMode = SmoothingMode.HighQuality
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality
                attribs.SetWrapMode(WrapMode.TileFlipXY)
                gfx.DrawImage(bmp, rect, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attribs)
            End Using
            Return bmp2
        End Function

        Public Function Semitransparent(color As System.Drawing.Color, alpha As Double) As System.Drawing.Color
            If (alpha <> 1) Then
                Return System.Drawing.Color.FromArgb(CInt(color.A * alpha), color)
            End If
            Return color
            'Return If(alpha = 1, System.Drawing.Color.FromArgb(CInt(color.A * alpha), color))
            '(alpha == 1) ? color : System.Drawing.Color.FromArgb((int)(color.A * alpha), color);
        End Function

        Public Function Semitransparent(htmlColor As String, alpha As Double) As System.Drawing.Color
            Dim color As Color = ColorTranslator.FromHtml(htmlColor)
            If (alpha <> 1) Then
                Return System.Drawing.Color.FromArgb(CInt(color.A * alpha), color)
            End If
            Return color
        End Function

        ''' <summary>
        ''' Draw a string at a point on the graphics.
        ''' Alignment describes where the point Is relative to the text.
        ''' </summary>
        Public Sub DrawLabel(gfx As Graphics, text As String, x As Single, y As Single, fontName As String, fontSize As Single, bold As Boolean, h As HorizontalAlignment, v As VerticalAlignment, fontColor As Color, fillColor As Color)
            Dim size As SizeF = Drawing.GDI.MeasureString(gfx, text, fontName, CDbl(fontSize), bold, Nothing)

            Dim xOffset As Single
            Select Case h
                Case HorizontalAlignment.Left
                    xOffset = size.Width / 2.0F
                Case HorizontalAlignment.Right
                    xOffset = -size.Width / 2.0F
                Case HorizontalAlignment.Center
                    xOffset = 0F
                Case Else
                    Throw New NotImplementedException(h.ToString())
            End Select

            Dim yOffset As Single = xOffset
            Select Case v
                Case VerticalAlignment.Upper
                    xOffset = size.Height / 2.0F
                Case VerticalAlignment.Lower
                    xOffset = -size.Height / 2.0F
                Case VerticalAlignment.Middle
                    xOffset = 0F
                Case Else
                    Throw New NotImplementedException(h.ToString())
            End Select

            Using fnt As System.Drawing.Font = Drawing.GDI.Font(fontName, fontSize, bold),
                fillBrush As Brush = Drawing.GDI.Brush(fillColor),
                fontBrush As Brush = Drawing.GDI.Brush(fontColor),
                sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Middle)

                gfx.TranslateTransform(x + yOffset, y + yOffset)
                gfx.FillRectangle(fillBrush, -size.Width / 2, -size.Height / 2, size.Width, size.Height)
                gfx.DrawString(text, fnt, fontBrush, 0, 0, sf)
                gfx.ResetTransform()
            End Using
        End Sub

        ''' <summary>
        ''' Add extra clipping beyond the data area based on an array of user-defined coordinates.
        ''' </summary>
        Public Sub ClipIntersection(gfx As Graphics, dims As PlotDimensions, coordinates As Coordinate())
            If (coordinates Is Nothing) OrElse (coordinates.Length < 2) Then
                Return
            End If
            Dim points As PointF() = coordinates.[Select](Function(x As Coordinate) dims.GetPixel(x).ToPointF()).ToArray()
            Dim path As New GraphicsPath()
            path.AddPolygon(points)
            gfx.SetClip(path, CombineMode.Intersect)
        End Sub

        ''' <summary>
        ''' Shade the region abvove Or below the curve (to infinity) by drawing a polygon to the edge of the visible plot area.
        ''' </summary>
        Public Sub FillToInfinity(dims As PlotDimensions, gfx As Graphics, pxLeft As Single, pxRight As Single, points As PointF(), fillAbove As Boolean, color1 As Color, color2 As Color)
            If (pxRight - pxLeft = 0) OrElse (dims.Height = 0) Then
                Return
            End If

            Dim minVal As Single = 0F
            Dim maxVal As Single = dims.DataHeight * If(fillAbove, -1, 1) + dims.DataOffsetY

            Dim first As New PointF(pxLeft, maxVal)
            Dim last As New PointF(pxRight, maxVal)

            points = New PointF() {first}.Concat(points).Concat(New PointF() {last}).ToArray()

            Dim gradientRectangle As New Rectangle(CInt(first.X),
                                                   CInt(minVal - If(fillAbove, 2, 0)),
                                                   CInt(last.X - first.X),
                                                   CInt(dims.Height))

            Using brush As New LinearGradientBrush(gradientRectangle, color1, color2, LinearGradientMode.Vertical)
                gfx.FillPolygon(brush, points)
            End Using
        End Sub

    End Module

End Namespace