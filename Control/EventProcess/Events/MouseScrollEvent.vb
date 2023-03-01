Namespace ScottPlot.Control.EventProcess.Events

	''' <summary>
	''' This event describes a zoom operation performed by scrolling the mouse wheel.
	''' </summary>
	Public Class MouseScrollEvent
		Implements IUIEvent

		Private ReadOnly X As Single
		Private ReadOnly Y As Single
		Private ReadOnly ScrolledUp As Boolean
		Private ReadOnly Configuration As Configuration
		Private ReadOnly Settings As Settings

		Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
			Get
				Return Configuration.QualityConfiguration.MouseWheelScrolled
			End Get
		End Property

		Public Sub New(x As Single, y As Single, scrolledUp As Boolean, config As Configuration, settings As Settings)
			Me.X = x
			Me.Y = y
			Me.ScrolledUp = scrolledUp
			Me.Configuration = config
			Me.Settings = settings
		End Sub

		Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
			Dim increment As Double = 1 + Configuration.ScrollWheelZoomFraction
			Dim decrement As Double = 1 - Configuration.ScrollWheelZoomFraction
			Dim xFrac As Double = If(ScrolledUp, increment, decrement)
			Dim yFrac As Double = If(ScrolledUp, increment, decrement)
			If Configuration.LockHorizontalAxis Then
				xFrac = 1
			End If

			If Configuration.LockVerticalAxis Then
				yFrac = 1
			End If
			Settings.AxesZoomTo(xFrac, yFrac, X, Y)
		End Sub

	End Class

End Namespace