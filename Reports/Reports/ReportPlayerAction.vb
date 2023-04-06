Imports AudioPlayerLibrary
Imports PlayerActions


''' <summary>
''' This is a wrapper class to make report generator happy.
''' </summary>
Public Class ReportPlayerAction
    Inherits ReportDataMirror(Of PlayerAction)

#Region " PlayerAction mirror properties "

    Public Property IsEnabled As Boolean

    Public Property Name As String

    Public Property ExecutionType As String ' ExecutionTypes

    Public Property DelayType As String ' DelayTypes

    Public Property DelayReference As String ' DelayReferences

    ' Do not use directly in the report
    Public Property DelayReferenceAction As IPlayerAction

    Public Property DelayReferenceName As String

    Public Property DelayReferenceType As String

    Public Property DelayReferenceShortFileName As String

    Public Property DelayBefore As TimeSpan

    Public Property HasDuration As Boolean

    Public Property Duration As TimeSpan

    Public Property Index As Integer

    Public Property CanExecute As Boolean

    Public Property ActionType As String

    Public Property NextAction As IPlayerAction

    Public Property ParallelParent As IPlayerAction

    Public Property Parallels As IList(Of IPlayerAction)

    Public Property ParallelIndex As Integer

#End Region


#Region " PlayerActionFile mirror properties "

    Public Property IsMuted As Boolean

    Public Property Volume As Double

    Public Property Balance As Double

    Public Property StartPosition As TimeSpan

    Public Property StopPosition As TimeSpan

    Public Property Effects As IList(Of ISoundAutomation)

    Public Property ShortFileName As String

    Public Property AbsFileToPlay As String

    Public Property IsLoadingFailed As Boolean

#End Region


#Region " PlayerActionEffect mirror properties "

    Public Property OperationType As String ' EffectOperationTypes

    Public Property TargetList As IList(Of ISoundProducer)

    Public Property AutomationPointList As AutomationPointCollection

#End Region


#Region " PlayerActionPowerPoint mirror properties "

    Public Property SetSlideAction As String ' SetSlideActions

    Public Property SlideIndex As Integer

#End Region


#Region " PlayerActionComment mirror properties "

    Public Property Description As String

#End Region


#Region " PlayerActionText mirror properties "

    Public Property Text As String

    Public Property Channel As Integer

#End Region


#Region " Constructors "

    Private Shared sIsMapAdded As Boolean


    Public Sub New(orig As PlayerAction)
        If Not sIsMapAdded Then
            AddPropertyMap(Function() DelayReferenceType,
                           Function(src) If(src.ReferenceAction IsNot Nothing, CType(src.ReferenceAction, PlayerAction).ActionType, String.Empty))
            AddPropertyMap(Function() DelayReferenceShortFileName,
                           Function(src) If(src.ReferenceAction IsNot Nothing AndAlso TypeOf src.ReferenceAction Is PlayerActionFile,
                                            CType(src.ReferenceAction, PlayerActionFile).ShortFileName, String.Empty))
            sIsMapAdded = True
        End If

        CopyFields(orig)
    End Sub

#End Region

End Class
