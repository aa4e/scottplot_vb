Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' This plot type displays a collection of Bar objects, allowing each Bar to be positioned and styled individually.
    ''' </summary>
    Public Class BarSeries
        Implements IPlottable

#Region "PROPS, FIELDS"

        Public ReadOnly Bars As List(Of Bar)

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        Public ReadOnly Property Count As Integer
            Get
                Return Bars.Count
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New()
            Me.Bars = New List(Of Bar)()
        End Sub

        Public Sub New(bars As List(Of Bar))
            Me.Bars = bars
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If (Bars.Count() = 0) Then
                Return ScottPlot.AxisLimits.NoLimits
            End If

            Dim limits As AxisLimits = Bars.First().GetLimits()
            For Each bar As Bar In Bars.Skip(1)
                limits = bar.GetLimits().Expand(limits)
            Next
            Return limits
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Private Function GetPixelRect(dims As PlotDimensions, bar As Bar) As RectangleF
            If bar.IsVertical Then
                Dim left As Single = dims.GetPixelX(bar.Position - bar.Thickness / 2)
                Dim right As Single = dims.GetPixelX(bar.Position + bar.Thickness / 2)
                Dim bot As Single = dims.GetPixelY(bar.ValueBase)
                Dim top As Single = dims.GetPixelY(bar.Value)
                Dim width As Single = right - left
                Dim height As Single = bot - top
                If (bar.Value < 0) Then
                    top = bot
                    height = -height
                End If
                Return New RectangleF(left, top, width, height)
            Else
                Dim left As Single = dims.GetPixelX(bar.ValueBase)
                Dim right As Single = dims.GetPixelX(bar.Value)
                Dim top As Single = dims.GetPixelY(bar.Position + bar.Thickness / 2)
                Dim bot As Single = dims.GetPixelY(bar.Position - bar.Thickness / 2)
                Dim width As Single = right - left
                Dim height As Single = bot - top
                If (bar.Value < 0) Then
                    left = right
                    width = -width
                End If
                Return New RectangleF(left, top, width, height)
            End If
        End Function

        ''' <summary>
        ''' Return the bar located under the given coordinate (or null if no bar is there).
        ''' </summary>
        Public Function GetBar(coordinate As Coordinate) As Bar
            For Each bar As Bar In Bars
                If bar.GetLimits().Contains(coordinate) Then
                    Return bar
                End If
            Next
            Return Nothing
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible AndAlso (Bars.Count() > 0) Then
                Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                    brsh As Brush = Drawing.GDI.Brush(Color.Black),
                    pen As Pen = Drawing.GDI.Pen(Color.Black)

                    For Each bar As Bar In Me.Bars
                        Dim rect As RectangleF = GetPixelRect(dims, bar)

                        'fill
                        CType(brsh, SolidBrush).Color = bar.FillColor
                        gfx.FillRectangle(brsh, rect)

                        'outline
                        If (bar.LineWidth > 0) Then
                            pen.Color = bar.LineColor
                            pen.Width = bar.LineWidth
                            gfx.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
                        End If

                        'text label
                        If (Not String.IsNullOrWhiteSpace(bar.Label)) Then
                            Using font As System.Drawing.Font = Drawing.GDI.Font(bar.Font)
                                CType(brsh, SolidBrush).Color = bar.Font.Color

                                Dim drawBelow As Boolean = (bar.Value < 0)
                                If bar.IsVertical Then
                                    Dim pos As Single = If(drawBelow, rect.Bottom, rect.Top)
                                    Using sf As StringFormat = Drawing.GDI.StringFormat(HorizontalAlignment.Center,
                                                                                If(drawBelow, VerticalAlignment.Upper, VerticalAlignment.Lower))
                                        gfx.DrawString(bar.Label, font, brsh, rect.Left + rect.Width / 2, pos, sf)
                                    End Using
                                Else
                                    Dim pos As Single = If(drawBelow, rect.Left, rect.Right)
                                    Using sf As StringFormat = Drawing.GDI.StringFormat(If(drawBelow, HorizontalAlignment.Right, HorizontalAlignment.Left),
                                                                            VerticalAlignment.Middle)
                                        gfx.DrawString(bar.Label, font, brsh, pos, rect.Top + rect.Height / 2, sf)
                                    End Using
                                End If
                            End Using
                        End If
                    Next
                End Using
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace