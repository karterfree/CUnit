namespace CUnit.Framework.Assets
{
	using CUnit.Framework.Core;
	using System;
	using System.Diagnostics;
	using System.Reflection;

	internal class BaseСriterion<TValue>
	{
		internal TValue TestValue { get; set; }

		protected TValue expectedValue;

		protected string customErrorMessage;

		internal StackTrace AssertStackTrace { get; set; }

		protected internal void That(Func<AssertResult> func) {
			var testResult = func.Invoke();
			TestContext.CurrentContext.AssertResults.Add(testResult);
			Debug.WriteLine($"Result: {testResult.Success}. {testResult.ErrorMessage}");
		}

		protected internal AssertResult GetTestResult(AssertResultType type, bool success, string errorMessage) {
			return new AssertResult {
				Type = type,
				TestMethod = GetTestMethod(AssertStackTrace),
				StackTrace = success ? null : AssertStackTrace,
				Success = success,
				Value = TestValue,
				ExpectedValue = expectedValue,
				ErrorMessage = string.IsNullOrEmpty(customErrorMessage)
					? errorMessage
					: customErrorMessage
			};
		}

		protected internal MethodBase GetTestMethod(StackTrace stackTrace) {
			return stackTrace.GetFrame(0).GetMethod();
		}

		protected internal string ValueToString(object value) {
			if (value == null) {
				return "NULL";
			} else if (value.GetType() == typeof(string) && string.IsNullOrEmpty(value.ToString())) {
				return "\"\"";
			} else {
				return value.ToString();
			}
		}
	}
}
