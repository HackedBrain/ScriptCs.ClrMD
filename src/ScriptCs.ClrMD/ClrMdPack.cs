using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using ScriptCs.Contracts;

namespace HackedBrain.ScriptCs.ClrMd
{
	public partial class ClrMdPack : IScriptPackContext
	{
		private const int DefaultAttachWaitTimeMilliseconds = 5000;

		private DataTarget currentDataTarget;
		private ClrRuntime currentClrRuntime;
		private Process currentProcess;
		private IOutputWriter outputWriter;

		public ClrMdPack()
		{
			this.outputWriter = new ConsoleOutputWriter();
		}

		public bool IsAttached
		{
			get
			{
				return this.AttachedProcess != null;
			}
		}

		public ClrRuntime ClrRuntime
		{
			get
			{
				return this.currentClrRuntime;
			}
		}

		public Process AttachedProcess
		{
			get
			{
				return this.currentProcess;
			}
		}

		public ClrRuntime Attach(string processName)
		{
			return this.Attach(processName, ClrMdPack.DefaultAttachWaitTimeMilliseconds);
		}

		public ClrRuntime Attach(string processName, int attachWaitTimeMilliseconds)
		{
			Process[] processes = Process.GetProcessesByName(processName);

			if(processes.Length == 0)
			{
				throw new ArgumentException(string.Format("No process with the name \"{0}\" appears to be running.", processName));
			}
			else if(processes.Length > 1)
			{
				throw new InvalidOperationException(string.Format("Multiple processes ({0}) with the name \"{1}\" are currently running. Please use AttachToProcess overload specifying process Id instead.", processes.Length, processName));
			}

			return this.Attach(processes[0], ClrMdPack.DefaultAttachWaitTimeMilliseconds);
		}

		public ClrRuntime Attach(int processId)
		{
			Process processToAttachTo = Process.GetProcessById(processId);

			return this.Attach(processToAttachTo, ClrMdPack.DefaultAttachWaitTimeMilliseconds);
		}

		public ClrRuntime Attach(int processId, int attachWaitTimeMilliseconds)
		{
			Process processToAttachTo = Process.GetProcessById(processId);

			return this.Attach(processToAttachTo, attachWaitTimeMilliseconds);
		}

		public ClrRuntime Attach(Process process, int attachWaitTimeMilliseconds)
		{
			if(this.currentProcess != null)
			{
				throw new InvalidOperationException(string.Format("Already attached to process {0}:{1}, use Detach() first.", this.currentProcess.ProcessName, this.currentProcess.Id));
			}

			DataTarget dataTarget = DataTarget.AttachToProcess(process.Id, (uint)attachWaitTimeMilliseconds);

			// Make sure the CLR is even found
			if(dataTarget.ClrVersions.Count == 0)
			{
				throw new InvalidOperationException(string.Format("The specified process {0}:{1} does not appear to have the CLR loaded in it.", process.ProcessName, process.Id));
			}
			else if(dataTarget.ClrVersions.Count > 1)
			{
				// REVISIT: what happens if there's multiple ClrVersions?
			}

			ClrInfo clrInfo = dataTarget.ClrVersions[0];

			string dacLocation = clrInfo.TryGetDacLocation();

			// Make sure we found the DAC location, otherwise we can't create the runtime
			if(string.IsNullOrEmpty(dacLocation))
			{
				throw new InvalidOperationException(string.Format("Unable to locate the DAC for the target version of the CLR runtime ({0}) for process {1}:{2}.", clrInfo.Version, process.ProcessName, process.Id));
			}

			// Make sure that if we're unloaded we don't kill off the process
			dataTarget.DebuggerInterface.SetProcessOptions(DEBUG_PROCESS.DETACH_ON_EXIT);

			this.currentClrRuntime = dataTarget.CreateRuntime(dacLocation);
			this.currentDataTarget = dataTarget;
			this.currentProcess = process;

			this.outputWriter.WriteLine("Successfully attached to {0}:{1}...", process.ProcessName, process.Id);
			this.outputWriter.WriteLine("CLR Version: {0}", clrInfo.Version);

			return this.currentClrRuntime;
		}

		public void Detach()
		{
			this.EnsureAttachedToProcess();

			this.currentDataTarget.DebuggerInterface.DetachProcesses();
			this.currentDataTarget.Dispose();

			this.currentProcess = null;
			this.currentClrRuntime = null;
			this.currentDataTarget = null;
		}

		private void EnsureAttachedToProcess()
		{
			if(this.currentProcess == null)
			{
				throw new InvalidOperationException("Not attached to any process right now.");
			}
		}
	}
}
