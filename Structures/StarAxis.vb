Imports System.Linq
Imports System.Drawing

Namespace ScottPlot

    Public Class StarAxis

#Region "PROPS, FILEDS"

        ''' <summary>
        ''' The ticks for each spoke.
        ''' </summary>
        Public Property Ticks As StarAxisTick()

        ''' <summary>
        ''' The number of spokes to draw.
        ''' </summary>
        Public Property NumberOfSpokes As Integer

        ''' <summary>
        ''' Labels for each category. Length must be equal to the number of columns (categories) in the original data.
        ''' </summary>
        Public CategoryLabels As String()

        ''' <summary>
        ''' Icons for each category. Length must be equal to the number of columns (categories) in the original data.
        ''' </summary>
        Public CategoryImages As System.Drawing.Image()

        ''' <summary>
        ''' Controls rendering style of the concentric circles (ticks) of the web.
        ''' </summary>
        Public Property AxisType As RadarAxis

        ''' <summary>
        ''' Indicates the type of axis chart to render.
        ''' </summary>
        Public Property ImagePlacement As ImagePlacement

        ''' <summary>
        ''' Color of the axis lines and concentric circles representing ticks.
        ''' </summary>
        Public Property WebColor As Color = Color.Gray

        ''' <summary>
        ''' If true, each value will be written in text on the plot.
        ''' </summary>
        Public Property ShowAxisValues As Boolean = True

        ''' <summary>
        ''' If true, category labels will be written in text on the plot (provided they exist).
        ''' </summary>
        Public Property ShowCategoryLabels As Boolean = True

        ''' <summary>
        ''' Determines whether each spoke should be labeled, or just the first.
        ''' </summary>
        Public Property LabelEachSpoke As Boolean

        ''' <summary>
        ''' The drawing surface to use.
        ''' </summary>
        Public Property Graphics As Graphics

        ''' <summary>
        ''' Font used for labeling values on the plot.
        ''' </summary>
        Public Font As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' Determines the width of each spoke and the axis lines.
        ''' </summary>
        Public Property LineWidth As Integer = 1
        Public Property XAxisIndex As Integer = 0
        Public Property YAxisIndex As Integer = 0

#End Region '/PROPS, FILEDS

#Region "METHODS"

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False)
            Dim sweepAngle As Double = 2 * Math.PI / NumberOfSpokes
            Dim minScale As Double = {dims.PxPerUnitX, dims.PxPerUnitX}.Min()
            Dim origin As New PointF(dims.GetPixelX(0.0), dims.GetPixelY(0.0))

            RenderRings(origin, minScale, sweepAngle)
            RenderSpokes(origin, minScale, sweepAngle)

            If (CategoryImages IsNot Nothing) Then
                RenderImages(origin, minScale, sweepAngle)
            ElseIf ShowCategoryLabels AndAlso (CategoryLabels IsNot Nothing) Then
                RenderLabels(dims, origin, minScale, sweepAngle)
            End If
        End Sub

        Private Sub RenderRings(origin As PointF, minScale As Double, sweepAngle As Double)
            Using pen As Pen = Drawing.GDI.Pen(WebColor, LineWidth)
                For Each tick In Ticks
                    Dim tickDistancePx As Double = tick.Location * minScale
                    If (AxisType = RadarAxis.Circle) Then
                        Graphics.DrawEllipse(pen,
                                             CInt(origin.X - tickDistancePx),
                                             CInt(origin.Y - tickDistancePx),
                                             CInt(tickDistancePx * 2),
                                             CInt(tickDistancePx * 2))
                    ElseIf (AxisType = RadarAxis.Polygon) Then
                        Dim points(NumberOfSpokes - 1) As PointF
                        For j As Integer = 0 To NumberOfSpokes - 1
                            Dim x As Single = CSng(tickDistancePx * Math.Cos(sweepAngle * j - Math.PI / 2) + origin.X)
                            Dim y As Single = CSng(tickDistancePx * Math.Sin(sweepAngle * j - Math.PI / 2) + origin.Y)
                            points(j) = New PointF(x, y)
                        Next
                        Graphics.DrawPolygon(pen, points)
                    End If
                Next
            End Using
        End Sub

        Private Sub RenderSpokes(origin As PointF, minScale As Double, sweepAngle As Double)
            Using pen As Pen =Drawing.GDI.Pen(WebColor, LineWidth),
                fnt As System.Drawing.Font =Drawing.GDI.Font(Font),
                fontBrush As Brush =Drawing.GDI.Brush(Me.Font.Color),
                sf As New StringFormat()

                For i As Integer = 0 To Me.NumberOfSpokes - 1
                    Dim destination As New PointF(
                        CSng(1.1 * Math.Cos(sweepAngle * i - Math.PI / 2) * minScale + origin.X),
                        CSng(1.1 * Math.Sin(sweepAngle * i - Math.PI / 2) * minScale + origin.Y))
                    Graphics.DrawLine(pen, origin, destination)

                    For j As Integer = 0 To Ticks.Length - 1
                        Dim tickDistancePx As Double = Ticks(j).Location * minScale
                        If ShowAxisValues Then
                            If LabelEachSpoke Then
                                Dim x As Single = CSng(tickDistancePx * Math.Cos(sweepAngle * i - Math.PI / 2) + origin.X)
                                Dim y As Single = CSng(tickDistancePx * Math.Sin(sweepAngle * i - Math.PI / 2) + origin.Y)

                                sf.Alignment = If((x < origin.X), StringAlignment.Far, StringAlignment.Near)
                                sf.LineAlignment = If((y < origin.Y), StringAlignment.Far, StringAlignment.Near)

                                Dim value As Double = Ticks(j).Labels(i)
                                Graphics.DrawString($"{value:f1}", fnt, fontBrush, x, y, sf)

                            ElseIf (i = 0) Then
                                Dim value As Double = Ticks(j).Labels(0)
                                Graphics.DrawString($"{value:f1}", fnt, fontBrush, origin.X, CSng(-tickDistancePx + origin.Y), sf)
                            End If
                        End If
                    Next
                Next
            End Using
        End Sub

        Private Sub RenderImages(origin As PointF, minScale As Double, sweepAngle As Double)
            For i As Integer = 0 To NumberOfSpokes - 1
                Dim sweepOffset As Double = If(ImagePlacement = ImagePlacement.Inside, sweepAngle / 2, 0)
                Dim cosinus As Double = Math.Cos(sweepAngle * i + sweepOffset - Math.PI / 2)
                Dim sinus As Double = Math.Sin(sweepAngle * i + sweepOffset - Math.PI / 2)
                Dim imageWidth As Integer = CategoryImages(i).Width
                Dim imageHeight As Integer = CategoryImages(i).Height

                Dim imageDestination As New PointF(
                    CSng(1.45 * cosinus * minScale + origin.X - imageWidth / 2 * cosinus),
                    CSng(1.45 * sinus * minScale + origin.Y - imageHeight / 2 * sinus))
                Dim rect As New RectangleF(
                    CSng(imageDestination.X - CategoryImages(i).Width / 2),
                    CSng(imageDestination.Y - CategoryImages(i).Height / 2),
                    CategoryImages(i).Width,
                    CategoryImages(i).Height)
                Graphics.DrawImage(CategoryImages(i), rect)
            Next
        End Sub

        Private Sub RenderLabels(dims As PlotDimensions, origin As PointF, minScale As Double, sweepAngle As Double)
            Using fnt As System.Drawing.Font =Drawing.GDI.Font(Font),
                fontBrush As Brush =Drawing.GDI.Brush(Me.Font.Color),
                sf As StringFormat =Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Middle)

                For i As Integer = 0 To NumberOfSpokes - 1
                    Dim textDestination As New PointF(
                        CSng(1.3 * Math.Cos(sweepAngle * i - Math.PI / 2) * minScale + origin.X),
                        CSng(1.3 * Math.Sin(sweepAngle * i - Math.PI / 2) * minScale + origin.Y))
                    If (Math.Abs(textDestination.X - origin.X) < 0.1) Then
                        sf.Alignment = StringAlignment.Center
                    Else
                        sf.Alignment = If(dims.GetCoordinateX(textDestination.X) < 0, StringAlignment.Far, StringAlignment.Near)
                    End If
                    Graphics.DrawString(CategoryLabels(i), fnt, fontBrush, textDestination, sf)
                Next
            End Using
        End Sub

#End Region '/METHODS

    End Class

    Public Structure StarAxisTick

        Public ReadOnly Location As Double
        Public ReadOnly Labels As Double()

        Public Sub New(location As Double, labels As Double())
            Me.Location = location
            Me.Labels = labels
        End Sub

        Public Sub New(location As Double, max As Double)
            Me.Location = location
            Me.Labels = New Double() {location * max}
        End Sub

    End Structure

End Namespace