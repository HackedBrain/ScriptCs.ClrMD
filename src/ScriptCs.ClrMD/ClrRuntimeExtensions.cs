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

			return ClrRuntimeExtensions.GetHeapStatsByTypeForObjectSet(clrRuntime, clrRuntime.GetHeap().EnumerateObjects());
		}

		public static IEnumerable<TypeHeapStat> GetFinalizerQueueHeapStatsByType(this ClrRuntime clrRuntime)
		{
			Contract.Requires(clrRuntime != null);

			return ClrRuntimeExtensions.GetHeapStatsByTypeForObjectSet(clrRuntime, clrRuntime.EnumerateFinalizerQueue());
		}

		private static IEnumerable<TypeHeapStat> GetHeapStatsByTypeForObjectSet(ClrRuntime clrRuntime, IEnumerable<ulong> objectIdSet)
		{
			ClrHeap heap = clrRuntime.GetHeap();

			return from oid in objectIdSet
				   let ot = heap.GetObjectType(oid)
				   group oid by ot into objectTypeGroup
				   let otSize = objectTypeGroup.Sum(oid => (uint)objectTypeGroup.Key.GetSize(oid))
				   select new TypeHeapStat
				   {
					   TypeName = objectTypeGroup.Key.Name,
					   TotalHeapSize = otSize,
					   NumberOfInstances = objectTypeGroup.Count()
				   };
		}
	}
}
