Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Namespace ScottPlot

    <Obsolete("This class will be deleted in a future version. See ScottPlot FAQ for details.")>
    Public Class MultiPlot

#Region "PROPS, FIELDS"

        Public ReadOnly Subplots As Plot()
        Public ReadOnly Rows As Integer
        Public ReadOnly Cols As Integer
        Public ReadOnly Width As Integer
        Public ReadOnly Height As Integer

        Private ReadOnly Bmp As Bitmap
        Private ReadOnly Gfx As Graphics

        Public ReadOnly Property SubplotCount As Integer
            Get
                Return (Rows * Cols)
            End Get
        End Property

        Public ReadOnly Property SubplotWidth As Integer
            Get
                Return CInt(Width / Cols)
            End Get
        End Property

        Public ReadOnly Property SubplotHeight As Integer
            Get
                Return CInt(Height / Rows)
            End Get
        End Property

#End Region '/PROPS, FIELDS

#Region "CTOR"

        Public Sub New(Optional width As Integer = 800, Optional height As Integer = 600, Optional rows As Integer = 1, Optional cols As Integer = 1)
            If (rows < 1) OrElse (cols < 1) Then
                Throw New ArgumentException("Must have at least 1 row and column.")
            End If

            Me.Width = width
            Me.Height = height
            Me.Rows = rows
            Me.Cols = cols
            Bmp = New Bitmap(width, height)
            Gfx = Graphics.FromImage(Bmp)

            Subplots = New Plot(Me.SubplotCount - 1) {}
            For i As Integer = 0 To SubplotCount - 1
                Subplots(i) = New Plot(SubplotWidth, SubplotHeight)
            Next
        End Sub

#End Region '/CTOR

#Region "METHODS"

        Private Sub Render()
            Gfx.Clear(Color.White)
            Dim subplotIndex As Integer = 0
            For row As Integer = 0 To Rows - 1
                For col As Integer = 0 To Cols - 1
                    Dim subplotBmp As Bitmap = Subplots(subplotIndex).Render(SubplotWidth, SubplotHeight, False)
                    Dim pt As New Point(col * SubplotWidth, row * SubplotHeight)
                    Gfx.DrawImage(subplotBmp, pt)
                    subplotIndex += 1
                Next
            Next
        End Sub

        Public Function GetBitmap() As Bitmap
            Render()
            Return Bmp
        End Function

        Public Sub SaveFig(filePath As String)
            filePath = Path.GetFullPath(filePath)
            Dim directoryName As String = Path.GetDirectoryName(filePath)
            If (Not Directory.Exists(directoryName)) Then
                Throw New Exception($"ERROR: folder does not exist: {directoryName}.")
            End If

            Dim format As ImageFormat
            Dim extension As String = Path.GetExtension(filePath).ToUpper()
            Select Case extension
                Case ".JPG", ".JPEG"
                    format = ImageFormat.Jpeg 'TODO: use jpgEncoder to set custom compression level
                Case ".PNG"
                    format = ImageFormat.Png
                Case ".TIF", ".TIFF"
                    format = ImageFormat.Tiff
                Case ".BMP"
                    format = ImageFormat.Bmp
                Case Else
                    Throw New NotImplementedException($"Extension not supported: {extension}.")
            End Select

            Render()
            Bmp.Save(filePath, format)
        End Sub

        Public Function GetSubplot(rowIndex As Integer, columnIndex As Integer) As Plot
            If (rowIndex < 0) OrElse (rowIndex >= Rows) Then
                Throw New ArgumentException("Invalid row index.")
            End If
            If (columnIndex < 0) OrElse (columnIndex >= Cols) Then
                Throw New ArgumentException("Invalid column index.")
            End If

            Dim subplotIndex As Integer = rowIndex * Cols + columnIndex
            Return Subplots(subplotIndex)
        End Function

#End Region '/METHODS

    End Class

End Namespace