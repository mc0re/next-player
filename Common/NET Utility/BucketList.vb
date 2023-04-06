Public Class BucketList(Of T)

#Region " Fields "

    Private mBuckets As New List(Of Tuple(Of Integer, T))()

#End Region


#Region " Properties "

    Public Property FileName As String

    Public Property Timestamp As Date

#End Region


#Region " API "

    ''' <summary>
    ''' Get an item for the given width.
    ''' </summary>
    ''' <param name="creator">A function to create a new item or a cloned item</param>
    Public Function TryGetForWidth(
        width As Integer,
        ByRef item As T,
        creator As Func(Of T, T)
    ) As Boolean
        Dim idx As Integer
        Dim isFound = GetBucketByIndex(width, creator, idx)

        Dim bucket = mBuckets(idx)
        item = bucket.Item2

        ' Mark for recalculation if was not present before
        If Not isFound Then Return False

        ' Mark for recalculation if the width is too small
        Return bucket.Item1 >= width
    End Function


    ''' <summary>
    ''' Add new item for the given width or update the existing bucket.
    ''' </summary>
    ''' <param name="creator">
    ''' Method to create a new object of type <typeparamref name="T"/>,
    ''' either from an existing object (clone) or anew (then the parameter is null).
    ''' </param>
    ''' <param name="updater">
    ''' Method to update a found object, if needed. Which is if the object was found,
    ''' but does not conform to the requirement set by <paramref name="width"/>.
    ''' </param>
    Public Function AddOrUpdate(width As Integer, creator As Func(Of T, T), updater As Action(Of T)) As T
        Dim item As T

        Dim idx As Integer
        Dim isFound = GetBucketByIndex(width, creator, idx)

        If Not TryGetForWidth(width, item, creator) Then
            updater.Invoke(item)

            ' The width might have changed
            mBuckets(idx) = Tuple.Create(width, item)
        End If

        Return item
    End Function


    ''' <summary>
    ''' Get total size of the list.
    ''' </summary>
    ''' <param name="itemSz">
    ''' As the item type is not known at compile time,
    ''' this is the item size calculation callback
    ''' </param>
    Public Function GetSize(itemSz As Func(Of T, Long)) As Long
        Return mBuckets.
                Where(Function(bucket) bucket IsNot Nothing).
                Sum(Function(bucket) itemSz(bucket.Item2))
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' FInd or create a place to keep the given width.
    ''' </summary>
    ''' <returns></returns>
    Private Function GetBucketByIndex(width As Integer, creator As Func(Of T, T), ByRef index As Integer) As Boolean
        Dim idx = GetBucketIndex(width)
        index = idx

        Dim hasBucket = idx < mBuckets.Count AndAlso mBuckets(idx) IsNot Nothing

        ' Resize the list if needed
        While idx >= mBuckets.Count
            mBuckets.Add(Nothing)
        End While

        If Not hasBucket Then
            ' Bucket match not found, return what we can and mark for re-calculation
            ' First look for cmaller resolution
            Dim existing = mBuckets.Take(idx).LastOrDefault(Function(b) b IsNot Nothing)

            If existing Is Nothing Then
                ' If not found - look for larger resolution
                existing = mBuckets.Skip(idx + 1).FirstOrDefault(Function(b) b IsNot Nothing)
            End If

            If existing Is Nothing Then
                ' Create a new entry
                mBuckets(idx) = Tuple.Create(width, creator.Invoke(Nothing))
            Else
                ' Copy the data to at least return something
                mBuckets(idx) = Tuple.Create(width, creator.Invoke(existing.Item2))
            End If

            Return False
        Else
            Return True
        End If
    End Function


    ''' <summary>
    ''' Return the 0-based index of the bucket, which should contain the given width.
    ''' </summary>
    Private Shared Function GetBucketIndex(width As Integer) As Integer
        ' Let's make it power-of-2 buckets with the first bucket containing 0..127.
        Dim idx = 0
        Dim upper = 128

        While upper <= width
            idx += 1
            upper *= 2
        End While

        Return idx
    End Function

#End Region

End Class
