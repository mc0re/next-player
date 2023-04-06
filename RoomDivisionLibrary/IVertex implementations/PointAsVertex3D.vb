Imports System.Diagnostics.CodeAnalysis
Imports Common
Imports MIConvexHull


<CLSCompliant(True)>
Public Class PointAsVertex3D(Of TCookie)
	Implements IVertex, IPoint3D, IEquatable(Of PointAsVertex3D(Of TCookie))

#Region " IVertex.Position read-only property "

	Public ReadOnly Property Position As Double() Implements IVertex.Position

#End Region


#Region " Point read-only property "

	''' <summary>
	''' The actual point.
	''' </summary>
	Public ReadOnly Property Point As IPoint3D

#End Region


#Region " IPoint3D properties "


	Public ReadOnly Property X As Double Implements IPoint3D.X
		Get
			Return Point.X
		End Get
	End Property


	Public ReadOnly Property Y As Double Implements IPoint3D.Y
		Get
			Return Point.Y
		End Get
	End Property


	Public ReadOnly Property Z As Double Implements IPoint3D.Z
		Get
			Return Point.Z
		End Get
	End Property

#End Region


#Region " Cookie read-only property "

	Public ReadOnly Property Ref As TCookie

#End Region


#Region " IEquatable implementation "

	Public Shadows Function Equals(other As PointAsVertex3D(Of TCookie)) As Boolean Implements IEquatable(Of PointAsVertex3D(Of TCookie)).Equals
		Return Point3DHelper.IsSame(Me, other)
	End Function

#End Region


#Region " Init and clean-up "

	Friend Sub New(point As IPoint3D, payload As TCookie)
		Ref = payload
		Me.Point = point
		Position = New Double() {point.X, point.Y, point.Z}
	End Sub

#End Region


#Region " ToString "

	<ExcludeFromCodeCoverage>
	Public Overrides Function ToString() As String
		Return String.Format("{0}: {1}", Point3DHelper.Create(Me), Ref)
	End Function

#End Region

End Class
