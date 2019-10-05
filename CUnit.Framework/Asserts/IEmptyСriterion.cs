namespace CUnit.Framework
{
	public interface IEmptyСriterion
	{
		void BeEmpty(string errorMessage = "");
		void NotBeEmpty(string errorMessage = "");
	}
}
