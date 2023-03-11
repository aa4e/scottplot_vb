Imports System.Collections.Generic

Namespace ScottPlot.Plottable

    Public Class ScatterPlotListDraggable
        Inherits ScatterPlotList(Of Double)
        Implements IDraggable

        Public Event Dragged As EventHandler Implements ScottPlot.Plottable.IDraggable.Dragged

        Private Property IndexUnderMouse As Integer = -1
        Public Property DragEnabled As Boolean = True Implements ScottPlot.Plottable.IDraggable.DragEnabled
        Public ReadOnly Property DragCursor As Cursor = Cursor.Hand Implements ScottPlot.Plottable.IDraggable.DragCursor
        Public Property DragSnap As SnapLogic.ISnap2D = New SnapLogic.NoSnap2D() Implements ScottPlot.Plottable.IDraggable.DragSnap

        ''' <summary>
        ''' Assign custom the logic here to control where individual points can be moved. This logic occurs after snapping.
        ''' </summary>
        Public Property MovePointFunc As Func(Of List(Of Double), List(Of Double), Integer, Coordinate, Coordinate)

        Public Sub DragTo(coordinateX As Double, coordinateY As Double, fixedSize As Boolean) Implements ScottPlot.Plottable.IDraggable.DragTo
            If DragEnabled AndAlso IndexUnderMouse >= 0 Then
                Dim requested As New Coordinate(coordinateX, coordinateY)
                Dim snapped As Coordinate = DragSnap.Snap(requested)
                Dim actual As Coordinate = MovePointFunc(Xs, Ys, IndexUnderMouse, snapped)
                Xs(IndexUnderMouse) = actual.X
                Ys(IndexUnderMouse) = actual.Y
                RaiseEvent Dragged(Me, EventArgs.Empty)
            End If
        End Sub

        Public Function IsUnderMouse(coordinateX As Double, coordinateY As Double, snapX As Double, snapY As Double) As Boolean Implements ScottPlot.Plottable.IDraggable.IsUnderMouse
            For i As Integer = 0 To MyBase.Count - 1
                Dim dx As Double = Math.Abs(NumericConversion.GenericToDouble(Of Double)(Xs, i) - coordinateX)
                Dim dy As Double = Math.Abs(NumericConversion.GenericToDouble(Of Double)(Ys, i) - coordinateY)
                If (dx <= snapX) AndAlso (dy <= snapY) Then
                    IndexUnderMouse = i
                    Return True
                End If
            Next
            IndexUnderMouse = -1
            Return False
        End Function

        Public Sub New()
            MovePointFunc = Function(xs, ys, index, moveTo) moveTo
        End Sub

    End Class

End Namespace