Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' This class represents a single Bar shown as part of a collection of Bars on a <see cref="BarSeries"/> plot.
    ''' </summary>
    Public Class Bar

        ''' <summary>
        ''' Styling of the text displayed above the bar.
        ''' </summary>
        Public ReadOnly Font As New ScottPlot.Drawing.Font()

        ''' <summary>
        ''' Horizontal position of the center of the bar (or vertical position if it's a horizontal bar).
        ''' </summary>
        Public Property Position As Double

        ''' <summary>
        ''' Vertical position of the top of the bar (or right edge if it's a horizontal bar).
        ''' </summary>
        Public Property Value As Double

        ''' <summary>
        ''' Vertical position of the bottom of the bar (щr left edge if it's a horizontal bar).
        ''' </summary>
        Public Property ValueBase As Double

        ''' <summary>
        ''' Text to display above the bar.
        ''' </summary>
        Public Property Label As String

        ''' <summary>
        ''' Horizontal width of the bar in axis units (or vertical height if it's a horizontal bar).
        ''' </summary>
        Public Property Thickness As Double = 0.8

        ''' <summary>
        ''' Color filling the rectangular area of the bar.
        ''' </summary>
        Public Property FillColor As Color = Color.Gray

        ''' <summary>
        ''' Color of the line outlining the rectangular area of the bar.
        ''' </summary>
        Public Property LineColor As Color = Color.Black

        ''' <summary>
        ''' Width of the line outlining the rectangular area of the bar.
        ''' </summary>
        Public Property LineWidth As Single = 0

        ''' <summary>
        ''' Indicates whether bars extend upward (vertical, default) or two the right (horizontal).
        ''' </summary>
        Public Property IsVertical As Boolean = True

        Public Function GetLimits() As AxisLimits
            Dim top As Double = Math.Max(Value, ValueBase)
            Dim bot As Double = Math.Min(Value, ValueBase)
            Dim left As Double = Position - Thickness / 2
            Dim right As Double = Position + Thickness / 2
            'Make room for labels, if present
            If (Label IsNot Nothing) Then
                Dim span As Double = top - bot
                top += 0.1 * span
                bot -= 0.1 * span
            End If
            Return If(IsVertical,
                New AxisLimits(left, right, bot, top),
                New AxisLimits(bot, top, left, right))
        End Function

    End Class

End Namespace