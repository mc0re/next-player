Imports System.ComponentModel
Imports System.Windows.Media
Imports Common
Imports System.IO


''' <summary>
''' Keeps all skinned parameters together.
''' </summary>
Public Class SkinConfiguration
	Inherits PropertyChangedHelper
	Implements ISkinConfiguration

#Region " Common settings "

#Region " MainBackgroundColor notifying property "

	Private mMainBackgroundColor As Color


	<Category("Brushes"), Description("Windows background colour")>
	Public Property MainBackgroundColor As Color
		Get
			Return mMainBackgroundColor
		End Get
		Set(value As Color)
			SetField(mMainBackgroundColor, value, Function() MainBackgroundColor)
		End Set
	End Property

#End Region


#Region " FrameColor notifying property "

	Private mFrameColor As Color


	<Category("Brushes"), Description("Frame colour")>
	Public Property FrameColor As Color
		Get
			Return mFrameColor
		End Get
		Set(value As Color)
			SetField(mFrameColor, value, Function() FrameColor)
		End Set
	End Property

#End Region


#Region " MainTextColor notifying property "

	Private mMainTextColor As Color


	<Category("Brushes"), Description("Ordinary text colour")>
	Public Property MainTextColor As Color
		Get
			Return mMainTextColor
		End Get
		Set(value As Color)
			SetField(mMainTextColor, value, Function() MainTextColor)
		End Set
	End Property

#End Region


#Region " MainFontSize notifying property "

	Private mMainFontSize As Double = 13.333


	<Category("Common Properties"), Description("Main font size")>
	Public Property MainFontSize As Double
		Get
			Return mMainFontSize
		End Get
		Set(value As Double)
			SetField(mMainFontSize, value, Function() MainFontSize)
		End Set
	End Property

#End Region


#Region " MainFontWeight notifying property "

	Private mMainFontWeight As FontWeight = FontWeights.Normal


	<Category("Common Properties"), Description("Main font weight (Normal, Bold)")>
	Public Property MainFontWeight As FontWeight
		Get
			Return mMainFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mMainFontWeight, value, Function() MainFontWeight)
		End Set
	End Property

#End Region


#Region " MainFontStyle notifying property "

	Private mMainFontStyle As FontStyle = FontStyles.Normal


	<Category("Common Properties"), Description("Main font style (Normal, Italic)")>
	Public Property MainFontStyle As FontStyle
		Get
			Return mMainFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mMainFontStyle, value, Function() MainFontStyle)
		End Set
	End Property

#End Region

#End Region


#Region " Buttons "

#Region " ActionButtonBackgroundColor notifying property "

	Private mActionButtonBackgroundColor As Color


	<Category("Brushes"), Description("Background colour for main action buttons - add file, play, etc.")>
	Public Property ActionButtonBackgroundColor As Color
		Get
			Return mActionButtonBackgroundColor
		End Get
		Set(value As Color)
			SetField(mActionButtonBackgroundColor, value, Function() ActionButtonBackgroundColor)
		End Set
	End Property

#End Region


#Region " ActionButtonFrameColor notifying property "

	Private mActionButtonFrameColor As Color


	<Category("Brushes"), Description("Frame colour for main action buttons - add file, play, etc.")>
	Public Property ActionButtonFrameColor As Color
		Get
			Return mActionButtonFrameColor
		End Get
		Set(value As Color)
			SetField(mActionButtonFrameColor, value, Function() ActionButtonFrameColor)
		End Set
	End Property

#End Region


#Region " ActionButtonTextColor notifying property "

	Private mActionButtonTextColor As Color


	<Category("Brushes"), Description("Text colour for main action buttons - add file, play, etc.")>
	Public Property ActionButtonTextColor As Color
		Get
			Return mActionButtonTextColor
		End Get
		Set(value As Color)
			SetField(mActionButtonTextColor, value, Function() ActionButtonTextColor)
		End Set
	End Property

#End Region


#Region " SecondaryActionButtonColor notifying property "

	Private mSecondaryActionButtonColor As Color


	<Category("Brushes"), Description("Colour for secondary buttons - open file, ok, etc.")>
	Public Property SecondaryActionButtonColor As Color
		Get
			Return mSecondaryActionButtonColor
		End Get
		Set(value As Color)
			SetField(mSecondaryActionButtonColor, value, Function() SecondaryActionButtonColor)
		End Set
	End Property

#End Region


#Region " SecondaryButtonTextColor notifying property "

	Private mSecondaryButtonTextColor As Color


	<Category("Brushes"), Description("Text colour for secondary action buttons - open file, ok, etc.")>
	Public Property SecondaryButtonTextColor As Color
		Get
			Return mSecondaryButtonTextColor
		End Get
		Set(value As Color)
			SetField(mSecondaryButtonTextColor, value, Function() SecondaryButtonTextColor)
		End Set
	End Property

#End Region

#End Region


#Region " Common items "

#Region " GraphicsColor notifying property "

	Private mGraphicsColor As Color


	<Category("Brushes"), Description("Colour for advanced graphics - waveforms, effects")>
	Public Property GraphicsColor As Color
		Get
			Return mGraphicsColor
		End Get
		Set(value As Color)
			SetField(mGraphicsColor, value, Function() GraphicsColor)
		End Set
	End Property

#End Region


#Region " GraphicsFrameColor notifying property "

	Private mGraphicsFrameColor As Color


	<Category("Brushes"), Description("Frame colour for advanced graphics - waveforms, effects")>
	Public Property GraphicsFrameColor As Color
		Get
			Return mGraphicsFrameColor
		End Get
		Set(value As Color)
			SetField(mGraphicsFrameColor, value, Function() GraphicsFrameColor)
		End Set
	End Property

#End Region


#Region " GraphicsMarkColor notifying property "

	Private mGraphicsMarkColor As Color


	<Category("Brushes"), Description("Mark colour for advanced graphics - waveforms, effects")>
	Public Property GraphicsMarkColor As Color
		Get
			Return mGraphicsMarkColor
		End Get
		Set(value As Color)
			SetField(mGraphicsMarkColor, value, Function() GraphicsMarkColor)
		End Set
	End Property

#End Region


#Region " AttentionColor notifying property "

	Private mAttentionColor As Color


	<Category("Brushes"), Description("Colour for elements that need attention - playing item, erroneous file, etc.")>
	Public Property AttentionColor As Color
		Get
			Return mAttentionColor
		End Get
		Set(value As Color)
			SetField(mAttentionColor, value, Function() AttentionColor)
		End Set
	End Property

#End Region


#Region " FailedFontWeight notifying property "

	Private mFailedFontWeight As FontWeight = FontWeights.Bold


	<Category("Common Properties"), Description("Loading failed font weight (Normal, Bold)")>
	Public Property FailedFontWeight As FontWeight
		Get
			Return mFailedFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mFailedFontWeight, value, Function() FailedFontWeight)
		End Set
	End Property

#End Region


#Region " FailedFontStyle notifying property "

	Private mFailedFontStyle As FontStyle = FontStyles.Normal


	<Category("Common Properties"), Description("Loading failed font style (Normal, Italic)")>
	Public Property FailedFontStyle As FontStyle
		Get
			Return mFailedFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mFailedFontStyle, value, Function() FailedFontStyle)
		End Set
	End Property

#End Region


#Region " DisabledTextColor notifying property "

	Private mDisabledTextColor As Color


	<Category("Brushes"), Description("Disabled text colour")>
	Public Property DisabledTextColor As Color
		Get
			Return mDisabledTextColor
		End Get
		Set(value As Color)
			SetField(mDisabledTextColor, value, Function() DisabledTextColor)
		End Set
	End Property

#End Region

#End Region


#Region " Playlist settings "

#Region " SelectedBackgroundColor notifying property "

	Private mSelectedBackgroundColor As Color


	<Category("Brushes"), Description("Background colour for selected item")>
	Public Property SelectedBackgroundColor As Color
		Get
			Return mSelectedBackgroundColor
		End Get
		Set(value As Color)
			SetField(mSelectedBackgroundColor, value, Function() SelectedBackgroundColor)
		End Set
	End Property

#End Region


#Region " SelectedForegroundColor notifying property "

	Private mSelectedForegroundColor As Color


	<Category("Brushes"), Description("Foreground (text) colour for selected item")>
	Public Property SelectedForegroundColor As Color
		Get
			Return mSelectedForegroundColor
		End Get
		Set(value As Color)
			SetField(mSelectedForegroundColor, value, Function() SelectedForegroundColor)
		End Set
	End Property

#End Region


#Region " SelectedFrameColor notifying property "

	Private mSelectedFrameColor As Color


	<Category("Brushes"), Description("Frame colour for selected item")>
	Public Property SelectedFrameColor As Color
		Get
			Return mSelectedFrameColor
		End Get
		Set(value As Color)
			SetField(mSelectedFrameColor, value, Function() SelectedFrameColor)
		End Set
	End Property

#End Region


#Region " NextBackgroundColor notifying property "

	Private mNextBackgroundColor As Color


	<Category("Brushes"), Description("Background colour for next item")>
	Public Property NextBackgroundColor As Color
		Get
			Return mNextBackgroundColor
		End Get
		Set(value As Color)
			SetField(mNextBackgroundColor, value, Function() NextBackgroundColor)
		End Set
	End Property

#End Region


#Region " NextForegroundColor notifying property "

	Private mNextForegroundColor As Color


	<Category("Brushes"), Description("Foreground (text) colour for next item")>
	Public Property NextForegroundColor As Color
		Get
			Return mNextForegroundColor
		End Get
		Set(value As Color)
			SetField(mNextForegroundColor, value, Function() NextForegroundColor)
		End Set
	End Property

#End Region


#Region " NextFrameColor notifying property "

	Private mNextFrameColor As Color


	<Category("Brushes"), Description("Frame colour for next item")>
	Public Property NextFrameColor As Color
		Get
			Return mNextFrameColor
		End Get
		Set(value As Color)
			SetField(mNextFrameColor, value, Function() NextFrameColor)
		End Set
	End Property

#End Region

#End Region


#Region " Headers "

#Region " ActionHeaderColor notifying property "

	Private mActionHeaderColor As Color


	<Category("Brushes"), Description("Action and playlist header text colour")>
	Public Property ActionHeaderColor As Color
		Get
			Return mActionHeaderColor
		End Get
		Set(value As Color)
			SetField(mActionHeaderColor, value, Function() ActionHeaderColor)
		End Set
	End Property

#End Region


#Region " ActionHeaderFontWeight notifying property "

	Private mActionHeaderFontWeight As FontWeight = FontWeights.Bold


	<Category("Common Properties"), Description("Action and playlist header font weight (Normal, Bold)")>
	Public Property ActionHeaderFontWeight As FontWeight
		Get
			Return mActionHeaderFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mActionHeaderFontWeight, value, Function() ActionHeaderFontWeight)
		End Set
	End Property

#End Region


#Region " ActionHeaderFontStyle notifying property "

	Private mActionHeaderFontStyle As FontStyle = FontStyles.Italic


	<Category("Common Properties"), Description("Action and playlist header font style (Normal, Italic)")>
	Public Property ActionHeaderFontStyle As FontStyle
		Get
			Return mActionHeaderFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mActionHeaderFontStyle, value, Function() ActionHeaderFontStyle)
		End Set
	End Property

#End Region


#Region " SectionHeaderColor notifying property "

	Private mSectionHeaderColor As Color


	<Category("Brushes"), Description("Section header text colour")>
	Public Property SectionHeaderColor As Color
		Get
			Return mSectionHeaderColor
		End Get
		Set(value As Color)
			SetField(mSectionHeaderColor, value, Function() SectionHeaderColor)
		End Set
	End Property

#End Region


#Region " SectionHeaderFontWeight notifying property "

	Private mSectionHeaderFontWeight As FontWeight = FontWeights.Bold


	<Category("Common Properties"), Description("Section header font weight (Normal, Bold)")>
	Public Property SectionHeaderFontWeight As FontWeight
		Get
			Return mSectionHeaderFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mSectionHeaderFontWeight, value, Function() SectionHeaderFontWeight)
		End Set
	End Property

#End Region


#Region " SectionHeaderFontStyle notifying property "

	Private mSectionHeaderFontStyle As FontStyle = FontStyles.Normal


	<Category("Common Properties"), Description("Section header font style (Normal, Italic)")>
	Public Property SectionHeaderFontStyle As FontStyle
		Get
			Return mSectionHeaderFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mSectionHeaderFontStyle, value, Function() SectionHeaderFontStyle)
		End Set
	End Property

#End Region


#Region " ColumnHeaderColor notifying property "

	Private mColumnHeaderColor As Color


	<Category("Brushes"), Description("Column header text colour")>
	Public Property ColumnHeaderColor As Color
		Get
			Return mColumnHeaderColor
		End Get
		Set(value As Color)
			SetField(mColumnHeaderColor, value, Function() ColumnHeaderColor)
		End Set
	End Property

#End Region


#Region " ColumnHeaderFontWeight notifying property "

	Private mColumnHeaderFontWeight As FontWeight = FontWeights.Bold


	<Category("Common Properties"), Description("Column header font weight (Normal, Bold)")>
	Public Property ColumnHeaderFontWeight As FontWeight
		Get
			Return mColumnHeaderFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mColumnHeaderFontWeight, value, Function() ColumnHeaderFontWeight)
		End Set
	End Property

#End Region


#Region " ColumnHeaderFontStyle notifying property "

	Private mColumnHeaderFontStyle As FontStyle = FontStyles.Italic


	<Category("Common Properties"), Description("Column header font style (Normal, Italic)")>
	Public Property ColumnHeaderFontStyle As FontStyle
		Get
			Return mColumnHeaderFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mColumnHeaderFontStyle, value, Function() ColumnHeaderFontStyle)
		End Set
	End Property

#End Region


#Region " ItemHeaderColor notifying property "

	Private mItemHeaderColor As Color


	<Category("Brushes"), Description("Item header text colour")>
	Public Property ItemHeaderColor As Color
		Get
			Return mItemHeaderColor
		End Get
		Set(value As Color)
			SetField(mItemHeaderColor, value, Function() ItemHeaderColor)
		End Set
	End Property

#End Region


#Region " ItemHeaderFontWeight notifying property "

	Private mItemHeaderFontWeight As FontWeight = FontWeights.Bold


	<Category("Common Properties"), Description("Item header font weight (Normal, Bold)")>
	Public Property ItemHeaderFontWeight As FontWeight
		Get
			Return mItemHeaderFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mItemHeaderFontWeight, value, Function() ItemHeaderFontWeight)
		End Set
	End Property

#End Region


#Region " ItemHeaderFontStyle notifying property "

	Private mItemHeaderFontStyle As FontStyle = FontStyles.Italic


	<Category("Common Properties"), Description("Item header font style (Normal, Italic)")>
	Public Property ItemHeaderFontStyle As FontStyle
		Get
			Return mItemHeaderFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mItemHeaderFontStyle, value, Function() ItemHeaderFontStyle)
		End Set
	End Property

#End Region

#End Region


#Region " Action items "

#Region " ManualStartColor notifying property "

	Private mManualStartColor As Color


	<Category("Brushes"), Description("Manually started main-line action text colour")>
	Public Property ManualStartColor As Color
		Get
			Return mManualStartColor
		End Get
		Set(value As Color)
			SetField(mManualStartColor, value, Function() ManualStartColor)
		End Set
	End Property

#End Region


#Region " ActiveManualFontWeight notifying property "

	Private mActiveManualFontWeight As FontWeight = FontWeights.Bold


	<Category("Common Properties"), Description("Active main-line action with manual start font weight (Normal, Bold)")>
	Public Property ActiveManualFontWeight As FontWeight
		Get
			Return mActiveManualFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mActiveManualFontWeight, value, Function() ActiveManualFontWeight)
		End Set
	End Property

#End Region


#Region " ActiveManualFontStyle notifying property "

	Private mActiveManualFontStyle As FontStyle = FontStyles.Normal


	<Category("Common Properties"), Description("Active main-line action with manual start font style (Normal, Italic)")>
	Public Property ActiveManualFontStyle As FontStyle
		Get
			Return mActiveManualFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mActiveManualFontStyle, value, Function() ActiveManualFontStyle)
		End Set
	End Property

#End Region


#Region " ActiveTextColor notifying property "

	Private mActiveTextColor As Color


	<Category("Brushes"), Description("Active item text colour")>
	Public Property ActiveTextColor As Color
		Get
			Return mActiveTextColor
		End Get
		Set(value As Color)
			SetField(mActiveTextColor, value, Function() ActiveTextColor)
		End Set
	End Property

#End Region


#Region " ActiveFontWeight notifying property "

	Private mActiveFontWeight As FontWeight = FontWeights.Normal


	<Category("Common Properties"), Description("Active action font weight (Normal, Bold)")>
	Public Property ActiveFontWeight As FontWeight
		Get
			Return mActiveFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mActiveFontWeight, value, Function() ActiveFontWeight)
		End Set
	End Property

#End Region


#Region " ActiveFontStyle notifying property "

	Private mActiveFontStyle As FontStyle = FontStyles.Normal


	<Category("Common Properties"), Description("Active action font style (Normal, Italic)")>
	Public Property ActiveFontStyle As FontStyle
		Get
			Return mActiveFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mActiveFontStyle, value, Function() ActiveFontStyle)
		End Set
	End Property

#End Region


#Region " CommentTextColor notifying property "

	Private mCommentTextColor As Color


	<Category("Brushes"), Description("Comment item text colour")>
	Public Property CommentTextColor As Color
		Get
			Return mCommentTextColor
		End Get
		Set(value As Color)
			SetField(mCommentTextColor, value, Function() CommentTextColor)
		End Set
	End Property

#End Region


#Region " CommentFontWeight notifying property "

	Private mCommentFontWeight As FontWeight = FontWeights.Bold


	<Category("Common Properties"), Description("Comment action font weight (Normal, Bold)")>
	Public Property CommentFontWeight As FontWeight
		Get
			Return mCommentFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mCommentFontWeight, value, Function() CommentFontWeight)
		End Set
	End Property

#End Region


#Region " CommentFontStyle notifying property "

	Private mCommentFontStyle As FontStyle = FontStyles.Italic


	<Category("Common Properties"), Description("Comment action font style (Normal, Italic)")>
	Public Property CommentFontStyle As FontStyle
		Get
			Return mCommentFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mCommentFontStyle, value, Function() CommentFontStyle)
		End Set
	End Property

#End Region


#Region " ProducerColor notifying property "

	Private mProducerColor As Color


	<Category("Brushes"), Description("Producer colour")>
	Public Property ProducerColor As Color
		Get
			Return mProducerColor
		End Get
		Set(value As Color)
			SetField(mProducerColor, value, Function() ProducerColor)
		End Set
	End Property

#End Region


#Region " EffectButtonBackgroundColor notifying property "

	Private mEffectButtonBackgroundColor As Color


	<Category("Brushes"), Description("Effect background colour")>
	Public Property EffectButtonBackgroundColor As Color
		Get
			Return mEffectButtonBackgroundColor
		End Get
		Set(value As Color)
			SetField(mEffectButtonBackgroundColor, value, Function() EffectButtonBackgroundColor)
		End Set
	End Property

#End Region

#End Region


#Region " PlayIndicatorColor notifying property "

	Private mPlayIndicatorColor As Color


	<Category("Brushes"), Description("Playing indicator colour")>
	Public Property PlayIndicatorColor As Color
		Get
			Return mPlayIndicatorColor
		End Get
		Set(value As Color)
			SetField(mPlayIndicatorColor, value, Function() PlayIndicatorColor)
		End Set
	End Property

#End Region


#Region " PlayingFontWeight notifying property "

	Private mPlayingFontWeight As FontWeight = FontWeights.Normal


	<Category("Common Properties"), Description("Playing action font weight (Normal, Bold)")>
	Public Property PlayingFontWeight As FontWeight
		Get
			Return mPlayingFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mPlayingFontWeight, value, Function() PlayingFontWeight)
		End Set
	End Property

#End Region


#Region " PlayingFontStyle notifying property "

	Private mPlayingFontStyle As FontStyle = FontStyles.Normal


	<Category("Common Properties"), Description("Playing action font style (Normal, Italic)")>
	Public Property PlayingFontStyle As FontStyle
		Get
			Return mPlayingFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mPlayingFontStyle, value, Function() PlayingFontStyle)
		End Set
	End Property

#End Region


#Region " NextBorder notifying property "

	Private mNextBorder As Double


	<Category("Common Properties"), Description("Border thickness for next action")>
	Public Property NextBorder As Double
		Get
			Return mNextBorder
		End Get
		Set(value As Double)
			SetField(mNextBorder, value, Function() NextBorder)
		End Set
	End Property

#End Region


#Region " ClockFontSize notifying property "

	Private mClockFontSize As Double = 40


	<Category("Common Properties"), Description("Large clock font size")>
	Public Property ClockFontSize As Double
		Get
			Return mClockFontSize
		End Get
		Set(value As Double)
			SetField(mClockFontSize, value, Function() ClockFontSize)
		End Set
	End Property

#End Region


#Region " ClockFontColor notifying property "

	Private mClockFontColor As Color


	<Category("Brushes"), Description("Large clock colour")>
	Public Property ClockFontColor As Color
		Get
			Return mClockFontColor
		End Get
		Set(value As Color)
			SetField(mClockFontColor, value, Function() ClockFontColor)
		End Set
	End Property

#End Region


#Region " ClockFontWeight notifying property "

	Private mClockFontWeight As FontWeight = FontWeights.Normal


	<Category("Common Properties"), Description("Large clock font weight (Normal, Bold)")>
	Public Property ClockFontWeight As FontWeight
		Get
			Return mClockFontWeight
		End Get
		Set(value As FontWeight)
			SetField(mClockFontWeight, value, Function() ClockFontWeight)
		End Set
	End Property

#End Region


#Region " ClockFontStyle notifying property "

	Private mClockFontStyle As FontStyle = FontStyles.Normal


	<Category("Common Properties"), Description("Large clock font style (Normal, Italic)")>
	Public Property ClockFontStyle As FontStyle
		Get
			Return mClockFontStyle
		End Get
		Set(value As FontStyle)
			SetField(mClockFontStyle, value, Function() ClockFontStyle)
		End Set
	End Property

#End Region


#Region " Stream load / save operations "

	''' <summary>
	''' Save skin to a stream as XML.
	''' </summary>
	Public Sub Save(strm As Stream)
		Dim xmlStr = Markup.XamlWriter.Save(Me).Replace(" ", vbCrLf & vbTab)

		Using strw As New StreamWriter(strm)
			strw.Write(xmlStr)
		End Using
	End Sub


	''' <summary>
	''' Load skin from an XML stream.
	''' </summary>
	Public Sub Load(strm As Stream)
		Try
			Dim skin = Markup.XamlReader.Load(strm)

			For Each pi In GetType(SkinConfiguration).GetProperties()
				Dim readVal = pi.GetValue(skin)
				pi.SetValue(Me, readVal)
			Next

		Catch ex As Exception
			MessageBox.Show(ex.Message, "Loading skin")
		End Try
	End Sub

#End Region

End Class
