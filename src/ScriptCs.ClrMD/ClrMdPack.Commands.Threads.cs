using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;

namespace HackedBrain.ScriptCs.ClrMd
{
	partial class ClrMdPack
	{
		public void DumpClrThreads()
		{
			this.DumpClrThreads(liveThreadsOnly: true, showCallstack: true);
		}

		public void DumpClrThreads(bool liveThreadsOnly, bool showCallstack)
		{
			IEnumerable<ClrThread> threads = this.currentClrRuntime.Threads;

			if(liveThreadsOnly)
			{
				threads = threads.Where(t => t.IsAlive);
			}
			
			foreach(ClrThread thread in threads)
			{
				this.outputWriter.WriteLine("OS ThreadID: {0:X}", thread.OSThreadId);
				this.outputWriter.WriteLine("Managed ThreadID: {0:D}", thread.ManagedThreadId);
				this.outputWriter.WriteLine("Current Exception: {0}", thread.CurrentException != null ? thread.CurrentException.Type.Name : "<none>");
				this.outputWriter.WriteLine("Lock Count: {0}", thread.LockCount);
				// TODO: figure out good formatting for these attributes
				//this.outputWriter.WriteLine("IsAlive - IsBackground - IsThreadPoolWorker - IsSTA - IsFinalizer");
				//this.outputWriter.WriteLine("  {0}   -      {1}     -        {2}         -  {3}  -     {4}", ClrMdPack.YorN(thread.IsAlive), ClrMdPack.YorN(thread.IsBackground), ClrMdPack.YorN(thread.IsThreadpoolWorker), ClrMdPack.YorN(thread.IsSTA), ClrMdPack.YorN(thread.IsFinalizer));

				if(showCallstack)
				{
					this.outputWriter.WriteLine("Callstack:");

					if(thread.StackTrace.Count > 0)
					{
						foreach(ClrStackFrame frame in thread.StackTrace)
						{
							this.outputWriter.WriteLine("{0,12:X} {1,12:X} {2}", frame.InstructionPointer, frame.StackPointer, frame.DisplayString);
						}
					}
					else
					{
						this.outputWriter.WriteLine("<none>");
					}
				}

				this.outputWriter.WriteLineSeparator();
			}
		}

		private static string YorN(bool value)
		{
			return value ? "Y" : "N";
		}
	}
}
