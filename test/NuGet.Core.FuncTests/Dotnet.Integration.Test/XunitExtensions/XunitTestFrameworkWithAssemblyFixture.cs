using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Dotnet.Integration.Test.XunitExtensions
{
    public class XunitTestFrameworkWithAssemblyFixture : XunitTestFramework
    {
        public XunitTestFrameworkWithAssemblyFixture(IMessageSink messageSink)
            : base(messageSink)
        { }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new XunitTestFrameworkExecutorWithAssemblyFixture(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}
