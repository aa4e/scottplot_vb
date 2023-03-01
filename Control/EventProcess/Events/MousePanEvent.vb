Namespace ScottPlot.Control.EventProcess.Events

	''' <summary>
	''' This event describes represents interactive panning.
	''' It is assume the plot has already been reset to the pre-mouse-interaction state,
	''' and processing of this event pans the plot on the axes according to the distance the mouse has moved.
	''' This is typically called on MouseMove events when the left button is held down.
	''' </summary>
	Public Class MousePanEvent
		Implements IUIEvent

		Private ReadOnly Input As InputState
		Private ReadOnly Configuration As Configuration
		Private ReadOnly Settings As Settings

		Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
			Get
				Return Configuration.QualityConfiguration.MouseInteractiveDragged
			End Get
		End Property

		Public Sub New(input As InputState, config As Configuration, settings As Settings)
			Me.Input = input
			Me.Configuration = config
			Me.Settings = settings
		End Sub

		Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
			Dim x As Single = If(Input.ShiftDown OrElse Configuration.LockHorizontalAxis, Settings.MouseDownX, Input.X)
			Dim y As Single = If(Input.CtrlDown OrElse Configuration.LockVerticalAxis, Settings.MouseDownY, Input.Y)
			Settings.MousePan(x, y)
		End Sub

	End Class

End Namespace