Imports System.Runtime.CompilerServices

Namespace ScottPlot.Renderable

    ''' <summary>
    ''' This class stores MUTABLE axis limits and pixel size information for a SINGLE AXIS. 
    ''' Unlike PlotDimensions(immutable objects created just before rendering), values in this class are intended for long term storage.
    ''' </summary>
    Public Class AxisDimensions

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' Size of the entire figure (in pixels) if it were to be exported as an image.
        ''' </summary>
        Public ReadOnly Property FigureSizePx As Single = 0

        ''' <summary>
        ''' Side of just the data area (in pixels). The data area is the inner rectangle that displays plots.
        ''' </summary>
        Public ReadOnly Property DataSizePx As Single = 0

        ''' <summary>
        ''' Offset of the data area (in pixels) relative to the left or top edge of the figure.
        ''' </summary>
        Public ReadOnly Property DataOffsetPx As Single = 0

        ''' <summary>
        ''' Lower edge of the data area (axis units).
        ''' </summary>
        Public ReadOnly Property Min As Double = Double.NaN

        ''' <summary>
        ''' Upper edge of the data area (axis units).
        ''' </summary>
        Public ReadOnly Property Max As Double = Double.NaN

        ''' <summary>
        ''' Limit beyond which the plot cannot be zoomed in.
        ''' </summary>
        Public ReadOnly Property OuterBoundaryMin As Double = Double.NegativeInfinity

        ''' <summary>
        ''' Limit beyond which the plot cannot be zoomed in.
        ''' </summary>
        Public ReadOnly Property OuterBoundaryMax As Double = Double.PositiveInfinity

        ''' <summary>
        ''' Limit which will always be visible on the data area.
        ''' </summary>
        Public ReadOnly Property InnerBoundaryMin As Double = Double.PositiveInfinity

        ''' <summary>
        ''' Limit which will always be visible on the data area.
        ''' </summary>
        Public ReadOnly Property InnerBoundaryMax As Double = Double.NegativeInfinity

        ''' <summary>
        ''' False until axes are intentionally set. Unset axes default to NaN min/max limits.
        ''' </summary>
        Public ReadOnly Property HasBeenSet As Boolean = False

        ''' <summary>
        ''' Size of the view boundaries. This should always be greater or equal to the <see cref="Span"/>.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SpanBound As Double
            Get
                Return (OuterBoundaryMax - OuterBoundaryMin)
            End Get
        End Property

        ''' <summary>
        ''' True if min or max is NaN.
        ''' </summary>
        Public ReadOnly Property IsNan As Boolean
            Get
                Return (Double.IsNaN(Min) OrElse Double.IsNaN(Max))
            End Get
        End Property

        ''' <summary>
        ''' Size of the data area (axis units).
        ''' </summary>
        Public ReadOnly Property Span As Double
            Get
                Return (Max - Min)
            End Get
        End Property

        ''' <summary>
        ''' Center of the data area (axis units).
        ''' </summary>
        Public ReadOnly Property Center As Double
            Get
                Return (Max + Min) * 0.5
            End Get
        End Property

        ''' <summary>
        ''' Number of pixels for each axis unit.
        ''' </summary>
        Public ReadOnly Property PxPerUnit As Double
            Get
                Return (DataSizePx / Span)
            End Get
        End Property

        ''' <summary>
        ''' Size of 1 screen pixel in axis units.
        ''' </summary>
        Public ReadOnly Property UnitsPerPx As Double
            Get
                Return (Span / DataSizePx)
            End Get
        End Property

        ''' <summary>
        ''' Indicates whether pixel values ascend in the same direciton as axis unit values.
        ''' Horizontal axes are Not inverted (both ascend from left to right).
        ''' Vertical axes are inverted (units ascend from bottom to top but pixel positions ascend from top to bottom).
        ''' </summary>
        Public IsInverted As Boolean = False

        ''' <summary>
        ''' Limit remember/recall is used while mouse dragging.
        ''' </summary>
        Private MinRemembered As Double = Double.NaN

        ''' <summary>
        ''' Limit remember/recall is used while mouse dragging.
        ''' </summary>
        Private MaxRemembered As Double = Double.NaN

        ''' <summary>
        ''' If true, min/max cannot bet set.
        ''' </summary>
        Private LockedLimits As Boolean = False

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"Axis ({Min} to {Max}), figure size {FigureSizePx}, data size {DataSizePx}."
        End Function

        ''' <summary>
        ''' Limit remember/recall is used while mouse dragging.
        ''' </summary>
        Public Sub Remember()
            MinRemembered = Min
            MaxRemembered = Max
        End Sub

        ''' <summary>
        ''' Limit remember/recall is used while mouse dragging.
        ''' </summary>
        Public Sub Recall()
            _Min = MinRemembered
            _Max = MaxRemembered
        End Sub

        ''' <summary>
        ''' Return limits that contain no NaNs. NaNs will be replaced with +/- 10.
        ''' </summary>
        Public Function RationalLimits() As Tuple(Of Double, Double)
            Dim min As Double = If(Double.IsNaN(Me.Min), -10, Me.Min)
            Dim max As Double = If(Double.IsNaN(Me.Max), 10, Me.Max)
            Return If(min = max, New Tuple(Of Double, Double)(min - 0.5, max + 0.5), New Tuple(Of Double, Double)(min, max))
        End Function

        ''' <summary>
        ''' Reset the axis as if it were never set.
        ''' </summary>
        Public Sub ResetLimits()
            _Min = Double.NaN
            _Max = Double.NaN
            _HasBeenSet = False
            SetBoundsOuter()
            SetBoundsInner()
        End Sub

        ''' <summary>
        ''' Resize/reposition this axis according to the given pixel units.
        ''' </summary>
        Public Sub Resize(figureSizePx As Single, Optional dataSizePx As Single? = Nothing, Optional dataOffsetPx As Single? = Nothing)
            Me._FigureSizePx = figureSizePx
            Me._DataSizePx = If(dataSizePx, Me.DataSizePx)
            Me._DataOffsetPx = If(dataOffsetPx, Me.DataOffsetPx)
        End Sub

        ''' <summary>
        ''' Set data size and offset based on desired padding between the edge of the figure and data area.
        ''' </summary>
        Public Sub SetPadding(padBefore As Single, padAfter As Single)
            _DataOffsetPx = padBefore
            _DataSizePx = FigureSizePx - padBefore - padAfter
        End Sub

        ''' <summary>
        ''' Set boundaries beyond which this axis cannot be panned or zoomed.
        ''' </summary>
        Public Sub SetBoundsOuter(Optional lower As Double = Double.NegativeInfinity, Optional upper As Double = Double.PositiveInfinity)
            _OuterBoundaryMin = lower
            _OuterBoundaryMax = upper
        End Sub

        ''' <summary>
        ''' Set boundaries beyond which this axis cannot be panned or zoomed.
        ''' </summary>
        Public Sub SetBoundsInner(Optional lower As Double = Double.NegativeInfinity, Optional upper As Double = Double.PositiveInfinity)
            _InnerBoundaryMin = lower
            _InnerBoundaryMax = upper
        End Sub

        ''' <summary>
        ''' Modify axis limits such that none extend beyond the boundaries.
        ''' </summary>
        Private Sub ApplyBounds()
            If (Span > SpanBound) Then
                _Min = OuterBoundaryMin
                _Max = OuterBoundaryMax
                Return
            End If
            If (Min < OuterBoundaryMin) Then
                Dim span As Double = span
                _Min = OuterBoundaryMin
                _Max = OuterBoundaryMin + span
            End If
            If (Max > OuterBoundaryMax) Then
                Dim span2 As Double = Span
                _Max = OuterBoundaryMax
                _Min = OuterBoundaryMax - span2
            End If
            If (Min > InnerBoundaryMin) Then
                _Min = InnerBoundaryMin
            End If
            If (Max < InnerBoundaryMax) Then
                _Max = InnerBoundaryMax
            End If
        End Sub

        ''' <summary>
        ''' Set axis limits.
        ''' </summary>
        Public Sub SetAxis(min As Double?, max As Double?)
            If (Not LockedLimits) Then
                _HasBeenSet = True
                _Min = If(min, Me.Min)
                _Max = If(max, Me.Max)
                ApplyBounds()
            End If
        End Sub

        ''' <summary>
        ''' Shift min and max by the given number of units.
        ''' </summary>
        Public Sub Pan(units As Double)
            If (Not LockedLimits) Then
                _Min += units
                _Max += units
                ApplyBounds()
            End If
        End Sub

        ''' <summary>
        ''' Shift min and max by the given number of pixels.
        ''' </summary>
        Public Sub PanPx(pixels As Single)
            If (Not LockedLimits) Then
                Pan(pixels * UnitsPerPx)
            End If
        End Sub

        ''' <summary>
        ''' Zoom by simultaneously adjusting Min and Max
        ''' </summary>
        ''' <param name="frac">1 for no change, 2 zooms in, 0.5 zooms out.</param>
        ''' <param name="zoomTo">If given, zoom toward/from this alternative center point.</param>
        Public Sub Zoom(Optional frac As Double = 1, Optional zoomTo As Double? = Nothing)
            If Not LockedLimits Then
                Dim value As Double = zoomTo.GetValueOrDefault()
                If (zoomTo is Nothing) Then
                    value = Center
                    zoomTo = New Double?(value)
                End If
                Dim t = RationalLimits()
                _Min = t.Item1
                _Max = t.Item2
                Dim spanLeft As Double = zoomTo.Value - Min
                Dim spanRight As Double = Max - zoomTo.Value
                _Min = zoomTo.Value - spanLeft / frac
                _Max = zoomTo.Value + spanRight / frac
                ApplyBounds()
            End If
        End Sub

        ''' <summary>
        ''' Get the pixel location on the figure for a given position in axis units.
        ''' </summary>
        Public Function GetPixel(unit As Double) As Single
            Dim unitsFromMin As Double = If(IsInverted, Max - unit, unit - Min)
            Dim pxFromMin As Double = unitsFromMin * PxPerUnit
            Dim pixel As Double = DataOffsetPx + pxFromMin
            Return CSng(pixel)
        End Function

        ''' <summary>
        ''' Get the axis unit position for the given pixel location on the figure.
        ''' </summary>
        Public Function GetUnit(pixel As Single) As Double
            Dim pxFromMin As Double = If(IsInverted, DataSizePx + DataOffsetPx - pixel, pixel - DataOffsetPx)
            Return (pxFromMin * UnitsPerPx + Min)
        End Function

        ''' <summary>
        ''' Sets a flag indicating whether axis limits are mutable.
        ''' </summary>
        Public Sub LockLimits(Optional locked As Boolean = True)
            LockedLimits = locked
        End Sub

#End Region '/METHODS

    End Class

End Namespace