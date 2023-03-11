Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing

Namespace ScottPlot.Renderable

    Public Class ErrorMessage
        Inherits Message

        Public Sub New()
            HAlign = HorizontalAlignment.Left
            VAlign = VerticalAlignment.Upper
            FontColor = Color.Black
            FillColor = Color.FromArgb(50, Color.Red)
            BorderColor = Color.Black
        End Sub

    End Class

    Public Class BenchmarkMessage
        Inherits Message

        Private Sw As New Stopwatch()
        Private MaxRenderTimes As Integer = 100
        Private RenderTimes As New List(Of Double)()

        Public ReadOnly Property MSec As Double
            Get
                Return (Sw.ElapsedTicks * 1000.0 / Stopwatch.Frequency)
            End Get
        End Property

        Public ReadOnly Property Hz As Double
            Get
                Return If(MSec > 0, 1000.0 / MSec, 0)
            End Get
        End Property

        Public ReadOnly Property Message As String
            Get
                Return $"Rendered in {MSec:00.00} ms ({Hz:0000.00} Hz)"
            End Get
        End Property

        Public Sub New()
            Me.HAlign = HorizontalAlignment.Left
            Me.VAlign = VerticalAlignment.Lower
            Me.FontColor = Color.Black
            Me.FillColor = Color.FromArgb(200, Color.Yellow)
            Me.BorderColor = Color.Black
        End Sub

        Public Sub Restart()
            Me.Sw.Restart()
        End Sub

        Public Sub [Stop]()
            Sw.Stop()
            RenderTimes.Add(Sw.Elapsed.TotalMilliseconds)
            While (RenderTimes.Count > MaxRenderTimes)
                RenderTimes.RemoveAt(0)
            End While
            Text = Message
        End Sub

        ''' <summary>
        ''' Returns an array of render times (in milliseconds) of the last several renders.
        ''' The most recent renders are at the end of the array.
        ''' </summary>
        Public Function GetRenderTimes() As Double()
            Return RenderTimes.ToArray()
        End Function

    End Class

    Public Class Message
        Implements IRenderable

        Public Text As String

        Public HAlign As HorizontalAlignment
        Public VAlign As VerticalAlignment

        Public FontColor As Color = Color.Black
        Public FontName As String = Drawing.InstalledFont.Monospace()
        Public FontSize As Single = 12.0F
        Public FontBold As Boolean

        Public FillColor As Color = Color.LightGray
        Public BorderColor As Color = Color.Black
        Public BorderWidth As Single = 1
        Public Padding As Single = 3

        Public Property IsVisible As Boolean = False Implements ScottPlot.Renderable.IRenderable.IsVisible

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Renderable.IRenderable.Render
            If (Not IsVisible) OrElse String.IsNullOrWhiteSpace(Text) Then
                Return
            End If

            Using gfx As Graphics =Drawing.GDI.Graphics(bmp, dims, lowQuality, False),
                fnt As System.Drawing.Font =Drawing.GDI.Font(FontName, FontSize, FontBold),
                fontBrush As New SolidBrush(FontColor),
                fillBrush As New SolidBrush(FillColor),
                borderPen As New Pen(BorderColor, BorderWidth)

                Dim textSize As SizeF =Drawing.GDI.MeasureString(gfx, Text, fnt)
                Dim textHeight As Single = textSize.Height
                Dim textWidth As Single = textSize.Width

                Dim textY As Single = 0
                If (VAlign = VerticalAlignment.Upper) Then
                    textY = dims.DataOffsetY + Padding
                ElseIf (VAlign = VerticalAlignment.Middle) Then
                    textY = dims.DataOffsetY + dims.DataHeight / 2 - textHeight / 2
                ElseIf (VAlign = VerticalAlignment.Lower) Then
                    textY = dims.DataOffsetY + dims.DataHeight - textHeight - Padding
                End If

                Dim textX As Single = 0
                If (HAlign = HorizontalAlignment.Left) Then
                    textX = dims.DataOffsetX + Padding
                ElseIf HAlign = HorizontalAlignment.Center Then
                    textX = dims.DataOffsetX + dims.DataWidth / 2 - textWidth / 2
                ElseIf HAlign = HorizontalAlignment.Right Then
                    textX = dims.DataOffsetX + dims.DataWidth - textWidth - Padding
                End If

                Dim textRect As New RectangleF(textX, textY, textWidth, textHeight)
                gfx.FillRectangle(fillBrush, textRect)
                gfx.DrawRectangle(borderPen, Rectangle.Round(textRect))
                gfx.DrawString(Text, fnt, fontBrush, textX, textY)
            End Using
        End Sub

    End Class

End Namespace