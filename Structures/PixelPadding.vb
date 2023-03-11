Namespace ScottPlot

    Public Structure PixelPadding

        Public Left As Single
        Public Right As Single
        Public Bottom As Single
        Public Top As Single

        Public Sub New(left As Single, right As Single, bottom As Single, top As Single)
            Me.Left = left
            Me.Right = right
            Me.Bottom = bottom
            Me.Top = top
        End Sub

    End Structure

End Namespace