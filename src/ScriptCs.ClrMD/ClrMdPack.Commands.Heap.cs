using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

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

		public void DumpObjects(string typeName)
		{
			ClrHeap clrHeap = this.ClrRuntime.GetHeap();
			
			IEnumerable<ulong> objectIds = clrHeap.EnumerateObjects();

			var objects = (from objectId in objectIds
						  let type = clrHeap.GetObjectType(objectId)
						  let heapGeneration = clrHeap.GetGeneration(objectId)
						  let heapSize = (long)type.GetSize(objectId)
						  where type.Name.StartsWith(typeName)
						  select new
						  {
							  ObjectId = objectId,
							  ObjectType = type,
							  HeapGeneration = heapGeneration,
							  HeapSize = heapSize
						  }).ToList();

			foreach(var objectData in objects)
			{
				this.outputWriter.WriteLine("0x{0:x12} {1:n0} {2}", objectData.ObjectId, objectData.HeapGeneration, objectData.ObjectType.Name);
			}

			this.outputWriter.WriteLineSeparator();
			this.outputWriter.WriteLine("Total Objects: {0:n0}", objects.Count);
			this.outputWriter.WriteLine("Total Heap Size Consumed: {0:n0}", objects.Sum(o => o.HeapSize));
		}

		public void DumpObject(ulong objectRef)
		{
			this.DumpObject(objectRef, false);
		}

		public void DumpObject(ulong objectRef, bool dumpFields)
		{
			ClrHeap clrHeap = this.ClrRuntime.GetHeap();

			if(clrHeap.IsInHeap(objectRef))
			{
				ClrType objectType = clrHeap.GetObjectType(objectRef);

				this.outputWriter.WriteLine("Address: 0x{0:x12}", objectRef);
				this.outputWriter.WriteLine("Type: {0} (IsValueClass={1})", objectType.Name, objectType.IsValueClass);
				this.outputWriter.WriteLine("Heap Size: {0:n0}", objectType.GetSize(objectRef));
				this.outputWriter.WriteLine("Heap Generation: {0:n0}", clrHeap.GetGeneration(objectRef));

				if(objectType.IsArray)
				{
					this.outputWriter.WriteLine("Array Length: {0:n0}", objectType.GetArrayLength(objectRef));
					this.outputWriter.WriteLine(string.Empty);
					this.outputWriter.WriteLine("* Use DumpArray(0x{0:x12}) to see individual array values.", objectRef);
				}
				else
				{
					if(objectType.HasSimpleValue)
					{
						this.outputWriter.WriteLine("Value: {0}", objectType.GetValue(objectRef));
					}
					
					if(dumpFields)
					{
						this.outputWriter.WriteLine(string.Empty);
						this.outputWriter.WriteLine("Fields:");

						foreach(ClrInstanceField field in objectType.Fields)
						{
							string fieldOutputValue;

							if(field.HasSimpleValue)
							{
								fieldOutputValue = field.GetFieldValue(objectRef).ToString();
							}
							else
							{
								fieldOutputValue = field.GetFieldAddress(objectRef).ToString("x12");
							}

							this.outputWriter.WriteLine("  {0,25}[{1}]: {2}", field.Name, field.Type.Name, fieldOutputValue);
						}
					}
				}
			}
			else
			{
				this.outputWriter.WriteLine("Specified object reference not found on heap!");
			}
		}

		public void DumpArray(ulong objectRef)
		{
			this.DumpArray(objectRef, 0, int.MaxValue, false);
		}

		public void DumpArray(ulong objectRef, int startIndex, int length)
		{
			this.DumpArray(objectRef, 0, length, false);
		}

		public void DumpArray(ulong objectRef, int startIndex, bool dumpFields)
		{
			this.DumpArray(objectRef, 0, int.MaxValue, dumpFields);
		}

		public void DumpArray(ulong objectRef, int startIndex, int length, bool dumpFields)
		{
			ClrHeap clrHeap = this.ClrRuntime.GetHeap();

			if(clrHeap.IsInHeap(objectRef))
			{
				ClrType objectType = clrHeap.GetObjectType(objectRef);

				if(objectType.IsArray)
				{
					this.outputWriter.WriteLine("Address: 0x{0:x12}", objectRef);
					this.outputWriter.WriteLine("Type: {0}", objectType.Name, objectType.IsValueClass);
					this.outputWriter.WriteLine("Heap Size: {0:n0}", objectType.GetSize(objectRef));
					this.outputWriter.WriteLine("Heap Generation: {0:n0}", clrHeap.GetGeneration(objectRef));
					this.outputWriter.WriteLine("Array Length: {0:n0}", objectType.GetArrayLength(objectRef));
					this.outputWriter.WriteLine(string.Empty);

					if(length == int.MaxValue)
					{
						length = objectType.GetArrayLength(objectRef) - startIndex;
					}

					bool arrayComponentTypeIsObjectReference = objectType.ArrayComponentType.IsObjectReference;

					for(int index = startIndex; index < startIndex + length; index++)
					{
						this.outputWriter.WriteLine("Element at index #{0:n0}", index);
						this.outputWriter.WriteLineSeparator();

						if(!arrayComponentTypeIsObjectReference)
						{
							this.outputWriter.WriteLine("{0}", objectType.GetArrayElementValue(objectRef, index));
						}
						else
						{
							ulong elementObjectRef = (ulong)objectType.GetArrayElementValue(objectRef, index);
							
							if(elementObjectRef != 0)
							{
								this.DumpObject(elementObjectRef, dumpFields);
							}
							else
							{
								this.outputWriter.WriteLine("<null>");
							}
						}

						this.outputWriter.WriteLineSeparator();
					}
				}
				else
				{
					this.outputWriter.WriteLine("Specified object reference is not an array type, it's a {0}.", objectType.Name);
				}
			}
			else
			{
				this.outputWriter.WriteLine("Specified object reference not found on heap!");
			}
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

			ClrHeap clrHeap = this.currentClrRuntime.GetHeap();

			this.outputWriter.WriteLine("Total Heap Size: {0:n0}", clrHeap.TotalHeapSize);
			;
			this.outputWriter.WriteLine("(Gen 0: {1:n0}, Gen 1: {1:n0}; Gen 2: {2:n0}; Gen 3: {3:n0})", clrHeap.GetSizeByGen(0), clrHeap.GetSizeByGen(1), clrHeap.GetSizeByGen(2), clrHeap.GetSizeByGen(3));
		}
	}
}
