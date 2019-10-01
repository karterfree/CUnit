namespace CUnit.Framework.Assets
{
	using CUnit.Framework.Core;
	using System;

	internal class ActionСriterion : BaseСriterion<object>, IActionСriterion
	{
		public void Throw<TException>(Action action, string errorMessage = "") where TException : Exception {
			customErrorMessage = errorMessage;
			That(() => TestThrow<TException>(action));
		}

		public void NotThrow(Action action, string errorMessage = "") {
			customErrorMessage = errorMessage;
			That(() => TestThrow(action));
		}

		private AssertResult TestThrow<TException>(Action action) where TException : Exception {
			bool success = false;
			try {
				action.Invoke();
			} catch (TException e) {
				success = true;
				TestValue = e.GetType().FullName;
			} catch (Exception e) {
				success = false;
				TestValue = e.GetType().FullName;
			} finally {
				expectedValue = typeof(TException).FullName;
			}

			string errorMessage = $"Expected exception: <{expectedValue}>. Got exception: <{ValueToString(TestValue)}> ";
			return GetTestResult(AssertResultType.Throw, success, errorMessage);
		}

		//todo: rename
		private AssertResult TestThrow(Action action) {
			bool success = true;
			try {
				action.Invoke();
			} catch (Exception e) {
				success = false;
				TestValue = e.GetType().FullName;
			} finally {
				expectedValue = null;
			}

			string errorMessage = success
				? $"Did not get exception."
				: $"Did not expect exception. Got exception: <{ValueToString(TestValue)}> ";
			return GetTestResult(AssertResultType.Throw, success, errorMessage);
		}
	}
}
