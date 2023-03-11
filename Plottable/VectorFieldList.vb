Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' The VectorField displays arrows representing a 2D array of 2D vectors.
    ''' </summary>
    Public Class VectorFieldList
        Implements IPlottable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Tuples define location and direction of vectors to display as arrows.
        ''' Users may manipulate this List to add/remove their own vectors.
        ''' </summary>
        Public ReadOnly RootedVectors As New List(Of Tuple(Of Coordinate, CoordinateVector))()

        ''' <summary>
        ''' Advanced configuration options that control how vectors are drawn as arrows
        ''' </summary>
        Public ReadOnly ArrowStyle As New Renderable.ArrowStyle()

        Public Property Label As String = String.Empty

        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex

        ''' <summary>
        ''' Color to draw the arrows (if <see cref="Colormap"/> is null).
        ''' </summary>
        Public Property Color As Color = Color.Blue

        ''' <summary>
        ''' If defined, this colormap is used to color each arrow based on its magnitude.
        ''' </summary>
        Public Property Colormap As Drawing.Colormap

        ''' <summary>
        ''' If <see cref="Colormap"/> is defined, each arrow's magnitude is run 
        ''' through this function to get the fraction (from 0 to 1) along the colormap to sample from.
        ''' </summary>
        Public ColormapScaler As Func(Of Double, Double) = Function(x) x

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New()
        End Sub

        Public Sub New(rootedVectors As List(Of Tuple(Of Coordinate, CoordinateVector)))
            Me.RootedVectors = rootedVectors
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(Label), "", $" ({Label})")
            Return $"PlottableVectorField{lbl} with {RootedVectors.Count} vectors."
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Return {}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If (Not RootedVectors.Any()) Then
                Return AxisLimits.NoLimits
            Else
                Return New AxisLimits(
                    RootedVectors.Select(Function(x) x.Item1.X - x.Item2.X).Min(),
                    RootedVectors.Select(Function(x) x.Item1.X + x.Item2.X).Max(),
                    RootedVectors.Select(Function(x) x.Item1.Y - x.Item2.Y).Min(),
                    RootedVectors.Select(Function(x) x.Item1.Y + x.Item2.Y).Max())
            End If
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality, True)
                For Each t As Tuple(Of Coordinate, CoordinateVector) In RootedVectors
                    Dim coordinate As Coordinate = t.Item1
                    Dim vector As CoordinateVector = t.Item2
                    Dim clr As Color = If(Colormap Is Nothing, Color, Colormap.GetColor(ColormapScaler(vector.Magnitude), 1))
                    ArrowStyle.RenderArrow(dims, gfx, coordinate, vector, clr)
                Next
            End Using
        End Sub

#End Region '/METHODS

    End Class

End Namespace