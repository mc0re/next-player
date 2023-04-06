''' <summary>
''' Defines a 3D plane.
''' </summary>
''' <remarks>
''' Use this interface as parameter type, when you don't care, what does this
''' plane represent or carries along, and only care about its geometric properties.
''' Use one of the implementing types, if you need to care about the plane's references.
''' </remarks>
<CLSCompliant(True)>
Public Interface IPlane3D

#Region " Properties "

    ''' <summary>
    ''' 0-point on the plane.
    ''' </summary>
    ReadOnly Property Point As IPoint3D


    ''' <summary>
    ''' A normal vector for the plane (not necessarily a unit vector).
    ''' </summary>
    ReadOnly Property Normal As Vector3D


    ''' <summary>
    ''' Distance to the origin (0, 0, 0).
    ''' </summary>
    ReadOnly Property Offset As Double

#End Region


#Region " API "

    ''' <summary>
    ''' Create a new plane by shifting this plane
    ''' by <paramref name="byOffset"/> along its normal.
    ''' </summary>
    Function Shift(byOffset As Double) As IPlane3D


    ''' <summary>
    ''' Find a line, which is intersection of the two planes.
    ''' The line is relative to "this" plane.
    ''' </summary>
    ''' <param name="byPlane"></param>
    ''' <param name="inside"></param>
    ''' <returns>
    ''' A line and inside direction.
    ''' If the planes are parallel, <see cref="PlaneIntersectionResult.State"/> is set appropritary.
    ''' </returns>
    Function Intersect(byPlane As IPlane3D, inside As Integer) As PlaneIntersectionResult


    ''' <summary>
    ''' Get the distance of a point from the plane.
    ''' </summary>
    ''' <remarks>
    ''' It is not the actual distance in meters, but the sign
    ''' is the important thing.
    ''' To get the real distance, divide this by <see cref="Normal"/> length.
    ''' </remarks>
    Function GetDistanceToPoint(c As IPoint3D) As Double


    ''' <summary>
    ''' Check whether the given point belongs to this plane.
    ''' </summary>
    Function IsPointOnPlane(c As IPoint3D) As Boolean


    ''' <summary>
    ''' Project point <paramref name="c"/> on this plane.
    ''' </summary>
    ''' <returns>
    ''' The relative coordinates
    ''' </returns>
    Function GetProjection(c As IPoint3D) As IPoint2D


    ''' <summary>
    ''' Find coordinates of point <paramref name="c"/> on this plane.
    ''' </summary>
    ''' <returns>
    ''' The relative coordinates or Nothing, if <paramref name="c"/> is not on the plane
    ''' </returns>
    Function GetProjectionIfOnPlane(c As IPoint3D) As IPoint2D


    ''' <summary>
    ''' Find a point corresponding to a projection
    ''' (<paramref name="x"/>, <paramref name="y"/>) on this plane.
    ''' </summary>
    ''' <returns>
    ''' A point in 3D space.
    ''' </returns>
    Function GetPoint(x As Double, y As Double) As IPoint3D


    ''' <summary>
    ''' Find a point corresponding to a projection
    ''' <paramref name="p"/> on this plane.
    ''' </summary>
    ''' <returns>
    ''' A point in 3D space.
    ''' </returns>
    Function GetPoint(p As IPoint2D) As IPoint3D

#End Region

End Interface
