using FSH.Playground.Blazor.Services;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace FSH.Playground.Blazor.Services.Api;

/// <summary>
/// Delegating handler that adds the JWT token to API requests and handles 401 responses
/// by attempting to refresh the access token. If refresh fails, signs out the user and
/// notifies Blazor components via IAuthStateNotifier.
///
/// Proactively refreshes tokens that are near expiry to avoid 401-driven refresh loops.
/// </summary>
internal sealed class AuthorizationHeaderHandler : DelegatingHandler
{
    /// <summary>
    /// Refresh the token if it expires within this window.
    /// Prevents 401 → refresh → retry cycles by refreshing before expiry.
    /// </summary>
    private static readonly TimeSpan ExpiryBuffer = TimeSpan.FromSeconds(30);

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICircuitTokenCache _circuitTokenCache;
    private readonly ILogger<AuthorizationHeaderHandler> _logger;
    private bool _signOutInitiated;

    public AuthorizationHeaderHandler(
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider,
        ICircuitTokenCache circuitTokenCache,
        ILogger<AuthorizationHeaderHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _circuitTokenCache = circuitTokenCache;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessTokenAsync();

        // Proactive refresh: if the token is near expiry, refresh before sending
        if (!string.IsNullOrEmpty(accessToken) && IsTokenNearExpiry(accessToken))
        {
            _logger.LogDebug("Access token near expiry, proactively refreshing");
            var refreshed = await TryRefreshTokenAsync(cancellationToken);
            if (!string.IsNullOrEmpty(refreshed))
            {
                accessToken = refreshed;
            }
        }

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Reactive refresh: handle unexpected 401 (e.g., token revoked server-side)
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (_signOutInitiated || string.IsNullOrEmpty(accessToken))
            {
                return response;
            }

            _logger.LogInformation("Received 401, attempting token refresh");
            var newAccessToken = await TryRefreshTokenAsync(cancellationToken);

            if (!string.IsNullOrEmpty(newAccessToken))
            {
                _logger.LogInformation("Token refresh successful, retrying request");
                using var retryRequest = await CloneHttpRequestMessageAsync(request);
                retryRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newAccessToken);
                response.Dispose();
                response = await base.SendAsync(retryRequest, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Token refresh failed, signing out user");
                _signOutInitiated = true;
                await SignOutUserAsync();
            }
        }

        return response;
    }

    private static bool IsTokenNearExpiry(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(accessToken))
            {
                return false;
            }

            var jwt = handler.ReadJwtToken(accessToken);
            return jwt.ValidTo <= DateTime.UtcNow.Add(ExpiryBuffer);
        }
        catch
        {
            return false;
        }
    }

    private async Task SignOutUserAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null)
            {
                try
                {
                    if (!httpContext.Response.HasStarted)
                    {
                        await httpContext.SignOutAsync("Cookies");
                        _logger.LogInformation("User signed out due to expired refresh token");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogDebug(ex, "Could not sign out via cookies (response started)");
                }

                var authStateNotifier = _serviceProvider.GetService<IAuthStateNotifier>();
                authStateNotifier?.NotifySessionExpired();
            }
        }
        catch (Microsoft.AspNetCore.Components.NavigationException)
        {
            // Expected — NavigateTo with forceLoad throws this to interrupt execution
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle session expiration");
        }
    }

    private Task<string?> GetAccessTokenAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(_circuitTokenCache.AccessToken))
            {
                return Task.FromResult<string?>(_circuitTokenCache.AccessToken);
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return Task.FromResult(httpContext.User.FindFirst("access_token")?.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get access token");
        }

        return Task.FromResult<string?>(null);
    }

    private async Task<string?> TryRefreshTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            var tokenRefreshService = _serviceProvider.GetService<ITokenRefreshService>();
            if (tokenRefreshService is null)
            {
                _logger.LogWarning("TokenRefreshService is not registered");
                return null;
            }

            return await tokenRefreshService.TryRefreshTokenAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return null;
        }
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        foreach (var header in request.Headers.Where(h => !string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase)))
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);
            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        foreach (var option in request.Options)
        {
            clone.Options.TryAdd(option.Key, option.Value);
        }

        return clone;
    }
}
