Imports System.Diagnostics.CodeAnalysis
Imports Common


''' <summary>
''' Information about a single WASAPI device.
''' </summary>
Public Class WasapiInterfaceInfo
	Inherits PropertyChangedHelper

#Region " Id notifying property "

	Private mId As String


	Public Property Id As String
		Get
			Return mId
		End Get
		Set(value As String)
			SetField(mId, value, NameOf(Id))
		End Set
	End Property

#End Region


#Region " Name notifying property "

	Private mName As String


	Public Property Name As String
		Get
			Return mName
		End Get
		Set(value As String)
			SetField(mName, value, NameOf(Name))
		End Set
	End Property

#End Region


#Region " Icon notifying property "

	Private mIcon As String


	Public Property Icon As String
		Get
			Return mIcon
		End Get
		Set(value As String)
			SetField(mIcon, value, NameOf(Icon))
		End Set
	End Property

#End Region


#Region " IsDefault notifying property "

	Private mIsDefault As Boolean


	Public Property IsDefault As Boolean
		Get
			Return mIsDefault
		End Get
		Set(value As Boolean)
			SetField(mIsDefault, value, NameOf(IsDefault))
		End Set
	End Property

#End Region


#Region " ToString "

	<ExcludeFromCodeCoverage>
	Public Overrides Function ToString() As String
		Return Name
	End Function

#End Region

End Class
