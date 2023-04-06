Imports Microsoft.VisualStudio.DebuggerVisualizers
Imports GeometryVisualizers
Imports Common


Module TestConsole

    Public Sub ShowVisualizer(objectToVisualize As Object, visualizerType As Type)
        Dim visualizerHost As New VisualizerDevelopmentHost(objectToVisualize, visualizerType)
        visualizerHost.ShowVisualizer()
    End Sub


    Sub Main()
        Dim lp1 = New TestPoint3D(-2, -1, -1)
        Dim lp2 = New TestPoint3D(0, 1, 1)

        Dim sp1 = New TestPoint3D(-1, -1, -1)
        Dim sp2 = New TestPoint3D(-2, -1, 0)
        Dim sp3 = New TestPoint3D(-2, -1, 2)
        Dim sp4 = New TestPoint3D(2, -1, 2)
        Dim sp5 = New TestPoint3D(2, -1, 0)

        'Console.WriteLine("Show a point")
        'ShowVisualizer(lp1, GetType(Point3DVisualizer))

        'Console.WriteLine("Show a line")
        'ShowVisualizer(Line3DHelper.Create(lp1, lp2), GetType(Line3DVisualizer))

        'Console.WriteLine("Show a line segment")
        'Dim side = LineSegment3DHelper.Create(sp1, sp2)
        'ShowVisualizer(side, GetType(LineSegment3DVisualizer))

        'Console.WriteLine("Show a plane")
        'Dim plane1 = Plane3DHelper.Create3Points(sp1, sp2, sp3)
        'ShowVisualizer(plane1, GetType(Plane3DVisualizer))

        'Console.WriteLine("Show a polygon")
        'Dim pl = Polygon3DHelper.Create(
        '    Plane3DHelper.Create3Points(sp1, sp2, sp3),
        '    sp1, sp2, sp3, sp4, sp5)
        'ShowVisualizer(pl, GetType(Polygon3DVisualizer))

        Console.WriteLine("Show a polyhedron")
        Dim pl1 = Plane3DHelper.CreatePointNormal(sp1, New Vector3D(1, 1, 1), {"A"})
        Dim pl2 = Plane3DHelper.CreatePointNormal(New TestPoint3D(1, 1, 1), New Vector3D(-1, -1, -1), {"B"})
        Dim pl3 = Plane3DHelper.CreatePointNormal(New TestPoint3D(-1, 1, 0), New Vector3D(1, -1, 0), {"C"})
        Dim pl4 = Plane3DHelper.CreatePointNormal(New TestPoint3D(1, -1, 0), New Vector3D(-1, 1, 0), {"D"})

        Dim ph = PolyhedronHelper.CreateFromBorders(
            {
                Plane3DHelper.CreateBorder(pl1, 1, pl1.References),
                Plane3DHelper.CreateBorder(pl2, 1, pl2.References),
                Plane3DHelper.CreateBorder(pl3, 1, pl3.References),
                Plane3DHelper.CreateBorder(pl4, 1, pl4.References)
            })
        ShowVisualizer(ph, GetType(PolyhedronVisualizer))
    End Sub

End Module
