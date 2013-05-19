﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;

namespace HackedBrain.ScriptCs.ClrMd
{
	partial class ClrMdPack
	{
		public void DumpAppDomains()
		{
			foreach(ClrAppDomain appDomain in this.currentClrRuntime.AppDomains)
			{
				this.outputWriter.WriteLine("Id: {0}", appDomain.Id);
				this.outputWriter.WriteLine("Name: {0}", appDomain.Name);
				this.outputWriter.WriteLine("Address: {0}", appDomain.Address);
				this.outputWriter.WriteLine("AppBase: {0}", appDomain.AppBase);
				this.outputWriter.WriteLine("Config File: {0}", appDomain.ConfigurationFile);

				this.outputWriter.WriteLineSeparator();
			}
		}

		public void DumpModules()
		{
			foreach(ClrAppDomain appDomain in this.currentClrRuntime.AppDomains)
			{
				this.DumpModulesForAppDomain(appDomain);
			}
		}

		public void DumpModules(int appDomainId)
		{
			Contract.Requires(appDomainId > 0);
			
			ClrAppDomain appDomain = this.currentClrRuntime.AppDomains.FirstOrDefault(ad => ad.Id == appDomainId);

			if(appDomain != null)
			{
				this.DumpModulesForAppDomain(appDomain);	
			}
			else
			{
				this.outputWriter.WriteLine("No AppDomain located with an Id of {0}.", appDomainId);
			}
		}

		public void DumpModules(string appDomainName)
		{
			Contract.Requires(!string.IsNullOrEmpty(appDomainName));

			ClrAppDomain appDomain = this.currentClrRuntime.AppDomains.FirstOrDefault(ad => ad.Name == appDomainName);

			if(appDomain != null)
			{
				this.DumpModulesForAppDomain(appDomain);
			}
			else
			{
				this.outputWriter.WriteLine("No AppDomain located with a Name of \"{0}\".", appDomainName);
			}
		}

		private void DumpModulesForAppDomain(ClrAppDomain appDomain)
		{
			Contract.Requires(appDomain != null);

			this.outputWriter.WriteLine("AppDomain: {0} ({1})", appDomain.Name, appDomain.Id);
			this.outputWriter.WriteLine(string.Empty);
			
			foreach(ClrModule module in appDomain.Modules)
			{
				string isDynamicIndicator;

				if(module.IsDynamic)
				{
					isDynamicIndicator = " (dynamic)";
				}
				else
				{
					isDynamicIndicator = string.Empty;
				}

				this.outputWriter.WriteLine("{0}{1} - {2}", module.Name, isDynamicIndicator, module.MetadataAddress);
			}
		}
	}
}