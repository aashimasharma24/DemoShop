using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, (int Count, DateTime ResetTime)> _requestCounters = new();
        private const int Limit = 5;
        private const int ResetSeconds = 60;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var key = GenerateClientKey(context);
            if (_requestCounters.ContainsKey(key))
            {
                var (count, resetTime) = _requestCounters[key];

                if (DateTime.UtcNow < resetTime)
                {
                    if (count >= Limit)
                    {
                        context.Response.StatusCode = 429;
                        await context.Response.WriteAsync("Too many requests. Try again later.");
                        return;
                    }
                    _requestCounters[key] = (count + 1, resetTime);
                }
                else
                {
                    _requestCounters[key] = (1, DateTime.UtcNow.AddSeconds(ResetSeconds));
                }
            }
            else
            {
                _requestCounters[key] = (1, DateTime.UtcNow.AddSeconds(ResetSeconds));
            }

            await _next(context);
        }

        private static string GenerateClientKey(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
