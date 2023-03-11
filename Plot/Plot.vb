Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot

    Partial Public Class Plot

#Region "PROPS, FIELDS"

        ''' <summary>
        ''' The settings object stores all state (configuration and data) for a plot.
        ''' </summary>
        Private ReadOnly Settings As New Settings()

        ''' <summary>
        ''' Plot image width (pixels).
        ''' </summary>
        Public Property Width As Single
            Get
                Return CSng(Settings.Width)
            End Get
            Set(value As Single)
                Resize(value, CSng(Settings.Height))
            End Set
        End Property

        ''' <summary>
        ''' Plot image height (pixels).
        ''' </summary>
        Public Property Height As Single
            Get
                Return CSng(Settings.Height)
            End Get
            Set(value As Single)
                Resize(CSng(Settings.Width), value)
            End Set
        End Property

        ''' <summary>
        ''' Version in the format "1.2.3" (or "1.2.3-beta" for pre-releases).
        ''' </summary>
        Public Shared ReadOnly Property Version As String
            Get
                Dim v As Version = GetType(Plot).Assembly.GetName().Version
                Return $"{v.Major}.{v.Minor}.{v.Build}"
            End Get
        End Property

        ''' <summary>
        ''' The palette defines default colors to use for new plottables.
        ''' </summary>
        Public Property Palette As IPalette
            Get
                Return Settings.PlottablePalette
            End Get
            Set(value As IPalette)
                If (value is Nothing) Then
                    Throw New ArgumentNullException()
                End If
                Settings.PlottablePalette = value
            End Set
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        ''' <summary>
        ''' A ScottPlot stores data in plottable objects and draws it on a bitmap when <see cref="Render()"/> is called.
        ''' </summary>
        ''' <param name="width">Default width (pixels) to use when rendering.</param>
        ''' <param name="height">Default height (pixels) to use when rendering.</param>
        Public Sub New(Optional width As Integer = 800, Optional height As Integer = 600)
            If (width <= 0) OrElse (height <= 0) Then
                Throw New ArgumentException("width and height must each be greater than 0.")
            End If
            Style(ScottPlot.Style.Default)
            Resize(width, height)
        End Sub

#End Region '/CTOR

#Region "ADD, CLEAR, AND REMOVE PLOTTABLES"

        ''' <summary>
        ''' Add a plottable to the plot.
        ''' </summary>
        ''' <param name="plottable">A plottable the user created.</param>
        Public Sub Add(plottable As Plottable.IPlottable)
            Settings.Plottables.Add(plottable)
        End Sub

        ''' <summary>
        ''' Clear all plottables.
        ''' </summary>
        Public Sub Clear()
            Settings.Plottables.Clear()
            Settings.ResetAxisLimits()
        End Sub

        ''' <summary>
        ''' Remove all plottables of the given type.
        ''' </summary>
        ''' <param name="plottableType">All plottables of this type will be removed.</param>
        Public Sub Clear(plottableType As Type)
            Dim plottablesWithSameType As List(Of Plottable.IPlottable) = Settings.Plottables.Where(Function(x As Plottable.IPlottable)
                                                                                                        Return x.GetType() is plottableType
                                                                                                    End Function).ToList()
            While (plottablesWithSameType.Count > 0)
                Settings.Plottables.Remove(plottablesWithSameType(0))
                plottablesWithSameType.RemoveAt(0)
            End While

            If (Settings.Plottables.Count = 0) Then
                Settings.ResetAxisLimits()
            End If
        End Sub

        ''' <summary>
        ''' Remove a specific plottable.
        ''' </summary>
        ''' <param name="plottable">The plottable to remove.</param>
        Public Sub Remove(plottable As Plottable.IPlottable)
            Settings.Plottables.Remove(plottable)
            If (Settings.Plottables.Count = 0) Then
                Settings.ResetAxisLimits()
            End If
        End Sub

        ''' <summary>
        ''' Remove the plottable at the specified index of the list.
        ''' </summary>
        ''' <param name="index">The zero-based index of the element to remove.</param>
        Public Sub RemoveAt(index As Integer)
            Settings.Plottables.RemoveAt(index)
            If (Settings.Plottables.Count = 0) Then
                Settings.ResetAxisLimits()
            End If
        End Sub

        ''' <summary>
        ''' Move a plottable in the list. Plottables near the end are rendered last (on top).
        ''' </summary>
        Public Sub Move(oldIndex As Integer, newIndex As Integer)
            Settings.Plottables.Move(oldIndex, newIndex)
        End Sub

        ''' <summary>
        ''' Move a plottable to the front so it is rendered first and appears beneath all others.
        ''' </summary>
        Public Sub MoveFirst(plottable As Plottable.IPlottable)
            Settings.Plottables.Remove(plottable)
            Settings.Plottables.Insert(0, plottable)
        End Sub

        ''' <summary>
        ''' Move a plottable to the end so it is rendered last and appears above all others.
        ''' </summary>
        Public Sub MoveLast(plottable As Plottable.IPlottable)
            Settings.Plottables.Remove(plottable)
            Settings.Plottables.Add(plottable)
        End Sub

        ''' <summary>
        ''' Return a copy of the list of plottables.
        ''' </summary>
        ''' <returns>List of plottables.</returns>
        Public Function GetPlottables() As Plottable.IPlottable()
            Return Settings.Plottables.ToArray()
        End Function

        ''' <summary>
        ''' Return the draggable plottable under the mouse cursor (or null if there isn't one).
        ''' </summary>
        Public Function GetDraggable(xPixel As Double, yPixel As Double, Optional snapDistancePixels As Integer = 5) As Plottable.IDraggable
            Dim enabledDraggables As Plottable.IDraggable() = Settings.Plottables _
                .Where(Function(x) x is GetType(Plottable.IDraggable)) _
                .Select(Function(x) CType(x, Plottable.IDraggable)) _
                .Where(Function(x As Plottable.IDraggable) x.DragEnabled) _
                .Where(Function(x As Plottable.IDraggable)
                           Return x is GetType(Plottable.IPlottable) AndAlso CType(x, Plottable.IPlottable).IsVisible
                       End Function).ToArray()

            For Each draggable As Plottable.IDraggable In enabledDraggables
                Dim xAxisIndex As Integer = CType(draggable, Plottable.IPlottable).XAxisIndex
                Dim yAxisIndex As Integer = CType(draggable, Plottable.IPlottable).YAxisIndex
                Dim xUnitsPerPx As Double = Settings.GetXAxis(xAxisIndex).Dims.UnitsPerPx
                Dim yUnitsPerPx As Double = Settings.GetYAxis(yAxisIndex).Dims.UnitsPerPx

                Dim snapWidth As Double = xUnitsPerPx * snapDistancePixels
                Dim snapHeight As Double = yUnitsPerPx * snapDistancePixels
                Dim xCoords As Double = GetCoordinateX(CSng(xPixel), xAxisIndex)
                Dim yCoords As Double = GetCoordinateY(CSng(yPixel), yAxisIndex)
                If draggable.IsUnderMouse(xCoords, yCoords, snapWidth, snapHeight) Then
                    Return draggable
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' Return the highest hittable plottable at the given point (or null if no hit).
        ''' </summary>
        Public Function GetHittable(xPixel As Double, yPixel As Double) As Plottable.IPlottable
            For Each p In GetPlottables().Where(Function(x) x is GetType(Plottable.IHittable)).Reverse()
                Dim xAxisIndex As Integer = p.XAxisIndex
                Dim yAxisIndex As Integer = p.YAxisIndex

                Dim xCoords As Double = GetCoordinateX(CSng(xPixel), xAxisIndex)
                Dim yCoords As Double = GetCoordinateY(CSng(yPixel), yAxisIndex)
                Dim c As New Coordinate(xCoords, yCoords)

                Dim h As Plottable.IHittable = CType(p, Plottable.IHittable)
                If h.HitTest(c) Then
                    Return p
                End If
            Next
            Return Nothing
        End Function

#End Region '/ADD, CLEAR, AND REMOVE PLOTTABLES

#Region "PLOTTABLE VALIDATION"

        ''' <summary>
        ''' Throw an exception if any plottable contains an invalid state.
        ''' </summary>
        ''' <param name="deep">Check every individual value for validity. This is more thorough, but slower.</param>
        Public Sub Validate(Optional deep As Boolean = True)
            For Each plottable As Plottable.IPlottable In Settings.Plottables
                plottable.ValidateData(deep)
            Next
        End Sub

#End Region '/PLOTTABLE VALIDATION

#Region "PLOT SETTINGS AND STYLING"

        ''' <summary>
        ''' The Settings module stores manages plot state and advanced configuration.
        ''' Its class structure changes frequently, and users are highly advised Not to interact with it directly.
        ''' This method returns the settings module for advanced users and developers to interact with.
        ''' </summary>
        ''' <param name="showWarning">Show a warning message indicating this method is only intended for developers.</param>
        ''' <returns>Settings used by the plot.</returns>
        Public Function GetSettings(Optional showWarning As Boolean = True) As Settings
            If (showWarning) Then
                Diagnostics.Debug.WriteLine("WARNING: GetSettings() is only for development and testing. Be aware its class structure changes frequently!")
            End If
            Return Settings
        End Function

        ''' <summary>
        ''' Update the default size for new renders.
        ''' </summary>
        ''' <param name="width">Width (pixels) for future renders.</param>
        ''' <param name="height">Height (pixels) for future renders.</param>
        Public Sub Resize(width As Single, height As Single)
            Settings.Resize(width, height)
        End Sub

        ''' <summary>
        ''' Return a new color from the <see cref="Palette"/> based on the number of plottables already in the plot.
        ''' Use this to ensure every plottable gets a unique color.
        ''' </summary>
        ''' <param name="alpha">Value from 0 (transparent) to 1 (opaque).</param>
        ''' <returns>New color.</returns>
        Public Function GetNextColor(Optional alpha As Double = 1.0) As Color
            Return Color.FromArgb(CInt(alpha * 255), Settings.GetNextColor())
        End Function

        ''' <summary>
        ''' Set the colors and fonts of many plot components at once using a predefined theme.
        ''' </summary>
        Public Sub Style(style As Styles.IStyle)
            If (style is Nothing) Then
                Throw New ArgumentNullException(NameOf(style))
            End If

            Settings.FigureBackground.Color = style.FigureBackgroundColor
            Settings.DataBackground.Color = style.DataBackgroundColor

            For Each ax As Renderable.Axis In Settings.Axes
                ax.LabelStyle(color:=style.AxisLabelColor, fontName:=style.AxisLabelFontName)
                ax.TickLabelStyle(color:=style.TickLabelColor, fontName:=style.TickLabelFontName)
                ax.MajorGrid(color:=style.GridLineColor)
                ax.MinorGrid(color:=style.GridLineColor)
                ax.TickMarkColor(majorColor:=style.TickMajorColor, minorColor:=style.TickMinorColor)
                ax.Line(color:=style.FrameColor)
            Next

            XAxis2.LabelStyle(color:=style.TitleFontColor, fontName:=style.TitleFontName)

            For Each p As Plottable.IStylable In Settings.Plottables.Where(Function(x) x is GetType(Plottable.IStylable))
                p.SetStyle(style.TickMajorColor, style.TickLabelColor)
            Next
        End Sub

        ''' <summary>
        ''' Set the color of specific plot components.
        ''' </summary>
        ''' <param name="figureBackground">Color for area beneath the axis ticks and labels and around the data area.</param>
        ''' <param name="dataBackground">Color for area inside the data frame but beneath the grid and plottables.</param>
        ''' <param name="grid">Color for grid lines.</param>
        ''' <param name="tick">Color for axis tick marks and frame lines.</param>
        ''' <param name="axisLabel">Color for axis labels and tick labels.</param>
        ''' <param name="titleLabel">Color for the top axis label (XAxis2's title).</param>
        ''' <param name="dataBackgroundImage">Bitmap to display behind the data area.</param>
        ''' <param name="figureBackgroundImage">Bitmap to display behind the entire figure.</param>
        Public Sub Style(Optional figureBackground As Color? = Nothing,
                         Optional dataBackground As Color? = Nothing,
                         Optional grid As Color? = Nothing,
                         Optional tick As Color? = Nothing,
                         Optional axisLabel As Color? = Nothing,
                         Optional titleLabel As Color? = Nothing,
                         Optional dataBackgroundImage As Bitmap = Nothing,
                         Optional figureBackgroundImage As Bitmap = Nothing)

            Settings.FigureBackground.Color = If(figureBackground, Settings.FigureBackground.Color)
            Settings.DataBackground.Color = If(dataBackground, Settings.DataBackground.Color)

            Settings.FigureBackground.Bitmap = If(figureBackgroundImage, Settings.FigureBackground.Bitmap)
            Settings.DataBackground.Bitmap = If(dataBackgroundImage, Settings.DataBackground.Bitmap)

            For Each axis As Renderable.Axis In Settings.Axes
                axis.Label(color:=axisLabel)
                axis.TickLabelStyle(color:=tick)
                axis.MajorGrid(color:=grid)
                axis.MinorGrid(color:=grid)
                If tick.HasValue Then
                    axis.TickMarkColor(tick.Value)
                End If
                axis.Line(color:=tick)
            Next

            XAxis2.Label(color:=titleLabel)

            For Each p As Plottable.IPlottable In Settings.Plottables.Where(Function(x) x is GetType(Plottable.IStylable))
                CType(p, Plottable.IStylable).SetStyle(tick, axisLabel)
            Next
        End Sub

        ''' <summary>
        ''' Reset axis padding to the default values for all axes.
        ''' </summary>
        Public Sub ResetLayout()
            For Each axis As Renderable.Axis In Settings.Axes
                axis.ResetLayout()
            Next
        End Sub

#End Region '/PLOT SETTINGS AND STYLING

#Region "RENDERABLE CUSTOMIZATION"

        ''' <summary>
        ''' If enabled, the benchmark displays render information in the corner of the plot.
        ''' </summary>
        ''' <param name="enable">True/false defines whether benchmark is enabled. Null will not change the benchmark.</param>
        ''' <returns>True if the benchmark is enabled.</returns>
        Public Function Benchmark(Optional enable As Boolean? = True) As Boolean
            If enable.HasValue Then
                Settings.BenchmarkMessage.IsVisible = enable.Value
            End If
            Return Settings.BenchmarkMessage.IsVisible
        End Function

        ''' <summary>
        ''' Return an array of times for the last several renders.
        ''' The last element of the array is the most recently rendered frame time.
        ''' </summary>
        Public Function BenchmarkTimes() As Double()
            Return Settings.BenchmarkMessage.GetRenderTimes()
        End Function

        ''' <summary>
        ''' Configure legend visibility and location. 
        ''' Optionally you can further customize the legend by interacting with the object it returns.
        ''' </summary>
        ''' <param name="enable">Whether or not the legend is visible (or null for no change).</param>
        ''' <param name="location">Position of the legend relative to the data area.</param>
        ''' <returns>The legend itself. Use public fields to further customize its appearance and behavior.</returns>
        Public Function Legend(Optional enable As Boolean? = True, Optional location As Alignment? = Alignment.LowerRight) As Renderable.Legend
            If enable.HasValue Then
                Settings.CornerLegend.IsVisible = enable.Value
                Settings.CornerLegend.Location = If(location, Settings.CornerLegend.Location)
            End If
            Return Settings.CornerLegend
        End Function

#End Region '/RENDERABLE CUSTOMIZATION

#Region "COPY AND EQUALS"

        ''' <summary>
        ''' The GUID helps identify individual plots.
        ''' </summary>
        Private ReadOnly Guid As Guid = Guid.NewGuid()

        ''' <summary>
        ''' Return a new Plot with all the same Plottables (and some of the styles) of this one.
        ''' </summary>
        ''' <returns>A new plot similar to this one.</returns>
        Public Function Copy() As Plot
            Dim oldSettings As Settings = Settings
            Dim oldPlot As Plot = Me

            Dim newPlot As New Plot(oldSettings.Width, oldSettings.Height)
            For Each plottable As Plottable.IPlottable In oldPlot.GetPlottables()
                newPlot.Add(plottable)
            Next
            newPlot.AxisAuto(Nothing, Nothing)

            newPlot.XLabel(oldSettings.XAxis.Label())
            newPlot.YLabel(oldSettings.YAxis.Label())
            newPlot.Title(oldSettings.XAxis2.Label())

            Return newPlot
        End Function


        ''' <summary>
        ''' Every plot has a globally unique ID (GUID) that can help differentiate it from other plots.
        ''' </summary>
        ''' <returns>A string representing the GUID.</returns>
        Public Function GetGuid() As String
            Return Guid.ToString()
        End Function

        ''' <summary>
        ''' Returns true if the given plot is the exact same plot as this one.
        ''' </summary>
        ''' <param name="obj">The plot to compare this one to.</param>
        ''' <returns>True if the two plots have the same GUID.</returns>
        Public Overrides Function Equals(obj As Object) As Boolean
            Return (obj.GetHashCode() = GetHashCode())
        End Function

        ''' <summary>
        ''' Returns an integer unique to this instance (based on the GUID).
        ''' </summary>
        ''' <returns>An integer representing the GUID.</returns>
        Public Overrides Function GetHashCode() As Integer
            Return Guid.GetHashCode()
        End Function

#End Region '/COPY AND EQUALS

#Region "METHODS"

        ''' <summary>
        ''' Brief description of this plot.
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"ScottPlot ({Settings.Width}x{Settings.Height}) with {Settings.Plottables.Count:n0} plottables."
        End Function

#End Region '/METHODS

    End Class

End Namespace