using ApiBase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBase.Data
{
    public class ApiDbContext : IdentityDbContext<IdentityUser>
    {
        //DbSets here!
        public virtual DbSet<Todo> Todos { get; set; }
        public virtual DbSet<Apple> Apples { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<IdentityRole>().HasData(
                new { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new { Id = "2", Name = "Customer", NormalizedName = "CUSTOMER" }
            );


        }
    }
}
