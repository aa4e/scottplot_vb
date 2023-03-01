Imports System.Drawing

Namespace ScottPlot.Drawing

    Public Class Font

        Public Size As Single = 12
        Public Color As Color = Color.Black
        Public Alignment As Alignment = Alignment.UpperLeft
        Public Bold As Boolean = False
        Public Rotation As Single = 0
        Public Property Family As FontFamily = InstalledFont.SansFamily

        Public Property Name As String
            Get
                Return Family.Name
            End Get
            Set(value As String)
                Family = InstalledFont.ValidFontFamily(value) 'ensure only valid font names can be assigned
            End Set
        End Property

    End Class

End Namespace