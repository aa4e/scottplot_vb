Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Text placed at a location relative to the data area that does not move when the axis limits change.
    ''' </summary>
    Public Class Annotation
        Implements IPlottable

#Region "PROPS"

        Public ReadOnly Font As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' Horizontal location (in pixel units) relative to the data area.
        ''' </summary>
        Public Property X As Double

        ''' <summary>
        ''' Vertical position (in pixel units) relative to the data area.
        ''' </summary>
        Public Property Y As Double

        ''' <summary>
        ''' Text displayed in the annotation.
        ''' </summary>
        Public Property Label As String

        Public Property Background As Boolean = True
        Public Property BackgroundColor As Color = Color.Yellow

        Public Property Shadow As Boolean = True
        Public Property ShadowColor As Color = Color.FromArgb(25, Color.Black)

        Public Property Border As Boolean = True
        Public Property BorderWidth As Single = 1
        Public Property BorderColor As Color = Color.Black

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.YAxisIndex

#End Region '/PROPS

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"PlottableAnnotation at ({X} px, {Y} px)."
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If Double.IsNaN(X) OrElse Double.IsInfinity(X) Then
                Throw New InvalidOperationException("XPixel must be a valid number.")
            End If
            If Double.IsNaN(Y) OrElse Double.IsInfinity(Y) Then
                Throw New InvalidOperationException("YPixel must be a valid number.")
            End If
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If (Not IsVisible) Then
                Return
            End If
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, False),
                fnt As System.Drawing.Font = Drawing.GDI.Font(Font),
                fontBrush As New SolidBrush(Font.Color),
                shadowBrush As New SolidBrush(ShadowColor),
                backgroundBrush As New SolidBrush(BackgroundColor),
                borderPen As New Pen(BorderColor, BorderWidth)

                Dim size As SizeF = Drawing.GDI.MeasureString(gfx, Label, fnt)

                Dim x As Double = If((Me.X >= 0), Me.X, (dims.DataWidth + Me.X - size.Width))
                Dim y As Double = If((Me.Y >= 0), Me.Y, (dims.DataHeight + Me.Y - size.Height))
                Dim location As New PointF(CSng(x + dims.DataOffsetX), CSng(y + dims.DataOffsetY))

                If Background AndAlso Shadow Then
                    gfx.FillRectangle(shadowBrush, location.X + 5, location.Y + 5, size.Width, size.Height)
                End If
                If Background Then
                    gfx.FillRectangle(backgroundBrush, location.X, location.Y, size.Width, size.Height)
                End If
                If Border Then
                    gfx.DrawRectangle(borderPen, location.X, location.Y, size.Width, size.Height)
                End If
                gfx.DrawString(Label, fnt, fontBrush, location)
            End Using
        End Sub

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return AxisLimits.NoLimits
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

#End Region '/METHODS

    End Class

End Namespace