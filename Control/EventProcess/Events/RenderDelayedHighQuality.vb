Namespace ScottPlot.Control.EventProcess.Events

	Friend Class RenderDelayedHighQuality
		Implements IUIEvent

		Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
			Get
				Return RenderType.LowQualityThenHighQualityDelayed
			End Get
		End Property

		Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
		End Sub

	End Class

End Namespace