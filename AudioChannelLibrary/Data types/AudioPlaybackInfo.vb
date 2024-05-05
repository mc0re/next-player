Imports Common


''' <summary>
''' Collection of playback-related properties.
''' </summary>
Public Class AudioPlaybackInfo
    Inherits PropertyChangedHelper
    Implements IPositionRelative, ISimpleVolume, ISimplePanning

#Region " ActionName property (for identification) "

    Public Property ActionName As String

#End Region


#Region " IsMuted property "

    Private mIsMuted As Boolean


    ''' <summary>
    ''' Whether the output is muted; previous volume is retained
    ''' and restored upon unmuting.
    ''' </summary>
    Public Property IsMuted As Boolean Implements ISimpleVolume.IsMuted
        Get
            Return mIsMuted
        End Get
        Set(value As Boolean)
            SetField(mIsMuted, value, Function() IsMuted)
        End Set
    End Property

#End Region


#Region " Volume property "

    Private mVolume As Single = 1


    ''' <summary>
    ''' Volume, 0 = no sound, 1 = max.
    ''' </summary>
    Public Property Volume As Single Implements ISimpleVolume.Volume
        Get
            Return mVolume
        End Get
        Set(value As Single)
            SetField(mVolume, value, NameOf(Volume))
        End Set
    End Property

#End Region


#Region " PanningModel property "

    Private mPanningModel As PanningModels = PanningModels.Fixed


    ''' <summary>
    ''' How to pan the source.
    ''' </summary>
    Public Property PanningModel As PanningModels Implements ISimplePanning.PanningModel
        Get
            Return mPanningModel
        End Get
        Set(value As PanningModels)
            SetField(mPanningModel, value, Function() PanningModel)
        End Set
    End Property

#End Region


#Region " Panning property "

    Private mPanning As Single = 0


    ''' <summary>
    ''' Left/right output panning value (-1..1).
    ''' </summary>
    Public Property Panning As Single Implements ISimplePanning.Panning
        Get
            Return mPanning
        End Get
        Set(value As Single)
            SetField(mPanning, value, NameOf(Panning))
        End Set
    End Property

#End Region


#Region " X property "

    Private mX As Single = 0


    Public Property X As Single Implements IPositionRelative.X
        Get
            Return mX
        End Get
        Set(value As Single)
            SetField(mX, value, Function() X)
        End Set
    End Property

#End Region


#Region " Y property "

    Private mY As Single = 0


    Public Property Y As Single Implements IPositionRelative.Y
        Get
            Return mY
        End Get
        Set(value As Single)
            SetField(mY, value, Function() Y)
        End Set
    End Property

#End Region


#Region " Z property "

    Private mZ As Single = 0


    Public Property Z As Single Implements IPositionRelative.Z
        Get
            Return mZ
        End Get
        Set(value As Single)
            SetField(mZ, value, Function() Z)
        End Set
    End Property

#End Region


#Region " CoefficientGenerator property "

    Public Property CoefficientGenerator As ICoefficientGenerator

#End Region

End Class
