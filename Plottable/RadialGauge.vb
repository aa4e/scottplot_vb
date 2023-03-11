Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' This class represents a single radial gauge.
    ''' It has level and styling options and can be rendered onto an existing bitmap using any radius.
    ''' </summary>
    Friend Class RadialGauge

#Region "PROPS, FIELDS"

        Private Const DEG_PER_RAD As Double = 180.0 / Math.PI

        ''' <summary>
        ''' Location of the base of the gauge (degrees).
        ''' </summary>
        Public Property StartAngle As Double

        ''' <summary>
        ''' Current level of this gauge (degrees).
        ''' </summary>
        Public Property SweepAngle As Double

        ''' <summary>
        ''' Maximum angular size of the gauge (swept degrees).
        ''' </summary>
        Public Property MaximumSizeAngle As Double

        ''' <summary>
        ''' Angle where the background starts (degrees).
        ''' </summary>
        Public Property BackStartAngle As Double

        ''' <summary>
        ''' If true angles end clockwise relative to their base.
        ''' </summary>
        Public Property Clockwise As Boolean

        ''' <summary>
        ''' Used internally to get the angle swept by the gauge background. 
        ''' It's equal to 360 degrees if <see cref="CircularBackground"/> is set to true. 
        ''' Also, returns a positive value is the gauge is drawn clockwise and a negative one otherwise.
        ''' </summary>
        Friend Property BackAngleSweep As Double
            Get
                Dim maxBackAngle As Double = If(CircularBackground, 360, MaximumSizeAngle)
                If (Not Clockwise) Then
                    maxBackAngle = -maxBackAngle
                End If
                Return maxBackAngle
            End Get
            Private Set(value As Double)
                _BackAngleSweep = value 'Added for the sweepAngle check in DrawArc due to System.Drawing throwing an OutOfMemoryException.
            End Set
        End Property
        Private _BackAngleSweep As Double = 0

        ''' <summary>
        ''' If true the background will always be drawn as a complete circle regardless of <see cref="MaximumSizeAngle"/>.
        ''' </summary>
        Public Property CircularBackground As Boolean = True

        ''' <summary>
        ''' Font used to render values at the tip of the gauge.
        ''' </summary>
        Public Property Font As ScottPlot.Drawing.Font

        ''' <summary>
        ''' Size of the font relative to the line thickness.
        ''' </summary>
        Public Property FontSizeFraction As Double

        ''' <summary>
        ''' Text to display on top of the label.
        ''' </summary>
        Public Property Label As String

        ''' <summary>
        ''' Location of the label text along the length of the gauge.
        ''' Low values place the label near the base and high values place the label at its tip.
        ''' </summary>
        Public Property LabelPositionFraction As Double

        ''' <summary>
        ''' Size of the gauge (pixels).
        ''' </summary>
        Public Property Width As Double

        ''' <summary>
        ''' Color of the gauge foreground.
        ''' </summary>
        Public Property Color As Color

        ''' <summary>
        ''' Color of the gauge background.
        ''' </summary>
        Public Property BackgroundColor As Color

        ''' <summary>
        ''' Style of the base of the gauge.
        ''' </summary>
        Public Property StartCap As LineCap = LineCap.Round

        ''' <summary>
        ''' Style of the tip of the gauge.
        ''' </summary>
        Public Property EndCap As LineCap = LineCap.Round

        ''' <summary>
        ''' Defines the location of each gauge relative to the start angle and distance from the center.
        ''' </summary>
        Public Property Mode As RadialGaugeMode

        ''' <summary>
        ''' Indicates whether or not labels will be rendered as text.
        ''' </summary>
        Public Property ShowLabels As Boolean

#End Region '/PROPS, FIELDS

#Region "CTOR"

#End Region '/CTOR

#Region "METHODS"

        ''' <summary>
        ''' Render the gauge onto an existing Bitmap
        ''' </summary>
        ''' <param name="gfx">active graphics object</param>
        ''' <param name="dims">plot dimensions (used to determine pixel scaling)</param>
        ''' <param name="centerPixel">pixel location on the bitmap to center the gauge on</param>
        ''' <param name="radius">distance from the center (pixel units) to render the gauge</param>
        Public Sub Render(gfx As Graphics, dims As PlotDimensions, centerPixel As PointF, radius As Single)
            Me.RenderBackground(gfx, centerPixel, radius)
            Me.RenderGaugeForeground(gfx, centerPixel, radius)
            Me.RenderGaugeLabels(gfx, dims, centerPixel, radius)
        End Sub

        Private Sub RenderBackground(gfx As Graphics, center As PointF, radius As Single)
            If Me.Mode = RadialGaugeMode.SingleGauge Then
                Return
            End If
            Using pen As Pen =Drawing.GDI.Pen(Me.BackgroundColor, 1.0, LineStyle.Solid, False)
                pen.Width = CSng(Me.Width)
                pen.StartCap = LineCap.Round
                pen.EndCap = LineCap.Round
                If Math.Abs(Me.BackAngleSweep) <= 0.01 Then
                    Me.BackAngleSweep = 0.0
                End If
                gfx.DrawArc(pen, center.X - radius, center.Y - radius, radius * 2.0F, radius * 2.0F, CSng(Me.BackStartAngle), CSng(Me.BackAngleSweep))
            End Using
        End Sub

        Public Sub RenderGaugeForeground(gfx As Graphics, center As PointF, radius As Single)
            Using pen As Pen =Drawing.GDI.Pen(Me.Color, 1.0, LineStyle.Solid, False)
                pen.Width = CSng(Me.Width)
                pen.StartCap = Me.StartCap
                pen.EndCap = Me.EndCap
                If Math.Abs(Me.SweepAngle) <= 0.01 Then
                    Me.SweepAngle = 0.0
                End If
                gfx.DrawArc(pen, center.X - radius, center.Y - radius, radius * 2.0F, radius * 2.0F, CSng(Me.StartAngle), CSng(Me.SweepAngle))
            End Using
        End Sub

        Private Sub RenderGaugeLabels(gfx As Graphics, dims As PlotDimensions, center As PointF, radius As Single)
            If ShowLabels Then
                'TODO: use this so font size is in pixels not pt
                Using brsh As Brush =Drawing.GDI.Brush(Font.Color),
                    fnt As New System.Drawing.Font(Font.Name, CSng(Width * FontSizeFraction), FontStyle.Bold),
                    sf As StringFormat =Drawing.GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Middle)

                    Dim letterRectangles As RectangleF() = RadialGauge.MeasureCharacters(gfx, fnt, Label, 800, 100)

                    Dim totalLetterWidths As Double = letterRectangles.Select(Function(rect) rect.Width).Sum()
                    Dim textWidthFrac As Double = totalLetterWidths / radius

                    Dim angle As Double = ReduceAngle(StartAngle + SweepAngle * LabelPositionFraction)
                    Dim angle2 As Double = (1 - 2 * LabelPositionFraction) * DEG_PER_RAD * textWidthFrac / 2
                    Dim isPositive As Boolean = (SweepAngle > 0)
                    angle += If(isPositive, angle2, -angle2)

                    Dim isBelow As Boolean = (angle < 180) AndAlso (angle > 0)
                    Dim sign As Integer = If(isBelow, 1, -1)
                    Dim theta As Double = angle * Math.PI / 180
                    theta += textWidthFrac / 2 * sign

                    For i As Integer = 0 To Label.Length - 1
                        theta -= letterRectangles(i).Width / 2 / radius * sign
                        Dim rotation As Double = (theta - Math.PI / 2 * sign) * DEG_PER_RAD
                        Dim x As Single = center.X + radius * CSng(Math.Cos(theta))
                        Dim y As Single = center.Y + radius * CSng(Math.Sin(theta))

                        gfx.RotateTransform(CSng(rotation))
                        gfx.TranslateTransform(x, y, MatrixOrder.Append)
                        gfx.DrawString(Label(i).ToString(), fnt, brsh, 0, 0, sf)
                       Drawing.GDI.ResetTransformPreservingScale(gfx, dims)

                        theta -= letterRectangles(i).Width / 2 / radius * sign
                    Next
                End Using
            End If
        End Sub

        ''' <summary>
        ''' Return an array indicating the size of each character in a string.
        ''' Specifiy the maximum expected size to avoid issues associated with text wrapping.
        ''' </summary>
        Private Shared Function MeasureCharacters(gfx As Graphics, font As System.Drawing.Font, text As String, Optional maxWidth As Integer = 800, Optional maxHeight As Integer = 100) As RectangleF()
            Using sf As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center,
                .Trimming = StringTrimming.None,
                .FormatFlags = StringFormatFlags.MeasureTrailingSpaces
            }
                Dim charRanges As CharacterRange() = Enumerable.Range(0, text.Length).Select(Function(x) New CharacterRange(x, 1)).ToArray()

                sf.SetMeasurableCharacterRanges(charRanges)

                Dim imageRectangle As New RectangleF(0, 0, maxWidth, maxHeight)
                Dim characterRegions As Region() = gfx.MeasureCharacterRanges(text, font, imageRectangle, sf)
                Dim characterRectangles As RectangleF() = characterRegions.Select(Function(x) x.GetBounds(gfx)).ToArray()

                Return characterRectangles
            End Using
        End Function

        ''' <summary>
        ''' Reduces an angle into the range [0°-360°].
        ''' Angles greater than 360 will roll-over (370º becomes 10º).
        ''' Angles less than 0 will roll-under (-10º becomes 350º).
        ''' </summary>
        ''' <param name="angle">Angle value.</param>
        ''' <returns>Angle whithin [0°-360°].</returns>
        Public Shared Function ReduceAngle(angle As Double) As Double
            angle = angle Mod 360.0
            If (angle < 0) Then
                angle += 360.0
            End If
            Return angle
        End Function

#End Region '/METHODS

    End Class

End Namespace