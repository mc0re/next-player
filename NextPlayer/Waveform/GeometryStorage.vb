Imports System.Collections.Concurrent
Imports System.Runtime.InteropServices
Imports Common


Public Class GeometryStorage

#Region " Fields "

    Private mStorage As New ConcurrentDictionary(Of String, BucketList(Of GeometryStorageItem))

#End Region


#Region " API "

    ''' <summary>
    ''' Check geometry cache, return existing geometry if available.
    ''' </summary>
    ''' <returns>True if the figure is up to date, False if update is needed</returns>
    ''' <remarks>
    ''' If data for the wrong resolution is found, a copy is returned,
    ''' so it can be modified later on, when the right resolution is calculated.
    ''' </remarks>
    Public Function GetFigureFromCache(
        fileName As String, fileTimestamp As Date, outWidth As Integer, ByRef cacheItem As GeometryStorageItem
    ) As Boolean

        Dim buckets As BucketList(Of GeometryStorageItem) = Nothing

        If Not mStorage.TryGetValue(fileName, buckets) OrElse buckets.Timestamp <> fileTimestamp Then
            ' If the file is not found, or the timestamp is wrong, return nothing
            mStorage.TryRemove(fileName, Nothing)
            cacheItem = Nothing
            Return False
        End If

        ' If it is, but the width is wrong, return what we have and mark for update
        Return buckets.TryGetForWidth(outWidth, cacheItem,
                                      Function(orig) New GeometryStorageItem With {
                                        .Figure = orig.Figure})
    End Function


    ''' <summary>
    ''' Add or replace Geometry in the cache.
    ''' </summary>
    ''' <remarks>Report the currently used memory</remarks>
    Public Function AddFigureToCache(
        fileName As String, fileTimestamp As Date, outWidth As Integer, fig As PathFigure
    ) As GeometryStorageItem

        Dim buckets = mStorage.GetOrAdd(fileName,
                                       Function(fname) New BucketList(Of GeometryStorageItem) With {
            .FileName = fileName, .Timestamp = fileTimestamp
        })

        Dim item = buckets.AddOrUpdate(outWidth,
            Function(orig) New GeometryStorageItem With {
                                        .Figure = orig?.Figure},
            Sub(el) el.Figure = fig)

        ' Dependency properties are used, so it's an estimate
        Static itemSize As Long = Marshal.SizeOf(Of Point)() + Marshal.SizeOf(Of Boolean)() * 2
        Dim sz = mStorage.Values.Sum(
            Function(f) f.GetSize(
                Function(b) If(b.Figure Is Nothing, 0, b.Figure.Segments.Count * itemSize)))
        InterfaceMapper.GetImplementation(Of IMessageLog)().LogFigureCacheInfo(sz)

        Return item
    End Function

#End Region

End Class

