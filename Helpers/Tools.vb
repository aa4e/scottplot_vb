Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text

Namespace ScottPlot

    Public Module Tools

        Public Function GetRandomColor(Optional rand As Random = Nothing) As Color
            If (rand is Nothing) Then
                rand = New Random()
            End If
            Dim randomColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256))
            Return randomColor
        End Function

        Public Function GetRandomBrush() As Brush
            Return New SolidBrush(Tools.GetRandomColor())
        End Function

        Public Function Blend(colorA As Color, colorB As Color, fractionA As Double) As Color
            fractionA = Math.Max(fractionA, 0)
            fractionA = Math.Min(fractionA, 1)
            Dim r As Byte = CByte((colorA.R * fractionA) + colorB.R * (1 - fractionA))
            Dim g As Byte = CByte((colorA.G * fractionA) + colorB.G * (1 - fractionA))
            Dim b As Byte = CByte((colorA.B * fractionA) + colorB.B * (1 - fractionA))
            Return Color.FromArgb(r, g, b)
        End Function

        <Obsolete("Use ScottPlot.Plot.Version", True)>
        Public Function GetVersionString(Optional justThreeDigits As Boolean = True) As String
            Dim version As Version = GetType(Plot).Assembly.GetName().Version
            If justThreeDigits Then
                Return $"{version.Major}.{version.Minor}.{version.Build}"
            End If
            Return version.ToString()
        End Function

        Public Function GetFrameworkVersionString() As String
            Return $".NET {Environment.Version.ToString()}"
        End Function

        Public Function BitmapHash(bmp As Bitmap) As String
            Dim bmpBytes As Byte() = Tools.BitmapToBytes(bmp)
            Dim md5 As HashAlgorithm = Security.Cryptography.MD5.Create()
            Dim hashString As New StringBuilder()
            Dim hashBytes As Byte() = md5.ComputeHash(bmpBytes)
            For i As Integer = 0 To hashBytes.Length - 1
                hashString.Append(hashBytes(i).ToString("X2"))
            Next
            Return hashString.ToString()
        End Function

        Public Function BitmapFromBytes(bytes As Byte(), size As Size, Optional format As PixelFormat = PixelFormat.Format8bppIndexed) As Bitmap
            Dim bmp As New Bitmap(size.Width, size.Height, format)
            Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim bmpData As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat)
            Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length)
            bmp.UnlockBits(bmpData)
            Return bmp
        End Function

        Public Function BitmapToBytes(bmp As Bitmap) As Byte()
            Dim bytesPerPixel As Double = Image.GetPixelFormatSize(bmp.PixelFormat) / 8
            Dim array As Byte() = New Byte(CInt(bmp.Width * bmp.Height * bytesPerPixel - 1)) {}
            Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim bmpData As BitmapData = bmp.LockBits(rect, ImageLockMode.[ReadOnly], bmp.PixelFormat)
            Marshal.Copy(bmpData.Scan0, array, 0, array.Length)
            bmp.UnlockBits(bmpData)
            Return array
        End Function

        <Obsolete("Use ScottPlot.Config.Fonts.GetValidFontName()", True)>
        Public Function VerifyFont(fontName As String) As String
            Return Nothing
        End Function

        Public Function ScientificNotation(value As Double, Optional decimalPlaces As Integer = 2, Optional preceedWithPlus As Boolean = True) As String
            Dim output As New StringBuilder()
            If (Math.Abs(value) > 0.0001) AndAlso (Math.Abs(value) < 10000.0) Then
                value = Math.Round(value, decimalPlaces)
                output.Append(value)
            Else
                Dim exponent As Integer = CInt(Math.Log10(value))
                Dim multiplier As Double = Math.Pow(10, exponent)
                Dim mantissa As Double = value / multiplier
                mantissa = Math.Round(mantissa, decimalPlaces)
                output.Append($"{mantissa}e{exponent}")
            End If
            If preceedWithPlus AndAlso (output(0) <> "-") Then
                output = output.Insert(0, "+")
            End If
            Return output.ToString()
        End Function

        Public Sub DesignerModeDemoPlot(plt As ScottPlot.Plot)
            Dim pointCount As Integer = 101
            Dim pointSpacing As Double = 0.01
            Dim dataXs As Double() = DataGen.Consecutive(pointCount, pointSpacing)
            Dim dataSin As Double() = DataGen.Sin(pointCount)
            Dim dataCos As Double() = DataGen.Cos(pointCount)

            plt.AddScatter(dataXs, dataSin)
            plt.AddScatter(dataXs, dataCos)
            plt.AxisAuto(0)
            plt.Title("ScottPlot User Control")
            plt.YLabel("Sample Data")
        End Sub

        Public Function DateTimesToDoubles(dateTimeArray As DateTime()) As Double()
            Dim positions As Double() = New Double(dateTimeArray.Length - 1) {}
            For i As Integer = 0 To positions.Length - 1
                positions(i) = dateTimeArray(i).ToOADate()
            Next
            Return positions
        End Function

        Private Function DoubleArray(Of T)(dataIn As T()) As Double()
            Dim dataOut As Double() = New Double(dataIn.Length - 1) {}
            For i As Integer = 0 To dataIn.Length - 1
                dataOut(i) = NumericConversion.GenericToDouble(Of T)(dataIn(i))
            Next
            Return dataOut
        End Function

        Public Function DoubleArray(dataIn As Byte()) As Double()
            Return Tools.DoubleArray(Of Byte)(dataIn)
        End Function

        Public Function DoubleArray(dataIn As Integer()) As Double()
            Return Tools.DoubleArray(Of Integer)(dataIn)
        End Function

        Public Function DoubleArray(dataIn As Single()) As Double()
            Return Tools.DoubleArray(Of Single)(dataIn)
        End Function

        Public Sub ApplyBaselineSubtraction(data As Double(), index1 As Integer, index2 As Integer)
            Dim baselineSum As Double = 0
            For i As Integer = index1 To index2 - 1
                baselineSum += data(i)
            Next

            Dim baselineAverage As Double = baselineSum / (index2 - index1)
            For i As Integer = 0 To data.Length - 1
                data(i) -= baselineAverage
            Next
        End Sub

        Public Function Log10(dataIn As Double()) As Double()
            Dim dataOut As Double() = New Double(dataIn.Length - 1) {}
            For i As Integer = 0 To dataOut.Length - 1
                dataOut(i) = If(dataIn(i) > 0,
                    Math.Log10(dataIn(i)),
                    0)
            Next
            Return dataOut
        End Function

        Public Function ConvertPolarCoordinates(rs As Double(), thetas As Double()) As Tuple(Of Double(), Double())
            Dim xs As Double() = New Double(rs.Length - 1) {}
            Dim ys As Double() = New Double(rs.Length - 1) {}
            For i As Integer = 0 To rs.Length - 1
                Dim x As Double = rs(i)
                Dim y As Double = thetas(i)
                xs(i) = x * Math.Cos(y)
                ys(i) = x * Math.Sin(y)
            Next
            Return New Tuple(Of Double(), Double())(xs, ys)
        End Function

        Public Sub LaunchBrowser(url As String)
            Try
                Process.Start(url)
            Catch ex As Exception
                Debug.WriteLine(ex)
                'If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
                '    Process.Start(New ProcessStartInfo("cmd", "/c start " + url) With {.CreateNoWindow = True})
                'ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.Linux) Then
                '    Process.Start("xdg-open", url)
                'Else
                '    If Not RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
                '        Throw
                '    End If
                '    Process.Start("open", url)
                'End If
            End Try
        End Sub

        Public Function Round(data As Double(), Optional decimals As Integer = 2) As Double()
            Dim rounded As Double() = New Double(data.Length - 1) {}
            For i As Integer = 0 To data.Length - 1
                rounded(i) = Math.Round(data(i), decimals)
            Next
            Return rounded
        End Function

        ''' <summary>
        ''' Return a copy of the given array padded with the given value at both sidees.
        ''' </summary>
        Public Function Pad(values As Double(), Optional padCount As Integer = 1, Optional padWithLeft As Double = 0.0, Optional padWithRight As Double = 0.0, Optional cloneEdges As Boolean = False) As Double()
            Dim padded As Double() = New Double(values.Length + padCount * 2 - 1) {}
            Array.Copy(values, 0, padded, padCount, values.Length)

            If cloneEdges Then
                padWithLeft = values(0)
                padWithRight = values(values.Length - 1)
            End If

            For i As Integer = 0 To padCount - 1
                padded(i) = padWithLeft
                padded(padded.Length - 1 - i) = padWithRight
            Next
            Return padded
        End Function

        <Obsolete("TODO Get OS name.")>
        Public Function GetOsName(Optional details As Boolean = True) As String
            Dim text As String = String.Empty '= "Unknown"
            'If RuntimeInformation.IsOSPlatform(OSPlatform.Linux) Then
            '    text = "Linux"
            'ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.OSX) Then
            '    text = "MacOS"
            'ElseIf RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            '    text = "Windows"
            'End If
            If details Then
                text += $" ({System.Environment.OSVersion})"
            End If
            Return text
        End Function

        Public Function SimpleHash(input As Double()) As Integer
            Dim bytes As Byte() = input.SelectMany(Function(n) BitConverter.GetBytes(n)).ToArray()
            Dim hash As Integer = 0
            For Each b In bytes
                hash = (hash * 31) Xor b
            Next
            Return hash
        End Function

        Private Function NormPDF(x As Double, mu As Double, sigma As Double) As Double
            Return (1 / (sigma * Math.Sqrt(2 * Math.PI))) * Math.Exp(-0.5 * (x - mu / sigma) * (x - mu / sigma))
        End Function

        Public Function XYToIntensitiesGaussian(xs As Integer(), ys As Integer(), width As Integer, height As Integer, sigma As Integer) As Double(,)
            Dim output As Double(,) = New Double(height - 1, width - 1) {}
            Dim intermediate As Double(,) = New Double(height - 1, width - 1) {} 'Each cell has the number of hits. This is the array before any blurring

            Dim radius As Integer = 2 '2 standard deviations is ~0.95, i.e. close enough
            For i As Integer = 0 To xs.Length - 1
                If (xs(i) >= 0) AndAlso (xs(i) < width) AndAlso (ys(i) >= 0) AndAlso (ys(i) < height) Then
                    intermediate(ys(i), xs(i)) += 1
                End If
            Next

            Dim kernel As Double() = New Double(2 * radius * sigma + 1 - 1) {}
            For i As Integer = 0 To kernel.Length - 1
                kernel(i) = NormPDF(i - kernel.Length / 2, 0, sigma)
            Next

            For i As Integer = 0 To height - 1 'Blurs up and down, i.e. a vertical kernel. Gaussian Blurs are special in that it can be decomposed this way, saving time
                For j As Integer = 0 To width - 1
                    Dim sum As Double = 0
                    Dim kernelSum As Double = 0 'The kernelSum can be precomputed, but this gives incorrect output at the edges of the image
                    For k As Integer = -radius * sigma To radius * sigma
                        If (i + k >= 0) AndAlso (i + k < height) Then
                            sum += intermediate(i + k, j) * kernel(CInt(k + kernel.Length / 2))
                            kernelSum += kernel(CInt(k + kernel.Length / 2))
                        End If
                    Next
                    output(i, j) = sum / kernelSum
                Next
            Next

            For i As Integer = 0 To height - 1 'Blurs left and right, i.e. a horizontal kernel
                For j As Integer = 0 To width - 1
                    Dim sum As Double = 0
                    Dim kernelSum As Double = 0
                    For k As Integer = -radius * sigma To radius * sigma
                        If (j + k >= 0) AndAlso (j + k < width) Then
                            sum += output(i, j + k) * kernel(CInt(k + kernel.Length / 2))
                            kernelSum += kernel(CInt(k + kernel.Length / 2))
                        End If
                    Next
                    output(i, j) = sum / kernelSum
                Next
            Next
            Return output
        End Function

        Public Function XYToIntensitiesDensity(xs As Integer(), ys As Integer(), width As Integer, height As Integer, sampleWidth As Integer) As Double(,)
            Dim output As Double(,) = New Double(height - 1, width - 1) {}
            Dim points = xs.Zip(ys, Function(x, y) New Tuple(Of Integer, Integer)(x, y)).ToArray()
            points = points.OrderBy(Function(p) p.Item1).ToArray()
            Dim xsSorted As Integer() = points.Select(Function(p) p.Item1).ToArray()

            For i As Integer = 0 To height - height Mod sampleWidth - 1 Step sampleWidth
                For j As Integer = 0 To width - width Mod sampleWidth - 1 Step sampleWidth
                    Dim count As Integer = 0
                    For k As Integer = 0 To sampleWidth - 1
                        For l As Integer = 0 To sampleWidth - 1
                            Dim index As Integer = Array.BinarySearch(xsSorted, j + l)
                            If (index > 0) Then
                                For m As Integer = index To xs.Length - 1 'Multiple points w/ same x value
                                    If (points(m).Item1 = j + l) AndAlso (points(m).Item2 = i + k) Then
                                        count += 1 ' Increments number of hits in sampleWidth sized square
                                    End If
                                Next
                            End If
                        Next
                    Next

                    For k As Integer = 0 To sampleWidth - 1
                        For l As Integer = 0 To sampleWidth - 1
                            output(i + k, j + l) = count
                        Next
                    Next
                Next
            Next
            Return output
        End Function

        Public Function XYToIntensities(mode As IntensityMode,
                                        xs As Integer(),
                                        ys As Integer(),
                                        width As Integer,
                                        height As Integer,
                                        sampleWidth As Integer) As Double(,)
            Select Case mode
                Case IntensityMode.Gaussian
                    Return XYToIntensitiesGaussian(xs, ys, width, height, sampleWidth)
                Case IntensityMode.Density
                    Return XYToIntensitiesDensity(xs, ys, width, height, sampleWidth)
                Case Else
                    Throw New NotImplementedException($"{NameOf(mode)} is not a supported {NameOf(IntensityMode)}.")
            End Select
        End Function

        Public Function ToDifferentBase(number As Double,
                                        Optional radix As Integer = 16,
                                        Optional decimalPlaces As Integer = 3,
                                        Optional padInteger As Integer = 0,
                                        Optional dropTrailingZeroes As Boolean = True,
                                        Optional decimalSymbol As Char = "."c) As String
            If (number < 0) Then
                Return "-" & Tools.ToDifferentBase(Math.Abs(number), radix, decimalPlaces, padInteger, dropTrailingZeroes, decimalSymbol)
            ElseIf (number = 0) Then
                Return "0"
            End If

            Dim symbols As Char() = "0123456789ABCDEF".ToCharArray()
            If (radix > symbols.Length) OrElse (radix <= 1) Then
                Throw New ArgumentOutOfRangeException($"{NameOf(radix)}")
            End If

            Dim epsilon As Double = Math.Pow(radix, -decimalPlaces)
            If (radix > symbols.Length) Then
                Throw New ArgumentOutOfRangeException($"{NameOf(radix)}")
            End If

            Dim integerLength As Integer = CInt(Math.Ceiling(Math.Log(number, radix)))
            Dim decimalLength As Integer = If(number Mod 1.0 > epsilon, decimalPlaces, 0)
            Dim decimalPart As Double = number Mod 1.0
            Dim output As New System.Text.StringBuilder()

            For i As Integer = 0 To integerLength - 1
                If (number = radix) AndAlso (padInteger = 0) Then
                    output = output.Insert(0, "10")
                Else
                    output = output.Insert(0, symbols(CInt(number Mod radix)).ToString())
                End If
                number /= radix
            Next

            While (output.Length < padInteger)
                output = output.Insert(0, "0")
            End While

            If (decimalLength <> 0) Then
                If (output.Length = 0) Then
                    output.Append("0")
                End If

                output.Append(decimalSymbol)
                output.Append(Tools.ToDifferentBase(Math.Round(decimalPart * Math.Pow(radix, decimalPlaces)), radix, decimalPlaces, decimalPlaces, dropTrailingZeroes, decimalSymbol))

                If dropTrailingZeroes Then
                    While (output(output.Length - 1) = "0"c)
                        output = output.Remove(output.Length - 2, 1)
                    End While

                    If (output(output.Length - 1) = decimalSymbol) Then
                        output = output.Remove(output.Length - 2, 1)
                    End If
                End If
            End If
            Return output.ToString()
        End Function

        Public Function Uses24HourClock(culture As CultureInfo) As Boolean
            Return culture.DateTimeFormat.LongTimePattern.Contains("H")
        End Function

    End Module

End Namespace