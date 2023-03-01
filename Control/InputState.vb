Namespace ScottPlot.Control

	Public Class InputState

		Public Property X As Single = Single.NaN
		Public Property Y As Single = Single.NaN

		Public Property LeftWasJustPressed As Boolean = False
		Public Property RightWasJustPressed As Boolean = False
		Public Property MiddleWasJustPressed As Boolean = False

		Public ReadOnly Property ButtonDown As Boolean
			Get
				Return (LeftWasJustPressed OrElse RightWasJustPressed OrElse MiddleWasJustPressed)
			End Get
		End Property

		Public Property ShiftDown As Boolean = False
		Public Property CtrlDown As Boolean = False
		Public Property AltDown As Boolean = False

		Public Property WheelScrolledUp As Boolean = False
		Public Property WheelScrolledDown As Boolean = False

	End Class

End Namespace