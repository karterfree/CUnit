namespace CUnit.Framework.Driver
{
	using CUnit.Framework;
    using CUnit.Framework.Driver.Report;
    using System;
	using System.Collections.Generic;
    using System.IO;
    using System.Linq;
	using System.Reflection;
	using Terrasoft.Core;

	public class CUnitTestDriver
	{
		private CUnitTestRunningOptions _options { get; set; }

		private ITestRunningTracer _tracer { get; set; }
		internal ITestRunningTracer Tracer { get => _tracer ?? (_tracer = new TestRunningTracer()); }

		private ITestReportBuilder _reportBuilder { get; set; }
		internal ITestReportBuilder ReportBuilder { get => _reportBuilder ?? (_reportBuilder = CreateReportBuilder()); }

		public UserConnection UserConnection { get; set; }

		private ITestReportBuilder CreateReportBuilder() {
			return new NUnitReportBuilder() {
				SourceTracer = Tracer
			};
		}

		public void Run(CUnitTestRunningOptions options) {
			_options = options;
			var manager = new TestAssemblyManager(_options.ResolvePath, _options.TestAssemblyFileMask);
			manager.Items.ForEach(x => Run(x));
		}

		private void Run(TestAssemblyItem testAssemblyItem) {
			using (var resolver = new TestAssemblyResolver(testAssemblyItem.ResolveFolder)) {
				RunTestAssembly(testAssemblyItem);
			}
		}

		private void RunTestAssembly(TestAssemblyItem testAssemblyItem) {
			Stage stage = (Stage)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(typeof(Stage).Assembly.FullName, typeof(Stage).FullName);
			RunTestAssembly(stage.LoadAssembly(File.ReadAllBytes(testAssemblyItem.FullName)));
		}

		public void RunTestAssembly(Assembly assembly) {
			Tracer.Trace(assembly, (testResult) => {
				var testClasses = GetTypesWithAttribute(assembly, typeof(TestSuiteAttribute));
				testClasses.ForEach(testFixtureClassType => ExecuteTest(testFixtureClassType));
			});
		}

		public string GetReport() {
			return ReportBuilder.BuildReport();
		}

		protected virtual List<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType) {
			return assembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Length > 0).ToList();
		}


		protected virtual void ExecuteTest(Type testFixtureClassType) {
			Tracer.Trace(testFixtureClassType, (testResult) => {
				if (IsIgnored(testFixtureClassType)) {
					testResult.Ignored = true;
				} else {
					testResult.Executed = true;
					var createMethod = GetGenericMethod(this.GetType(), testFixtureClassType, "CreateTestClassInstance");
					var instance = createMethod.Invoke(this, new object[] { });
					SetUserConnection(instance, UserConnection);
					InvokeSpecificMethodByAttribute(instance, typeof(SuiteSetUpAttribute));
					var methods = GetMethodsWithAttribute(testFixtureClassType, typeof(TestAttribute));
					methods.ForEach(methodInfo => ExecuteMethod(instance, methodInfo));
					InvokeSpecificMethodByAttribute(instance, typeof(SuiteTearDownAttribute));
				}
			});
		}

		protected virtual MethodInfo GetGenericMethod(Type type, Type genericType, string methodName) {
			MethodInfo response = type
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(method => method.Name == methodName && method.ContainsGenericParameters)
				.FirstOrDefault();
			return response?.MakeGenericMethod(genericType);
		}

		protected virtual T CreateTestClassInstance<T>() {
			return (T)Activator.CreateInstance<T>();
		}

		protected virtual void InvokeSpecificMethodByAttribute(object instance, Type attributeType) {
			if (attributeType == null) {
				return;
			}
			var method = GetMethodsWithAttribute(instance.GetType(), attributeType).FirstOrDefault();
			if (method != null) {
				method.Invoke(instance, new object[] { });
			}
		}

		protected virtual List<MethodInfo> GetMethodsWithAttribute(Type type, Type attributeType) {
			return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(m => m.GetCustomAttributes(attributeType, false).Length > 0).ToList();
		}

		protected virtual void ExecuteMethod(object instance, MethodInfo methodInfo) {
			Tracer.Trace(methodInfo, (testResult) => {
				if (IsIgnored(methodInfo)) {
					testResult.Ignored = true;
				} else {
					testResult.Executed = true;
					Tracer.Try(testResult, x => InvokeSpecificMethodByAttribute(instance, typeof(TestSetUpAttribute)));
					Tracer.Try(testResult, x => methodInfo.Invoke(instance, new object[] { }));
					Tracer.Try(testResult, x => InvokeSpecificMethodByAttribute(instance, typeof(TestTearDownAttribute)));
				}
			});
		}

		protected virtual void SetUserConnection<T>(T testClassInstance, UserConnection userConnection) {
			PropertyInfo propertyInfo = testClassInstance.GetType().GetProperty("UserConnection");
			if (propertyInfo != null) {
				propertyInfo.SetValue(testClassInstance, Convert.ChangeType(userConnection, propertyInfo.PropertyType), null);
			}
		}

		protected virtual bool IsIgnored(Type testSuite) {
			return testSuite.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0;
		}

		protected virtual bool IsIgnored(MethodInfo testCase) {
			return testCase.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0;
		}


		
	}
}
