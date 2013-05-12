ScriptCS.ClrMD
====
This is a [ScriptCS](http://github.com/scriptcs/scriptcs) script pack that brings in features 
of the ClrMD API which can then be used for scripted or interactive diagnostics (via REPL) 
against any running CLR process.


What is ClrMD?
====
For an introduction to ClrMD and its API you can [check out this blog post](http://blogs.msdn.com/b/dotnet/archive/2013/05/01/net-crash-dump-and-live-process-inspection.aspx) on [the .NET Framework Blog](http://blogs.msdn.com/b/dotnet/).

Getting Started with the ClrMD Script Pack
====
REPL Style
----
Using ScriptCS.ClrMD in REPL mode is probably the most powerful way to work with it, so let's take a quick look at how that works:

   1. Start by installing the ScriptCs.ClrMD script pack: ```scriptcs -install ScriptCs.ClrMD```
   2. Launch scriptcs in REPL mode: ```scriptcs```
   3. Once you are at the REPL prompt you can simply attach to a process and begin interacting with it using ClrMD plus the various extensions that are automatically 
   imported by the ScriptCs.ClrMD script pack like so:

```csharp
   // First require the ClrMdPack
   > var clrmd = Require<ClrMdPack>();
   
   // Now attach to a running .NET process which will give us back a ClrMD ClrRuntime instance
   > var clrRuntime = clrmd.AttachToProcess("MyDotNetApplication");
   
   // Now we can simply us the raw ClrMD API directly
   > Console.WriteLine(clrRuntime.Threads.Count);
   
   // Or we can use an extension method included by the script pack for higher level analytics
   > clrRuntime.GetHeapStatsByType().ToList().ForEach(s => Console.WriteLine("{0,12:n0} {1,12:n0} {2}", s.TotalHeapSize, s.NumberOfInstances, s.TypeName));
```

Here's a sample of the output the command above command might output to the REPL window:
```
      1,672           21 System.String[]
      1,838           20 System.Char[]
      2,008           24 System.Int32[]
      2,968           53 System.RuntimeType
     12,232          249 System.String
     36,192           42 System.Object[]
```

```csharp
    // Finally we can detatch from the process to let it resume executing once we're done inspecting it
    > clrmd.DetatchFromCurrentProcess();
```

Script Style
----
Of course if you have standard analysis that you might want to perform, it would make sense to store those in a reusable script. 
Here's a quick example of what that kind of workflow might look like:

   _... coming soon ..._
