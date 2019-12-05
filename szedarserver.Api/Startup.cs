using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using szedarserver.Core.Domain.Context;
using Microsoft.EntityFrameworkCore;
using szedarserver.Infrastructure.IServices;
using szedarserver.Infrastructure.Services;
using szedarserver.Core.IRepositories;
using szedarserver.Core.Repositories;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using szedarserver.Infrastructure.Extensions;
using szedarserver.Infrastructure.MappingProfile;
using szedarserver.Infrastructure.Setting;

namespace szedarserver.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IJwtExtension, JwtExtension>();
            services.AddTransient<ITournamentService, TournamentService>();
            services.AddTransient<ISwissService, SwissService>();
            services.AddTransient<ISwissRepository, SwissRepository>();
            services.AddTransient<ITournamentRepository, TournamentRepository>();
            services.AddControllers();
            services.AddDbContextPool<DataBaseContext>(
                optionsBuilder => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SzedarDBConnection"), 
                b => b.MigrationsAssembly("szedarserver.Api")));
            //services.AddAutoMapper(typeof(Startup));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000");
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });

            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
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
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("0f33ac630f544bc4a21aca21a472fb02")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddMvc();
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

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
