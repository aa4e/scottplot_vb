Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A polygon is a collection of X/Y points that are all connected to form a closed shape.
    ''' Polygons can be optionally filled with a color Or a gradient.
    ''' </summary>
    Public Class Polygon
        Implements IPlottable, IHasColor

#Region "PROPS, FIELDS"

        Public Property Xs As Double()
        Public Property Ys As Double()
        Public Property Label As String
        Public Property LineWidth As Double = 1.0
        Public Property LineColor As Color = Color.Black
        Public Property Fill As Boolean = True
        Public Property FillColor As Color = Color.Gray

        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return FillColor
            End Get
            Set(value As Color)
                FillColor = value
            End Set
        End Property

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property HatchColor As Color = Color.Transparent
        Public Property HatchStyle As Drawing.HatchStyle

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(xs As Double(), ys As Double())
            Me.Xs = xs
            Me.Ys = ys
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottablePolygon{lbl} with {PointCount} points"
        End Function

        Public ReadOnly Property PointCount As Integer
            Get
                Return Xs.Length
            End Get
        End Property

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Dim xMin As Double = Xs(0)
            Dim xMax As Double = Xs(0)
            Dim yMin As Double = Ys(0)
            Dim yMax As Double = Ys(0)
            For i As Integer = 1 To Xs.Length - 1
                xMin = Math.Min(xMin, Xs(i))
                xMax = Math.Max(xMax, Xs(i))
                yMin = Math.Min(yMin, Ys(i))
                yMax = Math.Max(yMax, Ys(i))
            Next
            Return New AxisLimits(xMin, xMax, yMin, yMax)
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim singleLegendItem As New LegendItem(Me) With {
                .Label = Label,
                .Color = If(Fill, FillColor, LineColor),
                .LineWidth = If(Fill, 10, LineWidth),
                .MarkerShape = MarkerShape.None,
                .HatchColor = HatchColor,
                .HatchStyle = HatchStyle}
            Return New LegendItem() {singleLegendItem}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            Validate.AssertHasElements("Xs", Xs)
            Validate.AssertHasElements("Ys", Ys)
            Validate.AssertEqualLength(Of Double, Double)("Xs and Ys", Xs, Ys)
            If (Xs.Length < 3) Then
                Throw New InvalidOperationException("Polygons must contain at least 3 points.")
            End If
            If deep Then
                Validate.AssertAllReal("Xs", Xs)
                Validate.AssertAllReal("Ys", Ys)
            End If
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Dim points As PointF() = New PointF(Me.Xs.Length - 1) {}
            For i As Integer = 0 To Me.Xs.Length - 1
                points(i) = New PointF(dims.GetPixelX(Me.Xs(i)), dims.GetPixelY(Me.Ys(i)))
            Next

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, True)
                If Fill Then
                    Using fillBrush As Brush = Drawing.GDI.Brush(FillColor, New Color?(HatchColor), HatchStyle)
                        gfx.FillPolygon(fillBrush, points)
                    End Using
                End If
                If (LineWidth > 0) Then
                    Using outlinePen As Pen =Drawing.GDI.Pen(LineColor, CDbl(CSng(LineWidth)), LineStyle.Solid, False)
                        gfx.DrawPolygon(outlinePen, points)
                    End Using
                End If
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace