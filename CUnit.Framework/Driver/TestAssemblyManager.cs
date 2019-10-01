namespace CUnit.Framework.Driver
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
    using System.Reflection;

    internal class TestAssemblyManager
	{
		private List<TestAssemblyItem> _items;
		internal List<TestAssemblyItem> Items { get => _items; }

		internal string BasePath { get; private set; }

		internal string Mask { get; private set; }

		internal TestAssemblyManager(string basePath, string mask) {
			BasePath = basePath;
			Mask = mask;
			_items = new List<TestAssemblyItem>();
			AddItemsByPath(BasePath);
		}

		private void AddItemsByPath(string path) {
			_items.AddRange(FetchItemsByPath(path));
			foreach (var folderPath in FetchFoldersByPath(path)) {
				AddItemsByPath(folderPath);
			}
		}

		private List<TestAssemblyItem> FetchItemsByPath(string path) {
			var directoryInfo = new DirectoryInfo(path);
			return GetAllowedFiles(directoryInfo)
				.Select(info => CreateTestAssemblyItem(info))
				.ToList();
		}

		private List<FileInfo> GetAllowedFiles(DirectoryInfo directoryInfo) {
			return directoryInfo.GetFiles().Where(f => IsAssembly(f) && IsAlloweByMask(f)).ToList();
		}

		private List<string> FetchFoldersByPath(string path) {
			var info = new DirectoryInfo(path);
			return info.GetDirectories().Select(d => d.FullName).Where(x => x != null).ToList();
		}

		private bool IsAssembly(FileInfo fileInfo) {
			return fileInfo.Extension == FileSufix.DLL;
		}
		
		private bool IsAlloweByMask(FileInfo fileInfo) {
			return true;
		}

		private TestAssemblyItem CreateTestAssemblyItem(FileInfo fileInfo) {
			var content = File.ReadAllBytes(fileInfo.FullName);
			var assembly = Assembly.Load(content);
			return HasTestSuites(assembly)
				? new TestAssemblyItem() {
					Name = assembly.FullName,
					Content = content,
					ResolveFolder = fileInfo.DirectoryName
				}
				: null;
		}

		private bool HasTestSuites(Assembly assembly) {
			return assembly != null
				? GetTestSuiteClasses(assembly).Any()
				: false;
		}

		private List<string> GetTestSuiteClasses(Assembly assembly) {
			var testSuiteAttributeType = typeof(TestSuiteAttribute);
			return assembly.GetTypes()
				.Where(type => type.GetCustomAttributes(testSuiteAttributeType, true).Length > 0)
				.Select(x => x.FullName).ToList();
		}

	}
}
