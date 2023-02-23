// Decompiled with JetBrains decompiler
// Type: Sitecore.IdentityServer.Host.Startup
// Assembly: Sitecore.IdentityServer.Host, Version=7.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DBD4228D-91D1-4BD6-ACE9-55C493882BEA
// Assembly location: C:\Stash\IdentityServer 7\Sitecore.IdentityServer.Host.dll
// XML documentation location: C:\Stash\IdentityServer 7\Sitecore.IdentityServer.Host.xml

using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Sitecore.Framework.Runtime.Configuration;
using Sitecore.Framework.Runtime.Plugins;
using Sitecore.Identity.Localization;
using Sitecore.Plugin.IdentityServer.Configuration;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

#nullable enable
namespace Sitecore.IdentityServer.Host
{
    public sealed class Startup
    {
        private readonly
#nullable disable
        ISitecorePluginManager pluginManager;
        private readonly IWebHostEnvironment env;
        private readonly ISitecoreConfiguration configuration;

        public Startup(ISitecorePluginManager pluginManager, IWebHostEnvironment env, ISitecoreConfiguration configuration)
        {
            this.pluginManager = pluginManager;
            this.env = env;
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAntiforgery(opt =>
            {
                opt.Cookie.SameSite = SameSiteMode.None;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
            services.AddControllersWithViews().AddRazorRuntimeCompilation(options =>
            {
                string root = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty, "sitecore");
                options.FileProviders.Add(new PhysicalFileProvider(root));
            }).AddSitecoreLocalization();


            if (this.configuration.GetSection("Sitecore:IdentityServerHost:KeysConnectionString").Value != null)
            {
                // LarsE (start)
                services.AddDbContext<KeysContext>(options =>
                    options.UseSqlServer(this.configuration.GetSection("Sitecore:IdentityServerHost:KeysConnectionString").Value));

                services.AddDataProtection().SetApplicationName("ID7").PersistKeysToDbContext<KeysContext>();
                // LarsE (end)
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISitecoreConfiguration scConfig, IEnumerable<ClientConfig> clientConfigs)
        {
            SitecoreIdentityServerOptions hostOptions = scConfig.GetSection("Sitecore:IdentityServerHost").Get<SitecoreIdentityServerOptions>() ?? new SitecoreIdentityServerOptions();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/home/error");
            }

            app.UseRouting();
            app.UseCors(policy =>
            {
                policy.WithOrigins(Startup.GetAllowedOrigins(clientConfigs));
                policy.AllowAnyHeader();
                policy.WithMethods("GET", "POST");
            });
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseSitecoreRequestLocalization(options => options.DefaultRequestCulture = new RequestCulture(hostOptions.DefaultCulture));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Account}/{action=Login}/{id?}");
                endpoints.MapControllerRoute("device-flow", "{controller=Device}/{action=Index}");
            });
        }

        private static string[] GetAllowedOrigins(IEnumerable<ClientConfig> clientConfigs)
        {
            List<string> source = new();
            foreach (ClientConfig clientConfig in clientConfigs.Where(c => c.AllowedCorsOrigins != null))
            {
                source.AddRange(clientConfig.AllowedCorsOrigins.Where(o => !string.IsNullOrEmpty(o)));
            }

            return source.Distinct<string>().ToArray<string>();
        }
    }
}
