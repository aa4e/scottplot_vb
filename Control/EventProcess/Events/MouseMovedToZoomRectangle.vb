Namespace ScottPlot.Control.EventProcess.Events

	''' <summary>
	''' This event occurs when the user is actively middle-click-dragging to zoom.
	''' A zoom window is drawn on the screen, but axis limits have Not yet been changed.
	''' </summary>
	Public Class MouseMovedToZoomRectangle
		Implements IUIEvent

		Private ReadOnly X As Single
		Private ReadOnly Y As Single
		Private ReadOnly Settings As Settings
		Private ReadOnly Configuration As Configuration

		Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
			Get
				Return Configuration.QualityConfiguration.MouseInteractiveDragged
			End Get
		End Property

		Public Sub New(x As Single, y As Single, settings As Settings, configuration As Configuration)
			Me.X = x
			Me.Y = y
			Me.Settings = settings
			Me.Configuration = configuration
		End Sub

		Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
			Settings.MouseZoomRect(X, Y)
		End Sub

	End Class

End Namespace