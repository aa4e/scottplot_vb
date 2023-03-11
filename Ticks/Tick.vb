Namespace ScottPlot.Ticks

    Public Structure Tick

        Public ReadOnly Position As Double
        Public ReadOnly Label As String
        Public ReadOnly IsMajor As Boolean
        Public ReadOnly IsDateTime As Boolean

        Public ReadOnly Property DateTime As DateTime
            Get
                Return DateTime.FromOADate(Position)
            End Get
        End Property

        Public Sub New(position As Double, label As String, isMajor As Boolean, isDateTime As Boolean)
            Me.Position = position
            Me.Label = label
            Me.IsMajor = isMajor
            Me.IsDateTime = isDateTime
        End Sub

        Public Overrides Function ToString() As String
            Dim tickType As String = If(IsMajor, "Major Tick", "Minor Tick")
            Dim tickLabel As String = If(String.IsNullOrEmpty(Label), "(unlabeled)", $"labeled '{Label}'")
            Dim tickPosition As String = If(IsDateTime, DateTime.ToString(), Position.ToString())
            Return $"{tickType} at {tickPosition} {tickLabel}."
        End Function

    End Structure

End Namespace