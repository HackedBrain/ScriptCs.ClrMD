using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("ScriptCs.ClrMD")]
[assembly: AssemblyDescription("A ScriptCS script pack that brings in features of the ClrMD API which can then be used for scripted or interactive diagnostics (via REPL) against any running CLR process.")]
[assembly: AssemblyCompany("Hacked.Brain Software")]
[assembly: AssemblyProduct("ScriptCs.ClrMD")]
[assembly: AssemblyCopyright("Copyright © Drew Marsh 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif

[assembly: AssemblyVersion("0.1.3.0")]
[assembly: AssemblyFileVersion("0.1.3.0")]
