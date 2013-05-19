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
		public void DumpAppDomains()
		{
			foreach(ClrAppDomain appDomain in this.currentClrRuntime.AppDomains)
			{
				this.outputWriter.WriteLine("Id: {0}", appDomain.Id);
				this.outputWriter.WriteLine("Name: {0}", appDomain.Name);
				this.outputWriter.WriteLine("Address: {0}", appDomain.Address);
				this.outputWriter.WriteLine("AppBase: {0}", appDomain.AppBase);
				this.outputWriter.WriteLine("Config File: {0}", appDomain.ConfigurationFile);

				this.outputWriter.WriteLineSeparator();
			}
		}
	}
}
