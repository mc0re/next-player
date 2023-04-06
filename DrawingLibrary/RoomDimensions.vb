Imports Common


Public Class RoomDimensions

	Public ReadOnly RoomXNeg As Double

	Public ReadOnly RoomXPos As Double

	Public ReadOnly RoomYNeg As Double

	Public ReadOnly RoomYPos As Double

	Public ReadOnly RoomZNeg As Double

	Public ReadOnly RoomZPos As Double

	Public ReadOnly AudXNeg As Double

	Public ReadOnly AudXPos As Double

	Public ReadOnly AudYNeg As Double

	Public ReadOnly AudYPos As Double


	Public ReadOnly Property RoomXSize As Double
		Get
			Return RoomXPos - RoomXNeg
		End Get
	End Property


	Public ReadOnly Property RoomYSize As Double
		Get
			Return RoomYPos - RoomYNeg
		End Get
	End Property


	Public ReadOnly Property RoomZSize As Double
		Get
			Return RoomZPos - RoomZNeg
		End Get
	End Property


	Public ReadOnly Property RoomXCenter As Double
		Get
			Return RoomXNeg + RoomXSize / 2
		End Get
	End Property


	Public ReadOnly Property RoomYCenter As Double
		Get
			Return RoomYNeg + RoomYSize / 2
		End Get
	End Property


	Public ReadOnly Property RoomZCenter As Double
		Get
			Return RoomZNeg + RoomZSize / 2
		End Get
	End Property


	Public ReadOnly Property AudXSize As Double
		Get
			Return AudXPos - AudXNeg
		End Get
	End Property


	Public ReadOnly Property AudYSize As Double
		Get
			Return AudYPos - AudYNeg
		End Get
	End Property


	Public Sub New(room As Room3D)
		RoomXNeg = -room.XLeft
		RoomXPos = room.XRight
		RoomYNeg = -room.YBack
		RoomYPos = room.YFront
		RoomZNeg = -room.ZBelow
		RoomZPos = room.ZAbove
		AudXNeg = -room.AudienceLeft
		AudXPos = room.AudienceRight
		AudYNeg = -room.AudienceBack
		AudYPos = room.AudienceFront
	End Sub

End Class
