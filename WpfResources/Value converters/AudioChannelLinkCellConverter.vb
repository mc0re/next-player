Imports System.ComponentModel
Imports System.Globalization
Imports System.Windows.Media
Imports AudioChannelLibrary


<ValueConversion(GetType(IList(Of AudioChannelLinkMapping)), GetType(Geometry))>
Public Class AudioChannelLinkCellConverter
    Implements IValueConverter

#Region " IValueConverter implementation "

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

        Dim geom As New PathGeometry()
        Dim mapList = TryCast(value, IList(Of AudioChannelLinkMapping))
        If mapList Is Nothing Then Return geom

        AddMappingLines(geom, mapList.OrderBy(Function(k) -k.NofChannels).ToList())

        Return geom
    End Function


    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

#End Region


#Region " Calculations "

    Private Sub AddMappingLines(geom As PathGeometry, mapList As IList(Of AudioChannelLinkMapping))
        Dim verCount = mapList.Count

        For verIdx = 0 To verCount - 1
            Dim mapping = mapList(verIdx)
            Dim horCount = mapping.NofChannels

            If mapping.MappingList.Where(Function(m) m.IsSet).Count() = 1 Then
                geom.Figures.Add(
                    CreateSingleChannelGeometry(horCount, mapping.MappingList, verIdx, verCount))
            Else
                For horIdx = 0 To horCount - 1
                    If mapping.MappingList(horIdx).IsSet Then
                        geom.Figures.Add(CreateMappingGeometry(
                            horIdx, horCount, verIdx, verCount,
                            If(horCount = 1, 0, mapping.Panning)))
                    End If
                Next
            End If
        Next
    End Sub


    ''' <summary>
    ''' Create a PathFigure on a field 0..1 x 0..1.
    ''' </summary>
    ''' <param name="panning">A value between -1 and 1</param>
    ''' <returns>A new freezed PathFigure</returns>
    Private Shared Function CreateMappingGeometry(
        horIdx As Integer, horCount As Integer, verIdx As Integer, verCount As Integer,
        panning As Single
    ) As PathFigure
        Dim fig As New PathFigure() With {
            .IsClosed = True
        }

        Dim left = horIdx / horCount
        Dim right = (horIdx + 1) / horCount
        Dim top = verIdx / verCount
        Dim bottom = (verIdx + 1) / verCount
        Dim panPoint = (panning + 1) / 2

        fig.StartPoint = New Point(left, top)
        fig.Segments = New PathSegmentCollection(2) From {
            New LineSegment(New Point(right, top), True),
            New LineSegment(New Point(panPoint, bottom), True)
        }

        fig.Freeze()

        Return fig
    End Function


    Private Function CreateSingleChannelGeometry(
        horCount As Integer, mappingList As BindingList(Of AudioChannelMappingItem), verIdx As Integer, verCount As Integer
    ) As PathFigure

        Dim fig As New PathFigure() With {
            .IsClosed = True
        }

        Dim horIdx = mappingList.IndexOf(mappingList.First(Function(m) m.IsSet))
        Dim left = horIdx / horCount
        Dim right = (horIdx + 1) / horCount
        Dim top = verIdx / verCount
        Dim bottom = (verIdx + 1) / verCount

        fig.StartPoint = New Point(left, top)
        fig.Segments = New PathSegmentCollection(3) From {
            New LineSegment(New Point(right, top), True),
            New LineSegment(New Point(right, bottom), True),
            New LineSegment(New Point(left, bottom), True)
        }

        fig.Freeze()

        Return fig
    End Function

#End Region

End Class
