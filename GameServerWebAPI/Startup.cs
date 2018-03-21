using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using GameServerWebAPI.Controllers;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using Polly;
using Refit;

namespace GameServerWebAPI
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
            var registry = services.AddPolicyRegistry();
            var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));

            services.Configure<SteamApiOptions>(Configuration.GetSection("SteamApiOptions"));

            ConfigureApiOptions(services);
            ConfigureOpenApi(services);
            ConfigureHealth(services);
            ConfigureVersioning(services);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHttpClient("Refit", options =>
            {
                options.BaseAddress = new Uri("https://api.steampowered.com/IGameServersService/");
                //options.BaseAddress = new Uri("http://localhost:56338/");
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
                    .AddUrlCheck("https://api.steampowered.com")
                    .AddHealthCheckGroup(
                        "memory",
                        group => group
                            .AddPrivateMemorySizeCheck(1000) // Maximum private memory
                            .AddVirtualMemorySizeCheck(2000)
                            .AddWorkingSetCheck(1000),
                        CheckStatus.Unhealthy
                    );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiSettings()
                {
                    Description = "DotNext SPb 2018 Real-world Web API",
                    DocExpansion = "list",
                    Title = "DotNext API",
                    Version = "1.0",
                    UseJsonEditor = true
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvcWithDefaultRoute();
        }
    }
}
