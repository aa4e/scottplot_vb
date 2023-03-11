Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' This class describes a single item that appears in the figure legend.
    ''' </summary>
    Public Class LegendItem

#Region "PROPS, FIELDS"

        Public HatchStyle As Drawing.HatchStyle
        Public ReadOnly Parent As IPlottable

        Public Property Label As String
        Public Property Color As Color
        Public Property HatchColor As Color
        Public Property BorderColor As Color
        Public Property BorderWith As Single
        Public Property BorderLineStyle As LineStyle
        Public Property LineStyle As LineStyle

        Public Property LineWidth As Double
            Get
                Dim hasLine As IHasLine = TryCast(Parent, IHasLine)
                If (hasLine is Nothing) Then
                    Return _LineWidth
                End If
                Return Math.Min(hasLine.LineWidth, 10.0)
            End Get
            Set(value As Double)
                _LineWidth = value
            End Set
        End Property
        Private Property _LineWidth As Double

        Public ReadOnly Property LineColor As Color
            Get
                Dim hasLine As IHasLine = TryCast(Parent, IHasLine)
                If (hasLine is Nothing) Then
                    Return Color
                End If
                Return hasLine.LineColor
            End Get
        End Property

        Public Property MarkerShape As MarkerShape

        Public Property MarkerSize As Single
            Get
                Dim hasMarker As IHasMarker = TryCast(Parent, IHasMarker)
                If (hasMarker is Nothing) Then
                    Return _MarkerSize
                End If
                Return hasMarker.MarkerSize
            End Get
            Set(value As Single)
                _MarkerSize = value
            End Set
        End Property
        Private _MarkerSize As Single

        Public ReadOnly Property MarkerLineWidth As Single
            Get
                Dim hasMarker As IHasMarker = TryCast(Parent, IHasMarker)
                If (hasMarker is Nothing) Then
                    Return CSng(LineWidth)
                End If
                Return Math.Min(hasMarker.MarkerLineWidth, 3)
            End Get
        End Property

        Public ReadOnly Property MarkerColor As Color
            Get
                Dim hasMarker As IHasMarker = TryCast(Parent, IHasMarker)
                If (hasMarker is Nothing) Then
                    Return Color
                End If
                Return hasMarker.MarkerColor
            End Get
        End Property

        Public ReadOnly Property ShowAsRectangleInLegend As Boolean
            Get
                Dim hasVeryLargeLineWidth As Boolean = (LineWidth >= 10)
                Dim hasArea As Boolean = (Parent IsNot Nothing) AndAlso (TypeOf Parent is IHasArea)
                Return (hasVeryLargeLineWidth OrElse hasArea)
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(parent As IPlottable)
            Me.Parent = parent
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Shared Function SingleItem(parent As IPlottable, label As String) As LegendItem()
            Dim leg As New LegendItem(parent) With {.Label = label}
            Return New LegendItem() {leg}
        End Function

        Public Sub Render(gfx As Graphics, x As Single, y As Single, labelWidth As Single, labelHeight As Single, labelFont As System.Drawing.Font, symbolWidth As Single, symbolPad As Single, outlinePen As Pen, textBrush As SolidBrush, legendItemHideBrush As Brush)
            'Draw text
            gfx.DrawString(Me.Label, labelFont, textBrush, x + symbolWidth, y)

            'Prepare values for drawing a line
            outlinePen.Color = Me.Color
            outlinePen.Width = 1
            Dim lineY As Single = y + labelHeight / 2
            Dim lineX1 As Single = x + symbolPad
            Dim lineX2 As Single = lineX1 + symbolWidth - symbolPad * 2

            If ShowAsRectangleInLegend Then
                Dim rectOrigin As New PointF(lineX1, lineY - 5)
                Dim rectSize As New SizeF(lineX2 - lineX1, 10)
                Dim rect As New RectangleF(rectOrigin, rectSize)

                'Draw a rectangle
                Using legendItemFillBrush As Brush = Drawing.GDI.Brush(Color, HatchColor, HatchStyle),
                    legendItemOutlinePen As Pen = Drawing.GDI.Pen(BorderColor, BorderWith, BorderLineStyle)
                    gfx.FillRectangle(legendItemFillBrush, rect)
                    gfx.DrawRectangle(legendItemOutlinePen, rect.X, rect.Y, rect.Width, rect.Height)
                End Using
            Else
                'Draw a line
                If (LineWidth > 0) AndAlso (LineStyle <> LineStyle.None) Then
                    Using linePen As Pen = Drawing.GDI.Pen(LineColor, LineWidth, LineStyle, False)
                        gfx.DrawLine(linePen, lineX1, lineY, lineX2, lineY)
                    End Using
                End If

                'And perhaps a marker in the middle of the line
                Dim lineXcenter As Single = (lineX1 + lineX2) / 2
                Dim pixelLocation As New PointF(lineXcenter, lineY)
                If (MarkerShape <> MarkerShape.None) AndAlso (MarkerSize > 0) Then
                    MarkerTools.DrawMarker(gfx, pixelLocation, MarkerShape, MarkerSize, MarkerColor, MarkerLineWidth)
                End If
            End If

            'Typically invisible legend items don't make it in the list.'
            'If they do, display them simulating semi-transparency by drawing a white box over the legend item
            If (Not Parent.IsVisible) Then
                Dim hideRectOrigin As New PointF(lineX1, y)
                Dim hideRectSize As New SizeF(symbolWidth + labelWidth + symbolPad, labelHeight)
                Dim hideRect As New RectangleF(hideRectOrigin, hideRectSize)
                gfx.FillRectangle(legendItemHideBrush, hideRect)
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace