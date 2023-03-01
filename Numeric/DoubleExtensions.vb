Imports System.Runtime.CompilerServices

Namespace ScottPlot

    Friend Module DoubleExtensions

        <Extension()>
        Public Function IsFinite(x As Double) As Boolean
            Return (Not Double.IsNaN(x)) AndAlso (Not Double.IsInfinity(x))
        End Function

    End Module

End Namespace