Imports System.ComponentModel
Imports Common

''' <summary>
''' Shows the text in a window.
''' </summary>
<Description("Box")>
<Serializable>
Public Class RenderTextInterface
    Inherits TextOutputInterfaceBase

#Region " Implementation field "

    <NonSerialized>
    Private mWindow As ITextRenderer

#End Region


#Region " Left notifying property "

    Private mLeft As Double


    ''' <summary>
    ''' Window left side position [pixels].
    ''' </summary>
    Public Property Left As Double
        Get
            Return mLeft
        End Get
        Set(value As Double)
            SetField(mLeft, value, Function() Left)
        End Set
    End Property

#End Region


#Region " Top notifying property "

    Private mTop As Double


    ''' <summary>
    ''' Window top side position [pixels].
    ''' </summary>
    Public Property Top As Double
        Get
            Return mTop
        End Get
        Set(value As Double)
            SetField(mTop, value, Function() Top)
        End Set
    End Property

#End Region


#Region " Width notifying property "

    Private mWidth As Double


    ''' <summary>
    ''' Window width [pixels].
    ''' </summary>
    Public Property Width As Double
        Get
            Return mWidth
        End Get
        Set(value As Double)
            SetField(mWidth, value, Function() Width)
        End Set
    End Property

#End Region


#Region " Height notifying property "

    Private mHeight As Double


    ''' <summary>
    ''' Window height [pixels].
    ''' </summary>
    Public Property Height As Double
        Get
            Return mHeight
        End Get
        Set(value As Double)
            SetField(mHeight, value, Function() Height)
        End Set
    End Property

#End Region


#Region " Background notifying property "

    Private mBackground As Integer


    ''' <summary>
    ''' Window background colour.
    ''' Use <c>System.Drawing.Color.FromArgb</c>
    ''' </summary>
    <Category("Appearance"), Description("The background colour")>
    Public Property Background As Integer
        Get
            Return mBackground
        End Get
        Set(value As Integer)
            SetField(mBackground, value, Function() Background)
        End Set
    End Property

#End Region


#Region " Foreground notifying property "

    Private mForeground As Integer


    ''' <summary>
    ''' Window text colour.
    ''' Use <c>System.Drawing.Color.FromArgb</c>
    ''' </summary>
    <Category("Appearance"), Description("The foreground colour")>
    Public Property Foreground As Integer
        Get
            Return mForeground
        End Get
        Set(value As Integer)
            SetField(mForeground, value, Function() Foreground)
        End Set
    End Property

#End Region


#Region " Margin notifying property "

    Private mMargin As Double


    ''' <summary>
    ''' Distance between the window border and the text [pixels].
    ''' </summary>
    Public Property Margin As Double
        Get
            Return mMargin
        End Get
        Set(value As Double)
            SetField(mMargin, value, Function() Margin)
        End Set
    End Property

#End Region


#Region " IsDynamic notifying property "

    Private mIsDynamic As Boolean


    ''' <summary>
    ''' If <see langword="True"/>, the text is scaled to fit the window.
    ''' If <see langword="False"/>, the font size is set explicitly by <see cref="FontSize"/>.
    ''' </summary>
    Public Property IsDynamic As Boolean
        Get
            Return mIsDynamic
        End Get
        Set(value As Boolean)
            SetField(mIsDynamic, value, Function() IsDynamic)
        End Set
    End Property

#End Region


#Region " FontSize notifying property "

    Private mFontSize As Double = 20


    ''' <summary>
    ''' If <see cref="IsDynamic"/> is <see langword="False"/>,
    ''' the font size is set explicitly [font units].
    ''' </summary>
    Public Property FontSize As Double
        Get
            Return mFontSize
        End Get
        Set(value As Double)
            SetField(mFontSize, value, Function() FontSize)
        End Set
    End Property

#End Region


#Region " ScrollMode notifying property "

    Private mScrollMode As ScrollModes


    ''' <summary>
    ''' What to do if there is more text than fits the window.
    ''' Only works if <see cref="IsDynamic"/> is <see langword="False"/>.
    ''' </summary>
    Public Property ScrollMode As ScrollModes
        Get
            Return mScrollMode
        End Get
        Set(value As ScrollModes)
            SetField(mScrollMode, value, Function() ScrollMode)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New()
        Width = 400
        Height = 100
        Margin = 20
        Background = &HFF000000
        Foreground = &HFFFFFFFF
        IsDynamic = True
    End Sub

#End Region


#Region " Equals overrides "

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim other = TryCast(obj, RenderTextInterface)
        If other Is Nothing Then Return False

        Return MyBase.Equals(other) AndAlso
            Left = other.Left AndAlso
            Top = other.Top AndAlso
            Width = other.Width AndAlso
            Height = other.Height AndAlso
            Background = other.Background AndAlso
            Foreground = other.Foreground
    End Function


    Public Overrides Function GetHashCode() As Integer
        Return MyBase.GetHashCode() Xor
            Left.GetHashCode() Xor
            Top.GetHashCode() Xor
            Width.GetHashCode() Xor
            Height.GetHashCode() Xor
            Background.GetHashCode() Xor
            Foreground.GetHashCode()
    End Function

#End Region


#Region " ToString override "

    Public Overrides Function ToString() As String
        Return $"Text window #{mPhysicalChannel?.Channel}: {Left},{Top}-{Width},{Height}"
    End Function

#End Region


#Region " Text operations "

    Public Overrides Sub SendText(text As String)
        If Not String.IsNullOrEmpty(text) Then
            If mWindow Is Nothing Then
                Dim factory = InterfaceMapper.GetImplementation(Of ITextRendererFactory)()
                mWindow = factory.Create(Me, mPhysicalChannel)
            End If

            mWindow.Show(text)
        Else
            mWindow?.Hide()
            mWindow = Nothing
        End If
    End Sub

#End Region

End Class
