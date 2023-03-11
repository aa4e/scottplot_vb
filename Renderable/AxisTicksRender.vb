Imports System.Collections.Generic
Imports System.Linq
Imports System.Drawing

Namespace ScottPlot.Renderable

    Friend Module AxisTicksRender

        Private Function EdgeIsVertical(edge As Edge) As Boolean
            Return (edge = Edge.Left) OrElse (edge = Edge.Right)
        End Function

        Private Function EdgeIsHorizontal(edge As Edge) As Boolean
            Return (edge = Edge.Top) OrElse (edge = Edge.Bottom)
        End Function

        Public Sub RenderGridLines(dims As PlotDimensions, gfx As Graphics, positions As Double(),
                                   gridLineStyle As LineStyle, gridLineColor As Color, gridLineWidth As Single, edge As Edge)
            If (positions is Nothing) OrElse (positions.Length = 0) OrElse (gridLineStyle = LineStyle.None) Then
                Return
            End If

            'don't draw grid lines on the last pixel to prevent drawing over the data frame
            Dim xEdgeLeft As Single = dims.DataOffsetX + 1
            Dim xEdgeRight As Single = dims.DataOffsetX + dims.DataWidth - 1
            Dim yEdgeTop As Single = dims.DataOffsetY + 1
            Dim yEdgeBottom As Single = dims.DataOffsetY + dims.DataHeight - 1

            If AxisTicksRender.EdgeIsVertical(edge) Then
                Dim x As Single = If(edge = Edge.Left, dims.DataOffsetX, dims.DataOffsetX + dims.DataWidth)
                Dim x2 As Single = If(edge = Edge.Left, dims.DataOffsetX + dims.DataWidth, dims.DataOffsetX)
                Dim ys As IEnumerable(Of Single) = positions _
                    .Select(Function(i) dims.GetPixelY(i)) _
                    .Where(Function(y) (yEdgeTop < y) AndAlso (y < yEdgeBottom))
                If (gridLineStyle <> LineStyle.None) Then
                    Using pen As Pen = Drawing.GDI.Pen(gridLineColor, gridLineWidth, gridLineStyle)
                        For Each y As Single In ys
                            gfx.DrawLine(pen, x, y, x2, y)
                        Next
                    End Using
                End If
            End If

            If AxisTicksRender.EdgeIsHorizontal(edge) Then
                Dim y As Single = If(edge = Edge.Top, dims.DataOffsetY, dims.DataOffsetY + dims.DataHeight)
                Dim y2 As Single = If(edge = Edge.Top, dims.DataOffsetY + dims.DataHeight, dims.DataOffsetY)
                Dim xs As IEnumerable(Of Single) = positions _
                    .Select(Function(i) dims.GetPixelX(i)) _
                    .Where(Function(x) (xEdgeLeft < x) AndAlso (x < xEdgeRight))
                If (gridLineStyle <> LineStyle.None) Then
                    Using pen As Pen = Drawing.GDI.Pen(gridLineColor, gridLineWidth, gridLineStyle)
                        For Each x As Single In xs
                            gfx.DrawLine(pen, x, y, x, y2)
                        Next
                    End Using
                End If
            End If
        End Sub

        Public Sub RenderTickMarks(dims As PlotDimensions, gfx As Graphics, positions As Double(),
                                   tickLength As Single, tickColor As Color, edge As Edge, pixelOffset As Single)
            If (positions is Nothing) OrElse (positions.Length = 0) Then
                Return
            End If

            If AxisTicksRender.EdgeIsVertical(edge) Then
                Dim x As Single = If(edge = Edge.Left, dims.DataOffsetX - pixelOffset, dims.DataOffsetX + dims.DataWidth + pixelOffset)
                Dim tickDelta As Single = If(edge = Edge.Left, -tickLength, tickLength)
                Dim ys As IEnumerable(Of Single) = positions.Select(Function(i) dims.GetPixelY(i))
                Using pen As Pen = Drawing.GDI.Pen(tickColor)
                    For Each y As Single In ys
                        gfx.DrawLine(pen, x, y, x + tickDelta, y)
                    Next
                End Using
            End If

            If AxisTicksRender.EdgeIsHorizontal(edge) Then
                Dim y As Single = If(edge = Edge.Top, dims.DataOffsetY - pixelOffset, dims.DataOffsetY + dims.DataHeight + pixelOffset)
                Dim tickDelta As Single = If(edge = Edge.Top, -tickLength, tickLength)
                Dim xs As IEnumerable(Of Single) = positions.Select(Function(i) dims.GetPixelX(i))
                Using pen As Pen = Drawing.GDI.Pen(tickColor)
                    For Each x As Single In xs
                        gfx.DrawLine(pen, x, y, x, y + tickDelta)
                    Next
                End Using
            End If
        End Sub

        Public Sub RenderTickLabels(dims As PlotDimensions, gfx As Graphics, tc As Ticks.TickCollection,
                                    tickFont As ScottPlot.Drawing.Font, edge As Edge,
                                    rotation As Single, rulerMode As Boolean, PixelOffset As Single,
                                    MajorTickLength As Single, MinorTickLength As Single)
            If (tc.TickLabels is Nothing) OrElse (tc.TickLabels.Length = 0) Then
                Return
            End If

            Using fnt As System.Drawing.Font = Drawing.GDI.Font(tickFont),
                brush As Brush = Drawing.GDI.Brush(tickFont.Color),
                sf As StringFormat = Drawing.GDI.StringFormat()

                Dim visibleMajorTicks As Ticks.Tick() = tc.GetVisibleMajorTicks(dims)

                Select Case edge
                    Case Edge.Bottom
                        For i As Integer = 0 To visibleMajorTicks.Length - 1
                            Dim x As Single = dims.GetPixelX(visibleMajorTicks(i).Position)
                            Dim y As Single = dims.DataOffsetY + dims.DataHeight + MajorTickLength + PixelOffset

                            gfx.TranslateTransform(x, y)
                            gfx.RotateTransform(-rotation)
                            sf.Alignment = If(rotation = 0, StringAlignment.Center, StringAlignment.Far)
                            If rulerMode Then
                                sf.Alignment = StringAlignment.Near
                            End If
                            sf.LineAlignment = If(rotation = 0, StringAlignment.Near, StringAlignment.Center)
                            gfx.DrawString(visibleMajorTicks(i).Label, fnt, brush, 0, 0, sf)
                            Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
                        Next

                    Case Edge.Top
                        For i As Integer = 0 To visibleMajorTicks.Length - 1
                            Dim x As Single = dims.GetPixelX(visibleMajorTicks(i).Position)
                            Dim y As Single = dims.DataOffsetY - MajorTickLength - PixelOffset

                            gfx.TranslateTransform(x, y)
                            gfx.RotateTransform(-rotation)
                            sf.Alignment = If(rotation = 0, StringAlignment.Center, StringAlignment.Near)
                            If rulerMode Then
                                sf.Alignment = StringAlignment.Near
                            End If
                            sf.LineAlignment = If(rotation = 0, StringAlignment.Far, StringAlignment.Center)
                            gfx.DrawString(visibleMajorTicks(i).Label, fnt, brush, 0, 0, sf)
                            Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
                        Next

                    Case Edge.Left
                        For i As Integer = 0 To visibleMajorTicks.Length - 1
                            Dim x As Single = dims.DataOffsetX - PixelOffset - MajorTickLength
                            Dim y As Single = dims.GetPixelY(visibleMajorTicks(i).Position)

                            gfx.TranslateTransform(x, y)
                            gfx.RotateTransform(-rotation)
                            sf.Alignment = StringAlignment.Far
                            sf.LineAlignment = If(rulerMode, StringAlignment.Far, StringAlignment.Center)
                            If (rotation = 90) Then
                                sf.Alignment = StringAlignment.Center
                                sf.LineAlignment = StringAlignment.Far
                            End If
                            gfx.DrawString(visibleMajorTicks(i).Label, fnt, brush, 0, 0, sf)
                            Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
                        Next

                    Case Edge.Right
                        For i As Integer = 0 To visibleMajorTicks.Length - 1
                            Dim x As Single = dims.DataOffsetX + PixelOffset + MajorTickLength + dims.DataWidth
                            Dim y As Single = dims.GetPixelY(visibleMajorTicks(i).Position)

                            gfx.TranslateTransform(x, y)
                            gfx.RotateTransform(-rotation)
                            sf.Alignment = StringAlignment.Near
                            sf.LineAlignment = If(rulerMode, StringAlignment.Far, StringAlignment.Center)
                            If (rotation = 90) Then
                                sf.Alignment = StringAlignment.Center
                                sf.LineAlignment = StringAlignment.Near
                            End If
                            gfx.DrawString(visibleMajorTicks(i).Label, fnt, brush, 0, 0, sf)
                            Drawing.GDI.ResetTransformPreservingScale(gfx, dims)
                        Next

                    Case Else
                        Throw New NotImplementedException($"Unsupported edge type {edge}.")
                End Select

                If (Not String.IsNullOrWhiteSpace(tc.CornerLabel)) Then
                    Select Case edge
                        Case Edge.Left
                            sf.Alignment = StringAlignment.Far
                            sf.LineAlignment = StringAlignment.Far
                            gfx.DrawString(tc.CornerLabel,
                                           fnt,
                                           brush,
                                           dims.DataOffsetX - MajorTickLength - PixelOffset,
                                           dims.DataOffsetY,
                                           sf)

                        Case Edge.Bottom
                            sf.Alignment = StringAlignment.Far
                            sf.LineAlignment = StringAlignment.Near
                            gfx.DrawString(Environment.NewLine & tc.CornerLabel,
                                           fnt,
                                           brush,
                                           dims.DataOffsetX + dims.DataWidth,
                                           dims.DataOffsetY + dims.DataHeight + MajorTickLength + PixelOffset,
                                           sf)

                        Case Edge.Right
                            sf.Alignment = StringAlignment.Near
                            sf.LineAlignment = StringAlignment.Far
                            gfx.DrawString(Environment.NewLine & tc.CornerLabel,
                                           fnt,
                                           brush,
                                           dims.DataOffsetX + dims.DataWidth + MajorTickLength + PixelOffset,
                                           dims.DataOffsetY,
                                           sf)

                        Case Edge.Top
                            sf.Alignment = StringAlignment.Far
                            sf.LineAlignment = StringAlignment.Far
                            gfx.DrawString(tc.CornerLabel & Environment.NewLine & Environment.NewLine,
                                           fnt,
                                           brush,
                                           dims.DataOffsetX + dims.DataWidth,
                                           dims.DataOffsetY - MajorTickLength - PixelOffset,
                                           sf)

                        Case Else
                            Throw New NotImplementedException($"Unsupported edge type {edge}.")
                    End Select
                End If
            End Using
        End Sub

    End Module

End Namespace