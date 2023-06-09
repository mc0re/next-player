﻿Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows.Markup


' General Information about an assembly is controlled through the following 
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

<Assembly: AssemblyTitle("VoiceControlLibrary")>
<Assembly: AssemblyDescription("Includes types and controls for voice recognition")>
<Assembly: ComVisible(False)>

'In order to begin building localizable applications, set 
'<UICulture>CultureYouAreCodingWith</UICulture> in your .vbproj file
'inside a <PropertyGroup>.  For example, if you are using US english 
'in your source files, set the <UICulture> to "en-US".  Then uncomment the
'NeutralResourceLanguage attribute below.  Update the "en-US" in the line
'below to match the UICulture setting in the project file.

'<Assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)> 


'The ThemeInfo attribute describes where any theme specific and generic resource dictionaries can be found.
'1st parameter: where theme specific resource dictionaries are located
'(used if a resource is not found in the page, 
' or application resource dictionaries)

'2nd parameter: where the generic resource dictionary is located
'(used if a resource is not found in the page, 
'app, and any theme specific resource dictionaries)
<Assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)>



'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("b0c08fdd-cbc9-41e0-ada7-2cacf64ddc46")>

<Assembly: XmlnsDefinition("http://nextplayer.nikitins.dk/VoiceLibrary", "VoiceControlLibrary")>
<Assembly: XmlnsPrefix("http://nextplayer.nikitins.dk/VoiceLibrary", "voice")>
