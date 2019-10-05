namespace CUnit.Framework
{
	using System;

	public interface IActionСriterion
	{
		void Throw<TException>(Action action, string errorMessage = "") where TException : Exception;
		void NotThrow(Action action, string errorMessage = "");
	}
}
