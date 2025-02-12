using Data;
using Logic.Entity;
using Logic.Services;
using Logic.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace ChoiceofaNation_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("postgresSql");
            string connectionString1 = builder.Configuration.GetConnectionString("mySql");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            //builder.Services.AddDbContext<Data.DbContext>(x => x.UseNpgsql(connectionString));
            builder.Services.AddDbContext<Data.DbContext>(x => x.UseSqlServer(connectionString1));

            builder.Services.AddIdentity<User, Roles>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<Data.DbContext>();

            builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        builder.Configuration["JwtSettings:SecretKey"]))
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<JwtService>();
            builder.Services.AddSingleton<RefreshTokenService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy => policy.WithOrigins("http://localhost:3000", "https://choiseoda-nation-frontend.vercel.app/") 
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials()); 
            });

            var app = builder.Build();

            app.UseRouting();

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) // Виводимо помилки навіть у продакшн
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 
            });

            app.UseSwagger();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(options =>
            {
                options.WithOrigins("http://localhost:3000");
            });

            app.MapControllers();

            app.Run();
        }
    }
}