Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A function plot displays a curve using a function (Y as a function of X).
    ''' </summary>
    Public Class FunctionPlot
        Implements IPlottable, IHasLine, IHasColor

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' The function to translate an X to a Y (or null if undefined).
        ''' </summary>
        Public Functn As Func(Of Double, Double?)

        Public Property Label As String
        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property LineWidth As Double = 1 Implements ScottPlot.Plottable.IHasLine.LineWidth
        Public Property LineStyle As LineStyle = LineStyle.Solid Implements ScottPlot.Plottable.IHasLine.LineStyle
        Public Property Color As Color = Color.Black Implements ScottPlot.Plottable.IHasColor.Color
        Public Property LineColor As Color = Color.Black Implements ScottPlot.Plottable.IHasLine.LineColor
        Public Property FillType As FillType = FillType.NoFill
        Public Property FillColor As Color = Color.FromArgb(50, Color.Black)
        Public Property XMin As Double = Double.NegativeInfinity
        Public Property XMax As Double = Double.PositiveInfinity

        Public ReadOnly Property PointCount As Integer = 0

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(f As Func(Of Double, Double?))
            Me.Functn = f
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return AxisLimits.NoLimits
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Dim points As PointF() = GetPoints(dims)
            Using gfx =Drawing.GDI.Graphics(bmp, dims, lowQuality),
                penLine =Drawing.GDI.Pen(LineColor, LineWidth, LineStyle, True)
                If (FillType = FillType.FillAbove) OrElse (FillType = FillType.FillBelow) Then
                    Dim above As Boolean = (FillType = FillType.FillAbove)
                   Drawing.GDI.FillToInfinity(dims, gfx, points.First().X, points.Last().X, points, above, FillColor, FillColor)
                End If
                gfx.DrawLines(penLine, points)
            End Using
        End Sub

        Public Sub ValidateData(Optional deepValidation As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If (Me.Functn is Nothing) Then
                Throw New InvalidOperationException("Function cannot be null.")
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim lab As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableFunction{lab} displaying {PointCount} points."
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = Color,
                .LineStyle = LineStyle,
                .LineWidth = LineWidth,
                .MarkerShape = MarkerShape.None}
            Return New LegendItem() {leg}
        End Function

        Private Function GetPoints(dims As PlotDimensions) As PointF()
            Dim points As New List(Of PointF)

            Dim xStart As Double = If(XMin.IsFinite(), XMin, dims.XMin)
            Dim xEnd As Double = If(XMax.IsFinite(), XMax, dims.XMax)
            Dim width As Double = xEnd - xStart

            _PointCount = CInt(width * dims.PxPerUnitX) + 1

            For columnIndex As Integer = 0 To PointCount - 1
                Dim x As Double = columnIndex * dims.UnitsPerPxX + xStart
                Dim y As Double? = Me.Functn(x)

                If (y is Nothing) Then
                    Diagnostics.Debug.WriteLine($"Y({x}) failed because Y was null.")
                    Continue For
                End If

                If (Double.IsNaN(y.Value) OrElse Double.IsInfinity(y.Value)) Then
                    Diagnostics.Debug.WriteLine($"Y({x}) failed because y was not a real number.")
                    Continue For
                End If

                Dim xPx As Single = dims.GetPixelX(x)
                Dim yPx As Single = dims.GetPixelY(y.Value)
                points.Add(New PointF(xPx, yPx))
            Next

            Return points.ToArray()
        End Function

#End Region '/METHODS

    End Class

End Namespace