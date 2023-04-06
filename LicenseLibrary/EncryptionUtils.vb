Imports System.Diagnostics.CodeAnalysis
Imports System.Security.Cryptography
Imports System.Reflection
Imports System.IO

Public Module EncryptionUtils

#Region "Private Constants"

    ''' <summary>Index of the private key header</summary>
    Private Const MagicPrivIdx As Integer = &H8

    ''' <summary>Index of the public key header</summary>
    Private Const MagicPubIdx As Integer = &H14

    ''' <summary>Header size</summary>
    Private Const MagicSize As Integer = 4

    ''' <summary>Block size used in encrypt/decrypt actions</summary>
    Private Const BlockSize As Integer = 64

#End Region


#Region "Public Methods"

#Region "Get RSA from SNK file"

    ''' <summary>
    ''' Returns RSA object from *.snk key file.
    ''' </summary>
    ''' <param name="keyStream">Stream containing SNK data.</param>
    ''' <returns>RSACryptoServiceProvider</returns>
    Friend Function GetRsaFromSnkFile(keyStream As Stream) As RSACryptoServiceProvider
        If keyStream Is Nothing Then
            Throw New ArgumentNullException(NameOf(keyStream))
        End If

        Dim snkBytes As Byte() = GetStreamBytes(keyStream)
        If snkBytes Is Nothing Then
            Throw New ArgumentException("Invalid SNK file.")
        End If

        Dim rsa As RSACryptoServiceProvider = GetRsaFromSnkBytes(snkBytes)

        Return rsa
    End Function

#End Region


#Region " Get RSA public key from assembly "

    ''' <summary>
    ''' Retrieves an RSA public key from a signed assembly
    ''' </summary>
    ''' <param name="assembly">Signed assembly to retrieve the key from</param>
    ''' <returns>RSA Crypto Service Provider initialised with the key from the assembly</returns>
    Public Function GetPublicKeyFromAssembly(assembly As Assembly) As RSACryptoServiceProvider
        If assembly Is Nothing Then
            Throw New ArgumentNullException(NameOf(assembly), "Assembly may not be null")
        End If

        Dim pubkey = assembly.GetName().GetPublicKey()
        If pubkey.Length = 0 Then
            Throw New ArgumentException("No public key in assembly.")
        End If

        Dim rsaParams As RSAParameters = GetRsaParameters(pubkey)
        Dim rsa As New RSACryptoServiceProvider()
        rsa.ImportParameters(rsaParams)

        Return rsa
    End Function

#End Region


#Region "Get RSA Parameters"

    ''' <summary>
    ''' Returns RSAParameters from byte[]. Example to get rsa public key from assembly:
    ''' byte[] pubkey = Assembly.GetExecutingAssembly().GetName().GetPublicKey();
    ''' RSAParameters p = SnkUtil.GetRSAParameters(pubkey);
    ''' </summary>
    ''' <param name="keyBytes">keypair loaded as a byte array</param>
    ''' <returns>Parameters for initialising an RSA Crypto Provider with the passed key information</returns>
    Private Function GetRsaParameters(keyBytes As Byte()) As RSAParameters
        Dim ret As New RSAParameters()

        If (keyBytes Is Nothing) OrElse (keyBytes.Length < 1) Then
            Throw New ArgumentNullException(NameOf(keyBytes))
        End If

        Dim pubonly As Boolean = SnkBufIsPubLength(keyBytes)

        If (pubonly) AndAlso (Not CheckRsa1(keyBytes)) Then
            Return ret
        End If

        If (Not pubonly) AndAlso (Not CheckRsa2(keyBytes)) Then
            Return ret
        End If

        Dim magicIdx As Integer = If(pubonly, MagicPubIdx, MagicPrivIdx)

        ' Bitlen is stored here, but note this class is only set up for 1024 bit length keys
        Dim bitlenIdx As Integer = magicIdx + MagicSize
        Const bitlenSize As Integer = 4
        ' DWORD
        ' Exponent, In read file, will usually be { 1, 0, 1, 0 } or 65537
        Dim expIdx As Integer = bitlenIdx + bitlenSize
        Const expSize As Integer = 4

        'BYTE modulus[rsapubkey.bitlen/8]; == MOD; Size 128
        Dim modIdx As Integer = expIdx + expSize
        Const modSize As Integer = 128

        'BYTE prime1[rsapubkey.bitlen/16]; == P; Size 64
        Dim pIdx As Integer = modIdx + modSize
        Const pSize As Integer = 64

        'BYTE prime2[rsapubkey.bitlen/16]; == Q; Size 64
        Dim qIdx As Integer = pIdx + pSize
        Const qSize As Integer = 64

        'BYTE exponent1[rsapubkey.bitlen/16]; == DP; Size 64
        Dim dpIdx As Integer = qIdx + qSize
        Const dpSize As Integer = 64

        'BYTE exponent2[rsapubkey.bitlen/16]; == DQ; Size 64
        Dim dqIdx As Integer = dpIdx + dpSize
        Const dqSize As Integer = 64

        'BYTE coefficient[rsapubkey.bitlen/16]; == InverseQ; Size 64
        Dim invqIdx As Integer = dqIdx + dqSize
        Const invqSize As Integer = 64

        'BYTE privateExponent[rsapubkey.bitlen/8]; == D; Size 128
        Dim dIdx As Integer = invqIdx + invqSize
        Const dSize As Integer = 128

        ' Figure public params, Must reverse order (little vs. big endian issue)
        ret.Exponent = BlockCopy(keyBytes, expIdx, expSize)
        Array.Reverse(ret.Exponent)
        ret.Modulus = BlockCopy(keyBytes, modIdx, modSize)
        Array.Reverse(ret.Modulus)

        If pubonly Then
            Return ret
        End If

        ' Figure private params
        ' Must reverse order (little vs. big endian issue)
        ret.P = BlockCopy(keyBytes, pIdx, pSize)
        Array.Reverse(ret.P)

        ret.Q = BlockCopy(keyBytes, qIdx, qSize)
        Array.Reverse(ret.Q)

        ret.DP = BlockCopy(keyBytes, dpIdx, dpSize)
        Array.Reverse(ret.DP)

        ret.DQ = BlockCopy(keyBytes, dqIdx, dqSize)
        Array.Reverse(ret.DQ)

        ret.InverseQ = BlockCopy(keyBytes, invqIdx, invqSize)
        Array.Reverse(ret.InverseQ)

        ret.D = BlockCopy(keyBytes, dIdx, dSize)
        Array.Reverse(ret.D)

        Return ret
    End Function

#End Region


#Region "Asymmetric Encrypt"

    ''' <summary>
    ''' Encrypts a given byte array using the public key lifted from the passed assembly
    ''' </summary>
    ''' <param name="assembly">Assembly to retrieve the public key from</param>
    ''' <param name="dataToEncrypt">Data to be encrypted with the passed public key</param>
    ''' <returns>Encrypted data as a byte array, null if either argument is null</returns>
    <SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="Style is not a name")>
    <SuppressMessage("Style", "CC0014:Use Ternary operator.", Justification:="If is more readable here")>
    Public Function AsymmetricEncrypt(assembly As Assembly, dataToEncrypt As Byte()) As Byte()
        If assembly Is Nothing OrElse dataToEncrypt Is Nothing Then
            Return Nothing
        End If

        'Load the keys from the assembly
        Dim cipher As RSACryptoServiceProvider = GetPublicKeyFromAssembly(assembly)

        'Prepare variables used in encryption
        Dim localBlockSize = BlockSize
        Dim index = 0
        Dim bytesLeft = dataToEncrypt.Length
        Dim totalBytes As Integer

        'Ensure that the total bytes is a multiple of 128, the final encrypted content block will 
        'be padded to this length in bytes
        If (bytesLeft * 2) Mod 128 <> 0 Then
            totalBytes = (bytesLeft * 2) + (128 - ((bytesLeft * 2) Mod 128))
        Else
            totalBytes = bytesLeft * 2
        End If

        'Create the buffer for encrypted data
        Using memEncryptedTextbuffer As New MemoryStream(totalBytes)
            Dim block As Byte()
            Dim encryptedBlock As Byte()

            'Split the serialized data into smaller blocks for encryption
            While bytesLeft > 0
                'If the blocksize is too large, set it to the required amount
                If bytesLeft < localBlockSize Then
                    localBlockSize = bytesLeft
                End If

                'Get a block from the serialized license data and encrypt it
                block = BlockCopy(dataToEncrypt, index, localBlockSize)
                encryptedBlock = cipher.Encrypt(block, False)
                memEncryptedTextbuffer.Write(encryptedBlock, 0, encryptedBlock.Length)

                'Update position and size tracking
                index += localBlockSize
                bytesLeft -= localBlockSize
            End While
            cipher.Clear()

            'Dump the encrypted data to the caller
            memEncryptedTextbuffer.Position = 0
            Return memEncryptedTextbuffer.ToArray()
        End Using
    End Function

#End Region


#Region "Asymmetric Decrypt"

    ''' <summary>
    ''' Asymmetrically decrypts a given byte array using the keys from a strong named keyfile
    ''' passed as a byte array
    ''' </summary>
    ''' <param name="encryptedData">Data to be decrypted</param>
    ''' <param name="snkFileContent">Content of the keyfile</param>
    ''' <returns>Byte array of decrypted data</returns>
    Public Function AsymmetricDecrypt(encryptedData As Byte(), snkFileContent As Byte()) As Byte()
        Dim cipher As RSACryptoServiceProvider = GetRsaFromSnkBytes(snkFileContent)
        Return DoAsymmetricDecrypt(cipher, encryptedData)
    End Function


    ''' <summary>
    ''' Asymmetrically decrypts a given byte array using the keys taken from the provided
    ''' strong name key file path
    ''' </summary>
    ''' <param name="encryptedData">Data to be decrypted</param>
    ''' <param name="snkStream">Key file data as Stream</param>
    ''' <returns>Byte array of decrypted data</returns>
    Public Function AsymmetricDecrypt(encryptedData As Byte(), snkStream As Stream) As Byte()
        'Prepare the cipher
        Dim cipher As RSACryptoServiceProvider = GetRsaFromSnkFile(snkStream)
        Return DoAsymmetricDecrypt(cipher, encryptedData)
    End Function

#End Region


#Region "Symmetric Decrypt"

    ''' <summary>
    ''' Decrypts the passed data using the RijnDael algorithm using the supplied key and initialization vector
    ''' </summary>
    ''' <param name="itemToDecrypt">Data to be decrypted</param>
    ''' <param name="key">Key used to perform the decryption</param>
    ''' <param name="iv">Initialization vector supplied to the algorithm</param>
    ''' <returns>Byte array of decrypted data</returns>
    <SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
        Justification:="There is no problem disposing a stream multiple times")>
    Public Function SymmetricDecrypt(itemToDecrypt As Byte(), key As Byte(), iv As Byte()) As Byte()
        'If null is passed as an argument return an empty string
        If itemToDecrypt Is Nothing OrElse key Is Nothing OrElse iv Is Nothing Then
            Return Nothing
        End If

        'Dump the encrypted string to a buffer ready for decryption
        Using msEncryptedText As New MemoryStream(itemToDecrypt, 0, itemToDecrypt.Length, False)
            'Create and prepare the cypher
            Dim cipher As New RijndaelManaged() With {
                .BlockSize = 128,
                .Padding = PaddingMode.PKCS7
            }

            'Create the decryption stream and apply it to the passed data
            Using csOut As New CryptoStream(msEncryptedText, cipher.CreateDecryptor(key, iv), CryptoStreamMode.Read)
                Dim decryptedData As Byte() = New Byte(itemToDecrypt.Length - 1) {}
                Dim stringSize As Integer = csOut.Read(decryptedData, 0, itemToDecrypt.Length)
                cipher.Clear()

                'Trim the excess empty elements from the array and convert back to a string
                Dim trimmedData As Byte() = New Byte(stringSize - 1) {}
                Array.Copy(decryptedData, trimmedData, stringSize)

                Return trimmedData
            End Using
        End Using
    End Function

#End Region


#Region "Symmetric Encrypt"

    ''' <summary>
    ''' Encrypts the passed data using the RijnDael algorithm
    ''' </summary>
    ''' <param name="itemToEncrypt">Item to be encrypted</param>
    ''' <param name="key">Key supplied for the encryption</param>
    ''' <param name="iv">Initialisation Vector for the algorithm</param>
    ''' <returns>Byte array of encrypted data</returns>
    <SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
        Justification:="There is no problem disposing a stream multiple times")>
    Public Function SymmetricEncrypt(itemToEncrypt As Byte(), key As Byte(), iv As Byte()) As Byte()
		'If null is passed as an argument return an empty string
		If itemToEncrypt Is Nothing OrElse key Is Nothing OrElse iv Is Nothing Then
			Return Nothing
		End If

		'Create the cypher, encrypt the string and dump to a buffer in memory
		Dim cipher As New RijndaelManaged() With {
			.BlockSize = 128,
			.Padding = PaddingMode.PKCS7
		}

		'Write the data to a memory buffer
		Using msEncryptedTextBuffer As New MemoryStream()
			Using csOut As New CryptoStream(msEncryptedTextBuffer, cipher.CreateEncryptor(key, iv), CryptoStreamMode.Write)
				csOut.Write(itemToEncrypt, 0, itemToEncrypt.Length)
				csOut.Flush()
				csOut.FlushFinalBlock()

				'Read the encrypted data from the buffer and close all streams
				msEncryptedTextBuffer.Position = 0
				Dim data As Byte() = msEncryptedTextBuffer.ToArray()
				cipher.Clear()

				Return data
			End Using
		End Using
	End Function

#End Region


#Region "Block Copy"

	''' <summary>
	''' Copies a subset of a byte array to a new byte array
	''' </summary>
	''' <param name="source">Source array to copy the content from</param>
	''' <param name="startAt">Starting point in the array to copy from</param>
	''' <param name="size">Number of bytes to copy</param>
	''' <returns>
	''' New array containing the copied subset, null if the required size would overshoot the bounds 
	''' of the array
	''' </returns>
	Private Function BlockCopy(source As Byte(), startAt As Integer, size As Integer) As Byte()
		If (source Is Nothing) OrElse (source.Length < (startAt + size)) Then
			Return Nothing
		End If

		Dim ret As Byte() = New Byte(size - 1) {}
		Buffer.BlockCopy(source, startAt, ret, 0, size)
		Return ret
	End Function

#End Region

#End Region


#Region "Private Methods"

#Region "Get File Bytes"

	''' <summary>
	''' Reads a stream as an array of bytes
	''' </summary>
	''' <param name="fs">Input stream</param>
	Private Function GetStreamBytes(fs As Stream) As Byte()
		Using br As New BinaryReader(fs)
			Dim bytes As Byte() = br.ReadBytes(CInt(fs.Length))
			Return bytes
		End Using
	End Function

#End Region


#Region "Get RSA From SNK Bytes"

	''' <summary>
	''' Creates an RSA Provider using the passed keypair byte array
	''' </summary>
	''' <param name="snkBytes">Keypair as byte array</param>
	''' <returns>RSA Provider initialised with the passed keypair</returns>
	Private Function GetRsaFromSnkBytes(snkBytes As Byte()) As RSACryptoServiceProvider
		If snkBytes Is Nothing Then
			Throw New ArgumentNullException(NameOf(snkBytes))
		End If

		Dim param As RSAParameters = GetRsaParameters(snkBytes)

		Dim rsa As New RSACryptoServiceProvider()
		rsa.ImportParameters(param)
		Return rsa
	End Function

#End Region


#Region "Snk Buffer Is Public Length"

	''' <summary>
	''' Returns true if buffer length is public key size.
	''' </summary>
	''' <param name="keypair">Keypair as bytes</param>
	''' <returns>True if the buffer length is public key size, otherwise false</returns>
	Private Function SnkBufIsPubLength(keypair As ICollection(Of Byte)) As Boolean
		If keypair Is Nothing Then
			Return False
		End If
		Return (keypair.Count = 160)
	End Function

#End Region


#Region "Check RSA1"

	''' <summary>
	''' Check that RSA1 is in header (public key only).
	''' </summary>
	''' <param name="pubkey">Keypair to check</param>
	''' <returns>True if the header contains RSA1, otherwise false</returns>
	Private Function CheckRsa1(pubkey As Byte()) As Boolean
		' Check that RSA1 is in header.
		'                             R     S     A     1
		Dim check As Byte() = New Byte() {&H52, &H53, &H41, &H31}
		Return CheckMagic(pubkey, check, MagicPubIdx)
	End Function

#End Region


#Region "Check RSA2"

	''' <summary>
	''' Check that RSA2 is in header (public and private key).
	''' </summary>
	''' <param name="pubkey">Keypair to check</param>
	''' <returns>True if RSA2 is in the header, otherwise, false</returns>
	Private Function CheckRsa2(pubkey As Byte()) As Boolean
		' Check that RSA2 is in header.
		'                             R     S     A     2
		Dim check As Byte() = New Byte() {&H52, &H53, &H41, &H32}
		Return CheckMagic(pubkey, check, MagicPrivIdx)
	End Function

#End Region


#Region "Check Magic"

	''' <summary>
	''' Checks the "Magic" bytes that form the header for RSA definition
	''' </summary>
	''' <param name="keypair">Keypair to check</param>
	''' <param name="check">Bytes that we are trying to match</param>
	''' <param name="idx">Index of the header</param>
	''' <returns>True if the keypair contains the check bytes, otherwise false</returns>
	Private Function CheckMagic(keypair As Byte(), check As IList(Of Byte), idx As Integer) As Boolean
		Dim magic As Byte() = BlockCopy(keypair, idx, MagicSize)
		If magic Is Nothing Then
			Return False
		End If

		For i As Integer = 0 To MagicSize - 1
			If check(i) <> magic(i) Then
				Return False
			End If
		Next

		Return True
	End Function

#End Region


#Region "Do Asymmetric Decrypt"

	''' <summary>
	''' Performs an asymmetric decyption of passed data
	''' </summary>
	''' <param name="cipher">Cipher used to perform the decrypt</param>
	''' <param name="encryptedData">Data to decrypt</param>
	''' <returns>Byte array containing decrypted data</returns>
	Private Function DoAsymmetricDecrypt(cipher As RSACryptoServiceProvider, encryptedData As Byte()) As Byte()
		'Prepare block detail
		Dim localBlockSize As Integer = BlockSize * 2
		Dim index As Integer = 0
		Dim bytesLeft As Integer = encryptedData.Length

		Using memDecryptedTextbuffer As New MemoryStream(bytesLeft \ 2)
			Dim block As Byte()
			Dim decryptedBlock As Byte()

			'Split the serialized data into smaller blocks for processing
			While bytesLeft > 0
				'If the blocksize is too large, set it to the required amount
				If bytesLeft < localBlockSize Then
					localBlockSize = bytesLeft
				End If

				'Get a block from the encrypted data
				block = BlockCopy(encryptedData, index, localBlockSize)
				decryptedBlock = cipher.Decrypt(block, False)
				memDecryptedTextbuffer.Write(decryptedBlock, 0, decryptedBlock.Length)

				'Update position and size tracking
				index += localBlockSize
				bytesLeft -= localBlockSize
			End While

			cipher.Clear()

			'Dump the encrypted data to the caller
			memDecryptedTextbuffer.Position = 0
			Return memDecryptedTextbuffer.ToArray()
		End Using
	End Function

#End Region

#End Region

End Module
