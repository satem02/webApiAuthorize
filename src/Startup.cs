using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using webapi_authorize.Model;
using webapi_authorize.Services.Abstract;
using webapi_authorize.Services.Concrete;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.IdentityModel.Logging;

namespace webApiAuthorize
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.Configure<MyOptions>(Configuration);
            var myOptions = Configuration.Get<MyOptions>();

            services.AddTransient<IUserSessionService, DummyUserSessionService>();//UserSessionService
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthorization();

            services.AddMvc()
                        .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Authorization",
                    Description = "Authorization Web API V1",
                    TermsOfService = "None",
                    Contact = new Contact() { Name = "Şafak Temel", Email = "safak.temel@outlook.com", Url = "https://www.bethesna.com" }
                });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } } });
            });

            services.AddAuthentication(option =>
             {
                 option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                 option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
             }).AddJwtBearer(option =>
             {
                 option.SaveToken = true;
                 option.RequireHttpsMetadata = true;
                 option.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidAudience = myOptions.JwtOptions.Audience,
                     ValidIssuer = myOptions.JwtOptions.Issuer,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(myOptions.JwtOptions.Key))
                 };
             });

            IdentityModelEventSource.ShowPII = true; //To show detail of error and see the problem

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseCors(x => x
                 .AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader());


            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.DocumentTitle = "Title Documentation";
                c.DocExpansion(DocExpansion.None);
            });


        }
    }
}
