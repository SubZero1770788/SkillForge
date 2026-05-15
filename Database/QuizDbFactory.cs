using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace quiz_project.Database
{
    public class QuizDbFactory : IDesignTimeDbContextFactory<QuizDb>
    {
        // Used so dotnet-ef tool knows how to generate QuizDb when it derives from IdentityDbContext<User, Role, int>
        public QuizDb CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<QuizDb>();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var con = config.GetConnectionString("DefaultConnection");
            options.UseSqlite(con);

            return new QuizDb(options.Options);
        }
    }

}