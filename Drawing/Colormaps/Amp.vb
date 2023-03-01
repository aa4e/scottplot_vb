﻿Namespace ScottPlot.Drawing.Colormaps

    Public Class Amp
        Implements IColormap

        Public Function GetRGB(value As Byte) As Tuple(Of Byte, Byte, Byte) Implements ScottPlot.Drawing.IColormap.GetRGB
            Return New Tuple(Of Byte, Byte, Byte)(Amp.CmapLocal(value, 0), Amp.CmapLocal(value, 1), Amp.CmapLocal(value, 2))
        End Function

        Public ReadOnly Property Name As String = "Amp" Implements ScottPlot.Drawing.IColormap.Name

        Private Shared ReadOnly CmapLocal As Byte(,) = New Byte(,) {
            {241, 237, 236},
            {241, 236, 235},
            {240, 235, 233},
            {239, 233, 232},
            {239, 232, 231},
            {238, 231, 229},
            {238, 230, 228},
            {237, 229, 227},
            {237, 227, 225},
            {236, 226, 224},
            {236, 225, 222},
            {235, 224, 221},
            {235, 223, 220},
            {234, 221, 218},
            {234, 220, 217},
            {233, 219, 215},
            {233, 218, 214},
            {233, 217, 212},
            {232, 216, 211},
            {232, 214, 210},
            {231, 213, 208},
            {231, 212, 207},
            {230, 211, 205},
            {230, 210, 204},
            {230, 209, 202},
            {229, 207, 201},
            {229, 206, 200},
            {228, 205, 198},
            {228, 204, 197},
            {228, 203, 195},
            {227, 201, 194},
            {227, 200, 192},
            {226, 199, 191},
            {226, 198, 189},
            {226, 197, 188},
            {225, 196, 187},
            {225, 195, 185},
            {225, 193, 184},
            {224, 192, 182},
            {224, 191, 181},
            {223, 190, 179},
            {223, 189, 178},
            {223, 188, 176},
            {222, 186, 175},
            {222, 185, 174},
            {222, 184, 172},
            {221, 183, 171},
            {221, 182, 169},
            {221, 181, 168},
            {220, 180, 166},
            {220, 178, 165},
            {220, 177, 163},
            {219, 176, 162},
            {219, 175, 161},
            {219, 174, 159},
            {218, 173, 158},
            {218, 172, 156},
            {217, 170, 155},
            {217, 169, 153},
            {217, 168, 152},
            {216, 167, 150},
            {216, 166, 149},
            {216, 165, 148},
            {215, 164, 146},
            {215, 162, 145},
            {215, 161, 143},
            {214, 160, 142},
            {214, 159, 140},
            {214, 158, 139},
            {213, 157, 137},
            {213, 156, 136},
            {213, 154, 135},
            {212, 153, 133},
            {212, 152, 132},
            {212, 151, 130},
            {211, 150, 129},
            {211, 149, 127},
            {211, 148, 126},
            {210, 146, 125},
            {210, 145, 123},
            {210, 144, 122},
            {209, 143, 120},
            {209, 142, 119},
            {209, 141, 118},
            {208, 140, 116},
            {208, 139, 115},
            {208, 137, 113},
            {207, 136, 112},
            {207, 135, 111},
            {207, 134, 109},
            {206, 133, 108},
            {206, 132, 106},
            {205, 131, 105},
            {205, 129, 104},
            {205, 128, 102},
            {204, 127, 101},
            {204, 126, 100},
            {204, 125, 98},
            {203, 124, 97},
            {203, 122, 96},
            {203, 121, 94},
            {202, 120, 93},
            {202, 119, 91},
            {201, 118, 90},
            {201, 117, 89},
            {201, 116, 87},
            {200, 114, 86},
            {200, 113, 85},
            {200, 112, 84},
            {199, 111, 82},
            {199, 110, 81},
            {198, 109, 80},
            {198, 107, 78},
            {198, 106, 77},
            {197, 105, 76},
            {197, 104, 74},
            {197, 103, 73},
            {196, 101, 72},
            {196, 100, 71},
            {195, 99, 70},
            {195, 98, 68},
            {195, 97, 67},
            {194, 95, 66},
            {194, 94, 65},
            {193, 93, 63},
            {193, 92, 62},
            {192, 91, 61},
            {192, 89, 60},
            {192, 88, 59},
            {191, 87, 58},
            {191, 86, 57},
            {190, 84, 56},
            {190, 83, 54},
            {189, 82, 53},
            {189, 81, 52},
            {189, 79, 51},
            {188, 78, 50},
            {188, 77, 49},
            {187, 76, 48},
            {187, 74, 48},
            {186, 73, 47},
            {186, 72, 46},
            {185, 70, 45},
            {185, 69, 44},
            {184, 68, 43},
            {184, 66, 43},
            {183, 65, 42},
            {183, 64, 41},
            {182, 63, 41},
            {181, 61, 40},
            {181, 60, 39},
            {180, 59, 39},
            {180, 57, 38},
            {179, 56, 38},
            {178, 55, 38},
            {178, 53, 37},
            {177, 52, 37},
            {176, 51, 37},
            {176, 49, 37},
            {175, 48, 36},
            {174, 47, 36},
            {174, 45, 36},
            {173, 44, 36},
            {172, 43, 36},
            {171, 42, 36},
            {170, 40, 36},
            {170, 39, 36},
            {169, 38, 36},
            {168, 37, 36},
            {167, 36, 36},
            {166, 34, 37},
            {165, 33, 37},
            {164, 32, 37},
            {163, 31, 37},
            {162, 30, 37},
            {161, 29, 37},
            {160, 28, 38},
            {159, 27, 38},
            {158, 26, 38},
            {157, 25, 38},
            {156, 24, 39},
            {155, 23, 39},
            {154, 22, 39},
            {153, 21, 39},
            {152, 21, 39},
            {151, 20, 40},
            {149, 19, 40},
            {148, 19, 40},
            {147, 18, 40},
            {146, 17, 40},
            {145, 17, 41},
            {144, 16, 41},
            {142, 16, 41},
            {141, 16, 41},
            {140, 15, 41},
            {139, 15, 41},
            {137, 15, 41},
            {136, 15, 41},
            {135, 14, 41},
            {133, 14, 41},
            {132, 14, 41},
            {131, 14, 41},
            {129, 14, 41},
            {128, 14, 41},
            {127, 14, 41},
            {125, 14, 41},
            {124, 14, 41},
            {123, 14, 41},
            {121, 14, 41},
            {120, 14, 40},
            {119, 14, 40},
            {117, 14, 40},
            {116, 14, 40},
            {115, 14, 39},
            {113, 14, 39},
            {112, 14, 39},
            {111, 14, 38},
            {109, 14, 38},
            {108, 15, 38},
            {107, 15, 37},
            {105, 15, 37},
            {104, 15, 37},
            {103, 15, 36},
            {101, 15, 36},
            {100, 14, 35},
            {99, 14, 35},
            {97, 14, 34},
            {96, 14, 34},
            {95, 14, 33},
            {93, 14, 33},
            {92, 14, 33},
            {91, 14, 32},
            {90, 14, 31},
            {88, 14, 31},
            {87, 14, 30},
            {86, 14, 30},
            {84, 13, 29},
            {83, 13, 29},
            {82, 13, 28},
            {81, 13, 28},
            {79, 13, 27},
            {78, 13, 26},
            {77, 12, 26},
            {75, 12, 25},
            {74, 12, 25},
            {73, 12, 24},
            {72, 11, 23},
            {70, 11, 23},
            {69, 11, 22},
            {68, 11, 22},
            {67, 10, 21},
            {65, 10, 20},
            {64, 10, 20},
            {63, 10, 19},
            {61, 9, 18},
            {60, 9, 18}}

    End Class

End Namespace