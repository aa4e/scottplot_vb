Imports System.Linq
Imports System.Reflection
Imports System.Runtime.Serialization

Namespace ScottPlot

    Public Module Style

        Public ReadOnly Property Black As Styles.IStyle = New Styles.Black()
        Public ReadOnly Property Blue1 As Styles.IStyle = New Styles.Blue1()
        Public ReadOnly Property Blue2 As Styles.IStyle = New Styles.Blue2()
        Public ReadOnly Property Blue3 As Styles.IStyle = New Styles.Blue3()
        Public ReadOnly Property Burgundy As Styles.IStyle = New Styles.Burgundy()
        Public ReadOnly Property Control As Styles.IStyle = New Styles.Control()
        Public ReadOnly Property [Default] As Styles.IStyle = New Styles.Default()
        Public ReadOnly Property Gray1 As Styles.IStyle = New Styles.Gray1()
        Public ReadOnly Property Gray2 As Styles.IStyle = New Styles.Gray2()
        Public ReadOnly Property Hazel As Styles.IStyle = New Styles.Hazel()
        Public ReadOnly Property Light1 As Styles.IStyle = New Styles.Light1()
        Public ReadOnly Property Light2 As Styles.IStyle = New Styles.Light2()
        Public ReadOnly Property Monospace As Styles.IStyle = New Styles.Monospace()
        Public ReadOnly Property Pink As Styles.IStyle = New Styles.Pink()
        Public ReadOnly Property Seaborn As Styles.IStyle = New Styles.Seaborn()

        ''' <summary>
        ''' Return an array containing every available style.
        ''' </summary>
        Public Function GetStyles() As Styles.IStyle()
            Return Assembly.GetExecutingAssembly() _
                .GetTypes() _
                .Where(Function(x) x.IsClass) _
                .Where(Function(x) Not x.IsAbstract) _
                .Where(Function(x) x.GetInterfaces().Contains(GetType(Styles.IStyle))) _
                .Select(Function(x) DirectCast(FormatterServices.GetUninitializedObject(x), Styles.IStyle)) _
                .ToArray()
        End Function

    End Module

End Namespace