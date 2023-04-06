Imports System.Runtime.Serialization
Imports System.Globalization


<Serializable()>
Public Class ParseException
	Inherits Exception

#Region " Properties "

	Public Property Line As Integer

	Public Property Position As Integer

	Public Property Suggestion As String

#End Region


#Region " Init and clean-up "

	Public Sub New(errorMessage As String, suggestion As String, line As Integer, position As Integer)
		MyBase.New(errorMessage)
		Me.Line = line
		Me.Position = position
		Me.Suggestion = suggestion
	End Sub

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Return String.Format(CultureInfo.InvariantCulture, "Line {0}, position {1}.{2}{3}{2}{4}", Line, Position, vbCrLf, Message, Suggestion)
	End Function

#End Region


#Region " Standard constructors and overrides "

	Public Sub New()
		MyBase.New()
	End Sub


	Public Sub New(message As String)
		MyBase.New(message)
	End Sub


	Public Sub New(message As String, inner As Exception)
		MyBase.New(message, inner)
	End Sub


	Protected Sub New(info As SerializationInfo, context As StreamingContext)
		MyBase.New(info, context)
	End Sub


	Public Overrides Sub GetObjectData(info As SerializationInfo, context As StreamingContext)
		MyBase.GetObjectData(info, context)
	End Sub

#End Region

End Class
