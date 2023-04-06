<Serializable()>
Public Class WindowPosition
	Inherits PropertyChangedHelper

#Region " Left notifying property "

	Private mLeft As Double


	Public Property Left As Double
		Get
			Return mLeft
		End Get
		Set(value As Double)
			SetField(mLeft, value, Function() Left)
		End Set
	End Property

#End Region


#Region " Top notifying property "

	Private mTop As Double


	Public Property Top As Double
		Get
			Return mTop
		End Get
		Set(value As Double)
			SetField(mTop, value, Function() Top)
		End Set
	End Property

#End Region


#Region " Width notifying property "

	Private mWidth As Double


	Public Property Width As Double
		Get
			Return mWidth
		End Get
		Set(value As Double)
			SetField(mWidth, value, Function() Width)
		End Set
	End Property

#End Region


#Region " Height notifying property "

	Private mHeight As Double


	Public Property Height As Double
		Get
			Return mHeight
		End Get
		Set(value As Double)
			SetField(mHeight, value, Function() Height)
		End Set
	End Property

#End Region

End Class
