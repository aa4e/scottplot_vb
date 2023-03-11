Namespace ScottPlot.SnapLogic

    ''' <summary>
    ''' Snaps to the nearest position in a user-provided array.
    ''' </summary>
    Public Class Nearest1D
        Implements ISnap1D

        Private SnapPositions As Double()

        Public Sub New(positions As Double())
            SnapPositions = positions
        End Sub

        ''' <summary>
        ''' Returns the position of the item in the array closest to the given position.
        ''' </summary>
        Public Function Snap(value As Double) As Double Implements ScottPlot.SnapLogic.ISnap1D.Snap
            Dim index As Integer = SnapIndex(value)
            Return SnapPositions(index)
        End Function

        ''' <summary>
        ''' Returns the index of the item in the array closest to the given position.
        ''' </summary>
        Public Function SnapIndex(value As Double) As Integer
            Dim closestDistance As Double = Double.MaxValue
            Dim closestIndex As Integer = 0
            For i As Integer = 0 To SnapPositions.Length - 1
                Dim distance As Double = Math.Abs(value - SnapPositions(i))
                If (distance < closestDistance) Then
                    closestDistance = distance
                    closestIndex = i
                End If
            Next
            Return closestIndex
        End Function

    End Class

End Namespace