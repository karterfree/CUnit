namespace CUnit.Framework
{	
	using System;

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class IgnoreAttribute : Attribute
	{
		public string Reason { get; private set; }
		public IgnoreAttribute(string reason = "") {
			Reason = reason;
		}
	}
}
