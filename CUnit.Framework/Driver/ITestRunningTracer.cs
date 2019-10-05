using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CUnit.Framework.Driver
{
	internal interface ITestRunningTracer
	{
		List<TestResult> Assemblies { get; }

		List<TestResult> Suites { get; }

		List<TestResult> Tests { get; }

		void Trace(Assembly assembly, Action<TestResult> action);

		void Trace(Type type, Action<TestResult> action);

		void Trace(MethodInfo methodInfo, Action<TestResult> action);

		void Try(TestResult testResult, Action<TestResult> action);
	}
}
