using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _RestaurantAPI_;
using _RestaurantAPI_.Controllers;
using _RestaurantAPI_.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using _RestaurantAPI_.Middleware;
using _RestaurantAPI_.Services;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using _RestaurantAPI_.Models;
using _RestaurantAPI_.Models.Validators;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using _RestaurantAPI_.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI
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
            //    services.AddTransient<IWeatherForecastService, WeatherForecastService>();       //dependency injection
            var authenticationSettings = new AuthenticationSettings();

            Configuration.GetSection("Authentication").Bind(authenticationSettings);
            services.AddSingleton(authenticationSettings);

            services.AddAuthentication(option =>                            //konfiguracja serwisów do autentykacji
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality","German","Polish"));
                options.AddPolicy("AtLeast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
                options.AddPolicy("MinimumNumberOfRestaurants", builder =>
                        builder.AddRequirements(new MinimumNumberOfRestaurantsRequirement(2)));
            });

            services.AddControllers().AddFluentValidation();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, MinimumNumberOfRestaurantsRequirementHandler>();
            services.AddDbContext<RestaurantDbContext>();
            services.AddScoped<RestaurantSeeder>();
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IRestaurantService, RestaurantService>();       //dependency injection
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<RequestTimedOut>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor(); 
            services.AddSwaggerGen();
            services.AddCors(options =>                                        //obs³uga cors
                options.AddPolicy("FrontEndClient", builder =>
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(Configuration["AllowedOrigins"])   //Adres frontendowej apki.
                )
            ); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        // request pipeline - programy, w którym wyjœcie z jendego jest wejœciem do kolejnego. (middleware)
        // Tutaj dopisuje siê do programu odpowiednie middleware.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
        {
            app.UseResponseCaching();   //Umo¿liwia na³o¿enie cachowanie na metodê.
            app.UseStaticFiles();       //pliki statyczne - pod inn¹ œcie¿k¹ ni¿ api
            app.UseCors("FrontEndClient");
            seeder.Seed();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestTimedOut>();
            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
            });

            app.UseRouting();

            app.UseAuthorization(); //Wa¿ne jest miejsce gdzie tego u¿ywamy! 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
