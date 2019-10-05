namespace CUnit.Framework.Driver
{
	using CUnit.Framework.Core;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Terrasoft.Common;

	internal class TestRunningTracer: ITestRunningTracer
	{
		protected List<TestResult> _assemblies { get; set; }
		public List<TestResult> Assemblies { get => _assemblies ?? (_assemblies = new List<TestResult>()); }

		protected List<TestResult> _suites { get; set; }
		public List<TestResult> Suites { get => _suites ?? (_suites = new List<TestResult>()); }

		protected List<TestResult> _tests { get; set; }
		public List<TestResult> Tests { get => _tests ?? (_tests = new List<TestResult>()); }

		public void Trace(Assembly assembly, Action<TestResult> action) {
			TraceAssembly<TestResult>(assembly, (testResult) => {
				testResult.Assembly = assembly;
				Assemblies.Add(testResult);
				action.Invoke(testResult);
			});
		}

		public void Trace(Type type, Action<TestResult> action) {
			TraceClass<TestResult>(type, (testResult) => {
				testResult.Assembly = type.Assembly;
				testResult.Suite = type;
				Suites.Add(testResult);
				action.Invoke(testResult);
			});
		}

		public void Trace(MethodInfo methodInfo, Action<TestResult> action) {
			TraceMethod<TestResult>(methodInfo, (testResult) => {
				testResult.Test = methodInfo;
				testResult.Suite = methodInfo.DeclaringType;
				testResult.Assembly = methodInfo.DeclaringType.Assembly;
				Tests.Add(testResult);
				action.Invoke(testResult);
			});
		}

		/*internal virtual string GetReport() {
			return ReportBuilder.Build(this);
		}

		protected IUnitTestReportBuilder CreateReportBuilder();*/

		protected void TraceBeforeAssembly(TestResult testResult, Assembly assembly) {
		}

		protected void TraceAfterAssembly(TestResult testResult, Assembly assembly) {
		}

		protected void TraceBeforeSuite(TestResult testResult, Type testClass) {
		}

		protected void TraceAfterSuite(TestResult testResult, Type testClass) {
		}

		protected void TraceBeforeTest(TestResult testResult, MethodInfo methodInfo) {
		}

		protected void TraceAfterTest(TestResult testResult, MethodInfo methodInfo) {
			CollectAssertResults(testResult, methodInfo);
		}

		private void TraceAssembly<T>(Assembly assembly, Action<T> action) where T : TestResult, new() {
			Trace<T>((testResult) => {
				TraceBeforeAssembly(testResult, assembly);
				action.Invoke(testResult);
				TraceAfterAssembly(testResult, assembly);
			});
		}

		private void TraceClass<T>(Type testClass, Action<T> action) where T : TestResult, new() {
			Trace<T>((testResult) => {
				TraceBeforeSuite(testResult, testClass);
				action.Invoke(testResult);
				TraceAfterSuite(testResult, testClass);
			});
		}

		private void TraceMethod<T>(MethodInfo methodInfo, Action<T> action) where T : TestResult, new() {
			Trace<T>((testResult) => {
				TraceBeforeTest(testResult, methodInfo);
				action.Invoke(testResult);
				TraceAfterTest(testResult, methodInfo);
			});
		}

		public void Try(TestResult testResult, Action<TestResult> action) {
			try {
				action.Invoke(testResult);
			} catch (Exception e) {
				testResult.RuntimeExceptions.Add(GetRuntimeException(e));
			} finally {
				testResult.EndTime = DateTime.Now;
			}
		}

		private void Trace<T>(Action<T> action) where T : TestResult, new() {
			var testResult = new T();
			testResult.StartTime = DateTime.Now;
			Try(testResult, info => { action.Invoke((T)info); });
		}

		private RuntimeException GetRuntimeException(Exception e) {
			return new RuntimeException() {
				Message = e.Message,
				StackTrace = e.StackTrace,
				Type = e.GetType()
			};
		}

		private void CollectAssertResults(TestResult testResult, MethodInfo methodInfo) {
			var assetResults = TestContext.CurrentContext.AssertResults
					.Where(result => result.TestMethod.Name == methodInfo.Name &&
						result.TestMethod.DeclaringType.FullName == methodInfo.DeclaringType.FullName).ToList();
			testResult.Asserts = assetResults.Count;
			assetResults.Where(x => !x.Success).ForEach(x => {
				testResult.RuntimeExceptions.Add(new RuntimeException() {
					Message = x.ErrorMessage,
					StackTrace = x.StackTrace.ToString()
				});
			});
		}
	}
}
