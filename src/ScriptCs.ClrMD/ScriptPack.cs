﻿using ScriptCs.Contracts;

namespace HackedBrain.ScriptCs.ClrMd
{
	public class ScriptPack : IScriptPack
	{
		private ClrMdPack clrMdPack;
		
		IScriptPackContext IScriptPack.GetContext()
		{
			if(this.clrMdPack == null)
			{
				this.clrMdPack = new ClrMdPack();
			}

			return this.clrMdPack;
		}

		void IScriptPack.Initialize(IScriptPackSession session)
		{
			session.ImportNamespace("Microsoft.Diagnostics.Runtime");
			session.ImportNamespace("HackedBrain.ScriptCs.ClrMd");
		}

		void IScriptPack.Terminate()
		{
			if(this.clrMdPack != null
					&&
				this.clrMdPack.IsAttached)
			{
				this.clrMdPack.Detatch();
				this.clrMdPack = null;
			}
		}
	}
}
