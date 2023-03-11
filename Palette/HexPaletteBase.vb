Namespace ScottPlot.Palettes

    Public MustInherit Class HexPaletteBase
        Inherits PaletteBase

        Friend MustOverride ReadOnly Property HexColors As String()

        Public Sub New()
            MyBase.SetColors(PaletteBase.FromHexColors(HexColors))
        End Sub

    End Class

End Namespace