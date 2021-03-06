﻿using System;

namespace HackedBrain.ScriptCs.ClrMd
{
	public class ConsoleOutputWriter : IOutputWriter
	{
		public ConsoleOutputWriter()
		{
		}

		#region IOutputWriter implemenation

		public void WriteLine(string output)
		{
			Console.WriteLine(output);
		}

		public void WriteLine(string outputFormat, params object[] formatValues)
		{
			Console.WriteLine(outputFormat, formatValues);
		}

		public void WriteLineSeparator()
		{
			Console.WriteLine(new string('-', Console.WindowWidth / 2));
		}

		#endregion
	}
}
