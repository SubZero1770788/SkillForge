using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace quiz_project.Entities
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role()
                {
                    Id = 1,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new Role()
                {
                    Id = 2,
                    Name = "Moderator",
                    NormalizedName = "MODERATOR"
                },
                new Role()
                {
                    Id = 3,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new Role()
                {
                    Id = 4,
                    Name = "Creator",
                    NormalizedName = "CREATOR"
                }
            );
        }
    }
}