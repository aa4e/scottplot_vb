Namespace ScottPlot.Statistics

    Public Module Vector

        ''' <summary>
        ''' Returns whether or not vector operations are subject to hardware acceleration through JIT intrinsic support.
        ''' </summary>
        ''' <remarks>
        ''' Every operation must either be a JIT intrinsic or implemented over a JIT intrinsic as a thin wrapper.
        ''' Operations implemented over a JIT intrinsic should be inlined.
        ''' Methods that do not have a T type parameter are recognized as intrinsics.
        ''' </remarks>
        Friend ReadOnly Property IsHardwareAccelerated As Boolean
            Get
                Return False
            End Get
        End Property


    End Module

End Namespace