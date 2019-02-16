using ZPP.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace ZPP.Server.Authentication
{
    public static class Extensions
    {
        private static readonly string jwtConfiguration = "jwt";
        public static void AddJwt(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            var section = configuration.GetSection(jwtConfiguration);
            var jwtOptions = section.Get<JwtOptions>();

            services.Configure<JwtOptions>(section);
            services.AddSingleton(jwtOptions);
            services.AddSingleton<IJwtHandler, JwtHandler>();
            //services.AddTransient<IAccessTokenService, AccessTokenService>();
            services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            services.AddAuthentication(o =>
            {
                o.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
            }
            )

                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.ValidAudience,
                        ValidateAudience = jwtOptions.ValidateAudience,
                        ValidateLifetime = jwtOptions.ValidateLifetime
                    };
                })
                .AddGoogle("Google", googleOptions =>
                {
                    googleOptions.CallbackPath = new PathString("/external-handler");
                    googleOptions.ClientId = configuration["Authentication:Google:client_id"];
                    googleOptions.ClientSecret = configuration["Authentication:Google:client_secret"];
                    googleOptions.Events = new OAuthEvents
                    {
                        OnRemoteFailure = (RemoteFailureContext context) =>
                        {
                            context.Response.Redirect("/");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        },
                    };
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
                    facebookOptions.Events = new OAuthEvents
                    {
                        OnRemoteFailure = (RemoteFailureContext context) =>
                        {
                            context.Response.Redirect("/");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        },
                    };
                });
        }
    }
}
