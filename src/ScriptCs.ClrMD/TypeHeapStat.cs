using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HackedBrain.ScriptCs.ClrMd
{
	public struct TypeHeapStat
	{
		public string TypeName
		{
			get;
			set;
		}

		public long TotalHeapSize
		{
			get;
			set;
		}

		public int NumberOfInstances
		{
			get;
			set;
		}
	}
}
