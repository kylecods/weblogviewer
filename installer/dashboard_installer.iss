#define MyAppName "WebLogViewer"
#define MyAppVersion "1.0"
#define MyAppPublisher "kylecods"
#define MyAppExeName "WebLog.Dashboard.exe"
#define MyPath ""

[Setup]
AppId={{D5A323AE-2C4A-49CE-8485-EB285240E163}
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
Filename: "{app}\WebLog.Dashboard.exe"; Parameters: "/Install"; Flags: runhidden

[UninstallRun]
Filename: "{app}\WebLog.Dashboard.exe"; Parameters: "/Uninstall"; Flags: runhidden


[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"


