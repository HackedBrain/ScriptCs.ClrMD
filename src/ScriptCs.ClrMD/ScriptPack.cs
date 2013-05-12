using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCs.Contracts;

namespace HackedBrain.ScriptCs.ClrMd
{
	public class ScriptPack : IScriptPack
	{
		IScriptPackContext IScriptPack.GetContext()
		{
			return new ClrMdPack();
		}

		void IScriptPack.Initialize(IScriptPackSession session)
		{
			session.ImportNamespace("Microsoft.Diagnostics.Runtime");
			session.ImportNamespace("HackedBrain.ScriptCs.ClrMd");
		}

		void IScriptPack.Terminate()
		{
		}
	}
}
