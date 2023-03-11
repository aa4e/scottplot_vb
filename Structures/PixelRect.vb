Namespace ScottPlot

    Public Class PixelRect

#Region "CTOR"

        Public X As Single
        Public Y As Single
        Public Width As Single
        Public Height As Single

        Public Sub New(x As Single, y As Single, width As Single, height As Single)
            Me.X = x
            Me.Y = y
            Me.Width = width
            Me.Height = height
        End Sub

#End Region '/CTOR

#Region "PROPS"

        Public ReadOnly Property TopLeft As New Pixel(X, Y)
        Public ReadOnly Property TopRight As New Pixel(X, Y + Width)
        Public ReadOnly Property BottomLeft As New Pixel(X, Y + Height)
        Public ReadOnly Property BottomRight As New Pixel(X + Width, Y + Height)

#End Region '/PROPS

    End Class

End Namespace