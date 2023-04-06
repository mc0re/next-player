Imports System.Collections.Concurrent


''' <summary>
''' Dependency injection.
''' </summary>
Public Class InterfaceMapper

#Region " Storage "

    Private Shared ReadOnly sImplDictionary As New ConcurrentDictionary(Of Type, Object)

#End Region


#Region " Standard API "

    ''' <summary>
    ''' Set a given instance as implementation of interface TInterface.
    ''' </summary>
    Public Shared Sub SetInstance(Of TInterface)(instance As TInterface)
        sImplDictionary.AddOrUpdate(GetType(TInterface), Function(typ) instance, Function(typ, old) instance)
    End Sub


    ''' <summary>
    ''' Set a given type as implementation of interface T.
    ''' </summary>
    Public Shared Sub SetType(Of TInterface, TImpl)()
        sImplDictionary.AddOrUpdate(GetType(TInterface), Function(typ) GetType(TImpl), Function(typ, old) GetType(TImpl))
    End Sub


    ''' <summary>
    ''' Retrieve the implementing type of interface TInterface.
    ''' </summary>
    ''' <returns>Implementing type or Nothing</returns>
    Public Shared Function GetImplementingType(Of TInterface)() As Type
        Dim res As Object = Nothing

        If Not sImplDictionary.TryGetValue(GetType(TInterface), res) Then
            Return Nothing
        End If

        Return TryCast(res, Type)
    End Function


    ''' <summary>
    ''' Retrieve the implementation of interface TInterface.
    ''' </summary>
    ''' <returns>Implementing instance or Nothing</returns>
    Public Shared Function GetImplementation(Of TInterface)(Optional isOptional As Boolean = False) As TInterface
        Dim res As Object = Nothing

        sImplDictionary.TryGetValue(GetType(TInterface), res)

        Dim asType = TryCast(res, Type)
        If asType IsNot Nothing Then
            Return CType(Activator.CreateInstance(asType), TInterface)
        End If

        If res Is Nothing AndAlso Not isOptional Then
            Throw New ArgumentException($"Object for type '{GetType(TInterface).Name}' is not registered.")
        End If

        Return CType(res, TInterface)
    End Function

#End Region

End Class
