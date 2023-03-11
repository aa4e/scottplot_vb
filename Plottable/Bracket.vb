Imports System
Imports System.Drawing

Namespace ScottPlot.Plottable

    Public Class Bracket
        Implements IPlottable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Size of the small lines (in pixels) placed the edges of the bracket and between the center of the bracket and the label.
        ''' </summary>
        Public EdgeLength As Single = 5

        Public ReadOnly Font As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' Horizontal location (in pixel units) relative to the data area.
        ''' </summary>
        Public Property X1 As Double

        ''' <summary>
        ''' Vertical position (in pixel units) relative to the data area.
        ''' </summary>
        Public Property Y1 As Double

        ''' <summary>
        ''' Horizontal location (in pixel units) relative to the data area.
        ''' </summary>
        Public Property X2 As Double

        ''' <summary>
        ''' Vertical position (in pixel units) relative to the data area.
        ''' </summary>
        Public Property Y2 As Double

        ''' <summary>
        ''' Text displayed in the annotation.
        ''' </summary>
        Public Property Label As String

        ''' <summary>
        ''' Color of the bracket lines and text.
        ''' </summary>
        Public Property Color As Color = Color.Black

        ''' <summary>
        ''' Thickness (in pixels) of the lines.
        ''' </summary>
        Public Property LineWidth As Single = 1

        ''' <summary>
        ''' Controls whether the tip of the bracket is counter-clockwise from the line formed by the bracket base.
        ''' </summary>
        Public Property LabelCounterClockwise As Boolean = False

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(x1 As Double, y1 As Double, x2 As Double, y2 As Double)
            Me.X1 = x1
            Me.Y1 = y1
            Me.X2 = x2
            Me.Y2 = y2
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return New AxisLimits(Math.Min(X1, X2), Math.Max(X1, X2), Math.Min(Y1, Y2), Math.Max(Y1, Y2))
        End Function

        Private Function AngleBetweenVectors(reference As Statistics.Vector2, v As Statistics.Vector2) As Double
            reference = Statistics.Vector2.Normalize(reference)
            v = Statistics.Vector2.Normalize(v)
            Return Math.Acos(Statistics.Vector2.Dot(reference, v))
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                    pen As Pen = Drawing.GDI.Pen(Color, LineWidth),
                    brsh As Brush = Drawing.GDI.Brush(Color),
                    fnt As System.Drawing.Font = Drawing.GDI.Font(Font)

                    Dim v As New Statistics.Vector2(X2 - X1, Y2 - Y1)
                    Dim vPixel As New Statistics.Vector2(v.X * dims.PxPerUnitX, v.Y * dims.PxPerUnitY)
                    Dim vDirectionVector As Statistics.Vector2 = Statistics.Vector2.Normalize(vPixel)

                    If (v.X < 0) OrElse ((v.X = 0) AndAlso (v.Y < 0)) Then
                        vDirectionVector = Statistics.Vector2.Negate(vDirectionVector)
                    End If

                    Dim normal As Statistics.Vector2 = Statistics.Vector2.Normalize(New Statistics.Vector2(vPixel.Y, vPixel.X))
                    Dim antiNormal As Statistics.Vector2 = Statistics.Vector2.Negate(normal)

                    Dim clockwiseNormalVector As Statistics.Vector2 = If(AngleBetweenVectors(vDirectionVector, normal) > 0, normal, antiNormal)
                    Dim counterClockwiseNormalVector As Statistics.Vector2 = If(normal = clockwiseNormalVector, antiNormal, normal)

                    Dim edgeVector As Statistics.Vector2 = If(LabelCounterClockwise, counterClockwiseNormalVector, clockwiseNormalVector)

                    Dim globalTranslation As Statistics.Vector2 = edgeVector * EdgeLength
                    gfx.TranslateTransform(CSng(globalTranslation.X), CSng(globalTranslation.Y))

                    Dim pxStart1 As Pixel = dims.GetPixel(New Coordinate(X1, Y1))
                    Dim pxEnd1 As Pixel = dims.GetPixel(New Coordinate(X2, Y2))

                    Dim bracketHeadTranslation As Statistics.Vector2 = edgeVector * EdgeLength

                    Dim pxStart2 As Pixel = pxStart1.WithTranslation(CSng(bracketHeadTranslation.X), CSng(bracketHeadTranslation.Y))
                    Dim pxEnd2 As Pixel = pxEnd1.WithTranslation(CSng(bracketHeadTranslation.X), CSng(bracketHeadTranslation.Y))

                    gfx.DrawLine(pen, pxStart1.X, pxStart1.Y, pxStart2.X, pxStart2.Y)
                    gfx.DrawLine(pen, pxEnd1.X, pxEnd1.Y, pxEnd2.X, pxEnd2.Y)
                    gfx.DrawLine(pen, pxStart2.X, pxStart2.Y, pxEnd2.X, pxEnd2.Y)

                    If (Not String.IsNullOrWhiteSpace(Label)) Then
                        'draw the "sub" line between center of bracket and center of base of label
                        Dim halfVector As Statistics.Vector2 = New Statistics.Vector2(X1, Y1) + (0.5F * v)

                        Dim stubPixel1 As Pixel = dims.GetPixel(New Coordinate(halfVector.X, halfVector.Y)) _
                            .WithTranslation(CSng(bracketHeadTranslation.X), CSng(bracketHeadTranslation.Y))

                        Dim stubPixel2 As Pixel = stubPixel1 _
                            .WithTranslation(CSng(bracketHeadTranslation.X), CSng(bracketHeadTranslation.Y))

                        gfx.DrawLine(pen, stubPixel1.X, stubPixel1.Y, stubPixel2.X, stubPixel2.Y)

                        'draw label text
                        gfx.TranslateTransform(stubPixel2.X, stubPixel2.Y)
                        Dim angle As Single = CSng(-Math.Atan2(v.Y * dims.PxPerUnitY, v.X * dims.PxPerUnitX) * 180 / Math.PI)
                        If (angle < 0) Then
                            angle += 360
                        End If

                        Dim flippedText As Boolean = False
                        If (angle > 90) AndAlso (angle < 270) Then
                            flippedText = True
                            angle -= 180 'keep the text upright
                        End If

                        gfx.RotateTransform(angle)

                        Dim isInverted As Boolean = (edgeVector = antiNormal)

                        Dim labelHeight As Single = gfx.MeasureString(Label, fnt).Height
                        gfx.TranslateTransform(0, labelHeight * CSng(If((isInverted AndAlso Not flippedText) OrElse (Not isInverted AndAlso flippedText), 0, 1)))

                        gfx.DrawString(Label, fnt, brsh, 0, 0, New StringFormat() With {
                                       .Alignment = StringAlignment.Center,
                                       .LineAlignment = StringAlignment.Far})
                    End If
                End Using
            End If
        End Sub

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

#End Region '/METHODS

    End Class

End Namespace