Namespace ScottPlot.Palettes

    Public Class Custom
        Inherits PaletteBase

        Public Sub New(hexColors As String(), Optional name As String = "", Optional description As String = "")
            If (hexColors is Nothing) Then
                Throw New ArgumentNullException("Must provide at least one color.")
            End If
            If (hexColors.Length = 0) Then
                Throw New ArgumentException("Must provide at least one color.")
            End If
            SetColors(PaletteBase.FromHexColors(hexColors))
            SetName(name)
            SetDescription(description)
        End Sub

    End Class

End Namespace