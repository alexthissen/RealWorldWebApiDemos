using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using NSwag.AspNetCore;
using Polly;
using Refit;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace GameServerWebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null)
                {
                    builder.AddUserSecrets(appAssembly, optional: true);
                }
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            if (env.IsProduction())
            {
                builder.AddAzureKeyVault(Configuration["KeyVaultName"]);
                Configuration = builder.Build();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var registry = services.AddPolicyRegistry();
            var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));

            // Options for particular external services
            services.Configure<SteamApiOptions>(Configuration.GetSection("SteamApiOptions"));

            ConfigureApiOptions(services);
            ConfigureOpenApi(services);
            ConfigureHealth(services);
            ConfigureVersioning(services);

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHttpClient("Steam", options =>
            {
                options.BaseAddress = new Uri(Configuration["SteamApiOptions:BaseUrl"]);
                options.Timeout = TimeSpan.FromMilliseconds(15000);
                options.DefaultRequestHeaders.Add("ClientFactory", "Check");
            })
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(1500)))
            .AddServerErrorPolicyHandler(p => p.RetryAsync(3))
            .AddTypedClient(client => RestService.For<ISteamClient>(client));
        }

        private void ConfigureVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(options => {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                // Includes headers "api-supported-versions" and "api-deprecated-versions"
                options.ReportApiVersions = true;

                // Alternative to attribute based versioning
                //options.Conventions.Controller<GameServerController>()
                //    .HasDeprecatedApiVersion(new ApiVersion(0, 9))
                //    .HasApiVersion(1)
                //    .AdvertisesApiVersion(2)
                //    .Action(a => a.Get(default(int))).MapToApiVersion(1);
            });
        }

        private void ConfigureApiOptions(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://asp.net/core",
                        Detail = "Please refer to the errors property for additional details."
                    };
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });
        }

        private void ConfigureOpenApi(IServiceCollection services)
        {
            services.AddSwagger();
        }

        private void ConfigureHealth(IServiceCollection services)
        {
            services.AddHealthChecks(checks =>
            {
                checks
                    .AddUrlCheck(Configuration["SteamApiOptions:BaseUrl"],
                        response =>
                        {
                            var status = response.StatusCode == System.Net.HttpStatusCode.NotFound ? CheckStatus.Healthy : CheckStatus.Unhealthy;
                            return new ValueTask<IHealthCheckResult>(HealthCheckResult.FromStatus(status, "Steam API base URL reachable."));
                        }
                    );
                // TODO: Use feature toggle to add this functionality
                    //.AddHealthCheckGroup(
                    //    "memory",
                    //    group => group
                    //        .AddPrivateMemorySizeCheck(1000) // Maximum private memory
                    //        .AddVirtualMemorySizeCheck(2000)
                    //        .AddWorkingSetCheck(1000),
                    //    CheckStatus.Unhealthy
                    //);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Information);
            
            // Next call not required for .NET Core and Azure App Services
            //loggerFactory.AddAzureWebAppDiagnostics();

            loggerFactory.AddEventSourceLogger(); // ETW on Windows, dev/null on other platforms
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Do not expose Swagger interface in production
                app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiSettings()
                {
                    ShowRequestHeaders = true,
                    Description = "DotNext SPb 2018 Real-world Web API",
                    DocExpansion = "list",
                    Title = "DotNext API",
                    Version = "1.0",
                    UseJsonEditor = true
                });
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }
            
            app.UseMvcWithDefaultRoute();
        }
    }
}