Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A collection of X/Y coordinates that can be displayed as markers and/or connected lines.
    ''' Unlike the regular ScatterPlot, this plot type has Add() methods to easily add data.
    ''' </summary>
    Public Class ScatterPlotList(Of T)
        Implements IPlottable

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' If enabled, points will be connected by smooth lines instead of straight diagnal lines. 
        ''' <see cref="SmoothTension"/> adjusts the smoothnes of the lines.
        ''' </summary>
        Public Smooth As Boolean = False

        ''' <summary>
        ''' Tension to use for smoothing when <see cref="Smooth"/> is enabled.
        ''' </summary>
        Public SmoothTension As Double = 0.5

        Protected ReadOnly Xs As New List(Of T)()
        Protected ReadOnly Ys As New List(Of T)()

        Public Property Label As String
        Public Property IsVisible As Boolean = True Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property Color As Color = Color.Black
        Public Property LineWidth As Single = 1
        Public Property LineStyle As LineStyle = LineStyle.Solid
        Public Property MarkerSize As Single = 3
        Public Property MarkerShape As MarkerShape = MarkerShape.FilledCircle

        ''' <summary>
        ''' If enabled, scatter plot points will be connected by square corners rather than straight diagnal lines
        ''' </summary>
        Public Property StepDisplay As Boolean = False
        Public Property StepDisplayRight As Boolean = True

        Public ReadOnly Property Count As Integer
            Get
                Return Me.Xs.Count
            End Get
        End Property

        ''' <summary>
        ''' Defines behavior when <see cref="Xs"/> or <see cref="Ys"/> contains <see cref="Double.NaN"/>.
        ''' </summary>
        Public OnNaN As ScatterPlot.NanBehavior = ScatterPlot.NanBehavior.Throw

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
            If (Xs.Count <> Ys.Count) Then
                Throw New InvalidOperationException("Xs and Ys must be same length.")
            End If
        End Sub

        ''' <summary>
        ''' Clear the list of points.
        ''' </summary>
        Public Sub Clear()
            Xs.Clear()
            Ys.Clear()
        End Sub

        ''' <summary>
        ''' Add a single point to the list.
        ''' </summary>
        Public Sub Add(x As T, y As T)
            Xs.Add(x)
            Ys.Add(y)
        End Sub

        ''' <summary>
        ''' Add multiple points to the list.
        ''' </summary>
        Public Sub AddRange(xs As T(), ys As T())
            If (xs is Nothing) Then
                Throw New ArgumentException("Xs must not be null.")
            End If
            If (ys is Nothing) Then
                Throw New ArgumentException("Ys must not be null.")
            End If
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs and Ys must have the same length.")
            End If
            Me.Xs.AddRange(xs)
            Me.Ys.AddRange(ys)
        End Sub

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            If (Count = 0) Then
                Return AxisLimits.NoLimits
            End If

            Dim xs = Me.Xs.Select(Function(x) NumericConversion.GenericToDouble(x))
            Dim ys = Me.Ys.Select(Function(y) NumericConversion.GenericToDouble(y))

            If (Not xs.Any) OrElse (Not ys.Any) Then
                Return AxisLimits.NoLimits
            End If

            If (OnNaN = ScatterPlot.NanBehavior.Throw) Then
                Dim xMin As Double = xs.Min()
                Dim xMax As Double = xs.Max()
                Dim yMin As Double = ys.Min()
                Dim yMax As Double = ys.Max()

                If Double.IsNaN(xMin + xMax + yMin + yMax) Then
                    Throw New InvalidOperationException($"Data may not contain NaN unless {NameOf(OnNaN)} is changed.")
                End If
                Return New AxisLimits(xMin, xMax, yMin, yMax)
            Else
                xs = xs.Where(Function(x) Double.IsNaN(x))
                ys = ys.Where(Function(y) Double.IsNaN(y))

                If (Not xs.Any) OrElse (Not ys.Any) Then
                    Return AxisLimits.NoLimits
                End If

                Dim xMin As Double = xs.Min()
                Dim xMax As Double = xs.Max()
                Dim yMin As Double = ys.Min()
                Dim yMax As Double = ys.Max()

                Return New AxisLimits(xMin, xMax, yMin, yMax)
            End If
        End Function

        ''' <summary>
        ''' Return a new array containing pixel locations for each point of the scatter plot.
        ''' </summary>
        Private Function GetPoints(dims As PlotDimensions) As PointF()
            Return Enumerable.Range(0, Count) _
                .Select(Function(i) Coordinate.FromGeneric(Xs(i), Ys(i))) _
                .Select(Function(coord) coord.ToPixel(dims)) _
                .Select(Function(px) New PointF(px.X, px.Y)).ToArray()
        End Function

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            Dim points As PointF() = GetPoints(dims)
            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                penLine As Pen = Drawing.GDI.Pen(Color, LineWidth, LineStyle, True)

                Select Case OnNaN
                    Case ScatterPlot.NanBehavior.Throw
                        For Each pt As PointF In points
                            If Single.IsNaN(pt.X) OrElse Single.IsNaN(pt.Y) Then
                                Throw New NotImplementedException($"Data must not contain NaN if {NameOf(OnNaN)} is {OnNaN}.")
                            End If
                        Next
                        DrawLines(points, gfx, penLine)

                    Case ScatterPlot.NanBehavior.Ignore
                        DrawLinesIngoringNaN(points, gfx, penLine)

                    Case ScatterPlot.NanBehavior.Gap
                        DrawLinesWithGaps(points, gfx, penLine)
                End Select

                If (MarkerShape <> MarkerShape.None) AndAlso (MarkerSize > 0) AndAlso (Count > 0) Then
                    MarkerTools.DrawMarkers(gfx, points, MarkerShape, MarkerSize, Color, 1)
                End If
            End Using
        End Sub

        Private Sub DrawLines(points As PointF(), gfx As Graphics, penLine As Pen)
            Dim isLineVisible As Boolean = (LineWidth > 0) AndAlso (points.Length > 1) AndAlso (LineStyle <> LineStyle.None)
            If isLineVisible Then
                If StepDisplay Then
                    Dim pointsStep As PointF() = ScatterPlot.GetStepDisplayPoints(points, StepDisplayRight)
                    gfx.DrawLines(penLine, pointsStep)
                ElseIf Smooth Then
                    gfx.DrawCurve(penLine, points, CSng(SmoothTension))
                Else
                    gfx.DrawLines(penLine, points)
                End If
            End If
        End Sub

        Private Sub DrawLinesIngoringNaN(points As PointF(), gfx As Graphics, penLine As Pen)
            Dim pointsWithoutNaNs As PointF() = points.Where(Function(pt)
                                                                 Return (Not Double.IsNaN(pt.X)) AndAlso (Not Double.IsNaN(pt.Y))
                                                             End Function).ToArray()
            DrawLines(pointsWithoutNaNs, gfx, penLine)
        End Sub

        Private Sub DrawLinesWithGaps(points As PointF(), gfx As Graphics, penLine As Pen)
            Dim segment As New List(Of PointF)()
            For i As Integer = 0 To points.Length - 1
                If Double.IsNaN(points(i).X) OrElse Double.IsNaN(points(i).Y) Then
                    If segment.Any() Then
                        DrawLines(segment.ToArray(), gfx, penLine)
                        segment.Clear()
                    End If
                Else
                    segment.Add(points(i))
                End If
            Next
            If segment.Any() Then
                DrawLines(segment.ToArray(), gfx, penLine)
            End If
        End Sub

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = Color,
                .LineStyle = LineStyle,
                .LineWidth = LineWidth,
                .MarkerShape = MarkerShape,
                .MarkerSize = MarkerSize}
            Return New LegendItem() {leg}
        End Function

#End Region '/METHODS

    End Class

End Namespace