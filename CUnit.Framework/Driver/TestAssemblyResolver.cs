namespace CUnit.Framework.Driver
{
	using System;
    using System.Collections.Generic;
    using System.Reflection;
	using System.Web;

	internal class TestAssemblyResolver : IDisposable
	{
		private string _baseResolvePath;

		private List<string> _pathList;

		private ResolveEventHandler _resolveEventHandler;

		internal TestAssemblyResolver(string baseResolvePath) {
			_baseResolvePath = baseResolvePath;
		}

		public void Dispose() {
			if (_resolveEventHandler != null) {
				AppDomain.CurrentDomain.AssemblyResolve += _resolveEventHandler;
				FlashLoadedLibraries();
			}
		}

		internal void FlashLoadedLibraries() {
			HttpRuntime.UnloadAppDomain();
		}

		private Assembly TestResolveEventHandler(object sender, ResolveEventArgs args) {
			return null;
			/*var fileName = args.Name.Split(',')[0];
			var assemblyPath = GetFullAssemblyPath(fileName, FileSufix.DLL);
			var pdbPath = GetFullAssemblyPath(fileName, FileSufix.PDB);
			byte[] assemblyRaw = File.Exists(assemblyPath)
				? File.ReadAllBytes(assemblyPath)
				: null;
			byte[] pdbRaw = File.Exists(pdbPath)
				? File.ReadAllBytes(pdbPath)
				: null;
			return assemblyRaw != null
				? Assembly.Load(assemblyRaw, pdbRaw, SecurityContextSource.CurrentAppDomain)
				: null;*/
		}
	}
}