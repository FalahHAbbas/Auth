using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auth.Configs;
using Auth.DB;
using Auth.Repositories;
using Auth.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Auth {
    public class Startup {
        public Startup(IConfiguration configuration) { Configuration = configuration; }

        public IConfiguration Configuration { get; }

        public const int PageSize = 10;

        public  void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<AuthContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("local")),
                ServiceLifetime.Transient);

            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {Title = "My API", Version = "v1"});
                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme {
                        In = "header",
                        Description =
                            "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {"Bearer", Enumerable.Empty<string>()}
                });
            });
            AuthHelper.InitRoles();

            services.AddAutoMapper(typeof(Startup));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                jwtBearerOptions => {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "Issuer",
                        ValidAudience = "Audience",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("Hlkjds0-324mf34pojf-34r34fwlknef0943"))
                    };
                });
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddAuthorization(options =>
                options.AddPolicy("Main", HasPrivilegeRequirement.Of));
            services.AddScoped<IAuthorizationHandler, HasPrivilegeHandler>();

            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.NullValueHandling =
                    Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}