Imports System.Drawing
Imports System.Linq

Namespace ScottPlot.Drawing

    Public Class Colormap

#Region "CTOR"

        Private ReadOnly ThisColormap As IColormap
        Private Shared ReadOnly ColormapFactory As New ColormapFactory()

        Public Sub New()
            ThisColormap = ColormapFactory.GetDefaultColormap()
        End Sub

        Public Sub New(colormap As IColormap)
            ThisColormap = If(colormap, ColormapFactory.GetDefaultColormap())
        End Sub

#End Region '/CTOR

#Region "PROPS"

        Public Shared ReadOnly Property Algae As New Colormap(New Colormaps.Algae())
        Public Shared ReadOnly Property Amp As New Colormap(New Colormaps.Amp())
        Public Shared ReadOnly Property Balance As New Colormap(New Colormaps.Balance())
        Public Shared ReadOnly Property Blues As New Colormap(New Colormaps.Blues())
        Public Shared ReadOnly Property Curl As New Colormap(New Colormaps.Curl())
        Public Shared ReadOnly Property Deep As New Colormap(New Colormaps.Deep())
        Public Shared ReadOnly Property Delta As New Colormap(New Colormaps.Delta())
        Public Shared ReadOnly Property Dense As New Colormap(New Colormaps.Dense())
        Public Shared ReadOnly Property Diff As New Colormap(New Colormaps.Diff())
        Public Shared ReadOnly Property Grayscale As New Colormap(New Colormaps.Grayscale())
        Public Shared ReadOnly Property GrayscaleR As New Colormap(New Colormaps.GrayscaleR())
        Public Shared ReadOnly Property Greens As New Colormap(New Colormaps.Greens())
        Public Shared ReadOnly Property Haline As New Colormap(New Colormaps.Haline())
        Public Shared ReadOnly Property Ice As New Colormap(New Colormaps.Ice())
        Public Shared ReadOnly Property Inferno As New Colormap(New Colormaps.Inferno())
        Public Shared ReadOnly Property Jet As New Colormap(New Colormaps.Jet())
        Public Shared ReadOnly Property Magma As New Colormap(New Colormaps.Magma())
        Public Shared ReadOnly Property Matter As New Colormap(New Colormaps.Matter())
        Public Shared ReadOnly Property Oxy As New Colormap(New Colormaps.Oxy())
        Public Shared ReadOnly Property Phase As New Colormap(New Colormaps.Phase())
        Public Shared ReadOnly Property Plasma As New Colormap(New Colormaps.Plasma())
        Public Shared ReadOnly Property Rain As New Colormap(New Colormaps.Rain())
        Public Shared ReadOnly Property Solar As New Colormap(New Colormaps.Solar())
        Public Shared ReadOnly Property Speed As New Colormap(New Colormaps.Speed())
        Public Shared ReadOnly Property Tarn As New Colormap(New Colormaps.Tarn())
        Public Shared ReadOnly Property Tempo As New Colormap(New Colormaps.Tempo())
        Public Shared ReadOnly Property Thermal As New Colormap(New Colormaps.Thermal())
        Public Shared ReadOnly Property Topo As New Colormap(New Colormaps.Topo())
        Public Shared ReadOnly Property Turbid As New Colormap(New Colormaps.Turbid())
        Public Shared ReadOnly Property Turbo As New Colormap(New Colormaps.Turbo())
        Public Shared ReadOnly Property Viridis As New Colormap(New Colormaps.Viridis())

        ''' <summary>
        ''' Name of this colormap.
        ''' </summary>
        Public ReadOnly Property Name As String
            Get
                Return ThisColormap.Name
            End Get
        End Property

#End Region '/PROPS

#Region "METHODS"

        Public Overrides Function ToString() As String
            Return $"Colormap {Name}."
        End Function

        ''' <summary>
        ''' Create new instances of every colormap and return them as an array.
        ''' </summary>
        Public Shared Function GetColormaps() As Colormap()
            Return ColormapFactory.GetAvailableColormaps().ToArray()
        End Function

        ''' <summary>
        ''' Return the names of all available colormaps.
        ''' </summary>
        Public Shared Function GetColormapNames() As String()
            Return ColormapFactory.GetAvailableNames().ToArray()
        End Function

        ''' <summary>
        ''' Create a new colormap by its name.
        ''' </summary>
        ''' <param name="name">Colormap name.</param>
        ''' <param name="throwIfNotFound">If false the default colormap <see cref="Viridis"/> will be returned.</param>
        Public Shared Function GetColormapByName(name As String, Optional throwIfNotFound As Boolean = True) As Colormap
            If (Not throwIfNotFound) Then
                Return Colormap.ColormapFactory.CreateOrDefault(name)
            End If
            Return Colormap.ColormapFactory.CreateOrThrow(name)
        End Function

        Public Function GetRgb(value As Byte) As Tuple(Of Byte, Byte, Byte)
            Return ThisColormap.GetRgb(value)
        End Function

        Public Function GetRgb(fraction As Double) As Tuple(Of Byte, Byte, Byte)
            fraction = Math.Max(fraction, 0)
            fraction = Math.Min(fraction, 1)
            Return ThisColormap.GetRgb(CByte(fraction * 255))
        End Function

        Public Function GetInt32(value As Byte, Optional alpha As Byte = 255) As Integer
            Dim rgb As Tuple(Of Byte, Byte, Byte) = GetRgb(value)
            Return (CInt(alpha) << 24) Or (CInt(rgb.Item1) << 16) Or (CInt(rgb.Item2) << 8) Or CInt(rgb.Item3)
        End Function

        Public Function GetInt32(fraction As Double, Optional alpha As Byte = 255) As Integer
            Dim rgb As Tuple(Of Byte, Byte, Byte) = Me.GetRgb(fraction)
            Return (CInt(alpha) << 24) Or (CInt(rgb.Item1) << 16) Or (CInt(rgb.Item2) << 8) Or CInt(rgb.Item3)
        End Function

        Public Function GetColor(value As Byte, Optional alpha As Double = 1) As Color
            Dim alphaByte As Byte = CByte(255 * alpha)
            Return Color.FromArgb(GetInt32(value, alphaByte))
        End Function

        Public Function GetColor(fraction As Double, Optional alpha As Double = 1) As Color
            Dim alphaByte As Byte = CByte(255 * alpha)
            Return Color.FromArgb(GetInt32(fraction, alphaByte))
        End Function

        Public Function RandomColor(rand As Random, Optional alpha As Double = 1) As Color
            Dim alphaByte As Byte = CByte(255 * alpha)
            Return Color.FromArgb(GetInt32(rand.NextDouble(), alphaByte))
        End Function

        Public Sub Apply(bmp As Bitmap)
            Dim pal As System.Drawing.Imaging.ColorPalette = bmp.Palette
            For i As Integer = 0 To 255
                pal.Entries(i) = Me.GetColor(i)
            Next
            bmp.Palette = pal
        End Sub

        Public Shared Function IntenstitiesToRgb(intensities As Double(), cmap As IColormap) As Byte(,)
            Dim output As Byte(,) = New Byte(intensities.Length - 1, 2) {}
            For i As Integer = 0 To intensities.Length - 1
                Dim intensity As Double = intensities(i) * 255
                Dim pixelIntensity As Byte = CByte(Math.Max(Math.Min(intensity, 255), 0))
                Dim rgb = cmap.GetRgb(pixelIntensity)
                output(i, 0) = rgb.Item1
                output(i, 1) = rgb.Item2
                output(i, 2) = rgb.Item3
            Next
            Return output
        End Function

        ''' <summary>
        ''' Return an array of RGBA integer values for a single color where the alpha channel is defined by an array of values from 0 to 1.
        ''' </summary>
        Public Shared Function GetRGBAs(opacity As Double(), color As Color) As Integer()
            Dim rgbas As Integer() = New Integer(opacity.Length - 1) {}
            For i As Integer = 0 To opacity.Length - 1
                Dim alpha As Byte = CByte(Math.Max(Math.Min(opacity(i) * 255, 255), 0))
                Dim argb As Byte() = {color.B, color.G, color.R, alpha}
                rgbas(i) = BitConverter.ToInt32(argb, 0)
            Next
            Return rgbas
        End Function

        ''' <summary>
        ''' Convert intensities to colors using the given colormap and return the results as integer RGBA values.
        ''' </summary>
        Public Shared Function GetRGBAs(intensities As Double(), colorMap As Colormap, Optional minimumIntensity As Double = 0) As Integer()
            Dim rgbas As Integer() = New Integer(intensities.Length - 1) {}
            For i As Integer = 0 To intensities.Length - 1
                Dim pixelIntensity As Byte = CByte(Math.Max(Math.Min(intensities(i) * 255, 255), 0))
                Dim rgb As Tuple(Of Byte, Byte, Byte) = colorMap.GetRgb(pixelIntensity)
                Dim alpha As Byte = CByte(If(intensities(i) < minimumIntensity, 0, 255))
                Dim argb = {rgb.Item3, rgb.Item2, rgb.Item1, alpha}
                rgbas(i) = BitConverter.ToInt32(argb, 0)
            Next
            Return rgbas
        End Function

        ''' <summary>
        ''' Convert intensities to colors using the given colormap And return the results as integer RGBA values.
        ''' RGBA alpha value will be set according to the given array of opacities (values from 0 to 1).
        ''' </summary>
        Public Shared Function GetRGBAs(intensities As Double?(), colorMap As Colormap, Optional minimumIntensity As Double = 0) As Integer()
            Dim rgbas As Integer() = New Integer(intensities.Length - 1) {}
            For i As Integer = 0 To intensities.Length - 1
                If intensities(i).HasValue Then
                    Dim pixelIntensity As Byte = CByte(Math.Max(Math.Min(intensities(i).Value * 255, 255), 0))
                    Dim t = colorMap.GetRgb(pixelIntensity)
                    Dim alpha As Byte = CByte(If(intensities(i) < minimumIntensity, 0, 255))
                    Dim argb As Byte() = {t.Item3, t.Item2, t.Item1, alpha}
                    rgbas(i) = BitConverter.ToInt32(argb, 0)
                Else
                    Dim argb As Byte() = {0, 0, 0, 0}
                    rgbas(i) = BitConverter.ToInt32(argb, 0)
                End If
            Next
            Return rgbas
        End Function

        ''' <summary>
        ''' Return an array of RGBA integer values for a single color where the alpha
        ''' channel Is defined by an array of values from 0 to 1.
        ''' </summary>
        Public Shared Function GetRGBAs(opacity As Double?(), color As Color) As Integer()
            Dim rgbas As Integer() = New Integer(opacity.Length - 1) {}
            For i As Integer = 0 To opacity.Length - 1
                If opacity(i).HasValue Then
                    Dim alpha As Byte
                    If opacity(i).HasValue Then
                        alpha = CByte(Math.Max(Math.Min(opacity(i).Value * 255, 255), 0))
                    Else
                        alpha = 0
                    End If
                    Dim argb As Byte() = {color.B, color.G, color.R, alpha}
                    rgbas(i) = BitConverter.ToInt32(argb, 0)
                Else
                    Dim argb As Byte() = {0, 0, 0, 0}
                    rgbas(i) = BitConverter.ToInt32(argb, 0)
                End If
            Next
            Return rgbas
        End Function

        ''' <summary>
        ''' Convert intensities to colors using the given colormap And return the results as integer RGBA values.
        ''' RGBA alpha value will be set according to the given array of opacities (values from 0 to 1).
        ''' </summary>
        Public Shared Function GetRGBAs(intensities As Double?(), opacity As Double?(), colorMap As Colormap) As Integer()
            Dim rgbas As Integer() = New Integer(intensities.Length - 1) {}
            For i As Integer = 0 To intensities.Length - 1
                If intensities(i).HasValue Then
                    Dim pixelIntensity As Byte = CByte(Math.Max(Math.Min(intensities(i).Value * 255, 255), 0))
                    Dim t = colorMap.GetRgb(pixelIntensity)
                    Dim alpha As Byte
                    If opacity(i).HasValue Then
                        alpha = CByte(Math.Max(Math.Min(opacity(i).Value * 255, 255), 0))
                    Else
                        alpha = 0
                    End If
                    Dim argb As Byte() = {t.Item3, t.Item2, t.Item1, alpha}
                    rgbas(i) = BitConverter.ToInt32(argb, 0)
                Else
                    Dim argb As Byte() = {0, 0, 0, 0}
                    rgbas(i) = BitConverter.ToInt32(argb, 0)
                End If
            Next
            Return rgbas
        End Function

        ''' <summary>
        ''' Given an array of intensities (ranging from 0 to 1) return an array of colors according to the given colormap.
        ''' </summary>
        Public Shared Function GetColors(intensities As Double(), colorMap As Colormap) As Color()
            Dim colors As Color() = New Color(intensities.Length - 1) {}
            For i As Integer = 0 To intensities.Length - 1
                Dim pixelIntensity As Byte = CByte(Math.Max(Math.Min(intensities(i) * 255, 255), 0))
                Dim t = colorMap.GetRgb(pixelIntensity)
                colors(i) = Color.FromArgb(255, t.Item1, t.Item2, t.Item3)
            Next
            Return colors
        End Function

        ''' <summary>
        ''' Return a bitmap showing the gradient of colors in a colormap.
        ''' Defining min/max will create an image containing only part of the colormap.
        ''' </summary>
        Public Shared Function Colorbar(cmap As Colormap,
                                        width As Integer,
                                        height As Integer,
                                        Optional vertical As Boolean = False,
                                        Optional min As Double = 0,
                                        Optional max As Double = 1) As Bitmap
            If (width < 1) OrElse (height < 1) Then
                Return Nothing
            End If
            If (min < 0) Then
                Throw New ArgumentException($"Colorbar(): {NameOf(min)} must be >= 0.")
            End If
            If (max > 1) Then
                Throw New ArgumentException($"Colorbar(): {NameOf(max)} must be <= 1.")
            End If
            If (min >= max) Then
                Throw New ArgumentException($"Colorbar():{NameOf(min)} must < {NameOf(max)}.")
            End If

            Dim bmp As New Bitmap(width, height)
            Using gfx As Graphics = System.Drawing.Graphics.FromImage(bmp), pen As New Pen(Color.Magenta)
                If vertical Then
                    For y As Integer = 0 To height - 1
                        Dim fraction As Double = y / height
                        fraction = fraction * (max - min) + min
                        pen.Color = cmap.GetColor(fraction)
                        gfx.DrawLine(pen, 0, height - y - 1, width - 1, height - y - 1)
                    Next
                Else
                    For x As Integer = 0 To width - 1
                        Dim fraction As Double = x / width
                        fraction = fraction * (max - min) + min
                        pen.Color = cmap.GetColor(fraction)
                        gfx.DrawLine(pen, x, 0, x, height - 1)
                    Next
                End If
            End Using

            Return bmp

        End Function

#End Region '/METHODS

    End Class

End Namespace