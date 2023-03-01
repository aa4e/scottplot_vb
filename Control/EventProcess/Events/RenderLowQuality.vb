Namespace ScottPlot.Control.EventProcess.Events

	Friend Class RenderLowQuality
		Implements IUIEvent

		Public ReadOnly Property RenderType As RenderType Implements ScottPlot.Control.EventProcess.IUIEvent.RenderType
			Get
				Return RenderType.LowQuality
			End Get
		End Property

		Public Sub ProcessEvent() Implements ScottPlot.Control.EventProcess.IUIEvent.ProcessEvent
		End Sub

	End Class

End Namespace