Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' The Crosshair plot type draws a vertical and horizontal line to label a point
    ''' on the plot and displays the coordinates of that point in labels that overlap
    ''' the axis tick labels. 
    ''' 
    ''' This plot type is typically used in combination with
    ''' MouseMove events to track the location of the mouse And/Or with plot types that
    ''' have GetPointNearest() methods.
    ''' </summary>
    Public Class Crosshair
        Implements IPlottable, IHasLine, IHasColor

#Region "CTOR"

        Public Sub New()
            LineStyle = LineStyle.Dash
            LineWidth = 1
            Color = Color.FromArgb(200, Color.Red)
            HorizontalLine.PositionLabel = True
            VerticalLine.PositionLabel = True
        End Sub

#End Region '/CTOR

#Region "PROPS, FIELDS"

        Public Property IsVisible As Boolean = False Implements ScottPlot.Plottable.IPlottable.IsVisible
        Public Property XAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.XAxisIndex
        Public Property YAxisIndex As Integer = 0 Implements ScottPlot.Plottable.IPlottable.YAxisIndex
        Public Property Label As String = String.Empty

        Public ReadOnly HorizontalLine As New HLine()
        Public ReadOnly VerticalLine As New VLine()

        ''' <summary>
        ''' X position (axis units) of the vertical line.
        ''' </summary>
        Public Property X As Double
            Get
                Return VerticalLine.X
            End Get
            Set(value As Double)
                VerticalLine.X = value
            End Set
        End Property

        ''' <summary>
        ''' X position (axis units) of the horizontal line.
        ''' </summary>
        Public Property Y As Double
            Get
                Return HorizontalLine.Y
            End Get
            Set(value As Double)
                HorizontalLine.Y = value
            End Set
        End Property

        ''' <summary>
        ''' Style for horizontal and vertical lines.
        ''' </summary>
        Public Property LineStyle As LineStyle Implements ScottPlot.Plottable.IHasLine.LineStyle
            Get
                Return HorizontalLine.LineStyle
            End Get
            Set(value As LineStyle)
                HorizontalLine.LineStyle = value
                VerticalLine.LineStyle = value
            End Set
        End Property

        ''' <summary>
        ''' Line width for vertical and horizontal lines.
        ''' </summary>
        Public Property LineWidth As Double Implements ScottPlot.Plottable.IHasLine.LineWidth
            Get
                Return HorizontalLine.LineWidth
            End Get
            Set(value As Double)
                HorizontalLine.LineWidth = value
                VerticalLine.LineWidth = value
            End Set
        End Property

        <Obsolete("Get HorizontalLine.PositionLabelFont and VerticalLine.PositionLabelFont instead.", True)>
        Public Property LabelFont As ScottPlot.Drawing.Font

        <Obsolete("Get HorizontalLine.PositionLabelBackground and VerticalLine.PositionLabelBackground instead.", True)>
        Public Property LabelBackgroundColor As Color

        <Obsolete("Get HorizontalLine.PositionLabel and VerticalLine.PositionLabel instead.", True)>
        Public Property PositionLabel As Boolean

        ''' <summary>
        ''' Color for horizontal and vertical lines and their position label backgrounds.
        ''' </summary>
        Public Property Color As Color Implements ScottPlot.Plottable.IHasColor.Color
            Get
                Return HorizontalLine.Color
            End Get
            Set(value As Color)
                HorizontalLine.Color = value
                VerticalLine.Color = value
                HorizontalLine.PositionLabelBackground = value
                VerticalLine.PositionLabelBackground = value
            End Set
        End Property

        Public Property LineColor As Color Implements ScottPlot.Plottable.IHasLine.LineColor
            Get
                Return Color
            End Get
            Set(value As Color)
                Color = value
            End Set
        End Property

        ''' <summary>
        ''' If true, AxisAuto() will ignore the position of this line when determining axis limits.
        ''' </summary>
        Public Property IgnoreAxisAuto As Boolean = False

        <Obsolete("Use VerticalLine.PositionFormatter()")>
        Public Property IsDateTimeX As Boolean
            Get
                Return _IsDateTimeX
            End Get
            Set(value As Boolean)
                _IsDateTimeX = value
                VerticalLine.PositionFormatter = If(value,
                    Function(position As Double) DateTime.FromOADate(position).ToString(_StringFormatX),
                    Function(position As Double) position.ToString(_StringFormatX))
            End Set
        End Property
        Private _IsDateTimeX As Boolean

        <Obsolete("Use VerticalLine.PositionFormatter()")>
        Public Property StringFormatX As String
            Get
                Return _StringFormatX
            End Get
            Set(value As String)
                _StringFormatX = value
                VerticalLine.PositionFormatter = If(IsDateTimeX,
                    Function(position As Double) DateTime.FromOADate(position).ToString(_StringFormatX),
                    Function(position As Double) position.ToString(_StringFormatX))
            End Set
        End Property
        Private _StringFormatX As String = "F2"

        <Obsolete("Use VerticalLine.IsVisible")>
        Public Property IsVisibleX As Boolean
            Get
                Return VerticalLine.IsVisible
            End Get
            Set(value As Boolean)
                VerticalLine.IsVisible = value
            End Set
        End Property

        <Obsolete("Use HorizontalLine.PositionFormatter()")>
        Public Property IsDateTimeY As Boolean
            Get
                Return _IsDateTimeY
            End Get
            Set(value As Boolean)
                _IsDateTimeY = value
                HorizontalLine.PositionFormatter = If(value,
                    Function(position As Double) DateTime.FromOADate(position).ToString(_StringFormatY),
                    Function(position As Double) position.ToString(_StringFormatY))
            End Set
        End Property
        Private _IsDateTimeY As Boolean

        <Obsolete("Use HorizontalLine.PositionFormat()")>
        Public Property StringFormatY As String
            Get
                Return _StringFormatY
            End Get
            Set(value As String)
                _StringFormatY = value
                HorizontalLine.PositionFormatter = If(IsDateTimeY,
                    Function(position As Double) DateTime.FromOADate(position).ToString(_StringFormatY),
                    Function(position As Double) position.ToString(_StringFormatY))
            End Set
        End Property
        Private _StringFormatY As String = "F2"

        <Obsolete("Use HorizontalLine.IsVisible")>
        Public Property IsVisibleY As Boolean
            Get
                Return HorizontalLine.IsVisible
            End Get
            Set(value As Boolean)
                HorizontalLine.IsVisible = value
            End Set
        End Property

#End Region '/PROPS, FIELDS

#Region "METHODS"

        Public Function GetAxisLimits() As AxisLimits Implements ScottPlot.Plottable.IPlottable.GetAxisLimits
            Return If(IgnoreAxisAuto, AxisLimits.NoLimits, New AxisLimits(X, X, Y, Y))
        End Function

        Public Function GetLegendItems() As LegendItem() Implements ScottPlot.Plottable.IPlottable.GetLegendItems
            Dim leg As New LegendItem(Me) With {
                .Label = Label,
                .Color = Color,
                .LineStyle = LineStyle,
                .LineWidth = LineWidth,
                .MarkerShape = MarkerShape.None}
            Return New LegendItem() {leg}
        End Function

        Public Sub ValidateData(Optional deep As Boolean = False) Implements ScottPlot.Plottable.IPlottable.ValidateData
        End Sub

        Public Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False) Implements ScottPlot.Plottable.IPlottable.Render
            If IsVisible Then
                HorizontalLine.Render(dims, bmp, lowQuality)
                VerticalLine.Render(dims, bmp, lowQuality)
            End If
        End Sub

#End Region '/METHODS

    End Class

End Namespace