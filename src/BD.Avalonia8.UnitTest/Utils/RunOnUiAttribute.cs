namespace BD.Avalonia8.UnitTest.Utils;

#pragma warning disable SA1600 // Elements should be documented

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class RunOnUiAttribute : Attribute, IWrapTestMethod
{
    public TestCommand Wrap(TestCommand command) => new RunOnUiCommand(command);

    sealed class RunOnUiCommand(TestCommand innerCommand) : DelegatingTestCommand(innerCommand)
    {
        public override TestResult Execute(TestExecutionContext context)
        {
            var resultTask = Dispatcher.UIThread.InvokeAsync(() => RunTest(context));

            resultTask.Wait();

            if (resultTask.Result is Exception ex)
                throw ex;

            return (TestResult)resultTask.Result;
        }

        object RunTest(TestExecutionContext context)
        {
            try
            {
                return innerCommand.Execute(context);
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}