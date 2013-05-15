using System.Collections.Generic;
using System.Linq;
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

		public void DumpBlockedClrThreads()
		{
			IEnumerable<ClrThread> threads = this.currentClrRuntime.Threads;

			threads = threads.Where(t => t.BlockingObjects != null && t.BlockingObjects.Count > 0);

			if(threads.Any())
			{
				ClrHeap heap = this.ClrRuntime.GetHeap();

				foreach(ClrThread thread in threads)
				{
					this.outputWriter.WriteLine("OS ThreadID: {0:X}", thread.OSThreadId);
					this.outputWriter.WriteLine("Managed ThreadID: {0:D}", thread.ManagedThreadId);

					foreach(BlockingObject blockingObject in thread.BlockingObjects)
					{
						ClrType blockingObjectType = heap.GetObjectType(blockingObject.Object);

						this.outputWriter.WriteLine("{0:12X} {1}", blockingObject.Object, blockingObjectType.Name);
					}

					this.outputWriter.WriteLineSeparator();
				}
			}
			else
			{
				this.outputWriter.WriteLine("No threads are blocked at this time.");
			}
		}

		private static string YorN(bool value)
		{
			return value ? "Y" : "N";
		}
	}
}
