using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.ComponentModel;

/// <summary>
/// Implements the IAuthorizationHandler interface to handle authorization based on white-listed hosts.
/// </summary>
public class PermissionAuthorizedHandler : IAuthorizationHandler {

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAuthorizedHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">An instance of IHttpContextAccessor for accessing the HttpContext.</param>
    /// <param name="configuration">An instance of IConfiguration for accessing configuration settings.</param>
    public PermissionAuthorizedHandler(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration
    ) {
        HttpContextAccessor = httpContextAccessor;
        Configuration = configuration;
    }

    /// <summary>
    /// Gets the IConfiguration instance for accessing configuration settings.
    /// </summary>
    private IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the IHttpContextAccessor instance for accessing the HttpContext.
    /// </summary>
    private IHttpContextAccessor HttpContextAccessor { get; }

    /// <summary>
    /// Gets the current HttpContext or throws an exception if HttpContextAccessor is null.
    /// </summary>
    private HttpContext HttpContext => HttpContextAccessor.HttpContext
        ?? throw new InvalidEnumArgumentException();

    /// <summary>
    /// Handles authorization based on the white-listed hosts.
    /// </summary>
    /// <param name="context">The AuthorizationHandlerContext containing the authorization context.</param>
    public async Task HandleAsync(AuthorizationHandlerContext context) {

        // Retrieve the array of white-listed hosts from the configuration.
        var whiteList = Configuration.GetSection("Jwt:WhiteListedHosts").Get<string[]>() ?? new string[0];

        // Get the IP address of the host making the request.
        var hostIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        // Check if the "x-forward-for" header is present in the request.
        if (HttpContext.Request.Headers.ContainsKey("x-forwarded-for"))
            // If present, update hostIp with the value from the header.
            hostIp = HttpContext.Request.Headers["x-forwarded-for"].ToString();

        // If hostIp is not null, check if it is in the white-listed hosts.
        if (hostIp != null) {
            if (whiteList.Contains(hostIp)) {
                // If the host is white-listed, succeed the pending requirements in the authorization context.
                Log.Logger.Information($"Found in whitelist : {hostIp}");
                foreach (var item in context.PendingRequirements) context.Succeed(item);
            } else {

                Log.Logger.Information($"Not found in whitelist : {hostIp}");
            }

        }
    }
}
