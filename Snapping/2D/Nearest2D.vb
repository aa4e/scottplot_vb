Namespace ScottPlot.SnapLogic

    ''' <summary>
    ''' Snaps to the nearest position in a user-provided array.
    ''' </summary>
    Public Class Nearest2D
        Implements ISnap2D

        Private Coordinates As Coordinate()

        Public Sub New(coordinates As Coordinate())
            Me.Coordinates = coordinates
        End Sub

        Public Sub New(xs As Double(), ys As Double())
            If (xs.Length <> ys.Length) Then
                Throw New ArgumentException("Xs must have the same length as Ys.")
            End If
            Coordinates = New Coordinate(xs.Length - 1) {}
            For i As Integer = 0 To xs.Length - 1
                Coordinates(i) = New Coordinate(xs(i), ys(i))
            Next
        End Sub

        ''' <summary>
        ''' Returns the position of the item in the array closest to the given position.
        ''' </summary>
        Public Function Snap(value As Coordinate) As Coordinate Implements ScottPlot.SnapLogic.ISnap2D.Snap
            Dim closestIndex As Integer = SnapIndex(value)
            Return Me.Coordinates(closestIndex)
        End Function

        ''' <summary>
        ''' Returns the index of the item in the array closest to the given position.
        ''' </summary>
        Public Function SnapIndex(value As Coordinate) As Integer
            Dim closestDistance As Double = Double.MaxValue
            Dim closestIndex As Integer = 0
            For i As Integer = 0 To Coordinates.Length - 1
                Dim dX As Double = Math.Abs(Me.Coordinates(i).X - value.X)
                Dim dY As Double = Math.Abs(Me.Coordinates(i).Y - value.Y)
                Dim distance As Double = dX * dX + dY * dY
                If (distance < closestDistance) Then
                    closestDistance = distance
                    closestIndex = i
                End If
            Next
            Return closestIndex
        End Function

    End Class

End Namespace