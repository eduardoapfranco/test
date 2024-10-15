﻿using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ConstruaApp.Api.Jobs.Config
{
    public class BasicAuthenticationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _username;
        private readonly string _password;
        public BasicAuthenticationFilter(string username, string password)
        {
            _username = username;
            _password = password;
        }
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var header = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(header))
            {
                SetChallengeResponse(httpContext);
                return false;
            }
            var authValues = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(header);
            if (!"Basic".Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                SetChallengeResponse(httpContext);
                return false;
            }
            var parameter = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
            var parts = parameter.Split(':');
            if (parts.Length < 2)
            {
                SetChallengeResponse(httpContext);
                return false;
            }
            var username = parts[0];
            var password = parts[1];
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetChallengeResponse(httpContext);
                return false;
            }
            if (username == _username && password == _password)
            {
                return true;
            }
            SetChallengeResponse(httpContext);
            return false;
        }
        private void SetChallengeResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
            httpContext.Response.WriteAsync("Authentication is required.");
        }
    }
}
