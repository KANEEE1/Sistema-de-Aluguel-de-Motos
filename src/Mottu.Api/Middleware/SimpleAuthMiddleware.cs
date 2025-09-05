using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Mottu.Api.Services;
using System.Threading.Tasks;

namespace Mottu.Api.Middleware
{
    public class SimpleAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SimpleAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/auth"))
            {
                await _next(context);
                return;
            }

            var username = context.Request.Headers["X-Username"].FirstOrDefault();

            if (string.IsNullOrEmpty(username))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Header X-Username é obrigatório");
                return;
            }

            var authService = context.RequestServices.GetRequiredService<IAuthService>();
            var isAdmin = await authService.IsAdminAsync(username);
            var isDeliveryPerson = await authService.IsDeliveryPersonAsync(username);

            if (!isAdmin && !isDeliveryPerson)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Usuário inválido");
                return;
            }

            if (context.Request.Path.StartsWithSegments("/motorcycles") &&
                context.Request.Method != "GET" && !isAdmin)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Acesso de administrador necessário");
                return;
            }

            if (isDeliveryPerson && !isAdmin)
            {
                if (context.Request.Path.StartsWithSegments("/delivery-people"))
                {
                    if (context.Request.Method == "GET" && context.Request.Path.Value != "/delivery-people")
                    {
                        var pathSegments = context.Request.Path.Value?.Split('/') ?? Array.Empty<string>();
                        if (pathSegments.Length >= 3)
                        {
                            var requestedId = pathSegments[2];
                            if (requestedId != username)
                            {
                                context.Response.StatusCode = 403;
                                await context.Response.WriteAsync("Acesso negado: Só é possível visualizar o próprio perfil");
                                return;
                            }
                        }
                    }
                    else if (context.Request.Method == "PUT")
                    {
                        var pathSegments = context.Request.Path.Value?.Split('/') ?? Array.Empty<string>();
                        if (pathSegments.Length >= 3)
                        {
                            var requestedId = pathSegments[2];
                            if (requestedId != username)
                            {
                                context.Response.StatusCode = 403;
                                await context.Response.WriteAsync("Acesso negado: Só é possível atualizar o próprio perfil");
                                return;
                            }
                        }
                    }
                }

                if (context.Request.Path.StartsWithSegments("/rentals") && context.Request.Method == "PUT")
                {
                    var pathSegments = context.Request.Path.Value?.Split('/') ?? Array.Empty<string>();
                    if (pathSegments.Length >= 3 && pathSegments[3] == "return")
                    {
                    }
                }
            }

            context.Items["Username"] = username;
            context.Items["IsAdmin"] = isAdmin;
            context.Items["IsDeliveryPerson"] = isDeliveryPerson;

            await _next(context);
        }
    }
}
