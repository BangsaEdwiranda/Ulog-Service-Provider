using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.Text;

namespace ExampleWebApi
{
    public class Startup
    {
        private const string DefaultCorsPolicyName = "Default CORS Policy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // SameSiteMode.None is required to support SAML SSO.
                options.MinimumSameSitePolicy = SameSiteMode.None;

                // Some older browsers don't support SameSiteMode.None.
                options.OnAppendCookie = cookieContext => SameSite.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext => SameSite.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            // Optionally add support for JWT bearer tokens.
            // This is required only if JWT bearer tokens are used to authorize access to a web API.
            // It's not required for SAML SSO.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                    };
                });

            // Optionally add cross-origin request sharing services.
            // This is only required for the web API.
            // It's not required for SAML SSO.
            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName,
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200");
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                });
            });

            // Optionally add support for sessions.
            // This is only required to support issuing JWT bearer tokens.
            services.AddSession();

            // Add SAML SSO services.
            services.AddSaml(Configuration.GetSection("SAML"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(DefaultCorsPolicyName);
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
