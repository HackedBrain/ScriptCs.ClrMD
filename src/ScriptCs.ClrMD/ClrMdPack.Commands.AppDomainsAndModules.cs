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
			foreach(ClrAppDomain appDomain in this)
		}
	}
}
