Imports AudioPlayerLibrary


Friend MustInherit Class TestAction
    Implements IPlayerAction

#Region " Fields "

    Private mStartAction As Action

    Private mStopAction As Action

#End Region


#Region " Properties "

    Public Property Duration As TimeSpan Implements IDurationElement.Duration

    Public Property HasDuration As Boolean Implements IDurationElement.HasDuration

    Public Property CanExecute As Boolean = True Implements IPlayerAction.CanExecute

    Public Property DelayBefore As TimeSpan Implements IPlayerAction.DelayBefore

    Public Property Index As Integer Implements IPlayerAction.Index

    Public Property IsGlobalParallel As Boolean Implements IPlayerAction.IsGlobalParallel

    Public Property IsSaveNeeded As Boolean Implements IPlayerAction.IsSaveNeeded

    Public Property DelayReference As DelayReferences Implements IPlayerAction.DelayReference

    Public Property ReferenceAction As IPlayerAction Implements IPlayerAction.ReferenceAction

    Public Property DelayType As DelayTypes Implements IPlayerAction.DelayType

    Public Property ExecutionType As ExecutionTypes Implements IPlayerAction.ExecutionType

    Public Property IsActive As Boolean Implements IPlayerAction.IsActive

    Public Property IsEnabled As Boolean = True Implements IPlayerAction.IsEnabled

    Public Property IsPlaying As Boolean Implements IPlayerAction.IsPlaying

    Public Property Name As String Implements IPlayerAction.Name

    Public Property PlayPosition As TimeSpan Implements IPlayerAction.PlayPosition

    Public ReadOnly Property StartPosition As TimeSpan Implements IPlayerAction.StartPosition
        Get
            Return TimeSpan.Zero
        End Get
    End Property

    Public ReadOnly Property StopPosition As TimeSpan Implements IPlayerAction.StopPosition
        Get
            Return TimeSpan.Zero
        End Get
    End Property

    Public Property IsActiveParallel As Boolean Implements IPlayerAction.IsActiveParallel

    Public Property ParallelParent As IPlayerAction Implements IPlayerAction.ParallelParent

    Public Property ParallelIndex As Integer Implements IPlayerAction.ParallelIndex

    Public Property HasParallelIndex As Boolean Implements IPlayerAction.HasParallelIndex

    Public Property IsNext As Boolean Implements IPlayerAction.IsNext

    Public Property NextAction As IPlayerAction Implements IPlayerAction.NextAction

    Public Property StartTime As Double Implements IPlayerAction.StartTime

    Public Property Parallels As IList(Of IPlayerAction) = New List(Of IPlayerAction)() Implements IPlayerAction.Parallels

#End Region


#Region " Init and clean-up "

    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub


    Public Sub New(startAction As Action, stopAction As Action)
        mStartAction = startAction
        mStopAction = stopAction
    End Sub

#End Region


#Region " API "

    Public Sub PrepareStart() Implements IPlayerAction.PrepareStart
        IsPlaying = True
    End Sub


    Public Sub Start() Implements IPlayerAction.Start
        If mStartAction IsNot Nothing Then mStartAction()
    End Sub


    Public Sub [Stop](intendedResume As Boolean) Implements IPlayerAction.Stop
        IsPlaying = False
        If mStopAction IsNot Nothing Then mStopAction()
    End Sub


    Public Sub UpdatePlayPosition() Implements IPlayerAction.UpdatePlayPosition

    End Sub


    Public Sub ModifyVolume(delta As Single) Implements IPlayerAction.ModifyVolume

    End Sub

#End Region

End Class
