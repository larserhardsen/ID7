using Sitecore.Framework.Runtime.Commands;

namespace Sitecore.IdentityServer.Host
{
    public static class Program
    {
        /// <summary>Entrypoint for the application.</summary>
        /// <param name="args">The arguments to be processed.</param>
        /// <returns>The return result of the invoked command.</returns>
        public static Task<int> Main(string[] args) => SitecoreHostCommand.InvokeAsync(args);
    }
}