using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Threading.RateLimiting;

using Aplication.Interfaces;
using Aplication.Services;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using my_books.Data;
using Aplication.Interfaces.IJwt;
using Presentation.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using Aplication.Validators;
using Infrastructure.Email;
using Infrastructure.Events;
using Domain.Events;
using Aplication.EventHandlers;

namespace ReservationSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var allowedOrigins = new[]
            {
                "http://localhost:5173",
                "http://127.0.0.1:5173"
            };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });

            // Add services to the container.

            builder.Services.AddControllers();//.AddFluentValidation();
            builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Enter your JWT Access Token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                
                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            
            builder.Services.AddValidatorsFromAssemblyContaining<ReservationCreateDTOValidator>();

            builder.Services.AddValidatorsFromAssemblyContaining<ReviewCreateDTOValidator>();

            builder.Services.AddValidatorsFromAssemblyContaining<PropertyCreateDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<PropertyUpdateDTOValidator>();

            builder.Services.AddValidatorsFromAssemblyContaining<LockCreateDTOValidator>();


            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")));

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddScoped<IPropertyService, PropertyService>();

            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IJwtRepository, JwtRepository>();

            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

            builder.Services.AddScoped<ILockService, LockService>();
            builder.Services.AddScoped<ILockRepository, LockRepository>();

            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddTransient<INotificationService, NotificationService>();

            builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
            builder.Services.AddScoped<IDomainEventHandler<ReservationCreatedEvent>, ReservationNotificationHandler>();
            builder.Services.AddScoped<IDomainEventHandler<ReservationCanceledEvent>, ReservationNotificationHandler>();
            builder.Services.AddScoped<IDomainEventHandler<ReservationCompletedEvent>, ReservationNotificationHandler>();
            builder.Services.AddScoped<IDomainEventHandler<PropertyDeletedEvent>, ReservationNotificationHandler>();


            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),

                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],

                ValidateAudience = true,
                ValidAudience = builder.Configuration["JWT:Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            builder.Services.AddSingleton(tokenValidationParameters);


            //Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //Add Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //Add JWT Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenValidationParameters;
            });

            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();

            /*builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("per-user", context =>
                {
                    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });*/

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseCors("FrontendPolicy");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseRateLimiter();

            app.MapControllers();
            AppDbInitializer.SeedRoles(app).Wait();

            app.Run();
        }
    }
}
