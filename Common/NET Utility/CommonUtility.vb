Imports System.Runtime.CompilerServices


Public Module CommonUtility

	Public Const MillisecondsInSecond = 1000


	Public Const SecondsInMinute = 60


	''' <summary>
	''' Get a value lying in between two values.
	''' </summary>
	Public Function LinearInterpolation(valueFrom As Double, valueTo As Double, progress As Double) As Double
		Return valueFrom + (valueTo - valueFrom) * progress
	End Function


	''' <summary>
	''' Check whether the sequence is empty.
	''' </summary>
	''' <returns>True if empty</returns>
	<Extension()>
	Public Function IsEmpty(this As IEnumerable) As Boolean
		If this Is Nothing Then Throw New ArgumentNullException(NameOf(this))

		Return Not this.GetEnumerator().MoveNext()
	End Function


	''' <summary>
	''' Check whether the sequence is empty.
	''' </summary>
	''' <returns>True if empty</returns>
	<Extension()>
	Public Function IsEmpty(this As IList) As Boolean
		If this Is Nothing Then Throw New ArgumentNullException(NameOf(this))

		Return this.Count = 0
	End Function

End Module
