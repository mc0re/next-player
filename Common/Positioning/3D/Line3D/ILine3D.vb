<CLSCompliant(True)>
Public Interface ILine3D

#Region " Properties "

    ''' <summary>
    ''' A 0-point of the line.
    ''' </summary>
    ReadOnly Property Point As IPoint3D


    ''' <summary>
    ''' Vector along the line.
    ''' </summary>
    ReadOnly Property Vector As Vector3D

#End Region


#Region " API "

    ''' <summary>
    ''' Get the coordinate of the given point in line's coordinate system, if it is on the line.
    ''' </summary>
    ''' <returns>
    ''' Distance of point A is 0, point B - 1.
    ''' If the point is not on the line, the result is Nothing.
    ''' </returns>
    Function GetCoordinate(c As IPoint3D) As Double?


    ''' <summary>
    ''' Determine a point from the coordinate in line's coordinate system.
    ''' </summary>
    ''' <param name="u">Coordinate</param>
    Function GetPoint(u As Double) As IPoint3D


    ''' <summary>
    ''' Determine the line coordinate, when it is cut by a plane.
    ''' </summary>
    ''' <returns>Line coordinate or Nothing, if not cut</returns>
    ''' <remarks>
    ''' The returned coordinate can be negative.
    ''' </remarks>
    Function CutByPlane(plane As IPlane3D) As LineCutResult


    ''' <summary>
    ''' Determine the line coordinate, when it is cut by a sphere.
    ''' </summary>
    ''' <returns>Line coordinate (the greater one of the two) or Nothing, if not cut</returns>
    Function CutBySphere(sphere As Sphere3D) As LineCutResult

#End Region

End Interface
