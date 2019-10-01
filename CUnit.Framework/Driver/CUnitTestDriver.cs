namespace CUnit.Framework.Driver
{
	using CUnit.Framework;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Terrasoft.Core;

	public class CUnitTestDriver
	{
		private CUnitTestRunningOptions _options { get; set; }

		public IUnitTestReporter Reporter { get; set; }

		public UserConnection UserConnection { get; set; }


		public CUnitTestDriver() {
			Reporter = CreateReporter();
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
			Stage stage = (Stage)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(testAssemblyItem.Name, typeof(Stage).FullName);
			RunTestAssembly(stage.LoadAssembly(testAssemblyItem.Content));
		}

		public void RunTestAssembly(Assembly assembly) {
			Reporter.Trace(assembly, (testResult) => {
				var testClasses = GetTypesWithAttribute(assembly, typeof(TestSuiteAttribute));
				testClasses.ForEach(testFixtureClassType => ExecuteTest(testFixtureClassType));
			});
		}

		protected abstract IUnitTestReporter CreateReporter();

		protected virtual List<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType) {
			return assembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Length > 0).ToList();
		}


		protected virtual void ExecuteTest(Type testFixtureClassType) {
			Reporter.Trace(testFixtureClassType, (testResult) => {
				if (IsIgnored(testFixtureClassType)) {
					testResult.Ignored = true;
				} else {
					testResult.Executed = true;
					var createMethod = GetGenericMethod(this.GetType(), testFixtureClassType, "CreateTestClassInstance");
					var instance = createMethod.Invoke(this, new object[] { });
					SetUserConnection(instance, UserConnection);
					InvokeSpecificMethodByAttribute(instance, typeof(SuiteSetUpAttribute));
					var methods = GetMethodsWithAttribute(testFixtureClassType, TestAttribute);
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
			Reporter.Trace(methodInfo, (testResult) => {
				if (IsIgnored(methodInfo)) {
					testResult.Ignored = true;
				} else {
					testResult.Executed = true;
					Reporter.Try(testResult, x => InvokeSpecificMethodByAttribute(instance, typeof(TestSetUpAttribute)));
					Reporter.Try(testResult, x => methodInfo.Invoke(instance, new object[] { }));
					Reporter.Try(testResult, x => InvokeSpecificMethodByAttribute(instance, typeof(TestTearDownAttribute)));
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
