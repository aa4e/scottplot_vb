Namespace ScottPlot

    ''' <summary>
    ''' Represents a series of data values with a common name. Values from several DataSets can be grouped (by value index).
    ''' </summary>
    Public Class DataSet

        Public Label As String
        Public Values As Double()
        Public Errors As Double()

        Public Sub New(label As String, values As Double(), Optional errors As Double() = Nothing)
            Me.Values = values
            Me.Label = label
            Me.Errors = errors
            If (errors IsNot Nothing) AndAlso (errors.Length <> values.Length) Then
                Throw New ArgumentException("Values and errors must have identical length.")
            End If
        End Sub

    End Class

End Namespace