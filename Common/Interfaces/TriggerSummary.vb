Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' For showing trigger information to the user.
''' </summary>
<CLSCompliant(True)>
Public Class TriggerSummary

    ''' <summary>
    ''' Name of the next action.
    ''' </summary>
    ''' <remarks>
    ''' The actual IPlayerAction interface cannot be used, as it is defined
    ''' in a higher-level library.
    ''' </remarks>
    Public Property NextAction As String


    ''' <summary>
    ''' Time when the trigger shall fire, can be absolute or playlist.
    ''' </summary>
    Public Property NextTime As Date


    ''' <summary>
    ''' For debugging.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return NextAction
    End Function

End Class
