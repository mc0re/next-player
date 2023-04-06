Imports System.ComponentModel
Imports PlayerActions


''' <summary>
''' A progress bar with points visualization instead of ordinary bar.
''' </summary>
<TemplatePartAttribute(Name:="PART_Indicator", Type:=GetType(FrameworkElement))>
<TemplatePartAttribute(Name:="PART_Track", Type:=GetType(FrameworkElement))>
Public Class PointListProgressVisualizer
	Inherits PlayPositionControl

#Region " Points dependency property "

	Public Shared ReadOnly PointsProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Points), GetType(AutomationPointCollection), GetType(PointListProgressVisualizer),
		New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf PointsChanged)))


	<Category("Common Properties"), Description("A list of points to show")>
	Public Property Points As AutomationPointCollection
		Get
			Return CType(GetValue(PointsProperty), AutomationPointCollection)
		End Get
		Set(value As AutomationPointCollection)
			SetValue(PointsProperty, value)
		End Set
	End Property


	''' <summary>
	''' When changed through DependencyProperty.
	''' </summary>
	Private Shared Sub PointsChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, PointListProgressVisualizer)
		Dim oldValue = TryCast(args.OldValue, AutomationPointCollection)
		Dim newValue = TryCast(args.NewValue, AutomationPointCollection)

		If Not oldValue Is newValue Then
			If oldValue IsNot Nothing Then
				RemoveHandler oldValue.ListChanged, AddressOf this.PointsChangedHandler
			End If

			If newValue IsNot Nothing Then
				AddHandler newValue.ListChanged, AddressOf this.PointsChangedHandler
			End If
		End If
	End Sub


	''' <summary>
	''' When changed through collection.
	''' </summary>
	Private Sub PointsChangedHandler(sender As Object, args As ListChangedEventArgs)
		PointsVersion = 1 - PointsVersion
	End Sub

#End Region


#Region " PointsVersion read-only dependency property "

	Private Shared ReadOnly PointsVersionPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
		NameOf(PointsVersion), GetType(Integer), GetType(PointListProgressVisualizer),
		New PropertyMetadata(0))


	Public Shared ReadOnly PointsVersionProperty As DependencyProperty = PointsVersionPropertyKey.DependencyProperty


	<Category("Common Properties"), Description("Changed when Points change.")>
	Public Property PointsVersion As Integer
		Get
			Return CInt(GetValue(PointsVersionProperty))
		End Get
		Set(value As Integer)
			SetValue(PointsVersionPropertyKey, value)
		End Set
	End Property

#End Region


#Region " HasDuration dependency property "

	Public Shared ReadOnly HasDurationProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(HasDuration), GetType(Boolean), GetType(PointListProgressVisualizer))


	<Category("Common Properties"), Description("Whether the duration is set.")>
	Public Property HasDuration As Boolean
		Get
			Return CBool(GetValue(HasDurationProperty))
		End Get
		Set(value As Boolean)
			SetValue(HasDurationProperty, value)
		End Set
	End Property

#End Region


#Region " Duration dependency property "

	Public Shared ReadOnly DurationProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Duration), GetType(TimeSpan), GetType(PointListProgressVisualizer))


	<Category("Common Properties"), Description("Defined duration.")>
	Public Property Duration As TimeSpan
		Get
			Return CType(GetValue(DurationProperty), TimeSpan)
		End Get
		Set(value As TimeSpan)
			SetValue(DurationProperty, value)
		End Set
	End Property

#End Region


#Region " NoDurationFactor dependency property "

	Public Shared ReadOnly NoDurationFactorProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(NoDurationFactor), GetType(Double), GetType(PointListProgressVisualizer),
		New PropertyMetadata(1.0))


	<Category("Appearance"), Description("Parameter passed on for increasing the no-duration field")>
	Public Property NoDurationFactor As Double
		Get
			Return CDbl(GetValue(NoDurationFactorProperty))
		End Get
		Set(value As Double)
			SetValue(NoDurationFactorProperty, value)
		End Set
	End Property

#End Region


#Region " ShowEndBar dependency property "

	Public Shared ReadOnly ShowEndBarProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(ShowEndBar), GetType(Boolean), GetType(PlayPositionControl))


	<Category("Common Properties"), Description("Whether to show a bar in the end of the track.")>
	Public Property ShowEndBar As Boolean
		Get
			Return CBool(GetValue(ShowEndBarProperty))
		End Get
		Set(value As Boolean)
			SetValue(ShowEndBarProperty, value)
		End Set
	End Property

#End Region

End Class
