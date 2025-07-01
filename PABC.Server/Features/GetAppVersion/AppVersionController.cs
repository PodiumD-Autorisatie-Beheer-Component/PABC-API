using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Reflection;

namespace PABC.Server.Features.GetAppVersion
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AppVersionController
    {
        private static readonly AppVersion s_appVersion = GetAppVersionInternal();

        [HttpGet("app-version", Name = "GetAppVersion")]
        [ProducesResponseType<AppVersion>(200, MediaTypeNames.Application.Json)]
        public ActionResult<AppVersion> Get() => new OkObjectResult(s_appVersion);

        private static AppVersion GetAppVersionInternal()
        {
            var parts = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                ?.Split('+') ?? [];
            return new AppVersion(parts.ElementAtOrDefault(0), parts.ElementAtOrDefault(1));
        }

    }
    public readonly record struct AppVersion(string? Version, string? Revision);
}
