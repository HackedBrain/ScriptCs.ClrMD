using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using System.Diagnostics.Contracts;

namespace HackedBrain.ScriptCs.ClrMd
{
	public static class ClrRuntimeExtensions
	{
		public static IEnumerable<TypeHeapStat> GetHeapStatsByType(this ClrRuntime clrRuntime)
		{
			Contract.Requires(clrRuntime != null);

			ClrHeap heap = clrRuntime.GetHeap();

			IEnumerable<TypeHeapStat> results = from oid in heap.EnumerateObjects()
												let ot = heap.GetObjectType(oid)
												group oid by ot into objectTypeGroup
												let otSize = objectTypeGroup.Sum(oid => (uint)objectTypeGroup.Key.GetSize(oid))
												orderby otSize
												select new TypeHeapStat
												{
													TypeName = objectTypeGroup.Key.Name,
													TotalHeapSize = otSize,
													NumberOfInstances = objectTypeGroup.Count()
												};

			return results;
		}

		public static IEnumerable<TypeHeapStat> GetHeapStatsByType(this ClrRuntime clrRuntime, string typeNameFilter)
		{
			IEnumerable<TypeHeapStat> results = clrRuntime.GetHeapStatsByType();

			results = results.Where(hs => hs.TypeName.StartsWith(typeNameFilter));

			return results;
		}
	}
}
