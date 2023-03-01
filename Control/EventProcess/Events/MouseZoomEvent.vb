Namespace ScottPlot.Control.EventProcess.Events

	''' <summary>
	''' This event describes represents interactive zooming.
	''' It is assume the plot has already been reset to the pre-mouse-interaction state,
	''' and processing of this event zooms the plot on the axes according to the distance the mouse has moved.
	''' This is typically called on MouseMove events when the right button is held down.
	''' </summary>
	Public Class MouseZoomEvent
		Implements IUIEvent

		Private ReadOnly Input As InputState
		Private ReadOnly Configuration As Configuration
		Private ReadOnly Settings As Settings
		Private ReadOnly Plot As Plot

		Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
			Get
				Return Configuration.QualityConfiguration.MouseInteractiveDragged
			End Get
		End Property

		Public Sub New(input As InputState, config As Configuration, settings As Settings, plt As Plot)
			Me.Input = input
			Me.Configuration = config
			Me.Settings = settings
			Me.Plot = plt
		End Sub

		Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
			Dim originalLimits As AxisLimits = Plot.GetAxisLimits()

			If Input.ShiftDown AndAlso Input.CtrlDown Then
				Dim dx As Single = Input.X - Settings.MouseDownX
				Dim dy As Single = Settings.MouseDownY - Input.Y
				Dim delta As Single = Math.Max(dx, dy)
				Settings.MouseZoom(Settings.MouseDownX + delta, Settings.MouseDownY - delta)
			Else
				Dim x As Single = If(Input.ShiftDown, Settings.MouseDownX, Input.X)
				Dim y As Single = If(Input.CtrlDown, Settings.MouseDownY, Input.Y)
				Settings.MouseZoom(x, y)
			End If

			If Configuration.LockHorizontalAxis Then
				Plot.SetAxisLimitsX(originalLimits.XMin, originalLimits.XMax)
			End If
			If Configuration.LockVerticalAxis Then
				Plot.SetAxisLimitsY(originalLimits.YMin, originalLimits.YMax)
			End If
		End Sub

	End Class

End Namespace