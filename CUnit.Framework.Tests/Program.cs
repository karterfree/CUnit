using CUnit.Framework.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CUnit.Framework.Tests
{
	class Program
	{
		static void Main(string[] args) {

			var test = new TestSuite();
			test.Test();

			var driver = new CUnitTestDriver();
			driver.Run(new CUnitTestRunningOptions() {
				TestAssemblyFileMask = "*Examples*",
				ResolvePath = @"c:\Projects\git\CUnit\CUnit.Examples\bin\Debug\",
			});
			var report = driver.GetReport();
		}
	}
}
