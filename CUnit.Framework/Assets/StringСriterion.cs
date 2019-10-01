namespace CUnit.Framework.Assets
{
	using CUnit.Framework.Core;

	internal class StringСriterion : CommonСriterion<string>, IStringСriterion
	{
		public void BeEmpty(string errorMessage = "") {
			customErrorMessage = errorMessage;
			That(() => TestEmpty());
		}

		public void NotBeEmpty(string errorMessage = "") {
			That(() => TestEmpty(true));
		}

		public void Contains(char value, string errorMessage = "") {
			customErrorMessage = errorMessage;
			expectedValue = value.ToString();
			That(() => TestContains());
		}

		public void Contains(string value, string errorMessage = "") {
			customErrorMessage = errorMessage;
			expectedValue = value;
			That(() => TestContains(true));
		}

		private AssertResult TestEmpty(bool not = false) {
			bool success = TestValue != null && (not ^ string.IsNullOrEmpty(TestValue));
			string errorMessage = $"Expect: <{ValueToString(expectedValue)}>. Got: <{ValueToString(TestValue)}> ";
			return GetTestResult(AssertResultType.Empty, success, errorMessage);
		}

		private AssertResult TestContains(bool not = false) {
			bool success = TestValue != null && (not ^ TestValue.IndexOf(expectedValue) >= 0);
			string errorMessage = $"Expect: <{ValueToString(expectedValue)}>. Got: <{ValueToString(TestValue)}> ";
			return GetTestResult(AssertResultType.Empty, success, errorMessage);
		}
	}
}
