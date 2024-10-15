using AutoMapper;
using ConstruaApp.Api.Jobs;
using ConstruaApp.Api.Jobs.Config;
using ConstruaApp.Api.Jobs.Interfaces;
using DinkToPdf;
using DinkToPdf.Contracts;
using Hangfire;
using Hangfire.MySql;
using Infra.CrossCutting.Auth;
using Infra.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConstruaApp.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AddHangFire(services);
            AddCors(services);
            AddAutoMapper(services);
            AddSwagger(services);
            AddMVC(services);
            AddOptions(services);
            AddAuthentication(services);
            AddPDF(services);
            RegisterServices(services, Configuration);
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // [2020.10.27] Adicionado suporte ao ambiente de desenvolvimento do Ricardo criando um "appsettings.DevelopmentRicardo.json"
            //      por causa das connection strings ao MySQL
            //          Exclui minha alteração de environment (localmente na minha maquina apenas) no launchSettings.json
            //          usando "git update-index --assume-unchanged Modules\ConstruaApp.Api\Properties\launchSettings.json"

            if (!env.IsEnvironment("AzurePrd"))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Construa App - API v2");
            });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseHangfireServer();
            var username = Configuration.GetSection("HangFire:Username").Value;
            var password = Configuration.GetSection("HangFire:Password").Value;
            var options = new DashboardOptions() { Authorization = new[] { new BasicAuthenticationFilter(username, password) } };
            app.UseHangfireDashboard("/hangfire", options);
            ConfigureJobsHangFire(env);
        }


        private void AddCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        private void AddPDF(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        }


        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Construa App - API", Version = "v2" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        private void AddOptions(IServiceCollection services)
        {
            services.AddOptions();
        }


        // [2020.10.27 ricardag] Como o MySQL não guarda o Timezone dos TIMESTAMPS, vamos assumir que todos os horarios no banco 
        //      estão em hora local no fuso do servidor. Isso vai ajudar o App nos calculos de horario usando moment.js
        public class DateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Debug.Assert(typeToConvert == typeof(DateTime));
                return DateTime.Parse(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            }
        }

        private void AddMVC(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new DateTimeConverter()); });
        }

        private void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            new RootBootstrapper().RootRegisterServices(services, configuration);
        }


        private void AddAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.Load("Application"));
        }

        private void AddAuthentication(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(AuthSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        private void AddHangFire(IServiceCollection services)
        {
            services.AddScoped<IExecuteJobCreateDBMobile, ExecuteJobCreateDBMobile>();
            services.AddScoped<IExecuteJobDeleteDBMobile, ExecuteJobDeleteDBMobile>();
            services.AddScoped<IExecuteJobSendRDConversions, ExecuteJobSendRDConversions>();
            services.AddHangfire(config =>
            {
                config.UseStorage(new MySqlStorage(Configuration.GetConnectionString("MySqlJobs"), new MySqlStorageOptions
                {
                    TransactionTimeout = TimeSpan.FromMinutes(1),
                    PrepareSchemaIfNecessary = false,
                    TablesPrefix = "Hangfire_"
                }));
            });
        }

        private void ConfigureJobsHangFire(IWebHostEnvironment env)
        {
            RecurringJob.AddOrUpdate<IExecuteJobCreateDBMobile>(e => e.Execute(), Hangfire.Cron.Daily(), TimeZoneInfo.Utc);
            RecurringJob.AddOrUpdate<IExecuteJobDeleteDBMobile>(e => e.Execute(), Hangfire.Cron.Daily(), TimeZoneInfo.Utc);
            if (env.IsEnvironment("AzurePrd"))
            {
                RecurringJob.AddOrUpdate<IExecuteJobSendRDConversions>(e => e.Execute(), Hangfire.Cron.MinuteInterval(30));
            }
        }
    }
}
