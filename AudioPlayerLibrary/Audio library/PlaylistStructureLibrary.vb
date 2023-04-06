Public NotInheritable Class PlaylistStructureLibrary

#Region " Helper class "

    Private Class StructureDataDuringCalc

        Public Property GlobalParallelCount As Integer

        Public Property ParIndex As Integer

        Public Property LastMainAction As IPlayerAction

        Public Property LastMainProducer As ISoundProducer

        Public Property LastParallelProducer As ISoundProducer

        Public Property MainProducerLine As New List(Of ISoundProducer)

        Public Property LastParallelAction As IPlayerAction

    End Class

#End Region


#Region " API "

    ''' <summary>
    ''' Get the first playable on the main line producer.
    ''' </summary>
    Public Shared Function GetFirstAction(list As IEnumerable(Of IPlayerAction)) As IPlayerAction
        If list Is Nothing Then Return Nothing

        Return list.
               Where(Function(act) act.ExecutionType <> ExecutionTypes.Parallel AndAlso
                                   act.CanExecute AndAlso
                                   act.IsEnabled AndAlso
                                   TypeOf act Is ISoundProducer
               ).FirstOrDefault()
    End Function


    ''' <summary>
    ''' Collect global parallel actions from the playlist.
    ''' </summary>
    Public Shared Function GetGlobalParallels(list As IEnumerable(Of IPlayerAction)) As List(Of IPlayerAction)
        Dim globalParallelCount = 0
        Dim res As New List(Of IPlayerAction)()

        If list Is Nothing Then Return res

        For Each act In list.TakeWhile(Function(a) a.ExecutionType = ExecutionTypes.Parallel OrElse Not a.CanExecute OrElse Not a.IsEnabled)
            act.HasParallelIndex = False
            If Not act.CanExecute Then Continue For
            If Not act.IsEnabled Then Continue For

            act.IsGlobalParallel = True
            res.Add(act)
            If act.DelayType <> DelayTypes.Manual Then Continue For

            globalParallelCount += 1
            act.ParallelIndex = globalParallelCount
            act.HasParallelIndex = True
        Next

        Return res
    End Function


    ''' <summary>
    ''' Go through list items and adjust their structure-related properties.
    ''' </summary>
    Public Shared Function ArrangeStructure(list As ICollection(Of IPlayerAction)) As PlaylistStructureData
        ' Clear the items
        ClearStructureInfo(list)

        ' Process the global parallels
        Dim parallels = GetGlobalParallels(list)

        ' Now, go through all items.
        ' Set parallels tree and parent, effects tree, reference, NextAction, ParallelIndex.
        Dim globalIndex As Integer = 1
        Dim cache = New StructureDataDuringCalc With {
            .GlobalParallelCount = parallels.Where(Function(a) a.HasParallelIndex).Count()
        }

        For Each act In list
            act.Index = globalIndex
            act.HasParallelIndex = False
            globalIndex += 1

            If Not act.IsEnabled Then Continue For
            If Not act.CanExecute Then Continue For

            If act.ExecutionType = ExecutionTypes.Parallel Then
                ' Parallel action
                ArrangeParallelAction(act, cache)
            Else
                ' Main action
                ArrangeMainAction(act, cache)
            End If
        Next

        Dim data As New PlaylistStructureData With {
            .MaxActions = list.Count,
            .GlobalParallelList = parallels,
            .GlobalParallelCount = cache.GlobalParallelCount,
            .MaxParallels = If(list.Any(), list.Max(Function(a) a.ParallelIndex), 0)
        }

        Return data
    End Function

#End Region


#Region " Structure utility "

    ''' <summary>
    ''' Clear up items, in <see cref="ArrangeStructure"/> they are only added to.
    ''' </summary>
    Private Shared Sub ClearStructureInfo(list As IEnumerable(Of IPlayerAction))
        For Each act In list
            Dim asProducer = TryCast(act, ISoundProducer)
            Dim asEffect = TryCast(act, ISoundAutomation)

            act.IsGlobalParallel = False
            act.ParallelParent = Nothing
            act.Parallels.Clear()

            If asProducer IsNot Nothing Then
                asProducer.Effects.Clear()
                asProducer.GeneratedEffects.Clear()
            ElseIf asEffect IsNot Nothing Then
                asEffect.TargetList.Clear()
            End If
        Next
    End Sub


    ''' <summary>
    ''' Adjust main action, add it to the structure cache.
    ''' </summary>
    Private Shared Sub ArrangeMainAction(act As IPlayerAction, cache As StructureDataDuringCalc)
        ' Set Next for the previous main action to this main action
        If cache.LastMainAction IsNot Nothing Then
            cache.LastMainAction.NextAction = act
        End If

        ' Initialize parallel-line-related properties
        cache.ParIndex = cache.GlobalParallelCount

        ' Find a reference
        Select Case act.DelayReference
            Case DelayReferences.LastAction
                act.ReferenceAction = cache.LastMainAction

            Case DelayReferences.LastProducer
                act.ReferenceAction = cache.LastMainProducer

            Case Else
                ' Manual start or clock reference
                act.ReferenceAction = Nothing
        End Select

        ' Then add to the list and modify the reference points
        Dim asProducer = TryCast(act, ISoundProducer)
        Dim asEffect = TryCast(act, ISoundAutomation)

        If asProducer IsNot Nothing Then
            AmendMainProducerLine(asProducer, cache)
            cache.LastMainProducer = asProducer

        ElseIf asEffect IsNot Nothing AndAlso cache.LastMainProducer IsNot Nothing Then
            SetEffectTargets(asEffect, cache)
        End If

        cache.LastParallelAction = Nothing
        cache.LastParallelProducer = Nothing
        cache.LastMainAction = act
    End Sub


    ''' <summary>
    ''' Put a parallel action into the structure.
    ''' </summary>
    Private Shared Sub ArrangeParallelAction(act As IPlayerAction, cache As StructureDataDuringCalc)
        ' The last action on the main line is it's parent
        act.ParallelParent = cache.LastMainAction

        ' It's a child of that parent
        If cache.LastMainAction IsNot Nothing Then
            cache.LastMainAction.Parallels.Add(act)
        End If

        ' If the action cannot be executed, this is where processing end
        act.HasParallelIndex = False
        If Not act.CanExecute Then Return
        If Not act.IsEnabled Then Return

        ' Find a reference
        Select Case act.DelayReference
            Case DelayReferences.LastAction
                act.ReferenceAction = If(cache.LastParallelAction, cache.LastMainAction)

            Case DelayReferences.LastProducer
                act.ReferenceAction = If(cache.LastParallelProducer, cache.LastMainProducer)

            Case Else
                ' Manual start, wall clock or playlist clock
                act.ReferenceAction = Nothing
        End Select

        ' Then add to the list and modify the running references
        Dim asProducer = TryCast(act, ISoundProducer)
        Dim asEffect = TryCast(act, ISoundAutomation)

        If asProducer IsNot Nothing Then
            cache.LastParallelProducer = asProducer

        ElseIf asEffect IsNot Nothing Then
            SetEffectTargets(asEffect, cache)
        End If

        cache.LastParallelAction = act

        ' Set parallel index for manual starts
        If act.DelayType <> DelayTypes.Manual Then Return

        cache.ParIndex += 1
        act.ParallelIndex = cache.ParIndex
        act.HasParallelIndex = True
    End Sub


    ''' <summary>
    ''' Build a line of possibly simultaneous main-line producers, for applied effects.
    ''' </summary>
    Private Shared Sub AmendMainProducerLine(act As ISoundProducer, cache As StructureDataDuringCalc)
        If act.ExecutionType <> ExecutionTypes.MainContinuePrev Then
            cache.MainProducerLine.Clear()
        End If

        cache.MainProducerLine.Add(act)
    End Sub


    ''' <summary>
    ''' This is a sound effect, adjust effect targets.
    ''' </summary>
    Private Shared Sub SetEffectTargets(act As ISoundAutomation, cache As StructureDataDuringCalc)
        ' No presets for global parallels
        If act.IsGlobalParallel Then Return

        ' Set up effect targets for main line
        If act.EffectTargetMain = EffectTargets.Last AndAlso cache.LastMainProducer IsNot Nothing Then
            cache.LastMainProducer.Effects.Add(act)
            act.TargetList.Add(cache.LastMainProducer)

        ElseIf act.EffectTargetMain = EffectTargets.All Then
            For Each main In cache.MainProducerLine
                main.Effects.Add(act)
                act.TargetList.Add(main)
            Next
        End If

        ' Set up effect targets for parallel line
        If act.EffectTargetParallel = EffectTargets.Last AndAlso cache.LastParallelProducer IsNot Nothing Then
            cache.LastParallelProducer.Effects.Add(act)
            act.TargetList.Add(cache.LastParallelProducer)

        ElseIf act.EffectTargetParallel = EffectTargets.All Then
            For Each parProducer In cache.LastMainAction.Parallels.OfType(Of ISoundProducer)()
                parProducer.Effects.Add(act)
                act.TargetList.Add(parProducer)
            Next
        End If
    End Sub

#End Region

End Class
