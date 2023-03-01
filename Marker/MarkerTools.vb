Imports System.Collections.Generic
Imports System.Drawing

Namespace ScottPlot

    Public Module MarkerTools

#Const WINDOWS = True

        Public Sub DrawMarker(gfx As Graphics, pixelLocation As PointF, shape As MarkerShape, size As Single, brush As Brush, pen As Pen)
            If (size = 0) OrElse (shape = MarkerShape.None) Then
                Return
            End If
            Dim m As IMarker = Marker.Create(shape)
            MarkerTools.DrawMarker(gfx, pixelLocation, m, size, brush, pen)
        End Sub

        Public Sub DrawMarker(gfx As Graphics, pixelLocation As PointF, marker As IMarker, size As Single, brush As Brush, pen As Pen)
            If (size = 0) Then
                Return
            End If
            Dim radius As Single = size / 2
#If WINDOWS Then
            'If Not RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            pixelLocation = New PointF(pixelLocation.X + 0.5F, pixelLocation.Y)
            'End If
#End If
            marker.Draw(gfx, pixelLocation, radius, brush, pen)
        End Sub

        Public Sub DrawMarker(gfx As Graphics, pixelLocation As PointF, shape As MarkerShape, size As Single, color As Color, Optional linewidth As Single = 1)
            Using brush As New SolidBrush(color), pen As New Pen(color, linewidth)
                MarkerTools.DrawMarker(gfx, pixelLocation, shape, size, brush, pen)
            End Using
        End Sub

        Public Sub DrawMarkers(gfx As Graphics, pixelLocations As ICollection(Of PointF), shape As MarkerShape, size As Single, color As Color, Optional linewidth As Single = 1)
            Using brush As New SolidBrush(color), pen As New Pen(color, linewidth)
                Dim m As IMarker = Marker.Create(shape)
                For Each pixelLocation As PointF In pixelLocations
                    MarkerTools.DrawMarker(gfx, pixelLocation, m, size, brush, pen)
                Next
            End Using
        End Sub

        Friend Function DiamondPoints(center As PointF, radius As Single) As PointF()
            Dim points = New PointF() {
                New PointF(center.X - radius, center.Y),
                New PointF(center.X, center.Y - radius),
                New PointF(center.X + radius, center.Y),
                New PointF(center.X, center.Y + radius)}
            Return points
        End Function

        Friend Function DiamondPoints(rect As RectangleF) As PointF()
            Dim centerX As Single = rect.Left + rect.Width / 2
            Dim centerY As Single = rect.Top + rect.Height / 2
            Dim points = New PointF() {
                New PointF(rect.Left, centerY),
                New PointF(centerX, rect.Top),
                New PointF(rect.Right, centerY),
                New PointF(centerX, rect.Bottom)
            }
            Return points
        End Function

        Friend Sub DrawRadial(gfx As Graphics, pen As Pen, center As PointF, points As PointF())
            For Each pt As PointF In points
                gfx.DrawLine(pen, center, pt)
            Next
        End Sub

        Friend Function TriangleUpPoints(center As PointF, radius As Single) As PointF()
            Dim rect As New RectangleF(center.X - radius, center.Y - radius, radius + radius, radius + radius)
            Return TriangleUpPoints(rect).Item1
        End Function

        Friend Function TriangleDownPoints(center As PointF, radius As Single) As PointF()
            Dim rect As New RectangleF(center.X - radius, center.Y - radius, radius + radius, radius + radius)
            Return TriangleDownPoints(rect).Item1
        End Function

        Private Function TriangleUpPoints(rect As RectangleF) As Tuple(Of PointF(), PointF)
            Dim centerX As Single = rect.Left + rect.Width / 2
            Dim centerY As Single = rect.Top + rect.Height / 2
            Dim size As Single = rect.Width / 2
            Dim points As PointF() = {
                New PointF(centerX, centerY - size),
                New PointF(centerX - size * 0.866F, centerY + size / 2),
                New PointF(centerX + size * 0.866F, centerY + size / 2)
            }
            Return New Tuple(Of PointF(), PointF)(points, New PointF(centerX, centerY))
        End Function

        Private Function TriangleDownPoints(rect As RectangleF) As Tuple(Of PointF(), PointF)
            Dim centerX As Single = rect.Left + rect.Width / 2
            Dim centerY As Single = rect.Top + rect.Height / 2
            Dim size As Single = rect.Width / 2
            Dim points As PointF() =
            {
                New PointF(centerX, centerY + size),
                New PointF(centerX - size * 0.866F, centerY - size / 2),
                New PointF(centerX + size * 0.866F, centerY - size / 2)
            }
            Return New Tuple(Of PointF(), PointF)(points, New PointF(centerX, centerY))
        End Function

    End Module

End Namespace