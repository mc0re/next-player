Imports AudioPlayerLibrary


''' <summary>
''' Holds temporary data for structure arrangement.
''' </summary>
Class ArrangeStructureData

#Region " GlobalParallelList property "

	Private mGlobalParallelList As IEnumerable(Of IPlayerAction)


	''' <summary>
	''' When setting, initializes other properties.
	''' </summary>
	Public Property GlobalParallelList As IEnumerable(Of IPlayerAction)
		Get
			Return mGlobalParallelList
		End Get
		Set(value As IEnumerable(Of IPlayerAction))
			mGlobalParallelList = value.ToList()
			Dim cnt = mGlobalParallelList.Count()
			mGlobalParallelCount = cnt
			ParIndex = cnt
		End Set
	End Property

#End Region


#Region " GlobalParallelCount property "

	Private mGlobalParallelCount As Integer


	Public ReadOnly Property GlobalParallelCount As Integer
		Get
			Return mGlobalParallelCount
		End Get
	End Property

#End Region


#Region " ParIndex property "

	Private mParIndex As Integer


	Public Property ParIndex As Integer
		Get
			Return mParIndex
		End Get
		Set(value As Integer)
			mParIndex = value
			If MaxParallels < value Then
				mMaxParallels = value
			End If
		End Set
	End Property

#End Region


#Region " MaxParallels read-only property "

	Private mMaxParallels As Integer


	Public ReadOnly Property MaxParallels As Integer
		Get
			Return mMaxParallels
		End Get
	End Property

#End Region


#Region " Public fields "

	Public LastMainAction As IPlayerAction

	Public LastMainProducer As ISoundProducer

	Public LastParallelAction As IPlayerAction

	Public LastParallelProducer As ISoundProducer

	Public ReadOnly MainProducerLine As IList(Of ISoundProducer) = New List(Of ISoundProducer)

#End Region


#Region " Helper read-only properties "

	''' <summary>
	''' Get a reference producer action for a parallel action -
	''' parallel if exists, main if does not.
	''' </summary>
	Public ReadOnly Property LastProducer As IPlayerAction
		Get
			Return If(LastParallelProducer IsNot Nothing, LastParallelProducer, LastMainProducer)
		End Get
	End Property


	''' <summary>
	''' Get a reference action for a parallel action -
	''' last parallel action if exists, last main if does not.
	''' </summary>
	Public ReadOnly Property LastAction As IPlayerAction
		Get
			Return If(LastParallelAction IsNot Nothing, LastParallelAction, LastMainAction)
		End Get
	End Property

#End Region

End Class