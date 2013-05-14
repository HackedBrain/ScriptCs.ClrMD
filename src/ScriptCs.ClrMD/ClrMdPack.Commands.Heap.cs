using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackedBrain.ScriptCs.ClrMd
{
	partial class ClrMdPack
	{
		public void DumpHeapStatsByType()
		{
			this.DumpHeapStatsByType(0, 0, null);
		}

		public void DumpHeapStatsByType(string typeName)
		{
			this.DumpHeapStatsByType(0, 0, typeName);
		}

		public void DumpHeapStatsByType(long minSize)
		{
			this.DumpHeapStatsByType(minSize, 0, null);
		}

		public void DumpHeapStatsByType(long minSize, string typeName)
		{
			this.DumpHeapStatsByType(minSize, 0, typeName);
		}

		public void DumpHeapStatsByType(long minSize, long maxSize, string typeName)
		{
			this.EnsureAttachedToProcess();

			IEnumerable<TypeHeapStat> typeHeapStats = this.currentClrRuntime.GetHeapStatsByType();

			this.DumpHeapStatsByType(minSize, maxSize, typeName, typeHeapStats);
		}

		public void DumpFinalizerQueueHeapStatsByType()
		{
			this.DumpFinalizerQueueHeapStatsByType(0, 0, null);
		}

		public void DumpFinalizerQueueHeapStatsByType(int minSize)
		{
			this.DumpFinalizerQueueHeapStatsByType(minSize, 0, null);
		}

		public void DumpFinalizerQueueHeapStatsByType(string typeName)
		{
			this.DumpFinalizerQueueHeapStatsByType(0, 0, typeName);
		}

		public void DumpFinalizerQueueHeapStatsByType(int minSize, string typeName)
		{
			this.DumpFinalizerQueueHeapStatsByType(minSize, 0, typeName);
		}

		public void DumpFinalizerQueueHeapStatsByType(long minSize, long maxSize, string typeName)
		{
			this.EnsureAttachedToProcess();

			IEnumerable<TypeHeapStat> typeHeapStats = this.currentClrRuntime.GetFinalizerQueueHeapStatsByType();

			this.DumpHeapStatsByType(minSize, maxSize, typeName, typeHeapStats);
		}

		private void DumpHeapStatsByType(long minSize, long maxSize, string typeName, IEnumerable<TypeHeapStat> typeHeapStats)
		{
			IEnumerable<TypeHeapStat> filteredResults = typeHeapStats;
			
			if(minSize > 0)
			{
				filteredResults = filteredResults.Where(ths => ths.TotalHeapSize >= minSize);
			}

			if(maxSize > 0)
			{
				filteredResults = filteredResults.Where(ths => ths.TotalHeapSize <= maxSize);
			}

			bool typeFilterSpecified = !string.IsNullOrWhiteSpace(typeName);

			if(typeFilterSpecified)
			{
				filteredResults = filteredResults.Where(ths => ths.TypeName.StartsWith(typeName.Trim()));
			}

			filteredResults = filteredResults.OrderBy(ths => ths.TotalHeapSize);

			List<TypeHeapStat> bufferedFilteredResults = filteredResults.ToList();

			this.outputWriter.WriteLine("Heap Stats By Type");
			this.outputWriter.WriteLineSeparator();
			this.outputWriter.WriteLine("Filter options:");
			this.outputWriter.WriteLine("  Min Heap Size: {0}", minSize);
			this.outputWriter.WriteLine("  Max Heap Size: {0}", maxSize == 0 ? "<unbounded>" : maxSize.ToString());
			this.outputWriter.WriteLine("    Type filter: {0}", typeFilterSpecified ? typeName : "<none>");
			this.outputWriter.WriteLine(string.Empty);

			foreach(TypeHeapStat typeHeapStat in bufferedFilteredResults)
			{
				this.outputWriter.WriteLine("{0,12:n0} {1,12:n0} {2}", typeHeapStat.TotalHeapSize, typeHeapStat.NumberOfInstances, typeHeapStat.TypeName);
			}

			this.outputWriter.WriteLineSeparator();

			if(typeFilterSpecified)
			{
				this.outputWriter.WriteLine("Total # of types matching filter: {0:n0}", bufferedFilteredResults.Count);
				this.outputWriter.WriteLine("Total size of filtered objects: {0:n0}", bufferedFilteredResults.Sum(ths => ths.TotalHeapSize));
			}

			this.outputWriter.WriteLine("Total Heap Size: {0:n0}", this.currentClrRuntime.GetHeap().TotalHeapSize);
		}
	}
}
