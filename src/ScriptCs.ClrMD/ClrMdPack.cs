using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using ScriptCs.Contracts;

namespace HackedBrain.ScriptCs.ClrMd
{
	public partial class ClrMdPack : IScriptPackContext
	{
		private const int DefaultAttachWaitTimeMilliseconds = 5000;		
		
		public ClrRuntime AttachToProcess(string processName)
		{
			return this.AttachToProcess(processName, ClrMdPack.DefaultAttachWaitTimeMilliseconds);
		}
		
		public ClrRuntime AttachToProcess(string processName, int attachWaitTimeMilliseconds)
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

			return this.AttachToProcess(processes[0].Id);
		}

		public ClrRuntime AttachToProcess(int processId)
		{
			return this.AttachToProcess(processId, ClrMdPack.DefaultAttachWaitTimeMilliseconds);
		}

		public ClrRuntime AttachToProcess(int processId, int attachWaitTimeMilliseconds)
		{
			DataTarget dataTarget = DataTarget.AttachToProcess(processId, (uint)attachWaitTimeMilliseconds);

			// Make sure the CLR is even found
			if(dataTarget.ClrVersions.Count == 0)
			{
				throw new InvalidOperationException(string.Format("The specified process ({0}) does not appear to have the CLR loaded in it.", processId));
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
				throw new InvalidOperationException(string.Format("Unable to locate the DAC for the target version of the CLR runtime ({1}).", processId, clrInfo.Version));
			}

			return dataTarget.CreateRuntime(dacLocation);
		}
	}
}
