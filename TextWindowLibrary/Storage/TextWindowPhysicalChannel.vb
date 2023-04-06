Imports System.ComponentModel
Imports System.Xml.Serialization
Imports TextChannelLibrary


''' <summary>
''' Defines a single text window.
''' </summary>
<Serializable>
Public Class TextWindowPhysicalChannel
    Inherits TextPhysicalChannel

#Region " Implementation field "

    <NonSerialized>
    Private mWindow As TextWindow

#End Region


#Region " Left notifying property "

    Private mLeft As Double


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

    Private mBackground As System.Drawing.Color


    <XmlIgnore>
    <Category("Appearance"), Description("The background colour")>
    Public Property Background As System.Drawing.Color
        Get
            Return mBackground
        End Get
        Set(value As System.Drawing.Color)
            SetField(mBackground, value, Function() Background)
        End Set
    End Property


    <XmlElement(NameOf(Background))>
    Public Property BackgroundSerializable As Integer
        Get
            Return mBackground.ToArgb()
        End Get
        Set(value As Integer)
            mBackground = System.Drawing.Color.FromArgb(value)
        End Set
    End Property

#End Region


#Region " Foreground notifying property "

    Private mForeground As System.Drawing.Color


    <XmlIgnore>
    <Category("Appearance"), Description("The foreground colour")>
    Public Property Foreground As System.Drawing.Color
        Get
            Return mForeground
        End Get
        Set(value As System.Drawing.Color)
            SetField(mForeground, value, Function() Foreground)
        End Set
    End Property


    <XmlElement(NameOf(Foreground))>
    Public Property ForegroundSerializable As Integer
        Get
            Return mForeground.ToArgb()
        End Get
        Set(value As Integer)
            mForeground = System.Drawing.Color.FromArgb(value)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New()
        Left = 0
        Top = 0
        Width = 400
        Height = 100
        Background = System.Drawing.Color.Black
        Foreground = System.Drawing.Color.White
    End Sub

#End Region


#Region " Equals overrides "

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim other = TryCast(obj, TextWindowPhysicalChannel)
        If other Is Nothing Then Return False

        Return MyBase.Equals(other) AndAlso
            Left = other.Left AndAlso
            Top = other.Top AndAlso
            Width = other.Width AndAlso
            Height = other.Height AndAlso
            BackgroundSerializable = other.BackgroundSerializable AndAlso
            ForegroundSerializable = other.ForegroundSerializable
    End Function


    Public Overrides Function GetHashCode() As Integer
        Return MyBase.GetHashCode() Xor
            Left.GetHashCode() Xor
            Top.GetHashCode() Xor
            Width.GetHashCode() Xor
            Height.GetHashCode() Xor
            BackgroundSerializable.GetHashCode() Xor
            ForegroundSerializable.GetHashCode()
    End Function

#End Region


#Region " ToString override "

    Public Overrides Function ToString() As String
        Return $"Text window {Channel}: {Description}"
    End Function

#End Region


#Region " Text operations "

    Public Overrides Sub ShowText(text As String)
        ' To switch focus back
        Dim oldWin = Application.Current.Windows.OfType(Of Window)().SingleOrDefault(Function(w) w.IsActive)

        ' Create if does not exist
        If mWindow Is Nothing Then
            mWindow = New TextWindow() With {
                .Configuration = Me
            }
        End If

        mWindow.Text = text

        If Not mWindow.IsVisible Then
            mWindow.Show()
        End If

        mWindow.Topmost = True
        mWindow.Configuration.IsActive = True

        ' Set the focus back to main window
        If oldWin IsNot Nothing Then
            oldWin.Focus()
        End If
    End Sub


    Public Overrides Sub HideText()
        If mWindow Is Nothing Then Return

        mWindow.Text = String.Empty
        mWindow.Hide()
        mWindow.Configuration.IsActive = False
    End Sub

#End Region

End Class
