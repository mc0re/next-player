Imports Common

Public Class Plane3DPair(Of TRef)

    Public Property Plane1 As Plane3D(Of TRef)

    Public Property Plane2 As Plane3D(Of TRef)

    Public Property Direction1To2 As Integer

    Public Property Direction2To1 As Integer

    Public Property DirectionAudTo1 As Integer

    Public Property DirectionAudTo2 As Integer

    Public Property References As List(Of TRef)

End Class
