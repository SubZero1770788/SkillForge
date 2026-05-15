using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Database.Repositories;
using quiz_project.Entities;
using quiz_project.Entities.Repositories;
using quiz_project.Infrastructure.Repositories;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;
using quiz_project.ViewModels.Mappers;

namespace quiz_project.Infrastructure
{
    public static class ServiceConfigurer
    {
        public static WebApplicationBuilder Configure(WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllersWithViews(opt =>
            {
                var policy = new AuthorizationPolicyBuilder()
                 .RequireAuthenticatedUser()
                 .Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            });
            builder.Services.AddSignalR();

            // Database connection
            var connection = builder.Configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' was not found");

            // Adding services for identity and database 
            builder.Services.AddDbContext<QuizDb>(o => o.UseSqlite(connection));
            builder.Services.AddIdentity<User, Role>(opt =>
            {
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);

                opt.Password.RequireDigit = true;
                opt.Password.RequiredLength = 12;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;

                opt.User.RequireUniqueEmail = true;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            }).AddEntityFrameworkStores<QuizDb>();


            // Adding session and cookies support
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromMinutes(10);
                o.Cookie.HttpOnly = true;
                o.Cookie.IsEssential = true;
            });


            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IAttemptRepository, AttemptRepository>();
            builder.Services.AddScoped<IAttemptRepository, AttemptRepository>();
            builder.Services.AddScoped<IOnGoingQuizRepository, OnGoingQuizRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IAccessValidationService, AccessValidationService>();
            builder.Services.AddScoped<IPaginationService<Question>, PaginationService<Question>>();
            builder.Services.AddScoped<IQuizGameService, QuizGameService>();
            builder.Services.AddScoped<IQuizQueryService, QuizQueryService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddSingleton<IUserMapper, UserMapper>();
            builder.Services.AddSingleton<IQuizMapper, QuizMapper>();

            return builder;
        }
    }
}