''' <summary>
''' Defines a border in 3D space.
''' </summary>
<CLSCompliant(True)>
Public Interface IBorder3D

    ''' <summary>
    ''' The plane defining the border.
    ''' </summary>
    ReadOnly Property Plane As IPlane3D


    ''' <summary>
    ''' Which direction is inside.
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Inside As Integer

End Interface
