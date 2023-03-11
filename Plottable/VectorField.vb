Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' The VectorField displays arrows representing a 2D array of 2D vectors.
    ''' </summary>
    Public Class VectorField
        Implements IPlottable

#Region "PROPS, FIELDS"

        Private ReadOnly Xs As Double()
        Private ReadOnly Ys As Double()
        Private ReadOnly Vectors As Statistics.Vector2(,)
        Private ReadOnly VectorColors As Color()
        Private ReadOnly ArrowStyle As New Renderable.ArrowStyle()

        Public Property Label As String = String.Empty

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        ''' <summary>
        ''' Describes which part of the vector line will be placed at the data coordinates.
        ''' </summary>
        Public Property Anchor As ArrowAnchor
            Get
                Return ArrowStyle.Anchor
            End Get
            Set(value As ArrowAnchor)
                ArrowStyle.Anchor = value
            End Set
        End Property

        ''' <summary>
        ''' If enabled arrowheads will be drawn as lines scaled to each vector's magnitude.
        ''' </summary>
        Public Property ScaledArrowheads As Boolean
            Get
                Return ArrowStyle.ScaledArrowheads
            End Get
            Set(value As Boolean)
                ArrowStyle.ScaledArrowheads = value
            End Set
        End Property

        ''' <summary>
        ''' When using scaled arrowheads this defines the width of the arrow relative to the vector line's length.
        ''' </summary>
        Public Property ScaledArrowheadWidth As Double
            Get
                Return ArrowStyle.ScaledArrowheadWidth
            End Get
            Set(value As Double)
                ArrowStyle.ScaledArrowheadWidth = value
            End Set
        End Property

        ''' <summary>
        ''' When using scaled arrowheads this defines length of the arrowhead relative to the vector line's length.
        ''' </summary>
        Public Property ScaledArrowheadLength As Double
            Get
                Return ArrowStyle.ScaledArrowheadLength
            End Get
            Set(value As Double)
                ArrowStyle.ScaledArrowheadLength = value
            End Set
        End Property

        ''' <summary>
        ''' Marker drawn at each coordinate.
        ''' </summary>
        Public Property MarkerShape As MarkerShape
            Get
                Return ArrowStyle.MarkerShape
            End Get
            Set(value As MarkerShape)
                ArrowStyle.MarkerShape = value
            End Set
        End Property

        ''' <summary>
        ''' Size of markers to be drawn at each coordinate.
        ''' </summary>
        Public Property MarkerSize As Single
            Get
                Return ArrowStyle.MarkerSize
            End Get
            Set(value As Single)
                ArrowStyle.MarkerSize = value
            End Set
        End Property

        Public ReadOnly Property PointCount As Integer
            Get
                Return Vectors.Length
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(vectors As Statistics.Vector2(,), xs As Double(), ys As Double(), colormap As Drawing.Colormap, scaleFactor As Double, defaultColor As Color)
            Dim minMagnitudeSquared As Double = vectors(0, 0).LengthSquared()
            Dim maxMagnitudeSquared As Double = vectors(0, 0).LengthSquared()
            For i As Integer = 0 To xs.Length - 1
                For j As Integer = 0 To ys.Length - 1
                    If (vectors(i, j).LengthSquared() > maxMagnitudeSquared) Then
                        maxMagnitudeSquared = vectors(i, j).LengthSquared()
                    ElseIf (vectors(i, j).LengthSquared() < minMagnitudeSquared) Then
                        minMagnitudeSquared = vectors(i, j).LengthSquared()
                    End If
                Next
            Next
            Dim minMagnitude As Double = Math.Sqrt(minMagnitudeSquared)
            Dim maxMagnitude As Double = Math.Sqrt(maxMagnitudeSquared)

            Dim intensities As Double(,) = New Double(xs.Length - 1, ys.Length - 1) {}
            For i As Integer = 0 To xs.Length - 1
                For j As Integer = 0 To ys.Length - 1
                    If (colormap IsNot Nothing) Then
                        intensities(i, j) = (vectors(i, j).Length() - minMagnitude) / (maxMagnitude - minMagnitude)
                    End If
                    vectors(i, j) = Statistics.Vector2.Multiply(vectors(i, j), scaleFactor / (maxMagnitude * 1.2))
                Next
            Next

            Dim flattenedIntensities As Double() = intensities.Cast(Of Double)().ToArray()
            Me.VectorColors = If(colormap is Nothing,
                Enumerable.Range(0, flattenedIntensities.Length).Select(Function(x As Integer) defaultColor).ToArray(),
                colormap.GetColors(flattenedIntensities, colormap))
            Me.Vectors = vectors
            Me.Xs = xs
            Me.Ys = ys
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = VectorColors(0),
                .LineWidth = 10,
                .MarkerShape = MarkerShape.None}
            Return New LegendItem() {leg}
        End Function

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return New AxisLimits(Xs.Min() - 1, Xs.Max() + 1, Ys.Min() - 1, Ys.Max() + 1)
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality)
                    ArrowStyle.Render(dims, gfx, Xs, Ys, Vectors, VectorColors)
                End Using
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableVectorField{lbl} with {PointCount} vectors."
        End Function

#End Region '/METHODS

    End Class

End Namespace