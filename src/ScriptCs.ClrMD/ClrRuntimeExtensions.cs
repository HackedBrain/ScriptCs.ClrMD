using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

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

		public static IEnumerable<ClrType> GetTypesInModule(this ClrRuntime clrRuntime, string moduleShortName)
		{
			return (from cm in clrRuntime.EnumerateModules()
					where cm.Name.StartsWith(moduleShortName)
					from t in cm.EnumerateTypes()
					select t);
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

		public static string GetShortName(this ClrModule module)
		{
			string moduleFullName = module.Name;
			
			int lastPathSeparatorIndex = moduleFullName.LastIndexOf(Path.DirectorySeparatorChar);

			if(lastPathSeparatorIndex == -1)
			{
				lastPathSeparatorIndex = moduleFullName.LastIndexOf(Path.AltDirectorySeparatorChar);
			}

			string result;

			if(lastPathSeparatorIndex > -1)
			{
				result = moduleFullName.Substring(lastPathSeparatorIndex + 1);
			}
			else
			{
				result = moduleFullName;
			}

			return result;
		}
	}
}
