using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HackedBrain.ScriptCs.ClrMd
{
	public interface IOutputWriter
	{
		void WriteLine(string output);
		void WriteLine(string outputFormat, params object[] formatValues);
		void WriteLineSeparator();
	}
}
