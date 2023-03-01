Namespace ScottPlot.Control.EventProcess.Events

	''' <summary>
	''' This event calls <see cref="Plot.AxisAuto"/> on all axes. This is typically called after middle-clicking.
	''' </summary>
	Public Class MouseAxisAutoEvent
		Implements IUIEvent

		Private ReadOnly Configuration As Configuration
		Private ReadOnly Settings As Settings
		Private ReadOnly Plot As Plot

		Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
			Get
				Return Configuration.QualityConfiguration.AutoAxis
			End Get
		End Property

		Public Sub New(config As Configuration, settings As Settings, plt As Plot)
			Me.Configuration = config
			Me.Settings = settings
			Me.Plot = plt
		End Sub

		Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
			Settings.ZoomRectangle.Clear()
			If Configuration.LockVerticalAxis Then
				Return
			End If
			Plot.AxisAuto()
		End Sub

	End Class

End Namespace