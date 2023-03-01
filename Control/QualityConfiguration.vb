Namespace ScottPlot.Control

	''' <summary>
	''' This class defines the quality to use for renders after different interactive events occur.
	''' Programmatically-triggered events typically use high quality mode (anti-aliasing enabled).
	''' Real-time mouse-interactive events like zooming and panning typically use low quality mode.
	''' It is possible to automatically render using high quality after a period of inactivity.
	''' </summary>
	Public Class QualityConfiguration

		Public BenchmarkToggle As RenderType = RenderType.HighQuality
		Public AutoAxis As RenderType = RenderType.LowQualityThenHighQualityDelayed
		Public MouseInteractiveDragged As RenderType
		Public MouseInteractiveDropped As RenderType = RenderType.HighQuality
		Public MouseWheelScrolled As RenderType = RenderType.LowQualityThenHighQualityDelayed

	End Class

End Namespace