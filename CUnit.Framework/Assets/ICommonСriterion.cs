namespace CUnit.Framework
{
	public interface ICommonСriterion<TValue>
	{
		void BeNull(string errorMessage = "");
		void NotBeNull(string errorMessage = "");
		void BeEqual(TValue expectedValue, string errorMessage = "");
		void NotBeEqual(TValue expectedValue, string errorMessage = "");
	}
}
