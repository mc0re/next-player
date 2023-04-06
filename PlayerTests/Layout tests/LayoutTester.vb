Imports Common
Imports RoomDivisionLibrary


Public Class LayoutTester

#Region " Fields "

    Private channelList As New List(Of Point3D(Of String))()

    Private mLayouter As I3DLayouter(Of String)

#End Region


#Region " Factory API "

    Private Sub New(speakersCoords As Double()(), nofPolyhedrons As Integer, layouter As I3DLayouter(Of String))
        Dim room As New Room3D()
        room.SetAllSides(2)
        mLayouter = layouter

        Dim chNr = 1
        For Each spCoord In speakersCoords
            channelList.Add(Point3DHelper.Create(
                            spCoord(0), spCoord(1), spCoord(2), {"C" + chNr.ToString()}))
            chNr += 1
        Next

        ' Create and verify the layout
        mLayouter.PrepareLayout(room, channelList)
        Assert.AreEqual(nofPolyhedrons, mLayouter.GetPolyhedrons().Count)
    End Sub


    Public Shared Function CreateFlattened(
        speakersCoords As Double()(),
        nofPolyhedrons As Integer
    ) As LayoutTester

        Return New LayoutTester(speakersCoords, nofPolyhedrons, New Room3DFlattenedLayouter(Of String)())
    End Function

#End Region


#Region " Verification API "

    Public Sub Test(soundPosition As Double(), expectedOutIdx As Integer())
        If soundPosition Is Nothing Then
            ' {0, 0, 0) unless specified
            soundPosition = New Double(3) {}
        End If

        ' Get for the given point
        Dim c = Point3DHelper.Create(soundPosition(0), soundPosition(1), soundPosition(2))
        Dim outputs = mLayouter.GetReferences(c)
        Assert.IsNotNull(outputs)

        Assert.AreEqual(expectedOutIdx.Count, outputs.Count, "Reference count.")

        ' The order might differ
        Dim isFound(UBound(expectedOutIdx)) As Boolean

        For outpIdx = 0 To outputs.Count - 1
            For chIdx = 0 To UBound(expectedOutIdx)
                If channelList(expectedOutIdx(chIdx)).References.Contains(outputs(outpIdx)) Then
                    isFound(chIdx) = True
                End If
            Next
        Next

        For fndIdx = 0 To UBound(isFound)
            If Not isFound(fndIdx) Then
                Assert.Fail("Reference '{0}' not found in layout result '{1}'.",
                            expectedOutIdx(fndIdx),
                            String.Join("', '", outputs))
            End If
        Next
    End Sub

#End Region

End Class
