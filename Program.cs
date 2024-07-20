
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Orch_back_API.Entities;
using System.Text;
using System.Text.Json.Serialization;

namespace Orch_back_API
{
    /// <summary>Klasa program, w której znajdziemy Main'a</summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var config = builder.Configuration;
            var secret = config["Jwt:Secret"];

            /// <summary>
            ///     <p>Definicja CORS dla API. Dodajemy tutaj now¹ strategiê, która jest pod mój FrontEnd w Angularze na andresie http://localhost:4200</p>
            ///     <p>Akceptujemy dowoln¹ metodê HTTP przy ¿¹daniach z origina zdefiniowanego wy¿ej pod localhostem</p>
            ///     <p>Akceptujemy dowolny nag³ówek z origina zdefiniowanego wy¿ej pod localhostem</p>
            ///     <p>¯¹dania mog¹ zawieraæ dane uwierzytelniaj¹ce/p>
            /// </summary>
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });

            });

            /// <summary>Tutaj dodajemy autoryzacjê JWT. Poprzez metodê AddAuthentication i AddJwtBearer</summary>
            builder.Services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    /// <summary>Konfiguracja parametrów u¿ywanych do validacji tokenów</summary>
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                };
            });


            /// <summary>
            ///     <p>Dodanie kilku us³ug oraz DBContextu z ConnectionStringiem</p>
            /// </summary>
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });
            builder.Services.AddDbContext<MyJDBContext>(options =>
            {
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Orchard_INZDB;Trusted_Connection=True;");
                options.EnableSensitiveDataLogging();
            });

            /// <summary>Zbudowanie aplikacji</summary>
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            /// <summary>Konfiguracja Middleware'ów oraz Dodanie endpointów dla akcji kontrollerów do IEndpointRouteBuilder bez specyfikowania œcie¿ek</summary>
            /// <seealso cref="https://learn.microsoft.com/pl-pl/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0"/>
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CORSPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}