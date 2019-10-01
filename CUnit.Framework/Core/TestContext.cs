namespace CUnit.Framework.Core
{
    using CUnit.Framework.Assets;
    using System;
	using System.Collections.Generic;

	internal class TestContext
	{
		private List<AssertResult> _assertResults;
		internal List<AssertResult> AssertResults { get => _assertResults ?? (_assertResults = new List<AssertResult>()); }

		private static TestContext _currentContext;
		internal static TestContext CurrentContext { get => _currentContext ?? (_currentContext = CreateContext()); }

		protected static TestContext CreateContext() {
			return new TestContext();
		}

		internal void ClearAssertResults() {
			AssertResults.Clear();
		}
	}
}
