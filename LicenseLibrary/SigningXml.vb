Imports System.Security.Cryptography.Xml
Imports System.Xml
Imports System.IO
Imports System.Security.Cryptography


Public Module SigningXml

	''' <summary>
	''' Add a digital signature to the given unsigned XML document.
	''' </summary>
	''' <returns>The same document reference, but the signature is added</returns>
	Public Function Sign(originalXml As XmlDocument, snkStream As Stream) As XmlDocument
        ' ReSharper disable once RedundantQualifier
        Dim key = EncryptionUtils.GetRsaFromSnkFile(snkStream)

        ' Create a reference to be signed. Blank = Everything
        Dim emptyReference As New Reference() With {.Uri = String.Empty}

		' Add an enveloped transformation to the reference.
		Dim envelope = New XmlDsigEnvelopedSignatureTransform()
		emptyReference.AddTransform(envelope)

		' Sign
		Dim signedXml As New SignedXml(originalXml) With {.SigningKey = key}
		signedXml.AddReference(emptyReference)
		signedXml.ComputeSignature()

		Dim digitalSignature = signedXml.GetXml()
		originalXml.DocumentElement.AppendChild(originalXml.ImportNode(digitalSignature, True))

		Return originalXml
	End Function


	''' <summary>
	''' Check whether the signature is intact.
	''' </summary>
	''' <param name="signedDoc"></param>
	''' <param name="snkStream">SNK data as stream</param>
	''' <returns></returns>
	Public Function VerifySignature(signedDoc As XmlDocument, snkStream As Stream) As Boolean
        ' ReSharper disable once RedundantQualifier
        Dim key = EncryptionUtils.GetRsaFromSnkFile(snkStream)
        Return VerifySignature(signedDoc, key)
	End Function


	''' <summary>
	''' Check whether the signature is intact.
	''' </summary>
	''' <param name="signedDoc"></param>
	''' <param name="key">Prepared key</param>
	''' <returns></returns>
	Public Function VerifySignature(signedDoc As XmlDocument, key As RSACryptoServiceProvider) As Boolean
		Dim nsm = New XmlNamespaceManager(New NameTable())
		nsm.AddNamespace("dsig", SignedXml.XmlDsigNamespaceUrl)

		Try
			Dim signatureGenerator As New SignedXml(signedDoc)
			Dim signatureNode = signedDoc.SelectSingleNode("//dsig:Signature", nsm)
			signatureGenerator.LoadXml(CType(signatureNode, XmlElement))

			Return signatureGenerator.CheckSignature(key)

		Catch ex As Exception
			' Any error yields a verification failure.
			Return False
		End Try
	End Function

End Module
