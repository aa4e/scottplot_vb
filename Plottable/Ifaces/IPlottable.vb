Imports System.Drawing

Namespace ScottPlot.Plottable

    ''' <summary>
    ''' Every plottable object must implement this interface. Additional features are provided by adjacent interfaces.
    ''' </summary>
    Public Interface IPlottable

        ''' <summary>
        ''' Controls whether the plot will be rendered and contribute to automatic axis limit detection.
        ''' </summary>
        Property IsVisible As Boolean

        ''' <summary>
        ''' Index of the horizontal axis this plottable will use for coordinate/pixel conversions.
        ''' 0 is the bottom axis, 1 is the top axis, and higher numbers are additional custom axes.
        ''' </summary>
        Property XAxisIndex As Integer

        ''' <summary>
        ''' Index of the vertical axis this plottable will use for coordinate/pixel conversions.
        ''' 0 is the left axis, 1 is the right axis, and higher numbers are additional custom axes.
        ''' </summary>
        Property YAxisIndex As Integer

        ''' <summary>
        ''' This is called when it is time to draw the plottable on the canvas.
        ''' </summary>
        ''' <param name="dims">Spatial information about the plot and all axes to assist with coordinate/pixel conversions.</param>
        ''' <param name="bmp">The image on which this plottable will be drawn.</param>
        ''' <param name="lowQuality">If true, disable anti-aliased lines and text to achieve faster rendering.</param>
        Sub Render(dims As PlotDimensions, bmp As Bitmap, Optional lowQuality As Boolean = False)

        ''' <summary>
        ''' Returns the limits of the data contained in a plottable. If an axis has no data its min and max may be Double.NaN.
        ''' </summary>
        Function GetAxisLimits() As AxisLimits

        ''' <summary>
        ''' Returns items to show in the legend. Most plottables return a single item. in this array will appear in the legend.
        ''' Plottables which never appear in the legend should return an empty array (not null).
        ''' </summary>
        Function GetLegendItems() As LegendItem()

        ''' <summary>
        ''' Throw InvalidOperationException if ciritical variables are null or have incorrect sizes. 
        ''' Deep validation is slower but also checks every value for NaN and Infinity.
        ''' </summary>
        Sub ValidateData(Optional deep As Boolean = False)

    End Interface

End Namespace