using ComponentSpace.Saml2.Configuration;
using ComponentSpace.Saml2.Configuration.Resolver;
using ComponentSpace.Saml2.Exceptions;
using ExampleServiceProvider.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleServiceProvider
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
                options.Cookie.Name = "ExampleServiceProvider.Identity";
            });

            // Add SAML SSO services.
            services.AddSaml(Configuration.GetSection("SAML"));
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
                endpoints.MapControllers();
            });
        }

        // Demonstrates loading SAML configuration programmatically 
        // rather than through appsettings.json or another JSON configuration file.
        // This is useful if configuration is stored in a custom database, for example.
        // The SAML configuration is registered in ConfigureServices by calling:
        // services.AddSaml(config => ConfigureSaml(config));
        private void ConfigureSaml(SamlConfigurations samlConfigurations)
        {
            samlConfigurations.Configurations = new List<SamlConfiguration>()
            {
                new SamlConfiguration()
                {
                    LocalServiceProviderConfiguration = new LocalServiceProviderConfiguration()
                    {
                        Name = "https://ExampleServiceProvider",
                        Description = "Example Service Provider",
                        AssertionConsumerServiceUrl = "https://localhost:44360/SAML/AssertionConsumerService",
                        SingleLogoutServiceUrl = "https://localhost:44360/SAML/SingleLogoutService",
                        ArtifactResolutionServiceUrl = "https://localhost:44360/SAML/ArtifactResolutionService",
                        LocalCertificates = new List<Certificate>()
                        {
                            new Certificate()
                            {
                                FileName = "certificates/sp.pfx",
                                Password = "password"
                            }
                        }
                    },
                    PartnerIdentityProviderConfigurations = new List<PartnerIdentityProviderConfiguration>()
                    {
                        new PartnerIdentityProviderConfiguration()
                        {
                            Name = "https://ExampleIdentityProvider",
                            Description = "Example Identity Provider",
                            SignAuthnRequest = true,
                            SignLogoutRequest = true,
                            SignLogoutResponse = true,
                            WantLogoutRequestSigned = true,
                            WantLogoutResponseSigned = true,
                            SingleSignOnServiceUrl = "https://localhost:44313/SAML/SingleSignOnService",
                            SingleLogoutServiceUrl = "https://localhost:44313/SAML/SingleLogoutService",
                            ArtifactResolutionServiceUrl = "https://localhost:44313/SAML/ArtifactResolutionService",
                            PartnerCertificates = new List<Certificate>()
                            {
                                new Certificate()
                                {
                                    FileName = "certificates/idp.cer"
                                }
                            }
                        }
                    }
                }
            };
        }
    }

    // Demonstrates loading SAML configuration dynamically using a custom configuration resolver.
    // Hard-coded configuration is returned in this example but more typically configuration would be read from a custom database.
    // The configurationID parameter specifies the SAML configuration in a multi-tenancy application but is not used in this example.
    // The custom configuration resolver is registered in ConfigureServices by calling:
    // services.AddSaml();
    // services.AddTransient<ISamlConfigurationResolver, CustomConfigurationResolver>();
    public class CustomConfigurationResolver : AbstractSamlConfigurationResolver
    {
        public override Task<bool> IsLocalServiceProviderAsync(string configurationID)
        {
            return Task.FromResult(true);
        }

        public override Task<LocalServiceProviderConfiguration> GetLocalServiceProviderConfigurationAsync(string configurationID)
        {
            var localServiceProviderConfiguration = new LocalServiceProviderConfiguration()
            {
                Name = "https://ExampleServiceProvider",
                Description = "Example Service Provider",
                AssertionConsumerServiceUrl = "https://localhost:44360/SAML/AssertionConsumerService",
                SingleLogoutServiceUrl = "https://localhost:44360/SAML/SingleLogoutService",
                ArtifactResolutionServiceUrl = "https://localhost:44360/SAML/ArtifactResolutionService",
                LocalCertificates = new List<Certificate>()
                {
                    new Certificate()
                    {
                        FileName = "certificates/sp.pfx",
                        Password = "password"
                    }
                }
            };

            return Task.FromResult(localServiceProviderConfiguration);
        }

        public override Task<PartnerIdentityProviderConfiguration> GetPartnerIdentityProviderConfigurationAsync(string configurationID, string partnerName)
        {
            if (partnerName != "https://ExampleIdentityProvider")
            {
                throw new SamlConfigurationException($"The partner identity provider {partnerName} is not configured."); 
            }

            var partnerIdentityProviderConfiguration = new PartnerIdentityProviderConfiguration()
            {
                Name = "https://ExampleIdentityProvider",
                Description = "Example Identity Provider",
                SignAuthnRequest = true,
                SignLogoutRequest = true,
                SignLogoutResponse = true,
                WantLogoutRequestSigned = true,
                WantLogoutResponseSigned = true,
                SingleSignOnServiceUrl = "https://localhost:44313/SAML/SingleSignOnService",
                SingleLogoutServiceUrl = "https://localhost:44313/SAML/SingleLogoutService",
                ArtifactResolutionServiceUrl = "https://localhost:44313/SAML/ArtifactResolutionService",
                PartnerCertificates = new List<Certificate>()
                {
                    new Certificate()
                    {
                        FileName = "certificates/idp.cer"
                    }
                }
            };

            return Task.FromResult(partnerIdentityProviderConfiguration);
        }

        public override Task<IList<string>> GetPartnerIdentityProviderNamesAsync(string configurationID)
        {
            IList<string> partnerIdentityProviderNames = new List<string> { "https://ExampleIdentityProvider" };

            return Task.FromResult(partnerIdentityProviderNames);
        }
    }
}
