Namespace ScottPlot.Control.EventProcess.Events

	''' <summary>
	''' This event describes what happens when the mouse button is lifted after middle-click-dragging a rectangle to zoom into. 
	''' The coordinates of that rectangle are calculated, and the plot's axis limits are adjusted accordingly.
	''' </summary>
	Public Class ApplyZoomRectangleEvent
		Implements IUIEvent

		Private ReadOnly X As Single
		Private ReadOnly Y As Single
		Private ReadOnly Configuration As Configuration
		Private ReadOnly Settings As Settings
		Private ReadOnly Plot As Plot

		Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
			Get
				Return Configuration.QualityConfiguration.MouseInteractiveDropped
			End Get
		End Property

		Public Sub New(x As Single, y As Single, config As Configuration, settings As Settings, plt As Plot)
			Me.X = x
			Me.Y = y
			Me.Configuration = config
			Me.Settings = settings
			Me.Plot = plt
		End Sub

		Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
			Settings.RecallAxisLimits()
			Dim originalLimits As AxisLimits = Me.Plot.GetAxisLimits()

			Settings.MouseZoomRect(X, Y, True)

			If Configuration.LockHorizontalAxis Then
				Plot.SetAxisLimitsX(originalLimits.XMin, originalLimits.XMax)
			End If

			If Configuration.LockVerticalAxis Then
				Plot.SetAxisLimitsY(originalLimits.YMin, originalLimits.YMax)
			End If
		End Sub

	End Class

End Namespace