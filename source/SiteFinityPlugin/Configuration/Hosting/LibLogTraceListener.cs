using System.Diagnostics;
using Thinktecture.IdentityServer.Core.Logging;

namespace IdentityServer.SiteFinity.Configuration.Hosting
{
    internal class LibLogTraceListener : TraceListener
    {
        private static readonly ILog Logger = LogProvider.GetLogger("WebApi Diagnostics");

        public override void WriteLine(string message)
        {
            Logger.Debug(message);
        }

        public override void Write(string message)
        { }
    }
}