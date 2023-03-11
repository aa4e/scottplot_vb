Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Palettes

    Public MustInherit Class PaletteBase
        Implements IPalette

#Region "PROPS"

        Public Overridable ReadOnly Property Name As String Implements ScottPlot.IPalette.Name
        Public Overridable ReadOnly Property Description As String Implements ScottPlot.IPalette.Description
        Public Overridable ReadOnly Property Colors As Color() Implements ScottPlot.IPalette.Colors

        Public ReadOnly Property Length As Integer
            Get
                Return Colors.Length
            End Get
        End Property

#End Region '/PROPS

#Region "METHODS"

        Protected Sub SetColors(newColors As Color())
            Me._Colors = newColors
        End Sub

        Protected Sub SetName(newName As String)
            Me._Name = newName
        End Sub

        Protected Sub SetDescription(newDescription As String)
            Me._Description = newDescription
        End Sub

        Friend Shared Function FromHexColors(hexColors As String()) As Color()
            Return hexColors.Select(Function(x) ParseHex(x)).ToArray()
        End Function

        Public Function Count() As Integer Implements ScottPlot.IPalette.Count
            Return Colors.Length
        End Function

        Public Function GetColor(index As Integer) As Color Implements ScottPlot.IPalette.GetColor
            Return Colors(index Mod Colors.Length)
        End Function

        Public Function GetColor(index As Integer, Optional alpha As Double = 1.0) As Color Implements ScottPlot.IPalette.GetColor
            Return Color.FromArgb(CInt(alpha * 255), GetColor(index))
        End Function

        Public Function GetColors(count As Integer, Optional offset As Integer = 0, Optional alpha As Double = 1.0) As Color() Implements ScottPlot.IPalette.GetColors
            Return Enumerable.Range(offset, count).Select(Function(x) GetColor(x, alpha)).ToArray()
        End Function

        Public Function GetRGB(index As Integer) As Tuple(Of Byte, Byte, Byte) Implements ScottPlot.IPalette.GetRGB
            Dim c As Color = GetColor(index)
            Return New Tuple(Of Byte, Byte, Byte)(c.R, c.G, c.B)
        End Function

        Private Shared Function ParseHex(hexColor As String) As Color
            If (Not hexColor.StartsWith("#")) Then
                hexColor = "#" & hexColor
            End If
            If (hexColor.Length <> 7) Then
                Throw New InvalidOperationException($"Invalid hex color: {hexColor}.")
            End If
            Return ColorTranslator.FromHtml(hexColor)
        End Function

        Private Function GetEnumerator() As IEnumerator(Of Color) Implements IEnumerable(Of Color).GetEnumerator
            Return CType(Colors, IEnumerable(Of Color)).GetEnumerator()
        End Function

        Function IEnumerable_GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return Colors.GetEnumerator()
        End Function

#End Region '/METHODS

    End Class

End Namespace