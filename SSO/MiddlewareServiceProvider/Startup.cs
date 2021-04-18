using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiddlewareServiceProvider.Data;
using Shared;

namespace MiddlewareServiceProvider
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddRazorPages();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // SameSiteMode.None is required to support SAML SSO.
                options.MinimumSameSitePolicy = SameSiteMode.None;

                // Some older browsers don't support SameSiteMode.None.
                options.OnAppendCookie = cookieContext => SameSite.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext => SameSite.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            // Use a unique identity cookie name rather than sharing the cookie across applications in the domain.
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "MiddlewareServiceProvider.Identity";
            });

            // Add SAML SSO services.
            services.AddSaml(Configuration.GetSection("SAML"));

            // Add SAML authentication services.
            services.AddAuthentication().AddSaml(options =>
            {
                options.PartnerName = (httpContext) => Configuration["PartnerName"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
