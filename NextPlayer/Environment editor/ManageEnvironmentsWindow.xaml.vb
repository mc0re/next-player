Imports System.ComponentModel
Imports Common
Imports LicenseLibrary


Public Class ManageEnvironmentsWindow

#Region " Fields "

    Private mInitialColumns As Integer

#End Region


#Region " DeleteEnvironment command "

    Public Property DeleteEnvironmentCommand As New DelegateCommand(
        AddressOf DeleteEnvironmentCommandExecuted, AddressOf DeleteEnvironmentCommandCanExecute)


    ''' <summary>
    ''' Allow deletion, unless the deleted environment is the last one for this machine.
    ''' </summary>
    Private Function DeleteEnvironmentCommandCanExecute(param As Object) As Boolean
        Dim item = CType(param, NamedEnvironmentInfo)

        Dim isLastLocal =
            item.HasLocal AndAlso
            EnvironmentList.Select(Function(d) d.HasLocal).Count() = 1

        Return Not isLastLocal
    End Function


    ''' <summary>
    ''' Delete the given environment, both playlist and local.
    ''' </summary>
    Private Sub DeleteEnvironmentCommandExecuted(param As Object)
        Dim item = CType(param, NamedEnvironmentInfo)

        If ConfigurationManager.DeleteAll(item.Name) Then
            EnvironmentList.Remove(item)
        End If
    End Sub

#End Region


#Region " EnvironmentList dependency propery "

    Private Shared ReadOnly EnvironmentListPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(EnvironmentList), GetType(NamedEnvironmentCollection), GetType(ManageEnvironmentsWindow),
        New FrameworkPropertyMetadata(New NamedEnvironmentCollection()))


    Public Shared ReadOnly EnvironmentListProperty As DependencyProperty = EnvironmentListPropertyKey.DependencyProperty


    Public ReadOnly Property EnvironmentList As NamedEnvironmentCollection
        Get
            Return CType(GetValue(EnvironmentListProperty), NamedEnvironmentCollection)
        End Get
    End Property

#End Region


#Region " GeneratedHeaderTemplate dependency propery "

    Public Shared ReadOnly GeneratedHeaderTemplateProperty As DependencyProperty = DependencyProperty.RegisterAttached(
        NameOf(GeneratedHeaderTemplate), GetType(DataTemplate), GetType(ManageEnvironmentsWindow))


    <Description("To allow setting the DataGridColumnHeader's content template on Window declaration in XAML")>
    Public Property GeneratedHeaderTemplate As DataTemplate
        Get
            Return CType(GetValue(GeneratedHeaderTemplateProperty), DataTemplate)
        End Get
        Set(value As DataTemplate)
            SetValue(GeneratedHeaderTemplateProperty, value)
        End Set
    End Property


    Public Shared Sub SetGeneratedHeaderTemplate(elem As UIElement, templ As DataTemplate)
        elem.SetValue(GeneratedHeaderTemplateProperty, templ)
    End Sub


    Public Shared Function GetGeneratedHeaderTemplate(elem As UIElement) As DataTemplate
        Return CType(elem.GetValue(GeneratedHeaderTemplateProperty), DataTemplate)
    End Function

#End Region


#Region " GeneratedCellTemplate dependency propery "

    Public Shared ReadOnly GeneratedCellTemplateProperty As DependencyProperty = DependencyProperty.RegisterAttached(
        NameOf(GeneratedCellTemplate), GetType(DataTemplate), GetType(ManageEnvironmentsWindow))


    <Description("To allow setting the template on Window declaration in XAML")>
    Public Property GeneratedCellTemplate As DataTemplate
        Get
            Return CType(GetValue(GeneratedCellTemplateProperty), DataTemplate)
        End Get
        Set(value As DataTemplate)
            SetValue(GeneratedCellTemplateProperty, value)
        End Set
    End Property


    Public Shared Sub SetGeneratedCellTemplate(elem As UIElement, templ As DataTemplate)
        elem.SetValue(GeneratedCellTemplateProperty, templ)
    End Sub


    Public Shared Function GetGeneratedCellTemplate(elem As UIElement) As DataTemplate
        Return CType(elem.GetValue(GeneratedCellTemplateProperty), DataTemplate)
    End Function

#End Region


#Region " Init and clean-up "

    Private Sub LoadedHandler() Handles Me.Loaded
        mInitialColumns = EnvTable.Columns.Count

        LoadColumns()
        AddHandler ConfigurationManager.ConfigurationChanged, AddressOf ReloadColumns
    End Sub

#End Region


#Region " Utility "

    Private Sub ReloadColumns()
        LoadColumns()
    End Sub


    Private Sub LoadColumns()
        Dim machList = ReloadEnvironment()

        ' Create the missing columns, remove the extra ones
        Dim lastColIdx = EnvTable.Columns.Count - mInitialColumns

        For midx = 0 To machList.Count - 1
            Dim col As DataGridTemplateBoundColumn

            If midx >= lastColIdx Then
                col = New DataGridTemplateBoundColumn With {
                    .HeaderTemplate = GeneratedHeaderTemplate,
                    .CellTemplate = GeneratedCellTemplate,
                    .IsReadOnly = True,
                    .Binding = New Binding(String.Format("Playlist[{0}]", midx))
                }
                EnvTable.Columns.Add(col)
                lastColIdx += 1
            Else
                col = CType(EnvTable.Columns(midx + mInitialColumns), DataGridTemplateBoundColumn)
            End If

            col.Header = machList(midx).Name
        Next

        Dim machColumnCount = lastColIdx + mInitialColumns
        While machColumnCount > EnvTable.Columns.Count
            EnvTable.Columns.RemoveAt(machColumnCount)
        End While
    End Sub


    Private Function ReloadEnvironment() As List(Of MachineDescription)
        Dim envList = EnvironmentList

        Dim playList = AppConfiguration.Instance.CurrentActionCollectionTyped.EnvironmentList
        Dim thisId = MachineFingerPrint.MachineInstance

        ' Collect machine names and info
        Dim machList As New List(Of MachineDescription)()
        For Each mn In playList
            Dim mnId = MachineFingerPrint.Create(mn.MachineId)

            If Not machList.Any(Function(m) m.FingerPrint.IsMatching(mnId)) Then
                machList.Add(New MachineDescription With {
                            .Name = mn.MachineName,
                            .FingerPrint = mnId,
                            .IsThisMachine = thisId.IsMatching(mnId)
                        })
            End If
        Next

        machList.Sort(Function(a, b) If(
                      a.IsThisMachine = b.IsThisMachine,
                      String.Compare(a.Name, b.Name, StringComparison.CurrentCulture),
                      If(a.IsThisMachine, -1, 1)))

        ' Collect environment names and info
        Dim localList = AppConfiguration.Instance.EnvironmentSettingsList
        Dim envDescList As New List(Of EnvironmentDescription)()
        Dim localNames = (From e In localList Select e.Name Distinct).ToList()

        For Each en In From eName In (
            localNames.Union(From e In playList Select e.Name)
            ) Distinct Order By eName

            envDescList.Add(New EnvironmentDescription With {.Name = en, .HasLocal = localNames.Contains(en)})
        Next

        envDescList.Sort(Function(a, b) String.Compare(a.Name, b.Name, StringComparison.CurrentCulture))

        ' Now build a matrix of all environments, mark existing ones
        envList.Clear()
        For Each envDesc In envDescList
            Dim env As New NamedEnvironmentInfo(envDesc.Name, envDesc.HasLocal)

            For Each machInfo In machList
                Dim plEnv As New PlaylistEnvironmentDescription(
                    envDesc.Name, machInfo.Name,
                    ConfigurationManager.GetPlaylistEnv(envDesc.Name, machInfo.Name) IsNot Nothing)

                env.Playlist.Add(plEnv)
            Next

            envList.Add(env)
        Next

        Return machList
    End Function

#End Region

End Class
