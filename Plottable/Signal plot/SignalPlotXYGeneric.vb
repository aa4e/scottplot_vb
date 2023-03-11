Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' A variation of the SignalPlotConst optimized for unevenly-spaced ascending X values.
    ''' </summary>
    Public Class SignalPlotXYGeneric(Of TX As {Structure, IComparable}, TY As {Structure, IComparable})
        Inherits SignalPlotBase(Of TY)
        Implements IHasPointsGenericX(Of TX, TY)

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Indicates whether Xs have been validated to ensure all values are ascending.
        ''' Validation occurs before the first render (Not at assignment) to allow the user time to set min/max render indexes.
        ''' </summary>
        Private XsHaveBeenValidated As Boolean = False

        Public Property Xs As TX()
            Get
                Return _Xs
            End Get
            Set(value As TX())
                If (value is Nothing) Then
                    Throw New ArgumentException("XS cannot be null.")
                End If
                If (value.Length = 0) Then
                    Throw New ArgumentException("XS must have at least one element.")
                End If
                _Xs = value
                XsHaveBeenValidated = False
            End Set
        End Property
        Private _Xs As TX()

        Public Overrides Property Ys As TY()
            Get
                Return _Ys
            End Get
            Set(value As TY())
                If (value.Length = 0) Then
                    Throw New ArgumentException("YS must have at least one element.")
                End If
                MyBase.Ys = value
            End Set
        End Property

        Private Shared SubstractExp As Func(Of TX, TX, TX) = NumericConversion.CreateSubtractFunction(Of TX)()
        Private Shared LessThanOrEqualExp As Func(Of TX, TX, Boolean) = NumericConversion.CreateLessThanOrEqualFunction(Of TX)()

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New()
            If (Type.GetTypeCode(GetType(TX)) = TypeCode.Byte) Then
                Throw New InvalidOperationException("SignalXY plots cannot use a byte array for their horizontal axis positions.")
            End If
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Public Overrides Function GetAxisLimits() As AxisLimits
            Dim baseLimits As AxisLimits = MyBase.GetAxisLimits()
            Dim newXmin As Double = NumericConversion.GenericToDouble(Of TX)(Xs, MinRenderIndex) + OffsetX
            Dim newXmax As Double = NumericConversion.GenericToDouble(Of TX)(Xs, MaxRenderIndex) + OffsetX
            Return New AxisLimits(newXmin, newXmax, baseLimits.YMin, baseLimits.YMax)
        End Function

        Public Iterator Function ProcessInterval(x As Integer, from As Integer, length As Integer, dims As PlotDimensions) As IEnumerable(Of PointF)

            Dim startTx As TX = Nothing
            Dim endTx As TX = Nothing
            NumericConversion.DoubleToGeneric(Of TX)(dims.XMin + dims.XSpan / dims.DataWidth * x - OffsetX, startTx)
            NumericConversion.DoubleToGeneric(Of TX)(dims.XMin + dims.XSpan / dims.DataWidth * (x + 1) - OffsetX, endTx)

            Dim startIndex As Integer = Array.BinarySearch(Of TX)(Xs, from, length, startTx)
            If (startIndex < 0) Then
                startIndex = Not startIndex 'TEST: startIndex = ~startIndex;
            End If

            Dim endIndex As Integer = Array.BinarySearch(Of TX)(Xs, from, length, endTx)
            If (endIndex < 0) Then
                endIndex = Not endIndex
            End If

            If (startIndex = endIndex) Then
                Return
            End If

            Dim min As Double = 0
            Dim max As Double = 0
            Strategy.MinMaxRangeQuery(startIndex, endIndex - 1, min, max)

            Dim pointsCount As Integer = endIndex - startIndex
            Yield New PointF(CSng(x) + dims.DataOffsetX, dims.GetPixelY(Strategy.SourceElement(startIndex) + OffsetYAsDouble))
            If (pointsCount > 1) Then
                Yield New PointF(x + dims.DataOffsetX, dims.GetPixelY(min + OffsetYAsDouble))
                Yield New PointF(x + dims.DataOffsetX, dims.GetPixelY(max + OffsetYAsDouble))
                Yield New PointF(x + dims.DataOffsetX, dims.GetPixelY(Strategy.SourceElement(endIndex - 1) + OffsetYAsDouble))
            End If
            Return
        End Function

        Public Overrides Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False)
            If (Not XsHaveBeenValidated) Then
                Validate.AssertDoesNotDescend(Of TX)("xs", Xs, MinRenderIndex, MaxRenderIndex)
                XsHaveBeenValidated = True
            End If

            Using gfx As Graphics = Drawing.GDI.Graphics(bmp, dims, lowQuality),
                brush As New SolidBrush(Color),
                penHD As Pen = Drawing.GDI.Pen(Color, CSng(LineWidth), LineStyle, True)

                Dim pointBefore As PointF()
                Dim pointAfter As PointF()
                Dim searchFrom As Integer
                Dim searchTo As Integer

                'Calculate point before displayed points
                Dim x As TX = Nothing
                NumericConversion.DoubleToGeneric(dims.XMin - OffsetX, x)
                Dim pointBeforeIndex As Integer = Array.BinarySearch(Xs, MinRenderIndex, MaxRenderIndex - MinRenderIndex + 1, x)
                If (pointBeforeIndex < 0) Then
                    pointBeforeIndex = Not pointBeforeIndex
                End If

                If (pointBeforeIndex > MinRenderIndex) Then
                    pointBefore = New PointF() {
                        New PointF(dims.GetPixelX(NumericConversion.GenericToDouble(Xs, pointBeforeIndex - 1) + OffsetX),
                                   dims.GetPixelY(Strategy.SourceElement(pointBeforeIndex - 1) + OffsetYAsDouble))
                    }
                    searchFrom = pointBeforeIndex
                Else
                    pointBefore = New PointF() {}
                    searchFrom = MinRenderIndex
                End If

                'Calculate point after displayed points
                NumericConversion.DoubleToGeneric(Of TX)(dims.XMin - OffsetX, x)
                Dim pointAfterIndex As Integer = Array.BinarySearch(Xs, MinRenderIndex, MaxRenderIndex - MinRenderIndex + 1, x)
                If (pointAfterIndex < 0) Then
                    pointAfterIndex = Not pointAfterIndex
                End If

                If (pointAfterIndex <= MaxRenderIndex) Then
                    pointAfter = New PointF() {
                        New PointF(dims.GetPixelX(NumericConversion.GenericToDouble(Xs, pointAfterIndex) + OffsetX),
                                   dims.GetPixelY(Strategy.SourceElement(pointAfterIndex) + OffsetYAsDouble))
                    }
                    searchTo = pointAfterIndex
                Else
                    pointAfter = New PointF() {}
                    searchTo = MaxRenderIndex
                End If

                Dim visiblePoints As IEnumerable(Of PointF)
                If UseParallel Then
                    visiblePoints = Enumerable.Range(0, CInt(Math.Round(dims.DataWidth))) _
                                                     .AsParallel().AsOrdered().Select(Function(a) ProcessInterval(a, searchFrom, searchTo - searchFrom + 1, dims)) _
                                                     .SelectMany(Function(a) a)
                Else
                    visiblePoints = Enumerable.Range(0, CInt(Math.Round(dims.DataWidth))) _
                        .Select(Function(a) ProcessInterval(a, searchFrom, searchTo - searchFrom + 1, dims)) _
                        .SelectMany(Function(a) a)
                End If

                Dim pointsToDraw As PointF() = pointBefore.Concat(visiblePoints).Concat(pointAfter).ToArray()

                'Interpolate before displayed point to make it x = -1 (close to visible area);
                'this fix extreme zoom in bug
                If (pointBefore.Length > 0) AndAlso (pointsToDraw.Length >= 2) AndAlso (Not StepDisplay) Then
                    ' only extrapolate if points are different (otherwise extrapolated point may be infinity)
                    If (pointsToDraw(0).X <> pointsToDraw(1).X) Then
                        Dim x0 As Single = -1 + dims.DataOffsetX
                        Dim y0 As Single = pointsToDraw(1).Y + (pointsToDraw(0).Y - pointsToDraw(1).Y) * (x0 - pointsToDraw(1).X) / (pointsToDraw(0).X - pointsToDraw(1).X)
                        pointsToDraw(0) = New PointF(x0, y0)
                    End If
                End If

                ' Interpolate after displayed point to make it x = datasize.Width(close to visible area)
                ' this Fix extreme zoom in bug
                If (pointAfter.Length > 0) AndAlso (pointsToDraw.Length >= 2) AndAlso (Not StepDisplay) Then
                    Dim lastPoint As PointF = pointsToDraw(pointsToDraw.Length - 2)
                    Dim afterPoint As PointF = pointsToDraw(pointsToDraw.Length - 1)

                    ' only extrapolate if points are different (otherwise extrapolated point may be infinity)
                    If (afterPoint.X <> lastPoint.X) Then
                        Dim x1 As Single = dims.DataWidth + dims.DataOffsetX
                        Dim y1 As Single = lastPoint.Y + (afterPoint.Y - lastPoint.Y) * (x1 - lastPoint.X) / (afterPoint.X - lastPoint.X)
                        pointsToDraw(pointsToDraw.Length - 1) = New PointF(x1, y1)
                    End If
                End If

                Dim markersToDraw As PointF() = pointsToDraw

                'Simulate a step display by adding extra points at the corners.
                If (StepDisplay) Then
                    pointsToDraw = ScatterPlot.GetStepDisplayPoints(pointsToDraw, StepDisplayRight)
                End If

                'Fill below the line
                Select Case _FillType
                    Case FillType.NoFill

                    Case FillType.FillAbove
                        FillToInfinity(dims, gfx, pointsToDraw(0).X, pointsToDraw(pointsToDraw.Length - 1).X, pointsToDraw, True)

                    Case FillType.FillBelow
                        FillToInfinity(dims, gfx, pointsToDraw(0).X, pointsToDraw(pointsToDraw.Length - 1).X, pointsToDraw, False)

                    Case FillType.FillAboveAndBelow
                        FillToBaseline(dims, gfx, pointsToDraw(0).X, pointsToDraw(pointsToDraw.Length - 1).X, pointsToDraw, BaselineY)

                    Case Else
                        Throw New InvalidOperationException("Unsupported fill type.")
                End Select

                ' Draw lines
                If (pointsToDraw.Length > 1) AndAlso (LineStyle <> LineStyle.None) AndAlso (LineWidth > 0) Then
                    ValidatePoints(pointsToDraw)
                    gfx.DrawLines(penHD, pointsToDraw)
                End If

                ' draw markers
                If (markersToDraw.Length >= 1) Then
                    Dim dataSpanXPx As Single = markersToDraw(markersToDraw.Length - 1).X - markersToDraw(0).X
                    Dim markerPxRadius As Single = 0.3F * dataSpanXPx / markersToDraw.Length
                    markerPxRadius = If(markersToDraw.Length > 1,
                        Math.Min(markerPxRadius, MarkerSize / 2),
                        MarkerSize / 2)
                    Dim scaledMarkerSize As Single = markerPxRadius * 2

                    If (markerPxRadius > 0.3) Then
                        ShowMarkersInLegend = True

                        'skip not visible before and after points
                        Dim pointsWithMarkers = markersToDraw.Skip(pointBefore.Length) _
                            .Take(markersToDraw.Length - pointBefore.Length - pointAfter.Length).ToArray()
                        MarkerTools.DrawMarkers(gfx, pointsWithMarkers, MarkerShape, scaledMarkerSize, MarkerColor, MarkerLineWidth)
                    Else
                        ShowMarkersInLegend = False
                    End If
                End If
            End Using
        End Sub

        Public Overrides Sub ValidateData(Optional deep As Boolean = False)
            'base can only check Ys
            MyBase.ValidateData(deep)

            'X checking must be performed here
            Validate.AssertEqualLength(Of TX, TY)($"{NameOf(Xs)} and {NameOf(Ys)}", Xs, Ys)
            Validate.AssertHasElements(Of TX)(NameOf(Xs), Xs)

            If deep Then
                Validate.AssertAllReal(Of TX)("Xs", Xs)
                Validate.AssertDoesNotDescend(Of TX)("Xs", Xs, MinRenderIndex, MaxRenderIndex)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim lbl As String = If(String.IsNullOrWhiteSpace(label), "", $" ({label})")
            Return $"PlottableSignalXYGeneric{lbl} with {PointCount} points ({GetType(TX).Name}, {GetType(TY).Name})."
        End Function

        Private Function GetPointByIndex(index As Integer) As Tuple(Of TX, TY, Integer)
            Return New Tuple(Of TX, TY, Integer)(Xs(index), Ys(index), index)
        End Function

        ''' <summary>
        ''' Return the X/Y coordinates of the point nearest the X position.
        ''' </summary>
        ''' <param name="x">X position in plot space.</param>
        Public Shadows Function GetPointNearestX(x As TX) As Tuple(Of TX, TY, Integer) Implements ScottPlot.Plottable.IHasPointsGenericX(Of TX, TY).GetPointNearestX
            Dim index As Integer = Array.BinarySearch(Of TX)(Xs, MinRenderIndex, MaxRenderIndex - MinRenderIndex, x)
            If (index < 0) Then
                index = Not index
            Else 'x equal to XS element
                Return GetPointByIndex(index)
            End If

            'x lower then any MinRenderIndex
            If (index <= MinRenderIndex) Then
                Return GetPointByIndex(MinRenderIndex)
            End If

            'x higher then MaxRenderIndex
            If (index > MaxRenderIndex) Then
                Return GetPointByIndex(MaxRenderIndex)
            End If

            Dim distLeft As TX = SubstractExp(x, Xs(index - 1))
            Dim distRight As TX = SubstractExp(Xs(index), x)
            If LessThanOrEqualExp(distLeft, distRight) Then 'x closer to XS[index -1]
                Return GetPointByIndex(index - 1)
            End If
            Return GetPointByIndex(index) 'x closer to XS[index]
        End Function

        ''' <summary>
        ''' Return the vertical range of values between the given horizontal positions.
        ''' </summary>
        Public Shadows Function GetYDataRange(xMin As TX, xMax As TX) As Tuple(Of TY, TY) Implements ScottPlot.Plottable.IHasPointsGenericX(Of TX, TY).GetYDataRange
            Return MyBase.GetYDataRange(NumericConversion.GenericToDouble(Of TX)(xMin),
                                        NumericConversion.GenericToDouble(Of TX)(xMax))
        End Function

#End Region '/METHODS

    End Class

End Namespace