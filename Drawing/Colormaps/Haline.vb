﻿Namespace ScottPlot.Drawing.Colormaps

    Public Class Haline
        Implements IColormap

        Public Function GetRGB(value As Byte) As Tuple(Of Byte, Byte, Byte) Implements ScottPlot.Drawing.IColormap.GetRGB
            Return New Tuple(Of Byte, Byte, Byte)(Haline.Cmaplocal(value, 0), Haline.Cmaplocal(value, 1), Haline.Cmaplocal(value, 2))
        End Function

        Public ReadOnly Property Name As String = "Haline" Implements ScottPlot.Drawing.IColormap.Name

        Private Shared ReadOnly Cmaplocal As Byte(,) = New Byte(,) {
            {42, 24, 108},
            {42, 25, 110},
            {42, 25, 113},
            {43, 25, 115},
            {43, 25, 117},
            {44, 26, 120},
            {44, 26, 122},
            {45, 26, 125},
            {45, 26, 127},
            {45, 27, 130},
            {46, 27, 132},
            {46, 27, 135},
            {46, 28, 137},
            {46, 28, 140},
            {46, 28, 142},
            {46, 29, 145},
            {46, 29, 147},
            {46, 30, 149},
            {46, 30, 152},
            {46, 31, 154},
            {45, 32, 156},
            {45, 33, 157},
            {44, 34, 159},
            {43, 36, 160},
            {42, 37, 161},
            {41, 39, 162},
            {40, 41, 163},
            {38, 43, 163},
            {37, 45, 163},
            {36, 46, 163},
            {34, 48, 163},
            {33, 50, 162},
            {32, 52, 162},
            {30, 53, 161},
            {29, 55, 161},
            {28, 57, 160},
            {27, 58, 160},
            {25, 60, 159},
            {24, 61, 158},
            {23, 63, 158},
            {22, 64, 157},
            {21, 65, 156},
            {20, 67, 156},
            {19, 68, 155},
            {18, 69, 155},
            {17, 71, 154},
            {16, 72, 153},
            {15, 73, 153},
            {15, 74, 152},
            {14, 76, 151},
            {13, 77, 151},
            {13, 78, 150},
            {13, 79, 150},
            {12, 80, 149},
            {12, 81, 149},
            {12, 82, 148},
            {12, 83, 148},
            {12, 84, 147},
            {13, 85, 147},
            {13, 86, 146},
            {13, 87, 146},
            {14, 88, 145},
            {14, 89, 145},
            {15, 90, 145},
            {15, 91, 144},
            {16, 92, 144},
            {17, 93, 143},
            {17, 94, 143},
            {18, 95, 143},
            {19, 96, 142},
            {20, 97, 142},
            {20, 98, 142},
            {21, 99, 142},
            {22, 99, 141},
            {23, 100, 141},
            {24, 101, 141},
            {24, 102, 140},
            {25, 103, 140},
            {26, 104, 140},
            {27, 105, 140},
            {28, 106, 140},
            {29, 107, 139},
            {29, 107, 139},
            {30, 108, 139},
            {31, 109, 139},
            {32, 110, 139},
            {33, 111, 139},
            {34, 112, 138},
            {34, 113, 138},
            {35, 113, 138},
            {36, 114, 138},
            {37, 115, 138},
            {38, 116, 138},
            {38, 117, 138},
            {39, 118, 138},
            {40, 118, 137},
            {41, 119, 137},
            {41, 120, 137},
            {42, 121, 137},
            {43, 122, 137},
            {43, 123, 137},
            {44, 124, 137},
            {45, 124, 137},
            {45, 125, 137},
            {46, 126, 137},
            {47, 127, 137},
            {47, 128, 137},
            {48, 129, 137},
            {49, 130, 137},
            {49, 130, 136},
            {50, 131, 136},
            {51, 132, 136},
            {51, 133, 136},
            {52, 134, 136},
            {52, 135, 136},
            {53, 136, 136},
            {53, 137, 136},
            {54, 137, 136},
            {55, 138, 136},
            {55, 139, 136},
            {56, 140, 136},
            {56, 141, 136},
            {57, 142, 136},
            {57, 143, 136},
            {58, 144, 135},
            {58, 144, 135},
            {59, 145, 135},
            {59, 146, 135},
            {60, 147, 135},
            {60, 148, 135},
            {61, 149, 135},
            {61, 150, 135},
            {62, 151, 135},
            {62, 152, 134},
            {63, 153, 134},
            {63, 153, 134},
            {64, 154, 134},
            {65, 155, 134},
            {65, 156, 133},
            {66, 157, 133},
            {66, 158, 133},
            {67, 159, 133},
            {67, 160, 132},
            {68, 161, 132},
            {68, 162, 132},
            {69, 163, 132},
            {70, 164, 131},
            {70, 164, 131},
            {71, 165, 131},
            {72, 166, 130},
            {72, 167, 130},
            {73, 168, 130},
            {74, 169, 129},
            {74, 170, 129},
            {75, 171, 129},
            {76, 172, 128},
            {76, 173, 128},
            {77, 174, 127},
            {78, 174, 127},
            {79, 175, 126},
            {80, 176, 126},
            {81, 177, 125},
            {81, 178, 125},
            {82, 179, 124},
            {83, 180, 124},
            {84, 181, 123},
            {85, 182, 123},
            {86, 183, 122},
            {87, 184, 121},
            {88, 184, 121},
            {90, 185, 120},
            {91, 186, 119},
            {92, 187, 119},
            {93, 188, 118},
            {94, 189, 117},
            {95, 190, 117},
            {97, 191, 116},
            {98, 191, 115},
            {99, 192, 114},
            {101, 193, 114},
            {102, 194, 113},
            {104, 195, 112},
            {105, 196, 111},
            {107, 196, 110},
            {108, 197, 110},
            {110, 198, 109},
            {112, 199, 108},
            {113, 200, 107},
            {115, 200, 106},
            {117, 201, 105},
            {119, 202, 104},
            {120, 203, 104},
            {122, 203, 103},
            {124, 204, 102},
            {126, 205, 101},
            {128, 206, 100},
            {130, 206, 99},
            {132, 207, 98},
            {134, 208, 98},
            {137, 208, 97},
            {139, 209, 96},
            {141, 210, 95},
            {143, 210, 95},
            {146, 211, 94},
            {148, 211, 93},
            {151, 212, 93},
            {153, 212, 93},
            {155, 213, 92},
            {158, 214, 92},
            {160, 214, 92},
            {163, 215, 92},
            {165, 215, 92},
            {168, 216, 92},
            {170, 216, 92},
            {173, 216, 92},
            {175, 217, 93},
            {177, 217, 93},
            {180, 218, 94},
            {182, 218, 95},
            {184, 219, 96},
            {187, 219, 97},
            {189, 220, 98},
            {191, 220, 99},
            {193, 221, 100},
            {196, 221, 101},
            {198, 222, 102},
            {200, 222, 103},
            {202, 223, 105},
            {204, 223, 106},
            {206, 224, 108},
            {208, 224, 109},
            {210, 225, 111},
            {212, 225, 112},
            {214, 226, 114},
            {216, 226, 115},
            {218, 227, 117},
            {220, 227, 119},
            {222, 228, 121},
            {224, 229, 122},
            {225, 229, 124},
            {227, 230, 126},
            {229, 230, 128},
            {231, 231, 129},
            {233, 231, 131},
            {235, 232, 133},
            {236, 233, 135},
            {238, 233, 137},
            {240, 234, 138},
            {242, 234, 140},
            {243, 235, 142},
            {245, 236, 144},
            {247, 236, 146},
            {248, 237, 148},
            {250, 238, 150},
            {252, 238, 152},
            {253, 239, 154}}

    End Class

End Namespace