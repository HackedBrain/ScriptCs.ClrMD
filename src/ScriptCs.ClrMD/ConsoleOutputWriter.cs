using System;
using ScriptCs.Contracts;

namespace HackedBrain.ScriptCs.ClrMd
{
	public class ConsoleOutputWriter : IOutputWriter
	{
		private IConsole console;
		
		public ConsoleOutputWriter(IConsole console)
		{
			this.console = console;
		}

		#region IOutputWriter implemenation

		public void WriteLine(string output)
		{
			this.console.WriteLine(output);
		}

		public void WriteLine(string outputFormat, params object[] formatValues)
		{
			this.console.WriteLine(string.Format(outputFormat, formatValues));
		}

		public void WriteLineSeparator()
		{
			this.console.WriteLine("--------------------");
		}

		#endregion
	}
}
