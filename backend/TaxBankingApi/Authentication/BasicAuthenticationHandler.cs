using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace TaxBankingApi.Authentication;

public class BasicAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check if Authorization header exists
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(
                AuthenticateResult.Fail("Authorization header missing.")
            );
        }

        try
        {
            // Read Authorization header
            var authenticationHeader =
                AuthenticationHeaderValue.Parse(
                    Request.Headers["Authorization"].ToString()
                );

            // Only Basic Authentication is allowed
            if (authenticationHeader.Scheme != "Basic")
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Invalid authentication scheme.")
                );
            }

            // Decode Base64 username:password
            var credentialBytes =
                Convert.FromBase64String(
                    authenticationHeader.Parameter!
                );

            var credentials =
                Encoding.UTF8
                    .GetString(credentialBytes)
                    .Split(':', 2);

            // Check that username and password exist
            if (credentials.Length != 2)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Invalid credentials.")
                );
            }

            var username = credentials[0];
            var password = credentials[1];

            // Hardcoded login for TaxOra POC
            if (username != "admin" || password != "admin")
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        "Invalid username or password."
                    )
                );
            }

            // Create authenticated user
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(
                claims,
                Scheme.Name
            );

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(
                principal,
                Scheme.Name
            );

            return Task.FromResult(
                AuthenticateResult.Success(ticket)
            );
        }
        catch
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Invalid Authorization header."
                )
            );
        }
    }
}