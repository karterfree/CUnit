namespace CUnit.Framework.Driver.Report
{
	internal interface ITestReportBuilder
	{
		ITestRunningTracer SourceTracer { set; }

		string BuildReport();
	}
}
