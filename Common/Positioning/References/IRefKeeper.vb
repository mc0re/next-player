<CLSCompliant(True)>
Public Interface IRefKeeper(Of TRef)

    ReadOnly Property References As IReadOnlyCollection(Of TRef)

End Interface
