Imports Common


''' <summary>
''' Generic room layouter.
''' </summary>
''' <typeparam name="IPoint">Point interface (<see cref="IPoint2D"/> and <see cref="IPoint3D"/>)</typeparam>
''' <typeparam name="TPoint">Point type with references</typeparam>
''' <typeparam name="TRoom">Room type (<see cref="Room2D"/> and <see cref="Room3D"/>)</typeparam>
''' <typeparam name="TRef">Payload type</typeparam>
<CLSCompliant(True)>
Public Interface ILayouter(Of IPoint, TPoint As {IPoint, IRefKeeper(Of TRef)}, TRoom, TRef)

#Region " Events "

    ''' <summary>
    ''' Raised when a <see cref="PrepareLayout"/> is completed.
    ''' </summary>
    Event LayoutChanged()

#End Region


#Region " Properties "

    ''' <summary>
    ''' Room of the layout.
    ''' </summary>
    ReadOnly Property Room As TRoom

#End Region


#Region " API "

    ''' <summary>
    ''' Calculate the room division into parts according to the given list of vertices.
    ''' </summary>
    ''' <param name="room"></param>
    ''' <param name="vertices"></param>
    Sub PrepareLayout(room As TRoom, vertices As IEnumerable(Of TPoint))


    ''' <summary>
    ''' Find references closest to the given point.
    ''' </summary>
    ''' <param name="location">Point to check for</param>
    ''' <returns>A list of references, may be empty</returns>
    Function GetReferences(location As IPoint) As IReadOnlyCollection(Of TRef)

#End Region

End Interface
