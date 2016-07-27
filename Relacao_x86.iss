#define MyAppName "Relação"
#define MyAppVersion "2.0"
#define MyAppPublisher "Leonardo Seibt"
#define MyAppExeName "Relacao.exe"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=Relacao_Install
Compression=lzma
SolidCompression=true
PrivilegesRequired=none

[Files]
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\bin\Debug\Relacao.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\bin\Debug\Relacao.s3db"; DestDir: "{app}"; Flags: ignoreversion onlyifdoesntexist
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\bin\Debug\System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\bin\Debug\Xceed.Wpf.Toolkit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelComponente.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelFichaTecnica.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelFichaTecnica Agrupada.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelFichaTecnica Agrupada Desmembrada.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelFichaTecnica Agrupada Produtos.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelFichaTecnica Desmembrada.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelLinha.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelMateriaPrima.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelProduto.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelProdutosAgrupados.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelProdutosAgrupadosDesmembrados.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelTipoComponente.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelTipoMateriaPrima.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\RelTipoProduto.rpt"; DestDir: "{app}\Relatorios"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Imagens\Letter-R.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\bin\Debug\Relacao.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Agrupamento.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Program Files (x86)\SQLite ODBC Driver\sqlite3odbc.dll"; DestDir: "{sys}"
Source: "..\..\..\..\Downloads\CRRuntime_32bit_13_0_17.msi"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "..\..\..\..\Downloads\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "..\..\..\..\Downloads\msiexec.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppName}"; WorkingDir: "{app}"; IconFilename: "{app}\Letter-R.ico"; IconIndex: 0
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\Letter-R.ico"; IconIndex: 0

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Languages]
Name: "BrazilianPortuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Run]
Filename: "{tmp}\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"; Parameters: "/norestart /passive"; Check: FrameworkIsNotInstalled
Filename: "{tmp}\msiexec.exe"; Parameters: "/package {tmp}\CRRuntime_32bit_13_0_17.msi /passive"; WorkingDir: "{tmp}"; Check: CRRuntimeDotNet4IsNotInstalled

[Registry]
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\ODBC Data Sources"; ValueType: string; ValueName: "SQLite ODBC Driver - Relacao"; ValueData: "SQLite3 ODBC Driver"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "Driver"; ValueData: "C:\Windows\system32\sqlite3odbc.dll"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "Description"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "Database"; ValueData: "{app}\Relacao.s3db"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "Timeout"; ValueData: "100000"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "StepAPI"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "SyncPragma"; ValueData: "NORMAL"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "NoTXN"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "ShortNames"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "LongNames"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "NoCreat"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "NoWCHAR"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "FKSupport"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "OEMCP"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "LoadExt"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "BigInt"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "JDConv"; ValueData: "0"; Flags: createvalueifdoesntexist uninsdeletevalue
Root: "HKCU"; Subkey: "Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao"; ValueType: string; ValueName: "PWD"; Flags: createvalueifdoesntexist uninsdeletevalue

[Code]
function FrameworkIsNotInstalled: Boolean;
begin
  Result := not RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\.NETFramework\v4.0.30319\SKUs\.NETFramework,Version=v4.5');
end;

function CRRuntimeDotNet4IsNotInstalled: Boolean;
begin
  Result := not RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\SAP BusinessObjects\Crystal Reports for .NET Framework 4.0');
end;