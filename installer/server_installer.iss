#define MyAppName "WebLogServer"
#define MyAppVersion "1.0"
#define MyAppPublisher "kylecods"
#define MyAppExeName "WebLog.Server.exe"
#define MyPath ""

[Setup]
AppId={{761DBE67-14B9-4A9E-9BE1-F660C281E9C5}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
OutputBaseFilename=mysetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#MyPath}"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Run]
Filename: "{app}\WebLog.Server.exe"; Parameters: "/Install"; Flags: runhidden

[UninstallRun]
Filename: "{app}\WebLog.Server.exe"; Parameters: "/Uninstall"; Flags: runhidden


[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"


