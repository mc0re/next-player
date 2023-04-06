Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows.Markup


<Assembly: AssemblyTitle("Common")>
<Assembly: AssemblyDescription("Common types for this project")>

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("7fb470a0-8c9c-4751-9bf3-4d2ed616fee7")>
<Assembly: ComVisible(False)>

<Assembly: XmlnsDefinition("http://nextplayer.nikitins.dk/Common", "Common")>
<Assembly: XmlnsPrefix("http://nextplayer.nikitins.dk/Common", "common")>

' This can't be used as long as PlayerTests is not signed
'<Assembly: InternalsVisibleTo("PlayerTests")>
