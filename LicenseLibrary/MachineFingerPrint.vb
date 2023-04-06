Imports System.Management
Imports System.Security.Cryptography
Imports System.Text


''' <summary>
''' Stores the machine ID.
''' </summary>
''' <remarks>
''' External storage is done as a string.
''' Comparison takes care of a single component replacement.
''' </remarks>
Public Class MachineFingerPrint

#Region " Constants "

	Private Const Separator As Char = ":"c

#End Region


#Region " Private properties "

	Private Property CpuId As String


	Private Property MotherboardId As String


	Private Property VideoId As String


	Private Property DiskId As String

#End Region


#Region " Value public property "

	Private mFingerPrint As String


	''' <summary>
	''' Create a concatenated ID.
	''' </summary>
	''' <remarks>
	''' The order must match <see cref="Create"/>
	''' </remarks>
	Public ReadOnly Property FingerPrint As String
		Get
			If String.IsNullOrEmpty(mFingerPrint) Then
				mFingerPrint = String.Join(Separator, {CpuId, MotherboardId, VideoId, DiskId})
			End If

			Return mFingerPrint
		End Get
	End Property

#End Region


#Region " Init and clean-up "

	''' <summary>
	''' Create an instance, reflecting the current machine properties.
	''' </summary>
	Private Sub New(fillProperties As Boolean)
		If Not fillProperties Then Return

		CpuId = GetHash(GetCpuId())
		MotherboardId = GetHash(GetMotherboardId())
		VideoId = GetHash(GetVideoId())
		DiskId = GetHash(GetDiskId())
	End Sub


	''' <summary>
	''' Create a fingerprint from the given string.
	''' </summary>
	''' <remarks>
	''' The order must match <see cref="FingerPrint"/>
	''' </remarks>
	Public Shared Function Create(fingerPrint As String) As MachineFingerPrint
		Dim res As New MachineFingerPrint(False)

		Dim parts = fingerPrint.Split(Separator)
		If parts.Count > 0 Then res.CpuId = parts(0)
		If parts.Count > 1 Then res.MotherboardId = parts(1)
		If parts.Count > 2 Then res.VideoId = parts(2)
		If parts.Count > 3 Then res.DiskId = parts(3)

		Return res
	End Function

#End Region


#Region " Singleton for the current machine ID "


    ''' <summary>
    ''' Get fingerprint of the current machine.
    ''' </summary>
    Public Shared ReadOnly Property MachineInstance As MachineFingerPrint = New MachineFingerPrint(True)


#End Region


#Region " Verification "

    ''' <summary>
    ''' Compare the two fingerprints.
    ''' At most one of the components may mismatch.
    ''' </summary>
    ''' <returns>True if the machine ID matches</returns>
    Public Function IsMatching(other As MachineFingerPrint) As Boolean
		Dim count = 0

		If CpuId = other.CpuId Then count += 1
		If MotherboardId = other.MotherboardId Then count += 1
		If VideoId = other.VideoId Then count += 1
		If DiskId = other.DiskId Then count += 1

		Return count >= 3
	End Function


	''' <summary>
	''' Compare the two fingerprints.
	''' At most one of the components may mismatch.
	''' </summary>
	''' <returns>True if the machine ID matches</returns>
	Public Function IsMatching(other As String) As Boolean
		Return IsMatching(Create(other))
	End Function

#End Region


#Region " GetHash methods "

	''' <summary>
	''' Generates a 16 byte Unique Identification code of a given string.
	''' Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
	''' </summary>
	Private Shared Function GetHash(s As String) As String
		Dim sec As HashAlgorithm = New MD5CryptoServiceProvider()
		Dim enc As New ASCIIEncoding()
		Dim bt As Byte() = enc.GetBytes(s)

		Return GetHexString(sec.ComputeHash(bt))
	End Function


	Private Shared Function GetHexString(bt As Byte()) As String
		Dim s As New StringBuilder()

		For i As Integer = 0 To bt.Length - 1
			Dim b As Byte = bt(i)
			Dim n As Integer, n1 As Integer, n2 As Integer
			n = CInt(b)
			n1 = n And 15
			n2 = (n >> 4) And 15

			If n2 > 9 Then
				s.Append(ChrW(n2 - 10 + AscW("A"c)))
			Else
				s.Append(n2.ToString())
			End If

			If n1 > 9 Then
				s.Append(ChrW(n1 - 10 + AscW("A"c)))
			Else
				s.Append(n1.ToString())
			End If

			If (i + 1) <> bt.Length AndAlso (i + 1) Mod 2 = 0 Then
				s.Append("-")
			End If
		Next

		Return s.ToString()
	End Function

#End Region


#Region " Device ID utility methods "

    ''' <summary>
    ''' Return a hardware identifier.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Design", "CC0004:Catch block cannot be empty", Justification:="<Pending>")>
	Private Shared Function Identifier(wmiClass As String, wmiProperty As String) As String
		Dim result = String.Empty
		Dim mc As New ManagementClass(wmiClass)
		Dim moc = mc.GetInstances()

		For Each mo In moc
			'Only get the first one
			If String.IsNullOrEmpty(result) Then
				Try
					Dim prop = mo.Item(wmiProperty)
					If prop Is Nothing Then Continue For
					result = prop.ToString()
					Exit For
				Catch ex As Exception
					' Ignore
				End Try
			End If
		Next

		Return result
	End Function

#End Region


#Region " Device ID utility methods "

	''' <summary>
	''' CPU identifier.
	''' </summary>
	Private Shared Function GetCpuId() As String
		' Uses first CPU identifier available in order of preference
		' Don't get all identifiers, as it is very time consuming
		Dim retVal As String = Identifier("Win32_Processor", "UniqueId")

		If String.IsNullOrEmpty(retVal) Then
			'If no UniqueID, use ProcessorID
			retVal = Identifier("Win32_Processor", "ProcessorId")
		End If

		If String.IsNullOrEmpty(retVal) Then
			'If no ProcessorId, use Name
			retVal = Identifier("Win32_Processor", "Name")
		End If

		If String.IsNullOrEmpty(retVal) Then
			'If no Name, use Manufacturer
			retVal = Identifier("Win32_Processor", "Manufacturer")
		End If

		'Add clock speed for extra security
		retVal += Identifier("Win32_Processor", "MaxClockSpeed")

		Return retVal
	End Function


	''' <summary>
	''' Motherboard ID.
	''' </summary>
	Private Shared Function GetMotherboardId() As String
		Return Identifier("Win32_BaseBoard", "Model") &
			   Identifier("Win32_BaseBoard", "Manufacturer") &
			   Identifier("Win32_BaseBoard", "Name") &
			   Identifier("Win32_BaseBoard", "SerialNumber")
	End Function


	''' <summary>
	''' Main physical hard drive ID.
	''' </summary>
	Private Shared Function GetDiskId() As String
		Return Identifier("Win32_DiskDrive", "Model") &
			   Identifier("Win32_DiskDrive", "Manufacturer") &
			   Identifier("Win32_DiskDrive", "Signature") &
			   Identifier("Win32_DiskDrive", "TotalHeads")
	End Function


	''' <summary>
	''' Primary video controller ID.
	''' </summary>
	Private Shared Function GetVideoId() As String
		Return Identifier("Win32_VideoController", "DriverVersion") &
			   Identifier("Win32_VideoController", "Name")
	End Function


#If 0 Then

	''' <summary>
	''' Return a hardware identifier.
	''' </summary>
	<CodeAnalysis.SuppressMessage("Design", "CC0004:Catch block cannot be empty", Justification:="<Pending>")>
	Private Shared Function Identifier(wmiClass As String, wmiProperty As String, wmiMustBeTrue As String) As String
		Dim result = String.Empty
		Dim mc As New ManagementClass(wmiClass)
		Dim moc = mc.GetInstances()

		For Each mo In From mo1 In moc.OfType(Of ManagementBaseObject)() Where mo1(wmiMustBeTrue).ToString() = "True"
			'Only get the first one
			If String.IsNullOrEmpty(result) Then
				Try
					result = mo(wmiProperty).ToString()
					Exit Try
				Catch ex As Exception
					' Ignore
				End Try
			End If
		Next

		Return result
	End Function


	''' <summary>
	''' First enabled network card ID.
	''' </summary>
	Private Shared Function GetMacId() As String
		Return Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled")
	End Function


	''' <summary>
	''' BIOS Identifier.
	''' </summary>
	<CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="These are WIN32 strings, not names")>
	Private Shared Function GetBiosId() As String
#Disable Warning CC0108 ' You should use nameof instead of the parameter element name string
		Return Identifier("Win32_BIOS", "Manufacturer") &
			   Identifier("Win32_BIOS", "SMBIOSBIOSVersion") &
			   Identifier("Win32_BIOS", "IdentificationCode") &
			   Identifier("Win32_BIOS", "SerialNumber") &
			   Identifier("Win32_BIOS", "ReleaseDate") &
			   Identifier("Win32_BIOS", "Version")
#Enable Warning CC0108 ' You should use nameof instead of the parameter element name string
	End Function

#End If

#End Region

End Class
