Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Windows.Threading
Imports AudioChannelLibrary
Imports Common
Imports NAudio.Wave


''' <summary>
''' Calculates and keeps waveforms for audio files.
''' </summary>
Public Class WaveformStorage

#Region " Helper classes "

    Private Class SampleCacheItem
        Public Property FileName As String
        Public Property Timestamp As DateTime
        Public Property Samples As WeakReference(Of ICollection(Of Single))
        Public Property MaxSample As Single
    End Class

#End Region


#Region " GeometryUpdated events "

    Public Shared Event GeometryUpdated(fileName As String)

#End Region


#Region " Fields "

    Private Shared ReadOnly WorkerPool As IBackgroundWorkerPool(Of GeometryWorkItem)


    Private Shared mReporter As IMessageLog

#End Region


#Region " Sample cache "

    ''' <summary>
    ''' Keeps already read samples.
    ''' </summary>
    Private Shared ReadOnly SampleCache As New ConcurrentDictionary(Of String, SampleCacheItem)


    ''' <summary>
    ''' Check sample cache, return sample list if available.
    ''' If the timestamp does not match, return Nothing.
    ''' </summary>
    Private Shared Function GetSampleFromCache(fileName As String, fileTimestamp As Date) As ICollection(Of Single)
        Dim item As SampleCacheItem = Nothing
        If Not SampleCache.TryGetValue(fileName, item) Then Return Nothing
        If item.Timestamp <> fileTimestamp Then Return Nothing

        Dim res As ICollection(Of Single) = Nothing
        If Not item.Samples.TryGetTarget(res) Then Return Nothing

        Return res
    End Function


    ''' <summary>
    ''' Check sample cache, return sample list if available.
    ''' </summary>
    Private Shared Function GetCacheItem(fileName As String) As SampleCacheItem
        Dim item As SampleCacheItem = Nothing
        If Not SampleCache.TryGetValue(fileName, item) Then Return Nothing

        Return item
    End Function


    ''' <summary>
    ''' Add the given sample list to the cache.
    ''' </summary>
    Private Shared Sub AddSampleListToCache(fileName As String, fileTimestamp As Date, sample As IEnumerable(Of Single))
        Dim sampleList = sample.ToList()

        Dim item As New SampleCacheItem() With {
            .FileName = fileName, .Timestamp = fileTimestamp,
            .Samples = New WeakReference(Of ICollection(Of Single))(sampleList),
            .MaxSample = If(sampleList.Any(), (From s In sampleList Select Math.Abs(s)).Max(), 0)
        }

        SampleCache.AddOrUpdate(fileName, item, Function(fname, cacheItem) item)

        Dim sz = (
            From s In SampleCache.Values
            Let sl = CType(Nothing, ICollection(Of Single))
            Where s.Samples.TryGetTarget(sl)
            Select sl
            ).Sum(Function(s) If(s Is Nothing, 0, s.Count * 4))

        InterfaceMapper.GetImplementation(Of IMessageLog)().LogSampleCacheInfo(sz)
    End Sub

#End Region


#Region " Geometry cache "

    ''' <summary>
    ''' Keeps already created geometries.
    ''' </summary>
    Private Shared ReadOnly GeometryCache As New GeometryStorage()

#End Region


#Region " Get API "

    ''' <summary>
    ''' Get a waveform Geometry for the given audio file.
    ''' If not cached, request processing and return Nothing.
    ''' </summary>
    ''' <remarks>
    ''' Executed in UI thread. If the waveform is not available, starts
    ''' an asynchronous task, which, when finishes, raises GeometryUpdated event
    ''' in UI thread.
    ''' </remarks>
    ''' <param name="fileName">Filename to read</param>
    ''' <param name="outWidth">Output window width (for optimization)</param>
    Public Shared Function GetGeometry(fileName As String, outWidth As Integer, disp As Dispatcher) As PathFigure
        If String.IsNullOrWhiteSpace(fileName) Then Return Nothing

        Dim timestamp = IO.File.GetLastWriteTime(fileName)
        Dim cacheItem As GeometryStorageItem = Nothing

        If Not GeometryCache.GetFigureFromCache(fileName, timestamp, outWidth, cacheItem) Then
            If cacheItem Is Nothing Then
                ' No data found
                Dim fig = CreateEmptyWaveform(outWidth)
                cacheItem = GeometryCache.AddFigureToCache(fileName, timestamp, outWidth, fig)
            End If

            Dim workItem = New GeometryWorkItem With {
                .FileName = fileName,
                .Timestamp = timestamp,
                .UiDispatcher = disp,
                .Width = outWidth,
                .Output = cacheItem}

            EnqueueTask(workItem)
        End If

        Return cacheItem.Figure
    End Function


    ''' <summary>
    ''' Get a maximum sample for the waveform.
    ''' If not cached, return 0.
    ''' </summary>
    ''' <param name="fileName">Filename to read</param>
    Public Shared Function GetMaxSample(fileName As String) As Single
        If String.IsNullOrWhiteSpace(fileName) Then Return Nothing

        Dim item = GetCacheItem(fileName)
        If item Is Nothing Then Return 0

        Return item.MaxSample
    End Function


    ''' <summary>
    ''' Raises GeometryUpdated event on the given dispatcher.
    ''' </summary>
    Public Shared Sub ForceUpdate(filename As String, disp As Dispatcher)
        disp.BeginInvoke(Sub() RaiseEvent GeometryUpdated(filename))
    End Sub

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Kick-start the worker thread.
    ''' </summary>
    Shared Sub New()
        WorkerPool = New BackgroundWorkerPool(Of GeometryWorkItem)(AddressOf CalculateWaveform)
        InterfaceMapper.SetInstance(WorkerPool)
    End Sub


    ''' <summary>
    ''' Clean up the resources.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub Cleanup()
        WorkerPool.Dispose()
    End Sub

#End Region


#Region " To queue "

    ''' <summary>
    ''' Add the given task to the queue, unless already there or being processed.
    ''' </summary>
    ''' <remarks>For the same bucket, only get the largest work item</remarks>
    Private Shared Sub EnqueueTask(item As GeometryWorkItem)
        mReporter = InterfaceMapper.GetImplementation(Of IMessageLog)()
        WorkerPool.QueueWorkItem(item, Function(w) w.Output Is item.Output)
    End Sub

#End Region


#Region " From queue "

    ''' <summary>
    ''' </summary>
    Private Shared Sub CalculateWaveform(workItem As GeometryWorkItem, cancel As CancellationToken)
        If cancel.IsCancellationRequested Then Return

        mReporter.LogWaveformWork(True)

        Try
            ProcessGeometry(workItem)
        Catch ex As Exception
            mReporter.LogFileError("Exception {0} when processing '{1}'.", ex.Message, workItem.FileName)
        Finally
            mReporter.LogWaveformWork(False)
        End Try
    End Sub


    ''' <summary>
    ''' Process a single geometry request.
    ''' </summary>
    Private Shared Sub ProcessGeometry(item As GeometryWorkItem)
        Dim fileName = item.FileName
        Dim timestamp = item.Timestamp

        Dim samples = GetSampleFromCache(fileName, timestamp)
        If samples Is Nothing Then
            samples = LoadSamples(fileName)
            AddSampleListToCache(fileName, timestamp, samples)
        End If

        Dim pts = SamplesToPoints(samples, samples.Count, item.Width)
        item.Output.Figure = PointsToFigure(item.Width, pts)
        GeometryCache.AddFigureToCache(fileName, timestamp, item.Width, item.Output.Figure)

        ForceUpdate(fileName, item.UiDispatcher)
    End Sub

#End Region


#Region " Sample utility "

    ''' <summary>
    ''' Load samples from file.
    ''' </summary>
    Private Shared Function LoadSamples(fname As String) As ICollection(Of Single)
        ' 44000-48000 samples/sec
        ' Buffer contains approximatelly 1 second worth of audio
        Const bufLength = 50000

        If Not IO.File.Exists(fname) Then
            Return Array.Empty(Of Single)()
        End If

        Try
            'Dim sp As ISampleProvider = New AudioFileReader(fname)
            Using wstrm = InterfaceMapper.GetImplementation(Of IInputStreamProvider)().CreateStream(fname)
                Dim sp = wstrm.ToSampleProvider()

                Dim sampleList As New List(Of Single)()

                Dim lastRead As Integer
                Do
                    Dim buffer(bufLength - 1) As Single
                    lastRead = sp.Read(buffer, 0, bufLength)
                    sampleList.AddRange(buffer.Take(lastRead))
                Loop Until lastRead = 0

                Return sampleList
            End Using

        Catch ex As Exception
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileError(ex.Message)
            Return Array.Empty(Of Single)()
        End Try
    End Function


    ''' <summary>
    ''' Convert a list of samples into a PathGeometry.
    ''' </summary>
    ''' <remarks>
    ''' Produce max two segments per pixel.
    ''' </remarks>
    Private Shared Function SamplesToPoints(
        samples As IEnumerable(Of Single), inWidth As Long, outWidth As Integer
    ) As IEnumerable(Of Point)

        Dim pts As New List(Of Point)()

        If inWidth * 2 < outWidth Then
            ' Show as line
            Dim inX = -1L

            For Each value In samples
                inX += 1
                Dim outX = CInt(inX / inWidth * outWidth)
                pts.Add(New Point(outX, value))
            Next
        Else
            ' Show as area between positive and negative lines
            Dim posValuesSum(outWidth) As Double
            Dim posValuesCnt(outWidth) As Integer
            Dim negValuesSum(outWidth) As Double
            Dim negValuesCnt(outWidth) As Integer

            Dim inX = -1L

            For Each value In samples
                inX += 1
                Dim outX = CInt(inX / inWidth * outWidth)

                If value > 0 Then
                    posValuesSum(outX) += value
                    posValuesCnt(outX) += 1
                ElseIf value < 0 Then
                    negValuesSum(outX) += value
                    negValuesCnt(outX) += 1
                Else ' value = 0
                    posValuesCnt(outX) += 1
                    negValuesCnt(outX) += 1
                End If
            Next

            pts.Add(New Point(0, 0))

            For posX = 0 To outWidth - 1
                If posValuesCnt(posX) = 0 Then Continue For
                pts.Add(New Point(posX, posValuesSum(posX) / posValuesCnt(posX)))
            Next

            For negX = outWidth - 1 To 0 Step -1
                If negValuesCnt(negX) = 0 Then Continue For
                pts.Add(New Point(negX, negValuesSum(negX) / negValuesCnt(negX)))
            Next

            pts.Add(New Point(0, 0))
        End If

        Return pts
    End Function


    ''' <summary>
    ''' Add the points as LineSegments.
    ''' </summary>
    ''' <returns>A new freezed PathFigure</returns>
    Private Shared Function PointsToFigure(outWidth As Integer, pointList As IEnumerable(Of Point)) As PathFigure
        Dim fig As New PathFigure() With {
            .IsClosed = True
        }

        Dim maxX = 0

        If pointList.Any() Then
            fig.StartPoint = pointList.First()
            Dim lines = From pt In pointList Select New LineSegment(pt, True)
            maxX = CInt((From pt In pointList Select pt.X).Max())
            fig.Segments = New PathSegmentCollection(lines)
        Else
            fig.StartPoint = New Point(0, 0)
        End If

        If maxX < outWidth Then
            fig.Segments.Add(New LineSegment(New Point(maxX, 0), False))
            fig.Segments.Add(New LineSegment(New Point(outWidth, 0), True))
        End If

        fig.Freeze()

        Return fig
    End Function


    ''' <summary>
    ''' Create an "empty" figure.
    ''' </summary>
    Private Shared Function CreateEmptyWaveform(outWidth As Integer) As PathFigure
        Return PointsToFigure(outWidth, Array.Empty(Of Point)())
    End Function

#End Region

End Class
