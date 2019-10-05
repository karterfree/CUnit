namespace CUnit.Framework.Asserts
{
	using CUnit.Framework.Core;

	internal class CommonСriterion<TValue>: BaseСriterion<TValue>, ICommonСriterion<TValue>
	{
		public void BeEqual(TValue value, string errorMessage = "") {
			customErrorMessage = errorMessage;
			expectedValue = value;
			That(() => TestEqual());
		}

		public void NotBeEqual(TValue value, string errorMessage = "") {
			customErrorMessage = errorMessage;
			expectedValue = value;
			That(() => TestEqual(true));
		}

		public void BeNull(string errorMessage = "") {
			customErrorMessage = errorMessage;
			That(() => TestNull());
		}

		public void NotBeNull(string errorMessage = "") {
			customErrorMessage = errorMessage;
			That(() => TestNull(true));
		}

		private AssertResult TestEqual(bool not = false) {
			bool success = not ^ Equals(TestValue, expectedValue);
			string errorMessage = $"Expect: <{ValueToString(expectedValue)}>. Got: <{ValueToString(TestValue)}> ";
			return GetTestResult(AssertResultType.Equal, success, errorMessage);
		}

		private AssertResult TestNull(bool not = false) {
			bool success = not ^ TestValue == null;
			string errorMessage = $"Expect: <{ValueToString(null)}>. Got: <{ValueToString(TestValue)}> ";
			return GetTestResult(AssertResultType.Null, success, errorMessage);
		}
	}
}
