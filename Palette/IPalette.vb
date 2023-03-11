Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing

Namespace ScottPlot

    ''' <summary>
    ''' A palette is a collection of colors.
    ''' </summary>
    Public Interface IPalette
        Inherits IEnumerable(Of Color), IEnumerable

        ReadOnly Property Name As String
        ReadOnly Property Description As String
        ReadOnly Property Colors As Color()

        ''' <summary>
        ''' Get the color at the specified index (with rollover).
        ''' </summary>
        Function GetColor(index As Integer) As Color

        ''' <summary>
        ''' Get the color at the specified index (with rollover) with alpha (0 = transparent, 1 = opaque).
        ''' </summary>
        Function GetColor(index As Integer, Optional alpha As Double = 1) As Color

        ''' <summary>
        ''' Get the first several colors.
        ''' </summary>
        Function GetColors(count As Integer, Optional offset As Integer = 0, Optional alpha As Double = 1) As Color()

        ''' <summary>
        ''' Get the color at the specified index (with rollover).
        ''' </summary>
        <Obsolete("Stop using this and/or add RGBA.")>
        Function GetRGB(index As Integer) As Tuple(Of Byte, Byte, Byte)

        ''' <summary>
        ''' Returns the total number of colors in this palette.
        ''' </summary>
        Function Count() As Integer

    End Interface

End Namespace