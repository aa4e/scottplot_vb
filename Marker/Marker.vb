Imports System.Linq

Namespace ScottPlot

    Public Module Marker

        Private ReadOnly Rand As New Random(0)

        Public Function Random() As MarkerShape
            Return Marker.Random(Marker.Rand)
        End Function

        Public Function GetMarkers() As MarkerShape()
            Return [Enum].GetValues(GetType(MarkerShape)) _
                .Cast(Of MarkerShape) _
                .Where(Function(x) (x <> MarkerShape.None)).ToArray()
        End Function

        Public Function Random(rand As Random) As MarkerShape
            Dim markers As MarkerShape() = Marker.GetMarkers()
            Return markers(rand.Next(markers.Length))
        End Function

        ''' <summary>
        ''' Create a new Marker (IMarker class) from an old marker (MarkerStyle enum).
        ''' </summary>
        Public Function Create(shape As MarkerShape) As IMarker
            Select Case shape
                Case MarkerShape.None
                    Return New MarkerShapes.None()
                Case MarkerShape.FilledCircle
                    Return New MarkerShapes.FilledCircle()
                Case MarkerShape.FilledSquare
                    Return New MarkerShapes.FilledSquare()
                Case MarkerShape.OpenCircle
                    Return New MarkerShapes.OpenCircle()
                Case MarkerShape.OpenSquare
                    Return New MarkerShapes.OpenSquare()
                Case MarkerShape.FilledDiamond
                    Return New MarkerShapes.FilledDiamond()
                Case MarkerShape.OpenDiamond
                    Return New MarkerShapes.OpenDiamond()
                Case MarkerShape.Asterisk
                    Return New MarkerShapes.Asterisk()
                Case MarkerShape.HashTag
                    Return New MarkerShapes.Hashtag()
                Case MarkerShape.Cross
                    Return New MarkerShapes.Cross()
                Case MarkerShape.Eks
                    Return New MarkerShapes.Eks()
                Case MarkerShape.VerticalBar
                    Return New MarkerShapes.VerticalBar()
                Case MarkerShape.TriUp
                    Return New MarkerShapes.TriStarUp()
                Case MarkerShape.TriDown
                    Return New MarkerShapes.TriStarDown()
                Case MarkerShape.FilledTriangleUp
                    Return New MarkerShapes.FilledTriangleUp()
                Case MarkerShape.FilledTriangleDown
                    Return New MarkerShapes.FilledTriangleDown()
                Case MarkerShape.OpenTriangleUp
                    Return New MarkerShapes.OpenTriangleUp()
                Case MarkerShape.OpenTriangleDown
                    Return New MarkerShapes.OpenTriangleDown()
                Case Else
                    Throw New NotImplementedException($"Unsupported {shape.GetType()}: {shape}.")
            End Select
        End Function

    End Module

End Namespace